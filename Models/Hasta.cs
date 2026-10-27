namespace NDPProje.Modeller;

public class Hasta
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public string? Telefon { get; set; }

    public DateTime? DogumTarihi { get; set; }

    public Kullanici? Kullanici { get; set; }
}
