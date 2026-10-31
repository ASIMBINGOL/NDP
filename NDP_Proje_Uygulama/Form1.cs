using NDPProje.IsKurallari;
namespace NDP_Proje_Uygulama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            using var kayitFormu = new KayitFormu();

            kayitFormu.ShowDialog(this);
        }


        private async void btnGiris_Click(
            object sender,
            EventArgs e)
        {
            string eposta = txtEposta.Text.Trim();
            string sifre = txtSifre.Text;

            if (string.IsNullOrWhiteSpace(eposta) ||
                string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show(
                    "E-posta ve sifrenizi giriniz.",
                    "Eksik Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            btnGiris.Enabled = false;

            try
            {
                // Mevcut SQLite veritabanina baglan.
                await using var baglam =
                    VeritabaniErisimi.BaglamOlustur();

                // Giris yonetimini olustur.
                var girisYonetimi = new GirisYonetimi(baglam);

                // Kullanici bilgilerini dogrula.
                var sonuc = await girisYonetimi.GirisYapAsync(
                    eposta,
                    sifre);

                // Bir sonraki asamada burada
                // kullanicinin rolune gore panel acacagiz.
                switch (sonuc.Rol)
                {
                    case "Hasta":
                        using (var hastaPaneli = new HastaPaneli(
                            sonuc.KullaniciId,
                            sonuc.AdSoyad))
                        {
                            Hide();

                            try
                            {
                                hastaPaneli.ShowDialog();
                            }
                            finally
                            {
                                Show();
                                txtSifre.Clear();
                            }
                        }
                        break;

                    case "Doktor":

                        using (var doktorPaneli = new DoktorPaneli(
                            sonuc.KullaniciId,
                            sonuc.AdSoyad))
                        {
                            Hide();

                            try
                            {
                                doktorPaneli.ShowDialog(this);
                            }
                            finally
                            {
                                Show();
                                txtSifre.Clear();
                            }
                        }

                        break;


                    case "KurumYoneticisi":
                        // AdminPaneli acilacak.
                        break;

                    case "SaglikPersoneli":
                        // Personel yetkileri ayrica belirlenecek.
                        break;

                    default:
                        MessageBox.Show(
                            "Taninmayan kullanici rolu.");
                        break;
                }
            }
            catch (InvalidOperationException hata)
            {
                MessageBox.Show(
                    hata.Message,
                    "Giris Basarisiz",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Giris sirasinda bir hata olustu. " +
                    "Veritabani baglantisini kontrol ediniz.",
                    "Sistem Hatasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Butonu tekrar etkinlestir.
                btnGiris.Enabled = true;
            }
        }

    }
}
