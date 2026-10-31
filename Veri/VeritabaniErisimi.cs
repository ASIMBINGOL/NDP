
using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

namespace NDP_Proje_Uygulama;

public static class VeritabaniErisimi
{
    private static readonly string DosyaYolu =
        @"C:\Users\asimb\NDPProje\NDP_Projesi.db";

    public static VeritabaniBaglami BaglamOlustur()
    {
        if (!File.Exists(DosyaYolu))
        {
            throw new FileNotFoundException(
                "Mevcut veritabani dosyasi bulunamadi.",
                DosyaYolu);
        }

        var secenekler =
            new DbContextOptionsBuilder<VeritabaniBaglami>()
                .UseSqlite($"Data Source={DosyaYolu}")
                .Options;

        return new VeritabaniBaglami(secenekler);
    }
}
