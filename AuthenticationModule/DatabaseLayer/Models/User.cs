namespace AuthenticationModule.DatabaseLayer.Models;

public class User
{
    public int Id { get; set; }
    public string EmailAddress { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string ConfirmationToken { get; set; }
    public int EmailValidationStatusId { get; set; }
}
