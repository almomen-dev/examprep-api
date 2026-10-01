namespace Examprep.Application.DTOs
{
    public class TokenResponseDto
    {
        
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        
    }
}