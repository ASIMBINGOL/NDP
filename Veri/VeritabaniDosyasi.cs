
using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace NDPProje.Veri;

public static class VeritabaniDosyasi
{
    public static string DosyaYolunuBul()
    {
        DirectoryInfo? klasor =
            new DirectoryInfo(AppContext.BaseDirectory);

        // Konsol ve Windows Forms uygulamalarının
        // ortak ana proje klasörünü bul.
        while (klasor != null)
        {
            string projeDosyasi = Path.Combine(
                klasor.FullName,
                "NDPProje.csproj");

            if (File.Exists(projeDosyasi))
            {
                string veritabaniYolu = Path.Combine(
                    klasor.FullName,
                    "NDP_Projesi.db");

                if (!File.Exists(veritabaniYolu))
                {
                    throw new FileNotFoundException(
                        "Proje klasöründe veritabanı bulunamadı.",
                        veritabaniYolu);
                }

                return veritabaniYolu;
            }

            klasor = klasor.Parent;
        }

        throw new DirectoryNotFoundException(
            "Ana NDPProje klasörü bulunamadı.");
    }

    public static string BaglantiCumlesiOlustur()
    {
        var baglanti = new SqliteConnectionStringBuilder
        {
            DataSource = DosyaYolunuBul(),
            Mode = SqliteOpenMode.ReadWrite
        };

        return baglanti.ToString();
    }
}
