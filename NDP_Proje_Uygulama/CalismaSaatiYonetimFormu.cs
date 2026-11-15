#nullable enable
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class CalismaSaatiYonetimFormu : Form
{
    private sealed class KurumSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class GunSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class MesaiSatiri
    {
        public int Id { get; init; }
        public string Kurum { get; init; } = "";
        public string Gun { get; init; } = "";
        public string Acilis { get; init; } = "";
        public string Kapanis { get; init; } = "";
        public string Durum { get; init; } = "";
    }

    private readonly List<GunSecenegi> _gunler = new()
    {
        new() { Id = 1, Ad = "Pazartesi" },
        new() { Id = 2, Ad = "Salı" },
        new() { Id = 3, Ad = "Çarşamba" },
        new() { Id = 4, Ad = "Perşembe" },
        new() { Id = 5, Ad = "Cuma" },
        new() { Id = 6, Ad = "Cumartesi" },
        new() { Id = 0, Ad = "Pazar" },
    };

    private int _yoneticiId;
    private int _seciliId;
    private bool _dolduruluyor;
    private bool _islemYapiliyor;
    private List<CalismaSaatiBilgisi> _kayitlar = new();

    // Visual Studio Designer için parametresiz yapıcı.
    public CalismaSaatiYonetimFormu()
    {
        InitializeComponent();
        FiltreleriBaslat();
        Load += CalismaSaatiYonetimFormu_Load;
        dgvSaatler.SelectionChanged += (_, _) => SeciliKaydiGoster();
        chkKapali.CheckedChanged += (_, _) => SaatAlanlariniGuncelle();
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnKapat.Click += (_, _) => Close();
    }

    public CalismaSaatiYonetimFormu(int yoneticiId) : this()
    {
        _yoneticiId = yoneticiId;
    }

    private async void CalismaSaatiYonetimFormu_Load(object? sender, EventArgs e)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || _yoneticiId <= 0)
            return;
        await ListeyiYukleAsync();
    }

    private void SaatAlanlariniGuncelle()
    {
        txtAcilis.Enabled = txtKapanis.Enabled = !chkKapali.Checked;
    }

    private void FormuTemizle()
    {
        _dolduruluyor = true;
        try
        {
            _seciliId = 0;
            dgvSaatler.ClearSelection();
            dgvSaatler.CurrentCell = null;
            cmbKurum.Enabled = true;
            cmbGun.Enabled = true;
            if (cmbKurum.Items.Count > 0) cmbKurum.SelectedIndex = 0;
            if (cmbGun.Items.Count > 0) cmbGun.SelectedIndex = 0;
            txtAcilis.Text = "09:00";
            txtKapanis.Text = "17:00";
            chkKapali.Checked = false;
            lblDurum.Text = "Durum: Yeni gün kaydı (henüz kaydedilmedi)";
            btnSil.Enabled = false;
            SaatAlanlariniGuncelle();
        }
        finally { _dolduruluyor = false; }
    }

    private void SeciliKaydiGoster()
    {
        if (_dolduruluyor || _islemYapiliyor || dgvSaatler.CurrentRow?.DataBoundItem is not MesaiSatiri satir)
            return;
        var kayit = _kayitlar.FirstOrDefault(k => k.Id == satir.Id);
        if (kayit is null) return;
        _dolduruluyor = true;
        try
        {
            _seciliId = kayit.Id;
            foreach (var secenek in cmbKurum.Items)
                if (secenek is KurumSecenegi kurum && kurum.Id == kayit.KurumId)
                {
                    cmbKurum.SelectedItem = secenek;
                    break;
                }
            foreach (var secenek in cmbGun.Items)
                if (secenek is GunSecenegi gun && gun.Id == kayit.HaftaninGunu)
                {
                    cmbGun.SelectedItem = secenek;
                    break;
                }
            chkKapali.Checked = kayit.KapaliMi;
            txtAcilis.Text = kayit.AcilisSaati;
            txtKapanis.Text = kayit.KapanisSaati;
            cmbKurum.Enabled = false;
            cmbGun.Enabled = false;
            SaatAlanlariniGuncelle();
            lblDurum.Text = $"Durum: #{kayit.Id} düzenleniyor ({CalismaSaatiYonetimServisi.GunAdi(kayit.HaftaninGunu)})";
            btnSil.Enabled = true;
        }
        finally { _dolduruluyor = false; }
    }

    private async Task<bool> IslemYapAsync(Func<Task> islem)
    {
        if (_islemYapiliyor) return false;
        _islemYapiliyor = true;
        btnYeni.Enabled = btnYenile.Enabled = btnKaydet.Enabled = btnSil.Enabled = false;
        try
        {
            await islem();
            return true;
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show(ex.Message, "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Veritabanı kısıtı nedeniyle kayıt yapılamadı. Aynı kurumun aynı günü zaten kayıtlı olabilir.",
                "Veritabanı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                btnYeni.Enabled = btnYenile.Enabled = btnKaydet.Enabled = true;
                btnSil.Enabled = _seciliId != 0;
            }
        }
        return false;
    }

    private async Task ListeyiYukleAsync(int secilecekId = 0)
    {
        bool basarili = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new CalismaSaatiYonetimServisi(baglam, _yoneticiId);
            _kayitlar = (await servis.ListeleAsync()).ToList();
            var kurumlar = await baglam.Kurumlar.AsNoTracking().OrderBy(k => k.Ad)
                .Select(k => new KurumSecenegi
                {
                    Id = k.Id,
                    Ad = k.AktifMi ? k.Ad : k.Ad + " (Pasif)"
                }).ToListAsync();

            _dolduruluyor = true;
            try
            {
                dgvSaatler.DataSource = null;
                dgvSaatler.DataSource = _kayitlar.Select(k => new MesaiSatiri
                {
                    Id = k.Id,
                    Kurum = k.KurumAdi,
                    Gun = CalismaSaatiYonetimServisi.GunAdi(k.HaftaninGunu),
                    Acilis = k.KapaliMi ? "—" : k.AcilisSaati,
                    Kapanis = k.KapaliMi ? "—" : k.KapanisSaati,
                    Durum = k.KapaliMi ? "Kapalı" : "Açık"
                }).ToList();
                FiltreKaynakYenile();
                if (dgvSaatler.Columns.Contains("Id")) dgvSaatler.Columns["Id"]!.HeaderText = "No";
                if (dgvSaatler.Columns.Contains("Gun")) dgvSaatler.Columns["Gun"]!.HeaderText = "Gün";
                if (dgvSaatler.Columns.Contains("Acilis")) dgvSaatler.Columns["Acilis"]!.HeaderText = "Açılış";
                if (dgvSaatler.Columns.Contains("Kapanis")) dgvSaatler.Columns["Kapanis"]!.HeaderText = "Kapanış";

                cmbKurum.DataSource = null;
                cmbKurum.DisplayMember = nameof(KurumSecenegi.Ad);
                cmbKurum.ValueMember = nameof(KurumSecenegi.Id);
                cmbKurum.DataSource = kurumlar;
                cmbGun.DataSource = null;
                cmbGun.DisplayMember = nameof(GunSecenegi.Ad);
                cmbGun.ValueMember = nameof(GunSecenegi.Id);
                cmbGun.DataSource = _gunler.ToList();
                FiltreSecenekleriniYukle();
                FiltreleriUygula(false);
            }
            finally { _dolduruluyor = false; }
        });

        if (!basarili || IsDisposed) return;
        if (secilecekId > 0)
        {
            foreach (DataGridViewRow satir in dgvSaatler.Rows)
            {
                if (satir.DataBoundItem is MesaiSatiri s && s.Id == secilecekId)
                {
                    dgvSaatler.ClearSelection();
                    satir.Selected = true;
                    dgvSaatler.CurrentCell = satir.Cells[0];
                    SeciliKaydiGoster();
                    return;
                }
            }
        }
        FormuTemizle();
    }

    private async Task KaydetAsync()
    {
        if (cmbKurum.SelectedItem is not KurumSecenegi kurum ||
            cmbGun.SelectedItem is not GunSecenegi gun)
        {
            MessageBox.Show("Kurum ve gün seçiniz.", "Eksik Bilgi");
            return;
        }
        var kayit = new CalismaSaatiBilgisi
        {
            Id = _seciliId,
            KurumId = kurum.Id,
            HaftaninGunu = gun.Id,
            KapaliMi = chkKapali.Checked,
            AcilisSaati = txtAcilis.Text.Trim(),
            KapanisSaati = txtKapanis.Text.Trim()
        };
        if (MessageBox.Show(_seciliId == 0 ? "Yeni mesai günü kaydedilsin mi?" :
                "Mevcut mesai kaydı güncellensin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        int yeniId = 0;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new CalismaSaatiYonetimServisi(baglam, _yoneticiId);
            yeniId = await servis.KaydetAsync(kayit);
        });
        if (!tamam) return;
        await ListeyiYukleAsync(yeniId);
        MessageBox.Show("Çalışma saati kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task SilAsync()
    {
        if (_seciliId <= 0)
        {
            MessageBox.Show("Silmek istediğiniz günü listeden seçiniz.");
            return;
        }
        if (MessageBox.Show("Seçili çalışma günü kaydı kalıcı olarak silinsin mi?\n" +
                "Gelecekte aktif randevu varsa işlem engellenecektir.", "Kalıcı Silme",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int silinecekId = _seciliId;
        bool tamam = await IslemYapAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new CalismaSaatiYonetimServisi(baglam, _yoneticiId);
            await servis.SilAsync(silinecekId);
        });
        if (!tamam) return;
        await ListeyiYukleAsync();
        MessageBox.Show("Çalışma günü kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private bool _filtreYukleniyor;
    private List<MesaiSatiri> _filtreKaynak = new();

    private void FiltreleriBaslat()
    {
        _filtreYukleniyor = true;
        cmbFiltreDurum.Items.AddRange(new object[] { "Tümü", "Açık", "Kapalı" });
        cmbFiltreDurum.SelectedIndex = 0;
        FiltreSecenekleriDoldur(cmbFiltreKurum, Array.Empty<FiltreSecenegi>(), "Tüm hastaneler");
        FiltreSecenekleriDoldur(cmbFiltreGun,
            _gunler.Select(g => new FiltreSecenegi(g.Id, g.Ad)), "Tüm günler", -1);
        _filtreYukleniyor = false;
        cmbFiltreKurum.SelectedIndexChanged += (_, _) => FiltreleriUygula();
        cmbFiltreGun.SelectedIndexChanged += (_, _) => FiltreleriUygula();
        cmbFiltreDurum.SelectedIndexChanged += (_, _) => FiltreleriUygula();
    }

    private void FiltreKaynakYenile() =>
        _filtreKaynak = (dgvSaatler.DataSource as List<MesaiSatiri>)?.ToList() ?? new();

    private void FiltreSecenekleriniYukle()
    {
        _filtreYukleniyor = true;
        try
        {
            FiltreSecenekleriDoldur(cmbFiltreKurum,
                _kayitlar.GroupBy(k => k.KurumId)
                    .Select(g => new FiltreSecenegi(g.Key, g.First().KurumAdi))
                    .OrderBy(k => k.Ad), "Tüm hastaneler");
        }
        finally { _filtreYukleniyor = false; }
    }

    private void FiltreleriUygula(bool formuTemizle = true)
    {
        if (_filtreYukleniyor) return;
        int kurumId = SeciliFiltreId(cmbFiltreKurum);
        int gunId = SeciliFiltreId(cmbFiltreGun, -1);
        string durum = cmbFiltreDurum.SelectedItem?.ToString() ?? "Tümü";
        var idler = _kayitlar.Where(k =>
            (kurumId == 0 || k.KurumId == kurumId) &&
            (gunId == -1 || k.HaftaninGunu == gunId) &&
            (durum == "Tümü" || (k.KapaliMi ? "Kapalı" : "Açık") == durum))
            .Select(k => k.Id).ToHashSet();
        var gorunen = _filtreKaynak.Where(s => idler.Contains(s.Id)).ToList();
        bool eski = _dolduruluyor;
        _dolduruluyor = true;
        try
        {
            dgvSaatler.DataSource = null;
            dgvSaatler.DataSource = gorunen;
            if (dgvSaatler.Columns.Contains("Id")) dgvSaatler.Columns["Id"]!.HeaderText = "No";
            if (dgvSaatler.Columns.Contains("Gun")) dgvSaatler.Columns["Gun"]!.HeaderText = "Gün";
            if (dgvSaatler.Columns.Contains("Acilis")) dgvSaatler.Columns["Acilis"]!.HeaderText = "Açılış";
            if (dgvSaatler.Columns.Contains("Kapanis")) dgvSaatler.Columns["Kapanis"]!.HeaderText = "Kapanış";
            dgvSaatler.ClearSelection();
            dgvSaatler.CurrentCell = null;
        }
        finally { _dolduruluyor = eski; }
        lblKayitSayisi.Text = $"Gösterilen: {gorunen.Count} / Toplam: {_filtreKaynak.Count} çalışma günü";
        if (formuTemizle) FormuTemizle();
    }

    // Bu filtreleme yardımcıları sadece bu forma aittir.
    // Model ve iş kuralları katmanlarına ek bir bağımlılık getirmez.
    private sealed class FiltreSecenegi
    {
        public int Id { get; }
        public string Ad { get; }

        public FiltreSecenegi(int id, string ad)
        {
            Id = id;
            Ad = ad;
        }

        public override string ToString() => Ad;
    }

    private static int SeciliFiltreId(ComboBox kutu, int tumId = 0)
    {
        return (kutu.SelectedItem as FiltreSecenegi)?.Id ?? tumId;
    }

    private static void FiltreSecenekleriDoldur(
        ComboBox kutu,
        IEnumerable<FiltreSecenegi> secenekler,
        string tumYazisi,
        int tumId = 0)
    {
        int eskiId = SeciliFiltreId(kutu, tumId);
        var liste = new List<FiltreSecenegi>
        {
            new(tumId, tumYazisi)
        };
        liste.AddRange(secenekler.Where(s => s.Id != tumId)
            .GroupBy(s => s.Id).Select(g => g.First()));

        kutu.BeginUpdate();
        try
        {
            kutu.Items.Clear();
            foreach (var secenek in liste)
                kutu.Items.Add(secenek);
            kutu.SelectedItem = liste.FirstOrDefault(s => s.Id == eskiId)
                ?? liste[0];
        }
        finally
        {
            kutu.EndUpdate();
        }
    }

    private static bool FiltreMetinEslesir(string? kaynak, string? aranan)
    {
        return string.IsNullOrWhiteSpace(aranan) ||
            (kaynak ?? "").Contains(
                aranan.Trim(), StringComparison.CurrentCultureIgnoreCase);
    }
}
