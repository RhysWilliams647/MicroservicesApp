using AspnetRunBasics.ApiCollection.Infrastructure;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using AspnetRunBasics.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRunBasics.ApiCollection
{
    public class CatalogueApi : BaseHttpClientWithFactory, ICatalogueApi
    {
        private readonly IApiSettings settings;
        private readonly ILogger<CatalogueApi> logger;

        public CatalogueApi(IHttpClientFactory factory, IApiSettings settings, ILogger<CatalogueApi> logger) : base(factory)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            logger.LogDebug($"Our base address is {settings.BaseAddress}");
        }

        public async Task<IEnumerable<CatalogueModel>> GetCatalogue()
        {
            var message = new HttpRequestBuilder(settings.BaseAddress)
                .SetPath(settings.CataloguePath)
                .HttpMethod(HttpMethod.Get)
                .GetHttpMessage();

            return await SendRequest<IEnumerable<CatalogueModel>>(message);
        }

        public async Task<IEnumerable<CatalogueModel>> GetCatalogueByCategory(string categoryName)
        {
            var message = new HttpRequestBuilder(settings.BaseAddress)
                               .SetPath(settings.CataloguePath)
                               .AddToPath("GetProductByCategory")
                               .AddToPath(categoryName)
                               .HttpMethod(HttpMethod.Get)
                               .GetHttpMessage();

            return await SendRequest<IEnumerable<CatalogueModel>>(message);
        }

        public async Task<CatalogueModel> GetCatalogue(string id)
        {
            var message = new HttpRequestBuilder(settings.BaseAddress)
                               .SetPath(settings.CataloguePath)
                               .AddToPath(id)
                               .HttpMethod(HttpMethod.Get)
                               .GetHttpMessage();

            return await SendRequest<CatalogueModel>(message);
        }

        public async Task<CatalogueModel> CreateCatalogue(CatalogueModel model)
        {
            var message = new HttpRequestBuilder(settings.BaseAddress)
                                .SetPath(settings.CataloguePath)
                                .HttpMethod(HttpMethod.Post)
                                .GetHttpMessage();

            var json = JsonConvert.SerializeObject(model);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await SendRequest<CatalogueModel>(message);
        }
    }
}
