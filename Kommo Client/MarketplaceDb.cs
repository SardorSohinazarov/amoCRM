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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={nameof(MarketplaceDb)};");
        }
    }
}