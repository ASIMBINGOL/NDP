using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using NDPProje.IsKurallari;

namespace NDP_Proje_Uygulama
{
    public partial class KayitFormu : Form
    {
        public KayitFormu()
        {
            InitializeComponent();
        }

        private void KayitFormu_Load(object sender, EventArgs e)
        {

        }


        private async void btnKayitTamamla_Click(
            object sender, EventArgs e)
        {
            if (txtKayitSifre.Text != txtSifreTekrar.Text)
            {
                MessageBox.Show(
                    "Girilen sifreler eslesmiyor.",
                    "Kayit Hatasi");

                return;
            }

            btnKayitTamamla.Enabled = false;

            try
            {
                await using var baglam =
                    VeritabaniErisimi.BaglamOlustur();

                var kayitYonetimi = new HastaKayitYonetimi(baglam);

                await kayitYonetimi.KayitOlAsync(
                    txtAdSoyad.Text,
                    txtKayitEposta.Text,
                    txtTelefon.Text,
                    dtpDogumTarihi.Value,
                    txtKayitSifre.Text);

                MessageBox.Show(
                    "Hasta kaydiniz basariyla olusturuldu!",
                    "Kayit Basarili",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (InvalidOperationException hata)
            {
                MessageBox.Show(
                    hata.Message,
                    "Kayit Hatasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Kayit veritabanina eklenemedi. " +
                    "E-posta adresini kontrol ediniz.",
                    "Veritabani Hatasi");
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Beklenmeyen bir hata olustu. " +
                    "Veritabani baglantisini kontrol ediniz.",
                    "Hata");
            }
            finally
            {
                btnKayitTamamla.Enabled = true;
            }
        }

    }
}
