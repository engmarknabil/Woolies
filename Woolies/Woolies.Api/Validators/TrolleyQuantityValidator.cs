using System.Linq;
using FluentValidation;
using Woolies.Api.Models;

namespace Woolies.Api.Validators
{
    public class TrolleyQuantityValidator : AbstractValidator<TrolleyQuantity>
    {
        public TrolleyQuantityValidator()
        {
            
        }

        public TrolleyQuantityValidator(Trolley trolley)
        {
            RuleFor(quantity => quantity.Name)
                .Must(name => trolley.Products.Any(product => product.Name == name)).WithMessage("'Name' must be one of the Product names.");

            RuleFor(quantity => quantity.Quantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}