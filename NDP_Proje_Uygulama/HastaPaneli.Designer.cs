using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NDP_Proje_Uygulama;

partial class HastaPaneli
{
    private IContainer? components = null;

    private Panel pnlUstBaslik = null!;
    private Label lblBaslik = null!;
    private Label lblKarsilama = null!;

    private Panel pnlKisiselBilgiler = null!;
    private Label lblKisiselBilgilerBaslik = null!;
    private Label lblAdSoyadBaslik = null!;
    private Label lblAdSoyadDeger = null!;
    private Label lblEpostaBaslik = null!;
    private Label lblEpostaDeger = null!;
    private Label lblTelefonBaslik = null!;
    private Label lblTelefonDeger = null!;
    private Label lblDogumTarihiBaslik = null!;
    private Label lblDogumTarihiDeger = null!;

    private Button btnRandevuAra = null!;

    private Panel pnlRandevular = null!;
    private Label lblRandevularBaslik = null!;
    private Label lblRandevuBilgi = null!;
    private Button btnYenile = null!;
    private DataGridView dgvRandevular = null!;

    private Label lblAltBilgi = null!;
    private Button btnCikis = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        DataGridViewCellStyle baslikStili = new DataGridViewCellStyle();
        DataGridViewCellStyle hucreStili = new DataGridViewCellStyle();
        DataGridViewCellStyle alternatifSatirStili = new DataGridViewCellStyle();

        pnlUstBaslik = new Panel();
        lblBaslik = new Label();
        lblKarsilama = new Label();
        pnlKisiselBilgiler = new Panel();
        lblKisiselBilgilerBaslik = new Label();
        lblAdSoyadBaslik = new Label();
        lblAdSoyadDeger = new Label();
        lblEpostaBaslik = new Label();
        lblEpostaDeger = new Label();
        lblTelefonBaslik = new Label();
        lblTelefonDeger = new Label();
        lblDogumTarihiBaslik = new Label();
        lblDogumTarihiDeger = new Label();
        btnRandevuAra = new Button();
        pnlRandevular = new Panel();
        lblRandevularBaslik = new Label();
        lblRandevuBilgi = new Label();
        btnYenile = new Button();
        dgvRandevular = new DataGridView();
        lblAltBilgi = new Label();
        btnCikis = new Button();

        pnlUstBaslik.SuspendLayout();
        pnlKisiselBilgiler.SuspendLayout();
        pnlRandevular.SuspendLayout();
        ((ISupportInitialize)dgvRandevular).BeginInit();
        SuspendLayout();

        // ÜST BAŞLIK
        pnlUstBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlUstBaslik.BackColor = Color.FromArgb(24, 56, 91);
        pnlUstBaslik.Controls.Add(lblBaslik);
        pnlUstBaslik.Controls.Add(lblKarsilama);
        pnlUstBaslik.Location = new Point(0, 0);
        pnlUstBaslik.Name = "pnlUstBaslik";
        pnlUstBaslik.Size = new Size(1120, 110);
        pnlUstBaslik.TabIndex = 0;

        lblBaslik.AutoSize = false;
        lblBaslik.BackColor = Color.Transparent;
        lblBaslik.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblBaslik.ForeColor = Color.White;
        lblBaslik.Location = new Point(30, 18);
        lblBaslik.Name = "lblBaslik";
        lblBaslik.Size = new Size(1030, 47);
        lblBaslik.TabIndex = 0;
        lblBaslik.Text = "Hastane / Klinik Randevu Sistemi";
        lblBaslik.TextAlign = ContentAlignment.MiddleLeft;

        lblKarsilama.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblKarsilama.AutoEllipsis = true;
        lblKarsilama.AutoSize = false;
        lblKarsilama.BackColor = Color.Transparent;
        lblKarsilama.Font = new Font("Segoe UI", 11F);
        lblKarsilama.ForeColor = Color.FromArgb(216, 233, 250);
        lblKarsilama.Location = new Point(32, 66);
        lblKarsilama.Name = "lblKarsilama";
        lblKarsilama.Size = new Size(1020, 29);
        lblKarsilama.TabIndex = 1;
        lblKarsilama.Text = "Hoş geldiniz";
        lblKarsilama.TextAlign = ContentAlignment.MiddleLeft;

