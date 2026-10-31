using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama;

// Bu dosya mevcut RandevuAramaFormu.Designer.cs ile birlikte calisir.
// Designer dosyasini degistirmeyin.
public partial class RandevuAramaFormu : Form
{
    private int _kullaniciId;
    private bool _aramaDevamEdiyor;

    private sealed class Secenek
    {
        public int Id { get; init; }
        public string Ad { get; init; } = "";
    }

    // DataGridView'e gosterilecek bos randevu saati.
    private sealed class UygunSaat
    {
        [Browsable(false)]
        public int UygunlukAraligiId { get; init; }

        [Browsable(false)]
        public DateTime Baslangic { get; init; }

        public string Tarih => Baslangic.ToString("dd.MM.yyyy");
        public string Saat => Baslangic.ToString("HH:mm");
        public string Bitis => Baslangic.AddMinutes(SureDakika).ToString("HH:mm");
        public int SureDakika { get; init; }
        public string Ucret { get; init; } = "";
    }

    // Visual Studio Designer'in kullanabilecegi parametresiz kurucu.
    public RandevuAramaFormu()
    {
        InitializeComponent();
        OlaylariBagla();
    }

    public RandevuAramaFormu(int kullaniciId) : this()
    {
        _kullaniciId = kullaniciId;
    }

    private void OlaylariBagla()
    {
        cmbKurum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBolum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDoktor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbHizmet.DropDownStyle = ComboBoxStyle.DropDownList;

        dgvUygunSaatler.ReadOnly = true;
        dgvUygunSaatler.MultiSelect = false;
        dgvUygunSaatler.AllowUserToAddRows = false;
        dgvUygunSaatler.AllowUserToDeleteRows = false;
        dgvUygunSaatler.AutoGenerateColumns = true;
        dgvUygunSaatler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUygunSaatler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        btnRandevuOlustur.Enabled = false;

        Load += async (_, _) =>
        {
            if (_kullaniciId <= 0) return; // Designer onizlemesinde sorgu yok.
            TarihleriAyarla();
            await GuvenliCalistirAsync(KurumlariYukleAsync);
        };

        cmbKurum.SelectionChangeCommitted += async (_, _) =>
            await GuvenliCalistirAsync(BolumleriYukleAsync);
        cmbBolum.SelectionChangeCommitted += async (_, _) =>
            await GuvenliCalistirAsync(DoktorlariYukleAsync);
        cmbDoktor.SelectionChangeCommitted += async (_, _) =>
            await GuvenliCalistirAsync(HizmetleriYukleAsync);
        cmbHizmet.SelectionChangeCommitted += (_, _) => AramaSonuclariniSifirla();
        dtpBaslangic.ValueChanged += (_, _) => AramaSonuclariniSifirla();
        dtpBitis.ValueChanged += (_, _) => AramaSonuclariniSifirla();

        dgvUygunSaatler.SelectionChanged += (_, _) =>
        {
            if (!_aramaDevamEdiyor)
                btnRandevuOlustur.Enabled =
                    dgvUygunSaatler.SelectedRows.Count > 0 &&
                    dgvUygunSaatler.SelectedRows[0].DataBoundItem is UygunSaat;
        };

        btnUygunSaatleriBul.Click += async (_, _) => await UygunSaatleriBulAsync();
        btnRandevuOlustur.Click += async (_, _) => await RandevuOlusturAsync();
        btnGeriDon.Click += (_, _) => Close();
    }

    private void TarihleriAyarla()
    {
        dtpBaslangic.MinDate = DateTime.Today;
        dtpBitis.MinDate = DateTime.Today;
        dtpBaslangic.Value = DateTime.Today;
        dtpBitis.Value = DateTime.Today.AddDays(7);
    }

    private void AramaSonuclariniSifirla()
    {
        dgvUygunSaatler.DataSource = null;
        btnRandevuOlustur.Enabled = false;
        lblSonucBilgisi.Text = "Kriterleri seçip uygun randevuları arayınız.";
    }

    private static void ListeyiDoldur(ComboBox liste, List<Secenek> veriler)
    {
        liste.DataSource = null;
        liste.DisplayMember = nameof(Secenek.Ad);
        liste.ValueMember = nameof(Secenek.Id);
        liste.DataSource = veriler;
        liste.SelectedIndex = -1;
    }

    private static void ListeyiTemizle(ComboBox liste) =>
        ListeyiDoldur(liste, new List<Secenek>());

