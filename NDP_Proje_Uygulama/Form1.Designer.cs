
namespace NDP_Proje_Uygulama
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlMarka = new Panel();
            lblMarka = new Label();
            lblUstYazi = new Label();
            lblBaslik = new Label();
            pnlGirisAlani = new Panel();
            pnlKartGolge = new Panel();
            pnlGirisKart = new Panel();
            lblGirisBaslik = new Label();
            lblAciklama = new Label();
            lblEposta = new Label();
            pnlEpostaAlan = new Panel();
            txtEposta = new TextBox();
            lblSifre = new Label();
            pnlSifreAlan = new Panel();
            txtSifre = new TextBox();
            btnGiris = new Button();
            btnKayit = new Button();
            lblAltBilgi = new Label();
            pnlMarka.SuspendLayout();
            pnlGirisAlani.SuspendLayout();
            pnlGirisKart.SuspendLayout();
            pnlEpostaAlan.SuspendLayout();
            pnlSifreAlan.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMarka
            // 
            pnlMarka.BackColor = Color.FromArgb(19, 41, 73);
            pnlMarka.Controls.Add(lblMarka);
            pnlMarka.Controls.Add(lblUstYazi);
            pnlMarka.Controls.Add(lblBaslik);
            pnlMarka.Location = new Point(0, 0);
            pnlMarka.Name = "pnlMarka";
            pnlMarka.Size = new Size(376, 594);
            pnlMarka.TabIndex = 0;
            pnlMarka.Paint += pnlMarka_Paint;
            // 
            // lblMarka
            // 
            lblMarka.AutoSize = true;
            lblMarka.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblMarka.ForeColor = Color.White;
            lblMarka.Location = new Point(110, 55);
            lblMarka.Name = "lblMarka";
            lblMarka.Size = new Size(187, 40);
            lblMarka.TabIndex = 1;
            lblMarka.Text = "NDP SAĞLIK";
            // 
            // lblUstYazi
            // 
            lblUstYazi.AutoSize = true;
            lblUstYazi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUstYazi.ForeColor = Color.FromArgb(97, 191, 230);
            lblUstYazi.Location = new Point(43, 163);
            lblUstYazi.Name = "lblUstYazi";
            lblUstYazi.Size = new Size(152, 23);
            lblUstYazi.TabIndex = 2;
            lblUstYazi.Text = "HASTANE & KLİNİK";
            // 
            // lblBaslik
            // 
            lblBaslik.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            lblBaslik.ForeColor = Color.White;
            lblBaslik.Location = new Point(39, 202);
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Size = new Size(326, 170);
            lblBaslik.TabIndex = 3;
            lblBaslik.Text = "Sağlığınız için\nakıllı randevu\nsistemi.";
            // 
            // pnlGirisAlani
            // 
            pnlGirisAlani.BackColor = Color.FromArgb(244, 247, 251);
            pnlGirisAlani.Controls.Add(pnlGirisKart);
            pnlGirisAlani.Controls.Add(pnlKartGolge);
            pnlGirisAlani.Location = new Point(376, 0);
            pnlGirisAlani.Name = "pnlGirisAlani";
            pnlGirisAlani.Size = new Size(620, 594);
            pnlGirisAlani.TabIndex = 1;
            // 
            // pnlKartGolge
            // 
            pnlKartGolge.BackColor = Color.FromArgb(226, 232, 241);
            pnlKartGolge.Location = new Point(95, 54);
            pnlKartGolge.Name = "pnlKartGolge";
            pnlKartGolge.Size = new Size(444, 504);
            pnlKartGolge.TabIndex = 0;
            // 
            // pnlGirisKart
            // 
            pnlGirisKart.BackColor = Color.White;
            pnlGirisKart.Controls.Add(lblGirisBaslik);
            pnlGirisKart.Controls.Add(lblAciklama);
            pnlGirisKart.Controls.Add(lblEposta);
            pnlGirisKart.Controls.Add(pnlEpostaAlan);
            pnlGirisKart.Controls.Add(lblSifre);
            pnlGirisKart.Controls.Add(pnlSifreAlan);
            pnlGirisKart.Controls.Add(btnGiris);
            pnlGirisKart.Controls.Add(btnKayit);
            pnlGirisKart.Controls.Add(lblAltBilgi);
            pnlGirisKart.Location = new Point(85, 44);
            pnlGirisKart.Name = "pnlGirisKart";
            pnlGirisKart.Size = new Size(444, 504);
            pnlGirisKart.TabIndex = 1;
            // 
            // lblGirisBaslik
            // 
            lblGirisBaslik.AutoSize = true;
            lblGirisBaslik.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblGirisBaslik.ForeColor = Color.FromArgb(26, 47, 78);
            lblGirisBaslik.Location = new Point(34, 26);
            lblGirisBaslik.Name = "lblGirisBaslik";
            lblGirisBaslik.Size = new Size(376, 54);
            lblGirisBaslik.TabIndex = 0;
            lblGirisBaslik.Text = "Tekrar hoş geldiniz";
            // 
            // lblAciklama
            // 
            lblAciklama.Font = new Font("Segoe UI", 10F);
            lblAciklama.ForeColor = Color.FromArgb(108, 122, 145);
            lblAciklama.Location = new Point(37, 89);
            lblAciklama.Name = "lblAciklama";
            lblAciklama.Size = new Size(367, 42);
            lblAciklama.TabIndex = 1;
            lblAciklama.Text = "Randevularınızı yönetmek için hesabınıza giriş yapın.";
            // 
            // lblEposta
            // 
            lblEposta.AutoSize = true;
            lblEposta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEposta.ForeColor = Color.FromArgb(49, 67, 94);
            lblEposta.Location = new Point(36, 145);
            lblEposta.Name = "lblEposta";
            lblEposta.Size = new Size(70, 23);
            lblEposta.TabIndex = 2;
            lblEposta.Text = "E-posta";
            // 
            // pnlEpostaAlan
            // 
            pnlEpostaAlan.BackColor = Color.FromArgb(251, 252, 254);
            pnlEpostaAlan.BorderStyle = BorderStyle.FixedSingle;
            pnlEpostaAlan.Controls.Add(txtEposta);
            pnlEpostaAlan.Location = new Point(36, 174);
            pnlEpostaAlan.Name = "pnlEpostaAlan";
            pnlEpostaAlan.Size = new Size(372, 48);
            pnlEpostaAlan.TabIndex = 3;
            // 
            // txtEposta
            // 
            txtEposta.BackColor = Color.FromArgb(251, 252, 254);
            txtEposta.BorderStyle = BorderStyle.None;
            txtEposta.Font = new Font("Segoe UI", 11F);
            txtEposta.ForeColor = Color.FromArgb(33, 52, 79);
            txtEposta.Location = new Point(13, 10);
            txtEposta.Name = "txtEposta";
            txtEposta.PlaceholderText = "ornek@gmail.com";
            txtEposta.Size = new Size(344, 25);
            txtEposta.TabIndex = 0;
            txtEposta.TextChanged += textBox1_TextChanged;
            // 
            // lblSifre
            // 
            lblSifre.AutoSize = true;
            lblSifre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSifre.ForeColor = Color.FromArgb(49, 67, 94);
            lblSifre.Location = new Point(36, 240);
            lblSifre.Name = "lblSifre";
            lblSifre.Size = new Size(48, 23);
            lblSifre.TabIndex = 4;
            lblSifre.Text = "Şifre";
            // 
            // pnlSifreAlan
            // 
            pnlSifreAlan.BackColor = Color.FromArgb(251, 252, 254);
            pnlSifreAlan.BorderStyle = BorderStyle.FixedSingle;
            pnlSifreAlan.Controls.Add(txtSifre);
            pnlSifreAlan.Location = new Point(36, 269);
            pnlSifreAlan.Name = "pnlSifreAlan";
            pnlSifreAlan.Size = new Size(372, 48);
            pnlSifreAlan.TabIndex = 5;
            // 
            // txtSifre
            // 
            txtSifre.BackColor = Color.FromArgb(251, 252, 254);
            txtSifre.BorderStyle = BorderStyle.None;
            txtSifre.Font = new Font("Segoe UI", 11F);
            txtSifre.ForeColor = Color.FromArgb(33, 52, 79);
            txtSifre.Location = new Point(13, 10);
            txtSifre.Name = "txtSifre";
            txtSifre.PlaceholderText = "Şifrenizi girin";
            txtSifre.Size = new Size(344, 25);
            txtSifre.TabIndex = 0;
            txtSifre.UseSystemPasswordChar = true;
            // 
            // btnGiris
            // 
            btnGiris.BackColor = Color.FromArgb(25, 94, 165);
            btnGiris.Cursor = Cursors.Hand;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.FlatAppearance.MouseDownBackColor = Color.FromArgb(19, 68, 124);
            btnGiris.FlatAppearance.MouseOverBackColor = Color.FromArgb(32, 112, 194);
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGiris.ForeColor = Color.White;
            btnGiris.Location = new Point(36, 348);
            btnGiris.Name = "btnGiris";
            btnGiris.Size = new Size(372, 49);
            btnGiris.TabIndex = 6;
            btnGiris.Text = "GİRİŞ YAP";
            btnGiris.UseVisualStyleBackColor = false;
            btnGiris.Click += btnGiris_Click;
            // 
            // btnKayit
            // 
            btnKayit.BackColor = Color.White;
            btnKayit.Cursor = Cursors.Hand;
            btnKayit.FlatAppearance.BorderColor = Color.FromArgb(188, 205, 224);
            btnKayit.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 247, 255);
            btnKayit.FlatStyle = FlatStyle.Flat;
            btnKayit.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnKayit.ForeColor = Color.FromArgb(25, 94, 165);
            btnKayit.Location = new Point(36, 409);
            btnKayit.Name = "btnKayit";
            btnKayit.Size = new Size(372, 46);
            btnKayit.TabIndex = 7;
            btnKayit.Text = "YENİ HESAP OLUŞTUR";
            btnKayit.UseVisualStyleBackColor = false;
            btnKayit.Click += btnKayit_Click;
            // 
            // lblAltBilgi
            // 
            lblAltBilgi.Font = new Font("Segoe UI", 9F);
            lblAltBilgi.ForeColor = Color.FromArgb(127, 140, 157);
            lblAltBilgi.Location = new Point(36, 466);
            lblAltBilgi.Name = "lblAltBilgi";
            lblAltBilgi.Size = new Size(372, 22);
            lblAltBilgi.TabIndex = 8;
            lblAltBilgi.Text = "Güvenli hastane ve klinik randevu yönetimi";
            lblAltBilgi.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AcceptButton = btnGiris;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 247, 251);
            ClientSize = new Size(996, 594);
            Controls.Add(pnlGirisAlani);
            Controls.Add(pnlMarka);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimumSize = new Size(1012, 633);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NDP Sağlık | Hastane ve Klinik Randevu Sistemi";
            Load += Form1_Load;
            pnlMarka.ResumeLayout(false);
            pnlMarka.PerformLayout();
            pnlGirisAlani.ResumeLayout(false);
            pnlGirisKart.ResumeLayout(false);
            pnlGirisKart.PerformLayout();
            pnlEpostaAlan.ResumeLayout(false);
            pnlEpostaAlan.PerformLayout();
            pnlSifreAlan.ResumeLayout(false);
            pnlSifreAlan.PerformLayout();
            // Gölge paneli arkada kalacak.
            pnlKartGolge.SendToBack();

            // Beyaz giriş kartı önde olacak.
            pnlGirisKart.BringToFront();

            // Kartın üzerindeki kontroller görünür olacak.
            btnGiris.BringToFront();
            btnKayit.BringToFront();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMarka;
        private Label lblMarka;
        private Label lblUstYazi;
        private Label lblBaslik;

        private Panel pnlGirisAlani;
        private Panel pnlKartGolge;
        private Panel pnlGirisKart;

        private Label lblGirisBaslik;
        private Label lblAciklama;
        private Label lblEposta;

        private Panel pnlEpostaAlan;
        private TextBox txtEposta;

        private Label lblSifre;
        private Panel pnlSifreAlan;
        private TextBox txtSifre;

        private Button btnGiris;
        private Button btnKayit;

        private Label lblAltBilgi;
    }
}