        // KİŞİSEL BİLGİLER KARTI
        pnlKisiselBilgiler.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlKisiselBilgiler.BackColor = Color.White;
        pnlKisiselBilgiler.BorderStyle = BorderStyle.FixedSingle;
        pnlKisiselBilgiler.Controls.Add(lblKisiselBilgilerBaslik);
        pnlKisiselBilgiler.Controls.Add(lblAdSoyadBaslik);
        pnlKisiselBilgiler.Controls.Add(lblAdSoyadDeger);
        pnlKisiselBilgiler.Controls.Add(lblEpostaBaslik);
        pnlKisiselBilgiler.Controls.Add(lblEpostaDeger);
        pnlKisiselBilgiler.Controls.Add(lblTelefonBaslik);
        pnlKisiselBilgiler.Controls.Add(lblTelefonDeger);
        pnlKisiselBilgiler.Controls.Add(lblDogumTarihiBaslik);
        pnlKisiselBilgiler.Controls.Add(lblDogumTarihiDeger);
        pnlKisiselBilgiler.Location = new Point(28, 128);
        pnlKisiselBilgiler.Name = "pnlKisiselBilgiler";
        pnlKisiselBilgiler.Size = new Size(1064, 204);
        pnlKisiselBilgiler.TabIndex = 1;

        lblKisiselBilgilerBaslik.AutoSize = false;
        lblKisiselBilgilerBaslik.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblKisiselBilgilerBaslik.ForeColor = Color.FromArgb(24, 56, 91);
        lblKisiselBilgilerBaslik.Location = new Point(24, 14);
        lblKisiselBilgilerBaslik.Name = "lblKisiselBilgilerBaslik";
        lblKisiselBilgilerBaslik.Size = new Size(480, 34);
        lblKisiselBilgilerBaslik.TabIndex = 0;
        lblKisiselBilgilerBaslik.Text = "Kişisel Bilgilerim";
        lblKisiselBilgilerBaslik.TextAlign = ContentAlignment.MiddleLeft;

        // Sol sütun: Ad Soyad
        lblAdSoyadBaslik.AutoSize = false;
        lblAdSoyadBaslik.Font = new Font("Segoe UI", 9F);
        lblAdSoyadBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblAdSoyadBaslik.Location = new Point(26, 54);
        lblAdSoyadBaslik.Name = "lblAdSoyadBaslik";
        lblAdSoyadBaslik.Size = new Size(470, 22);
        lblAdSoyadBaslik.TabIndex = 1;
        lblAdSoyadBaslik.Text = "Ad Soyad";

        lblAdSoyadDeger.AutoEllipsis = true;
        lblAdSoyadDeger.AutoSize = false;
        lblAdSoyadDeger.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblAdSoyadDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblAdSoyadDeger.Location = new Point(26, 76);
        lblAdSoyadDeger.Name = "lblAdSoyadDeger";
        lblAdSoyadDeger.Size = new Size(470, 30);
        lblAdSoyadDeger.TabIndex = 2;
        lblAdSoyadDeger.Text = "-";
        lblAdSoyadDeger.TextAlign = ContentAlignment.MiddleLeft;

        // Sol sütun: E-posta
        lblEpostaBaslik.AutoSize = false;
        lblEpostaBaslik.Font = new Font("Segoe UI", 9F);
        lblEpostaBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblEpostaBaslik.Location = new Point(26, 119);
        lblEpostaBaslik.Name = "lblEpostaBaslik";
        lblEpostaBaslik.Size = new Size(470, 22);
        lblEpostaBaslik.TabIndex = 3;
        lblEpostaBaslik.Text = "E-posta";

        lblEpostaDeger.AutoEllipsis = true;
        lblEpostaDeger.AutoSize = false;
        lblEpostaDeger.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblEpostaDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblEpostaDeger.Location = new Point(26, 143);
        lblEpostaDeger.Name = "lblEpostaDeger";
        lblEpostaDeger.Size = new Size(470, 29);
        lblEpostaDeger.TabIndex = 4;
        lblEpostaDeger.Text = "-";
        lblEpostaDeger.TextAlign = ContentAlignment.MiddleLeft;

        // Sağ sütun: Telefon
        lblTelefonBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblTelefonBaslik.AutoSize = false;
        lblTelefonBaslik.Font = new Font("Segoe UI", 9F);
        lblTelefonBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblTelefonBaslik.Location = new Point(548, 54);
        lblTelefonBaslik.Name = "lblTelefonBaslik";
        lblTelefonBaslik.Size = new Size(482, 22);
        lblTelefonBaslik.TabIndex = 5;
        lblTelefonBaslik.Text = "Telefon";

        lblTelefonDeger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblTelefonDeger.AutoEllipsis = true;
        lblTelefonDeger.AutoSize = false;
        lblTelefonDeger.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTelefonDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblTelefonDeger.Location = new Point(548, 76);
        lblTelefonDeger.Name = "lblTelefonDeger";
        lblTelefonDeger.Size = new Size(482, 30);
        lblTelefonDeger.TabIndex = 6;
        lblTelefonDeger.Text = "-";
        lblTelefonDeger.TextAlign = ContentAlignment.MiddleLeft;

