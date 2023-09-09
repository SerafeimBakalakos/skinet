using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }

        [Required] // This is very useful. If the request says "basketItems" instead of "items", then an empty list will be stored and a 200 response will be sent. But if I use [Required], the absence of "items" will cause the request to fail
        public List<BasketItemDto> Items { get; set; } /*= new List<BasketItemDto>(); */ // Do not initialize this, as it will bypass the [Required] validation and allow empty Items again.
    }
}