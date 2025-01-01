using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IRaceRepository
    {

        Task<IEnumerable<Race>> GetAll();
        Task<Race> GetByIdAsync(int id);
        bool Add(Race race);
        bool Update(Race race);
        Task<bool> Delete(int id);
        bool Save();
        Task<Race> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Race>> GetRacesByCity(string city);
    }
}
