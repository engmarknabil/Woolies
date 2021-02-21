using System.Collections.Generic;
using FluentAssertions;
using Woolies.Api.Models;
using Woolies.Api.Validators;
using Xunit;

namespace Woolies.Api.Tests
{
    public class TrolleyValidatorTests
    {
        private const string Product1 = "Product1";
        private readonly Trolley _trolley;
        private readonly TrolleyValidator _trolleyValidator;

        public TrolleyValidatorTests()
        {
            _trolleyValidator = new TrolleyValidator();

            _trolley = new Trolley
            {
                Products = new List<TrolleyProduct>
                {
                    new TrolleyProduct
                    {
                        Name = Product1,
                        Price = 10
                    }
                },
                Specials = new List<TrolleySpecial>
                {
                    new TrolleySpecial
                    {
                        Quantities = new List<TrolleyQuantity>
                        {
                            new TrolleyQuantity
                            {
                                Name = Product1,
                                Quantity = 1
                            }
                        },
                        Total = 9
                    }
                },
                Quantities = new List<TrolleyQuantity>
                {
                    new TrolleyQuantity
                    {
                        Name = Product1,
                        Quantity = 1
                    }
                }
            };
        }

        [Fact]
        public void WhenTrolleyIsValid_ShouldNotReturnValidationeErrors()
        {
            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenPriceIsNotGreaterThanZero_ShouldReturnValidationError(decimal price)
        {
            // Arrange
            _trolley.Products[0].Price = price;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenQuantityIsLessThanZero_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Quantities[0].Quantity = -1;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenQuantityIsZero_ShouldNotReturnValidationError()
        {
            // Arrange
            _trolley.Quantities[0].Quantity = 0;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenSpecialQuantityIsNotGreaterThanZero_ShouldReturnValidationError(decimal quantity)
        {
            // Arrange
            _trolley.Specials[0].Quantities[0].Quantity = quantity;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void WhenSpecialTotalIsNotGreaterThanZero_ShouldReturnValidationError(decimal quantity)
        {
            // Arrange
            _trolley.Specials[0].Total = quantity;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenProductNameIsDuplicated_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Products.Add(new TrolleyProduct
            {
                Name = Product1,
                Price = 3
            });

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenQuantityNameIsDuplicated_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Quantities.Add(new TrolleyQuantity
            {
                Name = Product1,
                Quantity = 3
            });

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenSpecialQuantityNameIsDuplicated_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Specials[0].Quantities.Add(new TrolleyQuantity
            {
                Name = Product1,
                Quantity = 3
            });

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenQuantityNameIsNotFoundInProducts_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Quantities[0].Name = "Product2";

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenSpecialQuantityNameIsNotFoundInProducts_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Specials[0].Quantities[0].Name = "Product2";

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenSpecialTotalIsEqualToOriginalPrice_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Specials[0].Total = 10;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void WhenSpecialTotalIsGreateThanOriginalPrice_ShouldReturnValidationError()
        {
            // Arrange
            _trolley.Specials[0].Total = 19;

            // Act
            var result = _trolleyValidator.Validate(_trolley);

            // Assert
            result.IsValid.Should().BeFalse();
        }
    }
}
