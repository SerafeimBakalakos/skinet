using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Specifications
{
    /// <summary>
    /// Used for eager loading of navigation properties when requesting one or more Products
    /// </summary>
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(string sort, int? brandId, int? typeId)
            : base(x => 
                (!brandId.HasValue || x.ProductBrandId == brandId) && 
                (!typeId.HasValue || x.ProductTypeId == typeId)  
            )
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
            AddOrderBy(x => x.Name); // If an orderby is specified, then this will be overwritten (it should be named: SetOrderBy)
            
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(x => x.Name);
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int productId) 
        : base(p => p.Id == productId)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}