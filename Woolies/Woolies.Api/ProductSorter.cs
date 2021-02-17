using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Woolies.Api.Models;

namespace Woolies.Api
{
    public class ProductSorter
    {
        private static readonly
            Dictionary<SortOption, Func<IEnumerable<Product>, IResourceClient, Task<IEnumerable<Product>>>> Sorters =
                new Dictionary<SortOption, Func<IEnumerable<Product>, IResourceClient, Task<IEnumerable<Product>>>>
                {
                    [SortOption.Low] = (products, resourceClient) =>
                        Task.FromResult(products.OrderBy(product => product.Price).AsEnumerable()),
                    [SortOption.High] = (products, resourceClient) =>
                        Task.FromResult(products.OrderByDescending(product => product.Price).AsEnumerable()),
                    [SortOption.Ascending] = (products, resourceClient) =>
                        Task.FromResult(products.OrderBy(product => product.Name).AsEnumerable()),
                    [SortOption.Descending] = (products, resourceClient) =>
                        Task.FromResult(products.OrderByDescending(product => product.Name).AsEnumerable()),
                    [SortOption.Recommended] = SortByRecommended,
                };

        public static Task<IEnumerable<Product>> Sort(SortOption sortOption, IEnumerable<Product> products, IResourceClient resourceClient)
        {
            return Sorters[sortOption](products, resourceClient);
        }

        private static async Task<IEnumerable<Product>> SortByRecommended(IEnumerable<Product> products, IResourceClient resourceClient)
        {
            var shoppersHistory = await resourceClient.GetShoppersHistory();
            var productsSaleHistory = shoppersHistory.SelectMany(history => history.Products)
                .GroupBy(product => product.Name)
                .Select(
                    grouping => new
                    {
                        ProductName = grouping.Key,
                        TotalQuantity = grouping.Sum(product => product.Quantity)
                    });

            var currentProductsWithTheirSalesHistory =
                from product in products
                join productSaleHistory in productsSaleHistory on product.Name equals productSaleHistory.ProductName into gj
                from gr in gj.DefaultIfEmpty()
                select new
                {
                    Product = product,
                    SaleQuantity = gr?.TotalQuantity ?? 0
                };

            return currentProductsWithTheirSalesHistory
                .OrderByDescending(product => product.SaleQuantity)
                .Select(product => product.Product);
        }
    }
}
