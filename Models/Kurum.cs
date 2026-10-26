
namespace NDPProje.Modeller;

public class Kurum
{
    public int Id { get; set; }

    public int? UstKurumId { get; set; }

    public string Ad { get; set; } = string.Empty;

    public string KurumTuru { get; set; } = "Klinik";

    public string? Adres { get; set; }

    public string? Telefon { get; set; }

    public bool AktifMi { get; set; } = true;

    // Kurumun ust kurumu ve alt kurumlari
    public Kurum? UstKurum { get; set; }

    public ICollection<Kurum> AltKurumlar { get; set; }
        = new List<Kurum>();
}
