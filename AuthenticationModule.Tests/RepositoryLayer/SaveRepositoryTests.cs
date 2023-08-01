using AuthenticationModule.DatabaseLayer.Models;
using AuthenticationModule.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthenticationModule.Tests.RepositoryLayer;

public class SaveRepositoryTests : IClassFixture<TestDatabaseFixture>, ITestClass
{
    public TestDatabaseFixture Fixture { get; }
    private readonly SaveRepository _repository;

    public SaveRepositoryTests(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new SaveRepository(Fixture.CreateContext());
    }

    [Fact]
    public async void CreateSaveSuccess()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();

        var save = new Save
        {
            GameId = 1,
            SaveName = "CreatedByTestSuccess",
            SaveData = "CreatedByTestSuccess",
        };

        await _repository.CreateSave(1, save.GameId, save.SaveName, save.SaveData);
        context.ChangeTracker.Clear();

        var saveInDb = await context.Saves.FirstOrDefaultAsync(s => s.SaveName == save.SaveName);
        Assert.Equal(save.GameId, saveInDb.GameId);
        Assert.Equal(save.SaveName, saveInDb.SaveName);
        Assert.Equal(save.SaveData, saveInDb.SaveData);
    }

    public static IEnumerable<object[]> GetSavesWithBadFields()
    {
        yield return new object[]
        {
            new Save { UserId = -1, GameId = 1, SaveName = "123", SaveData = "456" },
            new Save { UserId = 1, GameId = -1, SaveName = "123", SaveData = "456" },
            new Save { UserId = 1, GameId = 1, SaveName = "", SaveData = "456" },
            new Save { UserId = 1, GameId = 1, SaveName = "123", SaveData = null! },
        };
    }

    [Theory]
    [MemberData(nameof(GetSavesWithBadFields))]
    public async void CreateSaveFailure(Save saveWithNegativeUserId, Save saveWithNegativeGameId,
        Save saveWithEmptySaveName, Save saveWithNullSaveData)
    {
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateSave(saveWithNegativeUserId.UserId, saveWithNegativeUserId.GameId,
                saveWithNegativeUserId.SaveName, saveWithNegativeUserId.SaveData));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateSave(saveWithNegativeGameId.UserId, saveWithNegativeGameId.GameId,
                saveWithNegativeGameId.SaveName, saveWithNegativeGameId.SaveData));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateSave(saveWithEmptySaveName.UserId, saveWithEmptySaveName.GameId,
                saveWithEmptySaveName.SaveName, saveWithEmptySaveName.SaveData));

        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await _repository.CreateSave(saveWithNullSaveData.UserId, saveWithNullSaveData.GameId,
                saveWithNullSaveData.SaveName, saveWithNullSaveData.SaveData));
    }

    [Fact]
    public async void UpdateSaveNameSuccess()
    {
        var date = DateTime.Now.ToShortTimeString();
        await _repository.UpdateSaveName(1, $"freshSave: {date}");
        await using var context = Fixture.CreateContext();
        var save = await context.Saves.FirstOrDefaultAsync(s => s.Id == 1);
        Assert.NotNull(save);
        
        Assert.Equal($"freshSave: {date}", save.SaveName);
    }
}
