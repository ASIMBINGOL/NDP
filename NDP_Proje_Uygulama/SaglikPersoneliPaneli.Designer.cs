#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NDP_Proje_Uygulama;

partial class SaglikPersoneliPaneli
{
    private IContainer? components = null;

    private Panel pnlUst = null!;
    private Label lblBaslik = null!;
    private Label lblKarsilama = null!;
    private Panel pnlOzet = null!;
    private Label lblKurum = null!;
    private Label lblToplam = null!;
    private Label lblBekleyen = null!;
    private Label lblGelen = null!;
    private Panel pnlListe = null!;
    private Label lblListeBaslik = null!;
    private Label lblKurumFiltre = null!;
    private Label lblTarihFiltre = null!;
    private Label lblDurumFiltre = null!;
    private Label lblArama = null!;
    private ComboBox cmbKurum = null!;
    private DateTimePicker dtpTarih = null!;
    private ComboBox cmbDurum = null!;
    private TextBox txtAra = null!;
    private DataGridView dgvRandevular = null!;
    private Label lblListeBilgisi = null!;
    private Panel pnlAlt = null!;
    private Button btnHastaGeldi = null!;
    private Button btnYenile = null!;
    private Button btnCikis = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlUst = new Panel();
        lblBaslik = new Label();
        lblKarsilama = new Label();
        pnlOzet = new Panel();
        lblKurum = new Label();
        lblToplam = new Label();
        lblBekleyen = new Label();
        lblGelen = new Label();
        pnlListe = new Panel();
        lblListeBaslik = new Label();
        lblKurumFiltre = new Label();
        lblTarihFiltre = new Label();
        lblDurumFiltre = new Label();
        lblArama = new Label();
        cmbKurum = new ComboBox();
        dtpTarih = new DateTimePicker();
        cmbDurum = new ComboBox();
        txtAra = new TextBox();
        dgvRandevular = new DataGridView();
        lblListeBilgisi = new Label();
        pnlAlt = new Panel();
        btnHastaGeldi = new Button();
        btnYenile = new Button();
        btnCikis = new Button();

        pnlUst.SuspendLayout();
        pnlOzet.SuspendLayout();
        pnlListe.SuspendLayout();
        ((ISupportInitialize)dgvRandevular).BeginInit();
        pnlAlt.SuspendLayout();
        SuspendLayout();

        // Ust baslik: mevcut doktor ve admin panelleriyle uyumlu.
        pnlUst.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlUst.BackColor = Color.FromArgb(24, 56, 91);
        pnlUst.Location = new Point(0, 0);
        pnlUst.Name = "pnlUst";
        pnlUst.Size = new Size(1180, 105);
        pnlUst.TabIndex = 0;
        pnlUst.Controls.Add(lblBaslik);
        pnlUst.Controls.Add(lblKarsilama);

        lblBaslik.AutoSize = false;
        lblBaslik.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblBaslik.ForeColor = Color.White;
        lblBaslik.Location = new Point(30, 17);
        lblBaslik.Size = new Size(1090, 48);
        lblBaslik.Text = "Sağlık Personeli - Hasta Karşılama Paneli";
        lblBaslik.Name = "lblBaslik";

        lblKarsilama.AutoSize = false;
        lblKarsilama.Font = new Font("Segoe UI", 10.5F);
        lblKarsilama.ForeColor = Color.FromArgb(216, 233, 250);
        lblKarsilama.Location = new Point(32, 68);
        lblKarsilama.Size = new Size(1090, 27);
        lblKarsilama.Text = "Hoş geldiniz, Sağlık Personeli";
        lblKarsilama.Name = "lblKarsilama";

        // Ozet bolumu.
        pnlOzet.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlOzet.BackColor = Color.White;
        pnlOzet.BorderStyle = BorderStyle.FixedSingle;
        pnlOzet.Location = new Point(27, 122);
        pnlOzet.Name = "pnlOzet";
        pnlOzet.Size = new Size(1126, 95);
        pnlOzet.TabIndex = 1;
        pnlOzet.Controls.Add(lblKurum);
        pnlOzet.Controls.Add(lblToplam);
        pnlOzet.Controls.Add(lblBekleyen);
        pnlOzet.Controls.Add(lblGelen);

