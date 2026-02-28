using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AuthMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Get the username stored in session after login
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Username = username;
            return View();
        }
    }
}