
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;

namespace NDPProje.Veri;

public static class OrnekVeriHazirlayici
{
    public static async Task HazirlaAsync()
    {
        await using var baglam = new VeritabaniBaglami();

        // Veritabani gecislerini uygula.
        await baglam.Database.MigrateAsync();

        // 1. Ornek kurum
        var kurum = await baglam.Kurumlar
            .FirstOrDefaultAsync(k => k.Ad == "Ornek Saglik Merkezi");

        if (kurum is null)
        {
            kurum = new Kurum
            {
                Ad = "Ornek Saglik Merkezi",
                KurumTuru = "Klinik",
                Adres = "Serdivan, Sakarya",
                Telefon = "02640000000",
                AktifMi = true
            };

            baglam.Kurumlar.Add(kurum);
            await baglam.SaveChangesAsync();
        }

        // 2. Ornek bolum
        var bolum = await baglam.Bolumler
            .FirstOrDefaultAsync(b =>
                b.KurumId == kurum.Id &&
                b.Ad == "Dahiliye");

        if (bolum is null)
        {
            bolum = new Bolum
            {
                KurumId = kurum.Id,
                Ad = "Dahiliye",
                Aciklama = "Dahiliye bolumu"
            };

            baglam.Bolumler.Add(bolum);
            await baglam.SaveChangesAsync();
        }

        // 3. Kurumun haftalik calisma saatleri
        for (int gun = 0; gun <= 6; gun++)
        {
            bool kapali = gun == (int)DayOfWeek.Sunday;

            var mesai = await baglam.KurumCalismaSaatleri
                .FirstOrDefaultAsync(s =>
                    s.KurumId == kurum.Id &&
                    s.HaftaninGunu == gun);

            if (mesai is null)
            {
                mesai = new KurumCalismaSaati
                {
                    KurumId = kurum.Id,
                    HaftaninGunu = gun,
                    KapaliMi = kapali,
                    AcilisSaati = kapali ? null : "09:00",
                    KapanisSaati = kapali
                        ? null
                        : gun == (int)DayOfWeek.Saturday
                            ? "13:00"
                            : "17:00"
                };

                baglam.KurumCalismaSaatleri.Add(mesai);
            }
        }

        await baglam.SaveChangesAsync();

        // 4. Ornek hasta kullanicisi
        var hastaKullanici = await baglam.Kullanicilar
            .FirstOrDefaultAsync(k =>
                k.Eposta == "hasta@ornek.local");

        if (hastaKullanici is null)
        {
            hastaKullanici = new Kullanici
            {
                AdSoyad = "Ornek Hasta",
                Eposta = "hasta@ornek.local",
                SifreOzeti = "TEST-VERISI",
                Rol = "Hasta",
                AktifMi = true
            };

            baglam.Kullanicilar.Add(hastaKullanici);
            await baglam.SaveChangesAsync();
        }

        var hasta = await baglam.Hastalar
            .FirstOrDefaultAsync(h =>
                h.KullaniciId == hastaKullanici.Id);

        if (hasta is null)
        {
            hasta = new Hasta
            {
                KullaniciId = hastaKullanici.Id,
                Telefon = "05000000000",
                DogumTarihi = new DateTime(1998, 5, 15)
            };

            baglam.Hastalar.Add(hasta);
            await baglam.SaveChangesAsync();
        }

        // 5. Ornek doktor kullanicisi
        var doktorKullanici = await baglam.Kullanicilar
            .FirstOrDefaultAsync(k =>
                k.Eposta == "doktor@ornek.local");

        if (doktorKullanici is null)
        {
            doktorKullanici = new Kullanici
            {
                AdSoyad = "Ornek Doktor",
                Eposta = "doktor@ornek.local",
                SifreOzeti = "TEST-VERISI",
                Rol = "Doktor",
                AktifMi = true
            };

            baglam.Kullanicilar.Add(doktorKullanici);
            await baglam.SaveChangesAsync();
        }

        var doktor = await baglam.Doktorlar
            .FirstOrDefaultAsync(d =>
                d.KullaniciId == doktorKullanici.Id &&
                d.KurumId == kurum.Id);

        if (doktor is null)
        {
            doktor = new Doktor
            {
                KullaniciId = doktorKullanici.Id,
                KurumId = kurum.Id,
                BolumId = bolum.Id,
                UzmanlikAlani = "Dahiliye",
                AktifMi = true
            };

            baglam.Doktorlar.Add(doktor);
            await baglam.SaveChangesAsync();
        }

        // 6. Ornek saglik hizmeti
        var hizmet = await baglam.SaglikHizmetleri
            .FirstOrDefaultAsync(h =>
                h.KurumId == kurum.Id &&
                h.Ad == "Genel Muayene");

        if (hizmet is null)
        {
            hizmet = new SaglikHizmeti
            {
                KurumId = kurum.Id,
                BolumId = bolum.Id,
                Ad = "Genel Muayene",
                Aciklama = "Ornek muayene hizmeti",
                SureDakika = 30,
                UcretKurus = 75000,
                AktifMi = true
            };

            baglam.SaglikHizmetleri.Add(hizmet);
            await baglam.SaveChangesAsync();
        }

        // 7. Doktorun bu hizmeti sunabildigini kaydet
        bool hizmetAtanmisMi =
            await baglam.CalisanHizmetleri.AnyAsync(x =>
                x.CalisanId == doktor.Id &&
                x.SaglikHizmetiId == hizmet.Id);

        if (!hizmetAtanmisMi)
        {
            baglam.CalisanHizmetleri.Add(new CalisanHizmeti
            {
                CalisanId = doktor.Id,
                SaglikHizmetiId = hizmet.Id,
                KurumId = kurum.Id
            });

            await baglam.SaveChangesAsync();
        }

        // 8. Gelecek bir gun icin doktor uygunlugu
        DateTime uygunGun = DateTime.Today.AddDays(1);

        while (uygunGun.DayOfWeek == DayOfWeek.Sunday)
        {
            uygunGun = uygunGun.AddDays(1);
        }

        DateTime uygunBaslangic = uygunGun.Date.AddHours(9);

        DateTime uygunBitis = uygunGun.Date.AddHours(
            uygunGun.DayOfWeek == DayOfWeek.Saturday
                ? 13
                : 17);

        var uygunluk = await baglam.UygunlukAraliklari
            .FirstOrDefaultAsync(a =>
                a.CalisanId == doktor.Id &&
                a.BaslangicTarihiSaati == uygunBaslangic);

        if (uygunluk is null)
        {
            uygunluk = new UygunlukAraligi
            {
                CalisanId = doktor.Id,
                KurumId = kurum.Id,
                BaslangicTarihiSaati = uygunBaslangic,
                BitisTarihiSaati = uygunBitis
            };

            baglam.UygunlukAraliklari.Add(uygunluk);
            await baglam.SaveChangesAsync();
        }

        Console.WriteLine("Ornek veriler hazirlandi.");
        Console.WriteLine($"Kurum numarasi: {kurum.Id}");
        Console.WriteLine($"Bolum numarasi: {bolum.Id}");
        Console.WriteLine($"Hasta numarasi: {hasta.Id}");
        Console.WriteLine($"Doktor numarasi: {doktor.Id}");
        Console.WriteLine($"Hizmet numarasi: {hizmet.Id}");
        Console.WriteLine($"Uygunluk numarasi: {uygunluk.Id}");
        Console.WriteLine(
            $"Deneme tarihi: {uygunGun:dd.MM.yyyy}");
        Console.WriteLine("Deneme saati: 10:00");
        Console.WriteLine("Hizmet suresi: 30 dakika");
        Console.WriteLine("Hizmet ucreti: 750 TL");
    }
}
