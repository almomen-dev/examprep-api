using Examprep.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examprep.API.Controllers
{
    using Asp.Versioning;

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/external")]
    public class ExternalController : ControllerBase
    {
        private readonly ExternalApiService _external;

        public ExternalController(ExternalApiService external)
        {
            _external = external;
        }

        [HttpGet("todos/{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            var result = await _external.GetTodoAsync(id);
            return Content(result, "application/json");
        }
    }
}
