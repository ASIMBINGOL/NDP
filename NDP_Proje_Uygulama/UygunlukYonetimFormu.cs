#nullable enable
using System.ComponentModel;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

public partial class UygunlukYonetimFormu : Form
{
    private sealed class KurumSecenegi
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class DoktorSecenegi
    {
        public int Id { get; init; }
        public int KurumId { get; init; }
        public string Ad { get; init; } = "";
    }

    private sealed class UygunlukSatiri
    {
        public int No { get; init; }
        public string Kurum { get; init; } = "";
        public string Doktor { get; init; } = "";
        public string Tarih { get; init; } = "";
        public string Baslangic { get; init; } = "";
        public string Bitis { get; init; } = "";
        public int Randevu { get; init; }
    }

    private int _yoneticiId;
    private int _seciliId;
    private bool _dolduruluyor;
    private bool _islemYapiliyor;
    private List<UygunlukBilgisi> _kayitlar = new();
    private List<DoktorSecenegi> _doktorlar = new();

    // Designer'ın açılabilmesi için parametresiz yapıcı.
    public UygunlukYonetimFormu()
    {
        InitializeComponent();
        FiltreleriBaslat();
        Load += UygunlukYonetimFormu_Load;
        dgvUygunluk.SelectionChanged += (_, _) => SeciliKaydiGoster();
        cmbKurum.SelectedIndexChanged += (_, _) =>
        {
            if (!_dolduruluyor) DoktorlariKurumunaGoreGoster();
        };
        btnYeni.Click += (_, _) => FormuTemizle();
        btnYenile.Click += async (_, _) => await ListeyiYukleAsync();
        btnKaydet.Click += async (_, _) => await KaydetAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        btnKapat.Click += (_, _) => Close();
    }

    public UygunlukYonetimFormu(int yoneticiId) : this() => _yoneticiId = yoneticiId;

