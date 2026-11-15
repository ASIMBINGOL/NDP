using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Sunum katmanina yalnizca gerekli bilgileri tasiyan liste satiri.
public sealed class PersonelRandevuSatiri
{
    [DisplayName("Randevu No")]
    public int RandevuNo { get; init; }

    [DisplayName("Saat")]
    public string Saat { get; init; } = "";

    [DisplayName("Hasta")]
    public string HastaAdi { get; init; } = "";

    [DisplayName("Doktor")]
    public string DoktorAdi { get; init; } = "";

    [DisplayName("Bölüm")]
    public string BolumAdi { get; init; } = "";

    [DisplayName("Sağlık Hizmeti")]
    public string HizmetAdi { get; init; } = "";

    [DisplayName("Randevu Durumu")]
    public string Durum { get; init; } = "";

    [DisplayName("Hasta Gelişi")]
    public string GelisBilgisi { get; init; } = "";

    [Browsable(false)]
    public DateTime BaslangicTarihiSaati { get; init; }

    [Browsable(false)]
    public DateTime BitisTarihiSaati { get; init; }

    [Browsable(false)]
    public DateTime? HastaGelisTarihiSaati { get; init; }
}

public sealed class PersonelKurumSecenegi
{
    public int Id { get; init; }
    public string Ad { get; init; } = "";
}

// Sağlık personelinin yetkisi randevuya ait kuruma göre her sorguda yeniden doğrulanır.
public sealed class SaglikPersoneliRandevuServisi
{
    private readonly VeritabaniBaglami _baglam;
    private readonly int _kullaniciId;

    public SaglikPersoneliRandevuServisi(VeritabaniBaglami baglam, int kullaniciId)
    {
        _baglam = baglam ?? throw new ArgumentNullException(nameof(baglam));
        _kullaniciId = kullaniciId;
    }

    private async Task<List<PersonelKurumSecenegi>> YetkiliKurumlariGetirAsync(
        CancellationToken iptalBelirteci = default)
    {
        // UI'daki secime guvenilmez: aktif hesap, dogru rol, aktif personel
        // profili ve aktif kurum her servis cagrisi icin dogrulanir.
        var kurumlar = await _baglam.SaglikPersonelleri.AsNoTracking()
            .Where(p => p.KullaniciId == _kullaniciId && p.AktifMi &&
                p.Kullanici != null && p.Kullanici.AktifMi &&
                p.Kullanici.Rol == "SaglikPersoneli" &&
                p.Kurum != null && p.Kurum.AktifMi)
            .Select(p => new { p.KurumId, KurumAdi = p.Kurum!.Ad })
            .ToListAsync(iptalBelirteci);

        if (kurumlar.Count == 0)
            throw new UnauthorizedAccessException(
                "Aktif bir sağlık personeli hesabı ve bağlı aktif hastane bulunamadı.");

        return kurumlar
            .GroupBy(k => k.KurumId)
            .Select(g => new PersonelKurumSecenegi
            {
                Id = g.Key,
                Ad = g.First().KurumAdi
            })
            .OrderBy(k => k.Ad)
            .ToList();
    }

    public async Task<IReadOnlyList<PersonelKurumSecenegi>> KurumlariListeleAsync(
        CancellationToken iptalBelirteci = default)
        => await YetkiliKurumlariGetirAsync(iptalBelirteci);

