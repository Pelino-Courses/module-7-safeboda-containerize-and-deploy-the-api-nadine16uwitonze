using SafeBoda.Core.Entities;

namespace SafeBoda.Application.Interfaces
{
    public interface IRiderRepository
    {
        Task<IEnumerable<Rider>> GetAllRidersAsync();
        Task<Rider?> GetRiderByIdAsync(Guid id);
        Task<Rider> CreateRiderAsync(Rider rider);
        Task UpdateRiderAsync(Rider rider);
        Task DeleteRiderAsync(Guid id);
    }
}