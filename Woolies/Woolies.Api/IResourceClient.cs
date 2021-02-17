using System.Collections.Generic;
using System.Threading.Tasks;
using Woolies.Api.Models;

namespace Woolies.Api
{
    public interface IResourceClient
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<ShopperHistory>> GetShoppersHistory();
    }
}