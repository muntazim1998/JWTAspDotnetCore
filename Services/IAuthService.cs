using JWTAuthDotnet9.Entities;
using JWTAuthDotnet9.Models;

namespace JWTAuthDotnet9.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto userDto);
        Task<string?> LoginAsync(UserDto userDto);
    }
}
