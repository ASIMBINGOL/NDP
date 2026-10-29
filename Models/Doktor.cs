namespace NDPProje.Modeller;

public class Doktor : Calisan
{
    public string UzmanlikAlani { get; set; } = string.Empty;

    public override string GorevBilgisi()
    {
        return $"Doktor - Uzmanlık: {UzmanlikAlani}";
    }
}