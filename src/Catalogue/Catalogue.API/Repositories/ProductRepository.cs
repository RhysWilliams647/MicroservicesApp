using Catalogue.API.Data.Interfaces;
using Catalogue.API.Entities;
using Catalogue.API.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogue.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogueContext context;

        public ProductRepository(ICatalogueContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await context
                .Products
                .Find(p => true)
                .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await context
                .Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await context
                .Products
                .Find(p => p.Name == name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            return await context
                .Products
                .Find(p => p.Category == categoryName)
                .ToListAsync();
        }

        public async Task Create(Product product)
        {
            await context.Products.InsertOneAsync(product);
        }

        public async Task<bool> Update(Product product)
        {
            var updateResult = await context
                .Products
                .ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
            return updateResult.IsAcknowledged
                && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            var deleteResult = await context
                .Products
                .DeleteOneAsync(p => p.Id == id);
            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }
    }
}
