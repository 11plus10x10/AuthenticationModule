using AuthenticationModule.DatabaseLayer.Models;
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

        var result = await _repository.CreateUser(
            "baklan@gmail.com",
            "noHash",
            "noSalt",
            "token",
            DateTime.UtcNow,
            1);

        //  Clearing the effect of _repository.CreateUser();
        context.ChangeTracker.Clear();

        Assert.True(result);
    }

    public static IEnumerable<object[]> GetUserWithBadFields()
    {
        yield return new object[]
        {
            new User
            {
                EmailAddress = "", PasswordHash = "123", PasswordSalt = "123", ConfirmationToken = "123",
                TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = 2
            },
            new User
            {
                EmailAddress = "gnilushka@gmail.com", PasswordHash = "", PasswordSalt = "123",
                ConfirmationToken = "123", TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = 2
            },
            new User
            {
                EmailAddress = "pollogrande@gmail.com", PasswordHash = "123", PasswordSalt = "",
                ConfirmationToken = "123", TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = 2
            },
            new User
            {
                EmailAddress = "cucumber@gmail.com", PasswordHash = "123", PasswordSalt = "123", ConfirmationToken = "",
                TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = 2
            },
            new User
            {
                EmailAddress = "coocnut@gmail.com", PasswordHash = "", PasswordSalt = "123", ConfirmationToken = "123",
                TokenGenerationTime = DateTime.MinValue, EmailValidationStatusId = 2
            },
            new User
            {
                EmailAddress = "vectorpacan@gmail.com", PasswordHash = "", PasswordSalt = "123",
                ConfirmationToken = "123", TokenGenerationTime = DateTime.UtcNow, EmailValidationStatusId = -1
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetUserWithBadFields))]
    public async void CreateUserFailureBadFields(User userWithEmptyMail, User userWithEmptyHash, User userWithEmptySalt,
        User userWithEmptyToken, User userWithBadTokenTime, User userWithNegativeStatusId)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyMail.EmailAddress, userWithEmptyMail.PasswordHash,
                userWithEmptyMail.PasswordSalt, userWithEmptyMail.ConfirmationToken,
                userWithEmptyMail.TokenGenerationTime, userWithEmptyMail.EmailValidationStatusId));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyHash.EmailAddress, userWithEmptyHash.PasswordHash,
                userWithEmptyHash.PasswordSalt, userWithEmptyHash.ConfirmationToken,
                userWithEmptyHash.TokenGenerationTime, userWithEmptyHash.EmailValidationStatusId));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptySalt.EmailAddress, userWithEmptySalt.PasswordHash,
                userWithEmptySalt.PasswordSalt, userWithEmptySalt.ConfirmationToken,
                userWithEmptySalt.TokenGenerationTime, userWithEmptySalt.EmailValidationStatusId));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithEmptyToken.EmailAddress, userWithEmptyToken.PasswordHash,
                userWithEmptyToken.PasswordSalt, userWithEmptyToken.ConfirmationToken,
                userWithEmptyToken.TokenGenerationTime, userWithEmptyToken.EmailValidationStatusId));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithBadTokenTime.EmailAddress, userWithBadTokenTime.PasswordHash,
                userWithBadTokenTime.PasswordSalt, userWithBadTokenTime.ConfirmationToken,
                userWithBadTokenTime.TokenGenerationTime, userWithBadTokenTime.EmailValidationStatusId));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateUser(userWithNegativeStatusId.EmailAddress,
                userWithNegativeStatusId.PasswordHash, userWithNegativeStatusId.PasswordSalt,
                userWithNegativeStatusId.ConfirmationToken, userWithNegativeStatusId.TokenGenerationTime,
                userWithNegativeStatusId.EmailValidationStatusId));
    }


    [Fact]
    public async void UpdatePasswordSuccess()
    {
        var result = await _repository.UpdatePassword(1, "miel");

        Assert.True(result);
    }

    [Theory]
    [InlineData(-1, "666")]
    [InlineData(1, "")]
    [InlineData(1, null)]
    public async void UpdatePasswordFailure(int userId, string newPasswordHash)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.UpdatePassword(userId, newPasswordHash));
    }

    [Fact]
    public async void UpdateConfirmationTokenSuccess()
    {
        var result = await _repository.UpdateConfirmationToken(1);

        Assert.True(result);
    }

    [Fact]
    public async void UpdateConfirmationTokenFailure()
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.UpdateConfirmationToken(-1));
    }
}
