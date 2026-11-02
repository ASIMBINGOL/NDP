#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

// Form yalnızca kullanıcı arayüzüdür. İş kuralları CalisanYonetimServisi içindedir.
public partial class CalisanYonetimFormu : Form
{
    private sealed class KurumSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class BolumSecenegi
    {
        public int Id { get; init; }
        public int KurumId { get; init; }
        public string Ad { get; init; } = "";
    }

    private int _yoneticiId;
    private int _seciliCalisanId;
    private bool _islemYapiliyor;
    private bool _formYukleniyor;
    private List<BolumSecenegi> _bolumler = new();
    private List<CalisanBilgisi> _calisanlar = new();

    // Visual Studio Designer için parametresiz kurucu.
    public CalisanYonetimFormu()
    {
        InitializeComponent();
        Load += async (_, _) =>
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && _yoneticiId > 0)
                await ListeyiYukleAsync();
        };
        dgvCalisanlar.SelectionChanged += (_, _) => SecimDegisti();
        cmbKurum.SelectedIndexChanged += (_, _) =>
        {
            if (!_formYukleniyor) BolumleriGuncelle();
        };
        cmbTur.SelectedIndexChanged += (_, _) =>
        {
            if (!_formYukleniyor) GorevEtiketiniGuncelle();
        };
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnDurum.Click += async (_, _) => await DurumDegistirAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnKapat.Click += (_, _) => Close();
    }

    public CalisanYonetimFormu(int yoneticiId) : this() => _yoneticiId = yoneticiId;

    private async Task<bool> IslemYapAsync(Func<Task> islem)
    {
        if (_islemYapiliyor) return false;
        _islemYapiliyor = true;
        btnYeni.Enabled = btnKaydet.Enabled = btnDurum.Enabled = false;
        btnSil.Enabled = btnYenile.Enabled = false;
        bool basarili = false;
        try
        {
            await islem();
            basarili = true;
        }
        catch (InvalidOperationException hata)
        {
            MessageBox.Show(hata.Message, "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException hata)
        {
            MessageBox.Show(hata.Message, "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Veri bütünlüğü veya benzersizlik kuralı nedeniyle işlem yapılamadı.",
                "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("İşlem tamamlanamadı. Veritabanı bağlantısını kontrol edin.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemYapiliyor = false;
            if (!IsDisposed)
            {
                btnYeni.Enabled = btnKaydet.Enabled = btnDurum.Enabled = true;
                btnSil.Enabled = btnYenile.Enabled = true;
            }
        }
        return basarili;
    }

    private async Task ListeyiYukleAsync()
    {
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var yonetim = new CalisanYonetimServisi(baglam, _yoneticiId);
            _calisanlar = (await yonetim.ListeleAsync()).ToList();
            var kurumlar = await baglam.Kurumlar.AsNoTracking()
                .OrderBy(k => k.Ad)
                .Select(k => new KurumSecenegi
                {
                    Id = k.Id,
                    Ad = k.AktifMi ? k.Ad : k.Ad + " (Pasif)"
                }).ToListAsync();
            _bolumler = await baglam.Bolumler.AsNoTracking().OrderBy(b => b.Ad)
                .Select(b => new BolumSecenegi { Id = b.Id, KurumId = b.KurumId, Ad = b.Ad })
                .ToListAsync();

            _formYukleniyor = true;
            try
            {
                dgvCalisanlar.DataSource = null;
                dgvCalisanlar.DataSource = _calisanlar;
                foreach (string gizli in new[] { "KullaniciId", "KurumId", "BolumId", "YeniSifre" })
                {
                    if (dgvCalisanlar.Columns.Contains(gizli))
                        dgvCalisanlar.Columns[gizli]!.Visible = false;
                }
                if (dgvCalisanlar.Columns.Contains("Id"))
                    dgvCalisanlar.Columns["Id"]!.HeaderText = "No";
                if (dgvCalisanlar.Columns.Contains("Tur"))
                    dgvCalisanlar.Columns["Tur"]!.HeaderText = "Tür";
                if (dgvCalisanlar.Columns.Contains("UzmanlikVeyaGorev"))
                    dgvCalisanlar.Columns["UzmanlikVeyaGorev"]!.HeaderText = "Uzmanlık / Görev";
                if (dgvCalisanlar.Columns.Contains("GorevBilgisi"))
                    dgvCalisanlar.Columns["GorevBilgisi"]!.Visible = false;

                cmbKurum.DataSource = null;
                cmbKurum.DisplayMember = nameof(KurumSecenegi.Ad);
                cmbKurum.ValueMember = nameof(KurumSecenegi.Id);
                cmbKurum.DataSource = kurumlar;
            }
            finally { _formYukleniyor = false; }
            FormuTemizle();
        });
    }

    private void BolumleriGuncelle(int? secilecekId = null)
    {
        int? kurumId = (cmbKurum.SelectedItem as KurumSecenegi)?.Id;
        _formYukleniyor = true;
        try
        {
            cmbBolum.DataSource = null;
            cmbBolum.DisplayMember = nameof(BolumSecenegi.Ad);
            cmbBolum.ValueMember = nameof(BolumSecenegi.Id);
            cmbBolum.DataSource = _bolumler.Where(b => b.KurumId == kurumId).ToList();
            cmbBolum.SelectedIndex = -1;
            if (secilecekId is int bolumId)
            {
                foreach (var item in cmbBolum.Items)
                {
                    if (item is BolumSecenegi bolum && bolum.Id == bolumId)
                    {
                        cmbBolum.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        finally { _formYukleniyor = false; }
    }

    private void GorevEtiketiniGuncelle()
    {
        bool doktorMu = (cmbTur.SelectedItem as string) == "Doktor";
        lblGorev.Text = doktorMu ? "Uzmanlık Alanı" : "Görevi (Örn: Hemşire)";
    }

    private void SecimDegisti()
    {
        if (_formYukleniyor || dgvCalisanlar.SelectedRows.Count == 0) return;
        if (dgvCalisanlar.SelectedRows[0].DataBoundItem is not CalisanBilgisi secilen) return;
        _seciliCalisanId = secilen.Id;
        _formYukleniyor = true;
        try
        {
            cmbTur.SelectedItem = secilen.Tur;
            cmbTur.Enabled = false; // TPH sınıf türü sonradan dönüştürülmez.
            foreach (var item in cmbKurum.Items)
            {
                if (item is KurumSecenegi kurum && kurum.Id == secilen.KurumId)
                {
                    cmbKurum.SelectedItem = item;
                    break;
                }
            }
            txtAdSoyad.Text = secilen.AdSoyad;
            txtEposta.Text = secilen.Eposta;
            txtGorev.Text = secilen.UzmanlikVeyaGorev;
            txtSifre.Clear();
            lblSifre.Text = "Yeni Şifre (değişmeyecekse boş bırak)";
            lblDurum.Text = secilen.AktifMi ? "Durum: Aktif" : "Durum: Pasif";
            btnDurum.Text = secilen.AktifMi ? "Pasifleştir" : "Aktifleştir";
        }
        finally { _formYukleniyor = false; }
        BolumleriGuncelle(secilen.BolumId);
        GorevEtiketiniGuncelle();
    }

    private void FormuTemizle()
    {
        _formYukleniyor = true;
        try
        {
            _seciliCalisanId = 0;
            dgvCalisanlar.ClearSelection();
            txtAdSoyad.Clear();
            txtEposta.Clear();
            txtGorev.Clear();
            txtSifre.Clear();
            cmbTur.Enabled = true;
            cmbTur.SelectedItem = "Doktor";
            cmbKurum.SelectedIndex = -1;
            lblSifre.Text = "Giriş Şifresi (yeni çalışan için zorunlu)";
            lblDurum.Text = "Durum: Yeni çalışan (aktif açılır)";
            btnDurum.Text = "Aktif / Pasif";
        }
        finally { _formYukleniyor = false; }
        BolumleriGuncelle();
        GorevEtiketiniGuncelle();
    }

    private async Task KaydetAsync()
    {
        if (cmbKurum.SelectedItem is not KurumSecenegi kurum)
        {
            MessageBox.Show("Aktif bir kurum seçiniz.");
            return;
        }
        string tur = cmbTur.SelectedItem as string ?? "";
        var kayit = new CalisanBilgisi
        {
            Id = _seciliCalisanId,
            AdSoyad = txtAdSoyad.Text,
            Eposta = txtEposta.Text,
            YeniSifre = txtSifre.Text,
            Tur = tur,
            KurumId = kurum.Id,
            BolumId = (cmbBolum.SelectedItem as BolumSecenegi)?.Id,
            UzmanlikVeyaGorev = txtGorev.Text
        };
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            int id = await new CalisanYonetimServisi(baglam, _yoneticiId).KaydetAsync(kayit);
            MessageBox.Show($"Çalışan kaydedildi (No: {id}).", "Başarılı",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
        if (tamam) await ListeyiYukleAsync();
    }

    private async Task DurumDegistirAsync()
    {
        if (_seciliCalisanId == 0)
        {
            MessageBox.Show("Önce listeden çalışan seçiniz.");
            return;
        }
        int id = _seciliCalisanId;
        if (MessageBox.Show("Seçili çalışanın aktif/pasif durumunu değiştirmek istiyor musunuz?",
            "Durum Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new CalisanYonetimServisi(baglam, _yoneticiId).DurumDegistirAsync(id);
            MessageBox.Show("Çalışanın durumu değiştirildi.", "Başarılı");
        });
        if (tamam) await ListeyiYukleAsync();
    }

    private async Task SilAsync()
    {
        if (_seciliCalisanId == 0)
        {
            MessageBox.Show("Önce listeden çalışan seçiniz.");
            return;
        }
        int id = _seciliCalisanId;
        if (MessageBox.Show("Seçilen çalışanı KALICI olarak silmek istiyor musunuz?\n" +
            "Randevu, uygunluk veya hizmet ataması varsa silme engellenir.",
            "Kalıcı Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new CalisanYonetimServisi(baglam, _yoneticiId).SilAsync(id);
            MessageBox.Show("Çalışan silindi.", "Başarılı");
        });
        if (tamam) await ListeyiYukleAsync();
    }
}
