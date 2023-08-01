using AuthenticationModule.DatabaseLayer;
using AuthenticationModule.DatabaseLayer.Models;
using AuthenticationModule.Exceptions;
using AuthenticationModule.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationModule.RepositoryLayer;

public class SaveRepository : IRepository
{
    public AuthenticationModuleContext Context { get; }

    public SaveRepository(AuthenticationModuleContext context)
    {
        Context = context;
    }

    public async Task CreateSave(int userId, int gameId, string saveName, string saveData)
    {
        if (userId.IsNegative() ||
            gameId.IsNegative() ||
            string.IsNullOrWhiteSpace(saveName) ||
            string.IsNullOrWhiteSpace(saveData))
        {
            throw new ArgumentException("Invalid arguments provided");
        }

        var newSave = new Save
        {
            UserId = userId,
            GameId = gameId,
            SaveName = saveName,
            SaveData = saveData,
        };

        try
        {
            Context.Saves.Add(newSave);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception during database query: {e.Message}");
            throw new CouldNotAddSaveToDatabaseException();
        }
    }

    public IEnumerable<Save> GetAllPlayerSaves(int userId)
    {
        foreach (var save in Context.Saves.Where(s => s.UserId == userId))
        {
            yield return save;
        }
    }
    
    public async Task UpdateSaveName(int saveId, string newSaveName)
    {
        if (saveId.IsNegative() || string.IsNullOrWhiteSpace(newSaveName))
        {
            throw new ArgumentException();
        }

        var saveUpdateResult = await Context.Saves
            .Where(s => s.Id == saveId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.SaveName, newSaveName));

        if (saveUpdateResult == 0) throw new CouldNotUpdateSave();
    }

    public async Task UpdateSaveData(int saveId, string newSaveData)
    {
        if (saveId.IsNegative() || string.IsNullOrWhiteSpace(newSaveData))
        {
            throw new ArgumentException();
        }

        var result = await Context.Saves
            .Where(s => s.Id == saveId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.SaveData, newSaveData));

        if (result == 0) throw new CouldNotUpdateSave();
    }
}
