using AuthUserApi.Models;
using AuthUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZstdSharp.Unsafe;

namespace AuthUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public RolesController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpGet("allRole")]
        public async Task<IActionResult> AllRole()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                if (roles is not null)
                {
                    return StatusCode(200, new { message = "Sikeres lekérdezés!", result = roles });
                }
                return StatusCode(400, new { message = "Sikertelen lekérdezés!", result = roles });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("byId")]
        public async Task<IActionResult> RoleById([FromQuery] Guid id)
        {
            try
            {
                var role = await _context.Roles.SingleOrDefaultAsync(u => u.Id == id);

                if (role is not null)
                {
                    return StatusCode(200, new { message = "Sikeres lekérdezés!", result = role });
                }
                return StatusCode(404, new { message = "Sikertelen lekérdezés! Nincs találat.", result = role });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("newRole")]
        public async Task<IActionResult> NewRole([FromBody] NewRoleDTO newRoleDTO)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = newRoleDTO.Name,
                    RegTime = DateTime.Now
                };

                if (role is not null)
                {
                    await _context.Roles.AddAsync(role);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, new { message = "Sikeres felvétel!", result = role });
                }
                return StatusCode(400, new { message = "Sikertelen felvétel!", result = role });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("updateRole")]
        public async Task<IActionResult> UpdateRole([FromQuery] Guid id, UpdateRoleDTO updateRoleDTO)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(u => u.Id == id);
                if (role is not null)
                {
                    role.Name = updateRoleDTO.Name;

                    _context.Roles.Update(role);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Sikeres frissítés!", result = role });
                }
                return StatusCode(404, new { message = "Sikertelen frissítés!", result = role });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole([FromQuery] Guid id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role is not null)
                {
                    _context.Roles.Remove(role);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Sikeres törlés!", result = role.Name });
                }
                return StatusCode(404, new { message = "Sikertelen törlés!", result = role });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
