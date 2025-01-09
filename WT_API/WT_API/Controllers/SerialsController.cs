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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;

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
      if (result == 2)
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
    public async Task<IActionResult> PutSerial(int id, [FromForm] CompleteSerial compSerial)
    {
      if (id != compSerial.id)
      {
        return BadRequest();
      }

      Serial serial = await _context.Serials.FindAsync(id);

      if (serial == null)
      {
        return NotFound();
      }

      var author = _context.Authors.FirstOrDefault(au => au.name == compSerial.authorName);
      if (author != null)
      {
        compSerial.authorId = author.id;
      }
      else
      {
        author = new Author();
        author.name = compSerial.authorName;
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        compSerial.authorId = author.id;
      }

      //banner image saving
      if (compSerial.bannerUpload != null)
      {
        IFormFile banner = compSerial.bannerUpload;
        string path = @"..\assets\img";

        Directory.CreateDirectory(path);

        string oldBanner = Path.Combine(path, NormaliseSerialTitle(serial.title), ".png");
        if (System.IO.File.Exists(oldBanner))
        {
          System.IO.File.Delete(oldBanner);
        }

        string newImgPath = Path.Combine(path, NormaliseSerialTitle(compSerial.title) + ".png");

        using (Stream fileStream = banner.OpenReadStream())
        {
          using (Image image = Image.Load(fileStream))
          {
            image.Save(newImgPath, new PngEncoder());
          }
        }

        serial.bannerPath = newImgPath;
      }

      serial.CompleteSerialMapper(compSerial);

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
    public async Task<ActionResult<Serial>> AddSerial([FromForm] CompleteSerial newSerial)
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

      Serial serial = new Serial();
      serial.CompleteSerialMapper(newSerial);

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
      if (newSerial.bannerUpload != null)
      {
        IFormFile banner = newSerial.bannerUpload;
        string path = @"..\assets\img";
        Directory.CreateDirectory(path);

        string oldBanner = Path.Combine(path, NormaliseSerialTitle(serial.title), ".png");
        if (System.IO.File.Exists(oldBanner))
        {
          System.IO.File.Delete(oldBanner);
        }

        string newImgPath = Path.Combine(path, NormaliseSerialTitle(newSerial.title) + ".png");

        using (Stream fileStream = banner.OpenReadStream())
        {
          using (Image image = Image.Load(fileStream))
          {
            image.Save(newImgPath, new PngEncoder());
          }
        }

        serial.bannerPath = newImgPath;
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
        return StatusCode(StatusCodes.Status417ExpectationFailed, "Serial already added/proposed");
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
      if (newSerial.bannerUpload != null)
      {
        IFormFile banner = newSerial.bannerUpload;
        string path = @"..\assets\img";

        Directory.CreateDirectory(path);

        string oldBanner = Path.Combine(path, NormaliseSerialTitle(serial.title), ".png");
        if (System.IO.File.Exists(oldBanner))
        {
          System.IO.File.Delete(oldBanner);
        }

        string newImgPath = Path.Combine(path, NormaliseSerialTitle(newSerial.title) + ".png");

        using (Stream fileStream = banner.OpenReadStream())
        {
          using (Image image = Image.Load(fileStream))
          {
            image.Save(newImgPath, new PngEncoder());
          }
        }

        serial.bannerPath = newImgPath;
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

      //author delete if they have no more serials

      Author author = await _context.Authors.FindAsync(serial.authorId);
      List<Serial> authorWorks = await _context.Serials.Where(ser => ser.authorId == author.id).ToListAsync();
      if (authorWorks.Count == 1)
      {
        _context.Authors.Remove(author);
      }
      await _context.SaveChangesAsync();

      //image delete
      string path = @"..\assets\img";
      string oldBanner = Path.Combine(path, NormaliseSerialTitle(serial.title) + ".png");
      if (System.IO.File.Exists(oldBanner))
      {
        System.IO.File.Delete(oldBanner);
      }

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

    [HttpPost("/Serials/like")]
    //[Authorize] //TODO need
    public async Task<IActionResult> LikeUnLike(string userId, int serId)
    {
      if(!SerialExists(serId) || !UserExists(userId))
      {
        return NotFound("Serial or user not found, dumbass");
      }

      LikedSerial? like = await _context.LikedSerials.FindAsync(userId, serId);

      if (like == null) {
        like = new LikedSerial();
        like.serialId = serId;
        like.userId = userId;
        _context.LikedSerials.Add(like);
      }
      else
      {
        _context.LikedSerials.Remove(like);
      }

      _context.SaveChanges();

      return StatusCode(StatusCodes.Status202Accepted);
    }

    //Get's serialTitle serial's cover image
    [HttpGet("images/{serialTitle}")]
    public IActionResult GetImage(string serialTitle)
    {
      string normalSerTit = NormaliseSerialTitle(serialTitle);
      string filePath = Path.Combine("..", "assets", "img", $"{normalSerTit}.png");

      FileInfo fileInfo = new FileInfo(filePath);

      if (!fileInfo.Exists)
      {
        return NotFound();
      }

      string eTag = fileInfo.LastWriteTimeUtc.Ticks.ToString();

      if (Request.Headers["If-None-Match"] == eTag)
      {
        return StatusCode(StatusCodes.Status304NotModified);
      }

      FileStream image = fileInfo.OpenRead();
      string mimeType = "image/" + fileInfo.Extension.TrimStart('.').ToLower();

      Response.Headers["ETag"] = eTag;
      Response.Headers["Cache-Control"] = "public, max-age=1800";
      Response.Headers["Response-Type"] = "blob";

      return File(image, mimeType);
    }

    //TEST TODO delete
    [HttpPost("images/")]
    public IActionResult SaveImage([FromForm] ProposedSerial serial)
    {
      if (serial.bannerUpload != null)
      {
        IFormFile banner = serial.bannerUpload;
        string path = @"..\img";
        string imgPath = Path.Combine(path, NormaliseSerialTitle(serial.title) + ".png");

        Directory.CreateDirectory(path);

        using (Stream fileStream = banner.OpenReadStream())
        {
          using (Image image = Image.Load(fileStream))
          {
            image.Save(imgPath, new PngEncoder());
          }
        }
        return Ok();
      }
      return StatusCode(405);

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

    private bool UserExists(string id)
    {
      return (_context.Users?.Any(u => u.Id == id)).GetValueOrDefault();
    }

    private string NormaliseSerialTitle(string title)
    {
      return title.Trim().Replace(' ', '_').Replace(':', '_').Replace('-', '_').ToLower();
    }

  }
}
