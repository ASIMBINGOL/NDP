
namespace NDPProje.Modeller;

public class CalisanHizmeti
{
    public int CalisanId { get; set; }

    public int SaglikHizmetiId { get; set; }

    public int KurumId { get; set; }

    public Calisan? Calisan { get; set; }

    public SaglikHizmeti? SaglikHizmeti { get; set; }
}
