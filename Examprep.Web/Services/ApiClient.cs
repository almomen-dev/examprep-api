using System.Net.Http.Headers;
using System.Net.Http.Json;
using Examprep.Application.DTOs;

namespace Examprep.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _ctx;

        public ApiClient(HttpClient http, IHttpContextAccessor ctx)
        {
            _http = http;
            _ctx = ctx;
            _http.BaseAddress = new Uri("http://almomen.runasp.net/");
        }

        private void AttachToken()
        {
            var token = _ctx.HttpContext?.Session.GetString("jwt");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // Auth
        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/v1/auth/login", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<TokenResponseDto>();
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/v1/auth/register", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<AuthResponseDto>();
        }

        // Questions
        public async Task<List<QuestionResponseDto>?> GetQuestionsAsync()
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<QuestionResponseDto>>("api/v1/questions");
        }

        public async Task<bool> CreateQuestionAsync(QuestionCreateDto dto)
        {
            AttachToken();
            var resp = await _http.PostAsJsonAsync("api/v1/questions", dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            AttachToken();
            var resp = await _http.DeleteAsync($"api/v1/questions/{id}");
            return resp.IsSuccessStatusCode;
        }






        public async Task<List<UserListDto>?> GetUsersAsync()
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<UserListDto>>("api/v1/users");
        }

        public async Task<bool> PromoteUserAsync(int id)
        {
            AttachToken();
            var resp = await _http.PutAsync($"api/v1/users/{id}/promote", null);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DemoteUserAsync(int id)
        {
            AttachToken();
            var resp = await _http.PutAsync($"api/v1/users/{id}/demote", null);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            AttachToken();
            var resp = await _http.DeleteAsync($"api/v1/users/{id}");
            return resp.IsSuccessStatusCode;
        }



        public async Task<CsvUploadResultDto?> UploadCsvAsync(Stream fileStream, string fileName)
        {
            AttachToken();

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "file", fileName);

            var resp = await _http.PostAsync("api/v1/questions/upload-csv", content);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<CsvUploadResultDto>();
        }


        public async Task<StartExamDto?> StartExamAsync(int count = 10, string? category = null)
        {
            AttachToken();
            var url = $"api/v1/exam/start?count={count}";
            if (!string.IsNullOrEmpty(category))
                url += $"&category={Uri.EscapeDataString(category)}";

            var resp = await _http.PostAsync(url, null);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<StartExamDto>();
        }

        public async Task<ExamResultDto?> SubmitExamAsync(SubmitExamDto dto)
        {
            AttachToken();
            var resp = await _http.PostAsJsonAsync("api/v1/exam/submit", dto);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ExamResultDto>();
        }

        public async Task<List<ExamHistoryDto>?> GetExamHistoryAsync()
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<ExamHistoryDto>>("api/v1/exam/history");
        }




        public async Task<List<AdminExamResultDto>?> GetAllExamResultsAsync()
        {
            AttachToken();
            return await _http.GetFromJsonAsync<List<AdminExamResultDto>>("api/v1/exam/admin/results");
        }


        public async Task<List<LeaderboardDto>?> GetLeaderboardAsync()
        {
            try
            {
                AttachToken();
                var response = await _http.GetAsync("api/v1/leaderboard");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<List<LeaderboardDto>>();
            }
            catch
            {
                return null;
            }
        }






        public async Task<UserStatsDto?> GetMyStatsAsync()
        {
            try
            {
                AttachToken();
                var response = await _http.GetAsync("api/v1/stats/me");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<UserStatsDto>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RecentAttemptDto>?> GetMyRecentAttemptsAsync(int count = 3)
        {
            try
            {
                AttachToken();
                var response = await _http.GetAsync($"api/v1/stats/recent?count={count}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<List<RecentAttemptDto>>();
            }
            catch
            {
                return null;
            }
        }
    }
}