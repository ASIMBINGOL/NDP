using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

public sealed class BolumBilgisi
{
    public int Id { get; init; }
    public int KurumId { get; init; }
    public string KurumAdi { get; init; } = "";
    public string Ad { get; init; } = "";
    public string Aciklama { get; init; } = "";
}

public sealed class BolumYonetimServisi : YetkiliYonetimServisi<BolumBilgisi>
{
    public BolumYonetimServisi(VeritabaniBaglami baglam, int yoneticiId)
        : base(baglam, yoneticiId) { }

    public override async Task<IReadOnlyList<BolumBilgisi>> ListeleAsync()
    {
        await YoneticiYetkisiniDogrulaAsync();
        return await Baglam.Bolumler.AsNoTracking()
            .OrderBy(b => b.Kurum!.Ad).ThenBy(b => b.Ad)
            .Select(b => new BolumBilgisi
            {
                Id = b.Id, KurumId = b.KurumId, KurumAdi = b.Kurum!.Ad,
                Ad = b.Ad, Aciklama = b.Aciklama ?? ""
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(BolumBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var ad = kayit.Ad.Trim();
        var aciklama = kayit.Aciklama.Trim();
        if (ad.Length is < 2 or > 100 || aciklama.Length > 500)
            throw new InvalidOperationException("Bölüm adı 2-100, açıklama en fazla 500 karakter olmalı.");
        if (!await Baglam.Kurumlar.AnyAsync(k => k.Id == kayit.KurumId && k.AktifMi))
            throw new InvalidOperationException("Aktif bir kurum seçiniz.");
        if (await Baglam.Bolumler.AnyAsync(b => b.KurumId == kayit.KurumId &&
                                            b.Ad == ad && b.Id != kayit.Id))
            throw new InvalidOperationException("Bu kurumda aynı adlı bölüm zaten mevcut.");

        Bolum bolum;
        if (kayit.Id == 0)
        {
            bolum = new Bolum();
            Baglam.Bolumler.Add(bolum);
        }
        else
        {
            bolum = await Baglam.Bolumler.FirstOrDefaultAsync(b => b.Id == kayit.Id)
                ?? throw new InvalidOperationException("Bölüm bulunamadı.");
            if (bolum.KurumId != kayit.KurumId)
            {
                bool iliski = await Baglam.Calisanlar.AnyAsync(c => c.BolumId == bolum.Id && c.KurumId == bolum.KurumId)
                    || await Baglam.SaglikHizmetleri.AnyAsync(h => h.BolumId == bolum.Id && h.KurumId == bolum.KurumId);
                if (iliski)
                    throw new InvalidOperationException("Çalışanı veya hizmeti olan bölüm başka kuruma taşınamaz.");
            }
        }
        bolum.KurumId = kayit.KurumId;
        bolum.Ad = ad;
        bolum.Aciklama = aciklama;
        await Baglam.SaveChangesAsync();
        return bolum.Id;
    }

    public override async Task SilAsync(int id)
    {
        await YoneticiYetkisiniDogrulaAsync();
        var bolum = await Baglam.Bolumler.FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new InvalidOperationException("Bölüm bulunamadı.");
        bool bagimliKayit = await Baglam.Calisanlar.AnyAsync(c => c.BolumId == id) ||
                            await Baglam.SaglikHizmetleri.AnyAsync(h => h.BolumId == id);
        if (bagimliKayit)
            throw new InvalidOperationException(
                "Doktor/personel veya hizmet içeren bölüm silinemez. Önce ilgili kayıtları yönetin.");
        Baglam.Bolumler.Remove(bolum);
        await Baglam.SaveChangesAsync();
    }
}
