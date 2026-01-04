using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models;
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationTypeController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public AccommodationTypeController(CampingDbContext context)
        {
            _context = context;
        }

        // GET: api/accommodationType
        [HttpGet]
        public async Task<ActionResult<List<AccommodationType>>> GetAccommodationTypes()
        {

            var types = await _context.AccommodationsTypes
            .ToListAsync();

            return types;
        }

        // GET: api/accommodationType/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AccommodationType>> GetAccommodationType(int id)
        {
            var accommodationType = await _context.AccommodationsTypes
                .FirstOrDefaultAsync(c => c.AccommodationTypeId == id);

            if (accommodationType == null)
            {
                return NotFound();
            }

            return accommodationType;
        }


        //  POST: api/accommodationType
        [HttpPost]
        public async Task<ActionResult<AccommodationType>> PostAccommodationType(AccommodationType accommodationType)
        {
            _context.AccommodationsTypes.Add(accommodationType);
            await _context.SaveChangesAsync();

            return Ok(accommodationType);
        }



        // PUT: api/accommodationType
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccommodationType(int id, AccommodationType accommodationType)
        {
            if (id != accommodationType.AccommodationTypeId)
            {
                return NotFound();
            }

            if (!AccommodationTypeExists(id))
            {
                return NotFound();
            }

            _context.Entry(accommodationType).State = EntityState.Modified;
            await _context.SaveChangesAsync(); 

            return NoContent();
        }

        // bool voor PUT method
        private bool AccommodationTypeExists(int id)
        {
            var typeExists = _context.AccommodationsTypes.Any(e => e.AccommodationTypeId == id);

            return typeExists;
        }


        // DELETE: api/accommodationType
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccommodationType(int id)
        {
            var accommodationType = await _context.AccommodationsTypes.FindAsync(id);

            if (accommodationType == null)
            {
                return NotFound();
            }

            _context.AccommodationsTypes.Remove(accommodationType);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}

