using System;
using FluentAssertions;
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
            var controller = new ExercisesController();
            var userResponse = controller.GetUser();
            userResponse.Should().BeEquivalentTo(new UserResponse
            {
                Name = "Mark Ibrahim",
                Token = new Guid("9395ebb4-ddfd-4585-9f0e-4e97d375bc44")
            });
        }
    }
}
