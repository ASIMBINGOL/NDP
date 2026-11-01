using Microsoft.EntityFrameworkCore;
using NDPProje.Veri;

namespace NDPProje.IsKurallari;

// Soyutlama: Bütün yönetim servisleri aynı ekleme/okuma/güncelleme/silme sözleşmesini kullanır.
public interface IYonetimIslemleri<T> where T : class
{
    Task<IReadOnlyList<T>> ListeleAsync();
    Task<int> KaydetAsync(T kayit);
    Task SilAsync(int id);
}

// Kalıtım + kapsülleme: Yetki doğrulama tek merkezden yapılır.
public abstract class YetkiliYonetimServisi<T> : IYonetimIslemleri<T> where T : class
{
    private readonly int _yoneticiKullaniciId;
    protected VeritabaniBaglami Baglam { get; }

    protected YetkiliYonetimServisi(VeritabaniBaglami baglam, int yoneticiKullaniciId)
    {
        Baglam = baglam ?? throw new ArgumentNullException(nameof(baglam));
        _yoneticiKullaniciId = yoneticiKullaniciId;
    }

    protected async Task YoneticiYetkisiniDogrulaAsync()
    {
        bool yetkili = await Baglam.Kullanicilar.AsNoTracking()
            .AnyAsync(k => k.Id == _yoneticiKullaniciId &&
                           k.Rol == "KurumYoneticisi" && k.AktifMi);
        if (!yetkili)
            throw new UnauthorizedAccessException("Bu işlem için aktif yönetici hesabı gerekir.");
    }

    public abstract Task<IReadOnlyList<T>> ListeleAsync();
    public abstract Task<int> KaydetAsync(T kayit);
    public abstract Task SilAsync(int id);
}
