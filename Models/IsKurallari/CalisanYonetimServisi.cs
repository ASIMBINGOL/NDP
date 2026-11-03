using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Arayüze yalnızca gereken alanlar döndürülür. Parola özeti dışarıya verilmez.
public sealed class CalisanBilgisi
{
    public int Id { get; init; }
    public int KullaniciId { get; init; }
    public int KurumId { get; init; }
    public int? BolumId { get; init; }
    public string KurumAdi { get; init; } = "";
    public string BolumAdi { get; init; } = "";
    public string AdSoyad { get; init; } = "";
    public string Eposta { get; init; } = "";
    public string Tur { get; init; } = "Doktor";
    public string UzmanlikVeyaGorev { get; init; } = "";
    public string GorevBilgisi { get; init; } = "";
    public bool AktifMi { get; init; }
    // Yeni kayıtta zorunlu; güncellemede boş bırakılırsa önceki parola korunur.
    public string YeniSifre { get; init; } = "";
}

// Kalıtım: mevcut ortak yetkilendirme ve CRUD sözleşmesini devralır.
// Kapsülleme: bütün doğrulamalar, parola üretimi ve veri değişiklikleri bu sınıftadır.
public sealed class CalisanYonetimServisi : YetkiliYonetimServisi<CalisanBilgisi>
{
    public CalisanYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public override async Task<IReadOnlyList<CalisanBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        var calisanlar = await Baglam.Calisanlar.AsNoTracking()
            .Include(c => c.Kullanici)
            .Include(c => c.Kurum)
            .Include(c => c.Bolum)
            .OrderBy(c => c.Kullanici!.AdSoyad)
            .ThenBy(c => c.Id)
            .ToListAsync();

