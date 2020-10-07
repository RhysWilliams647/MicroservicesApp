using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogueApi catalogueApi;
        private readonly IBasketApi basketApi;

        public IndexModel(ICatalogueApi catalogueApi, IBasketApi basketApi)
        {
            this.catalogueApi = catalogueApi ?? throw new ArgumentNullException(nameof(catalogueApi));
            this.basketApi = basketApi ?? throw new ArgumentNullException(nameof(basketApi));
        }

        public IEnumerable<CatalogueModel> ProductList { get; set; } = new List<CatalogueModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await catalogueApi.GetCatalogue();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            //if (!User.Identity.IsAuthenticated)
            //    return RedirectToPage("./Account/Login", new { area = "Identity" });

            var product = await catalogueApi.GetCatalogue(productId);

            var userName = "swn";
            var basket = await basketApi.GetBasket(userName);

            basket.Items.Add(new BasketItemModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                Color = "Black"
            });

            var basketUpdated = await basketApi.UpdateBasket(basket);
            return RedirectToPage("Cart");
        }
    }
}
