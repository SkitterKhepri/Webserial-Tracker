using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WT_API.Data;
using WT_API.Models;

namespace WT_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ChaptersController : ControllerBase
  {
    private readonly Context _context;

    public ChaptersController(Context context)
    {
      _context = context;
    }

    // GET: api/Chapters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chapter>>> GetChapters()
    {
      if (_context.Chapters == null)
      {
        return NotFound();
      }
      return await _context.Chapters.ToListAsync();
    }

    //GET: api/Chapters/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Chapter>> GetChapter(int id)
    {
      if (_context.Chapters == null)
      {
        return NotFound();
      }
      var chapter = await _context.Chapters.FindAsync(id);

      if (chapter == null)
      {
        return NotFound();
      }

      return chapter;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> PutChapter(int id, Chapter chapter)
    {
      if (id != chapter.id)
      {
        return BadRequest();
      }

      _context.Entry(chapter).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ChapterExists(id))
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

    // POST: api/Chapters
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<ActionResult<Chapter>> PostChapter(Chapter chapter)
    {
      if (_context.Chapters == null)
      {
        return Problem("Entity set 'Context.Chapters'  is null.");
      }
      _context.Chapters.Add(chapter);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetChapter", new { id = chapter.id }, chapter);
    }

    // DELETE: api/Chapters/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> DeleteChapter(int id)
    {
      if (_context.Chapters == null)
      {
        return NotFound();
      }
      var chapter = await _context.Chapters.FindAsync(id);
      if (chapter == null)
      {
        return NotFound();
      }

      _context.Chapters.Remove(chapter);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    [HttpPatch("{id}/approve")]
    [Authorize(Roles = "SAdmin, Admin")]
    public async Task<IActionResult> Approve(int id, Review review)
    {
      var chapter = await _context.Chapters.FindAsync(id);

      if (chapter == null)
        return NotFound();

      chapter.reviewStatus = review.reviewStatus;

      await _context.SaveChangesAsync();

      return StatusCode(StatusCodes.Status202Accepted);
    }

    private bool ChapterExists(int id)
    {
      return (_context.Chapters?.Any(e => e.id == id)).GetValueOrDefault();
    }
  }
}
