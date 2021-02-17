using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Woolies.Api.Models;

namespace Woolies.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly IResourceClient _resourceClient;

        public ExercisesController(IResourceClient resourceClient)
        {
            _resourceClient = resourceClient;
        }

        [HttpGet("user")]
        public UserResponse GetUser()
        {
            return new UserResponse
            {
                Name = "Mark Ibrahim",
                Token = new Guid(Constants.Token)
            };
        }

        [HttpGet("sort")]
        public async Task<IEnumerable<Product>> SortProducts([FromQuery] SortOption sortOption)
        {
            var products = await _resourceClient.GetProducts();
            return await ProductSorter.Sort(sortOption, products, _resourceClient);
        }
    }
}
