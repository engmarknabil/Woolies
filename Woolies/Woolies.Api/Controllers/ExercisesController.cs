using System;
using Microsoft.AspNetCore.Mvc;
using Woolies.Api.Models;

namespace Woolies.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        [HttpGet("user")]
        public UserResponse GetUser()
        {
            return new UserResponse
            {
                Name = "Mark Ibrahim",
                Token = new Guid("9395ebb4-ddfd-4585-9f0e-4e97d375bc44")
            };
        }
    }
}
