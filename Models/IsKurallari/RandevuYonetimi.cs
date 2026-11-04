
using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Randevu oluşturma, onaylama, iptal etme
// ve tamamlama iş kurallarını yöneten sınıf.
public class RandevuYonetimi
{
    private readonly VeritabaniBaglami _baglam;

    public RandevuYonetimi(VeritabaniBaglami baglam)
    {
        _baglam = baglam;
    }

    // ============================================
    // 1. RANDEVU OLUŞTURMA
    // ============================================

    public async Task<int> RandevuOlusturAsync(
        int hastaId,
        int calisanId,
        int saglikHizmetiId,
        int uygunlukAraligiId,
        DateTime baslangic,
        CancellationToken iptalBelirteci = default)
    {
        // Randevu gelecekte olmalı.
        if (baslangic <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "Randevu tarihi gelecekte olmalıdır.");
        }

        // EŞZAMANLI RANDEVU GÜVENLİĞİ
        // SQLite üzerinde yazma işlemlerinin
        // birbirini beklemesini sağlayan transaction.
        // Tüm kontroller ve kayıt aynı işlemde yapılır.
        await using var islem =
            await _baglam.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                iptalBelirteci);

        // Hasta kaydını kontrol et.
        bool hastaVarMi = await _baglam.Hastalar
            .AnyAsync(
                h => h.Id == hastaId,
                iptalBelirteci);

        if (!hastaVarMi)
        {
            throw new InvalidOperationException(
                "Hasta kaydı bulunamadı.");
        }

        // Çalışanın uygunluk aralığını bul.
        var uygunluk = await _baglam.UygunlukAraliklari
            .SingleOrDefaultAsync(
                a => a.Id == uygunlukAraligiId
                     && a.CalisanId == calisanId,
                iptalBelirteci);

        if (uygunluk is null)
        {
            throw new InvalidOperationException(
                "Çalışanın uygunluk aralığı bulunamadı.");
        }

        // Çalışanı ve kurumunu kontrol et.
        var calisan = await _baglam.Calisanlar
            .SingleOrDefaultAsync(
                c => c.Id == calisanId
                     && c.KurumId == uygunluk.KurumId,
                iptalBelirteci);

        if (calisan is null || !calisan.AktifMi)
        {
            throw new InvalidOperationException(
                "Çalışan bulunamadı veya aktif değil.");
        }

        // Kurum aktif olmalı.
        bool kurumAktifMi = await _baglam.Kurumlar
            .AnyAsync(
                k => k.Id == uygunluk.KurumId
                     && k.AktifMi,
                iptalBelirteci);

        if (!kurumAktifMi)
        {
            throw new InvalidOperationException(
                "Kurum bulunamadı veya aktif değil.");
        }

        // Sağlık hizmetini kontrol et.
        var hizmet = await _baglam.SaglikHizmetleri
            .SingleOrDefaultAsync(
                h => h.Id == saglikHizmetiId
                     && h.KurumId == uygunluk.KurumId
                     && h.AktifMi,
                iptalBelirteci);

        if (hizmet is null)
        {
            throw new InvalidOperationException(
                "Sağlık hizmeti bulunamadı veya aktif değil.");
        }

        // Çalışan bu hizmeti sunabiliyor mu?
        bool hizmetSunuluyorMu =
            await _baglam.CalisanHizmetleri.AnyAsync(
                x => x.CalisanId == calisanId
                     && x.SaglikHizmetiId == saglikHizmetiId
                     && x.KurumId == uygunluk.KurumId,
                iptalBelirteci);

        if (!hizmetSunuluyorMu)
        {
            throw new InvalidOperationException(
                "Bu çalışan seçilen hizmeti sunamıyor.");
        }

        // Hizmet süresi pozitif olmalı.
        if (hizmet.SureDakika <= 0)
        {
            throw new InvalidOperationException(
                "Sağlık hizmetinin süresi geçersiz.");
        }

        // Randevu bitişini hesapla.
        DateTime bitis = baslangic.AddMinutes(
            hizmet.SureDakika);

        // Uygunluk aralığı içinde kalmalı.
        if (baslangic < uygunluk.BaslangicTarihiSaati
            || bitis > uygunluk.BitisTarihiSaati)
        {
            throw new InvalidOperationException(
                "Randevu çalışanın uygunluk saatleri dışında.");
        }

        // Randevu aynı gün tamamlanmalı.
        if (baslangic.Date != bitis.Date)
        {
            throw new InvalidOperationException(
                "Randevu iki farklı güne yayılamaz.");
        }

        // Kurumun haftalık çalışma saatini bul.
        int gun = (int)baslangic.DayOfWeek;

        var mesai = await _baglam.KurumCalismaSaatleri
            .SingleOrDefaultAsync(
                s => s.KurumId == uygunluk.KurumId
                     && s.HaftaninGunu == gun,
                iptalBelirteci);

        if (mesai is null || mesai.KapaliMi)
        {
            throw new InvalidOperationException(
                "Kurum bu gün için açık değil.");
        }

        // Açılış saatini kontrol et.
        bool acilisGecerli = TimeOnly.TryParseExact(
            mesai.AcilisSaati,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var acilis);

        // Kapanış saatini kontrol et.
        bool kapanisGecerli = TimeOnly.TryParseExact(
            mesai.KapanisSaati,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var kapanis);

        if (!acilisGecerli || !kapanisGecerli)
        {
            throw new InvalidOperationException(
                "Kurumun çalışma saatleri hatalı.");
        }

        TimeOnly randevuBaslangici =
            TimeOnly.FromDateTime(baslangic);

        TimeOnly randevuBitisi =
            TimeOnly.FromDateTime(bitis);

        // Randevu mesai saatlerinde olmalı.
        if (randevuBaslangici < acilis
            || randevuBitisi > kapanis)
        {
            throw new InvalidOperationException(
                "Randevu kurumun çalışma saatleri dışında.");
        }

        // ÇALIŞANIN ÇAKIŞAN RANDEVULARINI KONTROL ET
        // Beklemede ve Onaylandi saatleri doludur.
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
                "Çalışanın bu saatte başka bir randevusu var.");
        }

        // HASTANIN ÇAKIŞAN RANDEVULARINI KONTROL ET
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
                "Hastanın bu saatte başka bir randevusu var.");
        }

        // Transaction beklerken randevu zamanı
        // geçmiş olabileceği için tekrar kontrol et.
        if (baslangic <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "Randevu zamanı geçti. Lütfen yeni saat seçin.");
        }

        // Bütün kontroller başarılı.
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

        // EŞZAMANLI GÜVENLİK:
        // Kontroller ve kayıt başarıyla tamamlandı.
        await islem.CommitAsync(iptalBelirteci);

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
                "Randevu bulunamadı.");
        }

        // Geçmiş randevular onaylanamaz.
        if (randevu.BaslangicTarihiSaati <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "Geçmiş tarihli randevular onaylanamaz.");
        }

        // Kullanıcı aktif doktor olmalı.
        bool doktorYetkiliMi = await _baglam.Kullanicilar
            .AnyAsync(k =>
                k.Id == onaylayanKullaniciId
                && k.Rol == "Doktor"
                && k.AktifMi);

        if (!doktorYetkiliMi)
        {
            throw new InvalidOperationException(
                "Bu kullanıcının randevu onaylama yetkisi yok.");
        }

        // Doktor sadece kendi randevusunu onaylar.
        bool randevuDoktoraAitMi = await _baglam.Doktorlar
            .AnyAsync(d =>
                d.Id == randevu.CalisanId
                && d.KurumId == randevu.KurumId
                && d.KullaniciId == onaylayanKullaniciId
                && d.AktifMi);

        if (!randevuDoktoraAitMi)
        {
            throw new InvalidOperationException(
                "Bu randevu ilgili doktora ait değil.");
        }

        // Yalnızca Beklemede -> Onaylandi
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
                "Randevu zaten işlenmiş veya beklemede değil.");
        }
    }

    // ============================================
    // 3. RANDEVU İPTAL ETME
    // ============================================

    public async Task RandevuIptalEtAsync(
        int randevuId,
        int islemYapanKullaniciId)
    {
        // İptal edilecek randevuyu bul.
        var randevu = await _baglam.Randevular
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == randevuId);

        if (randevu is null)
        {
            throw new InvalidOperationException(
                "Randevu bulunamadı.");
        }

        // İşlemi yapan kullanıcı aktif olmalı.
        var kullanici = await _baglam.Kullanicilar
            .AsNoTracking()
            .FirstOrDefaultAsync(k =>
                k.Id == islemYapanKullaniciId &&
                k.AktifMi);

        if (kullanici is null)
        {
            throw new InvalidOperationException(
                "Aktif kullanıcı bulunamadı.");
        }

        // Hasta sadece kendi randevusunu iptal eder.
        bool hastaYetkiliMi =
            kullanici.Rol == "Hasta" &&
            await _baglam.Hastalar.AnyAsync(h =>
                h.Id == randevu.HastaId &&
                h.KullaniciId == islemYapanKullaniciId);

        // Doktor sadece kendi randevusunu iptal eder.
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

        // Başlamış veya geçmiş randevular
        // iptal edilemez.
        DateTime simdi = DateTime.Now;

        if (randevu.BaslangicTarihiSaati <= simdi)
        {
            throw new InvalidOperationException(
                "Başlamış veya geçmiş randevular iptal edilemez.");
        }

        // Beklemede veya Onaylandi -> IptalEdildi
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
                "Randevu iptal edilemedi veya zaten işlenmiş.");
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
                "Randevu bulunamadı.");
        }

        // Sadece randevunun doktoru tamamlayabilir.
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

        // Randevunun bitiş zamanı gelmiş olmalı.
        DateTime simdi = DateTime.Now;

        if (randevu.BitisTarihiSaati > simdi)
        {
            throw new InvalidOperationException(
                "Randevu henüz sona ermedi.");
        }

        // Onaylandi -> Tamamlandi
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
                "Randevu onaylanmamış veya zaten işlenmiş.");
        }
    }
}
