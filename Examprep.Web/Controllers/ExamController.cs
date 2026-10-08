using Examprep.Application.DTOs;
using Examprep.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.Web.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApiClient _api;

        public ExamController(ApiClient api) { _api = api; }

        public async Task<IActionResult> Start(int count = 10, string? category = null)
        {
            if (HttpContext.Session.GetString("jwt") == null)
                return RedirectToAction("Login", "Account");

            // If no category passed → show selection page
            if (category == null && Request.Query.Count == 0)
            {
                var questions = await _api.GetQuestionsAsync();
                ViewBag.Categories = questions?
                    .Select(q => q.Category)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList() ?? new List<string?>();
                return View("SelectCategory");
            }

            // "All" or empty means mixed/all categories
            var cat = string.IsNullOrWhiteSpace(category) || category == "All" ? null : category;

            var start = await _api.StartExamAsync(count, cat);
            if (start == null)
            {
                TempData["Error"] = "Could not start exam. Try again later.";
                return RedirectToAction("Start");
            }
            return View("Start", start);   // 👈 Uses your ORIGINAL Start.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int attemptId, IFormCollection form)
        {
            var dto = new SubmitExamDto { AttemptId = attemptId };

            foreach (var key in form.Keys.Where(k => k.StartsWith("q_")))
            {
                var qid = int.Parse(key.Replace("q_", ""));
                var selected = form[key].ToString();

                if (!string.IsNullOrEmpty(selected))
                {
                    dto.Answers.Add(new ExamAnswerDto
                    {
                        QuestionId = qid,
                        SelectedOption = selected
                    });
                }
            }

            var result = await _api.SubmitExamAsync(dto);
            if (result == null)
            {
                TempData["Error"] = "Submit failed.";
                return RedirectToAction("Index", "Home");
            }

            return View("Result", result);
        }

        public async Task<IActionResult> History()
        {
            if (HttpContext.Session.GetString("jwt") == null)
                return RedirectToAction("Login", "Account");

            var history = await _api.GetExamHistoryAsync();
            return View(history ?? new List<ExamHistoryDto>());
        }

        public async Task<IActionResult> AllResults()
        {
            if (HttpContext.Session.GetString("role") != "Admin")
                return RedirectToAction("Index", "Home");

            var results = await _api.GetAllExamResultsAsync();
            return View(results ?? new List<AdminExamResultDto>());
        }
    }
}