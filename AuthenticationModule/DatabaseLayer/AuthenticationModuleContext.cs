using AuthenticationModule.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationModule.DatabaseLayer;

public sealed class AuthenticationModuleContext : DbContext
{
    public AuthenticationModuleContext(DbContextOptions<AuthenticationModuleContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Save> Saves { get; set; }
    public DbSet<Game> Games { get; set; }
}
