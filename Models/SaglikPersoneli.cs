namespace NDPProje.Modeller;

public class SaglikPersoneli : Calisan
{
    public string Gorev { get; set; } = string.Empty;

    public override string GorevBilgisi()
    {
        return $"Sağlık Personeli - Görev: {Gorev}";
    }
}
