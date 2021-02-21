using System.Collections.Generic;

namespace Woolies.Api.Models
{
    public class TrolleySpecial
    {
        public List<TrolleyQuantity> Quantities { get; set; } = new List<TrolleyQuantity>();
        public decimal Total { get; set; }

    }
}