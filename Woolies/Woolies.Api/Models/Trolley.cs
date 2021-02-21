using System.Collections.Generic;

namespace Woolies.Api.Models
{
    public class Trolley
    {
        public List<TrolleyProduct> Products { get; set; } = new List<TrolleyProduct>();
        public List<TrolleySpecial> Specials { get; set; } = new List<TrolleySpecial>();
        public List<TrolleyQuantity> Quantities { get; set; } = new List<TrolleyQuantity>();
    }
}