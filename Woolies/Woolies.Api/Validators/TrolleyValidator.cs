using System.Linq;
using FluentValidation;
using Woolies.Api.Models;

namespace Woolies.Api.Validators
{
    public class TrolleyValidator : AbstractValidator<Trolley>
    {
        public TrolleyValidator()
        {
            RuleFor(trolley => trolley.Products)
                .Must(products => products.Select(quantity => quantity.Name).IsUnique()).WithMessage("Names must be unique.")
                .ForEach(collection => collection.SetValidator(_ => new TrolleyProductValidator()));

            RuleFor(trolley => trolley.Quantities)
                .Must(quantities => quantities.Select(quantity => quantity.Name).IsUnique()).WithMessage("Names must be unique.");

            RuleForEach(trolley => trolley.Quantities)
                .SetValidator(trolley => new TrolleyQuantityValidator(trolley));

            RuleForEach(trolley => trolley.Specials)
                .SetValidator(trolley => new TrolleySpecialValidator(trolley));
        }


    }
}