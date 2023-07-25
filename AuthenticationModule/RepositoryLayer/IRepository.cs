using AuthenticationModule.DatabaseLayer;

namespace AuthenticationModule.RepositoryLayer;

public interface IRepository
{
    public AuthenticationModuleContext Context { get; }
}
