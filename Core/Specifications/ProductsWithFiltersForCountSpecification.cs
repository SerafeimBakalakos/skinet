using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductSpecParams productParams)
            : base(x =>
                (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) && 
                (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) && 
                (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId)  
            )
        {
            //TODO: The expression passed to the base contructor is also used in ProductsWithTypesAndBrandsSpecification.
            //      It also reduces readability, since it is passed to the fucking ctor and needs 4 lines.
            //      I should create and return the expression in ProductSpecParams or inside some specification class.
        }
    }
}