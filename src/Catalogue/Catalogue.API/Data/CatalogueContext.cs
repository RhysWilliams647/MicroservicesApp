using Catalogue.API.Data.Interfaces;
using Catalogue.API.Entities;
using Catalogue.API.Settings;
using MongoDB.Driver;
using System;

namespace Catalogue.API.Data
{
    public class CatalogueContext : ICatalogueContext
    {
        public CatalogueContext(ICatalogueDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            Products = database.GetCollection<Product>(settings.CollectionName);
            CatalogueContextSeed.SeedData(Products);

        }

        public IMongoCollection<Product> Products { get; }
    }
}
