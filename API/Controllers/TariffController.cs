using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models;
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public TariffController(CampingDbContext context)
        {
            _context = context;
        }

        // GET: api/tariff
        [HttpGet]
        public async Task<ActionResult<List<Tariff>>> GetTariffs()
        {
            var allTariffs = await _context.Tariffs
            .ToListAsync();

            return allTariffs;
        }

        // GET: api/tariff/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tariff>> GetTariff(int id)
        {
            var tariff = await _context.Tariffs
                .FirstOrDefaultAsync(c => c.TariffId == id);

            if (tariff == null)
            {
                return NotFound();
            }

            return tariff;
        }

        // POST: api/tariff
        [HttpPost]
        public async Task<ActionResult<Tariff>> PostTariff(Tariff tariff)
        {
            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync();

            return Ok(tariff);
        }

        // PUT: api/tariff
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTariff(int id, Tariff tariff)
        {
            if (id != tariff.TariffId)
            {
                return NotFound();
            }

            _context.Entry(tariff).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            if (!TariffExists(id))
            {
                return NotFound();
            }

           return NoContent();

        }

        // bool voor PUT method
        private bool TariffExists(int id)
        {
            var exists = _context.Tariffs.Any(e => e.TariffId == id);

            return exists;
        }

        // DELETE: api/tariff
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTariff(int id)
        {
            var tariff = await _context.Tariffs.FindAsync(id);

            if (tariff == null)
            {
                return NotFound();
            }

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
