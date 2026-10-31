using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace NDP_Proje_Uygulama;

public partial class HastaPaneli : Form
{
    private int _kullaniciId;
    private string _adSoyad = "";

    // Visual Studio Designer'in formu acabilmesi icin parametresiz kurucu.
    public HastaPaneli()
    {
        InitializeComponent();
        OlaylariBagla();
    }

    // Form1'den giris yapan hastanin bilgilerini alan kurucu.
    public HastaPaneli(int kullaniciId, string adSoyad) : this()
    {
        _kullaniciId = kullaniciId;
        _adSoyad = adSoyad;
        lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
    }

    private void OlaylariBagla()
    {
        // Tasarimci dosyasinda olay baglamiyoruz; hepsi burada bir kez baglanir.
        Load += HastaPaneli_Load;
        btnRandevuAra.Click += btnRandevuAra_Click;
        btnYenile.Click += btnYenile_Click;
        btnCikis.Click += btnCikis_Click;
    }

    private async void HastaPaneli_Load(object? sender, EventArgs e)
    {
        // Designer'da veritabanina baglanmaya calismasin.
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            _kullaniciId <= 0)
        {
            return;
        }

        if (await HastaBilgileriniYukleAsync() && !IsDisposed)
        {
            await RandevulariYukleAsync();
        }
    }

    private async Task<bool> HastaBilgileriniYukleAsync()
    {
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();

            var hasta = await baglam.Hastalar
                .AsNoTracking()
                .Include(h => h.Kullanici)
                .FirstOrDefaultAsync(h =>
                    h.KullaniciId == _kullaniciId &&
                    h.Kullanici != null &&
                    h.Kullanici.AktifMi &&
                    h.Kullanici.Rol == "Hasta");

            if (hasta == null)
            {
                MessageBox.Show(
                    "Hasta hesabınız bulunamadı veya kullanıma kapatılmış.",
                    "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return false;
            }

            lblKarsilama.Text = $"Hoş geldiniz, {hasta.Kullanici!.AdSoyad}";
            lblAdSoyadDeger.Text = hasta.Kullanici.AdSoyad;
            lblEpostaDeger.Text = hasta.Kullanici.Eposta;
            lblTelefonDeger.Text = hasta.Telefon ?? "-";
            lblDogumTarihiDeger.Text =
    $"{hasta.DogumTarihi:dd.MM.yyyy}";
            return true;
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Kişisel bilgileriniz yüklenemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private async Task RandevulariYukleAsync()
    {
        btnYenile.Enabled = false;

        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();

            var hasta = await baglam.Hastalar
                .AsNoTracking()
                .FirstOrDefaultAsync(h =>
                    h.KullaniciId == _kullaniciId &&
                    h.Kullanici != null &&
                    h.Kullanici.AktifMi &&
                    h.Kullanici.Rol == "Hasta");

            if (hasta == null)
            {
                dgvRandevular.DataSource = null;
                lblRandevuBilgi.Text = "Hasta kaydı bulunamadı.";
                return;
            }

            var randevular = await baglam.Randevular
                .AsNoTracking()
                .Where(r => r.HastaId == hasta.Id)
                .Include(r => r.Calisan)
                    .ThenInclude(c => c!.Kullanici)
                .Include(r => r.SaglikHizmeti)
                .OrderByDescending(r => r.BaslangicTarihiSaati)
                .ToListAsync();

            var gorunum = randevular.Select(r => new
            {
                RandevuNo = r.Id,
                Tarih = r.BaslangicTarihiSaati.ToString("dd.MM.yyyy"),
                Saat = r.BaslangicTarihiSaati.ToString("HH:mm"),
                Doktor = r.Calisan?.Kullanici?.AdSoyad ?? "Bilinmiyor",
                Hizmet = r.SaglikHizmeti?.Ad ?? "Bilinmiyor",
                Sure = r.SureDakikaKayit + " dk",
                Ucret = (r.UcretKurusKayit / 100m).ToString("N2") + " TL",
                Durum = r.Durum
            }).ToList();

            dgvRandevular.DataSource = null;
            dgvRandevular.DataSource = gorunum;
            dgvRandevular.ClearSelection();

            lblRandevuBilgi.Text = gorunum.Count == 0
                ? "Henüz randevunuz bulunmuyor. Yeni randevu arayabilirsiniz."
                : $"Toplam {gorunum.Count} randevunuz bulunuyor.";
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Randevularınız yüklenirken bir hata oluştu.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed)
            {
                btnYenile.Enabled = true;
            }
        }
    }

    private async void btnYenile_Click(object? sender, EventArgs e)
    {
        btnYenile.Enabled = false;
        try
        {
            if (await HastaBilgileriniYukleAsync() && !IsDisposed)
            {
                await RandevulariYukleAsync();
            }
        }
        finally
        {
            if (!IsDisposed)
            {
                btnYenile.Enabled = true;
            }
        }
    }

    private async void btnRandevuAra_Click(object? sender, EventArgs e)
    {
        // RandevuArama_Designer_Hazir.zip ile olusturulan form kullanilir.
        using var aramaFormu = new RandevuAramaFormu(_kullaniciId);

        if (aramaFormu.ShowDialog(this) == DialogResult.OK && !IsDisposed)
        {
            await RandevulariYukleAsync();
        }
    }

    private void btnCikis_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
