using Microsoft.AspNetCore.Mvc;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BlogAPI.DTOs;

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    color = c.Color
                })
                .ToListAsync();

            return Ok(categories);
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
        [HttpGet("{id}/blogs")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogsByCategory(int id)
        {
            var blogs = await _context.Blogs
                .Include(b => b.Categories) // Blog ile ilişkili kategorileri dahil et
                .Where(b => b.Categories.Any(c => c.Id == id)) // Kategoriye göre filtrele
                .ToListAsync();

            if (!blogs.Any())
            {
                return NotFound("Bu kategoriye ait blog bulunamadı.");
            }

            return Ok(blogs);
        }




        // POST: api/Category
        //[Authorize(Roles = "Admin")] // Yalnızca Admin kullanıcılar kategori oluşturabilir
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/Category/{id}
        //[Authorize(Roles = "Admin")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryDto.Name;
            category.Color = categoryDto.Color;

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
         [Authorize(Roles = "Admin")] 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Blogs) // İlişkili blogları getir
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound("Kategori bulunamadı.");
                }

                // Önce blog-kategori ilişkilerini kaldır
                foreach (var blog in category.Blogs.ToList())
                {
                    blog.Categories.Remove(category);
                }

                // Sonra kategoriyi sil
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Kategori başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Kategori silinirken bir hata oluştu: " + ex.Message });
            }
        }


        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
