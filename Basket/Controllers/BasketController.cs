using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Basket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {   
        string loginToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdHJpbmciLCJleHAiOjE2ODYwMDA3NTgsImlzcyI6IklNUEFDVC5Db2RlQ2hhbGxlbmdlIiwiYXVkIjoiSU1QQUNULkNvZGVDaGFsbGVuZ2UifQ.C1U42x-3kKrzBOYz8Uct6GbHGDesL3KhnE8ck-YwHsE";
        private readonly HttpClient httpClient;
        private List<Product>? allProducts = null;
        private Basket basket;
        

        public BasketController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            basket = new Basket();
            basket.Id = Guid.NewGuid();
            basket.LastEdited = DateTime.Now;
            basket.items = new List<BasketItem>();
        }

        private async void GetProductList()
        {
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
                return;
            }
        }

        [Route("basket")]
        [HttpGet]
        public ActionResult<Basket> GetCurrentBasket()
        {
            return Ok(basket);
        }
        
        [Route("basket/total")]
        [HttpGet]
        public ActionResult<double> GetCurrentBasketTotal()
        {
            if (basket.items is null || basket.items.Count == 0)
                return Ok(0);
            double basketTotal = 0;
            foreach (BasketItem item in basket.items)
                basketTotal += (item.product.price * item.Quantity);
            return Ok(basketTotal);
        }

        // Should not be a public API call, but left it there for testing purposes
        [Route("allproducts")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginToken);
                HttpResponseMessage prodCatResp = await httpClient.GetAsync($"https://azfun-impact-code-challenge-api.azurewebsites.net/api/GetAllProducts");

                if (prodCatResp.IsSuccessStatusCode)
                {
                    string prodCatRespBody = await prodCatResp.Content.ReadAsStringAsync();
                    allProducts = JsonSerializer.Deserialize<List<Product>>(prodCatRespBody);
                    if(allProducts is null)
                        return NotFound("Product List Empty");
                    return Ok(string.Format("Products retrieved successfully, {0} products total", allProducts.Count));
                }
                else
                {
                    return StatusCode((int)prodCatResp.StatusCode, "Product catalog request failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("cheapest/{count:int}")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetCheapestProducts(int count)
        {
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }

            List<Product>? cheapestProds = allProducts
                .OrderBy(p => p.price)
                .Take(count)
                .ToList();
            return cheapestProds;
        }

        [Route("topranked/{count:int}")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetTopRankedProducts(int count)
        {
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }

            List<Product>? topProds = allProducts
                .OrderByDescending(p => p.stars)
                .Take(count)
                .ToList();
            return topProds;
        }

        [Route("basket/addprod/{ProdId:int}/{Amount:int}")]
        [HttpPost]
        public async Task<ActionResult<string>> AddProductToBasket(int ProdId, int Amount)
        {
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }

            Product? prodToAdd = allProducts.Find(p => p.id == ProdId);
            if (prodToAdd is null)
                return NotFound("Product not found");
            BasketItem baskItem = new BasketItem();
            baskItem.product = prodToAdd;
            baskItem.Id = Guid.NewGuid();
            baskItem.Quantity = Amount;
            basket.items.Add(baskItem);
            basket.LastEdited = DateTime.Now;
            return Ok("Product added successfully");
        }
    }
}