using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        // Lifetime details of this dependency injection: 
        // When request is made at this route, the framework instantiates a new ApiController. 
        // To do so, a new repo and DBContext will be instantiated as a service.
        // When the request is finished, both will be disposed of.
        public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper)
        {
            _productsRepo = productsRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet] //Returned type for HTTP requests should be ActionResult<> 
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        { 
            // About parameters.
            // In GET requests, parameters are automatically bound to the query string, namely the portion of the URL after "?", by the ApiController.
            // Here, there is an object of ProductSpecParams that can be automatically bound, but confuses the ApiController.
            // The [FromQuery] attribute tells the ApiController to bind the parameters of ProductSpecParams to the query string.
            // GET requests do not have a body. In other requests, the data is usually passed from the body of the request.
            // In case of confusion, we would use [FromBody] or [FromForm] attributes.
            // Passing data [FromQuery] would also be possible, but unusual.

            // Eager loading of navigation properties with repo and specification patterns
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var products = await _productsRepo.ListAsync(spec);

            var countSpec = new ProductsWithFiltersForCountSpecification(productParams);
            int totalItems = await  _productsRepo.CountAsync(countSpec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
            // Mapping is done in the API controller for now. It would be more efficient to do it in the specification.
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] //Not necessary. Swagger new about this. All these attributes to facilitate Swagger would make the code cumbersome. Better let Swagger to pick up the most important ones.
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)] //Informs Swagger that a 404 response may be returned and its format.
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}