using System.Collections.Generic;

namespace Woolies.Api.Models
{
    public class Trolley
    {
        public List<TrolleyProduct> Products { get; set; } = new List<TrolleyProduct>();
        public List<TrolleySpecial> Specials { get; set; } = new List<TrolleySpecial>();
        public List<TrolleyQuantity> Quantities { get; set; } = new List<TrolleyQuantity>();
    }

    public class TrolleyProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class TrolleySpecial
    {
        public List<TrolleyQuantity> Quantities { get; set; }
        public decimal Total { get; set; }

    }

    public class TrolleyQuantity
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }
}