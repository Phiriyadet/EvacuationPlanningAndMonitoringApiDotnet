using Evacuation.API.Controllers;
using Evacuation.Application.DTOs.Location;
using Evacuation.Application.DTOs.Zone;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Shared.Result;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Evacuation.UnitTests.Controllers
{
    public class ZoneControllerTests
    {
        private readonly Mock<IZoneService> _mockService;
        private readonly ZoneController _controller;

        public ZoneControllerTests()
        {
            _mockService = new Mock<IZoneService>();
            _controller = new ZoneController(_mockService.Object);
        }

        #region GET api/zones
        [Fact]
        public async Task Get_ReturnsOk_WhenZonesExist()
        {
            // Arrange
            var zones = new List<ZoneDto>
        {
            new ZoneDto { ZoneId = "Z-1", NumberOfPeople = 100, UrgencyLevel = 3 },
            new ZoneDto { ZoneId = "Z-2", NumberOfPeople = 50, UrgencyLevel = 5 }
        };
            _mockService.Setup(s => s.GetAllZonesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<ZoneDto>>.Ok(zones));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ZoneDto>>(okResult.Value);
            Assert.Equal(2, ((List<ZoneDto>)returnValue).Count);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoZones()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllZonesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<ZoneDto>>.Fail("No zones found", null));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsServerError_WhenException()
        {
            // Arrange
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.GetAllZonesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<ZoneDto>>.Fail("Error retrieving zones", null, ex));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region GET api/zones/{id}
        [Fact]
        public async Task GetById_ReturnsOk_WhenZoneExists()
        {
            // Arrange
            var zone = new ZoneDto { ZoneId = "Z-1", NumberOfPeople = 100, UrgencyLevel = 3 };
            _mockService.Setup(s => s.GetZoneByIdAsync(1))
                .ReturnsAsync(OperationResult<ZoneDto>.Ok(zone));

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ZoneDto>(okResult.Value);
            Assert.Equal("Z-1", returnValue.ZoneId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenZoneDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetZoneByIdAsync(99))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Zone not found", null));

            // Act
            var result = await _controller.GetByIdAsync(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsServerError_WhenException()
        {
            // Arrange
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.GetZoneByIdAsync(1))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Error retrieving zone", null, ex));

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region POST api/zones
        [Fact]
        public async Task Post_ReturnsCreated_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateZoneDto { NumberOfPeople = 100, UrgencyLevel = 3, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var zoneDto = new ZoneDto { ZoneId = "Z-1", NumberOfPeople = 100, UrgencyLevel = 3 };
            _mockService.Setup(s => s.AddZoneAsync(createDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Ok(zoneDto));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ZoneDto>(createdResult.Value);
            Assert.Equal("Z-1", returnValue.ZoneId);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var createDto = new CreateZoneDto { NumberOfPeople = -1, UrgencyLevel = 6 };
            _mockService.Setup(s => s.AddZoneAsync(createDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Validation error", null));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsServerError_WhenException()
        {
            // Arrange
            var createDto = new CreateZoneDto { NumberOfPeople = 50, UrgencyLevel = 3, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.AddZoneAsync(createDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Error adding zone", null, ex));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region PUT api/zone/{id}
        [Fact]
        public async Task Put_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var updateDto = new UpdateZoneDto { NumberOfPeople = 150, UrgencyLevel = 4, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var zoneDto = new ZoneDto { ZoneId = "Z-1", NumberOfPeople = 150, UrgencyLevel = 4 };
            _mockService.Setup(s => s.UpdateZoneAsync(1, updateDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Ok(zoneDto));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ZoneDto>(okResult.Value);
            Assert.Equal(150, returnValue.NumberOfPeople);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateZoneDto { NumberOfPeople = -5, UrgencyLevel = 6 };
            _mockService.Setup(s => s.UpdateZoneAsync(1, updateDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Validation error", null));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsServerError_WhenException()
        {
            // Arrange
            var updateDto = new UpdateZoneDto { NumberOfPeople = 100, UrgencyLevel = 3, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.UpdateZoneAsync(1, updateDto))
                .ReturnsAsync(OperationResult<ZoneDto>.Fail("Error updating zone", null, ex));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region DELETE api/zone/{id}
        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteZoneAsync(1))
                .ReturnsAsync(OperationResult<bool>.Ok(true));

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenZoneDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteZoneAsync(99))
                .ReturnsAsync(OperationResult<bool>.Fail("Zone not found", false));

            // Act
            var result = await _controller.DeleteAsync(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsServerError_WhenException()
        {
            // Arrange
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.DeleteZoneAsync(1))
                .ReturnsAsync(OperationResult<bool>.Fail("Error deleting zone", false, ex));

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion
    }

}
