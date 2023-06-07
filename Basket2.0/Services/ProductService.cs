using Basket.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Basket.Services
{
    public class ProductService
    {
        public ProductService()
        {
            
        }

        public List<Product> GetTopRankedProducts(List<Product> allProducts, int count)
        {
            List<Product>? topProducts = allProducts
                .OrderByDescending(p => p.stars)
                .Take(count)
                .ToList();
            return topProducts;
        }

        public List<Product> GetPaginatedProductCatalog(List<Product> allProducts, int page, int pageSize)
        {
            List<Product>? pagedProducts = allProducts
                .OrderBy(p => p.price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return pagedProducts;
        }

        public List<Product> GetCheapestProducts(List<Product> allProducts, int count)
        {
            List<Product>? cheapestProducts = allProducts
                .OrderBy(p => p.price)
                .Take(count)
                .ToList();
            return cheapestProducts;
        }
    }
}