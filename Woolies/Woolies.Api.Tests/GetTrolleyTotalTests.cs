using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Woolies.Api.Controllers;
using Woolies.Api.Models;
using Xunit;

namespace Woolies.Api.Tests
{
    public class GetTrolleyTotalTests
    {

        private const string Product1 = "Product1";
        private const string Product2 = "Product2";

        private static readonly List<TrolleyProduct> Products = new List<TrolleyProduct>
        {
            new TrolleyProduct
            {
                Name = Product1,
                Price = 10
            },
            new TrolleyProduct
            {
                Name = Product2,
                Price = 20
            }
        };

        [Fact]
        public void WhenAnyQuantityIsLessThanSpecial_ShouldNotApplySpecial()
        {
            // Arrange
            var controller = new ExercisesController(new Mock<IResourceClient>(MockBehavior.Strict).Object);

            // Act
            var trolleyTotal = controller.GetTrolleyTotal(new Trolley
            {
                Products = Products,
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
                            },
                            new TrolleyQuantity
                            {
                                Name = Product2,
                                Quantity = 2
                            }
                        },
                        Total = 5
                    }
                },
                Quantities = new List<TrolleyQuantity>
                {
                    new TrolleyQuantity
                    {
                        Name = Product1,
                        Quantity = 1
                    },
                    new TrolleyQuantity
                    {
                        Name = Product2,
                        Quantity = 1
                    }
                }
            });

            // Assert
            trolleyTotal.Should().Be(30);
        }

        [Fact]
        public void WhenQuantityIsEqualOrMoreThanSpecial_ShouldApplySpecial()
        {
            // Arrange
            var controller = new ExercisesController(new Mock<IResourceClient>(MockBehavior.Strict).Object);

            // Act
            var trolleyTotal = controller.GetTrolleyTotal(new Trolley
            {
                Products = Products,
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
                            },
                            new TrolleyQuantity
                            {
                                Name = Product2,
                                Quantity = 2
                            }
                        },
                        Total = 5
                    }
                },
                Quantities = new List<TrolleyQuantity>
                {
                    new TrolleyQuantity
                    {
                        Name = Product1,
                        Quantity = 2
                    },
                    new TrolleyQuantity
                    {
                        Name = Product2,
                        Quantity = 2
                    }
                }
            });

            // Assert
            trolleyTotal.Should().Be(15);
        }

        [Fact]
        public void WhenTrolleyIsEligibleForSameSpecialMultipleTimes_ShouldApplySpecialMultipleTimes()
        {
            // Arrange
            var controller = new ExercisesController(new Mock<IResourceClient>(MockBehavior.Strict).Object);

            // Act
            var trolleyTotal = controller.GetTrolleyTotal(new Trolley
            {
                Products = Products,
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
                            },
                            new TrolleyQuantity
                            {
                                Name = Product2,
                                Quantity = 2
                            }
                        },
                        Total = 5
                    }
                },
                Quantities = new List<TrolleyQuantity>
                {
                    new TrolleyQuantity
                    {
                        Name = Product1,
                        Quantity = 2
                    },
                    new TrolleyQuantity
                    {
                        Name = Product2,
                        Quantity = 4
                    }
                }
            });

            // Assert
            trolleyTotal.Should().Be(10);
        }

        [Fact]
        public void WhenTrolleyIsEligibleForMultipleSpecials_ShouldApplyBestSpecialCombination()
        {
            // Arrange
            var controller = new ExercisesController(new Mock<IResourceClient>(MockBehavior.Strict).Object);

            // Act
            var trolleyTotal = controller.GetTrolleyTotal(new Trolley
            {
                Products = Products,
                Specials = new List<TrolleySpecial>
                {
                    new TrolleySpecial
                    {
                        Quantities = new List<TrolleyQuantity>
                        {
                            new TrolleyQuantity
                            {
                                Name = Product1,
                                Quantity = 3
                            }
                        },
                        Total = 5
                    },
                    new TrolleySpecial
                    {
                        Quantities = new List<TrolleyQuantity>
                        {
                            new TrolleyQuantity
                            {
                                Name = Product1,
                                Quantity = 6
                            }
                        },
                        Total = 9
                    },
                    new TrolleySpecial
                    {
                        Quantities = new List<TrolleyQuantity>
                        {
                            new TrolleyQuantity
                            {
                                Name = Product1,
                                Quantity = 8
                            }
                        },
                        Total = 9.5m
                    }
                },
                Quantities = new List<TrolleyQuantity>
                {
                    new TrolleyQuantity
                    {
                        Name = Product1,
                        Quantity = 10
                    }
                }
            });

            // Assert
            trolleyTotal.Should().Be(24);
        }
    }
}
