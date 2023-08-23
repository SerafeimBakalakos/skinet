using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductBrand) // Eager loading for navigational property
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(p => p.Id == id); // FindAsync does not work with queries. SingleOrDefault will throw error if there are duplicates
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            // Async programming:
            // The thread creates a Task to handle the DB communication.
            // While the Task executes in the background (probably by some other thread),
            // this thread is free to handle another HTTP request.
            // When the Task finished, it notifies this thread to collect the data.
            return await _context.Products
                .Include(p => p.ProductBrand) // Eager loading for navigational property
                .Include(p => p.ProductType)
                .ToListAsync(); // At this point, the query is sent to SQL

        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _context.ProductBrands.ToListAsync();
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }
    }
}