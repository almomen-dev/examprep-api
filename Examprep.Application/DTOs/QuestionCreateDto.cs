using System.ComponentModel.DataAnnotations;

namespace Examprep.Application.DTOs
{
    public class QuestionCreateDto
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Text { get; set; } = string.Empty;

        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public string? Category { get; set; }
    }
}