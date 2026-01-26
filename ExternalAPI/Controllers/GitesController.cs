using Microsoft.AspNetCore.Mvc;
using ExternalAPI.Classes;

namespace ExternalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GitesController : ControllerBase
    {
        private readonly ExternalApiTestService _service;

        public GitesController(ExternalApiTestService service)
        {
            _service = service;
        }

        // GET: /api/Gites/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GiteResponseDTO>> GetGiteById(int id)
        {
            try
            {
                var gite = await _service.GetGitesByIdAsync(id);
                return Ok(gite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /api/Gites
        [HttpPost]
        public async Task<ActionResult<GiteResponseDTO>> CreateGite(GiteDTO gite)
        {
            try
            {
                var createdGite = await _service.PostGiteAsync(gite);
                return Ok(createdGite);
            }
            catch (Exception ex)
                        {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
