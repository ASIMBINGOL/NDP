namespace NDP_Proje_Uygulama
{
    partial class KayitFormu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblAdSoyad = new Label();
            txtAdSoyad = new TextBox();
            lblKayitEposta = new Label();
            txtKayitEposta = new TextBox();
            lblTelefon = new Label();
            txtTelefon = new TextBox();
            lblDogumTarihi = new Label();
            dtpDogumTarihi = new DateTimePicker();
            lblKayitSifre = new Label();
            txtKayitSifre = new TextBox();
            label1 = new Label();
            txtSifreTekrar = new TextBox();
            btnKayitTamamla = new Button();
            SuspendLayout();
            // 
            // lblAdSoyad
            // 
            lblAdSoyad.AutoSize = true;
            lblAdSoyad.Location = new Point(196, 28);
            lblAdSoyad.Name = "lblAdSoyad";
            lblAdSoyad.Size = new Size(73, 20);
            lblAdSoyad.TabIndex = 0;
            lblAdSoyad.Text = "Ad Soyad";
            // 
            // txtAdSoyad
            // 
            txtAdSoyad.Location = new Point(288, 25);
            txtAdSoyad.Name = "txtAdSoyad";
            txtAdSoyad.Size = new Size(125, 27);
            txtAdSoyad.TabIndex = 1;
            // 
            // lblKayitEposta
            // 
            lblKayitEposta.AutoSize = true;
            lblKayitEposta.Location = new Point(196, 66);
            lblKayitEposta.Name = "lblKayitEposta";
            lblKayitEposta.Size = new Size(58, 20);
            lblKayitEposta.TabIndex = 2;
            lblKayitEposta.Text = "E-Posta";
            // 
            // txtKayitEposta
            // 
            txtKayitEposta.Location = new Point(288, 66);
            txtKayitEposta.Name = "txtKayitEposta";
            txtKayitEposta.Size = new Size(125, 27);
            txtKayitEposta.TabIndex = 3;
            // 
            // lblTelefon
            // 
            lblTelefon.AutoSize = true;
            lblTelefon.Location = new Point(196, 119);
            lblTelefon.Name = "lblTelefon";
            lblTelefon.Size = new Size(58, 20);
            lblTelefon.TabIndex = 5;
            lblTelefon.Text = "Telefon";
            // 
            // txtTelefon
            // 
            txtTelefon.Location = new Point(288, 112);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Size = new Size(125, 27);
            txtTelefon.TabIndex = 6;
            // 
            // lblDogumTarihi
            // 
            lblDogumTarihi.AutoSize = true;
            lblDogumTarihi.Location = new Point(196, 169);
            lblDogumTarihi.Name = "lblDogumTarihi";
            lblDogumTarihi.Size = new Size(98, 20);
            lblDogumTarihi.TabIndex = 7;
            lblDogumTarihi.Text = "Doğum Tarihi";
            // 
            // dtpDogumTarihi
            // 
            dtpDogumTarihi.Location = new Point(300, 164);
            dtpDogumTarihi.Name = "dtpDogumTarihi";
            dtpDogumTarihi.Size = new Size(250, 27);
            dtpDogumTarihi.TabIndex = 8;
            // 
            // lblKayitSifre
            // 
            lblKayitSifre.AutoSize = true;
            lblKayitSifre.Location = new Point(196, 210);
            lblKayitSifre.Name = "lblKayitSifre";
            lblKayitSifre.Size = new Size(39, 20);
            lblKayitSifre.TabIndex = 9;
            lblKayitSifre.Text = "Şifre";
            // 
            // txtKayitSifre
            // 
            txtKayitSifre.Location = new Point(288, 203);
            txtKayitSifre.Name = "txtKayitSifre";
            txtKayitSifre.Size = new Size(125, 27);
            txtKayitSifre.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(196, 249);
            label1.Name = "label1";
            label1.Size = new Size(39, 20);
            label1.TabIndex = 11;
            label1.Text = "Şifre";
            // 
            // txtSifreTekrar
            // 
            txtSifreTekrar.Location = new Point(288, 246);
            txtSifreTekrar.Name = "txtSifreTekrar";
            txtSifreTekrar.Size = new Size(125, 27);
            txtSifreTekrar.TabIndex = 12;
            // 
            // btnKayitTamamla
            // 
            btnKayitTamamla.Location = new Point(300, 288);
            btnKayitTamamla.Name = "btnKayitTamamla";
            btnKayitTamamla.Size = new Size(94, 29);
            btnKayitTamamla.TabIndex = 13;
            btnKayitTamamla.Text = "Kayıt Ol";
            btnKayitTamamla.UseVisualStyleBackColor = true;
            btnKayitTamamla.Click += btnKayitTamamla_Click;
            // 
            // KayitFormu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(632, 653);
            Controls.Add(btnKayitTamamla);
            Controls.Add(txtSifreTekrar);
            Controls.Add(label1);
            Controls.Add(txtKayitSifre);
            Controls.Add(lblKayitSifre);
            Controls.Add(dtpDogumTarihi);
            Controls.Add(lblDogumTarihi);
            Controls.Add(txtTelefon);
            Controls.Add(lblTelefon);
            Controls.Add(txtKayitEposta);
            Controls.Add(lblKayitEposta);
            Controls.Add(txtAdSoyad);
            Controls.Add(lblAdSoyad);
            MaximizeBox = false;
            Name = "KayitFormu";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Hasta Kayit Formu";
            Load += KayitFormu_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAdSoyad;
        private TextBox txtAdSoyad;
        private Label lblKayitEposta;
        private TextBox txtKayitEposta;
        private Label lblTelefon;
        private TextBox txtTelefon;
        private Label lblDogumTarihi;
        private DateTimePicker dtpDogumTarihi;
        private Label lblKayitSifre;
        private TextBox txtKayitSifre;
        private Label label1;
        private TextBox txtSifreTekrar;
        private Button btnKayitTamamla;
    }
}