using AuthMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AuthMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient("AuthApi");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/auth/login", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result is null)
            {
                ModelState.AddModelError("", "Login failed: empty response");
                return View(model);
            }

            // Store JWT and username in session
            HttpContext.Session.SetString("JWT", result.Token);
            HttpContext.Session.SetString("Username", model.Username);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient("AuthApi");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/auth/register", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Registration failed");
                return View(model);
            }

            // On success, redirect to login
            return RedirectToAction("Login");
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWT");
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login");
        }
    }
}