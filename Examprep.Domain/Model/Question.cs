namespace Examprep.Domain.Model
{
    public class Question
    {
        public int id { get; set; }
        public string text { get; set; }

        public int? UserId { get; set; }          // FK (nullable in case of old data)
        public User? User { get; set; }           // navigation to user
    }
}
