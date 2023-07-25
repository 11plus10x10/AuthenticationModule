using AuthenticationModule.DatabaseLayer;
using AuthenticationModule.DatabaseLayer.Models;
using AuthenticationModule.Exceptions;
using AuthenticationModule.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationModule.RepositoryLayer;

public class UserRepository : IRepository
{
    // public UserRepository(AuthenticationModuleContext context) : base(context)
    // {
    // }
    public AuthenticationModuleContext Context { get; }

    public UserRepository(AuthenticationModuleContext context)
    {
        Context = context;
    }

    public async Task<bool> CreateUser(
        string email,
        string passwordHash,
        string passwordSalt,
        string confirmationToken,
        DateTime tokenGenerationTime,
        int emailValidationStatusId)
    {
        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(passwordHash)
            || string.IsNullOrWhiteSpace(passwordSalt)
            || string.IsNullOrWhiteSpace(confirmationToken)
            || tokenGenerationTime == DateTime.MinValue
            || emailValidationStatusId.IsNegative())
        {
            throw new ArgumentException();
        }

        var newUser = new User
        {
            EmailAddress = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            ConfirmationToken = confirmationToken,
            TokenGenerationTime = tokenGenerationTime,
            EmailValidationStatusId = emailValidationStatusId
        };

        try
        {
            Context.Users.Add(newUser);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception during database query: {e.Message}");
            throw new CouldNotAddUserToDatabaseException();
        }

        return true;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine($"{nameof(email).ToUpper()} is either empty, null or contains only a whitespace.");
            throw new ArgumentException();
        }

        return await Context.Users.FirstOrDefaultAsync(u => u.EmailAddress == email)
               ?? throw new UserNotFoundException();
    }

    public async Task<bool> UpdatePassword(int userId, string newPasswordHash)
    {
        if (userId.IsNegative() || string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new ArgumentException();
        }

        var result = await Context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(user => user.SetProperty(p => p.PasswordHash, newPasswordHash));

        return result == 1 ? true : false;
    }

    public async Task<bool> UpdateConfirmationToken(int userId)
    {
        if (userId.IsNegative())
        {
            throw new ArgumentException();
        }

        var result = await Context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s
                => s.SetProperty(u => u.TokenGenerationTime, new DateTime()));

        return result == 1;
    }

    public async Task<User> GetUserById(int userId)
    {
        if (userId.IsNegative()) throw new ArgumentException();
        
        return await Context.Users.FirstOrDefaultAsync(u => u.Id == userId)
               ?? throw new UserNotFoundException();
    }
}
