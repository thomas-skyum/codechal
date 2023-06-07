using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Basket.Services;

namespace Basket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        //TODO: Get token from Impact API
        string loginToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdHJpbmciLCJleHAiOjE2ODYxNTkyNzIsImlzcyI6IklNUEFDVC5Db2RlQ2hhbGxlbmdlIiwiYXVkIjoiSU1QQUNULkNvZGVDaGFsbGVuZ2UifQ.DnCmEnfh3AHbbsCvURrqtbSsDIEOL7Q4iW4zwhy6ntE";
        private readonly HttpClient httpClient;
        private List<Product>? allProducts = null;

        private ProductService productService;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            productService = new ProductService();
        }


        // Should not be a public API call, but left it there for testing purposes
        [Route("allproducts")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
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

        [Route("topranked/{count:int}")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetTopRankedProducts(int count)
        {
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }

            return productService.GetTopRankedProducts(allProducts, count);
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

            return productService.GetCheapestProducts(allProducts, count);
        }

        [Route("page/{page:int}/{pageSize:int}")]
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetPaginatedProducts(int page, int pageSize)
        {
            if (pageSize > 1000)
                pageSize = 1000;
            if (allProducts is null || allProducts.Count == 0)
            {
                IActionResult outcome = await GetAllProducts();
                //TODO: add outcome checks and exception functionality
            }

            return productService.GetPaginatedProductCatalog(allProducts, page, pageSize);
        }
    }
}