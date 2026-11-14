
using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

namespace NDP_Proje_Uygulama;

public static class VeritabaniErisimi
{
    public static VeritabaniBaglami BaglamOlustur()
    {
        var secenekler =
            new DbContextOptionsBuilder<VeritabaniBaglami>()
                .UseSqlite(
                    VeritabaniDosyasi.BaglantiCumlesiOlustur())
                .Options;

        return new VeritabaniBaglami(secenekler);
    }
}
