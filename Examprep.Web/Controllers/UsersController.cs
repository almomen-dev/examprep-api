using Examprep.Application.DTOs;
using Examprep.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiClient _api;

        public UsersController(ApiClient api) { _api = api; }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("role") != "Admin")
                return RedirectToAction("Index", "Home");

            var users = await _api.GetUsersAsync();
            return View(users ?? new List<Examprep.Application.DTOs.UserListDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Promote(int id)
        {
            await _api.PromoteUserAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Demote(int id)
        {
            await _api.DemoteUserAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}