namespace Examprep.Application.DTOs
{
    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserEmail { get; set; }
    }
}