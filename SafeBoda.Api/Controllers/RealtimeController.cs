using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SafeBoda.Api.Hubs;
using SafeBoda.Core.Entities;

namespace SafeBoda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RealtimeController : ControllerBase
    {
        private readonly IHubContext<RealtimeHub> _hubContext;

        public RealtimeController(IHubContext<RealtimeHub> hubContext)
        {
            _hubContext = hubContext;
        }

       
        [HttpPost("trip")]
        public async Task<IActionResult> BroadcastTrip([FromBody] Trip trip)
        {
            var tripDto = new TripDto
            {
                Id = trip.Id,
                DriverId = trip.DriverId,
                RiderId = trip.RiderId,
                Amount = trip.Amount,                
                Status = trip.IsActive ? "Active" : "Completed"
            };

            await _hubContext.Clients.All.SendAsync("TripCreated", tripDto);
            return Ok(new { message = "Trip broadcasted successfully" });
        }

        [HttpPost("driver-status")]
        public async Task<IActionResult> BroadcastDriverStatus(Guid driverId, bool isOnline)
        {
            await _hubContext.Clients.All.SendAsync("DriverStatusChanged", new
            {
                DriverId = driverId,
                IsOnline = isOnline
            });
            return Ok(new { message = "Driver status broadcasted successfully" });
        }

        [HttpPost("rider-status")]
        public async Task<IActionResult> BroadcastRiderStatus(Guid riderId, bool isActive)
        {
            await _hubContext.Clients.All.SendAsync("RiderStatusChanged", new
            {
                RiderId = riderId,
                IsActive = isActive
            });
            return Ok(new { message = "Rider status broadcasted successfully" });
        }
    }
}
