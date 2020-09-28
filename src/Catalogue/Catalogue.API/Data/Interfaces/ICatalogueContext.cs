using Catalogue.API.Entities;
using MongoDB.Driver;

namespace Catalogue.API.Data.Interfaces
{
    public interface ICatalogueContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
