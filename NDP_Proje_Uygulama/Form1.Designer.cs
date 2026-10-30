namespace NDP_Proje_Uygulama
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblBaslik = new Label();
            lblAciklama = new Label();
            lblEposta = new Label();
            txtEposta = new TextBox();
            lblSifre = new Label();
            txtSifre = new TextBox();
            btnGiris = new Button();
            btnKayit = new Button();
            SuspendLayout();
            // 
            // lblBaslik
            // 
            lblBaslik.AutoSize = true;
            lblBaslik.Font = new Font("Elephant", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBaslik.ForeColor = SystemColors.MenuHighlight;
            lblBaslik.Location = new Point(296, 9);
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Size = new Size(222, 17);
            lblBaslik.TabIndex = 0;
            lblBaslik.Text = "Hastane / Klinik Randevu Sistemi";
            lblBaslik.Click += label1_Click;
            // 
            // lblAciklama
            // 
            lblAciklama.AutoSize = true;
            lblAciklama.Location = new Point(319, 26);
            lblAciklama.Name = "lblAciklama";
            lblAciklama.Size = new Size(155, 20);
            lblAciklama.TabIndex = 1;
            lblAciklama.Text = "Hesabınıza Gİriş Yapın";
            // 
            // lblEposta
            // 
            lblEposta.AutoSize = true;
            lblEposta.Location = new Point(319, 97);
            lblEposta.Name = "lblEposta";
            lblEposta.Size = new Size(60, 20);
            lblEposta.TabIndex = 2;
            lblEposta.Text = "E-posta";
            // 
            // txtEposta
            // 
            txtEposta.Location = new Point(319, 120);
            txtEposta.Name = "txtEposta";
            txtEposta.Size = new Size(125, 27);
            txtEposta.TabIndex = 3;
            txtEposta.TextChanged += textBox1_TextChanged;
            // 
            // lblSifre
            // 
            lblSifre.AutoSize = true;
            lblSifre.Location = new Point(319, 150);
            lblSifre.Name = "lblSifre";
            lblSifre.Size = new Size(39, 20);
            lblSifre.TabIndex = 4;
            lblSifre.Text = "Şifre";
            // 
            // txtSifre
            // 
            txtSifre.Location = new Point(319, 173);
            txtSifre.Name = "txtSifre";
            txtSifre.Size = new Size(125, 27);
            txtSifre.TabIndex = 5;
            txtSifre.UseSystemPasswordChar = true;
            // 
            // btnGiris
            // 
            btnGiris.Location = new Point(333, 206);
            btnGiris.Name = "btnGiris";
            btnGiris.Size = new Size(94, 29);
            btnGiris.TabIndex = 6;
            btnGiris.Text = "Giriş Yap";
            btnGiris.UseVisualStyleBackColor = true;
            // 
            // btnKayit
            // 
            btnKayit.Location = new Point(333, 241);
            btnKayit.Name = "btnKayit";
            btnKayit.Size = new Size(94, 29);
            btnKayit.TabIndex = 7;
            btnKayit.Text = "Kayıt Ol";
            btnKayit.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(832, 503);
            Controls.Add(btnKayit);
            Controls.Add(btnGiris);
            Controls.Add(txtSifre);
            Controls.Add(lblSifre);
            Controls.Add(txtEposta);
            Controls.Add(lblEposta);
            Controls.Add(lblAciklama);
            Controls.Add(lblBaslik);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Hastane / Klinik Randevu Sistemi";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblBaslik;
        private Label lblAciklama;
        private Label lblEposta;
        private TextBox txtEposta;
        private Label lblSifre;
        private TextBox txtSifre;
        private Button btnGiris;
        private Button btnKayit;
    }
}
