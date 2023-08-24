using AuthenticationModule.DatabaseLayer;

namespace AuthenticationModule.RepositoryLayer;

public class UserRepository : IRepository
{
    public AuthenticationModuleContext Context { get; }

    public UserRepository(AuthenticationModuleContext context)
    {
        Context = context;
    }
}
