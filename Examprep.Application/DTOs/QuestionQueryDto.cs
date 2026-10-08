namespace Examprep.Application.DTOs
{
    public class QuestionQueryDto
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }    // "id" or "text"
        public string? SortDir { get; set; }   // "asc" or "desc"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Category { get; set; }
    }
}