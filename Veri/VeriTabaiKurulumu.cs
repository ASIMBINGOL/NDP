using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace NDPProje.Veri;

public static class VeritabaniKurulumu
{
    public static void Hazirla()
    {
        var baglanti = new SqliteConnectionStringBuilder
        {
            DataSource = VeritabaniDosyasi.DosyaYolunuBul(),
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        var secenekler =
            new DbContextOptionsBuilder<VeritabaniBaglami>()
                .UseSqlite(baglanti.ToString())
                .Options;

        using var baglam = new VeritabaniBaglami(secenekler);

        if (!baglam.Database.GetMigrations().Any())
        {
            throw new InvalidOperationException(
                "Veri tabanı geçişleri bulunamadı. " +
                "Migrations dosyalarının projeye dahil olduğunu kontrol edin.");
        }

        // Veri tabanı yoksa oluşturur.
        // Varsa henüz uygulanmamış geçişleri uygular.
        baglam.Database.Migrate();
    }
}