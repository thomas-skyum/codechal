namespace Basket.Interfaces
{
    public interface IBasketService
    {
        void AddProductToBasket(Basket basket, Product product, int quantity);
        void RemoveProductFromBasket(Basket basket, Product product);
        void UpdateProductQuantity(Product product);
    }
}