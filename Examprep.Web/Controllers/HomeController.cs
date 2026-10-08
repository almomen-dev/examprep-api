using Examprep.Application.DTOs;
using Examprep.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _api;

        public HomeController(ApiClient api) { _api = api; }

        // Smart Router: Sends logged-in users to dashboard, others to landing page
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("role");
            var token = HttpContext.Session.GetString("jwt");

            if (!string.IsNullOrEmpty(token) && role == "Admin")
                return View("AdminDashboard");

            if (!string.IsNullOrEmpty(token))
                return View("UserDashboard");

            return View(); // not logged in ? landing page
        }

        // Dashboard: Used by sidebar link (/Home/Dashboard)
        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("role");
            var token = HttpContext.Session.GetString("jwt");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role == "Admin")
                return View("AdminDashboard");

            // Fetch real stats for the user
            var stats = await _api.GetMyStatsAsync();
            var recent = await _api.GetMyRecentAttemptsAsync(3);

            ViewBag.Stats = stats;
            ViewBag.Recent = recent ?? new List<RecentAttemptDto>();

            return View("UserDashboard");
        }

        // Leaderboard: Ranked list of users by average score
        public async Task<IActionResult> Leaderboard()
        {
            if (HttpContext.Session.GetString("jwt") == null)
                return RedirectToAction("Login", "Account");

            var data = await _api.GetLeaderboardAsync();
            ViewBag.CurrentEmail = HttpContext.Session.GetString("email");
            return View(data ?? new List<LeaderboardDto>());
        }

        public IActionResult Privacy() => View();
    }
}