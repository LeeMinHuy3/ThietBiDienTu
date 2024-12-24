namespace API.Models
{
    public class CartService : ICartService
    {
        private static List<CartItem> _cartItems = new List<CartItem>();

        public IEnumerable<CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public void AddToCart(CartItem cartItem)
        {
            var existingItem = _cartItems.FirstOrDefault(ci => ci.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity; // Cộng dồn số lượng nếu sản phẩm đã tồn tại
            }
            else
            {
                _cartItems.Add(cartItem);
            }
        }

        public void Checkout()
        {
            // Trừ số lượng sản phẩm trong giỏ hàng
            foreach (var item in _cartItems)
            {
                // Giả định có một service để giảm số lượng sản phẩm
                // ProductService.UpdateProductQuantity(item.ProductId, -item.Quantity);
            }

            _cartItems.Clear(); // Xóa giỏ hàng sau khi thanh toán
        }

        public void RemoveFromCart(int productId)
        {
            var itemToRemove = _cartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove != null)
            {
                _cartItems.Remove(itemToRemove);
            }
        }
    }
}
