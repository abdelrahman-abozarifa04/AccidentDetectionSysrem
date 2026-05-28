using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AccidentDetectionSysrem.DTO;
using AccidentDetectionSysrem.modelsef;

namespace AccidentDetectionSysrem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect _context;
        private readonly IConfiguration _configuration;

        public AuthController(AccidentDetectionSystemContect context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var allowedRoles = new[] { "User", "Admin", "Officer" };
            if (!allowedRoles.Contains(dto.Role))
                return BadRequest("Invalid role.");

            var user = new user
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new { token, message = "Account created successfully" });
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            var hashedPassword = HashPassword(dto.Password);

            if (user.PasswordHash != hashedPassword)
                return Unauthorized("Invalid email or password.");

            user.LastLoginTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // ✅ Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true });
        }

        // 🔐 Hash Password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // 🔑 Generate JWT
        private string GenerateJwtToken(user user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}