using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WT_API.Data;
using WT_API.Models;

namespace WT_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthorsController : ControllerBase
  {
    private readonly Context _context;

    public AuthorsController(Context context)
    {
      _context = context;
    }

    // GET: api/Authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
      if (_context.Authors == null)
      {
        return NotFound();
      }
      return await _context.Authors.ToListAsync();
    }

    //GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> GetAuthor(int id)
    {
      if (_context.Authors == null)
      {
        return NotFound();
      }
      var author = await _context.Authors.FindAsync(id);

      if (author == null)
      {
        return NotFound();
      }

      return author;
    }


    // PUT: api/Author/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, Author author)
    {
      if (id != author.id)
      {
        return BadRequest();
      }

      _context.Entry(author).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!AuthorExists(id))
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

    // POST: api/Authors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(Author author)
    {
      if (_context.Authors == null)
      {
        return Problem("Entity set 'Context.Authors'  is null.");
      }
      _context.Authors.Add(author);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetAuthor", new { id = author.id }, author);
    }

    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
      if (_context.Authors == null)
      {
        return NotFound();
      }
      var author = await _context.Authors.FindAsync(id);
      if (author == null)
      {
        return NotFound();
      }

      _context.Authors.Remove(author);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool AuthorExists(int id)
    {
      return (_context.Authors?.Any(e => e.id == id)).GetValueOrDefault();
    }
  }
}
