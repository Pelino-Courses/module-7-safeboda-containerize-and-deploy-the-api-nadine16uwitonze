using Microsoft.EntityFrameworkCore;
using SafeBoda.Application.Interfaces;
using SafeBoda.Core.Entities;
using SafeBoda.Infrastructure.Data;

namespace SafeBoda.Infrastructure.Repository
{
    public class RiderRepository : IRiderRepository
    {
        private readonly AppDbContext _context;

        public RiderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rider>> GetAllRidersAsync()
        {
            return await _context.Riders.ToListAsync();
        }

        public async Task<Rider?> GetRiderByIdAsync(Guid id)
        {
            return await _context.Riders.FindAsync(id);
        }

        public async Task<Rider> CreateRiderAsync(Rider rider)
        {
            await _context.Riders.AddAsync(rider);
            await _context.SaveChangesAsync();
            return rider;
        }

        public async Task UpdateRiderAsync(Rider rider)
        {
            _context.Riders.Update(rider);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRiderAsync(Guid id)
        {
            var rider = await _context.Riders.FindAsync(id);
            if (rider != null)
            {
                _context.Riders.Remove(rider);
                await _context.SaveChangesAsync();
            }
        }
    }
}