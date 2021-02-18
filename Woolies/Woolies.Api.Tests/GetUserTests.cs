using System;
using FluentAssertions;
using Moq;
using Woolies.Api.Controllers;
using Woolies.Api.Models;
using Xunit;

namespace Woolies.Api.Tests
{
    public class GetUserTests
    {
        [Fact]
        public void ShouldReturnCorrectUserResponse()
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
    }
}
