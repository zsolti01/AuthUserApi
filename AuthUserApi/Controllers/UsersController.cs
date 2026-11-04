using AuthUserApi.Models;
using AuthUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UsersController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpGet("byId")]
        public async Task<IActionResult> UserById()
        {
            var users = await _context.Users.ToListAsync();
            if (users is not null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés!", result = users });
            }
            return StatusCode(400, new { message = "Sikertelen lekérdezés!", result = users });
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody]NewUserDTO newUserDTO)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDTO.Name,
                Email = newUserDTO.Email,
                Password = newUserDTO.Password,
                RegTime = DateTime.Now
            };

            if (user is not null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres felvétel!", result = user });
            }
            return StatusCode(400, new { message = "Sikertelen felvétel!", result = user });
        }
    }
}
