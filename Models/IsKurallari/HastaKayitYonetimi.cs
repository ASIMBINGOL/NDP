
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

public class HastaKayitYonetimi
{
    private readonly VeritabaniBaglami _baglam;

    public HastaKayitYonetimi(VeritabaniBaglami baglam)
    {
        _baglam = baglam;
    }

    public async Task KayitOlAsync(
        string adSoyad,
        string eposta,
        string telefon,
        DateTime dogumTarihi,
        string sifre)
    {
        adSoyad = adSoyad.Trim();
        eposta = eposta.Trim().ToLowerInvariant();
        telefon = telefon.Trim();

        // Girilen bilgileri kontrol et.
        if (adSoyad.Length < 3 || adSoyad.Length > 150)
        {
            throw new InvalidOperationException(
                "Gecerli bir ad soyad giriniz.");
        }

        if (eposta.Length > 254 ||
            !MailAddress.TryCreate(eposta, out var adres) ||
            adres.Address != eposta)
        {
            throw new InvalidOperationException(
                "Gecerli bir e-posta adresi giriniz.");
        }

        if (telefon.Length < 10 || telefon.Length > 30)
        {
            throw new InvalidOperationException(
                "Gecerli bir telefon numarasi giriniz.");
        }

        if (dogumTarihi.Date > DateTime.Today ||
            dogumTarihi.Year < 1900)
        {
            throw new InvalidOperationException(
                "Dogum tarihi gecersiz.");
        }

        if (sifre.Length < 8)
        {
            throw new InvalidOperationException(
                "Sifre en az 8 karakter olmalidir.");
        }

        // Ayni e-posta ikinci kez kaydedilemez.
        bool epostaVarMi = await _baglam.Kullanicilar
            .AnyAsync(k => k.Eposta == eposta);

        if (epostaVarMi)
        {
            throw new InvalidOperationException(
                "Bu e-posta adresi zaten kayitli.");
        }

        // Sifreyi duz metin olarak saklamiyoruz.
        byte[] tuz = RandomNumberGenerator.GetBytes(16);

        byte[] ozet = Rfc2898DeriveBytes.Pbkdf2(
            sifre,
            tuz,
            210_000,
            HashAlgorithmName.SHA256,
            32);

        string sifreOzeti =
            $"PBKDF2-SHA256$210000$" +
            $"{Convert.ToBase64String(tuz)}$" +
            $"{Convert.ToBase64String(ozet)}";

        // Yeni kullanici sadece Hasta rolunde olusturulur.
        var kullanici = new Kullanici
        {
            AdSoyad = adSoyad,
            Eposta = eposta,
            SifreOzeti = sifreOzeti,
            Rol = "Hasta",
            AktifMi = true
        };

        var hasta = new Hasta
        {
            Kullanici = kullanici,
            Telefon = telefon,
            DogumTarihi = dogumTarihi.Date
        };

        // Iki iliskili kayit tek kaydetme isleminde eklenir.
        _baglam.Hastalar.Add(hasta);

        await _baglam.SaveChangesAsync();
    }
}
