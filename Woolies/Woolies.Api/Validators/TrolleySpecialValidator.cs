using System.Linq;
using FluentValidation;
using Woolies.Api.Business;
using Woolies.Api.Models;

namespace Woolies.Api.Validators
{
    public class TrolleySpecialValidator : AbstractValidator<TrolleySpecial>
    {
        public TrolleySpecialValidator()
        {
            
        }

        public TrolleySpecialValidator(Trolley trolley)
        {
            RuleFor(special => special.Quantities)
                .Must(quantities => quantities.Select(quantity => quantity.Name).IsUnique()).WithMessage("Names must be unique.")
                .ForEach(collection => collection.SetValidator(_ => new TrolleyQuantityValidator(trolley)));

            RuleFor(special => special.Total)
                .GreaterThan(0)
                .LessThan(special => TrolleyCalculator.CalculatePrice(trolley.Products, special.Quantities)).WithMessage("Special must be cheaper than Products full price");
        }
    }
}