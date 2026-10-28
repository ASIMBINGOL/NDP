
namespace NDPProje.Modeller;

public abstract class Calisan
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public int KurumId { get; set; }

    public int? BolumId { get; set; }

    public bool AktifMi { get; set; } = true;

    public Kullanici? Kullanici { get; set; }

    public Kurum? Kurum { get; set; }

    public Bolum? Bolum { get; set; }

    //kurum, bölüm ve çalışan ile sağlık hizmetlerine erişecek
    public ICollection<CalisanHizmeti> SunduguHizmetler
    { get; set; } = new List<CalisanHizmeti>();
}
