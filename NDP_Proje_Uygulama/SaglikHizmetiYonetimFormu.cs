#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class SaglikHizmetiYonetimFormu : Form
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

    private sealed class DoktorSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class HizmetSatiri
    {
        public int Id { get; init; }
        public string Kurum { get; init; } = "";
        public string Bolum { get; init; } = "";
        public string Hizmet { get; init; } = "";
        public int SureDakika { get; init; }
        public string Ucret { get; init; } = "";
        public string Durum { get; init; } = "";
    }

    private int _yoneticiId;
    private int _seciliHizmetId;
    private int _atamaYuklemeSurumu;
    private bool _formYukleniyor;
    private bool _islemYapiliyor;
    private List<SaglikHizmetiBilgisi> _hizmetler = new();
    private List<BolumSecenegi> _bolumler = new();

    // Visual Studio Windows Forms Designer bu yapıcıyı kullanır.
    public SaglikHizmetiYonetimFormu()
    {
        InitializeComponent();
        Load += SaglikHizmetiYonetimFormu_Load;
        dgvHizmetler.SelectionChanged += async (_, _) => await SecimDegistiAsync();
        cmbKurum.SelectedIndexChanged += (_, _) =>
        {
            if (!_formYukleniyor) BolumleriGuncelle();
        };
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnDurum.Click += async (_, _) => await DurumDegistirAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnDoktorAta.Click += async (_, _) => await DoktorAtaAsync();
        btnAtamaKaldir.Click += async (_, _) => await AtamaKaldirAsync();
        btnKapat.Click += (_, _) => Close();
    }

    public SaglikHizmetiYonetimFormu(int yoneticiId) : this()
    {
        _yoneticiId = yoneticiId;
    }

    private async void SaglikHizmetiYonetimFormu_Load(object? sender, EventArgs e)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || _yoneticiId <= 0)
            return;
        await ListeyiYukleAsync();
    }

    private async Task<bool> IslemYapAsync(Func<Task> islem)
    {
        if (_islemYapiliyor) return false;
        _islemYapiliyor = true;
        btnYeni.Enabled = btnYenile.Enabled = btnKaydet.Enabled = false;
        btnDurum.Enabled = btnSil.Enabled = false;
        btnDoktorAta.Enabled = btnAtamaKaldir.Enabled = false;
        try
        {
            await islem();
            return true;
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "İşlem Yapılamadı", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show(ex.Message, "Yetki Hatası", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Veritabanı ilişkisi veya benzersizlik kuralı nedeniyle işlem tamamlanamadı.",
                "Veritabanı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("İşlem tamamlanamadı. Veritabanı bağlantısını ve kayıtları kontrol edin.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemYapiliyor = false;
            if (!IsDisposed)
            {
                btnYeni.Enabled = btnYenile.Enabled = btnKaydet.Enabled = true;
                btnDurum.Enabled = btnSil.Enabled = true;
                btnDoktorAta.Enabled = btnAtamaKaldir.Enabled = true;
            }
        }
        return false;
    }

    private async Task ListeyiYukleAsync(int secilecekHizmetId = 0)
    {
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new SaglikHizmetiYonetimServisi(baglam, _yoneticiId);
            _hizmetler = (await servis.ListeleAsync()).ToList();
            var kurumlar = await baglam.Kurumlar.AsNoTracking().OrderBy(k => k.Ad)
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
                dgvHizmetler.DataSource = null;
                dgvHizmetler.DataSource = _hizmetler.Select(h => new HizmetSatiri
                {
                    Id = h.Id,
                    Kurum = h.KurumAdi,
                    Bolum = h.BolumAdi,
                    Hizmet = h.Ad,
                    SureDakika = h.SureDakika,
                    Ucret = (h.UcretKurus / 100m).ToString("N2") + " TL",
                    Durum = h.AktifMi ? "Aktif" : "Pasif"
                }).ToList();
                if (dgvHizmetler.Columns.Contains("Id"))
                    dgvHizmetler.Columns["Id"]!.HeaderText = "No";
                if (dgvHizmetler.Columns.Contains("SureDakika"))
                    dgvHizmetler.Columns["SureDakika"]!.HeaderText = "Süre (dk)";

                cmbKurum.DataSource = null;
                cmbKurum.DisplayMember = nameof(KurumSecenegi.Ad);
                cmbKurum.ValueMember = nameof(KurumSecenegi.Id);
                cmbKurum.DataSource = kurumlar;
            }
            finally { _formYukleniyor = false; }
            lblKayitSayisi.Text = $"Toplam {_hizmetler.Count} sağlık hizmeti bulundu.";
        });
        if (!tamam || IsDisposed) return;

        if (secilecekHizmetId > 0)
        {
            foreach (DataGridViewRow satir in dgvHizmetler.Rows)
            {
                if (satir.DataBoundItem is HizmetSatiri h && h.Id == secilecekHizmetId)
                {
                    dgvHizmetler.ClearSelection();
                    satir.Selected = true;
                    dgvHizmetler.CurrentCell = satir.Cells[0];
                    await SecimDegistiAsync();
                    return;
                }
            }
        }
        FormuTemizle();
    }

    private void BolumleriGuncelle(int? secilecekBolumId = null)
    {
        int? kurumId = (cmbKurum.SelectedItem as KurumSecenegi)?.Id;
        _formYukleniyor = true;
        try
        {
            cmbBolum.DataSource = null;
            cmbBolum.DisplayMember = nameof(BolumSecenegi.Ad);
            cmbBolum.ValueMember = nameof(BolumSecenegi.Id);
            cmbBolum.DataSource = _bolumler.Where(b => b.KurumId == kurumId).ToList();
            cmbBolum.SelectedIndex = -1; // Bölüm zorunlu değil.
            if (secilecekBolumId.HasValue)
            {
                foreach (var item in cmbBolum.Items)
                {
                    if (item is BolumSecenegi bolum && bolum.Id == secilecekBolumId)
                    {
                        cmbBolum.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        finally { _formYukleniyor = false; }
    }

    private void FormuTemizle()
    {
        _atamaYuklemeSurumu++;
        _formYukleniyor = true;
        try
        {
            _seciliHizmetId = 0;
            dgvHizmetler.ClearSelection();
            cmbKurum.SelectedIndex = -1;
            cmbBolum.DataSource = null;
            txtAd.Clear();
            txtAciklama.Clear();
            nudSure.Value = 30;
            nudUcret.Value = 0;
            lblDurum.Text = "Durum: Yeni hizmet (aktif açılır)";
            btnDurum.Text = "Aktif / Pasif";
            dgvAtamalar.DataSource = null;
            cmbDoktor.DataSource = null;
            lblAtamaBilgi.Text = "Atama için önce kayıtlı bir hizmet seçiniz.";
        }
        finally { _formYukleniyor = false; }
    }

    private async Task SecimDegistiAsync()
    {
        if (_formYukleniyor || IsDisposed || dgvHizmetler.SelectedRows.Count == 0) return;
        if (dgvHizmetler.SelectedRows[0].DataBoundItem is not HizmetSatiri satir) return;
        var hizmet = _hizmetler.FirstOrDefault(h => h.Id == satir.Id);
        if (hizmet == null) return;
        _seciliHizmetId = hizmet.Id;
        _formYukleniyor = true;
        try
        {
            foreach (var item in cmbKurum.Items)
            {
                if (item is KurumSecenegi kurum && kurum.Id == hizmet.KurumId)
                {
                    cmbKurum.SelectedItem = item;
                    break;
                }
            }
            txtAd.Text = hizmet.Ad;
            txtAciklama.Text = hizmet.Aciklama;
            nudSure.Value = Math.Clamp(hizmet.SureDakika, (int)nudSure.Minimum, (int)nudSure.Maximum);
            nudUcret.Value = Math.Clamp(hizmet.UcretKurus / 100m, nudUcret.Minimum, nudUcret.Maximum);
            lblDurum.Text = hizmet.AktifMi ? "Durum: Aktif" : "Durum: Pasif";
            btnDurum.Text = hizmet.AktifMi ? "Pasifleştir" : "Aktifleştir";
        }
        finally { _formYukleniyor = false; }
        BolumleriGuncelle(hizmet.BolumId);
        await AtamalariYukleAsync(hizmet.Id);
    }

    private async Task AtamalariYukleAsync(int hizmetId)
    {
        int surum = ++_atamaYuklemeSurumu;
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new SaglikHizmetiYonetimServisi(baglam, _yoneticiId);
            var atamalar = (await servis.AtananDoktorlariListeleAsync(hizmetId)).ToList();
            var hizmet = _hizmetler.FirstOrDefault(h => h.Id == hizmetId);
            if (hizmet == null) return;
            var doktorlar = await baglam.Doktorlar.AsNoTracking()
                .Where(d => d.KurumId == hizmet.KurumId && d.AktifMi && d.Kullanici!.AktifMi &&
                    (hizmet.BolumId == null || d.BolumId == hizmet.BolumId))
                .OrderBy(d => d.Kullanici!.AdSoyad)
                .Select(d => new DoktorSecenegi
                {
                    Id = d.Id, Ad = d.Kullanici!.AdSoyad + " (" + d.UzmanlikAlani + ")"
                }).ToListAsync();
            var atananlar = atamalar.Select(a => a.CalisanId).ToHashSet();
            var atanabilir = doktorlar.Where(d => !atananlar.Contains(d.Id)).ToList();
            if (IsDisposed || surum != _atamaYuklemeSurumu || _seciliHizmetId != hizmetId) return;
            _formYukleniyor = true;
            try
            {
                dgvAtamalar.DataSource = null;
                dgvAtamalar.DataSource = atamalar;
                if (dgvAtamalar.Columns.Contains("CalisanId"))
                    dgvAtamalar.Columns["CalisanId"]!.HeaderText = "No";
                if (dgvAtamalar.Columns.Contains("DoktorAdi"))
                    dgvAtamalar.Columns["DoktorAdi"]!.HeaderText = "Doktor";
                if (dgvAtamalar.Columns.Contains("BolumAdi"))
                    dgvAtamalar.Columns["BolumAdi"]!.HeaderText = "Bölüm";
                if (dgvAtamalar.Columns.Contains("UzmanlikAlani"))
                    dgvAtamalar.Columns["UzmanlikAlani"]!.HeaderText = "Uzmanlık";
                dgvAtamalar.ClearSelection();

                cmbDoktor.DataSource = null;
                cmbDoktor.DisplayMember = nameof(DoktorSecenegi.Ad);
                cmbDoktor.ValueMember = nameof(DoktorSecenegi.Id);
                cmbDoktor.DataSource = atanabilir;
                cmbDoktor.SelectedIndex = -1;
                lblAtamaBilgi.Text = $"Atanan doktor: {atamalar.Count}    |    Atanabilir: {atanabilir.Count}";
            }
            finally { _formYukleniyor = false; }
        }
        catch (Exception)
        {
            if (!IsDisposed && surum == _atamaYuklemeSurumu)
                MessageBox.Show("Doktor atamaları yüklenemedi.", "Hata", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
        }
    }

    private async Task KaydetAsync()
    {
        if (cmbKurum.SelectedItem is not KurumSecenegi kurum)
        {
            MessageBox.Show("Önce bir kurum seçiniz.");
            return;
        }
        var kayit = new SaglikHizmetiBilgisi
        {
            Id = _seciliHizmetId,
            KurumId = kurum.Id,
            BolumId = (cmbBolum.SelectedItem as BolumSecenegi)?.Id,
            Ad = txtAd.Text,
            Aciklama = txtAciklama.Text,
            SureDakika = (int)nudSure.Value,
            UcretKurus = decimal.ToInt64(nudUcret.Value * 100m)
        };
        int kaydedilenId = 0;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            kaydedilenId = await new SaglikHizmetiYonetimServisi(baglam, _yoneticiId)
                .KaydetAsync(kayit);
            MessageBox.Show($"Hizmet kaydedildi (No: {kaydedilenId}).", "Başarılı",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
        if (tamam) await ListeyiYukleAsync(kaydedilenId);
    }

    private async Task DurumDegistirAsync()
    {
        if (_seciliHizmetId <= 0)
        {
            MessageBox.Show("Önce listeden bir hizmet seçiniz.");
            return;
        }
        int id = _seciliHizmetId;
        if (MessageBox.Show("Seçili hizmetin aktif/pasif durumunu değiştirmek istiyor musunuz?",
            "Durum Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new SaglikHizmetiYonetimServisi(baglam, _yoneticiId).DurumDegistirAsync(id);
            MessageBox.Show("Hizmetin durumu değiştirildi.", "Başarılı");
        });
        if (tamam) await ListeyiYukleAsync(id);
    }

    private async Task SilAsync()
    {
        if (_seciliHizmetId <= 0)
        {
            MessageBox.Show("Önce listeden bir hizmet seçiniz.");
            return;
        }
        int id = _seciliHizmetId;
        if (MessageBox.Show("Seçili hizmet KALICI olarak silinsin mi?\n" +
            "Randevusu veya personel ataması olan hizmetler silinemez.",
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new SaglikHizmetiYonetimServisi(baglam, _yoneticiId).SilAsync(id);
            MessageBox.Show("Hizmet kalıcı olarak silindi.", "Başarılı");
        });
        if (tamam) await ListeyiYukleAsync();
    }

    private async Task DoktorAtaAsync()
    {
        if (_seciliHizmetId <= 0 || cmbDoktor.SelectedItem is not DoktorSecenegi doktor)
        {
            MessageBox.Show("Önce hizmet ve atanacak doktoru seçiniz.");
            return;
        }
        int hizmetId = _seciliHizmetId, doktorId = doktor.Id;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new SaglikHizmetiYonetimServisi(baglam, _yoneticiId)
                .DoktorAtaAsync(hizmetId, doktorId);
            MessageBox.Show("Doktor hizmete atandı.", "Başarılı");
        });
        if (tamam) await AtamalariYukleAsync(hizmetId);
    }

    private async Task AtamaKaldirAsync()
    {
        if (_seciliHizmetId <= 0 || dgvAtamalar.SelectedRows.Count == 0 ||
            dgvAtamalar.SelectedRows[0].DataBoundItem is not DoktorHizmetAtamasiBilgisi atama)
        {
            MessageBox.Show("Önce hizmeti ve atamasını kaldıracağınız doktoru seçiniz.");
            return;
        }
        int hizmetId = _seciliHizmetId, doktorId = atama.CalisanId;
        if (MessageBox.Show($"{atama.DoktorAdi} adlı doktorun hizmet ataması kaldırılsın mı?",
            "Atama Kaldır", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new SaglikHizmetiYonetimServisi(baglam, _yoneticiId)
                .DoktorAtamasiniKaldirAsync(hizmetId, doktorId);
            MessageBox.Show("Doktorun hizmet ataması kaldırıldı.", "Başarılı");
        });
        if (tamam) await AtamalariYukleAsync(hizmetId);
    }
}
