using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Ekrana ait veri aktarım nesnesi. Veritabanı varlığı doğrudan forma açılmaz.
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

// Kalıtım + çok biçimlilik: Ortak yönetim sözleşmesini özelleştirir.
// Kapsülleme: doğrulamalar ve veri değişiklikleri yalnızca servis üzerinden yapılır.
public sealed class SaglikHizmetiYonetimServisi : YetkiliYonetimServisi<SaglikHizmetiBilgisi>
{
    public SaglikHizmetiYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public override async Task<IReadOnlyList<SaglikHizmetiBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        return await Baglam.SaglikHizmetleri.AsNoTracking()
            .OrderBy(h => h.Kurum!.Ad).ThenBy(h => h.Ad)
            .Select(h => new SaglikHizmetiBilgisi
            {
                Id = h.Id,
                KurumId = h.KurumId,
                KurumAdi = h.Kurum!.Ad,
                BolumId = h.BolumId,
                BolumAdi = h.Bolum == null ? "Genel / Bölümsüz" : h.Bolum.Ad,
                Ad = h.Ad,
                Aciklama = h.Aciklama ?? "",
                SureDakika = h.SureDakika,
                UcretKurus = h.UcretKurus,
                AktifMi = h.AktifMi
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(SaglikHizmetiBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        string ad = (kayit.Ad ?? "").Trim();
        string aciklama = (kayit.Aciklama ?? "").Trim();
        if (ad.Length is < 2 or > 150)
            throw new InvalidOperationException("Hizmet adı 2 ile 150 karakter arasında olmalı.");
        if (aciklama.Length > 500)
            throw new InvalidOperationException("Açıklama en fazla 500 karakter olabilir.");
        if (kayit.SureDakika is < 1 or > 1440)
            throw new InvalidOperationException("Hizmet süresi 1 ile 1440 dakika arasında olmalı.");
        if (kayit.UcretKurus is < 0 or > 1000000000L)
            throw new InvalidOperationException("Hizmet ücreti 0 ile 10.000.000 TL arasında olmalı.");
        if (!await Baglam.Kurumlar.AnyAsync(k => k.Id == kayit.KurumId && k.AktifMi))
            throw new InvalidOperationException("Aktif bir kurum seçiniz.");
        if (kayit.BolumId is int bolumId &&
            !await Baglam.Bolumler.AnyAsync(b => b.Id == bolumId && b.KurumId == kayit.KurumId))
            throw new InvalidOperationException("Seçilen bölüm bu kuruma ait değil.");
        string adKucuk = ad.ToLowerInvariant();
        if (await Baglam.SaglikHizmetleri.AnyAsync(h => h.KurumId == kayit.KurumId &&
            h.Id != kayit.Id && h.Ad.ToLower() == adKucuk))
            throw new InvalidOperationException("Bu kurumda aynı adlı sağlık hizmeti zaten mevcut.");

        SaglikHizmeti hizmet;
        if (kayit.Id == 0)
        {
            hizmet = new SaglikHizmeti { AktifMi = true };
            Baglam.SaglikHizmetleri.Add(hizmet);
        }
        else
        {
            hizmet = await Baglam.SaglikHizmetleri.FirstOrDefaultAsync(h => h.Id == kayit.Id)
                ?? throw new InvalidOperationException("Güncellenecek hizmet bulunamadı.");
            if (hizmet.KurumId != kayit.KurumId)
            {
                bool bagimli = await Baglam.CalisanHizmetleri.AnyAsync(a => a.SaglikHizmetiId == hizmet.Id) ||
                              await Baglam.Randevular.AnyAsync(r => r.SaglikHizmetiId == hizmet.Id);
                if (bagimli)
                    throw new InvalidOperationException(
                        "Ataması veya randevusu bulunan hizmet başka kuruma taşınamaz.");
            }
            if (hizmet.BolumId != kayit.BolumId &&
                (await Baglam.CalisanHizmetleri.AnyAsync(a => a.SaglikHizmetiId == hizmet.Id) ||
                 await Baglam.Randevular.AnyAsync(r => r.SaglikHizmetiId == hizmet.Id)))
                throw new InvalidOperationException(
                    "Ataması veya randevusu bulunan hizmetin bölümü değiştirilemez.");
        }
        hizmet.KurumId = kayit.KurumId;
        hizmet.BolumId = kayit.BolumId;
        hizmet.Ad = ad;
        hizmet.Aciklama = aciklama;
        hizmet.SureDakika = kayit.SureDakika;
        hizmet.UcretKurus = kayit.UcretKurus;
        await Baglam.SaveChangesAsync();
        return hizmet.Id;
    }

    public async Task DurumDegistirAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var hizmet = await Baglam.SaglikHizmetleri.FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException("Hizmet bulunamadı.");
        if (hizmet.AktifMi)
        {
            bool gelecekRandevu = await Baglam.Randevular.AnyAsync(r =>
                r.SaglikHizmetiId == hizmetId && r.BaslangicTarihiSaati > DateTime.Now &&
                (r.Durum == "Beklemede" || r.Durum == "Onaylandi"));
            if (gelecekRandevu)
                throw new InvalidOperationException(
                    "Gelecekte aktif randevusu bulunan hizmet pasifleştirilemez.");
        }
        else if (!await Baglam.Kurumlar.AnyAsync(k => k.Id == hizmet.KurumId && k.AktifMi))
        {
            throw new InvalidOperationException("Pasif kurumdaki hizmet aktifleştirilemez.");
        }
        hizmet.AktifMi = !hizmet.AktifMi;
        await Baglam.SaveChangesAsync();
    }

    public override async Task SilAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var hizmet = await Baglam.SaglikHizmetleri.FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException("Silinecek hizmet bulunamadı.");
        bool bagimli = await Baglam.Randevular.AnyAsync(r => r.SaglikHizmetiId == hizmetId) ||
                       await Baglam.CalisanHizmetleri.AnyAsync(a => a.SaglikHizmetiId == hizmetId);
        if (bagimli)
            throw new InvalidOperationException(
                "Randevusu veya doktor ataması olan hizmet kalıcı silinemez. Pasifleştirmeyi kullanın.");
        Baglam.SaglikHizmetleri.Remove(hizmet);
        await Baglam.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<DoktorHizmetAtamasiBilgisi>> AtananDoktorlariListeleAsync(int hizmetId)
    {
        await YoneticiYetkisiniDogrulaAsync();
        if (!await Baglam.SaglikHizmetleri.AnyAsync(h => h.Id == hizmetId))
            throw new InvalidOperationException("Sağlık hizmeti bulunamadı.");
        return await Baglam.Doktorlar.AsNoTracking()
            .Where(d => d.SunduguHizmetler.Any(a => a.SaglikHizmetiId == hizmetId))
            .OrderBy(d => d.Kullanici!.AdSoyad)
            .Select(d => new DoktorHizmetAtamasiBilgisi
            {
                CalisanId = d.Id,
                DoktorAdi = d.Kullanici!.AdSoyad,
                BolumAdi = d.Bolum == null ? "Bölüm yok" : d.Bolum.Ad,
                UzmanlikAlani = d.UzmanlikAlani
            }).ToListAsync();
    }

    public async Task DoktorAtaAsync(int hizmetId, int doktorId)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var hizmet = await Baglam.SaglikHizmetleri.AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == hizmetId)
            ?? throw new InvalidOperationException("Hizmet bulunamadı.");
        if (!hizmet.AktifMi || !await Baglam.Kurumlar.AnyAsync(k => k.Id == hizmet.KurumId && k.AktifMi))
            throw new InvalidOperationException("Yalnızca aktif kurumun aktif hizmetine doktor atanabilir.");

