using Evacuation.API.Controllers;
using Evacuation.Application.DTOs.Location;
using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Evacuation.Shared.Result;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Evacuation.UnitTests.Controllers
{
    public class VehicleControllerTests
    {
        private readonly Mock<IVehicleService> _mockService;
        private readonly VehiclesController _controller;

        public VehicleControllerTests()
        {
            _mockService = new Mock<IVehicleService>();
            _controller = new VehiclesController(_mockService.Object);
        }

        #region GET api/vehicle
        [Fact]
        public async Task Get_ReturnsOk_WhenVehiclesExist()
        {
            // Arrange
            var vehicles = new List<VehicleDto>
        {
            new VehicleDto { VehicleId = "V-1", Type = VehicleType.Bus, Capacity = 10, Speed = 80 },
            new VehicleDto { VehicleId = "V-2", Type = VehicleType.Ambulance, Capacity = 2, Speed = 120 }
        };
            _mockService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<VehicleDto>>.Ok(vehicles));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<VehicleDto>>(okResult.Value);
            Assert.Equal(2, ((List<VehicleDto>)returnValue).Count);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoVehicles()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<VehicleDto>>.Fail("No vehicles found", null));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsServerError_WhenException()
        {
            // Arrange
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(OperationResult<IEnumerable<VehicleDto>>.Fail("Error retrieving vehicles", null, ex));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region GET api/vehicle/{id}
        [Fact]
        public async Task GetById_ReturnsOk_WhenVehicleExists()
        {
            // Arrange
            var vehicle = new VehicleDto { VehicleId = "V-1", Type = VehicleType.Bus, Capacity = 10, Speed = 80 };
            _mockService.Setup(s => s.GetVehicleByIdAsync(1))
                .ReturnsAsync(OperationResult<VehicleDto>.Ok(vehicle));

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<VehicleDto>(okResult.Value);
            Assert.Equal("V-1", returnValue.VehicleId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenVehicleDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetVehicleByIdAsync(99))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Vehicle not found", null));

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
            _mockService.Setup(s => s.GetVehicleByIdAsync(1))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Error retrieving vehicle", null, ex));

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region POST api/vehicle
        [Fact]
        public async Task Post_ReturnsCreated_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateVehicleDto { Type = VehicleType.Bus, Capacity = 10, Speed = 80, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var vehicleDto = new VehicleDto { VehicleId = "V-1", Type = VehicleType.Bus, Capacity = 10, Speed = 80 };
            _mockService.Setup(s => s.AddVehicleAsync(createDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Ok(vehicleDto));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<VehicleDto>(createdResult.Value);
            Assert.Equal("V-1", returnValue.VehicleId);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var createDto = new CreateVehicleDto { Type = VehicleType.Unknown };
            _mockService.Setup(s => s.AddVehicleAsync(createDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Validation error", null));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsServerError_WhenException()
        {
            // Arrange
            var createDto = new CreateVehicleDto { Type = VehicleType.Bus, Capacity = 10, Speed = 80, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.AddVehicleAsync(createDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Error adding vehicle", null, ex));

            // Act
            var result = await _controller.AddAsync(createDto);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region PUT api/vehicle/{id}
        [Fact]
        public async Task Put_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var updateDto = new UpdateVehicleDto { Type = VehicleType.Bus, Capacity = 15, Speed = 90, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var vehicleDto = new VehicleDto { VehicleId = "V-1", Type = VehicleType.Bus, Capacity = 15, Speed = 90 };
            _mockService.Setup(s => s.UpdateVehicleAsync(1, updateDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Ok(vehicleDto));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<VehicleDto>(okResult.Value);
            Assert.Equal(15, returnValue.Capacity);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var updateDto = new UpdateVehicleDto { Type = VehicleType.Unknown };
            _mockService.Setup(s => s.UpdateVehicleAsync(1, updateDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Validation error", null));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsServerError_WhenException()
        {
            // Arrange
            var updateDto = new UpdateVehicleDto { Type = VehicleType.Bus, Capacity = 15, Speed = 90, Location = new LocationCoordinatesDto { Latitude = 0, Longitude = 0 } };
            var ex = new Exception("DB error");
            _mockService.Setup(s => s.UpdateVehicleAsync(1, updateDto))
                .ReturnsAsync(OperationResult<VehicleDto>.Fail("Error updating vehicle", null, ex));

            // Act
            var result = await _controller.UpdateAsync(1, updateDto);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion

        #region DELETE api/vehicle/{id}
        [Fact]
        public async Task Delete_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteVehicleAsync(1))
                .ReturnsAsync(OperationResult<bool>.Ok(true));

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenVehicleDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteVehicleAsync(99))
                .ReturnsAsync(OperationResult<bool>.Fail("Vehicle not found", false));

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
            _mockService.Setup(s => s.DeleteVehicleAsync(1))
                .ReturnsAsync(OperationResult<bool>.Fail("Error deleting vehicle", false, ex));

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var serverResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverResult.StatusCode);
        }
        #endregion
    }

}
