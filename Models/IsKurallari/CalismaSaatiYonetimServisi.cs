#nullable enable
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Arayüz ve soyut sınıf: IYonetimIslemleri<T> / YetkiliYonetimServisi<T>.
// Bu DTO, veritabanı modelini doğrudan formun değiştirmesini engeller (kapsülleme).
public sealed class CalismaSaatiBilgisi
{
    public int Id { get; init; }
    public int KurumId { get; init; }
    public string KurumAdi { get; init; } = "";
    public int HaftaninGunu { get; init; }
    public string AcilisSaati { get; init; } = "";
    public string KapanisSaati { get; init; } = "";
    public bool KapaliMi { get; init; }
}

// Kalıtım ve çok biçimlilik: ortak CRUD sözleşmesinin özelleştirilmesi.
public sealed class CalismaSaatiYonetimServisi : YetkiliYonetimServisi<CalismaSaatiBilgisi>
{
    private static readonly string[] GunAdlari =
    {
        "Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"
    };

    public CalismaSaatiYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public static string GunAdi(int gun) => gun is >= 0 and <= 6 ? GunAdlari[gun] : "Bilinmiyor";

    public override async Task<IReadOnlyList<CalismaSaatiBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        return await Baglam.KurumCalismaSaatleri.AsNoTracking()
            .OrderBy(s => s.Kurum!.Ad)
            .ThenBy(s => s.HaftaninGunu)
            .Select(s => new CalismaSaatiBilgisi
            {
                Id = s.Id,
                KurumId = s.KurumId,
                KurumAdi = s.Kurum!.Ad,
                HaftaninGunu = s.HaftaninGunu,
                AcilisSaati = s.AcilisSaati ?? "",
                KapanisSaati = s.KapanisSaati ?? "",
                KapaliMi = s.KapaliMi
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(CalismaSaatiBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        if (kayit.HaftaninGunu is < 0 or > 6)
            throw new InvalidOperationException("Geçerli bir hafta günü seçiniz.");

        bool kurumVar = await Baglam.Kurumlar.AnyAsync(k => k.Id == kayit.KurumId && k.AktifMi);
        if (!kurumVar)
            throw new InvalidOperationException("Çalışma saati tanımlamak için aktif bir kurum seçiniz.");

        string acilis = "";
        string kapanis = "";
        if (!kayit.KapaliMi)
        {
            if (!TimeOnly.TryParseExact((kayit.AcilisSaati ?? "").Trim(), "HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var a) ||
                !TimeOnly.TryParseExact((kayit.KapanisSaati ?? "").Trim(), "HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var k))
                throw new InvalidOperationException("Açık günlerde saatleri 09:00 gibi HH:mm biçiminde yazınız.");
            if (a >= k)
                throw new InvalidOperationException("Açılış saati kapanış saatinden önce olmalıdır.");
            acilis = a.ToString("HH:mm", CultureInfo.InvariantCulture);
            kapanis = k.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        KurumCalismaSaati mesai;
        if (kayit.Id == 0)
        {
            bool ayniGun = await Baglam.KurumCalismaSaatleri.AnyAsync(s =>
                s.KurumId == kayit.KurumId && s.HaftaninGunu == kayit.HaftaninGunu);
            if (ayniGun)
                throw new InvalidOperationException("Bu kurumun seçilen günü için zaten mesai kaydı var. Güncelleme yapınız.");
            mesai = new KurumCalismaSaati
            {
                KurumId = kayit.KurumId,
                HaftaninGunu = kayit.HaftaninGunu
            };
            Baglam.KurumCalismaSaatleri.Add(mesai);
        }
        else
        {
            mesai = await Baglam.KurumCalismaSaatleri
                .FirstOrDefaultAsync(s => s.Id == kayit.Id)
                ?? throw new InvalidOperationException("Güncellenecek çalışma saati bulunamadı.");
            // Gün/kurum değiştirmek, mevcut haftalık programı ve randevuları karıştırabilir.
            if (mesai.KurumId != kayit.KurumId || mesai.HaftaninGunu != kayit.HaftaninGunu)
                throw new InvalidOperationException("Mevcut kaydın kurumu veya günü değiştirilemez. Ayrı bir gün kaydı oluşturun.");
        }

        // Kural: bekleyen/onaylanan gelecekteki randevular çalışma saati dışında bırakılamaz.
        await GelecekRandevulariDogrulaAsync(kayit.KurumId, kayit.HaftaninGunu,
            kayit.KapaliMi, acilis, kapanis);

        mesai.KapaliMi = kayit.KapaliMi;
        mesai.AcilisSaati = kayit.KapaliMi ? null : acilis;
        mesai.KapanisSaati = kayit.KapaliMi ? null : kapanis;
        await Baglam.SaveChangesAsync();
        return mesai.Id;
    }

    public override async Task SilAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var mesai = await Baglam.KurumCalismaSaatleri
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new InvalidOperationException("Silinecek çalışma saati bulunamadı.");
        // Haftanın bu gününde aktif gelecek randevu varsa mesaiyi silmek onları geçersiz bırakır.
        await GelecekRandevulariDogrulaAsync(mesai.KurumId, mesai.HaftaninGunu,
            kapaliMi: true, acilis: "", kapanis: "");
        Baglam.KurumCalismaSaatleri.Remove(mesai);
        await Baglam.SaveChangesAsync();
    }

    private async Task GelecekRandevulariDogrulaAsync(int kurumId, int gun,
        bool kapaliMi, string acilis, string kapanis)
    {
        DateTime simdi = DateTime.Now;
        // SQLite DateTime.DayOfWeek çevirisini garanti etmediğimiz için gün süzmesini bellekte yapıyoruz.
        var randevular = await Baglam.Randevular.AsNoTracking()
            .Where(r => r.KurumId == kurumId && r.BaslangicTarihiSaati > simdi &&
                        (r.Durum == "Beklemede" || r.Durum == "Onaylandi"))
            .Select(r => new { r.Id, r.BaslangicTarihiSaati, r.BitisTarihiSaati })
            .ToListAsync();

        TimeSpan acilisSaat = kapaliMi ? TimeSpan.Zero : TimeSpan.ParseExact(acilis, @"hh\:mm", CultureInfo.InvariantCulture);
        TimeSpan kapanisSaat = kapaliMi ? TimeSpan.Zero : TimeSpan.ParseExact(kapanis, @"hh\:mm", CultureInfo.InvariantCulture);
        var ilgili = randevular.Where(r => (int)r.BaslangicTarihiSaati.DayOfWeek == gun);
        foreach (var randevu in ilgili)
        {
            bool disarida = kapaliMi ||
                randevu.BaslangicTarihiSaati.Date != randevu.BitisTarihiSaati.Date ||
                randevu.BaslangicTarihiSaati.TimeOfDay < acilisSaat ||
                randevu.BitisTarihiSaati.TimeOfDay > kapanisSaat;
            if (disarida)
                throw new InvalidOperationException(
                    $"{GunAdi(gun)} günü için #{randevu.Id} numaralı gelecek randevu bu çalışma saatiyle çakışıyor. " +
                    "Önce randevuyu iptal edin veya çalışma saatlerini koruyun.");
        }
    }
}
