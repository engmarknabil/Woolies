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
            switch (sortOption)
            {
                case SortOption.Low:
                    return products.OrderBy(product => product.Price);
                case SortOption.High:
                    return products.OrderByDescending(product => product.Price);
                case SortOption.Ascending:
                    return products.OrderBy(product => product.Name);
                case SortOption.Descending:
                    return products.OrderByDescending(product => product.Name);
                case SortOption.Recommended:
                    return await SortByRecommended(products);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null);
            }
        }

        private async Task<IEnumerable<Product>> SortByRecommended(IEnumerable<Product> products)
        {
            var shoppersHistory = await _resourceClient.GetShoppersHistory();
            var productsSaleHistory = shoppersHistory.SelectMany(history => history.Products).GroupBy(product => product.Name)
                .Select(
                    grouping => new
                    {
                        ProductName = grouping.Key,
                        TotalQuantity = grouping.Sum(product => product.Quantity)
                    });

            var currentProductsWithTheirSalesHistory = 
                from product in products
                join productSaleHistory in productsSaleHistory on product.Name equals productSaleHistory
                    .ProductName into gj
                from gr in gj.DefaultIfEmpty()
                select new
                {
                    Product = product,
                    SaleQuantity = gr?.TotalQuantity ?? 0
                };

            return currentProductsWithTheirSalesHistory.OrderByDescending(product => product.SaleQuantity)
                .Select(product => product.Product);
        }
    }
}
