
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;
using NDPProje.Veri;
using NDPProje.Modeller;

// ============================================
// 1. ORNEK VERILERI HAZIRLAMA
// ============================================

await OrnekVeriHazirlayici.HazirlaAsync();

await using var baglam = new VeritabaniBaglami();

var randevuYonetimi = new RandevuYonetimi(baglam);

// ============================================
// 2. TEST ICIN KAYITLARI BULMA
// ============================================

var hasta = await baglam.Hastalar
    .OrderBy(h => h.Id)
    .FirstAsync();

var doktor = await baglam.Doktorlar
    .OrderBy(d => d.Id)
    .FirstAsync();

var hizmet = await baglam.SaglikHizmetleri
    .FirstAsync(h =>
        h.KurumId == doktor.KurumId &&
        h.AktifMi);

// Gelecek gunler icin uygunluk araligi sec.
DateTime yarin = DateTime.Today.AddDays(1);

var uygunluk = await baglam.UygunlukAraliklari
    .Where(a =>
        a.CalisanId == doktor.Id &&
        a.BaslangicTarihiSaati >= yarin)
    .OrderBy(a => a.BaslangicTarihiSaati)
    .FirstOrDefaultAsync();

// ============================================
// 3. RANDEVU OLUSTURMA TESTI
// ============================================

Console.WriteLine("\n--- RANDEVU OLUSTURMA TESTI ---");

if (uygunluk is null)
{
    Console.WriteLine(
        "Gelecek tarihli uygunluk araligi bulunamadi.");
}
else
{
    DateTime baslangic =
        uygunluk.BaslangicTarihiSaati.Date.AddHours(10);

    try
    {
        int randevuId =
            await randevuYonetimi.RandevuOlusturAsync(
                hasta.Id,
                doktor.Id,
                hizmet.Id,
                uygunluk.Id,
                baslangic);

        Console.WriteLine(
            "Randevu basariyla olusturuldu!");

        Console.WriteLine(
            $"Randevu numarasi: {randevuId}");

        Console.WriteLine(
            $"Tarih: {baslangic:dd.MM.yyyy HH:mm}");

        Console.WriteLine(
            $"Hizmet: {hizmet.Ad}");

        Console.WriteLine(
            $"Ucret: {hizmet.UcretKurus / 100m:N2} TL");

        Console.WriteLine("Durum: Beklemede");
    }
    catch (InvalidOperationException hata)
    {
        Console.WriteLine(
            $"Randevu olusturulamadi: {hata.Message}");
    }
}

// ============================================
// 4. NESNEYE DAYALI PROGRAMLAMA TESTI
// ============================================

Console.WriteLine(
    "\n--- NESNEYE DAYALI PROGRAMLAMA TESTI ---");

ICalisan[] calisanlar =
{
    new Doktor
    {
        UzmanlikAlani = "Kardiyoloji"
    },

    new SaglikPersoneli
    {
        Gorev = "Hemsire"
    }
};

foreach (ICalisan calisan in calisanlar)
{
    Console.WriteLine(calisan.GorevBilgisi());
}

// ============================================
// 5. RANDEVU TAMAMLAMA TESTI
// ============================================

Console.WriteLine(
    "\n--- RANDEVU TAMAMLAMA TESTI ---");

// Sadece onaylanmis ve bitis zamani gecmis
// randevulari tamamlamayi dene.

var tamamlanacakRandevu = await baglam.Randevular
    .AsNoTracking()
    .Where(r =>
        r.Durum == "Onaylandi" &&
        r.BitisTarihiSaati <= DateTime.Now)
    .OrderBy(r => r.Id)
    .FirstOrDefaultAsync();

if (tamamlanacakRandevu is null)
{
    Console.WriteLine(
        "Tamamlanabilecek onayli randevu bulunamadi.");
}
else
{
    try
    {
        var ilgiliDoktor = await baglam.Doktorlar
            .AsNoTracking()
            .FirstAsync(d =>
                d.Id == tamamlanacakRandevu.CalisanId);

        await randevuYonetimi.RandevuTamamlaAsync(
            tamamlanacakRandevu.Id,
            ilgiliDoktor.KullaniciId);

        Console.WriteLine(
            "Randevu basariyla tamamlandi!");

        var guncelRandevu = await baglam.Randevular
            .AsNoTracking()
            .FirstAsync(r =>
                r.Id == tamamlanacakRandevu.Id);

        Console.WriteLine(
            $"Randevu numarasi: {guncelRandevu.Id}");

        Console.WriteLine(
            $"Guncel durum: {guncelRandevu.Durum}");
    }
    catch (InvalidOperationException hata)
    {
        Console.WriteLine(
            $"Tamamlama islemi: {hata.Message}");
    }
}

// ============================================
// 6. RANDEVU IPTAL ETME TESTI
// ============================================

Console.WriteLine(
    "\n--- RANDEVU IPTAL ETME TESTI ---");

if (uygunluk is null)
{
    Console.WriteLine(
        "Iptal testi icin uygunluk araligi bulunamadi.");
}
else
{
    // Ana randevudan farkli bir saat kullaniyoruz.
    DateTime iptalTestSaati =
        uygunluk.BaslangicTarihiSaati.Date.AddHours(11);

    // Ayni deneme daha once yapildiysa
    // gereksiz yeni kayit olusturma.
    bool iptalTestiYapilmisMi =
        await baglam.Randevular.AnyAsync(r =>
            r.HastaId == hasta.Id &&
            r.CalisanId == doktor.Id &&
            r.BaslangicTarihiSaati == iptalTestSaati &&
            r.Durum == "IptalEdildi");

    if (iptalTestiYapilmisMi)
    {
        Console.WriteLine(
            "Bu tarih icin iptal testi daha once yapilmis.");
    }
    else
    {
        try
        {
            // Once iptal edecegimiz yeni bir
            // deneme randevusu olustur.
            int yeniRandevuId =
                await randevuYonetimi.RandevuOlusturAsync(
                    hasta.Id,
                    doktor.Id,
                    hizmet.Id,
                    uygunluk.Id,
                    iptalTestSaati);

            Console.WriteLine(
                $"Yeni deneme randevusu: {yeniRandevuId}");

            // Hasta kendi randevusunu iptal ediyor.
            var hastaKullaniciId = hasta.KullaniciId;

            await randevuYonetimi.RandevuIptalEtAsync(
                yeniRandevuId,
                hastaKullaniciId);

            Console.WriteLine(
                "Randevu basariyla iptal edildi!");

            var guncelRandevu = await baglam.Randevular
                .AsNoTracking()
                .FirstAsync(r => r.Id == yeniRandevuId);

            Console.WriteLine(
                $"Randevu numarasi: {guncelRandevu.Id}");

            Console.WriteLine(
                $"Guncel durum: {guncelRandevu.Durum}");
        }
        catch (InvalidOperationException hata)
        {
            Console.WriteLine(
                $"Iptal testi: {hata.Message}");
        }
    }
}

// ============================================
// 7. PROGRAMIN TAMAMLANMASI
// ============================================

Console.WriteLine("\nTum testler tamamlandi.");