    private async Task GuvenliCalistirAsync(Func<Task> islem)
    {
        try
        {
            await islem();
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Seçenekler yüklenemedi. Veritabanı bağlantısını kontrol ediniz.",
                "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task KurumlariYukleAsync()
    {
        ListeyiTemizle(cmbKurum);
        ListeyiTemizle(cmbBolum);
        ListeyiTemizle(cmbDoktor);
        ListeyiTemizle(cmbHizmet);
        AramaSonuclariniSifirla();

        await using var baglam = VeritabaniErisimi.BaglamOlustur();
        var veriler = await baglam.Kurumlar.AsNoTracking()
            .Where(k => k.AktifMi)
            .OrderBy(k => k.Ad)
            .Select(k => new { k.Id, k.Ad })
            .ToListAsync();

        ListeyiDoldur(cmbKurum, veriler.Select(k => new Secenek
        {
            Id = k.Id,
            Ad = k.Ad
        }).ToList());
    }

    private async Task BolumleriYukleAsync()
    {
        AramaSonuclariniSifirla();
        ListeyiTemizle(cmbBolum);
        ListeyiTemizle(cmbDoktor);
        ListeyiTemizle(cmbHizmet);
        if (cmbKurum.SelectedItem is not Secenek kurum) return;

        await using var baglam = VeritabaniErisimi.BaglamOlustur();
        var veriler = await baglam.Bolumler.AsNoTracking()
            .Where(b => b.KurumId == kurum.Id)
            .OrderBy(b => b.Ad)
            .Select(b => new { b.Id, b.Ad })
            .ToListAsync();

        ListeyiDoldur(cmbBolum, veriler.Select(b => new Secenek
        {
            Id = b.Id,
            Ad = b.Ad
        }).ToList());
    }

    private async Task DoktorlariYukleAsync()
    {
        AramaSonuclariniSifirla();
        ListeyiTemizle(cmbDoktor);
        ListeyiTemizle(cmbHizmet);
        if (cmbKurum.SelectedItem is not Secenek kurum ||
            cmbBolum.SelectedItem is not Secenek bolum) return;

        await using var baglam = VeritabaniErisimi.BaglamOlustur();
        var veriler = await baglam.Doktorlar.AsNoTracking()
            .Where(d => d.KurumId == kurum.Id &&
                        d.BolumId == bolum.Id &&
                        d.AktifMi && d.Kullanici != null &&
                        d.Kullanici.AktifMi)
            .OrderBy(d => d.Kullanici!.AdSoyad)
            .Select(d => new { d.Id, Ad = d.Kullanici!.AdSoyad })
            .ToListAsync();

        ListeyiDoldur(cmbDoktor, veriler.Select(d => new Secenek
        {
            Id = d.Id,
            Ad = d.Ad
        }).ToList());
    }

    private async Task HizmetleriYukleAsync()
    {
        AramaSonuclariniSifirla();
        ListeyiTemizle(cmbHizmet);
        if (cmbKurum.SelectedItem is not Secenek kurum ||
            cmbBolum.SelectedItem is not Secenek bolum ||
            cmbDoktor.SelectedItem is not Secenek doktor) return;

        await using var baglam = VeritabaniErisimi.BaglamOlustur();
        var veriler = await baglam.CalisanHizmetleri.AsNoTracking()
            .Where(ch => ch.KurumId == kurum.Id &&
                         ch.CalisanId == doktor.Id &&
                         ch.SaglikHizmeti != null &&
                         ch.SaglikHizmeti.AktifMi &&
                         ch.SaglikHizmeti.BolumId == bolum.Id)
            .OrderBy(ch => ch.SaglikHizmeti!.Ad)
            .Select(ch => new
            {
                Id = ch.SaglikHizmetiId,
                Ad = ch.SaglikHizmeti!.Ad
            })
            .ToListAsync();

        ListeyiDoldur(cmbHizmet, veriler.Select(h => new Secenek
        {
            Id = h.Id,
            Ad = h.Ad
        }).ToList());
    }

    private async Task UygunSaatleriBulAsync()
    {
        AramaSonuclariniSifirla();

        if (cmbKurum.SelectedItem is not Secenek kurum ||
            cmbBolum.SelectedItem is not Secenek bolum ||
            cmbDoktor.SelectedItem is not Secenek doktor ||
            cmbHizmet.SelectedItem is not Secenek hizmetSecimi)
        {
            MessageBox.Show("Lütfen hastane, bölüm, doktor ve hizmet seçiniz.",
                "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DateTime ilkGun = dtpBaslangic.Value.Date;
        DateTime sonGun = dtpBitis.Value.Date;
        if (sonGun < ilkGun || ilkGun < DateTime.Today)
        {
            MessageBox.Show("Geçerli bir gelecek tarih aralığı seçiniz.",
                "Tarih Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if ((sonGun - ilkGun).TotalDays > 30)
        {
            MessageBox.Show("Bir defada en fazla 31 günlük aralık arayabilirsiniz.",
                "Tarih Aralığı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        btnUygunSaatleriBul.Enabled = false;
        _aramaDevamEdiyor = true;
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var hasta = await baglam.Hastalar.AsNoTracking()
                .FirstOrDefaultAsync(h => h.KullaniciId == _kullaniciId &&
                                          h.Kullanici != null &&
                                          h.Kullanici.AktifMi &&
                                          h.Kullanici.Rol == "Hasta");
            if (hasta is null)
                throw new InvalidOperationException("Aktif hasta hesabınız bulunamadı.");

            var saglikHizmeti = await baglam.SaglikHizmetleri.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == hizmetSecimi.Id &&
                                          h.KurumId == kurum.Id &&
                                          h.BolumId == bolum.Id &&
                                          h.AktifMi);
            if (saglikHizmeti is null || saglikHizmeti.SureDakika <= 0)
                throw new InvalidOperationException("Seçilen hizmet şu anda sunulmuyor.");

            bool doktorYetkiliMi = await baglam.Doktorlar.AsNoTracking()
                .AnyAsync(d => d.Id == doktor.Id && d.KurumId == kurum.Id &&
                               d.BolumId == bolum.Id && d.AktifMi &&
                               d.Kullanici != null && d.Kullanici.AktifMi);
            bool kurumAktifMi = await baglam.Kurumlar.AsNoTracking()
                .AnyAsync(k => k.Id == kurum.Id && k.AktifMi);
            bool hizmetVeriyorMu = await baglam.CalisanHizmetleri.AsNoTracking()
                .AnyAsync(ch => ch.KurumId == kurum.Id &&
                                ch.CalisanId == doktor.Id &&
                                ch.SaglikHizmetiId == hizmetSecimi.Id);
            if (!doktorYetkiliMi || !kurumAktifMi || !hizmetVeriyorMu)
                throw new InvalidOperationException("Seçilen kurum, doktor veya hizmet artık uygun değil.");

            DateTime bitisHaric = sonGun.AddDays(1);
            var araliklar = await baglam.UygunlukAraliklari.AsNoTracking()
                .Where(a => a.CalisanId == doktor.Id && a.KurumId == kurum.Id &&
                            a.BaslangicTarihiSaati < bitisHaric &&
                            a.BitisTarihiSaati > ilkGun)
                .OrderBy(a => a.BaslangicTarihiSaati)
                .ToListAsync();

            var mesailer = await baglam.KurumCalismaSaatleri.AsNoTracking()
                .Where(m => m.KurumId == kurum.Id)
                .ToListAsync();

            // Doktor ve hastanin aktif randevulari yeni randevuyla cakisamaz.
            var doluRandevular = await baglam.Randevular.AsNoTracking()
                .Where(r => (r.CalisanId == doktor.Id || r.HastaId == hasta.Id) &&
                            (r.Durum == "Beklemede" || r.Durum == "Onaylandi") &&
                            r.BaslangicTarihiSaati < bitisHaric &&
                            r.BitisTarihiSaati > ilkGun)
                .Select(r => new { r.BaslangicTarihiSaati, r.BitisTarihiSaati })
                .ToListAsync();

            var sonuc = new List<UygunSaat>();
            var tekrarKontrol = new HashSet<DateTime>();
            DateTime simdi = DateTime.Now;

            for (DateTime gun = ilkGun; gun <= sonGun; gun = gun.AddDays(1))
            {
                var mesai = mesailer.FirstOrDefault(m =>
                    m.HaftaninGunu == (int)gun.DayOfWeek && !m.KapaliMi);
                if (mesai is null ||
                    !SaatOku(mesai.AcilisSaati, out var acilis) ||
                    !SaatOku(mesai.KapanisSaati, out var kapanis) ||
                    acilis >= kapanis)
                    continue;

                DateTime mesaiBas = gun.Add(acilis.ToTimeSpan());
                DateTime mesaiBit = gun.Add(kapanis.ToTimeSpan());
                DateTime gunBitis = gun.AddDays(1);

                foreach (var aralik in araliklar)
                {
                    if (aralik.BaslangicTarihiSaati >= gunBitis ||
                        aralik.BitisTarihiSaati <= gun)
                        continue;

                    DateTime bas = EnBuyuk(aralik.BaslangicTarihiSaati, mesaiBas, gun);
                    DateTime bit = EnKucuk(aralik.BitisTarihiSaati, mesaiBit, gunBitis);
                    DateTime aday = OnBesDakikayaYuvarla(bas);

                    while (aday.AddMinutes(saglikHizmeti.SureDakika) <= bit)
                    {
                        DateTime adayBitis = aday.AddMinutes(saglikHizmeti.SureDakika);
                        bool ayniGun = aday.Date == adayBitis.Date;
                        bool cakisma = doluRandevular.Any(r =>
                            r.BaslangicTarihiSaati < adayBitis &&
                            aday < r.BitisTarihiSaati);

                        if (aday > simdi && ayniGun && !cakisma &&
                            tekrarKontrol.Add(aday))
                        {
                            sonuc.Add(new UygunSaat
                            {
                                UygunlukAraligiId = aralik.Id,
                                Baslangic = aday,
                                SureDakika = saglikHizmeti.SureDakika,
                                Ucret = (saglikHizmeti.UcretKurus / 100m)
                                    .ToString("N2") + " TL"
                            });
                        }

                        // Kullaniciya 15 dakikalik baslangic dilimleri sunulur.
                        aday = aday.AddMinutes(15);
                    }
                }
            }

            var sirali = sonuc.OrderBy(s => s.Baslangic).Take(1500).ToList();
            dgvUygunSaatler.DataSource = sirali;
            dgvUygunSaatler.ClearSelection();
            lblSonucBilgisi.Text = sirali.Count == 0
                ? "Seçilen tarihlerde boş saat bulunamadı. Başka tarih deneyiniz."
                : $"{sirali.Count} uygun saat bulundu. Listeden birini seçiniz.";
        }
        catch (InvalidOperationException hata)
        {
            MessageBox.Show(hata.Message, "Randevu Arama",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
            MessageBox.Show("Uygun saatler hesaplanamadı. Veritabanı bağlantısını kontrol ediniz.",
                "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _aramaDevamEdiyor = false;
            if (!IsDisposed)
            {
                btnUygunSaatleriBul.Enabled = true;
                btnRandevuOlustur.Enabled =
                    dgvUygunSaatler.SelectedRows.Count > 0 &&
                    dgvUygunSaatler.SelectedRows[0].DataBoundItem is UygunSaat;
            }
        }
    }

    private async Task RandevuOlusturAsync()
    {
        if (dgvUygunSaatler.SelectedRows.Count == 0 ||
            dgvUygunSaatler.SelectedRows[0].DataBoundItem is not UygunSaat saat ||
            cmbDoktor.SelectedItem is not Secenek doktor ||
            cmbHizmet.SelectedItem is not Secenek hizmet)
        {
            MessageBox.Show("Lütfen listeden bir randevu saati seçiniz.",
                "Saat Seçimi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var onay = MessageBox.Show(
            $"{saat.Baslangic:dd.MM.yyyy HH:mm} için randevu oluşturulsun mu?\n" +
            $"Süre: {saat.SureDakika} dakika   Ücret: {saat.Ucret}",
            "Randevu Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (onay != DialogResult.Yes) return;

        btnRandevuOlustur.Enabled = false;
        try
        {
            await using var baglam = VeritabaniErisimi.BaglamOlustur();
            var hasta = await baglam.Hastalar.AsNoTracking()
                .FirstOrDefaultAsync(h => h.KullaniciId == _kullaniciId &&
                                          h.Kullanici != null &&
                                          h.Kullanici.AktifMi &&
                                          h.Kullanici.Rol == "Hasta");
            if (hasta is null)
                throw new InvalidOperationException("Hasta hesabınız bulunamadı.");

            // Mevcut is kurallari tum saat/cakisma kontrollerini yeniden yapar.
            var yonetim = new RandevuYonetimi(baglam);
            int yeniRandevuId = await yonetim.RandevuOlusturAsync(
                hasta.Id,
                doktor.Id,
                hizmet.Id,
                saat.UygunlukAraligiId,
                saat.Baslangic,
                CancellationToken.None);

            MessageBox.Show(
                $"Randevunuz oluşturuldu. Randevu numaranız: {yeniRandevuId}\n" +
                "Durum: Beklemede",
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException hata)
        {
            MessageBox.Show(hata.Message + "\nLütfen uygun saatleri yeniden arayınız.",
                "Randevu Oluşturulamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AramaSonuclariniSifirla();
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Randevu kaydedilemedi. Lütfen saatleri yeniden arayınız.",
                "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            AramaSonuclariniSifirla();
        }
        catch (Exception)
        {
            MessageBox.Show("Randevu işlemi tamamlanamadı. Lütfen yeniden deneyiniz.",
                "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            AramaSonuclariniSifirla();
        }
        finally
        {
            if (!IsDisposed)
                btnRandevuOlustur.Enabled = false;
        }
    }

    private static bool SaatOku(string? deger, out TimeOnly saat) =>
        TimeOnly.TryParseExact(deger, "HH:mm", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out saat);

    private static DateTime EnBuyuk(DateTime a, DateTime b, DateTime c) =>
        new[] { a, b, c }.Max();

    private static DateTime EnKucuk(DateTime a, DateTime b, DateTime c) =>
        new[] { a, b, c }.Min();

    private static DateTime OnBesDakikayaYuvarla(DateTime an)
    {
        long adim = TimeSpan.FromMinutes(15).Ticks;
        long yukari = ((an.Ticks + adim - 1) / adim) * adim;
        return new DateTime(yukari, an.Kind);
    }
}
