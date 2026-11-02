#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace NDP_Proje_Uygulama;

// Yetki doğrulaması hem panel açılışında hem yönetim servislerinde yapılır.
public partial class AdminPaneli : Form
{
    private int _kullaniciId;
    private string _adSoyad = "";
    private bool _yukleniyor;

    // Designer için parametresiz yapıcı.
    public AdminPaneli()
    {
        InitializeComponent();
        Load += AdminPaneli_Load;
        btnKurumlar.Click += async (_, _) => await KurumEkraniniAcAsync();
        btnBolumler.Click += async (_, _) => await BolumEkraniniAcAsync();
        btnCalisanlar.Click += async (_, _) => await CalisanEkraniniAcAsync();
        btnHizmetler.Click += async (_, _) => await HizmetEkraniniAcAsync();
        btnCalisma.Click += async (_, _) => await CalismaEkraniniAcAsync();
        btnUygunluk.Click += async (_, _) => await UygunlukEkraniniAcAsync();
        btnYenile.Click += async (_, _) => await OzetiYukleAsync();
        btnCikis.Click += (_, _) => Close();
    }

    public AdminPaneli(int kullaniciId, string adSoyad) : this()
    {
        _kullaniciId = kullaniciId;
        _adSoyad = adSoyad;
        lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
    }

    private async void AdminPaneli_Load(object? sender, EventArgs e)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || _kullaniciId <= 0)
            return;
        await OzetiYukleAsync();
    }

    private async Task<bool> YetkiliMiAsync()
    {
        await using var baglam = VeritabaniErisimi.BaglamOlustur();
        return await baglam.Kullanicilar.AsNoTracking()
            .AnyAsync(k => k.Id == _kullaniciId && k.AktifMi && k.Rol == "KurumYoneticisi");
    }

    private async Task OzetiYukleAsync()
    {
        if (_yukleniyor) return;
        _yukleniyor = true;
        btnYenile.Enabled = false;
        try
        {
            if (!await YetkiliMiAsync())
            {
                MessageBox.Show("Aktif yönetici hesabınız bulunamadı.", "Erişim Reddedildi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            int kurum = await baglam.Kurumlar.CountAsync(k => k.AktifMi);
            int bolum = await baglam.Bolumler.CountAsync();
            int doktor = await baglam.Doktorlar.CountAsync(d => d.AktifMi);
            int hizmet = await baglam.SaglikHizmetleri.CountAsync(h => h.AktifMi);
            if (IsDisposed) return;
            lblKurumOzet.Text = $"Aktif kurum: {kurum}";
            lblBolumOzet.Text = $"Bölüm: {bolum}";
            lblDoktorOzet.Text = $"Aktif doktor: {doktor}";
            lblHizmetOzet.Text = $"Aktif hizmet: {hizmet}";
        }
        catch (Exception)
        {
            MessageBox.Show("Yönetici özeti yüklenemedi. Veritabanı bağlantısını kontrol edin.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _yukleniyor = false;
            if (!IsDisposed) btnYenile.Enabled = true;
        }
    }

    private async Task KurumEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new KurumYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task BolumEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new BolumYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task CalisanEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new CalisanYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task HizmetEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new SaglikHizmetiYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task CalismaEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new CalismaSaatiYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task UygunlukEkraniniAcAsync()
    {
        if (!await GuvenliYetkiKontroluAsync()) return;
        using var form = new UygunlukYonetimFormu(_kullaniciId);
        form.ShowDialog(this);
        await OzetiYukleAsync();
    }

    private async Task<bool> GuvenliYetkiKontroluAsync()
    {
        try
        {
            if (await YetkiliMiAsync()) return true;
            MessageBox.Show("Bu işlem için aktif yönetici hesabı gerekir.", "Erişim Reddedildi");
            Close();
            return false;
        }
        catch (Exception)
        {
            MessageBox.Show("Yönetici yetkisi doğrulanamadı.", "Hata");
            return false;
        }
    }

    private static void HenuzHazirDegil(string baslik)
    {
        MessageBox.Show($"{baslik} modülünü sonraki aşamada ekleyeceğiz.\n" +
                        "Bu bölüm henüz hazırlanmadı.",
            "Modül Geliştirme Aşamasında", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
