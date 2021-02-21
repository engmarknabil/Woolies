using FluentValidation;
using Woolies.Api.Models;

namespace Woolies.Api.Validators
{
    public class TrolleyProductValidator : AbstractValidator<TrolleyProduct>
    {
        public TrolleyProductValidator()
        {
            RuleFor(product => product.Price)
                .GreaterThan(0);
        }
    }
}