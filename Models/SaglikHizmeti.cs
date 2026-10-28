
namespace NDPProje.Modeller;

public class SaglikHizmeti
{
    public int Id { get; set; }

    public int KurumId { get; set; }

    public int? BolumId { get; set; }

    public string Ad { get; set; } = string.Empty;

    public string? Aciklama { get; set; }

    public int SureDakika { get; set; }

    // Ucret kurus cinsinden tutulur.
    public long UcretKurus { get; set; }

    public bool AktifMi { get; set; } = true;

    public Kurum? Kurum { get; set; }

    public Bolum? Bolum { get; set; }

    public ICollection<CalisanHizmeti> CalisanAtamalari
        { get; set; } = new List<CalisanHizmeti>();
}
