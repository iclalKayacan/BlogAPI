using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public CommentsController(BlogDbContext context)
        {
            _context = context;
        }

        // POST: api/comments
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CreateCommentDto createCommentDto)
        {
            // Blog'un var olup olmadığını kontrol et
            var blog = await _context.Blogs.FindAsync(createCommentDto.BlogId);
            if (blog == null)
            {
                return NotFound($"Blog with ID {createCommentDto.BlogId} not found.");
            }

            // Yeni yorum oluştur
            var comment = new Comment
            {
                Content = createCommentDto.Content,
                Author = createCommentDto.Author,
                BlogId = createCommentDto.BlogId,
                Blog = blog, 
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostComment), new { id = comment.Id }, comment);
        }

    }
}
