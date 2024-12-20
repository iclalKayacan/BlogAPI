using Microsoft.AspNetCore.Mvc;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public CategoryController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [AllowAnonymous] // Anonim kullanıcılar kategorileri listeleyebilir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Category/{id}
        [AllowAnonymous] // Anonim kullanıcılar tek bir kategoriye erişebilir
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Category
        [Authorize(Roles = "Admin")] // Yalnızca Admin kullanıcılar kategori oluşturabilir
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/Category/{id}
        [Authorize(Roles = "Admin")] // Yalnızca Admin kullanıcılar kategori güncelleyebilir
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // DELETE: api/Category/{id}
        [Authorize(Roles = "Admin")] // Yalnızca Admin kullanıcılar kategori silebilir
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
