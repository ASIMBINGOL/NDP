#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class BolumYonetimFormu : Form
{
    private sealed class KurumSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private int _yoneticiId;
    private int _seciliBolumId;
    private bool _islemYapiliyor;
    private bool _listeDegisiyor;

    public BolumYonetimFormu()
    {
        InitializeComponent();
        dgvBolumler.SelectionChanged += (_, _) => SecimDegisti();
        Load += async (_, _) =>
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && _yoneticiId > 0)
                await ListeyiYukleAsync();
        };
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnKapat.Click += (_, _) => Close();
    }

    public BolumYonetimFormu(int yoneticiId) : this() => _yoneticiId = yoneticiId;

    private async Task IslemYapAsync(Func<Task> islem)
    {
        if (_islemYapiliyor) return;
        _islemYapiliyor = true;
        btnKaydet.Enabled = btnSil.Enabled = btnYenile.Enabled = false;
        try { await islem(); }
        catch (InvalidOperationException hata) { MessageBox.Show(hata.Message, "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (UnauthorizedAccessException hata) { MessageBox.Show(hata.Message, "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        catch (DbUpdateException) { MessageBox.Show("Bu bölüm ilişkiler veya benzersizlik kuralı nedeniyle değiştirilemedi.", "Veritabanı Hatası"); }
        catch (Exception) { MessageBox.Show("İşlem tamamlanamadı. Bağlantıyı kontrol ediniz.", "Hata"); }
        finally
        {
            _islemYapiliyor = false;
            if (!IsDisposed) btnKaydet.Enabled = btnSil.Enabled = btnYenile.Enabled = true;
        }
    }

    private async Task ListeyiYukleAsync()
    {
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var yonetim = new BolumYonetimServisi(baglam, _yoneticiId);
            var bolumler = await yonetim.ListeleAsync();
            var kurumlar = await baglam.Kurumlar.AsNoTracking().OrderBy(k => k.Ad)
                .Select(k => new KurumSecenegi { Id = k.Id, Ad = k.Ad }).ToListAsync();
            _listeDegisiyor = true;
            try
            {
                dgvBolumler.DataSource = null;
                dgvBolumler.DataSource = bolumler.ToList();
                dgvBolumler.ClearSelection();
                cmbKurum.DataSource = null;
                cmbKurum.DisplayMember = nameof(KurumSecenegi.Ad);
                cmbKurum.DataSource = kurumlar;
            }
            finally { _listeDegisiyor = false; }
            FormuTemizle();
        });
    }

    private void SecimDegisti()
    {
        if (_listeDegisiyor || dgvBolumler.SelectedRows.Count == 0) return;
        if (dgvBolumler.SelectedRows[0].DataBoundItem is not BolumBilgisi bolum) return;
        _seciliBolumId = bolum.Id;
        txtAd.Text = bolum.Ad;
        txtAciklama.Text = bolum.Aciklama;
        foreach (var item in cmbKurum.Items)
        {
            if (item is KurumSecenegi secenek && secenek.Id == bolum.KurumId)
            {
                cmbKurum.SelectedItem = item;
                break;
            }
        }
    }

    private void FormuTemizle()
    {
        _seciliBolumId = 0;
        txtAd.Clear();
        txtAciklama.Clear();
        cmbKurum.SelectedIndex = -1;
        dgvBolumler.ClearSelection();
    }

    private async Task KaydetAsync()
    {
        if (cmbKurum.SelectedItem is not KurumSecenegi kurum)
        {
            MessageBox.Show("Önce kurum seçiniz.");
            return;
        }
        var kayit = new BolumBilgisi
        {
            Id = _seciliBolumId, KurumId = kurum.Id,
            Ad = txtAd.Text, Aciklama = txtAciklama.Text
        };
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            int id = await new BolumYonetimServisi(baglam, _yoneticiId).KaydetAsync(kayit);
            MessageBox.Show($"Bölüm kaydedildi (No: {id}).", "Başarılı");
        });
        await ListeyiYukleAsync();
    }

    private async Task SilAsync()
    {
        if (_seciliBolumId == 0) { MessageBox.Show("Önce bölüm seçiniz."); return; }
        if (MessageBox.Show("Seçili bölümü kalıcı silmek istiyor musunuz?\n" +
            "Doktoru veya hizmeti olan bölüm silinemez.",
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new BolumYonetimServisi(baglam, _yoneticiId).SilAsync(_seciliBolumId);
            MessageBox.Show("Bölüm silindi.", "Başarılı");
        });
        await ListeyiYukleAsync();
    }
}
