using JWTAuthDotnet9.Entities;
using JWTAuthDotnet9.Models;
using JWTAuthDotnet9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthDotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
      
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            var user = await authService.RegisterAsync(userDto);
            if(user is null)
                return BadRequest("User Already Registered");
            return Ok(user);
        }

        [HttpPost("login")]
        public async  Task<ActionResult<string>> Login(UserDto userDto)
        {
            var userString= await authService.LoginAsync(userDto);
            if (userString is null)
                return BadRequest("User Not Found");
            if (userString=="wrongPass")
                return BadRequest("Wrong Password");
            return Ok(userString);

        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoint()
        {
            return Ok("You are authenticated!");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You are an admin!");
        }
      
    }
}
