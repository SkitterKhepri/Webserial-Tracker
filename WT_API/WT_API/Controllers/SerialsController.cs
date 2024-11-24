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
using WT_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace WT_API.Controllers
{
  [Route("[controller]")]
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

    // GET: Serials
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SerialDTO>>> GetSerials()
    {
      if (_context.Serials == null)
      {
        return NotFound();
      }
      var serialList = await _context.Serials.ToListAsync();
      List<SerialDTO> serialDTOs = new List<SerialDTO>();
      foreach (Serial serial in serialList)
      {
        SerialDTO serialDTO = new SerialDTO(serial);
        Author author = await _context.Authors.FindAsync(serialDTO.authorId);
        serialDTO.author = author;
        List<Chapter> chapters = await _context.Chapters.Where((ch) => ch.serialId == serialDTO.id).ToListAsync();
        serialDTO.chapters = chapters;
        serialDTOs.Add(serialDTO);
      }
      return Ok(serialDTOs);
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
    public async Task<ActionResult<SerialDTO>> GetSerial(int id)
    {
      if (_context.Serials == null)
      {
        return NotFound();
      }

      Serial serial = await _context.Serials.FindAsync(id);

      SerialDTO serialDTO = new SerialDTO(serial);
      
      serialDTO.chapters = await _context.Chapters.Where((ch) => ch.serialId == serialDTO.id).ToListAsync();
      serialDTO.author = await _context.Authors.FindAsync(serialDTO.authorId);

      if (serialDTO == null)
      {
        return NotFound();
      }

      return serialDTO;
    }

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
      if(result == 2)
      {
        return StatusCode(StatusCodes.Status406NotAcceptable, "Serial awaiting approval");
      }

      return Ok(updatedChs);
    }

    // PUT: api/Serials/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    //[Authorize(Roles = "SAdmin,Admin")]
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
    //[Authorize]
    public async Task<ActionResult<Serial>> PostSerial([FromForm] SerialDTO serial)
    {
      if (_context.Serials == null)
      {
        return Problem("Entity set 'Context.Serials'  is null. korte");
      }
      if (_context.Serials.Where(ser => ser.title == serial.title).Any())
      {
        Console.WriteLine("Serial with same title already in DB");
        return StatusCode(StatusCodes.Status417ExpectationFailed, "Serial already added");
      }
      if (serial.author.name != "")
      {
        var author = _context.Authors.FirstOrDefault(au => au.name == serial.author.name);
        if (author != null)
        {
          serial.authorId = author.id;
        }
        else
        {
          author = new Author();
          author.name = serial.author.name;
          _context.Authors.Add(author);
          await _context.SaveChangesAsync();
          serial.authorId = author.id;
        }
      }

      //banner image saving
      IFormFile? banner = serial.banerUpload;
      string path = $@"..\img\{serial.title}";
      FileStream fileStream = System.IO.File.Create(path);
      fileStream.Dispose();
      serial.bannerPath = path;

      _context.Serials.Add(serial);
      await _context.SaveChangesAsync();

      var (result, addedChs) = (0, 0);
      if (serial.reviewStatus)
      {
         (result, addedChs) = await _scraper.AddSerialChapters(serial);
      }
      
      return Ok(new {serial.title, addedChs});
    }

    // DELETE: api/Serials/5
    [HttpDelete("{id}")]
    //[Authorize(Roles = "SAdmin,Admin")]
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
      if(authorWorks.Count == 1)
      {
        _context.Authors.Remove(author);
      }
      await _context.SaveChangesAsync();

      return NoContent();
    }


    [HttpPatch("{id}/approve")]
    //[Authorize(Roles = "SAdmin, Admin")]
    public async Task<IActionResult> Approve(int id, Review review)
    {
      Serial serial = await _context.Serials.FindAsync(id);

      if (serial == null)
        return NotFound();

      _context.SaveChanges();

      return StatusCode(StatusCodes.Status202Accepted);
    }

    [HttpGet("images/{fileName}")]
    public IActionResult GetImage(string fileName)
    {
      string filePath = Path.Combine("..", "img", fileName);

      if (!System.IO.File.Exists(filePath))
      {
        return NotFound();
      }

      var image = System.IO.File.OpenRead(filePath);
      return File(image, GetMimeType(image.Name));
    }


    private string GetMimeType(string fileName)
    {
      string extension = Path.GetExtension(fileName).ToLowerInvariant();
      return extension switch
      {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".webp" => "image/webp"
      };
    }

    private bool SerialExists(int id)
    {
      return (_context.Serials?.Any(e => e.id == id)).GetValueOrDefault();
    }
  }
}
