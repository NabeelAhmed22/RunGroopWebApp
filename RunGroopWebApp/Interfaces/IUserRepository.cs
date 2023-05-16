using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetUserById(string Id);
        bool Add(AppUser user);
        Task Delete(AppUser user);
        bool Update(AppUser user);
        bool Save();

    }
}
