#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class HastaPaneli : Form
{
    private int _kullaniciId;
    private string _adSoyad = "";
    private bool _islemDevamEdiyor;

    // Designer'in formu acabilmesi icin parametresiz kurucu.
    public HastaPaneli()
    {
        InitializeComponent();
        OlaylariBagla();
    }

    // Form1'den giris yapan hastanin bilgileri aktarilir.
    public HastaPaneli(int kullaniciId, string adSoyad) : this()
    {
        _kullaniciId = kullaniciId;
        _adSoyad = adSoyad;
        lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
    }

    // Bu alanlar ekranda gorunmez; gercek kayit durumu ve tarih saklanir.
    // Iptal kararini yalnızca ekranda gorunen metne gore vermiyoruz.
    private sealed class RandevuSatiri
    {
        public int RandevuNo { get; init; }
        public string Tarih { get; init; } = "";
        public string Saat { get; init; } = "";
        public string Doktor { get; init; } = "";
        public string Hizmet { get; init; } = "";
        public string Sure { get; init; } = "";
        public string Ucret { get; init; } = "";
        public string Durum { get; init; } = "";

        [Browsable(false)]
        public string KayitDurumu { get; init; } = "";

        [Browsable(false)]
        public DateTime BaslangicTarihiSaati { get; init; }
    }

    private static string GorunenDurum(string kayitDurumu, DateTime baslangic)
    {
        return kayitDurumu == "Beklemede" && baslangic <= DateTime.Now
            ? "Süresi Geçti"
            : kayitDurumu;
    }

    private void OlaylariBagla()
    {
        // Designer'da Click/Load olayi bulunmuyor; burada bir kez baglanir.
        Load += HastaPaneli_Load;
        btnRandevuAra.Click += btnRandevuAra_Click;
        btnYenile.Click += btnYenile_Click;
        btnRandevuIptal.Click += btnRandevuIptal_Click;
        btnCikis.Click += btnCikis_Click;
        dgvRandevular.SelectionChanged += (_, _) => IptalButonunuGuncelle();

        dgvRandevular.ReadOnly = true;
        dgvRandevular.AutoGenerateColumns = true;
        dgvRandevular.AllowUserToAddRows = false;
        dgvRandevular.AllowUserToDeleteRows = false;
        dgvRandevular.MultiSelect = false;
        dgvRandevular.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        IptalButonunuGuncelle();
    }

    private async void HastaPaneli_Load(object? sender, EventArgs e)
    {
        // Designer onizlemesinde veritabanina baglanma.
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            _kullaniciId <= 0)
            return;

        if (await HastaBilgileriniYukleAsync() && !IsDisposed)
            await RandevulariYukleAsync();
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

            if (hasta is null)
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
            lblDogumTarihiDeger.Text = $"{hasta.DogumTarihi:dd.MM.yyyy}";
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
        btnRandevuIptal.Enabled = false;

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

            if (hasta is null)
            {
                dgvRandevular.DataSource = null;
                lblRandevuBilgi.Text = "Hasta kaydı bulunamadı.";
                return;
            }

            // Sadece giris yapan hastanin randevularini getir.
            var randevular = await baglam.Randevular
                .AsNoTracking()
                .Where(r => r.HastaId == hasta.Id)
                .Include(r => r.Calisan)
                    .ThenInclude(c => c!.Kullanici)
                .Include(r => r.SaglikHizmeti)
                .OrderByDescending(r => r.BaslangicTarihiSaati)
                .ToListAsync();

            var gorunum = randevular.Select(r => new RandevuSatiri
            {
                RandevuNo = r.Id,
                Tarih = r.BaslangicTarihiSaati.ToString("dd.MM.yyyy"),
                Saat = r.BaslangicTarihiSaati.ToString("HH:mm"),
                Doktor = r.Calisan?.Kullanici?.AdSoyad ?? "Bilinmiyor",
                Hizmet = r.SaglikHizmeti?.Ad ?? "Bilinmiyor",
                Sure = r.SureDakikaKayit + " dk",
                Ucret = (r.UcretKurusKayit / 100m).ToString("N2") + " TL",
                Durum = GorunenDurum(r.Durum, r.BaslangicTarihiSaati),
                KayitDurumu = r.Durum,
                BaslangicTarihiSaati = r.BaslangicTarihiSaati
            }).ToList();

            dgvRandevular.DataSource = null;
            dgvRandevular.DataSource = gorunum;
            dgvRandevular.ClearSelection();

            lblRandevuBilgi.Text = gorunum.Count == 0
                ? "Henüz randevunuz bulunmuyor. Yeni randevu arayabilirsiniz."
                : $"Toplam {gorunum.Count} randevunuz bulunuyor. İptal için bir satır seçiniz.";
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
                btnYenile.Enabled = !_islemDevamEdiyor;
                IptalButonunuGuncelle();
            }
        }
    }

    private RandevuSatiri? SeciliRandevu()
    {
        if (dgvRandevular.SelectedRows.Count == 0)
            return null;

        return dgvRandevular.SelectedRows[0].DataBoundItem as RandevuSatiri;
    }

    private void IptalButonunuGuncelle()
    {
        if (btnRandevuIptal is null || dgvRandevular is null)
            return;

        var secili = SeciliRandevu();
        btnRandevuIptal.Enabled = !_islemDevamEdiyor &&
            secili is not null &&
            (secili.KayitDurumu == "Beklemede" ||
             secili.KayitDurumu == "Onaylandi") &&
            secili.BaslangicTarihiSaati > DateTime.Now;
    }

    private async void btnRandevuIptal_Click(object? sender, EventArgs e)
    {
        if (_islemDevamEdiyor)
            return;

        var secili = SeciliRandevu();
        if (secili is null)
        {
            MessageBox.Show("Lütfen iptal edilecek randevuyu seçiniz.",
                "Randevu Seçimi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Arayuzde ilk kontrol. Gercek yetki ve tarih kontrolu
        // RandevuYonetimi.RandevuIptalEtAsync icinde de yapilir.
        if ((secili.KayitDurumu != "Beklemede" &&
             secili.KayitDurumu != "Onaylandi") ||
            secili.BaslangicTarihiSaati <= DateTime.Now)
        {
            MessageBox.Show(
                "Yalnızca henüz başlamamış, bekleyen veya onaylanmış randevular iptal edilebilir.",
                "İptal Edilemez", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            IptalButonunuGuncelle();
            return;
        }

        var onay = MessageBox.Show(
            $"{secili.RandevuNo} numaralı, {secili.Tarih} {secili.Saat} " +
            $"tarih ve saatli {secili.Doktor} randevunuzu iptal etmek istiyor musunuz?",
            "Randevu İptal Onayı", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (onay != DialogResult.Yes)
            return;

        _islemDevamEdiyor = true;
        btnYenile.Enabled = false;
        btnRandevuAra.Enabled = false;
        dgvRandevular.Enabled = false;
        IptalButonunuGuncelle();

        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var yonetim = new RandevuYonetimi(baglam);

            // Kullanici kimligi sunucu-tarafindaki is kurallarinda
            // hasta/randevu sahipligini kontrol etmek icin kullanilir.
            await yonetim.RandevuIptalEtAsync(secili.RandevuNo, _kullaniciId);

            MessageBox.Show("Randevunuz başarıyla iptal edildi.",
                "İptal Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!IsDisposed)
                await RandevulariYukleAsync();
        }
        catch (InvalidOperationException hata)
        {
            MessageBox.Show(hata.Message, "Randevu İptali",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (!IsDisposed)
                await RandevulariYukleAsync();
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Randevu iptal edilemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemDevamEdiyor = false;
            if (!IsDisposed)
            {
                btnYenile.Enabled = true;
                btnRandevuAra.Enabled = true;
                dgvRandevular.Enabled = true;
                IptalButonunuGuncelle();
            }
        }
    }

    private async void btnYenile_Click(object? sender, EventArgs e)
    {
        if (_islemDevamEdiyor)
            return;

        btnYenile.Enabled = false;
        try
        {
            if (await HastaBilgileriniYukleAsync() && !IsDisposed)
                await RandevulariYukleAsync();
        }
        finally
        {
            if (!IsDisposed)
                btnYenile.Enabled = true;
        }
    }

    private async void btnRandevuAra_Click(object? sender, EventArgs e)
    {
        if (_islemDevamEdiyor)
            return;

        using var aramaFormu = new RandevuAramaFormu(_kullaniciId);
        if (aramaFormu.ShowDialog(this) == DialogResult.OK && !IsDisposed)
            await RandevulariYukleAsync();
    }

    private void btnCikis_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
