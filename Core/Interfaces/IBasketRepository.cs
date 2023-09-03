using Core.Entities;

namespace Core.Interfaces
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        
        /// <summary>
        /// Update or create a CustomerBasket
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        
        Task<bool> DeleteBasketAsync(string basketId);
    }
}