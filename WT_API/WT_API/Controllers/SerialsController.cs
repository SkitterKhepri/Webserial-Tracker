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
    public class SerialsController : ControllerBase
    {
        private readonly Context _context;

        public SerialsController(Context context)
        {
            _context = context;
        }

        // GET: api/Serials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Serial>>> GetSerials()
        {
          if (_context.Serials == null)
          {
              return NotFound();
          }
            return await _context.Serials.ToListAsync();
        }

        // GET: api/Serials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Serial>> GetSerial(string id)
        {
          if (_context.Serials == null)
          {
              return NotFound();
          }
            var serial = await _context.Serials.FindAsync(id);

            if (serial == null)
            {
                return NotFound();
            }

            return serial;
        }

        // PUT: api/Serials/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSerial(string id, Serial serial)
        {
            if (id != serial.id)
            {
                return BadRequest();
            }

            _context.Entry(serial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SerialExists(id))
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

        // POST: api/Serials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Serial>> PostSerial(Serial serial)
        {
          if (_context.Serials == null)
          {
              return Problem("Entity set 'Context.Serials'  is null.");
          }
            _context.Serials.Add(serial);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SerialExists(serial.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSerial", new { id = serial.id }, serial);
        }

        // DELETE: api/Serials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSerial(string id)
        {
            if (_context.Serials == null)
            {
                return NotFound();
            }
            var serial = await _context.Serials.FindAsync(id);
            if (serial == null)
            {
                return NotFound();
            }

            _context.Serials.Remove(serial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SerialExists(string id)
        {
            return (_context.Serials?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
