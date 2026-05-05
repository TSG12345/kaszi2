using Kaszinó_projekt.Modells;
using Microsoft.EntityFrameworkCore;

namespace Kaszinó_projekt
{
    public class AppContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server = localhost; database=Kaszino; uid=root; pwd=;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adatokmodell>().HasKey(a => a.Id);
            modelBuilder.Entity<Kaszinobevitel>().HasKey(k => k.TranzakcióId);
            modelBuilder.Entity<JátékokAdatai>().HasKey(b => b.JátékId);

            // Add meg a tábla nevet és a külső kulcs kapcsolatot
            modelBuilder.Entity<JátékokAdatai>().ToTable("JátékokAdatai");
            modelBuilder.Entity<JátékokAdatai>()
                .HasOne(j => j.Felhasznalo)
                .WithMany()
                .HasForeignKey(j => j.FelhasznaloId);
        }

        public DbSet<Adatokmodell> Adatokmodell { get; set; }
        public DbSet<Kaszinobevitel> Kaszinobevitel { get; set; }
        public DbSet<JátékokAdatai> JátékokAdatai { get; set; }
    }
}