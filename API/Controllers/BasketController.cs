using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepo, IMapper mapper)
        {
            _mapper = mapper;
            _basketRepo = basketRepo;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepo.GetBasketAsync(id);
            // If this basket does not exist return a new one with the provided id
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {   
            //TODO: If in the POST body the list is named "productItems" instead of "items", a basket with 0 items will be stored and returned.

            // Our DTOs are only for validation, thus we only need them for inputs. 
            // Personally I hate receiving EntityDto and sending Entity, but ok.
            var customerBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket); // Automapper will automatically map the composed BasketItemDto to BasketItem.

            var updatedBasket = await _basketRepo.UpdateBasketAsync(customerBasket);
            return Ok(updatedBasket); //QUESTION: This could also be null. Why are we not handling this case?
        }

        [HttpDelete]
        public async Task DeleteBasketAsync(string id)
        {
            await _basketRepo.DeleteBasketAsync(id);
        }
    }
}