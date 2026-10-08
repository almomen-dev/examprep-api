using Examprep.Application.DTOs;
using Examprep.Application.Repositories;
using Examprep.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace Examprep.Application.Services
{
    public class QuestionService
    {
        private readonly IQuestionRepository _repo;
        private readonly ILogger<QuestionService> _logger;
        private readonly IMemoryCache _cache;

        public QuestionService(IQuestionRepository repo,
                               ILogger<QuestionService> logger,
                               IMemoryCache cache)
        {
            _repo = repo;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            const string cacheKey = "all_questions";

            // 1. Check cache
            if (_cache.TryGetValue(cacheKey, out List<QuestionResponseDto>? cached) && cached != null)
            {
                _logger.LogInformation("Returning cached questions");
                return cached;
            }

            // 2. Cache miss → fetch from DB
            _logger.LogInformation("Fetching questions from DB");
            var result = await _repo.GetAllQuestionsAsync();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }



        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(int id)
        {
            _logger.LogInformation("Fetching question with ID {Id}", id);
            var question = await _repo.GetQuestionByIdAsync(id);
            if (question == null)
            {
                _logger.LogWarning("Question with ID {Id} not found", id);
                return null;
            }
            return new QuestionResponseDto { Id = question.id, Text = question.text };
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(QuestionCreateDto dto, int userId)
        {
            var question = new Question
            {
                text = dto.Text,
                UserId = userId,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption,
                Category = dto.Category
            };

            await _repo.AddQuestionWithCounterAsync(question, userId);
            _cache.Remove("all_questions");
            // Reload with User included so we can return the email
            var created = await _repo.GetQuestionByIdAsync(question.id);

            return new QuestionResponseDto
            {
                Id = created!.id,
                Text = created.text,
                UserId = created.UserId,
                UserEmail = created.User?.Email
            };
        }
        public async Task<bool> UpdateQuestionAsync(int id, QuestionUpdateDto dto)
        {
            var question = await _repo.GetQuestionByIdAsync(id);
            if (question == null) return false;
            question.text = dto.Text;
            await _repo.UpdateQuestionAsync(question);

            _cache.Remove("all_questions");
            return true;
        }
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _repo.GetQuestionByIdAsync(id);
            if (question == null) return false;
             await _repo.DeleteQuestionAsync(question);


            _cache.Remove("all_questions");
            return true;
        }

        public async Task<List<QuestionResponseDto>> SearchQuestionsAsync(string search)
        {
            var questions = await _repo.GetAllQuestionsAsync();
            return questions
                .Where(q => q.Text.Contains(search))
                .ToList();
        }


        public async Task<PagedResultDto<QuestionResponseDto>> GetPagedAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(page, pageSize);

            return new PagedResultDto<QuestionResponseDto>
            {
                Items = items.Select(q => new QuestionResponseDto
                {
                    Id = q.id,
                    Text = q.text
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<QuestionResponseDto>> QueryAsync(QuestionQueryDto dto)
        {
            if (dto.Page < 1) dto.Page = 1;
            if (dto.PageSize < 1 || dto.PageSize > 100) dto.PageSize = 10;

            var (items, totalCount) = await _repo.QueryAsync(
    dto.Search, dto.Category, dto.SortBy, dto.SortDir, dto.Page, dto.PageSize);

            return new PagedResultDto<QuestionResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }



        public async Task<CsvUploadResultDto> BulkUploadAsync(List<QuestionCreateDto> dtos, int userId)
        {
            var result = new CsvUploadResultDto();
            var validQuestions = new List<Question>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Text) || dto.Text.Length < 3)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Invalid text: '{dto.Text}'");
                    continue;
                }

                if (dto.CorrectOption != null &&
                    !new[] { "A", "B", "C", "D" }.Contains(dto.CorrectOption.ToUpper()))
                {
                    result.FailedCount++;
                    result.Errors.Add($"Invalid CorrectOption for '{dto.Text}' — must be A, B, C, or D");
                    continue;
                }

                validQuestions.Add(new Question
                {
                    text = dto.Text,
                    UserId = userId,
                    OptionA = dto.OptionA,
                    OptionB = dto.OptionB,
                    OptionC = dto.OptionC,
                    OptionD = dto.OptionD,
                    CorrectOption = dto.CorrectOption?.ToUpper(),
                    Category = dto.Category
                });
            }

            if (validQuestions.Count > 0)
                await _repo.AddRangeAsync(validQuestions);

            result.SuccessCount = validQuestions.Count;
            _cache.Remove("all_questions");
            return result;
        }


    }
}
