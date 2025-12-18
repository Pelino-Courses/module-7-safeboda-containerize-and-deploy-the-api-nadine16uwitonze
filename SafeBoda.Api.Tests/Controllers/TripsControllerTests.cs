using SafeBoda.Api.Controllers;
using SafeBoda.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeBoda.Core.Entities;
using Microsoft.Extensions.Caching.Memory;
using Moq;


namespace SafeBoda.Api.Tests.Controllers
{
    public class TripsControllerTests
    {
        private readonly Mock<ITripRepository> _mockRepo;
        private readonly TripsController _controller;

        public TripsControllerTests()
        {
            _mockRepo = new Mock<ITripRepository>();

           
            var mockCache = new Mock<IMemoryCache>();

            
            object cacheEntry = null!;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheEntry))
                     .Returns(false);

           
            mockCache.Setup(c => c.Set(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()))
                     .Verifiable();

            
            _controller = new TripsController(_mockRepo.Object, mockCache.Object);
        }

        [Fact]
        public async Task GetAllTrips_ReturnsOkWithList()
        {
           
            var trips = new List<Trip>
            {
                new Trip { Id = Guid.NewGuid(), RiderId = Guid.NewGuid(), DriverId = Guid.NewGuid(), Amount = 5000, IsActive = true }
            };

            _mockRepo.Setup(r => r.GetAllTripsAsync()).ReturnsAsync(trips);

         
            var result = await _controller.GetAllTrips();

           
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetTripById_NotFound_WhenTripDoesNotExist()
        {
           
            _mockRepo.Setup(r => r.GetTripByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Trip?)null);

           
            var result = await _controller.GetTripById(Guid.NewGuid());

           
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetTripById_ReturnsTrip_WhenFound()
        {
           
            var id = Guid.NewGuid();
            var trip = new Trip { Id = id, RiderId = id, DriverId = id, Amount = 2000, IsActive = true };

            _mockRepo.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(trip);

           
            var result = await _controller.GetTripById(id);

          
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }
    }
}
