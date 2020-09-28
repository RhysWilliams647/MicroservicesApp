using Catalogue.API.Entities;
using Catalogue.API.Repositories.Interfaces;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalogue.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogueController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<CatalogueController> logger;

        public CatalogueController(
            IProductRepository productRepository,
            ILogger<CatalogueController> logger)
        {
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(await productRepository.GetProducts());
        }

        [HttpGet("{id:length(24)}", Name = nameof(CatalogueController.GetProduct))]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            var product = await productRepository.GetProduct(id);

            if (product == null)
            {
                logger.LogError($"Product {id} not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet]
        [Route("[action]/{categoryName}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string categoryName)
        {
            return Ok(await productRepository.GetProductsByCategory(categoryName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await productRepository.Create(product);

            return CreatedAtRoute(nameof(CatalogueController.GetProduct), new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await productRepository.Update(product));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            return Ok(await productRepository.Delete(id));
        }
    }
}
