using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MVC.Models;
using Newtonsoft.Json;
using System.Text;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7170/api/user/login", content);
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                Role = "customer"
            };

            var jsonContent = JsonConvert.SerializeObject(user);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7170/api/user/register", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
