using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogAPI.Controllers
{

    namespace BlogAPI.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            // Örnek kullanıcı verileri (gerçek projede veritabanından alınır)
            private static List<User> Users = new List<User>
        {
            new User { Id = 1, Username = "admin", Password = "password", Role = "Admin" },
            new User { Id = 2, Username = "author", Password = "password", Role = "Author" },
            new User { Id = 3, Username = "user", Password = "password", Role = "User" }
        };

            public AuthController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginDto loginDto)
            {
                // Kullanıcı doğrulama
                var user = Users.FirstOrDefault(u => u.Username == loginDto.Username && u.Password == loginDto.Password);
                if (user == null)
                {
                    return Unauthorized("Kullanıcı adı veya şifre hatalı!");
                }

                // JWT oluşturma
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
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

}
