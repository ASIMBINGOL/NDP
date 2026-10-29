
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Bu sinif randevu olusturma ve onaylama
// islemlerindeki is kurallarini yonetir.

public class RandevuYonetimi
{
    private readonly VeritabaniBaglami _baglam;

    // Yapici metot
    public RandevuYonetimi(VeritabaniBaglami baglam)
    {
        _baglam = baglam;
    }

    // ============================================
    // 1. RANDEVU OLUSTURMA
    // ============================================

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

        // Kurumun aktif olup olmadigini kontrol et.
        bool kurumAktifMi = await _baglam.Kurumlar
            .AnyAsync(
                k => k.Id == uygunluk.KurumId
                     && k.AktifMi,
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

        // Calisanin bu hizmeti sunup sunmadigini kontrol et.
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

        // Hizmetin suresine gore randevu bitisini hesapla.
        DateTime bitis = baslangic.AddMinutes(
            hizmet.SureDakika);

        // Randevu calisanin uygunluk araliginda olmali.
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

        // Kurumun o gun acik olup olmadigini kontrol et.
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

        // Kurumun acilis saatini kontrol et.
        bool acilisGecerli = TimeOnly.TryParseExact(
            mesai.AcilisSaati,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var acilis);

        // Kurumun kapanis saatini kontrol et.
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

        // Randevu kurumun calisma saatleri icinde olmali.
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

        // Tum kontroller basariliysa randevuyu olustur.
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

    // ============================================
    // 2. RANDEVU ONAYLAMA
    // ============================================

    public async Task RandevuOnaylaAsync(
        int randevuId,
        int onaylayanKullaniciId)
    {
        // Onaylanacak randevuyu bul.
        var randevu = await _baglam.Randevular
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == randevuId);

        if (randevu is null)
        {
            throw new InvalidOperationException(
                "Randevu bulunamadi.");
        }

        // Gecmis tarihli randevular onaylanamaz.
        if (randevu.BaslangicTarihiSaati <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "Gecmis tarihli randevular onaylanamaz.");
        }

        // Onaylayan kullanici doktor ve aktif olmali.
        bool doktorYetkiliMi = await _baglam.Kullanicilar
            .AnyAsync(k =>
                k.Id == onaylayanKullaniciId
                && k.Rol == "Doktor"
                && k.AktifMi);

        if (!doktorYetkiliMi)
        {
            throw new InvalidOperationException(
                "Bu kullanicinin randevu onaylama yetkisi yok.");
        }

        // Doktor sadece kendisine ait randevuyu onaylayabilir.
        bool randevuDoktoraAitMi = await _baglam.Doktorlar
            .AnyAsync(d =>
                d.Id == randevu.CalisanId
                && d.KurumId == randevu.KurumId
                && d.KullaniciId == onaylayanKullaniciId
                && d.AktifMi);

        if (!randevuDoktoraAitMi)
        {
            throw new InvalidOperationException(
                "Bu randevu ilgili doktora ait degil.");
        }

        // Sadece bekleyen randevular onaylanabilir.
        DateTime simdi = DateTime.Now;

        int degisenKayitSayisi = await _baglam.Randevular
            .Where(r =>
                r.Id == randevuId &&
                r.Durum == "Beklemede" &&
                r.BaslangicTarihiSaati > simdi)
            .ExecuteUpdateAsync(ayarlar =>
                ayarlar.SetProperty(
                    r => r.Durum,
                    "Onaylandi"));

