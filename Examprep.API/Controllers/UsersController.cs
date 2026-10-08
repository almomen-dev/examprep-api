using Asp.Versioning;
using Examprep.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repo.GetAllAsync();
            return Ok(users.Select(u => new
            {
                u.Id,
                u.Email,
                u.Role,
                u.QuestionCount
            }));
        }

        [HttpPut("{id}/promote")]
        public async Task<IActionResult> Promote(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Role = "Admin";
            await _repo.UpdateAsync(user);
            return Ok(new { message = "User promoted to Admin" });
        }

        [HttpPut("{id}/demote")]
        public async Task<IActionResult> Demote(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Role = "User";
            await _repo.UpdateAsync(user);
            return Ok(new { message = "User demoted to User" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _repo.DeleteAsync(user);
            return NoContent();
        }
    }
}