using Delez.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Delez.Api.Data;

public class DelezDbContext(DbContextOptions<DelezDbContext> options) : DbContext(options)
{
    public DbSet<Uporabnik> Uporabniki => Set<Uporabnik>();
    public DbSet<Skupina> Skupine => Set<Skupina>();
    public DbSet<Clanstvo> Clanstva => Set<Clanstvo>();
    public DbSet<Strosek> Stroski => Set<Strosek>();
    public DbSet<DelezStroska> DeleziStroskov => Set<DelezStroska>();
    public DbSet<Nakazilo> Nakazila => Set<Nakazilo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Uporabnik>(e =>
        {
            e.ToTable("uporabnik");
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Ime).HasColumnName("ime").HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
            e.Property(u => u.GesloHash).HasColumnName("geslo_hash").HasMaxLength(100).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Skupina>(e =>
        {
            e.ToTable("skupina");
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.Ime).HasColumnName("ime").HasMaxLength(100).IsRequired();
            e.Property(s => s.Opis).HasColumnName("opis").HasColumnType("text");
        });

        modelBuilder.Entity<Clanstvo>(e =>
        {
            e.ToTable("clanstvo", t => t.HasCheckConstraint(
                "ck_clanstvo_vloga",
                "vloga IN ('skrbnik', 'clan', 'omejen')"));
            e.Property(c => c.Id).HasColumnName("id");
            e.Property(c => c.Vloga).HasColumnName("vloga").HasMaxLength(20).IsRequired();
            e.Property(c => c.UporabnikId).HasColumnName("tk_uporabnik");
            e.Property(c => c.SkupinaId).HasColumnName("tk_skupina");

            e.HasIndex(c => new { c.UporabnikId, c.SkupinaId }).IsUnique();

            e.HasOne(c => c.Uporabnik)
                .WithMany(u => u.Clanstva)
                .HasForeignKey(c => c.UporabnikId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.Skupina)
                .WithMany(s => s.Clanstva)
                .HasForeignKey(c => c.SkupinaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Strosek>(e =>
        {
            e.ToTable("strosek");
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.Opis).HasColumnName("opis").HasMaxLength(200).IsRequired();
            e.Property(s => s.Znesek).HasColumnName("znesek").HasColumnType("numeric(10,2)");
            e.Property(s => s.Datum).HasColumnName("datum").HasColumnType("date");
            e.Property(s => s.SkupinaId).HasColumnName("tk_skupina");
            e.Property(s => s.PlacnikId).HasColumnName("tk_clanstvo_placnik");

            e.HasOne(s => s.Skupina)
                .WithMany(sk => sk.Stroski)
                .HasForeignKey(s => s.SkupinaId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.Placnik)
                .WithMany()
                .HasForeignKey(s => s.PlacnikId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DelezStroska>(e =>
        {
            e.ToTable("delez_stroska");
            e.Property(d => d.Id).HasColumnName("id");
            e.Property(d => d.Znesek).HasColumnName("znesek").HasColumnType("numeric(10,2)");
            e.Property(d => d.StrosekId).HasColumnName("tk_strosek");
            e.Property(d => d.ClanstvoId).HasColumnName("tk_clanstvo");

            e.HasOne(d => d.Strosek)
                .WithMany(s => s.Delezi)
                .HasForeignKey(d => d.StrosekId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.Clanstvo)
                .WithMany()
                .HasForeignKey(d => d.ClanstvoId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Nakazilo>(e =>
        {
            e.ToTable("nakazilo");
            e.Property(n => n.Id).HasColumnName("id");
            e.Property(n => n.Znesek).HasColumnName("znesek").HasColumnType("numeric(10,2)");
            e.Property(n => n.Datum).HasColumnName("datum").HasColumnType("date");
            e.Property(n => n.SkupinaId).HasColumnName("tk_skupina");
            e.Property(n => n.PlacnikId).HasColumnName("tk_clanstvo_placnik");
            e.Property(n => n.PrejemnikId).HasColumnName("tk_clanstvo_prejemnik");

            e.HasOne(n => n.Skupina)
                .WithMany(s => s.Nakazila)
                .HasForeignKey(n => n.SkupinaId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(n => n.Placnik)
                .WithMany()
                .HasForeignKey(n => n.PlacnikId)
                .OnDelete(DeleteBehavior.NoAction);

            e.HasOne(n => n.Prejemnik)
                .WithMany()
                .HasForeignKey(n => n.PrejemnikId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
