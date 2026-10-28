
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

    // Ust kurum ve alt kurum iliskisi
    public Kurum? UstKurum { get; set; }

    public ICollection<Kurum> AltKurumlar { get; set; }
        = new List<Kurum>();

    // Kurumun bolumleri
    public ICollection<Bolum> Bolumler { get; set; }
        = new List<Bolum>();

    // Kurumun calisma saatleri
    public ICollection<KurumCalismaSaati> CalismaSaatleri
        { get; set; } = new List<KurumCalismaSaati>();
    
    //Calisanlarin Bİlgisi tek sınıfta
    public ICollection<Calisan> Calisanlar { get; set; }
    = new List<Calisan>();

    //hastalarin ödeyecekleri ücretleri belirlemek için
    public ICollection<SaglikHizmeti> SaglikHizmetleri
    { get; set; } = new List<SaglikHizmeti>();
}
