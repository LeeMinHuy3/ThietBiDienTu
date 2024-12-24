using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace MVC.Controllers
{
    //[Authorize] // Tất cả các phương thức trong controller này đều yêu cầu đăng nhập
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [AllowAnonymous] // Cho phép truy cập không cần đăng nhập
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7170/api/products");
            if (response.IsSuccessStatusCode)
            {
                var productsJson = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(productsJson);
                return View(products);
            }
            return View(new List<Product>());
        }

        // Chỉ admin mới có quyền truy cập vào phương thức Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            // Xử lý upload ảnh
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(product.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/images/{fileName}";
            }

            // Gửi sản phẩm mới đến API
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7170/api/products", product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Quay về danh sách sản phẩm
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error creating product: {errorContent}");
                return View(product); // Trả về view nếu lỗi
            }
        }


        // Chỉ admin mới có quyền truy cập vào phương thức Edit
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7170/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var productJson = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(productJson);
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Product product)
        {
            // Xử lý upload ảnh nếu có
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(product.ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/images/{fileName}";
            }

            // Gửi sản phẩm đã chỉnh sửa tới API
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7170/api/products/{product.Id}", product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Quay về danh sách sản phẩm
            }
            else
            {
                ModelState.AddModelError("", "Unable to edit product.");
                return View(product); // Trả về view nếu lỗi
            }
        }

        // Chỉ admin mới có quyền truy cập vào phương thức Delete
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7170/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var productJson = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(productJson);
                return View(product);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7170/api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Quay về danh sách sản phẩm
            }
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Lấy thông tin sản phẩm từ API
            var productResponse = await _httpClient.GetAsync($"https://localhost:7170/api/products/{productId}");

            if (productResponse.IsSuccessStatusCode)
            {
                var productJson = await productResponse.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(productJson);

                // Tạo cart item với số lượng mặc định là 1
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = 1 // Số lượng mặc định
                };

                // Gửi cart item tới API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7170/api/cart", cartItem);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View("Error");
        }
    }
}
