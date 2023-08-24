using AuthenticationModule.DatabaseLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationModule.DatabaseLayer;

public sealed class AuthenticationModuleContext : IdentityDbContext<IdentityUser>
{
    public AuthenticationModuleContext(DbContextOptions<AuthenticationModuleContext> options) : base(options)
    {
    }

    public DbSet<Save> Saves { get; set; }
    public DbSet<Game> Games { get; set; }
}
