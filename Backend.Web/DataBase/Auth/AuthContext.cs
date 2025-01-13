using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataBase.Auth;

public class AuthContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public DbSet<IdentityUser> Users { get; set; }
    
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }
}