    private async void UygunlukYonetimFormu_Load(object? sender, EventArgs e)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || _yoneticiId <= 0)
            return;
        await ListeyiYukleAsync();
    }

    private void DoktorlariKurumunaGoreGoster(int secilecekDoktorId = 0)
    {
        if (cmbKurum.SelectedItem is not KurumSecenegi kurum) return;
        _dolduruluyor = true;
        try
        {
            cmbDoktor.DataSource = null;
            cmbDoktor.DisplayMember = nameof(DoktorSecenegi.Ad);
            cmbDoktor.ValueMember = nameof(DoktorSecenegi.Id);
            cmbDoktor.DataSource = _doktorlar.Where(d => d.KurumId == kurum.Id)
                .OrderBy(d => d.Ad).ToList();
            if (secilecekDoktorId > 0)
            {
                foreach (var item in cmbDoktor.Items)
                    if (item is DoktorSecenegi d && d.Id == secilecekDoktorId)
                    {
                        cmbDoktor.SelectedItem = item;
                        break;
                    }
            }
        }
        finally { _dolduruluyor = false; }
    }

    private void FormuTemizle()
    {
        _dolduruluyor = true;
        try
        {
            _seciliId = 0;
            dgvUygunluk.ClearSelection();
            dgvUygunluk.CurrentCell = null;
            cmbKurum.Enabled = true;
            cmbDoktor.Enabled = true;
            if (cmbKurum.Items.Count > 0) cmbKurum.SelectedIndex = 0;
            dtpTarih.Value = DateTime.Today.AddDays(1);
            txtBaslangic.Text = "09:00";
            txtBitis.Text = "17:00";
            lblDurum.Text = "Durum: Yeni uygunluk kaydı";
            btnSil.Enabled = false;
            btnKaydet.Enabled = true;
        }
        finally { _dolduruluyor = false; }
        DoktorlariKurumunaGoreGoster();
    }

    private void SeciliKaydiGoster()
    {
        if (_dolduruluyor || _islemYapiliyor ||
            dgvUygunluk.CurrentRow?.DataBoundItem is not UygunlukSatiri satir)
            return;
        var kayit = _kayitlar.FirstOrDefault(k => k.Id == satir.No);
        if (kayit is null) return;
        _dolduruluyor = true;
        try
        {
            _seciliId = kayit.Id;
            foreach (var item in cmbKurum.Items)
                if (item is KurumSecenegi k && k.Id == kayit.KurumId)
                {
                    cmbKurum.SelectedItem = item;
                    break;
                }
            dtpTarih.Value = kayit.BaslangicTarihiSaati.Date;
            txtBaslangic.Text = kayit.BaslangicTarihiSaati.ToString("HH:mm");
            txtBitis.Text = kayit.BitisTarihiSaati.ToString("HH:mm");
            cmbKurum.Enabled = false;
            cmbDoktor.Enabled = false;
            lblDurum.Text = kayit.RandevuSayisi > 0
                ? $"Durum: #{kayit.Id} — {kayit.RandevuSayisi} randevu bağlı, düzenlenemez"
                : $"Durum: #{kayit.Id} düzenleniyor";
            btnSil.Enabled = true;
            btnKaydet.Enabled = true;
        }
        finally { _dolduruluyor = false; }
        DoktorlariKurumunaGoreGoster(kayit.CalisanId);
    }

    private async Task<bool> GuvenliIslemAsync(Func<Task> islem)
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
            MessageBox.Show("Veritabanı ilişkileri nedeniyle işlem tamamlanamadı. Listeyi yenileyip tekrar deneyin.",
                "Veritabanı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("İşlem tamamlanamadı. Bağlantıyı ve kayıtları kontrol edin.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _islemYapiliyor = false;
            if (!IsDisposed)
            {
                btnYeni.Enabled = btnYenile.Enabled = true;
                btnKaydet.Enabled = true;
                btnSil.Enabled = _seciliId > 0;
            }
        }
        return false;
    }

    private async Task ListeyiYukleAsync(int secilecekId = 0)
    {
        bool tamam = await GuvenliIslemAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new UygunlukYonetimServisi(baglam, _yoneticiId);
            _kayitlar = (await servis.ListeleAsync()).ToList();
            var kurumlar = await baglam.Kurumlar.AsNoTracking().OrderBy(k => k.Ad)
                .Select(k => new KurumSecenegi
                {
                    Id = k.Id,
                    Ad = k.AktifMi ? k.Ad : k.Ad + " (Pasif)"
                }).ToListAsync();
            _doktorlar = await baglam.Doktorlar.AsNoTracking()
                .OrderBy(d => d.Kullanici!.AdSoyad)
                .Select(d => new DoktorSecenegi
                {
                    Id = d.Id,
                    KurumId = d.KurumId,
                    Ad = d.Kullanici!.AdSoyad + (d.AktifMi && d.Kullanici.AktifMi ? "" : " (Pasif)")
                }).ToListAsync();

            _dolduruluyor = true;
            try
            {
                dgvUygunluk.DataSource = null;
                dgvUygunluk.DataSource = _kayitlar.Select(a => new UygunlukSatiri
                {
                    No = a.Id,
                    Kurum = a.KurumAdi,
                    Doktor = a.DoktorAdi,
                    Tarih = a.BaslangicTarihiSaati.ToString("dd.MM.yyyy"),
                    Baslangic = a.BaslangicTarihiSaati.ToString("HH:mm"),
                    Bitis = a.BitisTarihiSaati.ToString("HH:mm"),
                    Randevu = a.RandevuSayisi
                }).ToList();
                FiltreKaynakYenile();
                cmbKurum.DataSource = null;
                cmbKurum.DisplayMember = nameof(KurumSecenegi.Ad);
                cmbKurum.ValueMember = nameof(KurumSecenegi.Id);
                cmbKurum.DataSource = kurumlar;
                FiltreSecenekleriniYukle();
                FiltreleriUygula(false);
            }
            finally { _dolduruluyor = false; }
        });
        if (!tamam || IsDisposed) return;
        if (secilecekId > 0)
        {
            foreach (DataGridViewRow satir in dgvUygunluk.Rows)
            {
                if (satir.DataBoundItem is UygunlukSatiri a && a.No == secilecekId)
                {
                    dgvUygunluk.ClearSelection();
                    satir.Selected = true;
                    dgvUygunluk.CurrentCell = satir.Cells[0];
                    SeciliKaydiGoster();
                    return;
                }
            }
        }
        FormuTemizle();
    }

    // Butonlar seçili kayıtta aktif kalır; engel varsa kullanıcıya nedenini açıklarız.
    // Gerçek iş kuralı kontrolü ayrıca UygunlukYonetimServisi içinde korunur.
    private bool BagliRandevuUyarisiGoster(string islem)
    {
        if (_seciliId <= 0) return false;

        var kayit = _kayitlar.FirstOrDefault(k => k.Id == _seciliId);
        if (kayit is null || kayit.RandevuSayisi == 0) return false;

        MessageBox.Show(
            $"#{kayit.Id} numaralı doktor uygunluğuna {kayit.RandevuSayisi} randevu bağlı.\n" +
            $"Bu nedenle {islem} işlemi yapılamaz.\n\n" +
            "Randevu geçmişini korumak için bu uygunluk kaydı değiştirilemez veya silinemez.",
            "İşlem Yapılamadı",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return true;
    }

    private async Task KaydetAsync()
    {
        if (BagliRandevuUyarisiGoster("güncelleme")) return;

        if (cmbKurum.SelectedItem is not KurumSecenegi kurum ||
            cmbDoktor.SelectedItem is not DoktorSecenegi doktor)
        {
            MessageBox.Show("Kurum ve doktor seçiniz.", "Eksik Bilgi");
            return;
        }
        if (!TimeOnly.TryParseExact(txtBaslangic.Text.Trim(), "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var baslangicSaat) ||
            !TimeOnly.TryParseExact(txtBitis.Text.Trim(), "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var bitisSaat))
        {
            MessageBox.Show("Saatleri 09:00 veya 17:00 gibi HH:mm biçiminde yazınız.", "Geçersiz Saat");
            return;
        }
        var tarih = dtpTarih.Value.Date;
        var kayit = new UygunlukBilgisi
        {
            Id = _seciliId,
            KurumId = kurum.Id,
            CalisanId = doktor.Id,
            BaslangicTarihiSaati = tarih.Add(baslangicSaat.ToTimeSpan()),
            BitisTarihiSaati = tarih.Add(bitisSaat.ToTimeSpan())
        };
        if (MessageBox.Show(_seciliId == 0 ? "Yeni doktor uygunluğu oluşturulsun mu?" :
                "Uygunluk kaydı güncellensin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        int id = 0;
        bool tamam = await GuvenliIslemAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new UygunlukYonetimServisi(baglam, _yoneticiId);
            id = await servis.KaydetAsync(kayit);
        });
        if (!tamam) return;
        await ListeyiYukleAsync(id);
        MessageBox.Show("Doktor uygunluk kaydı kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task SilAsync()
    {
        if (BagliRandevuUyarisiGoster("kalıcı silme")) return;

        if (_seciliId <= 0)
        {
            MessageBox.Show("Önce listeden bir uygunluk seçiniz.");
            return;
        }
        if (MessageBox.Show("Seçili uygunluk kalıcı olarak silinsin mi?\n" +
                "Randevu bağlantısı varsa sistem silmeyi engelleyecek.", "Kalıcı Silme",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int silinecekId = _seciliId;
        bool tamam = await GuvenliIslemAsync(async () =>
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var servis = new UygunlukYonetimServisi(baglam, _yoneticiId);
            await servis.SilAsync(silinecekId);
        });
        if (!tamam) return;
        await ListeyiYukleAsync();
        MessageBox.Show("Uygunluk kaydı silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private bool _filtreYukleniyor;
    private List<UygunlukSatiri> _filtreKaynak = new();

    private void FiltreleriBaslat()
    {
        _filtreYukleniyor = true;
        FiltreSecenekleriDoldur(cmbFiltreKurum, Array.Empty<FiltreSecenegi>(), "Tüm hastaneler");
        FiltreSecenekleriDoldur(cmbFiltreDoktor, Array.Empty<FiltreSecenegi>(), "Tüm doktorlar");
        cmbFiltreTarih.Items.Add("Tüm tarihler");
        cmbFiltreTarih.SelectedIndex = 0;
        _filtreYukleniyor = false;
        cmbFiltreKurum.SelectedIndexChanged += (_, _) => { FiltreDoktorlariniYukle(); FiltreleriUygula(); };
        cmbFiltreDoktor.SelectedIndexChanged += (_, _) => FiltreleriUygula();
        cmbFiltreTarih.SelectedIndexChanged += (_, _) => FiltreleriUygula();
    }

    private void FiltreKaynakYenile() =>
        _filtreKaynak = (dgvUygunluk.DataSource as List<UygunlukSatiri>)?.ToList() ?? new();

    private void FiltreSecenekleriniYukle()
    {
        _filtreYukleniyor = true;
        try
        {
            FiltreSecenekleriDoldur(cmbFiltreKurum,
                _kayitlar.GroupBy(a => a.KurumId)
                    .Select(g => new FiltreSecenegi(g.Key, g.First().KurumAdi))
                    .OrderBy(k => k.Ad), "Tüm hastaneler");
            FiltreDoktorlariniYukle();
            string oncekiTarih = cmbFiltreTarih.SelectedItem?.ToString() ?? "Tüm tarihler";
            cmbFiltreTarih.Items.Clear();
            cmbFiltreTarih.Items.Add("Tüm tarihler");
            foreach (var gun in _kayitlar.Select(a => a.BaslangicTarihiSaati.Date).Distinct()
                         .OrderByDescending(t => t))
                cmbFiltreTarih.Items.Add(gun.ToString("dd.MM.yyyy"));
            cmbFiltreTarih.SelectedItem = cmbFiltreTarih.Items.Cast<object>()
                .FirstOrDefault(x => x.ToString() == oncekiTarih) ?? "Tüm tarihler";
        }
        finally { _filtreYukleniyor = false; }
    }

    private void FiltreDoktorlariniYukle()
    {
        bool eski = _filtreYukleniyor;
        _filtreYukleniyor = true;
        try
        {
            int kurumId = SeciliFiltreId(cmbFiltreKurum);
            FiltreSecenekleriDoldur(cmbFiltreDoktor,
                _kayitlar.Where(a => kurumId == 0 || a.KurumId == kurumId)
                    .GroupBy(a => a.CalisanId)
                    .Select(g => new FiltreSecenegi(g.Key, g.First().DoktorAdi))
                    .OrderBy(k => k.Ad), "Tüm doktorlar");
        }
        finally { _filtreYukleniyor = eski; }
    }

    private void FiltreleriUygula(bool formuTemizle = true)
    {
        if (_filtreYukleniyor) return;
        int kurumId = SeciliFiltreId(cmbFiltreKurum);
        int doktorId = SeciliFiltreId(cmbFiltreDoktor);
        string tarih = cmbFiltreTarih.SelectedItem?.ToString() ?? "Tüm tarihler";
        var idler = _kayitlar.Where(a =>
            (kurumId == 0 || a.KurumId == kurumId) &&
            (doktorId == 0 || a.CalisanId == doktorId) &&
            (tarih == "Tüm tarihler" || a.BaslangicTarihiSaati.ToString("dd.MM.yyyy") == tarih))
            .Select(a => a.Id).ToHashSet();
        var gorunen = _filtreKaynak.Where(s => idler.Contains(s.No)).ToList();
        bool eski = _dolduruluyor;
        _dolduruluyor = true;
        try
        {
            dgvUygunluk.DataSource = null;
            dgvUygunluk.DataSource = gorunen;
            dgvUygunluk.ClearSelection();
            dgvUygunluk.CurrentCell = null;
        }
        finally { _dolduruluyor = eski; }
        lblKayitSayisi.Text = $"Gösterilen: {gorunen.Count} / Toplam: {_filtreKaynak.Count} doktor uygunluğu";
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
