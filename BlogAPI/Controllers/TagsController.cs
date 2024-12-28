using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using BlogAPI.Data; // Veritabanı context'inizi ekleyin
using Microsoft.EntityFrameworkCore;
using System;
using BlogAPI.DTOs;

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
            var tags = await _context.Tags
                .Select(tag => new TagDTO
                {
                    Id = tag.Id,
                    Name = tag.Name
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagDTO TagDto)
        {
            if (string.IsNullOrEmpty(TagDto.Name))
            {
                return BadRequest("Tag name is required.");
            }

            // Yeni bir Tag nesnesi oluşturun ve veritabanına ekleyin
            var tag = new Tag
            {
                Name = TagDto.Name
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            // Sadece eklenen Tag'in Id ve Name bilgilerini döndürün
            var responseDto = new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, responseDto);
        }


        [HttpPost("{blogId}/addTag")]
        public async Task<IActionResult> AddTagToBlog(int blogId, int tagId)
        {
            var blog = await _context.Blogs
                .Include(b => b.Tags) // İlişkili tagleri yükle
                .FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null)
            {
                return NotFound("Blog bulunamadı.");
            }

            var tag = await _context.Tags.FindAsync(tagId);
            if (tag == null)
            {
                return NotFound("Tag bulunamadı.");
            }

            if (!blog.Tags.Contains(tag))
            {
                blog.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Tag başarıyla blog'a eklendi." });
        }
    }
}
