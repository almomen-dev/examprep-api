using System.ComponentModel.DataAnnotations;

namespace Examprep.Application.DTOs
{
    public class QuestionUpdateDto
    {

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Text { get; set; } = string.Empty;


    }
}
