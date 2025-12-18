using Microsoft.AspNetCore.Mvc;
using SafeBoda.Core.Entities;
using SafeBoda.Application.Interfaces;

namespace SafeBoda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;

        public DriversController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        // GET: api/drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDriversAsync();

            var driverDtos = drivers.Select(d => new DriverDto
            {
                Id = d.Id,
                Name = d.Name,
                PhoneNumber = d.PhoneNumber,
                VehicleNumber = d.VehicleNumber,
                VehicleType = d.VehicleType,
                LicenseNumber = d.LicenseNumber
            });

            return Ok(driverDtos);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetDriverById(Guid id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            if (driver == null) return NotFound();

            var driverDto = new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                PhoneNumber = driver.PhoneNumber,
                VehicleNumber = driver.VehicleNumber,
                VehicleType = driver.VehicleType,
                LicenseNumber = driver.LicenseNumber
            };

            return Ok(driverDto);
        }

        
        [HttpPost]
        public async Task<ActionResult<DriverDto>> CreateDriver([FromBody] DriverRequest request)
        {
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                VehicleNumber = request.VehicleNumber,
                VehicleType = request.VehicleType,
                LicenseNumber = request.LicenseNumber
            };

            var created = await _driverRepository.CreateDriverAsync(driver);

            var driverDto = new DriverDto
            {
                Id = created.Id,
                Name = created.Name,
                PhoneNumber = created.PhoneNumber,
                VehicleNumber = created.VehicleNumber,
                VehicleType = created.VehicleType,
                LicenseNumber = created.LicenseNumber
            };

            return CreatedAtAction(nameof(GetDriverById), new { id = driverDto.Id }, driverDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(Guid id, [FromBody] DriverRequest request)
        {
            var existing = await _driverRepository.GetDriverByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = request.Name;
            existing.PhoneNumber = request.PhoneNumber;
            existing.VehicleNumber = request.VehicleNumber;
            existing.VehicleType = request.VehicleType;
            existing.LicenseNumber = request.LicenseNumber;

            await _driverRepository.UpdateDriverAsync(existing);
            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            var existing = await _driverRepository.GetDriverByIdAsync(id);
            if (existing == null) return NotFound();

            await _driverRepository.DeleteDriverAsync(id);
            return NoContent();
        }
    }

    
    public record DriverRequest(
        string Name,
        string PhoneNumber,
        string VehicleNumber,
        string VehicleType,
        string LicenseNumber
    );

    
    public class DriverDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
