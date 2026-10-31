#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NDP_Proje_Uygulama;

partial class DoktorPaneli
{
    private IContainer? components = null;

    private Panel pnlUstBaslik = null!;
    private Label lblBaslik = null!;
    private Label lblKarsilama = null!;

    private Panel pnlDoktorBilgileri = null!;
    private Label lblDoktorBilgileriBaslik = null!;
    private Label lblKurumBaslik = null!;
    private Label lblKurumDeger = null!;
    private Label lblUzmanlikBaslik = null!;
    private Label lblUzmanlikDeger = null!;
    private Label lblBekleyenBaslik = null!;
    private Label lblBekleyenDeger = null!;
    private Label lblToplamBaslik = null!;
    private Label lblToplamDeger = null!;

    private Panel pnlRandevular = null!;
    private Label lblRandevularBaslik = null!;
    private Label lblListeBilgisi = null!;
    private Label lblFiltreBaslik = null!;
    private ComboBox cmbDurumFiltresi = null!;
    private Button btnYenile = null!;
    private DataGridView dgvRandevular = null!;

    private Panel pnlAltIslemler = null!;
    private Button btnOnayla = null!;
    private Button btnIptal = null!;
    private Button btnTamamla = null!;
    private Button btnCikis = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
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
        pnlDoktorBilgileri = new Panel();
        lblDoktorBilgileriBaslik = new Label();
        lblKurumBaslik = new Label();
        lblKurumDeger = new Label();
        lblUzmanlikBaslik = new Label();
        lblUzmanlikDeger = new Label();
        lblBekleyenBaslik = new Label();
        lblBekleyenDeger = new Label();
        lblToplamBaslik = new Label();
        lblToplamDeger = new Label();
        pnlRandevular = new Panel();
        lblRandevularBaslik = new Label();
        lblListeBilgisi = new Label();
        lblFiltreBaslik = new Label();
        cmbDurumFiltresi = new ComboBox();
        btnYenile = new Button();
        dgvRandevular = new DataGridView();
        pnlAltIslemler = new Panel();
        btnOnayla = new Button();
        btnIptal = new Button();
        btnTamamla = new Button();
        btnCikis = new Button();

        pnlUstBaslik.SuspendLayout();
        pnlDoktorBilgileri.SuspendLayout();
        pnlRandevular.SuspendLayout();
        ((ISupportInitialize)dgvRandevular).BeginInit();
        pnlAltIslemler.SuspendLayout();
        SuspendLayout();

        // Baslik seridi
        pnlUstBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlUstBaslik.BackColor = Color.FromArgb(24, 56, 91);
        pnlUstBaslik.Controls.Add(lblBaslik);
        pnlUstBaslik.Controls.Add(lblKarsilama);
        pnlUstBaslik.Location = new Point(0, 0);
        pnlUstBaslik.Name = "pnlUstBaslik";
        pnlUstBaslik.Size = new Size(1120, 112);
        pnlUstBaslik.TabIndex = 0;

        lblBaslik.AutoSize = false;
        lblBaslik.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblBaslik.ForeColor = Color.White;
        lblBaslik.Location = new Point(30, 17);
        lblBaslik.Name = "lblBaslik";
        lblBaslik.Size = new Size(1038, 48);
        lblBaslik.TabIndex = 0;
        lblBaslik.Text = "Hastane / Klinik - Doktor Paneli";
        lblBaslik.TextAlign = ContentAlignment.MiddleLeft;

        lblKarsilama.AutoSize = false;
        lblKarsilama.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblKarsilama.Font = new Font("Segoe UI", 11F);
        lblKarsilama.ForeColor = Color.FromArgb(216, 233, 250);
        lblKarsilama.Location = new Point(32, 68);
        lblKarsilama.Name = "lblKarsilama";
        lblKarsilama.Size = new Size(1035, 27);
        lblKarsilama.TabIndex = 1;
        lblKarsilama.Text = "Hoş geldiniz, Doktor";
        lblKarsilama.TextAlign = ContentAlignment.MiddleLeft;

