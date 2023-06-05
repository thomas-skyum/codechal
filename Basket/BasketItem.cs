namespace Basket
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Product? product { get; set; }
        public int Quantity { get; set; }
        
    }
}