
namespace NDPProje.Modeller;

public class KurumCalismaSaati
{
    public int Id { get; set; }

    public int KurumId { get; set; }

    // Pazar 0, Pazartesi 1, ... Cumartesi 6
    public int HaftaninGunu { get; set; }

    // Saat bilgisi, ornegin 09:00
    public string? AcilisSaati { get; set; }

    public string? KapanisSaati { get; set; }

    public bool KapaliMi { get; set; }

    // Calisma saatinin ait oldugu kurum
    public Kurum? Kurum { get; set; }
}
