using CoffeeShop.Models;
using CoffeeShop.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly  IOrderService _orderService;
        private readonly IItemService _itemService;

        public OrderController(IOrderService orderService, IItemService itemService)
        {
            _orderService = orderService;
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderService.GetAllAsync());
        }

        [HttpGet("{id:length(24)}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get(string id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> Create(List<string> ItemIds)
        {
            int no = 1;
            var pastOrders = await _orderService.GetAllAsync();
            if(pastOrders.Count > 0)
            {
                Order lastOrder = pastOrders.Last();
                string lastOrderNo = lastOrder.OrderNo;
                no = Convert.ToInt32(lastOrderNo.Replace("O#", "")) + 1;
            }                                    

            Order order = new Order();
            order.OrderNo = $"O#{no.ToString().PadLeft(4,'0')}";
            order.ItemIds = ItemIds;

            var retVal = await CalculateOrderTotal(ItemIds);
            order.TotalPrice = retVal.TotalPrice;
            order.TotalTax = retVal.TotalTax;
            order.TotalAmount = retVal.TotalAmount;
            order.IsProcessed = true;

            await _orderService.CreateAsync(order);

            Random _random = new Random();
            await Task.Delay(_random.Next(2000, 5000)); //sleep for random seconds.

            return Ok($"Your order is ready with Order No. \"{order.OrderNo}\". Hope you enjoy your food!");
        }

        [HttpPut("{id:length(24)}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Update(string id, Order orderData)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            await _orderService.UpdateAsync(id, orderData);
            return NoContent();
        }

        [HttpPost]
        [Route("getordertotal")]
        public async Task<IActionResult> GetOrderTotal(List<string> ItemIds)
        {
            decimal TotalPrice = 0;
            decimal TotalTax = 0;
            decimal TotalAmount = 0;

            var retVal = await CalculateOrderTotal(ItemIds);
            TotalPrice = retVal.TotalPrice;
            TotalTax = retVal.TotalTax;
            TotalAmount = retVal.TotalAmount;

            return Ok(new { TotalPrice = TotalPrice, TotalTax = TotalTax, TotalAmount = TotalAmount });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<OrderValues> CalculateOrderTotal(List<string> ItemIds)
        {
            decimal LineItemPrice = 0;
            decimal LineItemTax = 0;
            decimal TotalPrice = 0;
            decimal TotalTax = 0;
            decimal TotalAmount = 0;

            foreach (var itemId in ItemIds)
            {
                var item = await _itemService.GetByIdAsync(itemId);
                if (item != null)
                {
                    bool IsDiscountToBeApplied = false;
                    if (item.IsDiscountedWithOtherItem)
                    {
                        var otherItemID = item.OtherItemId;

                        foreach (var findOtherItemId in ItemIds)
                        {
                            if (otherItemID == findOtherItemId)
                            {
                                IsDiscountToBeApplied = true;
                                break;
                            }
                        }
                    }

                    if (IsDiscountToBeApplied)
                    {
                        LineItemPrice = item.Price - ((item.DiscountPercent / (decimal)100.0) * item.Price);
                        LineItemTax = ((item.TaxPercent / (decimal)100.0) * LineItemPrice);
                    }
                    else
                    {
                        LineItemPrice = item.Price;
                        LineItemTax = ((item.TaxPercent / (decimal)100.0) * LineItemPrice);
                    }

                    TotalPrice += LineItemPrice;
                    TotalTax += LineItemTax;
                }
            }
            TotalAmount = TotalPrice + TotalTax;

            OrderValues values = new OrderValues();
            values.TotalPrice = TotalPrice;
            values.TotalTax = TotalTax;
            values.TotalAmount = TotalAmount;

            return values;
        }

        public struct OrderValues
        {
            public decimal TotalPrice;
            public decimal TotalTax;
            public decimal TotalAmount;
        }
    }
}
