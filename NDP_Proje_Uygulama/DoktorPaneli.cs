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

// Doktorun sadece kendi randevularini gormesini ve yonetmesini saglar.
// Tasarim kodu DoktorPaneli.Designer.cs dosyasindadir.
public partial class DoktorPaneli : Form
{
    private int _kullaniciId;
    private string _adSoyad = "";
    private List<int> _doktorKayitIdleri = new();
    private List<RandevuSatiri> _tumRandevular = new();
    private bool _islemDevamEdiyor;

    // Visual Studio Designer icin parametresiz kurucu.
    public DoktorPaneli()
    {
        InitializeComponent();
        OlaylariBagla();
    }

    // Form1'den giris yapan doktorun bilgileri gelir.
    public DoktorPaneli(int kullaniciId, string adSoyad) : this()
    {
        _kullaniciId = kullaniciId;
        _adSoyad = adSoyad;
        lblKarsilama.Text = $"Hoş geldiniz, {_adSoyad}";
    }

    // Designer'daki otomatik olusturulan sutunlardan gizlenecek alanlar.
    private sealed class RandevuSatiri
    {
        public int RandevuNo { get; init; }
        public string Hasta { get; init; } = "";
        public string Telefon { get; init; } = "";
        public string Kurum { get; init; } = "";
        public string Hizmet { get; init; } = "";
        public string Tarih { get; init; } = "";
        public string Saat { get; init; } = "";
        public string BitisSaati { get; init; } = "";
        public string Ucret { get; init; } = "";
        public string Durum { get; init; } = "";

        [Browsable(false)]
        public DateTime BaslangicTarihiSaati { get; init; }

        [Browsable(false)]
        public DateTime BitisTarihiSaati { get; init; }
    }

    private void OlaylariBagla()
    {
        dgvRandevular.ReadOnly = true;
        dgvRandevular.AutoGenerateColumns = true;
        dgvRandevular.AllowUserToAddRows = false;
        dgvRandevular.AllowUserToDeleteRows = false;
        dgvRandevular.RowHeadersVisible = false;
        dgvRandevular.MultiSelect = false;
        dgvRandevular.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRandevular.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        cmbDurumFiltresi.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDurumFiltresi.Items.AddRange(new object[]
        {
            "Tümü", "Beklemede", "Onaylandi", "IptalEdildi", "Tamamlandi"
        });
        cmbDurumFiltresi.SelectedIndex = 0;

        // Olaylar yalniz burada baglanir; Designer dosyasinda Click/Load yoktur.
        Load += DoktorPaneli_Load;
        btnYenile.Click += btnYenile_Click;
        btnOnayla.Click += btnOnayla_Click;
        btnIptal.Click += btnIptal_Click;
        btnTamamla.Click += btnTamamla_Click;
        btnCikis.Click += (_, _) => Close();
        cmbDurumFiltresi.SelectedIndexChanged += (_, _) => ListeyiFiltrele();
        dgvRandevular.SelectionChanged += (_, _) => IslemButonlariniGuncelle();

        IslemButonlariniGuncelle();
    }

    private async void DoktorPaneli_Load(object? sender, EventArgs e)
    {
        // Designer onizlemesinde DB baglantisi yapilmaz.
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            _kullaniciId <= 0)
            return;

