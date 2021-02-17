using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Woolies.Api.Models;

namespace Woolies.Api
{
    public class ResourceClient : IResourceClient
    {
        private readonly HttpClient _httpClient;

        public ResourceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<Product>> GetProducts()
        {
            return _httpClient.GetFromJsonAsync<IEnumerable<Product>>($"products?token={Constants.Token}");
        }

        public Task<IEnumerable<ShopperHistory>> GetShoppersHistory()
        {
            return _httpClient.GetFromJsonAsync<IEnumerable<ShopperHistory>>($"shopperHistory?token={Constants.Token}");
        }
    }
}