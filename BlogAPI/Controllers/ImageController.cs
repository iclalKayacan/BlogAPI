using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya yüklenmedi");

            // Dosya uzantısını kontrol et
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Geçersiz dosya formatı");

            try
            {
                // Benzersiz dosya adı oluştur
                var fileName = $"{Guid.NewGuid()}{extension}";

                // Uploads klasörünün yolunu al
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Dosyanın tam yolu
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // URL'i döndür
                var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dosya yüklenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
