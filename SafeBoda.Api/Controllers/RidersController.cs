using Microsoft.AspNetCore.Mvc;
using SafeBoda.Core.Entities;
using SafeBoda.Application.Interfaces;

namespace SafeBoda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RidersController : ControllerBase
    {
        private readonly IRiderRepository _riderRepository;

        public RidersController(IRiderRepository riderRepository)
        {
            _riderRepository = riderRepository;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RiderDto>>> GetAllRiders()
        {
            var riders = await _riderRepository.GetAllRidersAsync();

            var riderDtos = riders.Select(r => new RiderDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email
            });

            return Ok(riderDtos);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<RiderDto>> GetRiderById(Guid id)
        {
            var rider = await _riderRepository.GetRiderByIdAsync(id);
            if (rider == null) return NotFound();

            var riderDto = new RiderDto
            {
                Id = rider.Id,
                Name = rider.Name,
                Email = rider.Email
            };

            return Ok(riderDto);
        }

        
        [HttpPost]
        public async Task<ActionResult<RiderDto>> CreateRider([FromBody] RiderRequest request)
        {
            var rider = new Rider
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email
            };

            var created = await _riderRepository.CreateRiderAsync(rider);

            var riderDto = new RiderDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email
            };

            return CreatedAtAction(nameof(GetRiderById), new { id = riderDto.Id }, riderDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRider(Guid id, [FromBody] RiderRequest request)
        {
            var existing = await _riderRepository.GetRiderByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = request.Name;
            existing.Email = request.Email;

            await _riderRepository.UpdateRiderAsync(existing);
            return NoContent();
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRider(Guid id)
        {
            var existing = await _riderRepository.GetRiderByIdAsync(id);
            if (existing == null) return NotFound();

            await _riderRepository.DeleteRiderAsync(id);
            return NoContent();
        }
    }

    public record RiderRequest(string Name, string Email);

    
    public class RiderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
