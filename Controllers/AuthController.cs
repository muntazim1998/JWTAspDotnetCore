﻿using JWTAuthDotnet9.Entities;
using JWTAuthDotnet9.Models;
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
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user = new();
        [HttpPost("register")]
        public ActionResult<User> Register(UserDto userDto)
        {
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);
            user.Username= userDto.Username;
            user.HashedPassword = hashedPassword;
            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto userDto)
        {
            if (user.Username != userDto.Username)
                return BadRequest("User Not Found");
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, userDto.Password) == PasswordVerificationResult.Failed)
                return BadRequest("Wrong Password");
            string token = CreateToken(user);
            return Ok(token);

        }
        private string CreateToken(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claim,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
