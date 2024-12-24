namespace MVC.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string UserId { get; set; }
    }
}