        if (await DoktorBilgileriniYukleAsync() && !IsDisposed)
            await RandevulariYukleAsync();
    }

    private async Task<bool> DoktorBilgileriniYukleAsync()
    {
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();

            var doktorlar = await baglam.Doktorlar.AsNoTracking()
                .Where(d => d.KullaniciId == _kullaniciId && d.AktifMi &&
                            d.Kullanici != null && d.Kullanici.AktifMi &&
                            d.Kullanici.Rol == "Doktor" &&
                            d.Kurum != null && d.Kurum.AktifMi)
                .Select(d => new
                {
                    d.Id,
                    d.KurumId,
                    d.UzmanlikAlani,
                    AdSoyad = d.Kullanici!.AdSoyad
                })
                .ToListAsync();

            if (doktorlar.Count == 0)
            {
                MessageBox.Show(
                    "Bu kullanıcıya ait aktif bir doktor profili bulunamadı.",
                    "Erişim Reddedildi", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
                return false;
            }

            _doktorKayitIdleri = doktorlar.Select(d => d.Id).Distinct().ToList();
            var kurumIdleri = doktorlar.Select(d => d.KurumId).Distinct().ToList();
            var kurumAdlari = await baglam.Kurumlar.AsNoTracking()
                .Where(k => kurumIdleri.Contains(k.Id))
                .OrderBy(k => k.Ad)
                .Select(k => k.Ad)
                .ToListAsync();

            lblKarsilama.Text = $"Hoş geldiniz, {doktorlar[0].AdSoyad}";
            lblKurumDeger.Text = kurumAdlari.Count == 0
                ? "-"
                : string.Join(", ", kurumAdlari);
            lblUzmanlikDeger.Text = string.Join(", ", doktorlar
                .Select(d => d.UzmanlikAlani)
                .Where(ad => !string.IsNullOrWhiteSpace(ad))
                .Distinct());

            return true;
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Doktor bilgileriniz yüklenemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private async Task RandevulariYukleAsync()
    {
        btnYenile.Enabled = false;
        try
        {
            if (_doktorKayitIdleri.Count == 0)
                return;

            await using var baglam = VeritabaniErisimi.BaglamOlustur();

            // Yalnizca giris yapan doktora ait randevular.
            var randevular = await baglam.Randevular.AsNoTracking()
                .Where(r => _doktorKayitIdleri.Contains(r.CalisanId))
                .Include(r => r.Hasta)
                    .ThenInclude(h => h!.Kullanici)
                .Include(r => r.SaglikHizmeti)
                .OrderByDescending(r => r.BaslangicTarihiSaati)
                .ToListAsync();

            var kurumIdleri = randevular.Select(r => r.KurumId).Distinct().ToList();
            var kurumlar = await baglam.Kurumlar.AsNoTracking()
                .Where(k => kurumIdleri.Contains(k.Id))
                .Select(k => new { k.Id, k.Ad })
                .ToDictionaryAsync(k => k.Id, k => k.Ad);

            _tumRandevular = randevular.Select(r => new RandevuSatiri
            {
                RandevuNo = r.Id,
                Hasta = r.Hasta?.Kullanici?.AdSoyad ?? "Bilinmiyor",
                Telefon = r.Hasta?.Telefon ?? "-",
                Kurum = kurumlar.TryGetValue(r.KurumId, out string? ad) ? ad : "-",
                Hizmet = r.SaglikHizmeti?.Ad ?? "Bilinmiyor",
                Tarih = r.BaslangicTarihiSaati.ToString("dd.MM.yyyy"),
                Saat = r.BaslangicTarihiSaati.ToString("HH:mm"),
                BitisSaati = r.BitisTarihiSaati.ToString("HH:mm"),
                Ucret = (r.UcretKurusKayit / 100m).ToString("N2") + " TL",
                Durum = r.Durum,
                BaslangicTarihiSaati = r.BaslangicTarihiSaati,
                BitisTarihiSaati = r.BitisTarihiSaati
            }).ToList();

            lblBekleyenDeger.Text = _tumRandevular
                .Count(r => r.Durum == "Beklemede").ToString();
            lblToplamDeger.Text = _tumRandevular.Count.ToString();
            ListeyiFiltrele();
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Randevular yüklenemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed)
                btnYenile.Enabled = true;
            IslemButonlariniGuncelle();
        }
    }

    private void ListeyiFiltrele()
    {
        if (IsDisposed || dgvRandevular == null || cmbDurumFiltresi == null)
            return;

        var filtre = cmbDurumFiltresi.SelectedItem?.ToString() ?? "Tümü";
        var gorunen = filtre == "Tümü"
            ? _tumRandevular.ToList()
            : _tumRandevular.Where(r => r.Durum == filtre).ToList();

        dgvRandevular.DataSource = null;
        dgvRandevular.DataSource = gorunen;
        dgvRandevular.ClearSelection();
        lblListeBilgisi.Text = gorunen.Count == 0
            ? "Bu filtrede gösterilecek randevu bulunmuyor."
            : $"{gorunen.Count} randevu gösteriliyor. İşlem yapmak için satır seçiniz.";
        IslemButonlariniGuncelle();
    }

    private RandevuSatiri? SeciliRandevu()
    {
        return dgvRandevular.SelectedRows.Count > 0
            ? dgvRandevular.SelectedRows[0].DataBoundItem as RandevuSatiri
            : null;
    }

    private void IslemButonlariniGuncelle()
    {
        if (btnOnayla == null || btnIptal == null || btnTamamla == null)
            return;

        var randevu = SeciliRandevu();
        var simdi = DateTime.Now;
        btnOnayla.Enabled = !_islemDevamEdiyor && randevu != null &&
                           randevu.Durum == "Beklemede" &&
                           randevu.BaslangicTarihiSaati > simdi;
        btnIptal.Enabled = !_islemDevamEdiyor && randevu != null &&
                           (randevu.Durum == "Beklemede" ||
                            randevu.Durum == "Onaylandi") &&
                           randevu.BaslangicTarihiSaati > simdi;
        btnTamamla.Enabled = !_islemDevamEdiyor && randevu != null &&
                            randevu.Durum == "Onaylandi" &&
                            randevu.BitisTarihiSaati <= simdi;
    }

    private async void btnYenile_Click(object? sender, EventArgs e)
    {
        if (_islemDevamEdiyor) return;
        if (await DoktorBilgileriniYukleAsync() && !IsDisposed)
            await RandevulariYukleAsync();
    }

    private async void btnOnayla_Click(object? sender, EventArgs e) =>
        await RandevuIslemiYapAsync("Onayla");

    private async void btnIptal_Click(object? sender, EventArgs e) =>
        await RandevuIslemiYapAsync("Iptal");

    private async void btnTamamla_Click(object? sender, EventArgs e) =>
        await RandevuIslemiYapAsync("Tamamla");

    private async Task RandevuIslemiYapAsync(string islem)
    {
        if (_islemDevamEdiyor || SeciliRandevu() is not RandevuSatiri secili)
        {
            MessageBox.Show("Lütfen işlem yapılacak bir randevu seçiniz.",
                "Randevu Seçimi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Yanlis veya zamani gecmis bir durum icin islem baslatma.
        var simdi = DateTime.Now;
        bool izinli = islem switch
        {
            "Onayla" => secili.Durum == "Beklemede" && secili.BaslangicTarihiSaati > simdi,
            "Iptal" => (secili.Durum == "Beklemede" || secili.Durum == "Onaylandi") &&
                       secili.BaslangicTarihiSaati > simdi,
            "Tamamla" => secili.Durum == "Onaylandi" && secili.BitisTarihiSaati <= simdi,
            _ => false
        };
        if (!izinli)
        {
            MessageBox.Show("Bu randevunun durumu veya zamanı seçilen işleme uygun değil.",
                "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string eylem = islem switch
        {
            "Onayla" => "onaylamak",
            "Iptal" => "iptal etmek",
            _ => "tamamlandı olarak işaretlemek"
        };
        if (MessageBox.Show(
                $"{secili.RandevuNo} numaralı randevuyu {eylem} istiyor musunuz?",
                "İşlemi Onayla", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            != DialogResult.Yes)
            return;

        _islemDevamEdiyor = true;
        btnYenile.Enabled = false;
        cmbDurumFiltresi.Enabled = false;
        dgvRandevular.Enabled = false;
        IslemButonlariniGuncelle();

        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var yonetim = new RandevuYonetimi(baglam);

            // Yetki ve durum kontrollerini mevcut is kurallari da yapar.
            switch (islem)
            {
                case "Onayla":
                    await yonetim.RandevuOnaylaAsync(secili.RandevuNo, _kullaniciId);
                    break;
                case "Iptal":
                    await yonetim.RandevuIptalEtAsync(secili.RandevuNo, _kullaniciId);
                    break;
                case "Tamamla":
                    await yonetim.RandevuTamamlaAsync(secili.RandevuNo, _kullaniciId);
                    break;
            }

            MessageBox.Show("Randevu işlemi başarıyla tamamlandı.",
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (!IsDisposed)
                await RandevulariYukleAsync();
        }
        catch (InvalidOperationException hata)
        {
            MessageBox.Show(hata.Message, "Randevu İşlemi",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (!IsDisposed)
                await RandevulariYukleAsync();
        }
        catch (Exception)
        {
            MessageBox.Show("Randevu işlemi gerçekleştirilemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemDevamEdiyor = false;
            if (!IsDisposed)
            {
                btnYenile.Enabled = true;
                cmbDurumFiltresi.Enabled = true;
                dgvRandevular.Enabled = true;
                IslemButonlariniGuncelle();
            }
        }
    }
}
