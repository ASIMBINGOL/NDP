
using Microsoft.EntityFrameworkCore;
using NDPProje.Modeller;

namespace NDPProje.Veri;

public class VeritabaniBaglami : DbContext
{
    public DbSet<Kurum> Kurumlar => Set<Kurum>();

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
    }
}
