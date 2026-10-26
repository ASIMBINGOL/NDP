
using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

using var baglam = new VeritabaniBaglami();

baglam.Database.Migrate();

Console.WriteLine("Veritabani hazir: NDP_Projesi.db");
Console.WriteLine(
    $"Dosya konumu: {Path.GetFullPath("NDP_Projesi.db")}");
