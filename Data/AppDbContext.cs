using Microsoft.EntityFrameworkCore;
using StockPro.Data.Entities;

namespace StockPro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<StockMovement> StockMovements { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

                entity.HasMany(u => u.StockMovements)
                .WithOne(sm => sm.User)
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");
                entity.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id)
                .HasDefaultValueSql("NEWID()");

                entity.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

                entity.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.StockMovements)
                    .WithOne(sm => sm.Product)
                    .HasForeignKey(sm => sm.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Alerts)
                    .WithOne(a => a.Product)
                    .HasForeignKey(a => a.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(sm => sm.Id)
                .HasDefaultValueSql("NEWID()");

                entity.HasOne(sm => sm.Product)
               .WithMany(p => p.StockMovements)
               .HasForeignKey(sm => sm.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sm => sm.User)
                    .WithMany(u => u.StockMovements)
                    .HasForeignKey(sm => sm.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(a => a.Id)
                .HasDefaultValueSql("NEWID()");

                entity.HasOne(a => a.Product)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(a => a.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

        }
    }
}
