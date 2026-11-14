
using NDPProje.Veri;

// Ortak veritabanı dosyasının yolunu göster.
Console.WriteLine("Kullanılan veritabanı:");
Console.WriteLine(VeritabaniDosyasi.DosyaYolunuBul());

Console.WriteLine();

// Veritabanını yalnızca okumak için aç.
using var baglam = new VeritabaniBaglami();

Console.WriteLine("--- VERİTABANI KONTROLÜ ---");

Console.WriteLine(
    $"Kurum sayısı: {baglam.Kurumlar.Count()}");

Console.WriteLine(
    $"Bölüm sayısı: {baglam.Bolumler.Count()}");

Console.WriteLine(
    $"Hasta sayısı: {baglam.Hastalar.Count()}");

Console.WriteLine(
    $"Doktor sayısı: {baglam.Doktorlar.Count()}");

Console.WriteLine(
    $"Randevu sayısı: {baglam.Randevular.Count()}");

Console.WriteLine();
Console.WriteLine("Veritabanı kontrolü tamamlandı.");
