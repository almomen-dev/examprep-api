using Examprep.Application.DTOs;
using Examprep.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApiClient _api;

        public QuestionsController(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(string? category = null)
        {
            var token = HttpContext.Session.GetString("jwt");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var questions = await _api.GetQuestionsAsync();

            if (!string.IsNullOrEmpty(category) && questions != null)
                questions = questions.Where(q => q.Category == category).ToList();

            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewBag.Role = HttpContext.Session.GetString("role");
            ViewBag.Category = category;
            ViewBag.Categories = questions?
                .Select(q => q.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList() ?? new List<string?>();

            return View(questions ?? new List<QuestionResponseDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionCreateDto dto)
        {
            await _api.CreateQuestionAsync(dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteQuestionAsync(id);
            return RedirectToAction("Index");
        }





        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a CSV file.";
                return RedirectToAction("Index");
            }

            using var stream = file.OpenReadStream();
            var result = await _api.UploadCsvAsync(stream, file.FileName);

            if (result == null)
                TempData["Error"] = "Upload failed.";
            else
                TempData["Success"] = $"Uploaded {result.SuccessCount} questions. {result.FailedCount} failed.";

            return RedirectToAction("Index");
        }
    }
}