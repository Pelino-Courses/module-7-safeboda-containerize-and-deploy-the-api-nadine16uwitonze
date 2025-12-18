using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeBoda.Core.Entities;
using SafeBoda.Application.Interfaces;

namespace SafeBoda.Application.Repositories
{
    public class InMemoryTripRepository : ITripRepository
    {
        private readonly ConcurrentDictionary<Guid, Trip> _trips = new();

        public Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            var allTrips = _trips.Values.ToList();
            return Task.FromResult<IEnumerable<Trip>>(allTrips);
        }

        public Task<Trip?> GetTripByIdAsync(Guid id)
        {
            _trips.TryGetValue(id, out var trip);
            return Task.FromResult(trip);
        }

        public Task<Trip> CreateTripAsync(Trip trip)
        {
            trip.Id = trip.Id == Guid.Empty ? Guid.NewGuid() : trip.Id;
            _trips[trip.Id] = trip;
            return Task.FromResult(trip);
        }

        public Task UpdateTripAsync(Trip trip)
        {
            if (_trips.ContainsKey(trip.Id))
            {
                _trips[trip.Id] = trip;
            }
            return Task.CompletedTask;
        }

        public Task DeleteTripAsync(Guid id)
        {
            _trips.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}