
namespace NDPProje.Modeller;

public class Kullanici
{
    public int Id { get; set; }

    public string AdSoyad { get; set; } = string.Empty;

    public string Eposta { get; set; } = string.Empty;

    // Burada acik parola degil, guvenli parola ozeti tutulacak.
    public string SifreOzeti { get; set; } = string.Empty;

    public string Rol { get; set; } = "Hasta";

    public bool AktifMi { get; set; } = true;

    public Hasta? HastaProfili { get; set; }

    public ICollection<Calisan> Calisanliklari { get; set; }
        = new List<Calisan>();
}
