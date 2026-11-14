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
                Id = b.Id,
                KurumId = b.KurumId,
                KurumAdi = b.Kurum!.Ad,
                Ad = b.Ad,
                Aciklama = b.Aciklama ?? ""
            }).ToListAsync();
    }

    public override async Task<int> KaydetAsync(BolumBilgisi kayit)
    {
        await YoneticiYetkisiniDogrulaAsync();

        string ad = (kayit.Ad ?? "").Trim();
        string aciklama = (kayit.Aciklama ?? "").Trim();

        if (ad.Length is < 2 or > 100 || aciklama.Length > 500)
            throw new InvalidOperationException(
                "Bölüm adı 2-100, açıklama en fazla 500 karakter olmalı.");

        // Doğrulamalar ile kayıt işlemini aynı transaction'da yap.
        await using var islem = await Baglam.Database.BeginTransactionAsync();

        if (!await Baglam.Kurumlar.AnyAsync(k =>
                k.Id == kayit.KurumId && k.AktifMi))
            throw new InvalidOperationException("Aktif bir kurum seçiniz.");

        if (await Baglam.Bolumler.AnyAsync(b =>
                b.KurumId == kayit.KurumId &&
                b.Ad == ad && b.Id != kayit.Id))
            throw new InvalidOperationException(
                "Bu kurumda aynı adlı bölüm zaten mevcut.");

        if (kayit.Id == 0)
        {
            var yeniBolum = new Bolum
            {
                KurumId = kayit.KurumId,
                Ad = ad,
                Aciklama = aciklama
            };

            Baglam.Bolumler.Add(yeniBolum);
            await Baglam.SaveChangesAsync();
            await islem.CommitAsync();
            return yeniBolum.Id;
        }

        // İzlenen bir varlığın alternatif anahtarını doğrudan değiştiremeyiz.
        var mevcut = await Baglam.Bolumler.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == kayit.Id)
            ?? throw new InvalidOperationException("Bölüm bulunamadı.");

        if (mevcut.KurumId != kayit.KurumId)
        {
            bool calisaniVarMi = await Baglam.Calisanlar
                .AnyAsync(c => c.BolumId == kayit.Id);

            bool hizmetiVarMi = await Baglam.SaglikHizmetleri
                .AnyAsync(h => h.BolumId == kayit.Id);

            if (calisaniVarMi || hizmetiVarMi)
                throw new InvalidOperationException(
                    "Bu bölüme bağlı çalışan veya sağlık hizmeti var. " +
                    "Önce ilişkili kayıtları yönetin; bölümün kurumu değiştirilemez.");
        }

        // ExecuteUpdate, alternatif anahtarın izlenerek değiştirilmesi
        // hatasına yol açmadan aynı bölüm Id'sini korur.
        int degisen = await Baglam.Bolumler
            .Where(b => b.Id == kayit.Id)
            .ExecuteUpdateAsync(ayarlar => ayarlar
                .SetProperty(b => b.KurumId, kayit.KurumId)
                .SetProperty(b => b.Ad, ad)
                .SetProperty(b => b.Aciklama, aciklama));

        if (degisen != 1)
            throw new InvalidOperationException("Bölüm güncellenemedi.");

        await islem.CommitAsync();
        return kayit.Id;
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
