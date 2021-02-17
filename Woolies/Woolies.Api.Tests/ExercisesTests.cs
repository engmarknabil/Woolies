using System;
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
    public class ExercisesTests
    {
        [Fact]
        public void GetUser_ShouldReturnCorrectUserResponse()
        {
            // Arrange
            var controller = new ExercisesController(new Mock<IResourceClient>(MockBehavior.Strict).Object);

            // Act
            var userResponse = controller.GetUser();
            
            // Assert
            userResponse.Should().BeEquivalentTo(new UserResponse
            {
                Name = "Mark Ibrahim",
                Token = new Guid(Constants.Token)
            });
        }

        [Theory, AutoData]
        public async Task SortProducts_WhenSortOptionIsLow_ShouldSortProductsByPriceInAscendingOrder(List<Product> products)
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
            result.Should().BeEquivalentTo(products.OrderBy(product => product.Price));
        }

        [Theory, AutoData]
        public async Task SortProducts_WhenSortOptionIsHigh_ShouldSortProductsByPriceInDescendingOrder(List<Product> products)
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
            result.Should().BeEquivalentTo(products.OrderByDescending(product => product.Price));
        }

        [Theory, AutoData]
        public async Task SortProducts_WhenSortOptionIsAscending_ShouldSortProductsByNameInAscendingOrder(List<Product> products)
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
            result.Should().BeEquivalentTo(products.OrderBy(product => product.Name));
        }

        [Theory, AutoData]
        public async Task SortProducts_WhenSortOptionIsDescending_ShouldSortProductsByNameInDescendingOrder(List<Product> products)
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
            result.Should().BeEquivalentTo(products.OrderByDescending(product => product.Name));
        }

        [Theory, AutoData]
        public async Task SortProducts_WhenSortOptionIsRcommended_ShouldSortProductsByHistoricalSalesInDescendingOrder(List<Product> products, List<ShopperHistory> shoppersHistory)
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
            });
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
