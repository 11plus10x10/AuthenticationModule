using AuthenticationModule.Exceptions;
using AuthenticationModule.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthenticationModule.Tests.RepositoryLayer;

public class UserRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; }
    private readonly UserRepository _repository;

    public UserRepositoryTests(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new UserRepository(Fixture.CreateContext());
    }

    [Fact]
    public async void GetUserByEmailSuccess()
    {
        const string email = "pacan@gmail.com";
        var user = await _repository.GetUserByEmail(email);
        var userInDb = await Fixture.CreateContext()
            .Users.FirstOrDefaultAsync(u => u.EmailAddress == email);

        Assert.NotNull(userInDb);
        Assert.NotNull(user);
        Assert.Equivalent(userInDb, user);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async void GetUserByEmailFailureInvalidInput(string email)
    {
        await Assert
            .ThrowsAsync<ArgumentException>(
                async () => await _repository.GetUserByEmail(email));
    }

    [Fact]
    public async void GetUserByEmailFailure404()
    {
        await Assert
            .ThrowsAsync<UserNotFoundException>(
                async () => await _repository.GetUserByEmail("salopiter@mail.ru"));
    }

    [Fact]
    public async void GetUserByIdSuccess()
    {
        var user = await _repository.GetUserById(1);
        Assert.NotNull(user);

        var userInDb = await Fixture.CreateContext()
            .Users.FirstOrDefaultAsync(u => u.Id == 1);
        Assert.NotNull(userInDb);
        
        Assert.Equivalent(user, userInDb);
    }

    [Fact]
    public async void GetUserByIdFailure()
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.GetUserById(-1));
    }

    [Fact]
    public async void CreateUserSuccess()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        var result = await _repository.CreateUser(
            "baklan@gmail.com",
            "noHash",
            "noSalt",
            "token",
            DateTime.UtcNow, 
            1);
        
        context.ChangeTracker.Clear();
        
        Assert.True(result);
    }
}
