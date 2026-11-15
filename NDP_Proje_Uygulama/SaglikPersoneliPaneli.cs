using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class SaglikPersoneliPaneli : Form
{
    private int _kullaniciId;
    private string _adSoyad = "";
    private bool _filtrelerHazirlaniyor;
    private bool _islemDevamEdiyor;
    private List<PersonelRandevuSatiri> _randevular = new();
    private readonly CompareInfo _turkceKarsilastirma =
        CultureInfo.GetCultureInfo("tr-TR").CompareInfo;

    // Visual Studio Form Tasarımcısı için.
    public SaglikPersoneliPaneli()
    {
        InitializeComponent();
        OlaylariBagla();
    }

    // Giriş ekranından kimliği doğrulanmış kullanıcının Id'si gelir.
    public SaglikPersoneliPaneli(int kullaniciId, string adSoyad) : this()
    {
        _kullaniciId = kullaniciId;
        _adSoyad = adSoyad;
        lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
    }

    private void OlaylariBagla()
    {
        _filtrelerHazirlaniyor = true;
        cmbDurum.Items.AddRange(new object[]
        {
            "Tümü", "Geldi", "Gelmedi", "Beklemede", "Onaylandi", "IptalEdildi", "Tamamlandi"
        });
        cmbDurum.SelectedIndex = 0;
        dtpTarih.Value = DateTime.Today;
        _filtrelerHazirlaniyor = false;

        Load += SaglikPersoneliPaneli_Load;
        btnHastaGeldi.Click += btnHastaGeldi_Click;
        btnYenile.Click += async (_, _) => await RandevulariYenileAsync();
        btnCikis.Click += (_, _) => Close();
        cmbKurum.SelectedIndexChanged += async (_, _) =>
        {
            if (!_filtrelerHazirlaniyor)
                await RandevulariYenileAsync();
        };
        dtpTarih.ValueChanged += async (_, _) =>
        {
            if (!_filtrelerHazirlaniyor)
                await RandevulariYenileAsync();
        };
        cmbDurum.SelectedIndexChanged += (_, _) => FiltreleriUygula();
        txtAra.TextChanged += (_, _) => FiltreleriUygula();
        dgvRandevular.SelectionChanged += (_, _) => IslemButonlariniGuncelle();
        IslemButonlariniGuncelle();
    }

    private async void SaglikPersoneliPaneli_Load(object? sender, EventArgs e)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || _kullaniciId <= 0)
            return;

        try
        {
            _filtrelerHazirlaniyor = true;
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new SaglikPersoneliRandevuServisi(baglam, _kullaniciId);
            var kurumlar = await servis.KurumlariListeleAsync();

            var secenekler = new List<PersonelKurumSecenegi>
            {
                new() { Id = 0, Ad = "Tüm görevli hastaneler" }
            };
            secenekler.AddRange(kurumlar);
            cmbKurum.DisplayMember = nameof(PersonelKurumSecenegi.Ad);
            cmbKurum.ValueMember = nameof(PersonelKurumSecenegi.Id);
            cmbKurum.DataSource = secenekler;
            cmbKurum.SelectedIndex = kurumlar.Count == 1 ? 1 : 0;
            lblKurum.Text = "Görevli Hastane / Klinik: " +
                string.Join(", ", kurumlar.Select(k => k.Ad));
            lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
        }
        catch (UnauthorizedAccessException hata)
        {
            MessageBox.Show(hata.Message, "Erişim Reddedildi",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        catch (Exception)
        {
            MessageBox.Show("Sağlık personeli bilgileri yüklenemedi. Veritabanını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }
        finally
        {
            _filtrelerHazirlaniyor = false;
        }
        if (!IsDisposed)
            await RandevulariYenileAsync();
    }

    private int? SeciliKurumId()
    {
        return cmbKurum.SelectedItem is PersonelKurumSecenegi secenek && secenek.Id > 0
            ? secenek.Id : null;
    }

    private async Task RandevulariYenileAsync()
    {
        if (IsDisposed || _islemDevamEdiyor || _filtrelerHazirlaniyor ||
            cmbKurum.DataSource is null)
            return;

        _islemDevamEdiyor = true;
        btnYenile.Enabled = false;
        btnHastaGeldi.Enabled = false;
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new SaglikPersoneliRandevuServisi(baglam, _kullaniciId);
            _randevular = (await servis.RandevulariListeleAsync(
                dtpTarih.Value.Date, SeciliKurumId())).ToList();
            FiltreleriUygula();
        }
        catch (UnauthorizedAccessException hata)
        {
            MessageBox.Show(hata.Message, "Erişim Reddedildi",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
        }
        catch (Exception)
        {
            MessageBox.Show("Randevular yüklenemedi. Veritabanı bağlantısını veya migration durumunu kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemDevamEdiyor = false;
            if (!IsDisposed)
            {
                btnYenile.Enabled = true;
                IslemButonlariniGuncelle();
            }
        }
    }

    private bool MetinIceriyor(string kaynak, string aranan) =>
        _turkceKarsilastirma.IndexOf(kaynak ?? "", aranan,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

    private void FiltreleriUygula()
    {
        if (IsDisposed || _filtrelerHazirlaniyor)
            return;

        string filtre = cmbDurum.SelectedItem?.ToString() ?? "Tümü";
        string arama = txtAra.Text.Trim();

        var gorunen = _randevular.Where(r =>
            (filtre == "Tümü" ||
                filtre == "Geldi" && r.HastaGelisTarihiSaati.HasValue ||
                filtre == "Gelmedi" && !r.HastaGelisTarihiSaati.HasValue ||
                r.Durum == filtre) &&
            (arama.Length == 0 ||
                MetinIceriyor(r.RandevuNo.ToString(), arama) ||
                MetinIceriyor(r.HastaAdi, arama) ||
                MetinIceriyor(r.DoktorAdi, arama) ||
                MetinIceriyor(r.BolumAdi, arama) ||
                MetinIceriyor(r.HizmetAdi, arama)))
            .ToList();

        dgvRandevular.DataSource = null;
        dgvRandevular.DataSource = gorunen;
        dgvRandevular.ClearSelection();
        dgvRandevular.CurrentCell = null;

        // İstatistikler seçili hastane/tarihin tüm kayıtlarından hesaplanır.
        lblToplam.Text = $"Günlük Toplam\n{_randevular.Count}";
        lblBekleyen.Text = $"Henüz Gelmeyen\n{_randevular.Count(r => !r.HastaGelisTarihiSaati.HasValue && (r.Durum == "Beklemede" || r.Durum == "Onaylandi"))}";
        lblGelen.Text = $"Gelen Hasta\n{_randevular.Count(r => r.HastaGelisTarihiSaati.HasValue)}";
        lblListeBilgisi.Text = gorunen.Count == 0
            ? "Bu filtrelerde randevu bulunamadı."
            : dtpTarih.Value.Date == DateTime.Today
                ? $"{gorunen.Count} randevu gösteriliyor. Hasta gelişi için satır seçiniz."
                : $"{gorunen.Count} randevu gösteriliyor. Hasta Geldi işlemi yalnızca bugünkü randevularda yapılabilir.";
        IslemButonlariniGuncelle();
    }

    private PersonelRandevuSatiri? SeciliRandevu() =>
        dgvRandevular.SelectedRows.Count > 0
            ? dgvRandevular.SelectedRows[0].DataBoundItem as PersonelRandevuSatiri
            : null;

    private void IslemButonlariniGuncelle()
    {
        if (IsDisposed)
            return;

        var r = SeciliRandevu();
        btnHastaGeldi.Enabled = !_islemDevamEdiyor && r is not null &&
            r.HastaGelisTarihiSaati is null &&
            (r.Durum == "Beklemede" || r.Durum == "Onaylandi") &&
            r.BaslangicTarihiSaati.Date == DateTime.Today &&
            r.BitisTarihiSaati > DateTime.Now;
    }

    private async void btnHastaGeldi_Click(object? sender, EventArgs e)
    {
        if (_islemDevamEdiyor)
            return;
        var r = SeciliRandevu();
        if (r is null)
            return;

        if (MessageBox.Show(
            $"{r.HastaAdi} adlı hastanın {r.RandevuNo} numaralı randevuya geldiğini kaydetmek istiyor musunuz?",
            "Hasta Geliş Onayı", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        _islemDevamEdiyor = true;
        btnHastaGeldi.Enabled = false;
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new SaglikPersoneliRandevuServisi(baglam, _kullaniciId);
            await servis.HastaGeldiIsaretleAsync(r.RandevuNo);
            MessageBox.Show("Hasta gelişi kaydedildi. Randevunun onay durumu değiştirilmedi.",
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception hata) when (hata is InvalidOperationException ||
                                    hata is UnauthorizedAccessException)
        {
            MessageBox.Show(hata.Message, "İşlem Reddedildi",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("Hasta gelişi kaydedilemedi. Veritabanını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemDevamEdiyor = false;
            if (!IsDisposed)
                await RandevulariYenileAsync();
        }
    }
}
