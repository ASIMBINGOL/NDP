
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;

namespace NDPProje.Veri;

public class VeritabaniBaglami : DbContext
{
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
    }
}
