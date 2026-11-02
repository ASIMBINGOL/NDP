#nullable enable
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// DTO: Form, EF Core varlığını doğrudan değiştiremez (kapsülleme).
public sealed class UygunlukBilgisi
{
    public int Id { get; init; }
    public int KurumId { get; init; }
    public string KurumAdi { get; init; } = "";
    public int CalisanId { get; init; }
    public string DoktorAdi { get; init; } = "";
    public DateTime BaslangicTarihiSaati { get; init; }
    public DateTime BitisTarihiSaati { get; init; }
    public int RandevuSayisi { get; init; }
}

// Kalıtım + çok biçimlilik: YetkiliYonetimServisi<T>'nin metotlarını özelleştirir.
public sealed class UygunlukYonetimServisi : YetkiliYonetimServisi<UygunlukBilgisi>
{
    public UygunlukYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public override async Task<IReadOnlyList<UygunlukBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        return await Baglam.UygunlukAraliklari.AsNoTracking()
            .OrderByDescending(a => a.BaslangicTarihiSaati)
            .Select(a => new UygunlukBilgisi
            {
                Id = a.Id,
                KurumId = a.KurumId,
                KurumAdi = a.Calisan!.Kurum!.Ad,
                CalisanId = a.CalisanId,
                DoktorAdi = a.Calisan!.Kullanici!.AdSoyad,
                BaslangicTarihiSaati = a.BaslangicTarihiSaati,
                BitisTarihiSaati = a.BitisTarihiSaati,
                RandevuSayisi = Baglam.Randevular.Count(r => r.UygunlukAraligiId == a.Id)
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(UygunlukBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        DateTime baslangic = kayit.BaslangicTarihiSaati;
        DateTime bitis = kayit.BitisTarihiSaati;

        if (baslangic <= DateTime.Now)
            throw new InvalidOperationException("Doktor uygunluğu gelecekte başlamalıdır.");
        if (bitis <= baslangic || baslangic.Date != bitis.Date)
            throw new InvalidOperationException("Bitiş saati başlangıçtan sonra olmalı ve aynı takvim gününde kalmalıdır.");
        if (kayit.CalisanId <= 0 || kayit.KurumId <= 0)
            throw new InvalidOperationException("Kurum ve doktor seçiniz.");

        bool aktifDoktor = await Baglam.Doktorlar.AsNoTracking().AnyAsync(d =>
            d.Id == kayit.CalisanId && d.KurumId == kayit.KurumId && d.AktifMi &&
            d.Kurum!.AktifMi && d.Kullanici!.AktifMi && d.Kullanici.Rol == "Doktor");
        if (!aktifDoktor)
            throw new InvalidOperationException("Seçilen kurumda aktif bir doktor bulunamadı.");

        // Kurumun haftalık mesaisi ile tam uyumlu olmalıdır.
        var mesai = await Baglam.KurumCalismaSaatleri.AsNoTracking().FirstOrDefaultAsync(s =>
            s.KurumId == kayit.KurumId && s.HaftaninGunu == (int)baslangic.DayOfWeek);
        if (mesai is null || mesai.KapaliMi ||
            !TimeOnly.TryParseExact(mesai.AcilisSaati, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var acilis) ||
            !TimeOnly.TryParseExact(mesai.KapanisSaati, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var kapanis))
            throw new InvalidOperationException("Bu tarihte kurumun geçerli açık çalışma saati tanımı bulunmuyor.");
        if (TimeOnly.FromDateTime(baslangic) < acilis || TimeOnly.FromDateTime(bitis) > kapanis)
            throw new InvalidOperationException(
                $"Uygunluk kurumun {acilis:HH:mm}–{kapanis:HH:mm} çalışma saatleri içinde olmalıdır.");

        UygunlukAraligi? varlik = null;
        if (kayit.Id > 0)
        {
            varlik = await Baglam.UygunlukAraliklari
                .FirstOrDefaultAsync(a => a.Id == kayit.Id)
                ?? throw new InvalidOperationException("Güncellenecek uygunluk kaydı bulunamadı.");
            if (varlik.CalisanId != kayit.CalisanId || varlik.KurumId != kayit.KurumId)
                throw new InvalidOperationException("Kayıtlı uygunluğun doktoru veya kurumu değiştirilemez. Yeni kayıt açınız.");
            // Geçmiş (iptal edilmiş dâhil) randevu da audit kaydıdır; bağlantısı bozulmamalı.
            if (await Baglam.Randevular.AnyAsync(r => r.UygunlukAraligiId == kayit.Id))
                throw new InvalidOperationException("Bu uygunluğa randevu bağlanmış. Geçmişi korumak için değiştirilemez; yeni uygunluk ekleyiniz.");
        }

        // Aynı doktora ait aralıklar tam veya kısmen çakışamaz. Bitişik aralıklar kabul edilir.
        var digerAraliklar = await Baglam.UygunlukAraliklari.AsNoTracking()
            .Where(a => a.CalisanId == kayit.CalisanId && a.Id != kayit.Id)
            .Select(a => new { a.Id, a.BaslangicTarihiSaati, a.BitisTarihiSaati })
            .ToListAsync();
        var cakisabilen = digerAraliklar.FirstOrDefault(a =>
            a.BaslangicTarihiSaati < bitis && baslangic < a.BitisTarihiSaati);
        if (cakisabilen is not null)
            throw new InvalidOperationException(
                $"Bu doktorun #{cakisabilen.Id} numaralı uygunluk aralığıyla saatler çakışıyor.");

        if (varlik is null)
        {
            varlik = new UygunlukAraligi { CalisanId = kayit.CalisanId, KurumId = kayit.KurumId };
            Baglam.UygunlukAraliklari.Add(varlik);
        }
        varlik.BaslangicTarihiSaati = baslangic;
        varlik.BitisTarihiSaati = bitis;
        await Baglam.SaveChangesAsync();
        return varlik.Id;
    }

    public override async Task SilAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var varlik = await Baglam.UygunlukAraliklari
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new InvalidOperationException("Silinecek doktor uygunluğu bulunamadı.");
        if (await Baglam.Randevular.AnyAsync(r => r.UygunlukAraligiId == id))
            throw new InvalidOperationException(
                "Bu uygunluğa bağlı randevu kayıtları var. İptal edilmiş randevular da dâhil, geçmişi korumak için kalıcı silinemez.");
        Baglam.UygunlukAraliklari.Remove(varlik);
        await Baglam.SaveChangesAsync();
    }
}