        lblKurum.AutoSize = false;
        lblKurum.Location = new Point(24, 20);
        lblKurum.Size = new Size(630, 52);
        lblKurum.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblKurum.ForeColor = Color.FromArgb(35, 52, 73);
        lblKurum.Text = "Görevli Hastane / Klinik: -";
        lblKurum.Name = "lblKurum";

        lblToplam.AutoSize = false;
        lblToplam.Location = new Point(665, 15);
        lblToplam.Size = new Size(142, 65);
        lblToplam.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblToplam.ForeColor = Color.FromArgb(24, 56, 91);
        lblToplam.Text = "Günlük Toplam\n0";
        lblToplam.TextAlign = ContentAlignment.MiddleCenter;
        lblToplam.Name = "lblToplam";

        lblBekleyen.AutoSize = false;
        lblBekleyen.Location = new Point(814, 15);
        lblBekleyen.Size = new Size(142, 65);
        lblBekleyen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblBekleyen.ForeColor = Color.FromArgb(198, 125, 27);
        lblBekleyen.Text = "Henüz Gelmeyen\n0";
        lblBekleyen.TextAlign = ContentAlignment.MiddleCenter;
        lblBekleyen.Name = "lblBekleyen";

        lblGelen.AutoSize = false;
        lblGelen.Location = new Point(964, 15);
        lblGelen.Size = new Size(142, 65);
        lblGelen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGelen.ForeColor = Color.FromArgb(27, 127, 90);
        lblGelen.Text = "Gelen Hasta\n0";
        lblGelen.TextAlign = ContentAlignment.MiddleCenter;
        lblGelen.Name = "lblGelen";

        // Liste ve DataGridView bolumu.
        pnlListe.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlListe.BackColor = Color.White;
        pnlListe.BorderStyle = BorderStyle.FixedSingle;
        pnlListe.Location = new Point(27, 235);
        pnlListe.Name = "pnlListe";
        pnlListe.Size = new Size(1126, 424);
        pnlListe.TabIndex = 2;
        pnlListe.Controls.AddRange(new Control[]
        {
            lblListeBaslik, lblKurumFiltre, lblTarihFiltre,
            lblDurumFiltre, lblArama, cmbKurum, dtpTarih,
            cmbDurum, txtAra, dgvRandevular, lblListeBilgisi
        });

        lblListeBaslik.AutoSize = false;
        lblListeBaslik.Location = new Point(20, 12);
        lblListeBaslik.Size = new Size(660, 31);
        lblListeBaslik.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblListeBaslik.ForeColor = Color.FromArgb(24, 56, 91);
        lblListeBaslik.Text = "Randevular / Hasta Geliş Takibi";
        lblListeBaslik.Name = "lblListeBaslik";

        lblKurumFiltre.Location = new Point(21, 48);
        lblKurumFiltre.Size = new Size(250, 20);
        lblKurumFiltre.Text = "Hastane / Klinik";
        lblKurumFiltre.Name = "lblKurumFiltre";
        cmbKurum.Location = new Point(21, 72);
        cmbKurum.Name = "cmbKurum";
        cmbKurum.Size = new Size(286, 28);
        cmbKurum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbKurum.TabIndex = 0;

        lblTarihFiltre.Location = new Point(322, 48);
        lblTarihFiltre.Size = new Size(150, 20);
        lblTarihFiltre.Text = "Randevu Tarihi";
        lblTarihFiltre.Name = "lblTarihFiltre";
        dtpTarih.Location = new Point(322, 72);
        dtpTarih.Name = "dtpTarih";
        dtpTarih.Size = new Size(164, 28);
        dtpTarih.Format = DateTimePickerFormat.Custom;
        dtpTarih.CustomFormat = "dd.MM.yyyy";
        dtpTarih.TabIndex = 1;

        lblDurumFiltre.Location = new Point(500, 48);
        lblDurumFiltre.Size = new Size(155, 20);
        lblDurumFiltre.Text = "Durum / Geliş";
        lblDurumFiltre.Name = "lblDurumFiltre";
        cmbDurum.Location = new Point(500, 72);
        cmbDurum.Name = "cmbDurum";
        cmbDurum.Size = new Size(172, 28);
        cmbDurum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDurum.TabIndex = 2;

