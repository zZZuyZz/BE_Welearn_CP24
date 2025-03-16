using DataLayer.DbContext;
using DataLayer.DbObject;

namespace RepoLayer.Interface
{
    public interface IAccountRepo : IBaseRepo<Account>
    {
        Task<Account> GetByUsernameAsync(string email);
        Task<Account> GetByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password);
        Task<Account> GetProfileByIdAsync(int id);
        Task<Account> GetByEmailAsync(string email);

        Task<Account> GetMemberSignalrAsync(string username);
        Task<Account> GetUserByUsernameSignalrAsync(string username);
    }
}