using Kommo_Client.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kommo_Client
{
    public class MarketplaceDb : DbContext
    {
        public MarketplaceDb(DbContextOptions<MarketplaceDb> options)
            : base(options) 
            => Database.Migrate();

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasIndex(x => x.LeadId)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;Database={nameof(MarketplaceDb)};User Id=postgres;Password=Sardor0618!;");

            //optionsBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={nameof(MarketplaceDb)};");
        }
    }
}