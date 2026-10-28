
namespace NDPProje.Modeller;

public class Bolum
{
    public int Id { get; set; }

    public int KurumId { get; set; }

    public string Ad { get; set; } = string.Empty;

    public string? Aciklama { get; set; }

    // Bolumun bagli oldugu kurum
    public Kurum? Kurum { get; set; }

    //calisanlarin bölüm bilgisi
    public ICollection<Calisan> Calisanlar { get; set; }
    = new List<Calisan>();

    //hastaalrin tedaviye ödeyecekleri ücretler.
    public ICollection<SaglikHizmeti> SaglikHizmetleri
    { get; set; } = new List<SaglikHizmeti>();
}
