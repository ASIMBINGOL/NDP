
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Ekrana ait veri aktarım nesnesi.
// Veritabanı varlığı doğrudan forma açılmaz.
public sealed class SaglikHizmetiBilgisi
{
    public int Id { get; init; }
    public int KurumId { get; init; }
    public string KurumAdi { get; init; } = "";
    public int? BolumId { get; init; }
    public string BolumAdi { get; init; } = "";
    public string Ad { get; init; } = "";
    public string Aciklama { get; init; } = "";
    public int SureDakika { get; init; }
    public long UcretKurus { get; init; }
    public bool AktifMi { get; init; }
}

public sealed class DoktorHizmetAtamasiBilgisi
{
    public int CalisanId { get; init; }
    public string DoktorAdi { get; init; } = "";
    public string BolumAdi { get; init; } = "";
    public string UzmanlikAlani { get; init; } = "";
}

// Kalıtım + çok biçimlilik:
// Ortak yönetim sözleşmesini özelleştirir.
//
// Kapsülleme:
// Doğrulamalar ve veri değişiklikleri
// yalnızca servis üzerinden yapılır.
public sealed class SaglikHizmetiYonetimServisi
    : YetkiliYonetimServisi<SaglikHizmetiBilgisi>
{
    public SaglikHizmetiYonetimServisi(
        VeritabaniBaglami baglam,
        int yoneticiId)
        : base(baglam, yoneticiId)
    {
    }

    // ============================================
    // 1. SAĞLIK HİZMETLERİNİ LİSTELEME
    // ============================================

    public override async Task<
        IReadOnlyList<SaglikHizmetiBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();

        return await Baglam.SaglikHizmetleri
            .AsNoTracking()
            .OrderBy(h => h.Kurum!.Ad)
            .ThenBy(h => h.Ad)
            .Select(h => new SaglikHizmetiBilgisi
            {
                Id = h.Id,
                KurumId = h.KurumId,
                KurumAdi = h.Kurum!.Ad,
                BolumId = h.BolumId,
                BolumAdi = h.Bolum == null
                    ? "Genel / Bölümsüz"
                    : h.Bolum.Ad,
                Ad = h.Ad,
                Aciklama = h.Aciklama ?? "",
                SureDakika = h.SureDakika,
                UcretKurus = h.UcretKurus,
                AktifMi = h.AktifMi
            })
            .ToListAsync();
    }

    // ============================================
    // 2. HİZMET EKLEME VE GÜNCELLEME
    // ============================================

    public override async Task<int> KaydetAsync(
        SaglikHizmetiBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();

        string ad = (kayit.Ad ?? "").Trim();
        string aciklama = (kayit.Aciklama ?? "").Trim();

        // Temel doğrulamalar
        if (ad.Length is < 2 or > 150)
        {
            throw new InvalidOperationException(
                "Hizmet adı 2 ile 150 karakter arasında olmalı.");
        }

        if (aciklama.Length > 500)
        {
            throw new InvalidOperationException(
                "Açıklama en fazla 500 karakter olabilir.");
        }

        if (kayit.SureDakika is < 1 or > 1440)
        {
            throw new InvalidOperationException(
                "Hizmet süresi 1 ile 1440 dakika arasında olmalı.");
        }

        if (kayit.UcretKurus is < 0 or > 1000000000L)
        {
            throw new InvalidOperationException(
                "Hizmet ücreti 0 ile 10.000.000 TL arasında olmalı.");
        }

        await using var islem =
            await Baglam.Database.BeginTransactionAsync();

        // Aktif kurum kontrolü
        if (!await Baglam.Kurumlar.AnyAsync(k =>
                k.Id == kayit.KurumId &&
                k.AktifMi))
        {
            throw new InvalidOperationException(
                "Aktif bir kurum seçiniz.");
        }

        // Bölümün seçilen kuruma ait olması gerekir.
        if (kayit.BolumId is int bolumId &&
            !await Baglam.Bolumler.AnyAsync(b =>
                b.Id == bolumId &&
                b.KurumId == kayit.KurumId))
        {
            throw new InvalidOperationException(
                "Seçilen bölüm bu kuruma ait değil.");
        }

        // Aynı kurumda aynı isimli hizmet olamaz.
        string adKucuk = ad.ToLowerInvariant();

        if (await Baglam.SaglikHizmetleri.AnyAsync(h =>
                h.KurumId == kayit.KurumId &&
                h.Id != kayit.Id &&
                h.Ad.ToLower() == adKucuk))
        {
            throw new InvalidOperationException(
                "Bu kurumda aynı adlı sağlık hizmeti zaten mevcut.");
        }

        // ========================================
        // YENİ SAĞLIK HİZMETİ OLUŞTURMA
        // ========================================

        if (kayit.Id == 0)
        {
            var yeniHizmet = new SaglikHizmeti
            {
                KurumId = kayit.KurumId,
                BolumId = kayit.BolumId,
                Ad = ad,
                Aciklama = aciklama,
                SureDakika = kayit.SureDakika,
                UcretKurus = kayit.UcretKurus,
                AktifMi = true
            };

            Baglam.SaglikHizmetleri.Add(yeniHizmet);

            await Baglam.SaveChangesAsync();
            await islem.CommitAsync();

            return yeniHizmet.Id;
        }

        // ========================================
        // MEVCUT SAĞLIK HİZMETİNİ GÜNCELLEME
        // ========================================

        var mevcut = await Baglam.SaglikHizmetleri
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == kayit.Id)
            ?? throw new InvalidOperationException(
                "Güncellenecek hizmet bulunamadı.");

        // KURAL 1:
        // Herhangi bir randevusu varsa hizmetin
        // bilgileri değiştirilemez.
        //
        // Bekleyen, onaylanan, tamamlanan ve
        // iptal edilen randevuların tamamı kontrol edilir.

        bool randevusuVarMi =
            await Baglam.Randevular.AnyAsync(r =>
                r.SaglikHizmetiId == kayit.Id);

        if (randevusuVarMi)
        {
            throw new InvalidOperationException(
                "Bu sağlık hizmetine ait randevu kayıtları bulunmaktadır.\n\n" +
                "Randevusu olan bir hizmetin ücreti, süresi veya " +
                "diğer bilgileri güncellenemez.\n\n" +
                "Farklı ücret veya süre ile hizmet sunmak için " +
                "yeni bir sağlık hizmeti oluşturunuz.");
        }

        // KURAL 2:
        // Kayıtlı hizmetlerin süreleri değiştirilemez.
        //
        // Randevusu bulunmasa bile süre sabit kalır.
        // Farklı süre için yeni hizmet açılmalıdır.

        if (kayit.SureDakika != mevcut.SureDakika)
        {
            throw new InvalidOperationException(
                "Kayıtlı sağlık hizmetinin süresi değiştirilemez. " +
                "Farklı süre gerekiyorsa yeni bir sağlık hizmeti oluşturunuz.");
        }

        // Kurum veya bölüm değişikliği var mı?
        bool kurumDegisiyor =
            mevcut.KurumId != kayit.KurumId;

        bool bolumDegisiyor =
            mevcut.BolumId != kayit.BolumId;

        // KURAL 3:
        // Doktor ataması varsa kurum veya bölüm
        // değiştirme işlemi engellenir.
        //
        // Randevu kontrolü yukarıda zaten yapıldığı
        // için burada ikinci kez yapılmaz.

        if (kurumDegisiyor || bolumDegisiyor)
        {
            bool atamasiVarMi =
                await Baglam.CalisanHizmetleri
                    .AnyAsync(a =>
                        a.SaglikHizmetiId == kayit.Id);

            if (atamasiVarMi)
            {
                throw new InvalidOperationException(
                    "Doktor ataması bulunan hizmetin " +
                    "kurumu veya bölümü değiştirilemez. " +
                    "Önce ilişkili kayıtları yönetin.");
            }
        }

        // ========================================
        // GÜVENLİ GÜNCELLEME
        // ========================================
        //
        // KurumId alternatif anahtarın parçasıdır.
        // ExecuteUpdateAsync ile ilişkisi bulunmayan
        // hizmet aynı Id korunarak güncellenir.
        //
        // SureDakika güncellenmez.
        // Süre mevcut değeriyle korunur.

        int degisen = await Baglam.SaglikHizmetleri
            .Where(h => h.Id == kayit.Id)
            .ExecuteUpdateAsync(ayarlar => ayarlar
                .SetProperty(
                    h => h.KurumId,
                    kayit.KurumId)
                .SetProperty(
                    h => h.BolumId,
                    kayit.BolumId)
                .SetProperty(
                    h => h.Ad,
                    ad)
                .SetProperty(
                    h => h.Aciklama,
                    aciklama)
                .SetProperty(
                    h => h.UcretKurus,
                    kayit.UcretKurus));

        if (degisen != 1)
        {
            throw new InvalidOperationException(
                "Sağlık hizmeti güncellenemedi.");
        }

        await islem.CommitAsync();

        return kayit.Id;
    }

    // ============================================
    // 3. HİZMETİ PASİFLEŞTİRME / AKTİFLEŞTİRME
    // ============================================

    public async Task DurumDegistirAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        var hizmet = await Baglam.SaglikHizmetleri
            .FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException(
                "Hizmet bulunamadı.");

        // Pasifleştirme
        if (hizmet.AktifMi)
        {
            bool gelecekRandevu =
                await Baglam.Randevular.AnyAsync(r =>
                    r.SaglikHizmetiId == hizmetId &&
                    r.BaslangicTarihiSaati > DateTime.Now &&
                    (r.Durum == "Beklemede" ||
                     r.Durum == "Onaylandi"));

            if (gelecekRandevu)
            {
                throw new InvalidOperationException(
                    "Gelecekte aktif randevusu bulunan hizmet pasifleştirilemez.");
            }
        }
        // Aktifleştirme
        else if (!await Baglam.Kurumlar.AnyAsync(k =>
                     k.Id == hizmet.KurumId &&
                     k.AktifMi))
        {
            throw new InvalidOperationException(
                "Pasif kurumdaki hizmet aktifleştirilemez.");
        }

        hizmet.AktifMi = !hizmet.AktifMi;

        await Baglam.SaveChangesAsync();
    }

    // ============================================
    // 4. HİZMETİ KALICI SİLME
    // ============================================

    public override async Task SilAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        var hizmet = await Baglam.SaglikHizmetleri
            .FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException(
                "Silinecek hizmet bulunamadı.");

        bool bagimli =
            await Baglam.Randevular.AnyAsync(r =>
                r.SaglikHizmetiId == hizmetId)
            ||
            await Baglam.CalisanHizmetleri.AnyAsync(a =>
                a.SaglikHizmetiId == hizmetId);

        if (bagimli)
        {
            throw new InvalidOperationException(
                "Randevusu veya doktor ataması olan hizmet " +
                "kalıcı silinemez. Pasifleştirmeyi kullanın.");
        }

        Baglam.SaglikHizmetleri.Remove(hizmet);

        await Baglam.SaveChangesAsync();
    }

    // ============================================
    // 5. ATANAN DOKTORLARI LİSTELEME
    // ============================================

    public async Task<
        IReadOnlyList<DoktorHizmetAtamasiBilgisi>>
        AtananDoktorlariListeleAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        if (!await Baglam.SaglikHizmetleri.AnyAsync(h =>
                h.Id == hizmetId))
        {
            throw new InvalidOperationException(
                "Sağlık hizmeti bulunamadı.");
        }

        return await Baglam.Doktorlar
            .AsNoTracking()
            .Where(d =>
                d.SunduguHizmetler.Any(a =>
                    a.SaglikHizmetiId == hizmetId))
            .OrderBy(d => d.Kullanici!.AdSoyad)
            .Select(d => new DoktorHizmetAtamasiBilgisi
            {
                CalisanId = d.Id,
                DoktorAdi = d.Kullanici!.AdSoyad,
                BolumAdi = d.Bolum == null
                    ? "Bölüm yok"
                    : d.Bolum.Ad,
                UzmanlikAlani = d.UzmanlikAlani
            })
            .ToListAsync();
    }

    // ============================================
    // 6. DOKTORA SAĞLIK HİZMETİ ATAMA
    // ============================================

    public async Task DoktorAtaAsync(
        int hizmetId,
        int doktorId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        var hizmet = await Baglam.SaglikHizmetleri
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException(
                "Hizmet bulunamadı.");

        if (!hizmet.AktifMi ||
            !await Baglam.Kurumlar.AnyAsync(k =>
                k.Id == hizmet.KurumId &&
                k.AktifMi))
        {
            throw new InvalidOperationException(
                "Yalnızca aktif kurumun aktif hizmetine doktor atanabilir.");
        }

        var doktor = await Baglam.Doktorlar
            .AsNoTracking()
            .FirstOrDefaultAsync(d =>
                d.Id == doktorId &&
                d.KurumId == hizmet.KurumId &&
                d.AktifMi &&
                d.Kullanici!.AktifMi)
            ?? throw new InvalidOperationException(
                "Aynı kurumdaki aktif bir doktor seçiniz.");

        if (hizmet.BolumId is int bolumId &&
            doktor.BolumId != bolumId)
        {
            throw new InvalidOperationException(
                "Doktorun bölümü hizmetin bölümüyle eşleşmeli.");
        }

        if (await Baglam.CalisanHizmetleri.AnyAsync(a =>
                a.SaglikHizmetiId == hizmetId &&
                a.CalisanId == doktorId))
        {
            throw new InvalidOperationException(
                "Bu hizmet doktora zaten atanmış.");
        }

        Baglam.CalisanHizmetleri.Add(new CalisanHizmeti
        {
            CalisanId = doktorId,
            SaglikHizmetiId = hizmetId,
            KurumId = hizmet.KurumId
        });

        await Baglam.SaveChangesAsync();
    }

    // ============================================
    // 7. DOKTORUN HİZMET ATAMASINI KALDIRMA
    // ============================================

    public async Task DoktorAtamasiniKaldirAsync(
        int hizmetId,
        int doktorId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        var atama = await Baglam.CalisanHizmetleri
            .FirstOrDefaultAsync(a =>
                a.SaglikHizmetiId == hizmetId &&
                a.CalisanId == doktorId)
            ?? throw new InvalidOperationException(
                "Doktor-hizmet ataması bulunamadı.");

        // Randevular doktor-hizmet ilişkisine bağlıdır.
        // Tamamlanmış veya iptal edilmiş randevular
        // da geçmiş kayıt olarak korunmalıdır.
        //
        // Bu nedenle yalnızca gelecekteki randevuları
        // değil, bütün randevuları kontrol ediyoruz.

        bool herhangiBirRandevusuVarMi =
            await Baglam.Randevular.AnyAsync(r =>
                r.SaglikHizmetiId == hizmetId &&
                r.CalisanId == doktorId);

        if (herhangiBirRandevusuVarMi)
        {
            throw new InvalidOperationException(
                "Bu doktora ve hizmete ait randevu kayıtları " +
                "bulunduğu için atama kaldırılamaz. " +
                "Randevu geçmişi korunmalıdır.");
        }

        Baglam.CalisanHizmetleri.Remove(atama);

        await Baglam.SaveChangesAsync();
    }
}
