using BlogAPI.Data;
using BlogAPI.Models;
using BlogAPI.DTOs; // DTO'lar eklendi
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
            return await _context.Blogs
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .Include(b => b.Categories)
                .ToListAsync();
        }

        // GET: api/Blogs/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }

        // POST: api/Blogs
        [Authorize(Roles = "Author")]
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog(CreateBlogDto createBlogDto)
        {
            var blog = new Blog
            {
                Title = createBlogDto.Title,
                Content = createBlogDto.Content,
                Author = createBlogDto.Author,
                Summary = createBlogDto.Summary,
                ImageUrl = createBlogDto.ImageUrl, 
                CreatedAt = DateTime.UtcNow
            };

            if (createBlogDto.CategoryIds != null)
            {
                blog.Categories = await _context.Categories
                    .Where(c => createBlogDto.CategoryIds.Contains(c.Id))
                    .ToListAsync();
            }

            if (createBlogDto.TagIds != null)
            {
                blog.Tags = await _context.Tags
                    .Where(t => createBlogDto.TagIds.Contains(t.Id))
                    .ToListAsync();
            }

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlog", new { id = blog.Id }, blog);
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
        [Authorize(Roles = "Admin")]
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
