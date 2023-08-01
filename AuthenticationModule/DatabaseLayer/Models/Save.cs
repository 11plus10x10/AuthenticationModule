namespace AuthenticationModule.DatabaseLayer.Models;

public class Save
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public string SaveName { get; set; }
    public string SaveData { get; set; }
}
