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
    public class ItemService : IItemService
    {
        private readonly IMongoCollection<Item> _item;
        private readonly DbConfiguration _settings;

        public ItemService(IOptions<DbConfiguration> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _item = database.GetCollection<Item>(_settings.ItemCollectionName);
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _item.Find(c => true).ToListAsync();
        }
        public async Task<Item> GetByIdAsync(string id)
        {
            return await _item.Find<Item>(c => c.Id == id).FirstOrDefaultAsync();
        }
        public async Task<Item> CreateAsync(Item book)
        {
            await _item.InsertOneAsync(book);
            return book;
        }
        public async Task UpdateAsync(string id, Item book)
        {
            await _item.ReplaceOneAsync(c => c.Id == id, book);
        }
        public async Task DeleteAsync(string id)
        {
            await _item.DeleteOneAsync(c => c.Id == id);
        }
    }
}
