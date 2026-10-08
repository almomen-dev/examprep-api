using Examprep.Application.DTOs;
using Examprep.Application.Repositories;
using Examprep.Domain.Model;
using System.ComponentModel.DataAnnotations;

namespace Examprep.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repo;
        private readonly TokenService _tokenService;

        public AuthService(IUserRepository repo, TokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new ValidationException("Invalid email or password");

            if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
                throw new ValidationException("Invalid email or password");

            var accessToken = _tokenService.CreateToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repo.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role
            };
        }
        public async Task<TokenResponseDto> RefreshAsync(string refreshToken)
        {
            var user = await _repo.GetByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new ValidationException("Invalid or expired refresh token");

            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repo.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _repo.GetByEmailAsync(dto.Email);
            if (existingUser != null) { 
            throw new ValidationException("User with this email already exists.");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role == "Admin" ? "Admin" : "User"
            };  

            var createdUser = await _repo.AddAsync(user);

            return new AuthResponseDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                Role = createdUser.Role
            };
        }
   

    


    }
}
