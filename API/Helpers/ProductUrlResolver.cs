using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        
        public ProductUrlResolver(Microsoft.Extensions.Configuration.IConfiguration config) //Automapper has a same named IConfiguration
        {
            _config = config;
        }

        public string Resolve(Product source, ProductToReturnDto destination, 
            string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl)) // Kinda overcautious, since the DB was configured to not allow empty strings
            {
                return _config["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}