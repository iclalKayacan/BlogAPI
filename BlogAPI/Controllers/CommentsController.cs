using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize] // Yorum eklemek için kullanıcı doğrulama gereksinimi
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
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostComment), new { id = comment.Id }, comment);
        }

        // PUT: api/comments/{id}
        [Authorize] // Güncelleme işlemi için doğrulama
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto updateCommentDto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found.");
            }

            // Kullanıcının yetkisini kontrol et
            var currentUser = User.Identity?.Name;
            if (comment.Author != currentUser && !User.IsInRole("Admin"))
            {
                return Forbid("You are not allowed to update this comment.");
            }

            // Yorum güncelle
            comment.Content = updateCommentDto.Content;
            comment.Author = comment.Author; // Author değiştirilmemeli
            comment.CreatedAt = DateTime.UtcNow;

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound($"Comment with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/comments/{id}
        [Authorize] // Silme işlemi için doğrulama
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found.");
            }

            // Kullanıcının yetkisini kontrol et
            var currentUser = User.Identity?.Name;
            if (comment.Author != currentUser && !User.IsInRole("Admin"))
            {
                return Forbid("You are not allowed to delete this comment.");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
