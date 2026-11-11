using AuthUserApi.Models;
using AuthUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserRoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] NewUserRoleDTO newUserRoleDTO)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Id == newUserRoleDTO.UserId);
                var role = await _context.Roles.FindAsync(newUserRoleDTO.RoleId);

                if (user is null || role is null) return NotFound(new { message = "Hibás UserId vagy RoleId!" });
                user.Roles.Add(role);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres összerendelés!" });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
