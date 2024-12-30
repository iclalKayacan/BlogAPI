using BlogAPI.Data;
using BlogAPI.Models;
using BlogAPI.DTOs; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public BlogsController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: api/Blogs
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            try
            {
                var blogs = await _context.Blogs
                    .Include(b => b.Categories)  
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET: api/Blogs/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            try
            {
                var blog = await _context.Blogs
                    .Include(b => b.Tags) 
                    .Include(b => b.Categories)
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (blog == null)
                {
                    return NotFound();
                }

                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // POST: api/Blogs
        //[Authorize(Roles = "Admin")]
        // POST: api/Blogs
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog([FromBody] CreateBlogDto createBlogDto)
        {
            try
            {
                var blog = new Blog
                {
                    Title = createBlogDto.Title,
                    Content = createBlogDto.Content,
                    Author = createBlogDto.Author,
                    Summary = createBlogDto.Summary
                              ?? createBlogDto.Content.Substring(0, Math.Min(200, createBlogDto.Content.Length)),
                    ImageUrl = createBlogDto.ImageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                // Kategorileri ekle
                if (createBlogDto.CategoryIds != null && createBlogDto.CategoryIds.Any())
                {
                    var categories = await _context.Categories
                        .Where(c => createBlogDto.CategoryIds.Contains(c.Id))
                        .ToListAsync();
                    blog.Categories = categories;
                }

                // **Etiketleri ekle (Eksik olan kısım)**
                if (createBlogDto.TagIds != null && createBlogDto.TagIds.Any())
                {
                    var tags = await _context.Tags
                        .Where(t => createBlogDto.TagIds.Contains(t.Id))
                        .ToListAsync();
                    blog.Tags = tags;
                }

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                // Blog'u kategorileri ve etiketleriyle birlikte getir
                var createdBlog = await _context.Blogs
                    .Include(b => b.Categories)
                    .Include(b => b.Tags)      // Eklemek istediğiniz etiketleri de dahil edebilirsiniz
                    .FirstOrDefaultAsync(b => b.Id == blog.Id);

                return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, createdBlog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




        // PUT: api/Blogs/5
        //[Authorize(Roles = "Author")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog(int id, CreateBlogDto updateBlogDto)
        {
            var blog = await _context.Blogs
                .Include(b => b.Tags)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = updateBlogDto.Title;
            blog.Content = updateBlogDto.Content;
            blog.Author = updateBlogDto.Author;
            blog.Summary = updateBlogDto.Summary;
            blog.ImageUrl = updateBlogDto.ImageUrl;

            blog.UpdatedAt = DateTime.UtcNow;

            if (updateBlogDto.CategoryIds != null)
            {
                blog.Categories = await _context.Categories
                    .Where(c => updateBlogDto.CategoryIds.Contains(c.Id))
                    .ToListAsync();
            }

            if (updateBlogDto.TagIds != null)
            {
                blog.Tags = await _context.Tags
                    .Where(t => updateBlogDto.TagIds.Contains(t.Id))
                    .ToListAsync();
            }

            _context.Entry(blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
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


        // DELETE: api/Blogs/5
        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Tags)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
