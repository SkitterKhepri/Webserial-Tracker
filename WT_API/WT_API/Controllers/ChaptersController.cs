using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WT_API.Data;
using WT_API.Models;

namespace WT_API.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ChaptersController : ControllerBase
  {
    private readonly Context _context;

    public ChaptersController(Context context)
    {
      _context = context;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> PutChapter(int id, Chapter modChapter)
    {
      if (id != modChapter.id)
      {
        return BadRequest();
      }

      Chapter? chapter = await _context.Chapters.FindAsync(id);

      if(chapter == null)
      {
        return NotFound();
      }

      _context.Entry(chapter).CurrentValues.SetValues(modChapter);

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


    [HttpDelete("{id}")]
    [Authorize(Roles = "SAdmin, Admin")]
    public async Task<IActionResult> DeleteChapter(int id)
    {
      var chapter = await _context.Chapters.FindAsync(id);

      if (chapter == null)
      {
        return NotFound();
      }
      _context.Chapters.Remove(chapter);

      var newLastCh = await _context.Chapters.Where(ch => ch.serialId == chapter.serialId).OrderByDescending(ch => ch.id).FirstOrDefaultAsync();
      if (newLastCh != null)
      {
        newLastCh.isLastChapter = true;
      }

      await _context.SaveChangesAsync();
      return NoContent();
    }

  }
}
