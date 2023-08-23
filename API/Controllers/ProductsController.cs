using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        // Lifetime details of this dependency injection: 
        // When request is made at this route, the framework instantiates a new ApiController. 
        // To do so, a new repo and DBContext will be instantiated as a service.
        // When the request is finished, both will be disposed of.
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet] //Returned type for HTTP requests should be ActionResult<> 
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _repo.GetProductsAsync();
            return Ok(products); // no need for 2 separate lines
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _repo.GetProductByIdAsync(id);
        }
    }
}