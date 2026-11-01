#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class KurumYonetimFormu : Form
{
    private sealed class UstKurumSecenegi
    {
        public int? Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private int _yoneticiId;
    private int _seciliKurumId;
    private bool _islemYapiliyor;
    private bool _listeDegisiyor;
    private List<KurumBilgisi> _kurumlar = new();

    public KurumYonetimFormu()
    {
        InitializeComponent();
        cmbTur.Items.AddRange(new object[] { "Hastane", "Klinik", "SaglikMerkezi" });
        dgvKurumlar.SelectionChanged += (_, _) => SecimDegisti();
        Load += async (_, _) =>
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && _yoneticiId > 0)
                await ListeyiYukleAsync();
        };
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnDurum.Click += async (_, _) => await DurumDegistirAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnKapat.Click += (_, _) => Close();
        FormuTemizle();
    }

    public KurumYonetimFormu(int yoneticiId) : this() => _yoneticiId = yoneticiId;

    private async Task IslemYapAsync(Func<Task> islem)
    {
        if (_islemYapiliyor) return;
        _islemYapiliyor = true;
        btnKaydet.Enabled = btnSil.Enabled = btnDurum.Enabled = btnYenile.Enabled = false;
        try { await islem(); }
        catch (InvalidOperationException hata) { MessageBox.Show(hata.Message, "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (UnauthorizedAccessException hata) { MessageBox.Show(hata.Message, "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        catch (DbUpdateException) { MessageBox.Show("Kayıt ilişkili veriler veya benzersizlik kuralı nedeniyle güncellenemedi.", "Veritabanı Hatası"); }
        catch (Exception) { MessageBox.Show("İşlem tamamlanamadı. Bağlantıyı kontrol ediniz.", "Hata"); }
        finally
        {
            _islemYapiliyor = false;
            if (!IsDisposed)
                btnKaydet.Enabled = btnSil.Enabled = btnDurum.Enabled = btnYenile.Enabled = true;
        }
    }

    private async Task ListeyiYukleAsync()
    {
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var yonetim = new KurumYonetimServisi(baglam, _yoneticiId);
            _kurumlar = (await yonetim.ListeleAsync()).ToList();
            _listeDegisiyor = true;
            try
            {
                dgvKurumlar.DataSource = null;
                dgvKurumlar.DataSource = _kurumlar;
                dgvKurumlar.ClearSelection();
                UstKurumListesiniYukle(null);
            }
            finally { _listeDegisiyor = false; }
            FormuTemizle();
        });
    }

    private void UstKurumListesiniYukle(int? seciliUstId)
    {
        var liste = new List<UstKurumSecenegi> { new() { Ad = "(Üst kurum yok)", Id = null } };
        liste.AddRange(_kurumlar.Where(k => k.Id != _seciliKurumId)
            .Select(k => new UstKurumSecenegi { Id = k.Id, Ad = k.Ad }));
        cmbUst.DataSource = null;
        cmbUst.DisplayMember = nameof(UstKurumSecenegi.Ad);
        cmbUst.DataSource = liste;
        cmbUst.SelectedItem = liste.FirstOrDefault(x => x.Id == seciliUstId) ?? liste[0];
    }

    private void SecimDegisti()
    {
        if (_listeDegisiyor || dgvKurumlar.SelectedRows.Count == 0) return;
        if (dgvKurumlar.SelectedRows[0].DataBoundItem is not KurumBilgisi kurum) return;
        _seciliKurumId = kurum.Id;
        txtAd.Text = kurum.Ad;
        cmbTur.Text = kurum.KurumTuru;
        txtAdres.Text = kurum.Adres;
        txtTelefon.Text = kurum.Telefon;
        lblDurum.Text = kurum.AktifMi ? "Durum: Aktif" : "Durum: Pasif";
        btnDurum.Text = kurum.AktifMi ? "Pasifleştir" : "Aktifleştir";
        UstKurumListesiniYukle(kurum.UstKurumId);
    }

    private void FormuTemizle()
    {
        _seciliKurumId = 0;
        txtAd.Clear();
        cmbTur.Text = "Hastane";
        txtAdres.Clear();
        txtTelefon.Clear();
        lblDurum.Text = "Durum: Yeni kurum (aktif açılır)";
        btnDurum.Text = "Aktif/Pasif";
        UstKurumListesiniYukle(null);
        dgvKurumlar.ClearSelection();
    }

    private async Task KaydetAsync()
    {
        var kayit = new KurumBilgisi
        {
            Id = _seciliKurumId, Ad = txtAd.Text, KurumTuru = cmbTur.Text,
            Adres = txtAdres.Text, Telefon = txtTelefon.Text,
            UstKurumId = (cmbUst.SelectedItem as UstKurumSecenegi)?.Id
        };
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            int id = await new KurumYonetimServisi(baglam, _yoneticiId).KaydetAsync(kayit);
            MessageBox.Show($"Kurum kaydedildi (No: {id}).", "Başarılı");
        });
        await ListeyiYukleAsync();
    }

    private async Task DurumDegistirAsync()
    {
        if (_seciliKurumId == 0) { MessageBox.Show("Önce bir kurum seçiniz."); return; }
        if (MessageBox.Show("Seçili kurumun aktif/pasif durumunu değiştirmek istiyor musunuz?",
            "Durumu Değiştir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new KurumYonetimServisi(baglam, _yoneticiId).DurumDegistirAsync(_seciliKurumId);
            MessageBox.Show("Kurum durumu güncellendi.", "Başarılı");
        });
        await ListeyiYukleAsync();
    }

    private async Task SilAsync()
    {
        if (_seciliKurumId == 0) { MessageBox.Show("Önce bir kurum seçiniz."); return; }
        if (MessageBox.Show("Bu kurumu KALICI olarak silmek istiyor musunuz?\n" +
            "Bağımlı kayıtları olan kurumların silinmesine izin verilmez.",
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            await new KurumYonetimServisi(baglam, _yoneticiId).SilAsync(_seciliKurumId);
            MessageBox.Show("Kurum kalıcı olarak silindi.", "Başarılı");
        });
        await ListeyiYukleAsync();
    }
}
