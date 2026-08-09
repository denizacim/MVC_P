using Microsoft.EntityFrameworkCore;
using MVC_P.Models;

namespace MVC_P.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasMany(c => c.Events)
                  .WithOne(e => e.Club)
                  .HasForeignKey(e => e.ClubId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasIndex(r => new { r.EventId, r.UserId }).IsUnique();
            entity.HasOne(r => r.Event)
                  .WithMany(e => e.Registrations)
                  .HasForeignKey(r => r.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data: admin user, sample clubs and events
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, AdSoyad = "Site Admin", Email = "admin@example.com", SifreHash = "admin123", Rol = "Admin" }
        );

        modelBuilder.Entity<Club>().HasData(
            new Club { Id = 1, Ad = "Biliþim Kulübü", Aciklama = "Teknoloji ve yazýlým", KurulusTarihi = new DateTime(2020,1,1), AktifMi = true },
            new Club { Id = 2, Ad = "Spor Kulübü", Aciklama = "Spor etkinlikleri", KurulusTarihi = new DateTime(2019,5,10), AktifMi = true }
        );

        modelBuilder.Entity<Event>().HasData(
            new Event { Id = 1, ClubId = 1, Baslik = "C# Semineri", Aciklama = ".NET 8 yenilikleri", BaslangicTarihi = new DateTime(2025,1,10,10,0,0), BitisTarihi = new DateTime(2025,1,10,12,0,0), Kontenjan = 50, Konum = "A Blok Konferans", Durum = "Planlandi" },
            new Event { Id = 2, ClubId = 1, Baslik = "Hackathon", Aciklama = "24 saat kod", BaslangicTarihi = new DateTime(2025,2,5,9,0,0), BitisTarihi = new DateTime(2025,2,6,9,0,0), Kontenjan = 100, Konum = "Lab 1", Durum = "Planlandi" },
            new Event { Id = 3, ClubId = 2, Baslik = "Futbol Turnuvasi", Aciklama = "Bahar dönemi", BaslangicTarihi = new DateTime(2025,3,15,14,0,0), BitisTarihi = new DateTime(2025,3,15,18,0,0), Kontenjan = 32, Konum = "Saha 2", Durum = "Planlandi" }
        );
    }
}
