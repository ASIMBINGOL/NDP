
namespace NDPProje.Modeller;

public class UygunlukAraligi
{
    public int Id { get; set; }

    public int CalisanId { get; set; }

    public int KurumId { get; set; }

    public DateTime BaslangicTarihiSaati { get; set; }

    public DateTime BitisTarihiSaati { get; set; }

    public Calisan? Calisan { get; set; }

    public ICollection<Randevu> Randevular { get; set; }
        = new List<Randevu>();
}
