namespace API.Models
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCartItems();
        void AddToCart(CartItem cartItem);
        void Checkout();
        void RemoveFromCart(int productId);
    }
}
