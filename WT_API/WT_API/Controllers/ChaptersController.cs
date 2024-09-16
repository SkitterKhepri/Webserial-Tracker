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
