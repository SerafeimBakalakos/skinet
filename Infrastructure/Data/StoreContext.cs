using System.Reflection;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tell the ModelBuilder for this DbContext, that there are configurations in files from the Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // With SQLite we need to convert decimal to double for all entities that use decimal properties
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite") // too easy to make a typo here, which will not throw an error, but will prevent this code from working
            {
                // Access each entity classes (e.g. Product) in the DB
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    // Find decimal properties of this entity class
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));

                    foreach (var property in properties)
                    {
                        // Tell the ModelBuilder to convert this decimal property to double
                        modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();
                    }
                }
            }
        }
    }
}