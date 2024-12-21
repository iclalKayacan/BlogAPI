using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BlogDbContext _context; // DbContext'i dahil ettik

        public AuthController(IConfiguration configuration, BlogDbContext context)
        {
            _configuration = configuration;
            _context = context; // DbContext'i enjekte ettik
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Kullanıcı doğrulama
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.Password == loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı!");
            }

            // JWT oluşturma
            var token = GenerateJwtToken(user);

            // Kullanıcı bilgileri ve token'ı dön
            return Ok(new
            {
                Token = token,
                User = new
                {
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role
                }
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Kullanıcı adı kontrolü
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Kullanıcı adı zaten mevcut!");
            }

            // Yeni kullanıcı oluşturma
            var newUser = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password, // Şifreleme eklenebilir (ör. BCrypt)
                FullName = registerDto.FullName,
                Role = "User" // Varsayılan olarak "User" rolü atanıyor
            };

            // Kullanıcıyı veritabanına ekleme
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Kullanıcı başarıyla oluşturuldu!");
        }

        // JWT Token Oluşturma Metodu
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
