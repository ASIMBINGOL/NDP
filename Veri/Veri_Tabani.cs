
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;

namespace NDPProje.Veri;

public class VeritabaniBaglami : DbContext
{
    // Konsol uygulamasi ve EF Core gecisleri icin.
    public VeritabaniBaglami()
    {
    }

    // Windows Forms uygulamasindan baglanti seceneklerini almak icin.
    public VeritabaniBaglami(
        DbContextOptions<VeritabaniBaglami> secenekler)
        : base(secenekler)
    {
    }


    public DbSet<Kurum> Kurumlar => Set<Kurum>();
    public DbSet<Bolum> Bolumler => Set<Bolum>();
    public DbSet<KurumCalismaSaati> KurumCalismaSaatleri
        => Set<KurumCalismaSaati>();

    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Hasta> Hastalar => Set<Hasta>();
    public DbSet<Calisan> Calisanlar => Set<Calisan>();
    public DbSet<Doktor> Doktorlar => Set<Doktor>();
    public DbSet<SaglikPersoneli> SaglikPersonelleri
        => Set<SaglikPersoneli>();

    public DbSet<SaglikHizmeti> SaglikHizmetleri
    => Set<SaglikHizmeti>();

    public DbSet<CalisanHizmeti> CalisanHizmetleri
    => Set<CalisanHizmeti>();

    public DbSet<UygunlukAraligi> UygunlukAraliklari//çalışma saatleri
    => Set<UygunlukAraligi>();

