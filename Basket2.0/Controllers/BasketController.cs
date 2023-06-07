using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Basket.Services;

namespace Basket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {   
        //TODO: Get token from Impact API
        string loginToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdHJpbmciLCJleHAiOjE2ODYxNTkyNzIsImlzcyI6IklNUEFDVC5Db2RlQ2hhbGxlbmdlIiwiYXVkIjoiSU1QQUNULkNvZGVDaGFsbGVuZ2UifQ.DnCmEnfh3AHbbsCvURrqtbSsDIEOL7Q4iW4zwhy6ntE";
        private readonly HttpClient httpClient;
        private static List<Product>? allProducts;
        private static List<Basket>? allBaskets = new List<Basket>();
        BasketService basketService;
        

        public BasketController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            basketService = new BasketService();
        }

        //TODO: Refactor into one method only, shared by all controllers
        private async Task<IActionResult> GetAllProducts()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + loginToken);
                HttpResponseMessage productCatalogResonse = await httpClient.GetAsync($"https://azfun-impact-code-challenge-api.azurewebsites.net/api/GetAllProducts");

                if (productCatalogResonse.IsSuccessStatusCode)
                {
                    string responseBody = await productCatalogResonse.Content.ReadAsStringAsync();
                    allProducts = JsonSerializer.Deserialize<List<Product>>(responseBody);
                    if(allProducts is null)
                        return NotFound("Product List Empty");
                    return Ok(string.Format("Products retrieved successfully, {0} products total", allProducts.Count));
                }
                else
                {
                    return StatusCode((int)productCatalogResonse.StatusCode, "Product catalog request failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("basket/all")]
        [HttpGet]
        public ActionResult<List<Basket>> GetAllBaskets()
        {
            return Ok(allBaskets);
        }   

        [Route("basket/new")]
        [HttpPost]
        public ActionResult<Basket> CreateBasket()
        {
            Basket basket = basketService.CreateNewBasket();
            allBaskets.Add(basket);
            return Ok(basket);
        }

        [Route("basket/{BasketId:guid}/total")]
        [HttpGet]
        public ActionResult<double> GetBasketTotal(Guid BasketId)
        {
            //TODO: move to own function
            if (allBaskets is null || allBaskets.Count == 0)
                return NotFound("No baskets in collection");
            Basket? basket = allBaskets.Find(b => b.Id == BasketId);
            if (basket is null)
                return NotFound(string.Format("No basket found with Id = {0}",BasketId));
            if (basket.items is null || basket.items.Count == 0)
                return NotFound("Basket is empty");
            double basketTotal = 0;
            foreach (BasketItem item in basket.items)
                basketTotal += (item.product.price * item.Quantity);
            return Ok(basketTotal);
        }    

        [Route("basket/addproduct/{BasketId:Guid}/{ProductId:int}/{Amount:int}")]
        [HttpPost]
        public async Task<ActionResult<string>> AddProductToBasket(Guid BasketId, int ProductId, int Amount)
        {
            
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }
            
            Product? prodToAdd = allProducts.Find(p => p.id == ProductId);

            if (prodToAdd is null)
                return NotFound("Product not found");
            
            //TODO: add functionality to check if product is already in the basket, if so then change quantity instead of add new

            //TODO: move to own function
            if (allBaskets is null || allBaskets.Count == 0)
                return NotFound("No baskets in collection");
            Basket? basket = allBaskets.Find(b => b.Id == BasketId);
            if (basket is null)
                return NotFound(string.Format("No basket found with Id = {0}",BasketId));
            if (basket.items is null)
                basket.items = new List<BasketItem>();

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