        lblArama.Location = new Point(687, 48);
        lblArama.Size = new Size(340, 20);
        lblArama.Text = "Hasta / Doktor / Bölüm / Randevu No";
        lblArama.Name = "lblArama";
        txtAra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtAra.Location = new Point(687, 72);
        txtAra.Name = "txtAra";
        txtAra.Size = new Size(412, 28);
        txtAra.PlaceholderText = "Aramak için yazınız...";
        txtAra.TabIndex = 3;

        dgvRandevular.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvRandevular.Location = new Point(21, 115);
        dgvRandevular.Name = "dgvRandevular";
        dgvRandevular.Size = new Size(1078, 262);
        dgvRandevular.BackgroundColor = Color.White;
        dgvRandevular.BorderStyle = BorderStyle.FixedSingle;
        dgvRandevular.AllowUserToAddRows = false;
        dgvRandevular.AllowUserToDeleteRows = false;
        dgvRandevular.ReadOnly = true;
        dgvRandevular.MultiSelect = false;
        dgvRandevular.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvRandevular.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvRandevular.RowHeadersVisible = false;
        dgvRandevular.TabIndex = 4;
        dgvRandevular.EnableHeadersVisualStyles = false;
        dgvRandevular.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 56, 91);
        dgvRandevular.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvRandevular.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        dgvRandevular.ColumnHeadersHeight = 36;
        dgvRandevular.RowTemplate.Height = 32;
        dgvRandevular.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 248, 252);

        lblListeBilgisi.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        lblListeBilgisi.AutoSize = false;
        lblListeBilgisi.Location = new Point(21, 388);
        lblListeBilgisi.Size = new Size(1078, 24);
        lblListeBilgisi.ForeColor = Color.FromArgb(103, 115, 135);
        lblListeBilgisi.Text = "Liste yükleniyor...";
        lblListeBilgisi.Name = "lblListeBilgisi";

        // Alt islem butonlari.
        pnlAlt.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        pnlAlt.Location = new Point(27, 675);
        pnlAlt.Size = new Size(1126, 54);
        pnlAlt.Name = "pnlAlt";
        pnlAlt.TabIndex = 3;
        pnlAlt.Controls.Add(btnHastaGeldi);
        pnlAlt.Controls.Add(btnYenile);
        pnlAlt.Controls.Add(btnCikis);

        btnHastaGeldi.Location = new Point(0, 4);
        btnHastaGeldi.Size = new Size(226, 44);
        btnHastaGeldi.Name = "btnHastaGeldi";
        btnHastaGeldi.Text = "✓ Seçili Hasta Geldi";
        btnHastaGeldi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnHastaGeldi.ForeColor = Color.White;
        btnHastaGeldi.BackColor = Color.FromArgb(27, 127, 90);
        btnHastaGeldi.FlatStyle = FlatStyle.Flat;
        btnHastaGeldi.FlatAppearance.BorderSize = 0;
        btnHastaGeldi.TabIndex = 0;

        btnYenile.Location = new Point(242, 4);
        btnYenile.Size = new Size(162, 44);
        btnYenile.Name = "btnYenile";
        btnYenile.Text = "Listeyi Yenile";
        btnYenile.FlatStyle = FlatStyle.Flat;
        btnYenile.BackColor = Color.FromArgb(229, 239, 249);
        btnYenile.ForeColor = Color.FromArgb(24, 56, 91);
        btnYenile.TabIndex = 1;

        btnCikis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCikis.Location = new Point(961, 4);
        btnCikis.Size = new Size(165, 44);
        btnCikis.Name = "btnCikis";
        btnCikis.Text = "Oturumu Kapat";
        btnCikis.FlatStyle = FlatStyle.Flat;
        btnCikis.BackColor = Color.White;
        btnCikis.ForeColor = Color.FromArgb(24, 56, 91);
        btnCikis.TabIndex = 2;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(243, 247, 251);
        ClientSize = new Size(1180, 747);
        Controls.AddRange(new Control[] { pnlUst, pnlOzet, pnlListe, pnlAlt });
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1050, 710);
        Name = "SaglikPersoneliPaneli";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sağlık Personeli - Hasta Karşılama";

        pnlUst.ResumeLayout(false);
        pnlOzet.ResumeLayout(false);
        pnlListe.ResumeLayout(false);
        ((ISupportInitialize)dgvRandevular).EndInit();
        pnlAlt.ResumeLayout(false);
        ResumeLayout(false);
    }
}
