using Basket.Interfaces;

namespace Basket.Services
{
    public class BasketService : IBasketService
    {
        public BasketService()
        {
        }

        public Basket CreateNewBasket()
        {
            Basket basket = new Basket();
            basket.Id = Guid.NewGuid();
            basket.LastEdited = DateTime.Now;
            return basket;
        }
        public void AddProductToBasket(Basket basket, Product product, int quantity)
        {
            //basket.Add(product);
        }

        public void RemoveProductFromBasket(Basket basket, Product product)
        {
            //basket.Remove(product);
        }

        public void UpdateProductQuantity(Product product)
        {
            // var existingProduct = _basket.FirstOrDefault(p => p.Id == product.Id);
            // if (existingProduct != null)
            // {
            //     existingProduct.Quantity = product.Quantity;
            // }
        }
    }
}