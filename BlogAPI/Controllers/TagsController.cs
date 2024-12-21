using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using BlogAPI.Data; // Veritabanı context'inizi ekleyin
using Microsoft.EntityFrameworkCore;
using System;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public TagsController(BlogDbContext context)
        {
            _context = context;
        }

        // Tüm tagleri listeleme
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        // Blog'a tag ekleme
        [HttpPost("{blogId}/add-tags")]
        public async Task<IActionResult> AddTagsToBlog(int blogId, [FromBody] List<int> tagIds)
        {
            var blog = await _context.Blogs.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null) return NotFound("Blog bulunamadı!");

            var tags = await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();

            foreach (var tag in tags)
            {
                if (!blog.Tags.Any(t => t.Id == tag.Id))
                {
                    blog.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(blog);
        }
    }
}
