using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WT_API.Data;
using WT_API.Models;

namespace WT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikedSerialsController : ControllerBase
    {
        private readonly Context _context;

        public LikedSerialsController(Context context)
        {
            _context = context;
        }

        // GET: api/LikedSerials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikedSerial>>> GetLikedSerials()
        {
          if (_context.LikedSerials == null)
          {
              return NotFound();
          }
            return await _context.LikedSerials.ToListAsync();
        }

        // GET: api/LikedSerials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LikedSerial>> GetLikedSerial(int id)
        {
          if (_context.LikedSerials == null)
          {
              return NotFound();
          }
            var likedSerial = await _context.LikedSerials.FindAsync(id);

            if (likedSerial == null)
            {
                return NotFound();
            }

            return likedSerial;
        }

        // PUT: api/LikedSerials/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLikedSerial(int id, LikedSerial likedSerial)
        {
            if (id != likedSerial.id)
            {
                return BadRequest();
            }

            _context.Entry(likedSerial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikedSerialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LikedSerials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LikedSerial>> PostLikedSerial(LikedSerial likedSerial)
        {
          if (_context.LikedSerials == null)
          {
              return Problem("Entity set 'Context.LikedSerials'  is null.");
          }
            _context.LikedSerials.Add(likedSerial);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLikedSerial", new { id = likedSerial.id }, likedSerial);
        }

        // DELETE: api/LikedSerials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLikedSerial(int id)
        {
            if (_context.LikedSerials == null)
            {
                return NotFound();
            }
            var likedSerial = await _context.LikedSerials.FindAsync(id);
            if (likedSerial == null)
            {
                return NotFound();
            }

            _context.LikedSerials.Remove(likedSerial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LikedSerialExists(int id)
        {
            return (_context.LikedSerials?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
