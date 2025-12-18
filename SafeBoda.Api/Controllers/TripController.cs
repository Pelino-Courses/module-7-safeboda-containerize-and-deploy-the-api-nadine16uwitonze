using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SafeBoda.Core.Entities;
using SafeBoda.Application.Interfaces;

namespace SafeBoda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripRepository _tripRepository;
        private readonly IMemoryCache _cache;

        public TripsController(ITripRepository tripRepository, IMemoryCache cache)
        {
            _tripRepository = tripRepository;
            _cache = cache;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAllTrips()
        {
           
            if (_cache.TryGetValue("trips-cache", out IEnumerable<TripDto> cachedTrips))
                return Ok(cachedTrips);

            var trips = await _tripRepository.GetAllTripsAsync();

            var tripDtos = trips.Select(t => new TripDto
            {
                Id = t.Id,
                RiderId = t.RiderId,
                DriverId = t.DriverId,
                Amount = t.Amount,    
                Status = t.IsActive ? "Active" : "Completed"
            }).ToList();

           
            _cache.Set("trips-cache", tripDtos, TimeSpan.FromMinutes(1));

            return Ok(tripDtos);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDto>> GetTripById(Guid id)
        {
            var trip = await _tripRepository.GetTripByIdAsync(id);
            if (trip == null) return NotFound();

            var tripDto = new TripDto
            {
                Id = trip.Id,
                RiderId = trip.RiderId,
                DriverId = trip.DriverId,
                Amount = trip.Amount, 
                Status = trip.IsActive ? "Active" : "Completed"
            };

            return Ok(tripDto);
        }

        [HttpPost]
        public async Task<ActionResult<TripDto>> CreateTrip([FromBody] TripRequest request)
        {
            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                RiderId = request.RiderId,
                DriverId = request.DriverId,
                Amount = request.Price,  // 👈 correct
                Date = DateTime.UtcNow,
                IsActive = true
            };

            var createdTrip = await _tripRepository.CreateTripAsync(trip);

           
            _cache.Remove("trips-cache");

            var tripDto = new TripDto
            {
                Id = createdTrip.Id,
                RiderId = createdTrip.RiderId,
                DriverId = createdTrip.DriverId,
                Amount = createdTrip.Amount,
                Status = createdTrip.IsActive ? "Active" : "Completed"
            };

            return CreatedAtAction(nameof(GetTripById), new { id = tripDto.Id }, tripDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(Guid id, [FromBody] TripRequest request)
        {
            var existing = await _tripRepository.GetTripByIdAsync(id);
            if (existing == null) return NotFound();

            existing.RiderId = request.RiderId;
            existing.DriverId = request.DriverId;
            existing.Amount = request.Price;

            await _tripRepository.UpdateTripAsync(existing);

            _cache.Remove("trips-cache");

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var existing = await _tripRepository.GetTripByIdAsync(id);
            if (existing == null) return NotFound();

            await _tripRepository.DeleteTripAsync(id);
            
            _cache.Remove("trips-cache");

            return NoContent();
        }
    }
    
    public record TripRequest(Guid RiderId, Guid DriverId, decimal Price);
    
    public class TripDto
    {
        public Guid Id { get; set; }
        public Guid RiderId { get; set; }
        public Guid DriverId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
