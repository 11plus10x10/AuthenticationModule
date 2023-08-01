using AuthenticationModule.DatabaseLayer.Models;
using AuthenticationModule.Exceptions;
using AuthenticationModule.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthenticationModule.Tests.RepositoryLayer;

public class UserRepositoryTests : IClassFixture<TestDatabaseFixture>, ITestClass
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
    public async void GetUserByIdFailureArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.GetUserById(-1));
    }

    [Fact]
    public async void GetUserByIdFailure404()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(async ()
            => await _repository.GetUserById(666));
    }

    [Fact]
    public async void CreateUserSuccess()
    {
        await using var context = Fixture.CreateContext();
        //  Beginning transaction so no test data goes into DB.
        await context.Database.BeginTransactionAsync();

        var user = new User
        {
            Id = 2,
            EmailAddress = "baklan@gmail.com",
            PasswordHash = "noHash",
            PasswordSalt = "noSalt",
            ConfirmationToken = "token",
            IsEmailValidated = false,
        };
        await _repository.CreateUser(
            user.EmailAddress,
            user.PasswordHash,
            user.PasswordSalt,
            user.ConfirmationToken);

        //  Clearing the effect of _repository.CreateUser();
        context.ChangeTracker.Clear();

        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.EmailAddress == "baklan@gmail.com");
        Assert.Equivalent(user, userInDb);
    }

    public static IEnumerable<object[]> GetUserWithBadFields()
    {
        yield return new object[]
        {
            new User
            {
                EmailAddress = "", PasswordHash = "123", PasswordSalt = "123", ConfirmationToken = "123",
                IsEmailValidated = true,
            },
            new User
            {
                EmailAddress = "gnilushka@gmail.com", PasswordHash = "", PasswordSalt = "123",
                ConfirmationToken = "123", IsEmailValidated = true,
            },
            new User
            {
                EmailAddress = "pollogrande@gmail.com", PasswordHash = "123", PasswordSalt = "",
                ConfirmationToken = "123", IsEmailValidated = false,
            },
            new User
            {
                EmailAddress = "cucumber@gmail.com", PasswordHash = "123", PasswordSalt = "123", ConfirmationToken = "",
                IsEmailValidated = false,
            },
            new User
            {
                EmailAddress = "coocnut@gmail.com", PasswordHash = "", PasswordSalt = "123", ConfirmationToken = "123",
                IsEmailValidated = true,
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetUserWithBadFields))]
    public async void CreateUserFailureBadFields(User userWithEmptyMail, User userWithEmptyHash, User userWithEmptySalt,
        User userWithEmptyToken, User userWithBadTokenTime)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyMail.EmailAddress, userWithEmptyMail.PasswordHash,
                userWithEmptyMail.PasswordSalt, userWithEmptyMail.ConfirmationToken));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyHash.EmailAddress, userWithEmptyHash.PasswordHash,
                userWithEmptyHash.PasswordSalt, userWithEmptyHash.ConfirmationToken));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptySalt.EmailAddress, userWithEmptySalt.PasswordHash,
                userWithEmptySalt.PasswordSalt, userWithEmptySalt.ConfirmationToken));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyToken.EmailAddress, userWithEmptyToken.PasswordHash,
                userWithEmptyToken.PasswordSalt, userWithEmptyToken.ConfirmationToken));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithBadTokenTime.EmailAddress, userWithBadTokenTime.PasswordHash,
                userWithBadTokenTime.PasswordSalt, userWithBadTokenTime.ConfirmationToken));
    }


    [Fact]
    public async void UpdatePasswordHashSuccess()
    {
        const string newPasswordHash = "miel";
        await _repository.UpdatePasswordHash(1, newPasswordHash);
        var userInDb = await _repository.GetUserById(1);
        Assert.Equal(newPasswordHash, userInDb.PasswordHash);
    }

    [Theory]
    [InlineData(-1, "666")]
    [InlineData(1, "")]
    [InlineData(1, null)]
    public async void UpdatePasswordHashFailure(int userId, string newPasswordHash)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.UpdatePasswordHash(userId, newPasswordHash));
    }

    [Fact]
    public async void UpdateConfirmationTokenSuccess()
    {
        var newConfirmationToken = $"freshToken: {DateTime.Now.ToShortTimeString()}";
        await _repository
            .UpdateConfirmationToken(1, newConfirmationToken);

        var userInDb = await _repository.GetUserById(1);

        Assert.Equal(newConfirmationToken, userInDb.ConfirmationToken);
    }

    [Theory]
    [InlineData(-1, "")]
    [InlineData(-1, null)]
    [InlineData(1, "")]
    [InlineData(1, null)]
    public async void UpdateConfirmationTokenFailure(int userId, string newConfirmationToken)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.UpdateConfirmationToken(userId, newConfirmationToken));
    }
}
