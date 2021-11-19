using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShop.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal TaxPercent { get; set; }
        public bool IsDiscountedWithOtherItem { get; set; }
        public decimal DiscountPercent { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string OtherItemId { get; set; }

    }
}
