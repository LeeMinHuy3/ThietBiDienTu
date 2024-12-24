using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userRepository;

        public UserController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            var user = await _userRepository.GetUserByUsernameAndPassword(loginUser.Username, loginUser.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(user);
        }

        // POST: api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            var existingUser = await _userRepository.GetUserByUsernameAndPassword(newUser.Username, newUser.Password);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            await _userRepository.AddUser(newUser);
            return Ok(newUser);
        }
    }
}
