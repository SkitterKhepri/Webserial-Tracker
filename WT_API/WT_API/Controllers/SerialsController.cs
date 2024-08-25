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

    //Same as above, but adds all the new chapters of all serials first, probably takes really fucking long TODO: only admin
    [HttpGet("/update")]
    public async Task<ActionResult<IEnumerable<Serial>>> UpdatedSerials()
    {
      if (_context.Serials == null)
      {
        return NotFound();
      }
      await UpdateSerials();
      return await _context.Serials.ToListAsync();
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
    [HttpGet("/update/{id}")]
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
      await UpdateSerial(id);

      return serial;
    }

    // PUT: api/Serials/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSerial(int id, Serial serial)
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
      await AddSerialChapters(serial);
      
      return CreatedAtAction("GetSerial", new { id = serial.id }, serial);
    }

    // DELETE: api/Serials/5
    [HttpDelete("{id}")]
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
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool SerialExists(int id)
    {
      return (_context.Serials?.Any(e => e.id == id)).GetValueOrDefault();
    }


    //Add chapters of a serial, using Html Agility Pack
    [NonAction]
    public async Task AddSerialChapters(Serial serial)
    {
      
      HttpClient client = new HttpClient();
      //had to add this bc of Ward -- docs
      Uri fullUri = new Uri(serial.firstCh);
      client.BaseAddress = new Uri(fullUri.AbsoluteUri);
      HttpResponseMessage response = await client.GetAsync(serial.firstCh);
      string pageContent = await CheckUrl(response, serial.firstCh);
      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);
      HtmlNode nextCH;
      string nextCHURL;
      string currentUrl = serial.firstCh;

      while (true)
      {
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        
        //debug
        //Console.WriteLine(nextCH.InnerText);

        HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
        //probably dont need this if?
        if (chTitleNode != null)
        {
          string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
          string chLink = currentUrl;
          //TODO maybe this doesnt need to happen every time, just first maybe?
          if(!(_context.Chapters.Where(ch => ch.title == chTitle).Any()))
          {
            _context.Chapters.Add(new Chapter(serial.id, chTitle, chLink));
          }
        }
        if (nextCH == null)
        {
          nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
          if (nextCH == null)
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.otherNextChLinkXPaths);
            if (nextCH == null)
            {
              //TODO notify admin shits wrong -- but maybe nothings wrong????
              Console.WriteLine("none of the next chapter links work, maybe its over");
              _context.SaveChanges();
              break;
            }
          }
        }
        else
        {
          nextCHURL = nextCH.Attributes["href"].Value;
          Console.WriteLine(nextCHURL);
          response = await client.GetAsync(nextCHURL);
          string tbCheckedUrl = nextCHURL;
          if (nextCHURL[0] == '/')
          {
            tbCheckedUrl = client.BaseAddress + nextCHURL;
          }
          pageContent = await CheckUrl(response, tbCheckedUrl);
          currentUrl = tbCheckedUrl;
          htmlDoc.LoadHtml(pageContent);
        }
      }
    }



    private static async Task<string> CheckUrl(HttpResponseMessage response, string link) {

      string pageContent = "";
      HttpClient client2 = new HttpClient();
      Uri fullUri = new Uri(link);
      
      client2.BaseAddress = new Uri(fullUri.AbsoluteUri);

      if (response.IsSuccessStatusCode)
      {
        pageContent = await response.Content.ReadAsStringAsync();
      }
      //this one is for redirects, not a solution for every possible redirect <-TODO, but works for TGAB -- docs
      else if (response.StatusCode == HttpStatusCode.MovedPermanently)
      {
        Uri? redirectLink = response.Headers.Location;
        if (redirectLink != null)
        {
          HttpResponseMessage newResponse = await client2.GetAsync(redirectLink);
          newResponse.EnsureSuccessStatusCode();
          pageContent = await newResponse.Content.ReadAsStringAsync();
        }
      }
      //this one is handling 429 "too many requests" rate limiting issues, probably imperfectly, for Katalepsis -- docs
      else if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
          while (response.StatusCode == HttpStatusCode.TooManyRequests)
          {
            Thread.Sleep(50);
          }
          response.EnsureSuccessStatusCode();
          pageContent = await response.Content.ReadAsStringAsync();
        }

      client2.Dispose();
      return pageContent;
    }

    //Update all serials
    private async Task UpdateSerials()
    {
      List<Serial> serials = await _context.Serials.ToListAsync();
      foreach (Serial serial in serials)
      {
        Chapter? lastChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
        string lastLink;
        if (lastChapter == null)
        {
          lastLink = serial.firstCh;
        }
        else
        {
          lastLink = lastChapter.link;
        }
        serial.firstCh = lastLink;
        await AddSerialChapters(serial);
      }
    }

    //Update one specififc serial
    private async Task UpdateSerial(int id)
    {
      Serial serial = await _context.Serials.FindAsync(id);

      Chapter? lastChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
      string lastLink;
      if (lastChapter == null)
      {
        lastLink = serial.firstCh;
      }
      else
      {
        lastLink = lastChapter.link;
      }
      serial.firstCh = lastLink;
      await AddSerialChapters(serial);
    }
  }
}