        // Doktor bilgilerim: sol tarafta kurum ve uzmanlik, sagda istatistikler
        pnlDoktorBilgileri.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlDoktorBilgileri.BackColor = Color.White;
        pnlDoktorBilgileri.BorderStyle = BorderStyle.FixedSingle;
        pnlDoktorBilgileri.Location = new Point(28, 129);
        pnlDoktorBilgileri.Name = "pnlDoktorBilgileri";
        pnlDoktorBilgileri.Size = new Size(1064, 130);
        pnlDoktorBilgileri.TabIndex = 1;
        pnlDoktorBilgileri.Controls.Add(lblDoktorBilgileriBaslik);
        pnlDoktorBilgileri.Controls.Add(lblKurumBaslik);
        pnlDoktorBilgileri.Controls.Add(lblKurumDeger);
        pnlDoktorBilgileri.Controls.Add(lblUzmanlikBaslik);
        pnlDoktorBilgileri.Controls.Add(lblUzmanlikDeger);
        pnlDoktorBilgileri.Controls.Add(lblBekleyenBaslik);
        pnlDoktorBilgileri.Controls.Add(lblBekleyenDeger);
        pnlDoktorBilgileri.Controls.Add(lblToplamBaslik);
        pnlDoktorBilgileri.Controls.Add(lblToplamDeger);

        lblDoktorBilgileriBaslik.AutoSize = false;
        lblDoktorBilgileriBaslik.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblDoktorBilgileriBaslik.ForeColor = Color.FromArgb(24, 56, 91);
        lblDoktorBilgileriBaslik.Location = new Point(22, 11);
        lblDoktorBilgileriBaslik.Name = "lblDoktorBilgileriBaslik";
        lblDoktorBilgileriBaslik.Size = new Size(420, 32);
        lblDoktorBilgileriBaslik.Text = "Doktor Bilgilerim";

        lblKurumBaslik.AutoSize = false;
        lblKurumBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblKurumBaslik.Location = new Point(24, 48);
        lblKurumBaslik.Name = "lblKurumBaslik";
        lblKurumBaslik.Size = new Size(400, 19);
        lblKurumBaslik.Text = "Çalıştığım Hastane / Klinik";

        lblKurumDeger.AutoEllipsis = true;
        lblKurumDeger.AutoSize = false;
        lblKurumDeger.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblKurumDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblKurumDeger.Location = new Point(24, 72);
        lblKurumDeger.Name = "lblKurumDeger";
        lblKurumDeger.Size = new Size(420, 30);
        lblKurumDeger.Text = "-";

        lblUzmanlikBaslik.AutoSize = false;
        lblUzmanlikBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblUzmanlikBaslik.Location = new Point(470, 48);
        lblUzmanlikBaslik.Name = "lblUzmanlikBaslik";
        lblUzmanlikBaslik.Size = new Size(220, 19);
        lblUzmanlikBaslik.Text = "Uzmanlık Alanı";

        lblUzmanlikDeger.AutoEllipsis = true;
        lblUzmanlikDeger.AutoSize = false;
        lblUzmanlikDeger.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblUzmanlikDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblUzmanlikDeger.Location = new Point(470, 72);
        lblUzmanlikDeger.Name = "lblUzmanlikDeger";
        lblUzmanlikDeger.Size = new Size(220, 30);
        lblUzmanlikDeger.Text = "-";

        lblBekleyenBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblBekleyenBaslik.AutoSize = false;
        lblBekleyenBaslik.Font = new Font("Segoe UI", 9F);
        lblBekleyenBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblBekleyenBaslik.Location = new Point(747, 30);
        lblBekleyenBaslik.Name = "lblBekleyenBaslik";
        lblBekleyenBaslik.Size = new Size(145, 22);
        lblBekleyenBaslik.Text = "Bekleyen Randevu";
        lblBekleyenBaslik.TextAlign = ContentAlignment.MiddleCenter;

        lblBekleyenDeger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblBekleyenDeger.AutoSize = false;
        lblBekleyenDeger.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblBekleyenDeger.ForeColor = Color.FromArgb(37, 99, 235);
        lblBekleyenDeger.Location = new Point(747, 54);
        lblBekleyenDeger.Name = "lblBekleyenDeger";
        lblBekleyenDeger.Size = new Size(145, 43);
        lblBekleyenDeger.Text = "0";
        lblBekleyenDeger.TextAlign = ContentAlignment.MiddleCenter;

        lblToplamBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblToplamBaslik.AutoSize = false;
        lblToplamBaslik.ForeColor = Color.FromArgb(103, 115, 135);
        lblToplamBaslik.Location = new Point(902, 30);
        lblToplamBaslik.Name = "lblToplamBaslik";
        lblToplamBaslik.Size = new Size(130, 22);
        lblToplamBaslik.Text = "Toplam Randevu";
        lblToplamBaslik.TextAlign = ContentAlignment.MiddleCenter;

        lblToplamDeger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblToplamDeger.AutoSize = false;
        lblToplamDeger.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblToplamDeger.ForeColor = Color.FromArgb(35, 52, 73);
        lblToplamDeger.Location = new Point(902, 54);
        lblToplamDeger.Name = "lblToplamDeger";
        lblToplamDeger.Size = new Size(130, 43);
        lblToplamDeger.Text = "0";
        lblToplamDeger.TextAlign = ContentAlignment.MiddleCenter;

