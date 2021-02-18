using System.Collections.Generic;
using System.Linq;
using Woolies.Api.Models;

namespace Woolies.Api.Business
{
    public static class TrolleyCalculator
    {
        public static decimal CalculateTrolleyTotal(Trolley trolley)
        {
            var special = GetBestSpecial(trolley);
            if (special == null)
            {
                return CalculatePrice(trolley.Products, trolley.Quantities);
            }

            var fullPricedQuantities = GetFullPricedQuantities(trolley, special);
            return CalculatePrice(trolley.Products, fullPricedQuantities) + special.Total;
        }

        private static List<TrolleyQuantity> GetFullPricedQuantities(Trolley trolley, TrolleySpecial special)
        {
            return trolley.Quantities.Select(quantity => new TrolleyQuantity
            {
                Name = quantity.Name,
                Quantity = quantity.Quantity - GetSpecialQuantity(special, quantity.Name)
            }).ToList();
        }

        private static decimal GetSpecialQuantity(TrolleySpecial special, string productName)
        {
            return special.Quantities
                .FirstOrDefault(quantity => quantity.Name == productName)
                ?.Quantity ?? 0;
        }

        private static TrolleySpecial GetBestSpecial(Trolley trolley)
        {
            var eligibleSpecials = GetEligibleSpecials(trolley);
            return eligibleSpecials.Any() ? GetBestSpecial(trolley.Products, eligibleSpecials) : null;
        }

        private static List<TrolleySpecial> GetEligibleSpecials(Trolley trolley)
        {
            var eligibleSpecials = new List<TrolleySpecial>();
            
            var eligibleSingleSpecials = trolley.Specials.Where(special => IsEligible(trolley.Quantities, special)).ToList();
            var newEligibleSpecials = new List<TrolleySpecial>(eligibleSingleSpecials);
            while (newEligibleSpecials.Any())
            {
                eligibleSpecials.AddRange(newEligibleSpecials);
                newEligibleSpecials = newEligibleSpecials
                    .SelectMany(special => eligibleSingleSpecials, CombineSpecials)
                    .Where(special => IsEligible(trolley.Quantities, special))
                    .ToList();
            }

            return eligibleSpecials;
        }

        private static TrolleySpecial GetBestSpecial(List<TrolleyProduct> products, List<TrolleySpecial> eligibleSpecials)
        {
            return eligibleSpecials
                .OrderByDescending(special => CalculateSavings(products, special))
                .First();
        }

        private static decimal CalculateSavings(List<TrolleyProduct> products, TrolleySpecial special)
        {
            var fullPriceEquivalentOfSpecial = CalculatePrice(products, special.Quantities);
            return fullPriceEquivalentOfSpecial - special.Total;
        }

        private static decimal CalculatePrice(List<TrolleyProduct> products, List<TrolleyQuantity> quantities)
        {
            return quantities
                .Join(products, quantity => quantity.Name, product => product.Name,
                    (quantity, product) => quantity.Quantity * product.Price)
                .Sum(totalPrice => totalPrice);
        }

        private static TrolleySpecial CombineSpecials(TrolleySpecial special1, TrolleySpecial special2)
        {
            return new TrolleySpecial
            {
                Quantities = special1.Quantities.Concat(special2.Quantities)
                    .GroupBy(trolleyQuantity => trolleyQuantity.Name).Select(
                        grouping => new TrolleyQuantity
                        {
                            Name = grouping.Key,
                            Quantity = grouping.Sum(trolleyQuantity => trolleyQuantity.Quantity)
                        }).ToList(),
                Total = special1.Total + special2.Total
            };
        }

        private static bool IsEligible(List<TrolleyQuantity> trolleyQuantities, TrolleySpecial special)
        {
            return special.Quantities.All(specialQuantity => trolleyQuantities.Any(trolleyQuantity =>
                trolleyQuantity.Name == specialQuantity.Name && trolleyQuantity.Quantity >= specialQuantity.Quantity));
        }
    }
}