        if (degisenKayitSayisi != 1)
        {
            throw new InvalidOperationException(
                "Randevu zaten islenmis veya beklemede degil.");
        }
    }

    
    // ============================================
    // 3. RANDEVU IPTAL ETME
    // ============================================

    public async Task RandevuIptalEtAsync(
        int randevuId,
        int islemYapanKullaniciId)
    {
        // Iptal edilecek randevuyu bul.
        var randevu = await _baglam.Randevular
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == randevuId);

        if (randevu is null)
        {
            throw new InvalidOperationException(
                "Randevu bulunamadi.");
        }

        // Islemi yapan kullaniciyi kontrol et.
        var kullanici = await _baglam.Kullanicilar
            .AsNoTracking()
            .FirstOrDefaultAsync(k =>
                k.Id == islemYapanKullaniciId &&
                k.AktifMi);

        if (kullanici is null)
        {
            throw new InvalidOperationException(
                "Aktif kullanici bulunamadi.");
        }

        // Hasta sadece kendi randevusunu iptal edebilir.
        bool hastaYetkiliMi =
            kullanici.Rol == "Hasta" &&
            await _baglam.Hastalar.AnyAsync(h =>
                h.Id == randevu.HastaId &&
                h.KullaniciId == islemYapanKullaniciId);

        // Doktor sadece kendisine ait randevuyu iptal edebilir.
        bool doktorYetkiliMi =
            kullanici.Rol == "Doktor" &&
            await _baglam.Doktorlar.AnyAsync(d =>
                d.Id == randevu.CalisanId &&
                d.KurumId == randevu.KurumId &&
                d.KullaniciId == islemYapanKullaniciId &&
                d.AktifMi);

        if (!hastaYetkiliMi && !doktorYetkiliMi)
        {
            throw new InvalidOperationException(
                "Bu randevuyu iptal etme yetkiniz yok.");
        }

        // Baslamis veya gecmis randevular iptal edilemez.
        DateTime simdi = DateTime.Now;

        if (randevu.BaslangicTarihiSaati <= simdi)
        {
            throw new InvalidOperationException(
                "Baslamis veya gecmis randevular iptal edilemez.");
        }

        // Yalnizca bekleyen veya onaylanan randevular
        // iptal edilebilir.
        int degisenKayitSayisi = await _baglam.Randevular
            .Where(r =>
                r.Id == randevuId &&
                (r.Durum == "Beklemede" ||
                 r.Durum == "Onaylandi") &&
                r.BaslangicTarihiSaati > simdi)
            .ExecuteUpdateAsync(ayarlar =>
                ayarlar.SetProperty(
                    r => r.Durum,
                    "IptalEdildi"));

        if (degisenKayitSayisi != 1)
        {
            throw new InvalidOperationException(
                "Randevu iptal edilemedi veya zaten islenmis.");
        }
    }

        
    // ============================================
    // 4. RANDEVU TAMAMLAMA
    // ============================================

    public async Task RandevuTamamlaAsync(
        int randevuId,
        int islemYapanKullaniciId)
    {
        // Tamamlanacak randevuyu bul.
        var randevu = await _baglam.Randevular
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == randevuId);

        if (randevu is null)
        {
            throw new InvalidOperationException(
                "Randevu bulunamadi.");
        }

        // Sadece ilgili doktor randevuyu tamamlayabilir.
        bool doktorYetkiliMi = await _baglam.Doktorlar
            .AnyAsync(d =>
                d.Id == randevu.CalisanId &&
                d.KurumId == randevu.KurumId &&
                d.KullaniciId == islemYapanKullaniciId &&
                d.AktifMi &&
                d.Kullanici != null &&
                d.Kullanici.AktifMi &&
                d.Kullanici.Rol == "Doktor");

        if (!doktorYetkiliMi)
        {
            throw new InvalidOperationException(
                "Randevuyu tamamlama yetkiniz yok.");
        }

        // Randevu bitis zamani gelmis olmali.
        DateTime simdi = DateTime.Now;

        if (randevu.BitisTarihiSaati > simdi)
        {
            throw new InvalidOperationException(
                "Randevu henuz sona ermedi.");
        }

        // Sadece onaylanmis randevular tamamlanabilir.
        int degisenKayitSayisi = await _baglam.Randevular
            .Where(r =>
                r.Id == randevuId &&
                r.Durum == "Onaylandi" &&
                r.BitisTarihiSaati <= simdi)
            .ExecuteUpdateAsync(ayarlar =>
                ayarlar.SetProperty(
                    r => r.Durum,
                    "Tamamlandi"));

        if (degisenKayitSayisi != 1)
        {
            throw new InvalidOperationException(
                "Randevu onaylanmamis veya zaten islenmis.");
        }
    }

}