        // Liste karti
        pnlRandevular.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlRandevular.BackColor = Color.White;
        pnlRandevular.BorderStyle = BorderStyle.FixedSingle;
        pnlRandevular.Location = new Point(28, 277);
        pnlRandevular.Name = "pnlRandevular";
        pnlRandevular.Size = new Size(1064, 410);
        pnlRandevular.TabIndex = 2;
        pnlRandevular.Controls.Add(lblRandevularBaslik);
        pnlRandevular.Controls.Add(lblListeBilgisi);
        pnlRandevular.Controls.Add(lblFiltreBaslik);
        pnlRandevular.Controls.Add(cmbDurumFiltresi);
        pnlRandevular.Controls.Add(btnYenile);
        pnlRandevular.Controls.Add(dgvRandevular);

        lblRandevularBaslik.AutoSize = false;
        lblRandevularBaslik.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblRandevularBaslik.ForeColor = Color.FromArgb(24, 56, 91);
        lblRandevularBaslik.Location = new Point(22, 13);
        lblRandevularBaslik.Name = "lblRandevularBaslik";
        lblRandevularBaslik.Size = new Size(400, 32);
        lblRandevularBaslik.Text = "Randevularım";

        lblListeBilgisi.AutoSize = false;
        lblListeBilgisi.Font = new Font("Segoe UI", 9F);
        lblListeBilgisi.ForeColor = Color.FromArgb(97, 115, 139);
        lblListeBilgisi.Location = new Point(23, 47);
        lblListeBilgisi.Name = "lblListeBilgisi";
        lblListeBilgisi.Size = new Size(610, 26);
        lblListeBilgisi.Text = "İşlem yapmak için bir randevu seçiniz.";

        lblFiltreBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblFiltreBaslik.AutoSize = false;
        lblFiltreBaslik.Font = new Font("Segoe UI", 9F);
        lblFiltreBaslik.ForeColor = Color.FromArgb(97, 115, 139);
        lblFiltreBaslik.Location = new Point(660, 19);
        lblFiltreBaslik.Name = "lblFiltreBaslik";
        lblFiltreBaslik.Size = new Size(70, 27);
        lblFiltreBaslik.Text = "Durum:";
        lblFiltreBaslik.TextAlign = ContentAlignment.MiddleRight;

        cmbDurumFiltresi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cmbDurumFiltresi.Font = new Font("Segoe UI", 9.5F);
        cmbDurumFiltresi.Location = new Point(736, 20);
        cmbDurumFiltresi.Name = "cmbDurumFiltresi";
        cmbDurumFiltresi.Size = new Size(160, 25);
        cmbDurumFiltresi.TabIndex = 0;

        btnYenile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnYenile.BackColor = Color.FromArgb(233, 241, 254);
        btnYenile.Cursor = Cursors.Hand;
        btnYenile.FlatAppearance.BorderSize = 0;
        btnYenile.FlatStyle = FlatStyle.Flat;
        btnYenile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnYenile.ForeColor = Color.FromArgb(31, 78, 150);
        btnYenile.Location = new Point(914, 15);
        btnYenile.Name = "btnYenile";
        btnYenile.Size = new Size(126, 37);
        btnYenile.TabIndex = 1;
        btnYenile.Text = "Yenile";
        btnYenile.UseVisualStyleBackColor = false;

        dgvRandevular.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvRandevular.AllowUserToAddRows = false;
        dgvRandevular.AllowUserToDeleteRows = false;
        dgvRandevular.AllowUserToResizeRows = false;
        dgvRandevular.BackgroundColor = Color.White;
        dgvRandevular.BorderStyle = BorderStyle.None;
        dgvRandevular.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvRandevular.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvRandevular.ColumnHeadersHeight = 39;
        dgvRandevular.EnableHeadersVisualStyles = false;
        dgvRandevular.GridColor = Color.FromArgb(231, 237, 245);
        dgvRandevular.Location = new Point(22, 86);
        dgvRandevular.MultiSelect = false;
        dgvRandevular.Name = "dgvRandevular";
        dgvRandevular.ReadOnly = true;
        dgvRandevular.RowHeadersVisible = false;
        dgvRandevular.RowTemplate.Height = 33;
        dgvRandevular.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRandevular.Size = new Size(1018, 299);
        dgvRandevular.TabIndex = 2;

