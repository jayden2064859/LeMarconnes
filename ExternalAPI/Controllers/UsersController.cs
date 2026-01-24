using Microsoft.AspNetCore.Mvc;
using ExternalAPI.Classes;

namespace ExternalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ExternalApiTestService _service;

        public UsersController(ExternalApiTestService service)
        {
            _service = service;
        }

        // GET: /api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDTO>>> GetUsers()
        {
            try
            {
                var users = await _service.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
