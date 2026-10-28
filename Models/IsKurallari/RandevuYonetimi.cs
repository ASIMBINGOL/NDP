
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

//Hasta, çalışan, kurum ve hizmet kayıtlarının geçerli olması gerekiyor.
//Çalışan seçilen sağlık hizmetini sunabiliyor olmalı.
//Randevu hem kurumun hem çalışanın uygun saat aralıklarında kalmalı.
//Hastanın veya çalışanın aynı saatlerde başka bir bekleyen ya da onaylanmış randevusu olmamalı.
//Kontroller başarılıysa randevu Beklemede durumuyla veritabanına kaydedilmeli.

public class RandevuYonetimi
{
    private readonly VeritabaniBaglami _baglam;

    public RandevuYonetimi(VeritabaniBaglami baglam)
    {
        _baglam = baglam;
    }

    public async Task<int> RandevuOlusturAsync(
        int hastaId,
        int calisanId,
        int saglikHizmetiId,
        int uygunlukAraligiId,
        DateTime baslangic,
        CancellationToken iptalBelirteci = default)
    {
        // Randevu tarihi gelecekte olmali.
        if (baslangic <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "Randevu tarihi gelecekte olmalidir.");
        }

        // Hasta kaydini kontrol et.
        bool hastaVarMi = await _baglam.Hastalar
            .AnyAsync(
                h => h.Id == hastaId,
                iptalBelirteci);

        if (!hastaVarMi)
        {
            throw new InvalidOperationException(
                "Hasta kaydi bulunamadi.");
        }

        // Calisanin uygunluk araligini bul.
        var uygunluk = await _baglam.UygunlukAraliklari
            .SingleOrDefaultAsync(
                a => a.Id == uygunlukAraligiId
                     && a.CalisanId == calisanId,
                iptalBelirteci);

        if (uygunluk is null)
        {
            throw new InvalidOperationException(
                "Calisanin uygunluk araligi bulunamadi.");
        }

        // Calisani ve kurumunu kontrol et.
        var calisan = await _baglam.Calisanlar
            .SingleOrDefaultAsync(
                c => c.Id == calisanId
                     && c.KurumId == uygunluk.KurumId,
                iptalBelirteci);

        if (calisan is null || !calisan.AktifMi)
        {
            throw new InvalidOperationException(
                "Calisan bulunamadi veya aktif degil.");
        }

        bool kurumAktifMi = await _baglam.Kurumlar
            .AnyAsync(
                k => k.Id == uygunluk.KurumId && k.AktifMi,
                iptalBelirteci);

        if (!kurumAktifMi)
        {
            throw new InvalidOperationException(
                "Kurum bulunamadi veya aktif degil.");
        }

        // Saglik hizmetini kontrol et.
        var hizmet = await _baglam.SaglikHizmetleri
            .SingleOrDefaultAsync(
                h => h.Id == saglikHizmetiId
                     && h.KurumId == uygunluk.KurumId
                     && h.AktifMi,
                iptalBelirteci);

        if (hizmet is null)
        {
            throw new InvalidOperationException(
                "Saglik hizmeti bulunamadi veya aktif degil.");
        }

        // Calisan bu hizmeti sunabiliyor mu?
        bool hizmetSunuluyorMu =
            await _baglam.CalisanHizmetleri.AnyAsync(
                x => x.CalisanId == calisanId
                     && x.SaglikHizmetiId == saglikHizmetiId
                     && x.KurumId == uygunluk.KurumId,
                iptalBelirteci);

        if (!hizmetSunuluyorMu)
        {
            throw new InvalidOperationException(
                "Bu calisan secilen hizmeti sunamiyor.");
        }

        DateTime bitis = baslangic.AddMinutes(
            hizmet.SureDakika);

        // Randevu uygunluk araliginda kalmali.
        if (baslangic < uygunluk.BaslangicTarihiSaati
            || bitis > uygunluk.BitisTarihiSaati)
        {
            throw new InvalidOperationException(
                "Randevu calisanin uygunluk saatleri disinda.");
        }

        // Randevu ayni gun icinde tamamlanmali.
        if (baslangic.Date != bitis.Date)
        {
            throw new InvalidOperationException(
                "Randevu iki farkli gune yayilamaz.");
        }

        // Kurumun o gun acik oldugunu kontrol et.
        int gun = (int)baslangic.DayOfWeek;

        var mesai = await _baglam.KurumCalismaSaatleri
            .SingleOrDefaultAsync(
                s => s.KurumId == uygunluk.KurumId
                     && s.HaftaninGunu == gun,
                iptalBelirteci);

        if (mesai is null || mesai.KapaliMi)
        {
            throw new InvalidOperationException(
                "Kurum bu gun icin acik degil.");
        }

        bool acilisGecerli = TimeOnly.TryParseExact(
            mesai.AcilisSaati,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var acilis);

        bool kapanisGecerli = TimeOnly.TryParseExact(
            mesai.KapanisSaati,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var kapanis);

        if (!acilisGecerli || !kapanisGecerli)
        {
            throw new InvalidOperationException(
                "Kurumun calisma saatleri hatali.");
        }

        TimeOnly randevuBaslangici =
            TimeOnly.FromDateTime(baslangic);

        TimeOnly randevuBitisi =
            TimeOnly.FromDateTime(bitis);

        if (randevuBaslangici < acilis
            || randevuBitisi > kapanis)
        {
            throw new InvalidOperationException(
                "Randevu kurumun calisma saatleri disinda.");
        }

        // Calisanin cakisan randevularini kontrol et.
        bool calisanCakisiyorMu =
            await _baglam.Randevular.AnyAsync(
                r => r.CalisanId == calisanId
                     && (r.Durum == "Beklemede"
                         || r.Durum == "Onaylandi")
                     && r.BaslangicTarihiSaati < bitis
                     && baslangic < r.BitisTarihiSaati,
                iptalBelirteci);

        if (calisanCakisiyorMu)
        {
            throw new InvalidOperationException(
                "Calisanin bu saatte baska bir randevusu var.");
        }

        // Hastanin cakisan randevularini kontrol et.
        bool hastaCakisiyorMu =
            await _baglam.Randevular.AnyAsync(
                r => r.HastaId == hastaId
                     && (r.Durum == "Beklemede"
                         || r.Durum == "Onaylandi")
                     && r.BaslangicTarihiSaati < bitis
                     && baslangic < r.BitisTarihiSaati,
                iptalBelirteci);

        if (hastaCakisiyorMu)
        {
            throw new InvalidOperationException(
                "Hastanin bu saatte baska bir randevusu var.");
        }

        // Randevuyu onay bekliyor durumunda kaydet.
        var yeniRandevu = new Randevu
        {
            HastaId = hastaId,
            CalisanId = calisanId,
            SaglikHizmetiId = saglikHizmetiId,
            KurumId = uygunluk.KurumId,
            UygunlukAraligiId = uygunluk.Id,
            BaslangicTarihiSaati = baslangic,
            BitisTarihiSaati = bitis,
            SureDakikaKayit = hizmet.SureDakika,
            UcretKurusKayit = hizmet.UcretKurus,
            Durum = "Beklemede"
        };

        _baglam.Randevular.Add(yeniRandevu);

        await _baglam.SaveChangesAsync(iptalBelirteci);

        return yeniRandevu.Id;
    }
}