        var doktor = await Baglam.Doktorlar.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == doktorId && d.KurumId == hizmet.KurumId &&
                d.AktifMi && d.Kullanici!.AktifMi)
            ?? throw new InvalidOperationException("Aynı kurumdaki aktif bir doktor seçiniz.");
        if (hizmet.BolumId is int bolumId && doktor.BolumId != bolumId)
            throw new InvalidOperationException("Doktorun bölümü hizmetin bölümüyle eşleşmeli.");
        if (await Baglam.CalisanHizmetleri.AnyAsync(a =>
            a.SaglikHizmetiId == hizmetId && a.CalisanId == doktorId))
            throw new InvalidOperationException("Bu hizmet doktora zaten atanmış.");

        Baglam.CalisanHizmetleri.Add(new CalisanHizmeti
        {
            CalisanId = doktorId,
            SaglikHizmetiId = hizmetId,
            KurumId = hizmet.KurumId
        });
        await Baglam.SaveChangesAsync();
    }

    public async Task DoktorAtamasiniKaldirAsync(int hizmetId, int doktorId)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var atama = await Baglam.CalisanHizmetleri.FirstOrDefaultAsync(a =>
            a.SaglikHizmetiId == hizmetId && a.CalisanId == doktorId)
            ?? throw new InvalidOperationException("Doktor-hizmet ataması bulunamadı.");
        bool aktifGelecekRandevu = await Baglam.Randevular.AnyAsync(r =>
            r.SaglikHizmetiId == hizmetId && r.CalisanId == doktorId &&
            r.BaslangicTarihiSaati > DateTime.Now &&
            (r.Durum == "Beklemede" || r.Durum == "Onaylandi"));
        if (aktifGelecekRandevu)
            throw new InvalidOperationException(
                "Doktorun bu hizmet için gelecekte aktif randevusu var. Atama kaldırılamaz.");
        Baglam.CalisanHizmetleri.Remove(atama);
        await Baglam.SaveChangesAsync();
    }
}
