namespace Basket
{
    public class Basket
    {
        public Guid Id { get; set; }
        public List<BasketItem>? items { get; set; }
        public DateTime LastEdited { get; set; }
    }
}