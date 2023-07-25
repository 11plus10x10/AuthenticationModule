using AuthenticationModule.DatabaseLayer;
using AuthenticationModule.DatabaseLayer.Models;
using AuthenticationModule.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationModule.Tests;

public class TestDatabaseFixture
{
    private static readonly object Lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (Lock)
        {
            if (_databaseInitialized) return;
            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(
                new User { Id = 0, EmailAddress = "pacan@gmail.com", PasswordHash = "!@#", PasswordSalt = "123", ConfirmationToken = "token", TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = 0 },
                new Game { Id = 0, GameName = "Minesweeper" },
                new EmailValidationStatus { Id = 0, UserId = 0, ValidationStatus = true });
            context.SaveChanges();
            _databaseInitialized = true;
        }
    }
    
    public AuthenticationModuleContext CreateContext()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .Build();

        var connectionString = config["Data:DefaultConnection:ConnectionString"];
        
        var options = new DbContextOptionsBuilder<AuthenticationModuleContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new AuthenticationModuleContext(options.Options);
    }
}
