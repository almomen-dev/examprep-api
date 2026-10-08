namespace Examprep.Application.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}