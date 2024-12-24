using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private static List<CartItem> _cartItems = new List<CartItem>();

        [HttpGet]
        public IActionResult GetCartItems()
        {
            return Ok(_cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(CartItem cartItem)
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

            return Ok(_cartItems);
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            // Trừ số lượng sản phẩm trong giỏ hàng
            foreach (var item in _cartItems)
            {
                // Giả định có một service để giảm số lượng sản phẩm
                // ProductService.UpdateProductQuantity(item.ProductId, -item.Quantity);
            }

            _cartItems.Clear(); // Xóa giỏ hàng sau khi thanh toán
            return Ok("Checkout successful");
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            var itemToRemove = _cartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove != null)
            {
                _cartItems.Remove(itemToRemove);
            }

            return Ok(_cartItems);
        }
    }
}
