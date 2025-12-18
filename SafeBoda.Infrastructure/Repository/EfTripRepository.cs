using SafeBoda.Core.Entities;
using SafeBoda.Application.Interfaces;
using SafeBoda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SafeBoda.Infrastructure.Repository
{
    public class EfTripRepository : ITripRepository
    {
        private readonly AppDbContext _context;

        public EfTripRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            return await _context.Trips.ToListAsync();
        }

        public async Task<Trip?> GetTripByIdAsync(Guid id)
        {
            return await _context.Trips.FindAsync(id);
        }

        public async Task<Trip> CreateTripAsync(Trip trip)
        {
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return trip;
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTripAsync(Guid id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
            }
        }
    }
}