        // Sağ sütun: Doğum tarihi
        lblDogumTarihiBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblDogumTarihiBaslik.AutoSize = false;
        lblDogumTarihiBaslik.Font = new Font("Segoe UI", 9F);
        lblDogumTarihiBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblDogumTarihiBaslik.Location = new Point(548, 119);
        lblDogumTarihiBaslik.Name = "lblDogumTarihiBaslik";
        lblDogumTarihiBaslik.Size = new Size(482, 22);
        lblDogumTarihiBaslik.TabIndex = 7;
        lblDogumTarihiBaslik.Text = "Doğum Tarihi";

        lblDogumTarihiDeger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblDogumTarihiDeger.AutoEllipsis = true;
        lblDogumTarihiDeger.AutoSize = false;
        lblDogumTarihiDeger.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblDogumTarihiDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblDogumTarihiDeger.Location = new Point(548, 143);
        lblDogumTarihiDeger.Name = "lblDogumTarihiDeger";
        lblDogumTarihiDeger.Size = new Size(482, 29);
        lblDogumTarihiDeger.TabIndex = 8;
        lblDogumTarihiDeger.Text = "-";
        lblDogumTarihiDeger.TextAlign = ContentAlignment.MiddleLeft;

        // RANDEVU ARA BUTONU
        btnRandevuAra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnRandevuAra.BackColor = Color.FromArgb(37, 99, 235);
        btnRandevuAra.Cursor = Cursors.Hand;
        btnRandevuAra.FlatAppearance.BorderSize = 0;
        btnRandevuAra.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        btnRandevuAra.FlatStyle = FlatStyle.Flat;
        btnRandevuAra.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        btnRandevuAra.ForeColor = Color.White;
        btnRandevuAra.Location = new Point(28, 351);
        btnRandevuAra.Name = "btnRandevuAra";
        btnRandevuAra.Size = new Size(1064, 60);
        btnRandevuAra.TabIndex = 2;
        btnRandevuAra.Text = "Randevu Ara";
        btnRandevuAra.UseVisualStyleBackColor = false;

        // RANDEVULAR KARTI
        pnlRandevular.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlRandevular.BackColor = Color.White;
        pnlRandevular.BorderStyle = BorderStyle.FixedSingle;
        pnlRandevular.Controls.Add(lblRandevularBaslik);
        pnlRandevular.Controls.Add(lblRandevuBilgi);
        pnlRandevular.Controls.Add(btnYenile);
        pnlRandevular.Controls.Add(dgvRandevular);
        pnlRandevular.Location = new Point(28, 429);
        pnlRandevular.Name = "pnlRandevular";
        pnlRandevular.Size = new Size(1064, 255);
        pnlRandevular.TabIndex = 3;

        lblRandevularBaslik.AutoSize = false;
        lblRandevularBaslik.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblRandevularBaslik.ForeColor = Color.FromArgb(24, 56, 91);
        lblRandevularBaslik.Location = new Point(24, 13);
        lblRandevularBaslik.Name = "lblRandevularBaslik";
        lblRandevularBaslik.Size = new Size(630, 33);
        lblRandevularBaslik.TabIndex = 0;
        lblRandevularBaslik.Text = "Randevularım";

        lblRandevuBilgi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblRandevuBilgi.AutoSize = false;
        lblRandevuBilgi.Font = new Font("Segoe UI", 9.5F);
        lblRandevuBilgi.ForeColor = Color.FromArgb(103, 115, 135);
        lblRandevuBilgi.Location = new Point(26, 47);
        lblRandevuBilgi.Name = "lblRandevuBilgi";
        lblRandevuBilgi.Size = new Size(760, 26);
        lblRandevuBilgi.TabIndex = 1;
        lblRandevuBilgi.Text = "Randevularınız yükleniyor...";

        btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnYenile.BackColor = Color.FromArgb(232, 240, 252);
        btnYenile.Cursor = Cursors.Hand;
        btnYenile.FlatAppearance.BorderSize = 0;
        btnYenile.FlatStyle = FlatStyle.Flat;
        btnYenile.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnYenile.ForeColor = Color.FromArgb(30, 74, 140);
        btnYenile.Location = new Point(910, 17);
        btnYenile.Name = "btnYenile";
        btnYenile.Size = new Size(125, 36);
        btnYenile.TabIndex = 2;
        btnYenile.Text = "Yenile";
        btnYenile.UseVisualStyleBackColor = false;

