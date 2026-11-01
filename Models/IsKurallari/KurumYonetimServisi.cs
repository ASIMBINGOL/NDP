using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

public sealed class KurumBilgisi
{
    public int Id { get; init; }
    public string Ad { get; init; } = "";
    public string KurumTuru { get; init; } = "";
    public string Adres { get; init; } = "";
    public string Telefon { get; init; } = "";
    public int? UstKurumId { get; init; }
    public bool AktifMi { get; init; }
}

// Kurum işlemlerinin tamamı yetki, iş kuralı ve veri erişimini kapsüller.
public sealed class KurumYonetimServisi : YetkiliYonetimServisi<KurumBilgisi>
{
    public KurumYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public override async Task<IReadOnlyList<KurumBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        return await Baglam.Kurumlar.AsNoTracking().OrderBy(k => k.Ad)
            .Select(k => new KurumBilgisi
            {
                Id = k.Id, Ad = k.Ad, KurumTuru = k.KurumTuru,
                Adres = k.Adres ?? "", Telefon = k.Telefon ?? "",
                UstKurumId = k.UstKurumId, AktifMi = k.AktifMi
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(KurumBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        string ad = kayit.Ad.Trim();
        string tur = kayit.KurumTuru.Trim();
        string adres = kayit.Adres.Trim();
        string telefon = kayit.Telefon.Trim();
        if (ad.Length is < 2 or > 150)
            throw new InvalidOperationException("Kurum adı 2 ile 150 karakter arasında olmalı.");
        if (tur.Length is < 2 or > 20)
            throw new InvalidOperationException("Kurum türü 2 ile 20 karakter arasında olmalı.");
        if (adres.Length > 300 || telefon.Length > 30)
            throw new InvalidOperationException("Adres veya telefon alanı çok uzun.");

        if (kayit.UstKurumId is int ustId)
        {
            if (ustId == kayit.Id)
                throw new InvalidOperationException("Bir kurum kendi üst kurumu olamaz.");
            if (!await Baglam.Kurumlar.AnyAsync(k => k.Id == ustId))
                throw new InvalidOperationException("Seçilen üst kurum bulunamadı.");

            // Hiyerarşide çevrim oluşmasını engelle.
            int? konum = ustId;
            var gorulen = new HashSet<int>();
            while (konum is int id)
            {
                if (id == kayit.Id && kayit.Id != 0)
                    throw new InvalidOperationException("Üst kurum seçimi döngü oluşturamaz.");
                if (!gorulen.Add(id))
                    throw new InvalidOperationException("Kurum hiyerarşisinde döngü tespit edildi.");
                konum = await Baglam.Kurumlar.AsNoTracking()
                    .Where(k => k.Id == id).Select(k => k.UstKurumId)
                    .FirstOrDefaultAsync();
            }
        }

        Kurum kurum;
        if (kayit.Id == 0)
        {
            kurum = new Kurum { AktifMi = true };
            Baglam.Kurumlar.Add(kurum);
        }
        else
        {
            kurum = await Baglam.Kurumlar.FirstOrDefaultAsync(k => k.Id == kayit.Id)
                ?? throw new InvalidOperationException("Güncellenecek kurum bulunamadı.");
            // Durum değişiklikleri yalnızca DurumDegistirAsync ile yapılır.
        }
        kurum.Ad = ad;
        kurum.KurumTuru = tur;
        kurum.Adres = adres;
        kurum.Telefon = telefon;
        kurum.UstKurumId = kayit.UstKurumId;
        await Baglam.SaveChangesAsync();
        return kurum.Id;
    }

    public async Task DurumDegistirAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var kurum = await Baglam.Kurumlar.FirstOrDefaultAsync(k => k.Id == id)
            ?? throw new InvalidOperationException("Kurum bulunamadı.");
        if (kurum.AktifMi)
        {
            bool aktifAltKurum = await Baglam.Kurumlar
                .AnyAsync(k => k.UstKurumId == id && k.AktifMi);
            bool gelecekRandevu = await Baglam.Randevular.AnyAsync(r =>
                r.KurumId == id && r.BaslangicTarihiSaati > DateTime.Now &&
                (r.Durum == "Beklemede" || r.Durum == "Onaylandi"));
            if (aktifAltKurum || gelecekRandevu)
                throw new InvalidOperationException(
                    "Aktif alt kurum veya gelecek randevu varken kurum pasifleştirilemez.");
        }
        kurum.AktifMi = !kurum.AktifMi;
        await Baglam.SaveChangesAsync();
    }

    public override async Task SilAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var kurum = await Baglam.Kurumlar.FirstOrDefaultAsync(k => k.Id == id)
            ?? throw new InvalidOperationException("Silinecek kurum bulunamadı.");
        bool bagimliKayit =
            await Baglam.Kurumlar.AnyAsync(k => k.UstKurumId == id) ||
            await Baglam.Bolumler.AnyAsync(b => b.KurumId == id) ||
            await Baglam.Calisanlar.AnyAsync(c => c.KurumId == id) ||
            await Baglam.SaglikHizmetleri.AnyAsync(h => h.KurumId == id) ||
            await Baglam.KurumCalismaSaatleri.AnyAsync(s => s.KurumId == id) ||
            await Baglam.UygunlukAraliklari.AnyAsync(u => u.KurumId == id) ||
            await Baglam.Randevular.AnyAsync(r => r.KurumId == id);
        if (bagimliKayit)
            throw new InvalidOperationException(
                "İlişkili kayıtları olan kurum kalıcı silinemez. Pasifleştirmeyi kullanın.");
        Baglam.Kurumlar.Remove(kurum);
        await Baglam.SaveChangesAsync();
    }
}
