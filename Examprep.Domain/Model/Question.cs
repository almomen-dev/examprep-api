namespace Examprep.Domain.Model
{
    public class Question
    {
        public int id { get; set; }
        public string text { get; set; } = string.Empty;

        public int? UserId { get; set; }          // FK (nullable in case of old data)
        public User? User { get; set; }           // navigation to user

        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public string? Category { get; set; }
    }
}
