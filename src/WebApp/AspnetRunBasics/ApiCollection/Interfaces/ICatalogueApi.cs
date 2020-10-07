using AspnetRunBasics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection.Interfaces
{
    public interface ICatalogueApi
    {
        Task<IEnumerable<CatalogueModel>> GetCatalogue();

        Task<IEnumerable<CatalogueModel>> GetCatalogueByCategory(string categoryName);

        Task<CatalogueModel> GetCatalogue(string id);

        Task<CatalogueModel> CreateCatalogue(CatalogueModel model);
    }
}
