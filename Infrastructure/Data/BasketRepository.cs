using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        // Redis DB is NoSql, just {key, value} pairs.
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            // Each basket is stored as a json string in Redis db
            var data = await _database.StringGetAsync(basketId);
            
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var created = await _database.StringSetAsync(
                basket.Id, 
                JsonSerializer.Serialize(basket), 
                TimeSpan.FromDays(30)); // How long the basket will persist in DB. Business decision. It should be read from some config file.

            if (!created) return null;
            return await GetBasketAsync(basket.Id);
        }
    }
}