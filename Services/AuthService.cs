using JWTAuthDotnet9.Data;
using JWTAuthDotnet9.Entities;
using JWTAuthDotnet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthDotnet9.Services
{
    public class AuthService(UserDbContext userDbContext, IConfiguration configuration) : IAuthService
    {
        public async Task<string?>  LoginAsync(UserDto userDto)
        {
            var user=await userDbContext.Users.FirstOrDefaultAsync(x => x.Username == userDto.Username);
            if (user is null)
                return null;
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, userDto.Password) == PasswordVerificationResult.Failed)
                return "wrongPass";
            return CreateToken(user);
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            if(await userDbContext.Users.AnyAsync(x=>x.Username == userDto.Username))
                return null;
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);
            user.Username = userDto.Username;
            user.HashedPassword = hashedPassword;
            // Add the user to the database
            await userDbContext.Users.AddAsync(user);
            // Save changes to the database
            await userDbContext.SaveChangesAsync();
            return user;
        }
        private string CreateToken(User user)
        {

                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
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
