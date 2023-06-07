namespace Basket.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetTopRankedProducts(List<Product> allProducts, int count);
        IEnumerable<Product> GetPaginatedProductCatalog(int page, int pageSize);
        IEnumerable<Product> GetCheapestProducts(List<Product> allProducts, int count);
    }
}