using Microsoft.EntityFrameworkCore;
using CoreBank.API.Models;

namespace CoreBank.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.GonderenHesap)
                .WithMany(a => a.GonderilenIslemler)
                .HasForeignKey(t => t.GonderenHesapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AliciHesap)
                .WithMany(a => a.AlinanIslemler)
                .HasForeignKey(t => t.AliciHesapId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