        baslikStili.BackColor = Color.FromArgb(240, 245, 252);
        baslikStili.ForeColor = Color.FromArgb(24, 56, 91);
        baslikStili.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        baslikStili.SelectionBackColor = Color.FromArgb(240, 245, 252);
        baslikStili.SelectionForeColor = Color.FromArgb(24, 56, 91);
        dgvRandevular.ColumnHeadersDefaultCellStyle = baslikStili;
        hucreStili.Font = new Font("Segoe UI", 9F);
        hucreStili.ForeColor = Color.FromArgb(42, 59, 79);
        hucreStili.SelectionBackColor = Color.FromArgb(218, 233, 255);
        hucreStili.SelectionForeColor = Color.FromArgb(24, 56, 91);
        dgvRandevular.DefaultCellStyle = hucreStili;
        alternatifSatirStili.BackColor = Color.FromArgb(249, 251, 254);
        dgvRandevular.AlternatingRowsDefaultCellStyle = alternatifSatirStili;

        // Alt islem butonlari
        pnlAltIslemler.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlAltIslemler.BackColor = Color.Transparent;
        pnlAltIslemler.Location = new Point(28, 706);
        pnlAltIslemler.Name = "pnlAltIslemler";
        pnlAltIslemler.Size = new Size(1064, 55);
        pnlAltIslemler.TabIndex = 3;
        pnlAltIslemler.Controls.Add(btnOnayla);
        pnlAltIslemler.Controls.Add(btnIptal);
        pnlAltIslemler.Controls.Add(btnTamamla);
        pnlAltIslemler.Controls.Add(btnCikis);

        btnOnayla.BackColor = Color.FromArgb(22, 128, 82);
        btnOnayla.Cursor = Cursors.Hand;
        btnOnayla.FlatAppearance.BorderSize = 0;
        btnOnayla.FlatStyle = FlatStyle.Flat;
        btnOnayla.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnOnayla.ForeColor = Color.White;
        btnOnayla.Location = new Point(0, 4);
        btnOnayla.Name = "btnOnayla";
        btnOnayla.Size = new Size(190, 45);
        btnOnayla.TabIndex = 0;
        btnOnayla.Text = "Randevuyu Onayla";
        btnOnayla.UseVisualStyleBackColor = false;

        btnIptal.BackColor = Color.FromArgb(183, 50, 55);
        btnIptal.Cursor = Cursors.Hand;
        btnIptal.FlatAppearance.BorderSize = 0;
        btnIptal.FlatStyle = FlatStyle.Flat;
        btnIptal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnIptal.ForeColor = Color.White;
        btnIptal.Location = new Point(203, 4);
        btnIptal.Name = "btnIptal";
        btnIptal.Size = new Size(184, 45);
        btnIptal.TabIndex = 1;
        btnIptal.Text = "Randevuyu İptal Et";
        btnIptal.UseVisualStyleBackColor = false;

        btnTamamla.BackColor = Color.FromArgb(37, 99, 235);
        btnTamamla.Cursor = Cursors.Hand;
        btnTamamla.FlatAppearance.BorderSize = 0;
        btnTamamla.FlatStyle = FlatStyle.Flat;
        btnTamamla.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnTamamla.ForeColor = Color.White;
        btnTamamla.Location = new Point(400, 4);
        btnTamamla.Name = "btnTamamla";
        btnTamamla.Size = new Size(225, 45);
        btnTamamla.TabIndex = 2;
        btnTamamla.Text = "Muayeneyi Tamamla";
        btnTamamla.UseVisualStyleBackColor = false;

        btnCikis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCikis.BackColor = Color.FromArgb(253, 232, 232);
        btnCikis.Cursor = Cursors.Hand;
        btnCikis.FlatAppearance.BorderSize = 0;
        btnCikis.FlatStyle = FlatStyle.Flat;
        btnCikis.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCikis.ForeColor = Color.FromArgb(157, 34, 34);
        btnCikis.Location = new Point(904, 4);
        btnCikis.Name = "btnCikis";
        btnCikis.Size = new Size(160, 45);
        btnCikis.TabIndex = 3;
        btnCikis.Text = "Çıkış Yap";
        btnCikis.UseVisualStyleBackColor = false;

        // Form
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(243, 247, 252);
        ClientSize = new Size(1120, 782);
        Controls.Add(pnlUstBaslik);
        Controls.Add(pnlDoktorBilgileri);
        Controls.Add(pnlRandevular);
        Controls.Add(pnlAltIslemler);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(990, 740);
        Name = "DoktorPaneli";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Doktor Paneli | Hastane / Klinik Randevu Sistemi";

        pnlUstBaslik.ResumeLayout(false);
        pnlDoktorBilgileri.ResumeLayout(false);
        pnlRandevular.ResumeLayout(false);
        ((ISupportInitialize)dgvRandevular).EndInit();
        pnlAltIslemler.ResumeLayout(false);
        ResumeLayout(false);
    }
}
