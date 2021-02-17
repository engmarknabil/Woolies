using System.Collections.Generic;

namespace Woolies.Api.Models
{
    public class ShopperHistory
    {
        public decimal CustomerId { get; set; }
        public List<Product> Products { get; set; }
    }
}