    public DbSet<Randevu> Randevular//randevu
    => Set<Randevu>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(
                "Data Source=NDP_Projesi.db");
        }
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // Kurumlar
        modelBuilder.Entity<Kurum>(tablo =>
        {
            tablo.ToTable("Kurumlar");

            tablo.HasKey(k => k.Id);

            tablo.Property(k => k.Ad)
                .IsRequired()
                .HasMaxLength(150);

            tablo.Property(k => k.KurumTuru)
                .IsRequired()
                .HasMaxLength(20);

            tablo.Property(k => k.Adres)
                .HasMaxLength(300);

            tablo.Property(k => k.Telefon)
                .HasMaxLength(30);

            tablo.HasOne(k => k.UstKurum)
                .WithMany(k => k.AltKurumlar)
                .HasForeignKey(k => k.UstKurumId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Saglik hizmetleri
        modelBuilder.Entity<SaglikHizmeti>(tablo =>
        {
            tablo.ToTable("SaglikHizmetleri", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_SaglikHizmetleri_Sure",
                    "\"SureDakika\" > 0");

                ayarlar.HasCheckConstraint(
                    "CK_SaglikHizmetleri_Ucret",
                    "\"UcretKurus\" >= 0");
            });

            tablo.HasKey(h => h.Id);

            tablo.Property(h => h.Ad)
                .IsRequired()
                .HasMaxLength(150);

            tablo.Property(h => h.Aciklama)
                .HasMaxLength(500);

            tablo.HasAlternateKey(h => new
            {
                h.Id,
                h.KurumId
            });

            tablo.HasOne(h => h.Kurum)
                .WithMany(k => k.SaglikHizmetleri)
                .HasForeignKey(h => h.KurumId)
                .OnDelete(DeleteBehavior.Restrict);

            tablo.HasOne(h => h.Bolum)
                .WithMany(b => b.SaglikHizmetleri)
                .HasForeignKey(h => new
                {
                    h.BolumId,
                    h.KurumId
                })
                .HasPrincipalKey(b => new
                {
                    b.Id,
                    b.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Calisan ve saglik hizmeti arasindaki iliski
        modelBuilder.Entity<CalisanHizmeti>(tablo =>
        {
            tablo.ToTable("CalisanHizmetleri");

            tablo.HasKey(x => new
            {
                x.CalisanId,
                x.SaglikHizmetiId
            });

            tablo.HasOne(x => x.Calisan)
                .WithMany(c => c.SunduguHizmetler)
                .HasForeignKey(x => new
                {
                    x.CalisanId,
                    x.KurumId
                })
                .HasPrincipalKey(c => new
                {
                    c.Id,
                    c.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);

            tablo.HasOne(x => x.SaglikHizmeti)
                .WithMany(h => h.CalisanAtamalari)
                .HasForeignKey(x => new
                {
                    x.SaglikHizmetiId,
                    x.KurumId
                })
                .HasPrincipalKey(h => new
                {
                    h.Id,
                    h.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Bolumler
        modelBuilder.Entity<Bolum>(tablo =>
        {
            tablo.ToTable("Bolumler");

            tablo.HasKey(b => b.Id);

            tablo.Property(b => b.Ad)
                .IsRequired()
                .HasMaxLength(100);

            tablo.Property(b => b.Aciklama)
                .HasMaxLength(500);

            tablo.HasIndex(b => new { b.KurumId, b.Ad })
                .IsUnique();

            tablo.HasAlternateKey(b => new { b.Id, b.KurumId });

            tablo.HasOne(b => b.Kurum)
                .WithMany(k => k.Bolumler)
                .HasForeignKey(b => b.KurumId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Kurum calisma saatleri
        modelBuilder.Entity<KurumCalismaSaati>(tablo =>
        {
            tablo.ToTable("KurumCalismaSaatleri", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_CalismaSaatleri_Gun",
                    "\"HaftaninGunu\" BETWEEN 0 AND 6");

                ayarlar.HasCheckConstraint(
                    "CK_CalismaSaatleri_Saat",
                    "\"KapaliMi\" = 1 OR " +
                    "(\"AcilisSaati\" IS NOT NULL AND " +
                    "\"KapanisSaati\" IS NOT NULL AND " +
                    "\"AcilisSaati\" < \"KapanisSaati\")");
            });

            tablo.HasKey(s => s.Id);

            tablo.Property(s => s.AcilisSaati)
                .HasMaxLength(5);

            tablo.Property(s => s.KapanisSaati)
                .HasMaxLength(5);

            tablo.HasIndex(s => new
            {
                s.KurumId,
                s.HaftaninGunu
            }).IsUnique();

            tablo.HasOne(s => s.Kurum)
                .WithMany(k => k.CalismaSaatleri)
                .HasForeignKey(s => s.KurumId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Kullanicilar
        modelBuilder.Entity<Kullanici>(tablo =>
        {
            tablo.ToTable("Kullanicilar", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_Kullanicilar_Rol",
                    "\"Rol\" IN ('Hasta', 'Doktor', " +
                    "'SaglikPersoneli', 'KurumYoneticisi')");
            });

            tablo.HasKey(k => k.Id);

            tablo.Property(k => k.AdSoyad)
                .IsRequired()
                .HasMaxLength(150);

            tablo.Property(k => k.Eposta)
                .UseCollation("NOCASE")
                .IsRequired()
                .HasMaxLength(254);

            tablo.HasIndex(k => k.Eposta)
                .IsUnique();

            tablo.Property(k => k.SifreOzeti)
                .IsRequired();

            tablo.Property(k => k.Rol)
                .IsRequired()
                .HasMaxLength(30);
        });

        // Hastalar
        modelBuilder.Entity<Hasta>(tablo =>
        {
            tablo.ToTable("Hastalar");

            tablo.HasKey(h => h.Id);

            tablo.HasIndex(h => h.KullaniciId)
                .IsUnique();

            tablo.Property(h => h.Telefon)
                .HasMaxLength(30);

            tablo.HasOne(h => h.Kullanici)
                .WithOne(k => k.HastaProfili)
                .HasForeignKey<Hasta>(h => h.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Calisanlar ve kalitim
        modelBuilder.Entity<Calisan>(tablo =>
        {
            tablo.HasAlternateKey(c => new { c.Id, c.KurumId });

            tablo.ToTable("Calisanlar", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_Calisanlar_Turu",
                    "\"CalisanTuru\" IN " +
                    "('Doktor', 'SaglikPersoneli')");
            });

            tablo.HasKey(c => c.Id);

            tablo.HasIndex(c => new
            {
                c.KullaniciId,
                c.KurumId
            }).IsUnique();

            tablo.HasOne(c => c.Kullanici)
                .WithMany(k => k.Calisanliklari)
                .HasForeignKey(c => c.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            tablo.HasOne(c => c.Kurum)
                .WithMany(k => k.Calisanlar)
                .HasForeignKey(c => c.KurumId)
                .OnDelete(DeleteBehavior.Restrict);

            tablo.HasOne(c => c.Bolum)
                .WithMany(b => b.Calisanlar)
                .HasForeignKey(c => new
                {
                    c.BolumId,
                    c.KurumId
                })
                .HasPrincipalKey(b => new
                {
                    b.Id,
                    b.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);

            // Doktor ve saglik personeli ayni tabloda tutulur.
            tablo.HasDiscriminator<string>("CalisanTuru")
                .HasValue<Doktor>("Doktor")
                .HasValue<SaglikPersoneli>("SaglikPersoneli");
        });

        // Doktora ozel bilgiler
        modelBuilder.Entity<Doktor>(tablo =>
        {
            tablo.Property(d => d.UzmanlikAlani)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Saglik personeline ozel bilgiler
        modelBuilder.Entity<SaglikPersoneli>(tablo =>
        {
            tablo.Property(p => p.Gorev)
                .IsRequired()
                .HasMaxLength(100);
        });


        // Uygunluk araliklari
        modelBuilder.Entity<UygunlukAraligi>(tablo =>
        {
            tablo.ToTable("UygunlukAraliklari", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_UygunlukAraliklari_Tarih",
                    "\"BitisTarihiSaati\" > " +
                    "\"BaslangicTarihiSaati\"");
            });

            tablo.HasKey(a => a.Id);

            tablo.HasAlternateKey(a => new
            {
                a.Id,
                a.CalisanId,
                a.KurumId
            });

            tablo.HasIndex(a => new
            {
                a.CalisanId,
                a.BaslangicTarihiSaati,
                a.BitisTarihiSaati
            });

            tablo.HasOne(a => a.Calisan)
                .WithMany()
                .HasForeignKey(a => new
                {
                    a.CalisanId,
                    a.KurumId
                })
                .HasPrincipalKey(c => new
                {
                    c.Id,
                    c.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Randevular
        modelBuilder.Entity<Randevu>(tablo =>
        {
            tablo.ToTable("Randevular", ayarlar =>
            {
                ayarlar.HasCheckConstraint(
                    "CK_Randevular_Tarih",
                    "\"BitisTarihiSaati\" > " +
                    "\"BaslangicTarihiSaati\"");

                ayarlar.HasCheckConstraint(
                    "CK_Randevular_Sure",
                    "\"SureDakikaKayit\" > 0");

                ayarlar.HasCheckConstraint(
                    "CK_Randevular_Ucret",
                    "\"UcretKurusKayit\" >= 0");

                ayarlar.HasCheckConstraint(
                    "CK_Randevular_Durum",
                    "\"Durum\" IN " +
                    "('Beklemede', 'Onaylandi', " +
                    "'IptalEdildi', 'Tamamlandi')");
            });

            tablo.HasKey(r => r.Id);

            // Hastanin randevulari
            tablo.HasOne(r => r.Hasta)
                .WithMany()
                .HasForeignKey(r => r.HastaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Randevu calisani
            tablo.HasOne(r => r.Calisan)
                .WithMany()
                .HasForeignKey(r => new
                {
                    r.CalisanId,
                    r.KurumId
                })
                .HasPrincipalKey(c => new
                {
                    c.Id,
                    c.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);

            // Randevu hizmeti
            tablo.HasOne(r => r.SaglikHizmeti)
                .WithMany()
                .HasForeignKey(r => new
                {
                    r.SaglikHizmetiId,
                    r.KurumId
                })
                .HasPrincipalKey(h => new
                {
                    h.Id,
                    h.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);

            // Calisanin bu hizmeti sunabildigini dogrula
            tablo.HasOne<CalisanHizmeti>()
                .WithMany()
                .HasForeignKey(r => new
                {
                    r.CalisanId,
                    r.SaglikHizmetiId
                })
                .OnDelete(DeleteBehavior.Restrict);

            // Randevunun uygunluk araligini bagla
            tablo.HasOne(r => r.UygunlukAraligi)
                .WithMany(a => a.Randevular)
                .HasForeignKey(r => new
                {
                    r.UygunlukAraligiId,
                    r.CalisanId,
                    r.KurumId
                })
                .HasPrincipalKey(a => new
                {
                    a.Id,
                    a.CalisanId,
                    a.KurumId
                })
                .OnDelete(DeleteBehavior.Restrict);

            tablo.HasIndex(r => new
            {
                r.CalisanId,
                r.BaslangicTarihiSaati,
                r.BitisTarihiSaati
            });

            tablo.HasIndex(r => new
            {
                r.HastaId,
                r.BaslangicTarihiSaati
            });
        });

    }
}
