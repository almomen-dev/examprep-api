using Examprep.Application.DTOs;
using Examprep.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api;

        public AccountController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _api.LoginAsync(dto);
            if (result == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View(dto);
            }

            HttpContext.Session.SetString("jwt", result.AccessToken);
            HttpContext.Session.SetString("email", result.Email);
            HttpContext.Session.SetString("role", result.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _api.RegisterAsync(dto);
            if (result == null)
            {
                ViewBag.Error = "Registration failed (email may already exist)";
                return View(dto);
            }

            return RedirectToAction("Login");
        }
    }
}