        // TABLO GÖRÜNÜMÜ
        dgvRandevular.AllowUserToAddRows = false;
        dgvRandevular.AllowUserToDeleteRows = false;
        dgvRandevular.AllowUserToResizeRows = false;
        dgvRandevular.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvRandevular.AutoGenerateColumns = true;
        dgvRandevular.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRandevular.BackgroundColor = Color.White;
        dgvRandevular.BorderStyle = BorderStyle.None;
        dgvRandevular.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvRandevular.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvRandevular.ColumnHeadersHeight = 37;
        dgvRandevular.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvRandevular.Cursor = Cursors.Default;
        dgvRandevular.EnableHeadersVisualStyles = false;
        dgvRandevular.GridColor = Color.FromArgb(229, 233, 240);
        dgvRandevular.Location = new Point(24, 83);
        dgvRandevular.MultiSelect = false;
        dgvRandevular.Name = "dgvRandevular";
        dgvRandevular.ReadOnly = true;
        dgvRandevular.RowHeadersVisible = false;
        dgvRandevular.RowTemplate.Height = 32;
        dgvRandevular.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRandevular.Size = new Size(1014, 148);
        dgvRandevular.TabIndex = 3;

        baslikStili.Alignment = DataGridViewContentAlignment.MiddleLeft;
        baslikStili.BackColor = Color.FromArgb(240, 245, 252);
        baslikStili.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        baslikStili.ForeColor = Color.FromArgb(35, 52, 73);
        baslikStili.Padding = new Padding(5, 0, 0, 0);
        baslikStili.SelectionBackColor = Color.FromArgb(240, 245, 252);
        baslikStili.SelectionForeColor = Color.FromArgb(35, 52, 73);
        dgvRandevular.ColumnHeadersDefaultCellStyle = baslikStili;

        hucreStili.Alignment = DataGridViewContentAlignment.MiddleLeft;
        hucreStili.BackColor = Color.White;
        hucreStili.Font = new Font("Segoe UI", 9.5F);
        hucreStili.ForeColor = Color.FromArgb(45, 61, 82);
        hucreStili.Padding = new Padding(5, 0, 0, 0);
        hucreStili.SelectionBackColor = Color.FromArgb(218, 233, 255);
        hucreStili.SelectionForeColor = Color.FromArgb(28, 53, 91);
        dgvRandevular.DefaultCellStyle = hucreStili;

        alternatifSatirStili.BackColor = Color.FromArgb(249, 251, 254);
        dgvRandevular.AlternatingRowsDefaultCellStyle = alternatifSatirStili;

        // ALT BİLGİ VE ÇIKIŞ
        lblAltBilgi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblAltBilgi.AutoSize = false;
        lblAltBilgi.Font = new Font("Segoe UI", 9F);
        lblAltBilgi.ForeColor = Color.FromArgb(110, 125, 145);
        lblAltBilgi.Location = new Point(30, 704);
        lblAltBilgi.Name = "lblAltBilgi";
        lblAltBilgi.Size = new Size(650, 35);
        lblAltBilgi.TabIndex = 4;
        lblAltBilgi.Text = "Randevu aramak için yukarıdaki butona tıklayınız.";
        lblAltBilgi.TextAlign = ContentAlignment.MiddleLeft;

        btnCikis.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCikis.BackColor = Color.FromArgb(253, 232, 232);
        btnCikis.Cursor = Cursors.Hand;
        btnCikis.FlatAppearance.BorderSize = 0;
        btnCikis.FlatAppearance.MouseOverBackColor = Color.FromArgb(251, 210, 210);
        btnCikis.FlatStyle = FlatStyle.Flat;
        btnCikis.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCikis.ForeColor = Color.FromArgb(157, 34, 34);
        btnCikis.Location = new Point(935, 699);
        btnCikis.Name = "btnCikis";
        btnCikis.Size = new Size(157, 41);
        btnCikis.TabIndex = 5;
        btnCikis.Text = "Çıkış Yap";
        btnCikis.UseVisualStyleBackColor = false;

        // FORM
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(243, 247, 252);
        ClientSize = new Size(1120, 760);
        Controls.Add(pnlUstBaslik);
        Controls.Add(pnlKisiselBilgiler);
        Controls.Add(btnRandevuAra);
        Controls.Add(pnlRandevular);
        Controls.Add(lblAltBilgi);
        Controls.Add(btnCikis);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(990, 710);
        Name = "HastaPaneli";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hasta Paneli | Hastane / Klinik Randevu Sistemi";

        pnlUstBaslik.ResumeLayout(false);
        pnlKisiselBilgiler.ResumeLayout(false);
        pnlRandevular.ResumeLayout(false);
        ((ISupportInitialize)dgvRandevular).EndInit();
        ResumeLayout(false);
    }
}