        // Çok biçimlilik: aynı soyut Calisan referansı üzerinde
        // Doktor.GorevBilgisi veya SaglikPersoneli.GorevBilgisi çalışır.
        return calisanlar.Select(c => new CalisanBilgisi
        {
            Id = c.Id,
            KullaniciId = c.KullaniciId,
            KurumId = c.KurumId,
            BolumId = c.BolumId,
            KurumAdi = c.Kurum?.Ad ?? "",
            BolumAdi = c.Bolum?.Ad ?? "",
            AdSoyad = c.Kullanici?.AdSoyad ?? "",
            Eposta = c.Kullanici?.Eposta ?? "",
            Tur = c is Doktor ? "Doktor" : "SaglikPersoneli",
            UzmanlikVeyaGorev = c is Doktor d ? d.UzmanlikAlani
                : c is SaglikPersoneli p ? p.Gorev : "",
            GorevBilgisi = c.GorevBilgisi(),
            AktifMi = c.AktifMi && (c.Kullanici?.AktifMi ?? false)
        }).ToList();
    }

    public override async Task<int> KaydetAsync(CalisanBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        string ad = (kayit.AdSoyad ?? "").Trim();
        string eposta = (kayit.Eposta ?? "").Trim().ToLowerInvariant();
        string tur = (kayit.Tur ?? "").Trim();
        string gorev = (kayit.UzmanlikVeyaGorev ?? "").Trim();
        string yeniSifre = kayit.YeniSifre ?? "";

        if (ad.Length is < 3 or > 150)
            throw new InvalidOperationException("Ad soyad 3-150 karakter olmalıdır.");
        if (eposta.Length > 254 ||
            !MailAddress.TryCreate(eposta, out var posta) || posta.Address != eposta)
            throw new InvalidOperationException("Geçerli bir e-posta adresi giriniz.");
        if (tur != "Doktor" && tur != "SaglikPersoneli")
            throw new InvalidOperationException("Çalışan türünü seçiniz.");
        if (gorev.Length is < 2 or > 100)
            throw new InvalidOperationException("Uzmanlık / görev 2-100 karakter olmalıdır.");
        if (yeniSifre.Length > 0 && yeniSifre.Length < 8)
            throw new InvalidOperationException("Şifre en az 8 karakter olmalıdır.");
        if (kayit.Id == 0 && yeniSifre.Length == 0)
            throw new InvalidOperationException("Yeni çalışan için şifre gereklidir.");

        bool kurumAktif = await Baglam.Kurumlar
            .AnyAsync(k => k.Id == kayit.KurumId && k.AktifMi);
        if (!kurumAktif)
        {
            // Pasif kurumda yeni çalışan eklenmez veya başka kurumdan taşınmaz.
            // Mevcut çalışanın aynı kurumdaki kişisel bilgileri düzenlenebilir.
            bool mevcutKurumuMu = kayit.Id != 0 && await Baglam.Calisanlar
                .AnyAsync(c => c.Id == kayit.Id && c.KurumId == kayit.KurumId);
            if (!mevcutKurumuMu)
                throw new InvalidOperationException("Yeni kayıt veya kurum değişikliği için aktif kurum seçiniz.");
        }
        if (kayit.BolumId is int bolumId && !await Baglam.Bolumler
                .AnyAsync(b => b.Id == bolumId && b.KurumId == kayit.KurumId))
            throw new InvalidOperationException("Bölüm seçilen kuruma ait olmalıdır.");

        if (kayit.Id == 0)
        {
            if (await Baglam.Kullanicilar.AnyAsync(k => k.Eposta == eposta))
                throw new InvalidOperationException("Bu e-posta adresi zaten kayıtlı.");

            var kullanici = new Kullanici
            {
                AdSoyad = ad,
                Eposta = eposta,
                SifreOzeti = SifreOzetiOlustur(yeniSifre),
                Rol = tur,
                AktifMi = true
            };
            Calisan calisan = tur == "Doktor"
                ? new Doktor { UzmanlikAlani = gorev }
                : new SaglikPersoneli { Gorev = gorev };
            calisan.Kullanici = kullanici;
            calisan.KurumId = kayit.KurumId;
            calisan.BolumId = kayit.BolumId;
            calisan.AktifMi = true;
            Baglam.Calisanlar.Add(calisan);
            // Her iki kayıt aynı SaveChanges ile yazılır; biri başarısızsa ikisi de kalmaz.
            await Baglam.SaveChangesAsync();
            return calisan.Id;
        }

        var mevcut = await Baglam.Calisanlar
            .Include(c => c.Kullanici)
            .FirstOrDefaultAsync(c => c.Id == kayit.Id)
            ?? throw new InvalidOperationException("Çalışan bulunamadı.");
        if (mevcut.Kullanici is null)
            throw new InvalidOperationException("Çalışanın kullanıcı hesabı bulunamadı.");
        string gercekTur = mevcut is Doktor ? "Doktor" : "SaglikPersoneli";
        if (tur != gercekTur)
            throw new InvalidOperationException(
                "Kayıtlı çalışanın türü değiştirilemez. Yeni kayıt oluşturunuz.");
        if (mevcut.Kullanici.Rol != gercekTur)
            throw new InvalidOperationException("Kullanıcı rolüyle çalışan türü uyuşmuyor.");
        if (await Baglam.Kullanicilar.AnyAsync(k => k.Eposta == eposta && k.Id != mevcut.KullaniciId))
            throw new InvalidOperationException("Bu e-posta başka bir kullanıcıya ait.");

        if (mevcut.KurumId != kayit.KurumId)
        {
            // KurumId bazı ilişkilerde birleşik yabancı anahtarın parçası.
            // Randevu geçmişi ve atamalar varsa değiştirmek veri bütünlüğünü bozar.
            if (await IliskiliKayitVarMiAsync(mevcut.Id))
                throw new InvalidOperationException(
                    "Randevusu, uygunluğu veya hizmet ataması olan personelin kurumu değiştirilemez.");
            if (await Baglam.Calisanlar.AnyAsync(c => c.Id != mevcut.Id &&
                c.KullaniciId == mevcut.KullaniciId && c.KurumId == kayit.KurumId))
                throw new InvalidOperationException("Bu çalışan zaten seçilen kurumda kayıtlı.");
        }

        mevcut.Kullanici.AdSoyad = ad;
        mevcut.Kullanici.Eposta = eposta;
        if (yeniSifre.Length > 0)
            mevcut.Kullanici.SifreOzeti = SifreOzetiOlustur(yeniSifre);

        if (mevcut.KurumId != kayit.KurumId)
        {
            // KurumId alternatif anahtarın parçasıdır; EF anahtar değiştirmeyi
            // desteklemez. Bağımsız çalışanın eski kaydı silinip yeni kuruma
            // aynı kullanıcı hesabıyla yeni çalışan kaydı eklenir.
            Calisan tasinan = tur == "Doktor"
                ? new Doktor { UzmanlikAlani = gorev }
                : new SaglikPersoneli { Gorev = gorev };
            tasinan.Kullanici = mevcut.Kullanici;
            tasinan.KurumId = kayit.KurumId;
            tasinan.BolumId = kayit.BolumId;
            tasinan.AktifMi = mevcut.AktifMi;
            Baglam.Calisanlar.Remove(mevcut);
            Baglam.Calisanlar.Add(tasinan);
            await Baglam.SaveChangesAsync();
            return tasinan.Id;
        }

        mevcut.BolumId = kayit.BolumId;
        if (mevcut is Doktor doktor) doktor.UzmanlikAlani = gorev;
        if (mevcut is SaglikPersoneli personel) personel.Gorev = gorev;
        await Baglam.SaveChangesAsync();
        return mevcut.Id;
    }


    public async Task DurumDegistirAsync(int calisanId)
    {
        await YoneticiYetkisiniDogrulaAsync();

        var calisan = await Baglam.Calisanlar
            .Include(c => c.Kullanici)
            .FirstOrDefaultAsync(c => c.Id == calisanId)
            ?? throw new InvalidOperationException(
                "Çalışan bulunamadı.");

        if (calisan.Kullanici is null)
            throw new InvalidOperationException(
                "Kullanıcı hesabı bulunamadı.");

        // Aktif çalışan pasifleştirilmek isteniyorsa
        // gelecekteki randevuları koru.
        if (calisan.AktifMi)
        {
            bool gelecekRandevuVarMi =
                await Baglam.Randevular.AnyAsync(r =>
                    r.CalisanId == calisanId &&
                    r.BaslangicTarihiSaati > DateTime.Now &&
                    (r.Durum == "Beklemede" ||
                     r.Durum == "Onaylandi"));

            if (gelecekRandevuVarMi)
            {
                throw new InvalidOperationException(
                    "Bu çalışanın gelecekte bekleyen veya " +
                    "onaylanmış randevuları bulunuyor. " +
                    "Önce bu randevuları yönetin.");
            }
        }
        else
        {
            bool kurumAktifMi =
                await Baglam.Kurumlar.AnyAsync(k =>
                    k.Id == calisan.KurumId && k.AktifMi);

            if (!kurumAktifMi)
            {
                throw new InvalidOperationException(
                    "Pasif kurumdaki çalışan aktifleştirilemez.");
            }
        }

        calisan.AktifMi = !calisan.AktifMi;

        if (calisan.AktifMi)
        {
            calisan.Kullanici.AktifMi = true;
        }
        else
        {
            bool digerAktif = await Baglam.Calisanlar
                .AnyAsync(c =>
                    c.KullaniciId == calisan.KullaniciId &&
                    c.Id != calisan.Id &&
                    c.AktifMi);

            if (!digerAktif)
            {
                calisan.Kullanici.AktifMi = false;
            }
        }

        await Baglam.SaveChangesAsync();
    }


    public override async Task SilAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var calisan = await Baglam.Calisanlar.Include(c => c.Kullanici)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new InvalidOperationException("Çalışan bulunamadı.");
        if (await IliskiliKayitVarMiAsync(id))
            throw new InvalidOperationException(
                "Bu çalışanın randevusu, uygunluğu veya hizmet ataması var. " +
                "Geçmişi korumak için kalıcı silmek yerine pasifleştirin.");

        bool baskaCalisanlik = await Baglam.Calisanlar
            .AnyAsync(c => c.KullaniciId == calisan.KullaniciId && c.Id != id);
        Baglam.Calisanlar.Remove(calisan);
        if (!baskaCalisanlik && calisan.Kullanici is not null)
        {
            // Kullanıcıyı başka profil/ilişki kullanıyorsa kaldırma.
            bool hastaProfili = await Baglam.Hastalar
                .AnyAsync(h => h.KullaniciId == calisan.KullaniciId);
            if (!hastaProfili) Baglam.Kullanicilar.Remove(calisan.Kullanici);
        }
        await Baglam.SaveChangesAsync();
    }

    private async Task<bool> IliskiliKayitVarMiAsync(int calisanId) =>
        await Baglam.Randevular.AnyAsync(r => r.CalisanId == calisanId) ||
        await Baglam.UygunlukAraliklari.AnyAsync(u => u.CalisanId == calisanId) ||
        await Baglam.CalisanHizmetleri.AnyAsync(h => h.CalisanId == calisanId);

    private static string SifreOzetiOlustur(string sifre)
    {
        byte[] tuz = RandomNumberGenerator.GetBytes(16);
        byte[] ozet = Rfc2898DeriveBytes.Pbkdf2(
            sifre, tuz, 210_000, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2-SHA256$210000${Convert.ToBase64String(tuz)}$" +
               Convert.ToBase64String(ozet);
    }
}
