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

    //Same as above, but adds all the new chapters of all serials first
    [HttpGet("/Serials/update")]
    public async Task<ActionResult<IEnumerable<Serial>>> UpdateSerials()
    {
      if (_context.Serials == null)
      {
        return NotFound();
      }
      var (result, updatedChs) = await _scraper.UpdateSerials();
      return Ok(updatedChs);
    }


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
    public async Task<ActionResult<Serial>> UpdateSerial(int id)
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
      else if (result == 0)
      {
        return Problem("Error adding first chapter");
      }
      else return Ok(updatedChs);
    }


    [HttpPut("{id}")]
    //[Authorize(Roles = "SAdmin,Admin")] //TODO need
    public async Task<IActionResult> PutSerial(int id, SerialDTO serialDTO)
    {
      if (id != serialDTO.id)
      {
        return BadRequest();
      }

      Serial serial = await _context.Serials.FindAsync(id);

      if (serial == null)
      {
        return NotFound();
      }

      var author = _context.Authors.FirstOrDefault(au => au.name == serialDTO.author.name);
      if (author != null)
      {
        serialDTO.authorId = author.id;
      }
      else
      {
        author = new Author();
        author.name = serialDTO.author.name;
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        serialDTO.authorId = author.id;
      }

      //banner image saving
      if (serialDTO.banerUpload != null)
      {
        IFormFile banner = serialDTO.banerUpload;
        string path = @"..\img\";
        if (System.IO.File.Exists(path + serial.title + ".png"))
        {
          System.IO.File.Delete(path + serial.title + ".png");
          serial.bannerPath = path + serialDTO.title + ".png";
        }
        using (FileStream fileStream = new FileStream(path + serialDTO.title + ".png", FileMode.Create))
        {
          banner.CopyTo(fileStream);
        }
        serial.bannerPath = path + serial.title + ".png";
      }

      serial.DTOMapper(serialDTO);

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

    [HttpPost("/Serials/addSerial")]
    //[Authorize(Roles = "SAdmin,Admin")] //TODO need
    public async Task<ActionResult<Serial>> AddSerial([FromForm] AddedSerial newSerial)
    {
      if (_context.Serials == null)
      {
        return Problem("Entity set 'Context.Serials'  is null. korte");
      }
      if (_context.Serials.Where(ser => ser.title == newSerial.title).Any())
      {
        Console.WriteLine("Serial with same title already in DB");
        return StatusCode(StatusCodes.Status417ExpectationFailed, "Serial already added");
      }

      Serial serial = new Serial(newSerial);

      Author? author = _context.Authors.FirstOrDefault(au => au.name == newSerial.authorName);
      if (author != null)
      {
        serial.authorId = author.id;
      }
      else
      {
        author = new Author();
        author.name = newSerial.authorName;
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        serial.authorId = author.id;
      }

      //banner image saving
      if (newSerial.banerUpload != null)
      {
        IFormFile? banner = newSerial.banerUpload;
        string path = $@"..\img";
        using (FileStream fileStream = new FileStream(path + newSerial.title + ".png", FileMode.Create))
        {
          banner.CopyTo(fileStream);
        }
        serial.bannerPath = path + serial.title + ".png";
      }

      _context.Serials.Add(serial);
      await _context.SaveChangesAsync();

      //Adding chapters
      var (result, addedChs) = (0, 0);      
      (result, addedChs) = await _scraper.AddSerialChapters(serial);
      

      return Ok(new { serial.title, addedChs });
    }


    [HttpPost("/Serials/proposeSerial")]
    //[Authorize] //TODO need
    public async Task<ActionResult<Serial>> ProposeSerial([FromForm] ProposedSerial newSerial)
    {
      if (_context.Serials == null)
      {
        return Problem("Entity set 'Context.Serials'  is null. korte");
      }
      if (_context.Serials.Where(ser => ser.title == newSerial.title).Any())
      {
        Console.WriteLine("Serial with same title already in DB");
        return StatusCode(StatusCodes.Status417ExpectationFailed, "Serial already added");
      }

      Serial serial = new Serial(newSerial);

      Author? author = _context.Authors.FirstOrDefault(au => au.name == newSerial.authorName);
      if (author != null)
      {
        serial.authorId = author.id;
      }
      else
      {
        author = new Author();
        author.name = newSerial.authorName;
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        serial.authorId = author.id;
      }

      //banner image saving
      if (newSerial.banerUpload != null)
      {
        IFormFile? banner = newSerial.banerUpload;
        string path = $@"..\img";
        using (FileStream fileStream = new FileStream(path + newSerial.title + ".png", FileMode.Create))
        {
          banner.CopyTo(fileStream);
        }
        serial.bannerPath = path + serial.title + ".png";
      }

      _context.Serials.Add(serial);
      await _context.SaveChangesAsync();
      
      return Ok();
    }

    // DELETE: api/Serials/5
    [HttpDelete("{id}")]
    //[Authorize(Roles = "SAdmin,Admin")] //TODO need
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
    //[Authorize(Roles = "SAdmin, Admin")] //TODO need
    public async Task<IActionResult> Approve(int id, Review review)
    {
      Serial serial = await _context.Serials.FindAsync(id);

      if (serial == null)
        return NotFound();

      serial.reviewStatus = review.reviewStatus;

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
