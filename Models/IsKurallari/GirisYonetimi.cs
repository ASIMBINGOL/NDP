
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Giris yapan kullanicinin temel bilgileri.
// Parola ozeti bu sonuc nesnesinde tutulmaz.
public record GirisSonucu(
    int KullaniciId,
    string AdSoyad,
    string Rol
);

public class GirisYonetimi
{
    private readonly VeritabaniBaglami _baglam;

    public GirisYonetimi(VeritabaniBaglami baglam)
    {
        _baglam = baglam;
    }

    // E-posta ve sifre ile kullanici girisi.
    public async Task<GirisSonucu> GirisYapAsync(
        string eposta,
        string sifre)
    {
        eposta = (eposta ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(eposta) ||
            string.IsNullOrEmpty(sifre))
        {
            throw new InvalidOperationException(
                "E-posta ve sifre alanlarini doldurunuz.");
        }

        // Kullanici bilgilerini veritabanindan oku.
        var kullanici = await _baglam.Kullanicilar
            .AsNoTracking()
            .FirstOrDefaultAsync(k =>
                k.Eposta == eposta &&
                k.AktifMi);

        // Hesap yoksa veya sifre yanlissa girise izin verme.
        if (kullanici is null ||
            !SifreDogrula(sifre, kullanici.SifreOzeti))
        {
            throw new InvalidOperationException(
                "E-posta veya sifre hatali.");
        }

        // Giris yapan kullanicinin bilgilerini dondur.
        return new GirisSonucu(
            kullanici.Id,
            kullanici.AdSoyad,
            kullanici.Rol
        );
    }

    // Kayit asamasinda olusturdugumuz
    // PBKDF2-SHA256 sifre ozetini dogrula.
    private static bool SifreDogrula(
        string sifre,
        string kayitliOzet)
    {
        if (string.IsNullOrWhiteSpace(kayitliOzet))
        {
            return false;
        }

        // Kayit bicimi:
        // PBKDF2-SHA256$210000$tuz$ozet
        string[] parcalar = kayitliOzet.Split('$');

        if (parcalar.Length != 4 ||
            parcalar[0] != "PBKDF2-SHA256" ||
            parcalar[1] != "210000")
        {
            return false;
        }

        try
        {
            byte[] tuz = Convert.FromBase64String(
                parcalar[2]);

            byte[] kayitliSifreOzeti =
                Convert.FromBase64String(parcalar[3]);

            if (tuz.Length != 16 ||
                kayitliSifreOzeti.Length != 32)
            {
                return false;
            }

            // Girilen sifre icin ayni islemi uygula.
            byte[] hesaplananOzet =
                Rfc2898DeriveBytes.Pbkdf2(
                    sifre,
                    tuz,
                    210_000,
                    HashAlgorithmName.SHA256,
                    32);

            // Sabit zamanli karsilastirma.
            return CryptographicOperations.FixedTimeEquals(
                kayitliSifreOzeti,
                hesaplananOzet);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