    public async Task<IReadOnlyList<PersonelRandevuSatiri>> RandevulariListeleAsync(
        DateTime gun, int? kurumId = null, CancellationToken iptalBelirteci = default)
    {
        var kurumlar = await YetkiliKurumlariGetirAsync(iptalBelirteci);
        var idler = kurumlar.Select(k => k.Id).ToList();
        if (kurumId.HasValue && !idler.Contains(kurumId.Value))
            throw new UnauthorizedAccessException("Bu hastanenin randevularını görüntüleme yetkiniz yok.");

        var baslangic = gun.Date;
        var bitis = baslangic.AddDays(1);

        var sorgu = _baglam.Randevular.AsNoTracking()
            .Where(r => idler.Contains(r.KurumId) &&
                r.BaslangicTarihiSaati >= baslangic &&
                r.BaslangicTarihiSaati < bitis);
        if (kurumId is int seciliKurumId)
            sorgu = sorgu.Where(r => r.KurumId == seciliKurumId);

        var kayitlar = await sorgu
            .OrderBy(r => r.BaslangicTarihiSaati)
            .ThenBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                r.BaslangicTarihiSaati,
                r.BitisTarihiSaati,
                r.Durum,
                r.HastaGelisTarihiSaati,
                Hasta = r.Hasta != null && r.Hasta.Kullanici != null
                    ? r.Hasta.Kullanici.AdSoyad : "-",
                Doktor = r.Calisan != null && r.Calisan.Kullanici != null
                    ? r.Calisan.Kullanici.AdSoyad : "-",
                Bolum = r.Calisan != null && r.Calisan.Bolum != null
                    ? r.Calisan.Bolum.Ad : "-",
                Hizmet = r.SaglikHizmeti != null ? r.SaglikHizmeti.Ad : "-"
            })
            .ToListAsync(iptalBelirteci);

        return kayitlar.Select(r => new PersonelRandevuSatiri
        {
            RandevuNo = r.Id,
            Saat = r.BaslangicTarihiSaati.ToString("HH:mm") + "–" +
                   r.BitisTarihiSaati.ToString("HH:mm"),
            HastaAdi = r.Hasta,
            DoktorAdi = r.Doktor,
            BolumAdi = r.Bolum,
            HizmetAdi = r.Hizmet,
            Durum = r.Durum,
            GelisBilgisi = r.HastaGelisTarihiSaati.HasValue
                ? "Geldi (" + r.HastaGelisTarihiSaati.Value.ToString("HH:mm") + ")"
                : "Gelmedi",
            BaslangicTarihiSaati = r.BaslangicTarihiSaati,
            BitisTarihiSaati = r.BitisTarihiSaati,
            HastaGelisTarihiSaati = r.HastaGelisTarihiSaati
        }).ToList();
    }

    public async Task HastaGeldiIsaretleAsync(
        int randevuId, CancellationToken iptalBelirteci = default)
    {
        var kurumlar = await YetkiliKurumlariGetirAsync(iptalBelirteci);
        var idler = kurumlar.Select(k => k.Id).ToList();
        var simdi = DateTime.Now;

        var randevu = await _baglam.Randevular.AsNoTracking()
            .Where(r => r.Id == randevuId && idler.Contains(r.KurumId))
            .Select(r => new
            {
                r.BaslangicTarihiSaati,
                r.BitisTarihiSaati,
                r.Durum,
                r.HastaGelisTarihiSaati
            })
            .SingleOrDefaultAsync(iptalBelirteci);

        if (randevu is null)
            throw new InvalidOperationException("Randevu bulunamadı veya bu hastanede yetkiniz yok.");
        if (randevu.HastaGelisTarihiSaati.HasValue)
            throw new InvalidOperationException("Hasta gelişi daha önce kaydedilmiş.");
        if (randevu.Durum != "Beklemede" && randevu.Durum != "Onaylandi")
            throw new InvalidOperationException("Yalnızca bekleyen veya onaylı randevular için geliş kaydedilebilir.");
        if (randevu.BaslangicTarihiSaati.Date != simdi.Date ||
            randevu.BitisTarihiSaati <= simdi)
            throw new InvalidOperationException("Hasta gelişi yalnızca bugünkü, süresi dolmamış randevuya kaydedilebilir.");

        // Yarış durumunda iki kullanıcı aynı kaydı ikinci kez işaretleyemesin;
        // işlem anında durum/tarih/hastane koşulları tekrar denetlenir.
        int degisen = await _baglam.Randevular
            .Where(r => r.Id == randevuId && idler.Contains(r.KurumId) &&
                r.HastaGelisTarihiSaati == null &&
                (r.Durum == "Beklemede" || r.Durum == "Onaylandi") &&
                r.BaslangicTarihiSaati >= simdi.Date &&
                r.BaslangicTarihiSaati < simdi.Date.AddDays(1) &&
                r.BitisTarihiSaati > simdi)
            .ExecuteUpdateAsync(
                ayar => ayar.SetProperty(r => r.HastaGelisTarihiSaati, (DateTime?)simdi),
                iptalBelirteci);

        if (degisen != 1)
            throw new InvalidOperationException(
                "Randevu bu sırada değişti veya hasta gelişi zaten kaydedildi. Listeyi yenileyiniz.");
    }
}
