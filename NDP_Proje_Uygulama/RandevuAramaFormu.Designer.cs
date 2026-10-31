using System;
using System.Drawing;
using System.Windows.Forms;

namespace NDP_Proje_Uygulama;

partial class RandevuAramaFormu
{
    private System.ComponentModel.IContainer? components = null;

    private Panel panelUstBaslik = null!;
    private Label lblBaslik = null!;
    private Label lblAciklama = null!;
    private GroupBox grpFiltre = null!;
    private Label lblKurum = null!;
    private Label lblBolum = null!;
    private Label lblDoktor = null!;
    private Label lblHizmet = null!;
    private Label lblBaslangic = null!;
    private Label lblBitis = null!;
    private ComboBox cmbKurum = null!;
    private ComboBox cmbBolum = null!;
    private ComboBox cmbDoktor = null!;
    private ComboBox cmbHizmet = null!;
    private DateTimePicker dtpBaslangic = null!;
    private DateTimePicker dtpBitis = null!;
    private Button btnUygunSaatleriBul = null!;
    private GroupBox grpSonuclar = null!;
    private Label lblSonucBilgisi = null!;
    private DataGridView dgvUygunSaatler = null!;
    private Button btnRandevuOlustur = null!;
    private Button btnGeriDon = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        panelUstBaslik = new Panel();
        lblBaslik = new Label();
        lblAciklama = new Label();
        grpFiltre = new GroupBox();
        lblKurum = new Label();
        cmbKurum = new ComboBox();
        lblBolum = new Label();
        cmbBolum = new ComboBox();
        lblDoktor = new Label();
        cmbDoktor = new ComboBox();
        lblHizmet = new Label();
        cmbHizmet = new ComboBox();
        lblBaslangic = new Label();
        dtpBaslangic = new DateTimePicker();
        lblBitis = new Label();
        dtpBitis = new DateTimePicker();
        btnUygunSaatleriBul = new Button();
        grpSonuclar = new GroupBox();
        lblSonucBilgisi = new Label();
        dgvUygunSaatler = new DataGridView();
        btnRandevuOlustur = new Button();
        btnGeriDon = new Button();
        panelUstBaslik.SuspendLayout();
        grpFiltre.SuspendLayout();
        grpSonuclar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUygunSaatler).BeginInit();
        SuspendLayout();
        // 
        // panelUstBaslik
        // 
        panelUstBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panelUstBaslik.BackColor = Color.FromArgb(25, 70, 111);
        panelUstBaslik.Controls.Add(lblBaslik);
        panelUstBaslik.Controls.Add(lblAciklama);
        panelUstBaslik.Location = new Point(18, 18);
        panelUstBaslik.Name = "panelUstBaslik";
        panelUstBaslik.Size = new Size(1004, 72);
        panelUstBaslik.TabIndex = 0;
        // 
        // lblBaslik
        // 
        lblBaslik.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblBaslik.ForeColor = Color.White;
        lblBaslik.Location = new Point(18, 8);
        lblBaslik.Name = "lblBaslik";
        lblBaslik.Size = new Size(950, 37);
        lblBaslik.TabIndex = 0;
        lblBaslik.Text = "Hastane / Klinik - Randevu Ara";
        // 
        // lblAciklama
        // 
        lblAciklama.Font = new Font("Segoe UI", 9F);
        lblAciklama.ForeColor = Color.FromArgb(218, 233, 251);
        lblAciklama.Location = new Point(20, 45);
        lblAciklama.Name = "lblAciklama";
        lblAciklama.Size = new Size(945, 22);
        lblAciklama.TabIndex = 1;
        lblAciklama.Text = "Hastane, bölüm, doktor, hizmet ve tarih aralığını seçiniz.";
        // 
        // grpFiltre
        // 
        grpFiltre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grpFiltre.BackColor = Color.White;
        grpFiltre.Controls.Add(lblKurum);
        grpFiltre.Controls.Add(cmbKurum);
        grpFiltre.Controls.Add(lblBolum);
        grpFiltre.Controls.Add(cmbBolum);
        grpFiltre.Controls.Add(lblDoktor);
        grpFiltre.Controls.Add(cmbDoktor);
        grpFiltre.Controls.Add(lblHizmet);
        grpFiltre.Controls.Add(cmbHizmet);
        grpFiltre.Controls.Add(lblBaslangic);
        grpFiltre.Controls.Add(dtpBaslangic);
        grpFiltre.Controls.Add(lblBitis);
        grpFiltre.Controls.Add(dtpBitis);
        grpFiltre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grpFiltre.ForeColor = Color.FromArgb(25, 70, 111);
        grpFiltre.Location = new Point(18, 108);
        grpFiltre.Name = "grpFiltre";
        grpFiltre.Size = new Size(1004, 260);
        grpFiltre.TabIndex = 1;
        grpFiltre.TabStop = false;
        grpFiltre.Text = "Randevu Kriterleri";
        // 
        // lblKurum
        // 
        lblKurum.Font = new Font("Segoe UI", 9F);
        lblKurum.ForeColor = Color.FromArgb(52, 64, 84);
        lblKurum.Location = new Point(20, 32);
        lblKurum.Name = "lblKurum";
        lblKurum.Size = new Size(450, 23);
        lblKurum.TabIndex = 0;
        lblKurum.Text = "Hastane / Klinik";
        // 
        // cmbKurum
        // 
        cmbKurum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbKurum.Font = new Font("Segoe UI", 10F);
        cmbKurum.FormattingEnabled = true;
        cmbKurum.Location = new Point(20, 57);
        cmbKurum.Name = "cmbKurum";
        cmbKurum.Size = new Size(454, 31);
        cmbKurum.TabIndex = 0;
        // 
        // lblBolum
        // 
        lblBolum.Font = new Font("Segoe UI", 9F);
        lblBolum.ForeColor = Color.FromArgb(52, 64, 84);
        lblBolum.Location = new Point(522, 32);
        lblBolum.Name = "lblBolum";
        lblBolum.Size = new Size(450, 23);
        lblBolum.TabIndex = 1;
        lblBolum.Text = "Bölüm";
        // 
        // cmbBolum
        // 
        cmbBolum.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBolum.Font = new Font("Segoe UI", 10F);
        cmbBolum.FormattingEnabled = true;
        cmbBolum.Location = new Point(522, 57);
        cmbBolum.Name = "cmbBolum";
        cmbBolum.Size = new Size(454, 31);
        cmbBolum.TabIndex = 1;
        // 
        // lblDoktor
        // 
        lblDoktor.Font = new Font("Segoe UI", 9F);
        lblDoktor.ForeColor = Color.FromArgb(52, 64, 84);
        lblDoktor.Location = new Point(20, 101);
        lblDoktor.Name = "lblDoktor";
        lblDoktor.Size = new Size(450, 23);
        lblDoktor.TabIndex = 2;
        lblDoktor.Text = "Doktor";
        // 
        // cmbDoktor
        // 
        cmbDoktor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDoktor.Font = new Font("Segoe UI", 10F);
        cmbDoktor.FormattingEnabled = true;
        cmbDoktor.Location = new Point(20, 126);
        cmbDoktor.Name = "cmbDoktor";
        cmbDoktor.Size = new Size(454, 31);
        cmbDoktor.TabIndex = 2;
        // 
        // lblHizmet
        // 
        lblHizmet.Font = new Font("Segoe UI", 9F);
        lblHizmet.ForeColor = Color.FromArgb(52, 64, 84);
        lblHizmet.Location = new Point(522, 101);
        lblHizmet.Name = "lblHizmet";
        lblHizmet.Size = new Size(450, 23);
        lblHizmet.TabIndex = 3;
        lblHizmet.Text = "Sağlık Hizmeti";
        // 
        // cmbHizmet
        // 
        cmbHizmet.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbHizmet.Font = new Font("Segoe UI", 10F);
        cmbHizmet.FormattingEnabled = true;
        cmbHizmet.Location = new Point(522, 126);
        cmbHizmet.Name = "cmbHizmet";
        cmbHizmet.Size = new Size(454, 31);
        cmbHizmet.TabIndex = 3;
        // 
        // lblBaslangic
        // 
        lblBaslangic.Font = new Font("Segoe UI", 9F);
        lblBaslangic.ForeColor = Color.FromArgb(52, 64, 84);
        lblBaslangic.Location = new Point(20, 171);
        lblBaslangic.Name = "lblBaslangic";
        lblBaslangic.Size = new Size(450, 23);
        lblBaslangic.TabIndex = 4;
        lblBaslangic.Text = "Başlangıç Tarihi";
        // 
        // dtpBaslangic
        // 
        dtpBaslangic.Font = new Font("Segoe UI", 10F);
        dtpBaslangic.Format = DateTimePickerFormat.Short;
        dtpBaslangic.Location = new Point(20, 197);
        dtpBaslangic.Name = "dtpBaslangic";
        dtpBaslangic.Size = new Size(454, 30);
        dtpBaslangic.TabIndex = 4;
        // 
        // lblBitis
        // 
        lblBitis.Font = new Font("Segoe UI", 9F);
        lblBitis.ForeColor = Color.FromArgb(52, 64, 84);
        lblBitis.Location = new Point(522, 171);
        lblBitis.Name = "lblBitis";
        lblBitis.Size = new Size(450, 23);
        lblBitis.TabIndex = 5;
        lblBitis.Text = "Bitiş Tarihi";
        // 
        // dtpBitis
        // 
        dtpBitis.Font = new Font("Segoe UI", 10F);
        dtpBitis.Format = DateTimePickerFormat.Short;
        dtpBitis.Location = new Point(522, 197);
        dtpBitis.Name = "dtpBitis";
        dtpBitis.Size = new Size(454, 30);
        dtpBitis.TabIndex = 5;
        // 
        // btnUygunSaatleriBul
        // 
        btnUygunSaatleriBul.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnUygunSaatleriBul.BackColor = Color.FromArgb(37, 99, 235);
        btnUygunSaatleriBul.FlatAppearance.BorderSize = 0;
        btnUygunSaatleriBul.FlatStyle = FlatStyle.Flat;
        btnUygunSaatleriBul.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnUygunSaatleriBul.ForeColor = Color.White;
        btnUygunSaatleriBul.Location = new Point(768, 385);
        btnUygunSaatleriBul.Name = "btnUygunSaatleriBul";
        btnUygunSaatleriBul.Size = new Size(254, 44);
        btnUygunSaatleriBul.TabIndex = 2;
        btnUygunSaatleriBul.Text = "Uygun Randevuları Bul";
        btnUygunSaatleriBul.UseVisualStyleBackColor = false;
        // 
        // grpSonuclar
        // 
        grpSonuclar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpSonuclar.BackColor = Color.White;
        grpSonuclar.Controls.Add(lblSonucBilgisi);
        grpSonuclar.Controls.Add(dgvUygunSaatler);
        grpSonuclar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        grpSonuclar.ForeColor = Color.FromArgb(25, 70, 111);
        grpSonuclar.Location = new Point(18, 444);
        grpSonuclar.Name = "grpSonuclar";
        grpSonuclar.Size = new Size(1004, 237);
        grpSonuclar.TabIndex = 3;
        grpSonuclar.TabStop = false;
        grpSonuclar.Text = "Müsait Randevular";
        // 
        // lblSonucBilgisi
        // 
        lblSonucBilgisi.Font = new Font("Segoe UI", 9F);
        lblSonucBilgisi.ForeColor = Color.DimGray;
        lblSonucBilgisi.Location = new Point(20, 27);
        lblSonucBilgisi.Name = "lblSonucBilgisi";
        lblSonucBilgisi.Size = new Size(950, 24);
        lblSonucBilgisi.TabIndex = 0;
        lblSonucBilgisi.Text = "Kriterleri seçip uygun randevuları arayınız.";
        // 
        // dgvUygunSaatler
        // 
        dgvUygunSaatler.AllowUserToAddRows = false;
        dgvUygunSaatler.AllowUserToDeleteRows = false;
        dgvUygunSaatler.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvUygunSaatler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUygunSaatler.BackgroundColor = Color.White;
        dgvUygunSaatler.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUygunSaatler.Location = new Point(20, 54);
        dgvUygunSaatler.MultiSelect = false;
        dgvUygunSaatler.Name = "dgvUygunSaatler";
        dgvUygunSaatler.ReadOnly = true;
        dgvUygunSaatler.RowHeadersVisible = false;
        dgvUygunSaatler.RowHeadersWidth = 51;
        dgvUygunSaatler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUygunSaatler.Size = new Size(956, 161);
        dgvUygunSaatler.TabIndex = 0;
        // 
        // btnRandevuOlustur
        // 
        btnRandevuOlustur.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnRandevuOlustur.BackColor = Color.FromArgb(21, 128, 61);
        btnRandevuOlustur.Enabled = false;
        btnRandevuOlustur.FlatAppearance.BorderSize = 0;
        btnRandevuOlustur.FlatStyle = FlatStyle.Flat;
        btnRandevuOlustur.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRandevuOlustur.ForeColor = Color.White;
        btnRandevuOlustur.Location = new Point(688, 698);
        btnRandevuOlustur.Name = "btnRandevuOlustur";
        btnRandevuOlustur.Size = new Size(174, 44);
        btnRandevuOlustur.TabIndex = 4;
        btnRandevuOlustur.Text = "Randevu Oluştur";
        btnRandevuOlustur.UseVisualStyleBackColor = false;
        // 
        // btnGeriDon
        // 
        btnGeriDon.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnGeriDon.BackColor = Color.FromArgb(95, 105, 120);
        btnGeriDon.DialogResult = DialogResult.Cancel;
        btnGeriDon.FlatAppearance.BorderSize = 0;
        btnGeriDon.FlatStyle = FlatStyle.Flat;
        btnGeriDon.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnGeriDon.ForeColor = Color.White;
        btnGeriDon.Location = new Point(878, 698);
        btnGeriDon.Name = "btnGeriDon";
        btnGeriDon.Size = new Size(144, 44);
        btnGeriDon.TabIndex = 5;
        btnGeriDon.Text = "Geri Dön";
        btnGeriDon.UseVisualStyleBackColor = false;
        // 
        // RandevuAramaFormu
        // 
        AutoScaleDimensions = new SizeF(9F, 23F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(242, 245, 250);
        CancelButton = btnGeriDon;
        ClientSize = new Size(1040, 764);
        Controls.Add(panelUstBaslik);
        Controls.Add(grpFiltre);
        Controls.Add(btnUygunSaatleriBul);
        Controls.Add(grpSonuclar);
        Controls.Add(btnRandevuOlustur);
        Controls.Add(btnGeriDon);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1056, 803);
        Name = "RandevuAramaFormu";
        StartPosition = FormStartPosition.CenterParent;
        Text = "x\"";
        panelUstBaslik.ResumeLayout(false);
        grpFiltre.ResumeLayout(false);
        grpSonuclar.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvUygunSaatler).EndInit();
        ResumeLayout(false);
    }
}
