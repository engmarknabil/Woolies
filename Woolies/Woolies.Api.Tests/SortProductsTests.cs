using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Woolies.Api.Controllers;
using Woolies.Api.Models;
using Xunit;

namespace Woolies.Api.Tests
{
    public class SortProductsTests
    {
        [Theory]
        [InlineData(SortOption.Low)]
        [InlineData(SortOption.High)]
        [InlineData(SortOption.Ascending)]
        [InlineData(SortOption.Descending)]
        [InlineData(SortOption.Recommended)]
        public async Task WhenNoProducts_ShouldReturnEmptyList(SortOption sortOption)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(new List<Product>().AsEnumerable()));
            resourceClientMock
                .Setup(client => client.GetShoppersHistory())
                .Returns(Task.FromResult(new List<ShopperHistory>().AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(sortOption);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsLow_ShouldSortProductsByPriceInAscendingOrder(List<Product> products)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.Low);

            // Assert
            result.Should().BeEquivalentTo(products.OrderBy(product => product.Price), options => options.WithStrictOrdering());
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsHigh_ShouldSortProductsByPriceInDescendingOrder(List<Product> products)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.High);

            // Assert
            result.Should().BeEquivalentTo(products.OrderByDescending(product => product.Price), options => options.WithStrictOrdering());
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsAscending_ShouldSortProductsByNameInAscendingOrder(List<Product> products)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.Ascending);

            // Assert
            result.Should().BeEquivalentTo(products.OrderBy(product => product.Name), options => options.WithStrictOrdering());
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsDescending_ShouldSortProductsByNameInDescendingOrder(List<Product> products)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.Descending);

            // Assert
            result.Should().BeEquivalentTo(products.OrderByDescending(product => product.Name), options => options.WithStrictOrdering());
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsRecommended_ShouldSortProductsByHistoricalSalesInDescendingOrder(List<Product> products, List<ShopperHistory> shoppersHistory)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            shoppersHistory[0].Products[0] = CopyProduct(products[0], 1);
            shoppersHistory[0].Products[1] = CopyProduct(products[2], 2);
            shoppersHistory[0].Products.RemoveAt(2);
            shoppersHistory[1].Products[0] = CopyProduct(products[2], 2);
            shoppersHistory[1].Products[1] = CopyProduct(products[0], 2);
            shoppersHistory[1].Products.RemoveAt(2);
            shoppersHistory[2].Products[0] = CopyProduct(products[0], 2);
            shoppersHistory[2].Products[1] = CopyProduct(products[2], 3);
            shoppersHistory[2].Products.RemoveAt(2);

            resourceClientMock
                .Setup(client => client.GetShoppersHistory())
                .Returns(Task.FromResult(shoppersHistory.AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.Recommended);

            // Assert
            result.Should().BeEquivalentTo(new List<Product>
            {
                products[2],
                products[0],
                products[1]
            }, options => options.WithStrictOrdering());
        }

        [Theory, AutoData]
        public async Task WhenSortOptionIsRecommended_AndShoppersHistoryIsEmpty_ShouldKeepProductsOrder(List<Product> products)
        {
            // Arrange
            var resourceClientMock = new Mock<IResourceClient>(MockBehavior.Strict);
            resourceClientMock
                .Setup(client => client.GetProducts())
                .Returns(Task.FromResult(products.AsEnumerable()));

            resourceClientMock
                .Setup(client => client.GetShoppersHistory())
                .Returns(Task.FromResult(new List<ShopperHistory>().AsEnumerable()));

            var controller = new ExercisesController(resourceClientMock.Object);

            // Act
            var result = await controller.SortProducts(SortOption.Recommended);

            // Assert
            result.Should().BeEquivalentTo(products, options => options.WithStrictOrdering());
        }

        private static Product CopyProduct(Product product, int quantity)
        {
            return new Product
            {
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
        }
    }
}
