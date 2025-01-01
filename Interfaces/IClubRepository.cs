using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IClubRepository
    {

        Task<IEnumerable<Club>> GetAll();
        Task<Club> GetByIdAsync(int id);
        Task<Club> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Club>> GetClubByCity(string id);
        bool Add(Club club);
        bool Update(Club club);
        Task<bool> Delete(int id);
        bool Save();

    }
}
