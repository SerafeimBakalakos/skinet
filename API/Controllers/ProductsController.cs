using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _context;

        // Lifetime details of this dependency injection: 
        // When request is made at this route, the framework instantiates a new ApiController. 
        // To do so, a new DBContext will be instantiated as a service.
        // When the request is finished, both will be disposed of.
        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            // Async programming:
            // The thread creates a Task to handle the DB communication.
            // While the Task executes in the background (probably by some other thread),
            // this thread is free to handle another HTTP request.
            // When the Task finished, it notifies this thread to collect the data.
            var products = await _context.Products.ToListAsync();

            return products; // no need for 2 separate lines
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }
    }
}