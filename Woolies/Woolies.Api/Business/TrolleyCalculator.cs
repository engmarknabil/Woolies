using System.Collections.Generic;
using System.Linq;
using Woolies.Api.Models;

namespace Woolies.Api.Business
{
    public static class TrolleyCalculator
    {
        public static decimal CalculateTrolleyTotal(Trolley trolley)
        {
            var specialCombo = GetBestSpecialCombo(trolley);
            var fullPricedQuantities = GetFullPricedQuantities(trolley, specialCombo);
            return CalculatePrice(trolley.Products, fullPricedQuantities) + specialCombo.Total;
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

        private static TrolleySpecial GetBestSpecialCombo(Trolley trolley)
        {
            var bestCandidateSpecials = GetBestCandidateSpecialCombos(trolley);
            return GetBestSpecialCombo(trolley.Products, bestCandidateSpecials);
        }

        private static List<TrolleySpecial> GetBestCandidateSpecialCombos(Trolley trolley)
        {
            var eligibleIndividualSpecials = trolley.Specials.Where(special => IsEligible(trolley.Quantities, special)).ToList();

            var bestSpecialCombo = new TrolleySpecial();
            return GetBestCandidateSpecialCombos(trolley.Quantities, bestSpecialCombo, eligibleIndividualSpecials);
        }

        /// <summary>
        /// This method returns a list of only the best candidate special combos, by combining as many specials as possible.
        /// For example, if a trolley is eligible for special1 and special2 combined, only that special combo is returned,
        /// but special1 and special2 are not returned individually.
        /// </summary>
        private static List<TrolleySpecial> GetBestCandidateSpecialCombos(List<TrolleyQuantity> trolleyQuantities,
            TrolleySpecial eligibleSpecialCombo, List<TrolleySpecial> otherEligibleSpecialsToCombineWith)
        {
            var bestCandidateSpecialCombos = new List<TrolleySpecial>();

            for (int i = 0; i < otherEligibleSpecialsToCombineWith.Count; i++)
            {
                var newSpecialCombo = CombineSpecials(eligibleSpecialCombo, otherEligibleSpecialsToCombineWith[i]);
                if (IsEligible(trolleyQuantities, newSpecialCombo))
                {
                    bestCandidateSpecialCombos.AddRange(
                        GetBestCandidateSpecialCombos(trolleyQuantities, newSpecialCombo,
                            otherEligibleSpecialsToCombineWith.Skip(i).ToList()));
                }
            }

            if (!bestCandidateSpecialCombos.Any())
            {
                bestCandidateSpecialCombos.Add(eligibleSpecialCombo);
            }

            return bestCandidateSpecialCombos;
        }

        private static TrolleySpecial GetBestSpecialCombo(List<TrolleyProduct> products, List<TrolleySpecial> eligibleSpecials)
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

        public static decimal CalculatePrice(List<TrolleyProduct> products, List<TrolleyQuantity> quantities)
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
            return special.Quantities
                .All(specialQuantity =>
                    trolleyQuantities.Any(trolleyQuantity =>
                        trolleyQuantity.Name == specialQuantity.Name
                        && trolleyQuantity.Quantity >= specialQuantity.Quantity));
        }
    }
}