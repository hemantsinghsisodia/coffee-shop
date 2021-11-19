using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShop.Services
{
    public interface IItemService
    {
        Task<List<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(string id);
        Task<Item> CreateAsync(Item item);
        Task UpdateAsync(string id, Item item);
        Task DeleteAsync(string id);
    }
}
