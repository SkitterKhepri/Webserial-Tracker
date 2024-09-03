using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WT_API.Data;
using WT_API.Models;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;
using WT_API.Services;
using Microsoft.AspNetCore.Authorization;

namespace WT_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SerialsController : ControllerBase
  {
    private readonly Context _context;
    private readonly ScrapingService _scraper;

    public SerialsController(Context context, ScrapingService scraper)
    {
      _context = context;
      _scraper = scraper;
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

    //Same as above, but adds all the new chapters of all serials first, probably takes really fucking long TODO: only admin
    [HttpGet("/Serials/update")]
    public async Task<ActionResult<IEnumerable<Serial>>> UpdatedSerials()
    {
      if (_context.Serials == null)
      {
        return NotFound();
      }
      var (result, updatedChs) = await _scraper.UpdateSerials();
      return Ok(updatedChs);
    }

    // GET: api/Serials/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Serial>> GetSerial(int id)
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

    //Same as above, but adds the new chapters of the serial first
    [HttpGet("/Serials/update/{id}")]
    public async Task<ActionResult<Serial>> UpdatedSerial(int id)
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
      var (result, updatedChs) = await _scraper.UpdateSerial(id);

      return Ok(updatedChs);
    }

    // PUT: api/Serials/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> PutSerial(int id, Serial serial, string authorName)
    {
      if (id != serial.id)
      {
        return BadRequest();
      }

      _context.Entry(serial).State = EntityState.Modified;

      var author = _context.Authors.FirstOrDefault(au => au.name == authorName);
      if (author != null)
      {
        serial.authorId = author.id;
      }
      else if(author == null)
      {
        author = await _context.Authors.FindAsync(serial.authorId);
        author.name = authorName;
      }

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


    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Serial>> PostSerial(Serial serial, string authorName)
    {
      if (_context.Serials == null)
      {
        return Problem("Entity set 'Context.Serials'  is null. korte");
      }

      if (_context.Serials.Where(ser => ser.title == serial.title).Any())
      {
        Console.WriteLine("Serial with same title already in DB");
        return NoContent();
      }
      var author = _context.Authors.FirstOrDefault(au => au.name == authorName);
      if (author != null)
      {
        serial.authorId = author.id;
      }
      else
      {
        author = new Author();
        author.name = authorName;
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        Console.WriteLine(author.id);
        serial.authorId = author.id;
      }
      _context.Serials.Add(serial);
      await _context.SaveChangesAsync();
      //serial.id is the new id here
      var (result, addedChs) = await _scraper.AddSerialChapters(serial);
      
      return Ok(new {serial.title, author.name, addedChs});
    }

    // DELETE: api/Serials/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> DeleteSerial(int id)
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
      var chapters = await _context.Chapters.Where((ch) => ch.serialId == id).ToListAsync();
      foreach (var chapter in chapters)
      {
        _context.Chapters.Remove(chapter);
      }
      Author author = await _context.Authors.FindAsync(serial.authorId);
      List<Serial> authorWorks = await _context.Serials.Where(ser => ser.authorId == author.id).ToListAsync();
      if(authorWorks.Count == 0)
      {
        _context.Authors.Remove(author);
      }
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool SerialExists(int id)
    {
      return (_context.Serials?.Any(e => e.id == id)).GetValueOrDefault();
    }
  }
}
