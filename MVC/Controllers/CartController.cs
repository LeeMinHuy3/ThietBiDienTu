using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Newtonsoft.Json;

namespace MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7170/api/cart");
            if (response.IsSuccessStatusCode)
            {
                var cartItemsJson = await response.Content.ReadAsStringAsync();
                var cartItems = JsonConvert.DeserializeObject<IEnumerable<CartItem>>(cartItemsJson);
                return View(cartItems);
            }
            return View(new List<CartItem>());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItem cartItem)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7170/api/cart", cartItem);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View("Error");
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var response = await _httpClient.PostAsync("https://localhost:7170/api/cart/checkout", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home"); // Quay về trang chủ sau khi thanh toán
            }
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            // Gửi yêu cầu xóa sản phẩm từ giỏ hàng qua API
            var response = await _httpClient.DeleteAsync($"https://localhost:7170/api/cart/{productId}");

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true }); // Trả về JSON nếu xóa thành công
            }
            return Json(new { success = false }); // Trả về JSON nếu xóa thất bại
        }
    }
}
