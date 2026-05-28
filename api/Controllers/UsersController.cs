using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccidentDetectionSysrem.modelsef;

namespace AccidentDetectionSysrem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect _context;

        public UsersController(AccidentDetectionSystemContect context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersList = await _context.Users
                .Select(u => new
                {
                    id = u.UserId,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    role = u.Role,
                    isActive = u.IsActive,
                    createdAt = u.CreatedAt,
                    lastLoginTime = u.LastLoginTime
                })
                .ToListAsync();

            return Ok(new { users = usersList });
        }

        // GET: api/Users/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var usersList = await _context.Users.ToListAsync();
            var adminCount = usersList.Count(u => u.Role == "Admin");
            var officerCount = usersList.Count(u => u.Role == "Officer");
            var userCount = usersList.Count(u => u.Role == "User");
            var totalCount = usersList.Count;

            return Ok(new
            {
                stats = new
                {
                    admin = adminCount,
                    officer = officerCount,
                    user = userCount,
                    total = totalCount
                }
            });
        }

        // PUT: api/Users/deactivate
        [HttpPut("deactivate")]
        public async Task<IActionResult> DeactivateUser([FromBody] UserActionRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deactivated successfully" });
        }

        // PUT: api/Users/activate
        [HttpPut("activate")]
        public async Task<IActionResult> ActivateUser([FromBody] UserActionRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User activated successfully" });
        }
    }

    public class UserActionRequest
    {
        public int UserId { get; set; }
    }
}
