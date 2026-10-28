
namespace NDPProje.Modeller;

public class Randevu
{
    public int Id { get; set; }

    public int HastaId { get; set; }

    public int CalisanId { get; set; }

    public int SaglikHizmetiId { get; set; }

    public int KurumId { get; set; }

    public int UygunlukAraligiId { get; set; }

    public DateTime BaslangicTarihiSaati { get; set; }

    public DateTime BitisTarihiSaati { get; set; }

    // Randevu alindigi andaki sure ve ucret
    public int SureDakikaKayit { get; set; }

    public long UcretKurusKayit { get; set; }

    public string Durum { get; set; } = "Beklemede";

    public Hasta? Hasta { get; set; }

    public Calisan? Calisan { get; set; }

    public SaglikHizmeti? SaglikHizmeti { get; set; }

    public UygunlukAraligi? UygunlukAraligi { get; set; }
}
