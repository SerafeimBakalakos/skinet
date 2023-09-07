using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Migrations.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        // Validations should also exist here in the Data Acces Layer. Validations should also exist in the Presentation Layer, i.e. the DTOs and Controlers. And there should be client-side validations
        //QUESTION: What about validations in the Entities? The teacher of the Udemy course says that the ones in DAL cover those in Entities, but I am not sold.
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Id).IsRequired(); // Unnecessary statement. The Id would be required by default.
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            // builder.Property(p => p.Description).IsRequired().HasMaxLength(180); // This may create issues with seeded data later.
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.PictureUrl).IsRequired();

            // EF does these next relationships by default
            builder.HasOne(b => b.ProductBrand).WithMany()
                .HasForeignKey(p => p.ProductBrandId);
            builder.HasOne(b => b.ProductType).WithMany()
                .HasForeignKey(p => p.ProductTypeId);
        }
    }
}