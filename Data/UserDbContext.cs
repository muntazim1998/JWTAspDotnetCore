using JWTAuthDotnet9.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthDotnet9.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

    }
}
