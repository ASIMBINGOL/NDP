
namespace NDP_Proje_Uygulama
{
    partial class KayitFormu
    {
        private System.ComponentModel.IContainer components = null;

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
            lblSolUst = new Label();
            lblSolBaslik = new Label();
            lblSolAciklama = new Label();
            pnlSag = new Panel();
            pnlKartGolge = new Panel();
            pnlKayitKart = new Panel();
            lblBaslik = new Label();
            lblAciklama = new Label();
            lblAdSoyad = new Label();
            pnlAdSoyad = new Panel();
            txtAdSoyad = new TextBox();
            lblKayitEposta = new Label();
            pnlEposta = new Panel();
            txtKayitEposta = new TextBox();
            lblTelefon = new Label();
            pnlTelefon = new Panel();
            txtTelefon = new TextBox();
            lblDogumTarihi = new Label();
            dtpDogumTarihi = new DateTimePicker();
            lblKayitSifre = new Label();
            pnlSifre = new Panel();
            txtKayitSifre = new TextBox();
            label1 = new Label();
            pnlSifreTekrar = new Panel();
            txtSifreTekrar = new TextBox();
            btnKayitTamamla = new Button();
            btnGirisDon = new Button();
            lblAltBilgi = new Label();
            pnlMarka.SuspendLayout();
            pnlSag.SuspendLayout();
            pnlKayitKart.SuspendLayout();
            pnlAdSoyad.SuspendLayout();
            pnlEposta.SuspendLayout();
            pnlTelefon.SuspendLayout();
            pnlSifre.SuspendLayout();
            pnlSifreTekrar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMarka
            // 
            pnlMarka.BackColor = Color.FromArgb(19, 41, 73);
            pnlMarka.Controls.Add(lblMarka);
            pnlMarka.Controls.Add(lblSolUst);
            pnlMarka.Controls.Add(lblSolBaslik);
            pnlMarka.Controls.Add(lblSolAciklama);
            pnlMarka.Dock = DockStyle.Left;
            pnlMarka.Location = new Point(0, 0);
            pnlMarka.Name = "pnlMarka";
            pnlMarka.Size = new Size(360, 700);
            pnlMarka.TabIndex = 0;
            // 
            // lblMarka
            // 
            lblMarka.AutoSize = true;
            lblMarka.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblMarka.ForeColor = Color.White;
            lblMarka.Location = new Point(108, 55);
            lblMarka.Name = "lblMarka";
            lblMarka.Size = new Size(187, 40);
            lblMarka.TabIndex = 1;
            lblMarka.Text = "NDP SAĞLIK";
            // 
            // lblSolUst
            // 
            lblSolUst.AutoSize = true;
            lblSolUst.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSolUst.ForeColor = Color.FromArgb(97, 191, 230);
            lblSolUst.Location = new Point(39, 190);
            lblSolUst.Name = "lblSolUst";
            lblSolUst.Size = new Size(188, 23);
            lblSolUst.TabIndex = 2;
            lblSolUst.Text = "HASTA KAYIT SİSTEMİ";
            // 
            // lblSolBaslik
            // 
            lblSolBaslik.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            lblSolBaslik.ForeColor = Color.White;
            lblSolBaslik.Location = new Point(35, 233);
            lblSolBaslik.Name = "lblSolBaslik";
            lblSolBaslik.Size = new Size(313, 175);
            lblSolBaslik.TabIndex = 3;
            lblSolBaslik.Text = "Sağlığınız için\nilk adımı atın.";
            // 
            // lblSolAciklama
            // 
            lblSolAciklama.Font = new Font("Segoe UI", 11F);
            lblSolAciklama.ForeColor = Color.FromArgb(195, 211, 229);
            lblSolAciklama.Location = new Point(40, 425);
            lblSolAciklama.Name = "lblSolAciklama";
            lblSolAciklama.Size = new Size(283, 95);
            lblSolAciklama.TabIndex = 4;
            lblSolAciklama.Text = "Ücretsiz hasta hesabınızı oluşturun, doktorları ve uygun saatleri kolayca görüntüleyin.";
            // 
            // pnlSag
            // 
            pnlSag.BackColor = Color.FromArgb(244, 247, 251);
            pnlSag.Controls.Add(pnlKayitKart);
            pnlSag.Controls.Add(pnlKartGolge);
            pnlSag.Dock = DockStyle.Fill;
            pnlSag.Location = new Point(360, 0);
            pnlSag.Name = "pnlSag";
            pnlSag.Size = new Size(640, 700);
            pnlSag.TabIndex = 1;
            // 
            // pnlKartGolge
            // 
            pnlKartGolge.BackColor = Color.FromArgb(226, 232, 241);
            pnlKartGolge.Location = new Point(51, 38);
            pnlKartGolge.Name = "pnlKartGolge";
            pnlKartGolge.Size = new Size(554, 634);
            pnlKartGolge.TabIndex = 0;
            // 
            // pnlKayitKart
            // 
            pnlKayitKart.BackColor = Color.White;
            pnlKayitKart.Controls.Add(lblBaslik);
            pnlKayitKart.Controls.Add(lblAciklama);
            pnlKayitKart.Controls.Add(lblAdSoyad);
            pnlKayitKart.Controls.Add(pnlAdSoyad);
            pnlKayitKart.Controls.Add(lblKayitEposta);
            pnlKayitKart.Controls.Add(pnlEposta);
            pnlKayitKart.Controls.Add(lblTelefon);
            pnlKayitKart.Controls.Add(pnlTelefon);
            pnlKayitKart.Controls.Add(lblDogumTarihi);
            pnlKayitKart.Controls.Add(dtpDogumTarihi);
            pnlKayitKart.Controls.Add(lblKayitSifre);
            pnlKayitKart.Controls.Add(pnlSifre);
            pnlKayitKart.Controls.Add(label1);
            pnlKayitKart.Controls.Add(pnlSifreTekrar);
            pnlKayitKart.Controls.Add(btnKayitTamamla);
            pnlKayitKart.Controls.Add(btnGirisDon);
            pnlKayitKart.Controls.Add(lblAltBilgi);
            pnlKayitKart.Location = new Point(43, 30);
            pnlKayitKart.Name = "pnlKayitKart";
            pnlKayitKart.Size = new Size(554, 634);
            pnlKayitKart.TabIndex = 1;
            // 
            // lblBaslik
            // 
            lblBaslik.AutoSize = true;
            lblBaslik.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
            lblBaslik.ForeColor = Color.FromArgb(26, 47, 78);
            lblBaslik.Location = new Point(35, 25);
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Size = new Size(396, 52);
            lblBaslik.TabIndex = 0;
            lblBaslik.Text = "Hasta hesabı oluştur";
            // 
            // lblAciklama
            // 
            lblAciklama.Font = new Font("Segoe UI", 10F);
            lblAciklama.ForeColor = Color.FromArgb(108, 122, 145);
            lblAciklama.Location = new Point(38, 83);
            lblAciklama.Name = "lblAciklama";
            lblAciklama.Size = new Size(477, 30);
            lblAciklama.TabIndex = 1;
            lblAciklama.Text = "Bilgilerinizi doldurarak randevu sistemine katılın.";
            // 
            // lblAdSoyad
            // 
            lblAdSoyad.AutoSize = true;
            lblAdSoyad.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAdSoyad.ForeColor = Color.FromArgb(49, 67, 94);
            lblAdSoyad.Location = new Point(36, 125);
            lblAdSoyad.Name = "lblAdSoyad";
            lblAdSoyad.Size = new Size(87, 23);
            lblAdSoyad.TabIndex = 2;
            lblAdSoyad.Text = "Ad Soyad";
            // 
            // pnlAdSoyad
            // 
            pnlAdSoyad.BackColor = Color.FromArgb(251, 252, 254);
            pnlAdSoyad.BorderStyle = BorderStyle.FixedSingle;
            pnlAdSoyad.Controls.Add(txtAdSoyad);
            pnlAdSoyad.Location = new Point(36, 151);
            pnlAdSoyad.Name = "pnlAdSoyad";
            pnlAdSoyad.Size = new Size(482, 46);
            pnlAdSoyad.TabIndex = 3;
            // 
            // txtAdSoyad
            // 
            txtAdSoyad.BackColor = Color.FromArgb(251, 252, 254);
            txtAdSoyad.BorderStyle = BorderStyle.None;
            txtAdSoyad.Font = new Font("Segoe UI", 11F);
            txtAdSoyad.ForeColor = Color.FromArgb(33, 52, 79);
            txtAdSoyad.Location = new Point(12, 10);
            txtAdSoyad.Name = "txtAdSoyad";
            txtAdSoyad.PlaceholderText = "Adınız ve soyadınız";
            txtAdSoyad.Size = new Size(455, 25);
            txtAdSoyad.TabIndex = 0;
            // 
            // lblKayitEposta
            // 
            lblKayitEposta.AutoSize = true;
            lblKayitEposta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKayitEposta.ForeColor = Color.FromArgb(49, 67, 94);
            lblKayitEposta.Location = new Point(36, 211);
            lblKayitEposta.Name = "lblKayitEposta";
            lblKayitEposta.Size = new Size(123, 23);
            lblKayitEposta.TabIndex = 4;
            lblKayitEposta.Text = "E-posta adresi";
            // 
            // pnlEposta
            // 
            pnlEposta.BackColor = Color.FromArgb(251, 252, 254);
            pnlEposta.BorderStyle = BorderStyle.FixedSingle;
            pnlEposta.Controls.Add(txtKayitEposta);
            pnlEposta.Location = new Point(36, 237);
            pnlEposta.Name = "pnlEposta";
            pnlEposta.Size = new Size(482, 46);
            pnlEposta.TabIndex = 5;
            // 
            // txtKayitEposta
            // 
            txtKayitEposta.BackColor = Color.FromArgb(251, 252, 254);
            txtKayitEposta.BorderStyle = BorderStyle.None;
            txtKayitEposta.Font = new Font("Segoe UI", 11F);
            txtKayitEposta.ForeColor = Color.FromArgb(33, 52, 79);
            txtKayitEposta.Location = new Point(12, 10);
            txtKayitEposta.Name = "txtKayitEposta";
            txtKayitEposta.PlaceholderText = "ornek@gmail.com";
            txtKayitEposta.Size = new Size(455, 25);
            txtKayitEposta.TabIndex = 0;
            // 
            // lblTelefon
            // 
            lblTelefon.AutoSize = true;
            lblTelefon.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTelefon.ForeColor = Color.FromArgb(49, 67, 94);
            lblTelefon.Location = new Point(36, 297);
            lblTelefon.Name = "lblTelefon";
            lblTelefon.Size = new Size(68, 23);
            lblTelefon.TabIndex = 6;
            lblTelefon.Text = "Telefon";
            // 
            // pnlTelefon
            // 
            pnlTelefon.BackColor = Color.FromArgb(251, 252, 254);
            pnlTelefon.BorderStyle = BorderStyle.FixedSingle;
            pnlTelefon.Controls.Add(txtTelefon);
            pnlTelefon.Location = new Point(36, 323);
            pnlTelefon.Name = "pnlTelefon";
            pnlTelefon.Size = new Size(234, 46);
            pnlTelefon.TabIndex = 7;
            // 
            // txtTelefon
            // 
            txtTelefon.BackColor = Color.FromArgb(251, 252, 254);
            txtTelefon.BorderStyle = BorderStyle.None;
            txtTelefon.Font = new Font("Segoe UI", 11F);
            txtTelefon.ForeColor = Color.FromArgb(33, 52, 79);
            txtTelefon.Location = new Point(12, 10);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.PlaceholderText = "05xx xxx xx xx";
            txtTelefon.Size = new Size(208, 25);
            txtTelefon.TabIndex = 0;
            // 
            // lblDogumTarihi
            // 
            lblDogumTarihi.AutoSize = true;
            lblDogumTarihi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDogumTarihi.ForeColor = Color.FromArgb(49, 67, 94);
            lblDogumTarihi.Location = new Point(283, 297);
            lblDogumTarihi.Name = "lblDogumTarihi";
            lblDogumTarihi.Size = new Size(118, 23);
            lblDogumTarihi.TabIndex = 8;
            lblDogumTarihi.Text = "Doğum tarihi";
            // 
            // dtpDogumTarihi
            // 
            dtpDogumTarihi.CalendarFont = new Font("Segoe UI", 10F);
            dtpDogumTarihi.CalendarMonthBackground = Color.White;
            dtpDogumTarihi.CustomFormat = "dd.MM.yyyy";
            dtpDogumTarihi.Font = new Font("Segoe UI", 11F);
            dtpDogumTarihi.Format = DateTimePickerFormat.Custom;
            dtpDogumTarihi.Location = new Point(283, 330);
            dtpDogumTarihi.Name = "dtpDogumTarihi";
            dtpDogumTarihi.Size = new Size(235, 32);
            dtpDogumTarihi.TabIndex = 9;
            // 
            // lblKayitSifre
            // 
            lblKayitSifre.AutoSize = true;
            lblKayitSifre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKayitSifre.ForeColor = Color.FromArgb(49, 67, 94);
            lblKayitSifre.Location = new Point(36, 385);
            lblKayitSifre.Name = "lblKayitSifre";
            lblKayitSifre.Size = new Size(48, 23);
            lblKayitSifre.TabIndex = 10;
            lblKayitSifre.Text = "Şifre";
            // 
            // pnlSifre
            // 
            pnlSifre.BackColor = Color.FromArgb(251, 252, 254);
            pnlSifre.BorderStyle = BorderStyle.FixedSingle;
            pnlSifre.Controls.Add(txtKayitSifre);
            pnlSifre.Location = new Point(36, 411);
            pnlSifre.Name = "pnlSifre";
            pnlSifre.Size = new Size(234, 46);
            pnlSifre.TabIndex = 11;
            // 
            // txtKayitSifre
            // 
            txtKayitSifre.BackColor = Color.FromArgb(251, 252, 254);
            txtKayitSifre.BorderStyle = BorderStyle.None;
            txtKayitSifre.Font = new Font("Segoe UI", 11F);
            txtKayitSifre.ForeColor = Color.FromArgb(33, 52, 79);
            txtKayitSifre.Location = new Point(12, 10);
            txtKayitSifre.Name = "txtKayitSifre";
            txtKayitSifre.PlaceholderText = "En az 8 karakter";
            txtKayitSifre.Size = new Size(208, 25);
            txtKayitSifre.TabIndex = 0;
            txtKayitSifre.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(49, 67, 94);
            label1.Location = new Point(283, 385);
            label1.Name = "label1";
            label1.Size = new Size(102, 23);
            label1.TabIndex = 12;
            label1.Text = "Şifre tekrar";
            // 
            // pnlSifreTekrar
            // 
            pnlSifreTekrar.BackColor = Color.FromArgb(251, 252, 254);
            pnlSifreTekrar.BorderStyle = BorderStyle.FixedSingle;
            pnlSifreTekrar.Controls.Add(txtSifreTekrar);
            pnlSifreTekrar.Location = new Point(283, 411);
            pnlSifreTekrar.Name = "pnlSifreTekrar";
            pnlSifreTekrar.Size = new Size(235, 46);
            pnlSifreTekrar.TabIndex = 13;
            // 
            // txtSifreTekrar
            // 
            txtSifreTekrar.BackColor = Color.FromArgb(251, 252, 254);
            txtSifreTekrar.BorderStyle = BorderStyle.None;
            txtSifreTekrar.Font = new Font("Segoe UI", 11F);
            txtSifreTekrar.ForeColor = Color.FromArgb(33, 52, 79);
            txtSifreTekrar.Location = new Point(12, 10);
            txtSifreTekrar.Name = "txtSifreTekrar";
            txtSifreTekrar.PlaceholderText = "Şifreyi tekrar girin";
            txtSifreTekrar.Size = new Size(208, 25);
            txtSifreTekrar.TabIndex = 0;
            txtSifreTekrar.UseSystemPasswordChar = true;
            // 
            // btnKayitTamamla
            // 
            btnKayitTamamla.BackColor = Color.FromArgb(25, 94, 165);
            btnKayitTamamla.Cursor = Cursors.Hand;
            btnKayitTamamla.FlatAppearance.BorderSize = 0;
            btnKayitTamamla.FlatAppearance.MouseDownBackColor = Color.FromArgb(19, 68, 124);
            btnKayitTamamla.FlatAppearance.MouseOverBackColor = Color.FromArgb(32, 112, 194);
            btnKayitTamamla.FlatStyle = FlatStyle.Flat;
            btnKayitTamamla.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnKayitTamamla.ForeColor = Color.White;
            btnKayitTamamla.Location = new Point(36, 485);
            btnKayitTamamla.Name = "btnKayitTamamla";
            btnKayitTamamla.Size = new Size(482, 50);
            btnKayitTamamla.TabIndex = 14;
            btnKayitTamamla.Text = "HESAP OLUŞTUR";
            btnKayitTamamla.UseVisualStyleBackColor = false;
            btnKayitTamamla.Click += btnKayitTamamla_Click;
            // 
            // btnGirisDon
            // 
            btnGirisDon.BackColor = Color.White;
            btnGirisDon.Cursor = Cursors.Hand;
            btnGirisDon.DialogResult = DialogResult.Cancel;
            btnGirisDon.FlatAppearance.BorderColor = Color.FromArgb(188, 205, 224);
            btnGirisDon.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 247, 255);
            btnGirisDon.FlatStyle = FlatStyle.Flat;
            btnGirisDon.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGirisDon.ForeColor = Color.FromArgb(25, 94, 165);
            btnGirisDon.Location = new Point(36, 549);
            btnGirisDon.Name = "btnGirisDon";
            btnGirisDon.Size = new Size(482, 38);
            btnGirisDon.TabIndex = 15;
            btnGirisDon.Text = "Zaten hesabın var mı? Girişe dön";
            btnGirisDon.UseVisualStyleBackColor = false;
            // 
            // lblAltBilgi
            // 
            lblAltBilgi.Font = new Font("Segoe UI", 8.5F);
            lblAltBilgi.ForeColor = Color.FromArgb(127, 140, 157);
            lblAltBilgi.Location = new Point(36, 599);
            lblAltBilgi.Name = "lblAltBilgi";
            lblAltBilgi.Size = new Size(482, 22);
            lblAltBilgi.TabIndex = 16;
            lblAltBilgi.Text = "Bu ekran yalnızca hasta hesabı oluşturmak içindir.";
            lblAltBilgi.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // KayitFormu
            // 
            AcceptButton = btnKayitTamamla;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 247, 251);
            CancelButton = btnGirisDon;
            ClientSize = new Size(1000, 700);
            Controls.Add(pnlSag);
            Controls.Add(pnlMarka);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "KayitFormu";
            StartPosition = FormStartPosition.CenterParent;
            Text = "NDP Sağlık | Hasta Kaydı";
            Load += KayitFormu_Load;
            pnlMarka.ResumeLayout(false);
            pnlMarka.PerformLayout();
            pnlSag.ResumeLayout(false);
            // Gri gölge panelini arkaya gönder.
            pnlKartGolge.SendToBack();

            // Beyaz kayıt kartını öne getir.
            pnlKayitKart.BringToFront();

            // Panel düzenini tamamla.
            pnlSag.ResumeLayout(false);

            pnlKayitKart.ResumeLayout(false);
            pnlKayitKart.PerformLayout();
            pnlAdSoyad.ResumeLayout(false);
            pnlAdSoyad.PerformLayout();
            pnlEposta.ResumeLayout(false);
            pnlEposta.PerformLayout();
            pnlTelefon.ResumeLayout(false);
            pnlTelefon.PerformLayout();
            pnlSifre.ResumeLayout(false);
            pnlSifre.PerformLayout();
            pnlSifreTekrar.ResumeLayout(false);
            pnlSifreTekrar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMarka;
        private Label lblMarka;
        private Label lblSolUst;
        private Label lblSolBaslik;
        private Label lblSolAciklama;

        private Panel pnlSag;
        private Panel pnlKartGolge;
        private Panel pnlKayitKart;

        private Label lblBaslik;
        private Label lblAciklama;

        private Label lblAdSoyad;
        private Panel pnlAdSoyad;
        private TextBox txtAdSoyad;

        private Label lblKayitEposta;
        private Panel pnlEposta;
        private TextBox txtKayitEposta;

        private Label lblTelefon;
        private Panel pnlTelefon;
        private TextBox txtTelefon;

        private Label lblDogumTarihi;
        private DateTimePicker dtpDogumTarihi;

        private Label lblKayitSifre;
        private Panel pnlSifre;
        private TextBox txtKayitSifre;

        private Label label1;
        private Panel pnlSifreTekrar;
        private TextBox txtSifreTekrar;

        private Button btnKayitTamamla;
        private Button btnGirisDon;

        private Label lblAltBilgi;
    }
}
