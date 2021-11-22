using CoffeeShop.Configuration;
using CoffeeShop.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _order;
        private readonly DbConfiguration _settings;

        public OrderService(IOptions<DbConfiguration> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _order = database.GetCollection<Order>(_settings.OrderCollectionName);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _order.Find(c => true).ToListAsync();
        }
        public async Task<Order> GetByIdAsync(string id)
        {
            return await _order.Find<Order>(c => c.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Order> CreateAsync(Order book)
        {
            await _order.InsertOneAsync(book);
            return book;
        }
        public async Task UpdateAsync(string id, Order book)
        {
            await _order.ReplaceOneAsync(c => c.Id == id, book);
        }
    }
}
