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
                bool isCreated = user.Roles.Any(r => r.Id == newUserRoleDTO.RoleId);
                if (isCreated) return Conflict(new { message = "Ilyen szerepköre már van a Usernek" });
                user.Roles.Add(role);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres összerendelés!" });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteUserRoleDTO deleteUserRoleDTO)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == deleteUserRoleDTO.UserId);
                var role = await _context.Roles.FindAsync(deleteUserRoleDTO.RoleId);
                if (user is null || role is null) return NotFound(new { message = "Hibás UserId vagy RoleId!" });
                bool isCreated = user.Roles.Any(r => r.Id == deleteUserRoleDTO.RoleId);
                if (!isCreated) return Conflict(new { message = "Ilyen szerepkörrel már rendelkezik a User!" });
                user.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés!" });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("allUsersAllRoles")]
        public async Task<IActionResult> AllUsersAllRoles()
        {
            try
            {
                var users = await _context.Users.Include(u => u.Roles).Select(u => new { Users = u.Name, UserRoles = u.Roles }).ToListAsync();
                return Ok(users);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("userByIdAndRoles")]
        public async Task<IActionResult> UserByIdAndRoles([FromQuery] Guid id)
        {
            try
            {
                var user = await _context.Users.Select(u => new { u.Id, u.Name, UserRoles = u.Roles.Select(r => r.Name).ToList() }).SingleOrDefaultAsync(u => u.Id == id);
                return Ok(user);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
