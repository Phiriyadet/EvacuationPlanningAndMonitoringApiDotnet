using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _vehicleService.GetAllVehiclesAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving vehicles", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving the vehicle", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // POST api/<VehicleController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateVehicleDto vehicleDto)
        {
            var result = await _vehicleService.AddVehicleAsync(vehicleDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while adding the vehicle", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return CreatedAtAction(nameof(Get), new { id = result.Data!.VehicleId }, result.Data);
        }

        // PUT api/<VehicleController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateVehicleDto vehicleDto)
        {
            var result = await _vehicleService.UpdateVehicleAsync(id, vehicleDto);
            if (!result.IsSuccess) 
            {
                if (result.Exception != null) 
                {
                    return StatusCode(500, new { Message = "An error occurred while updating the vehicle", Error = result.Exception.Message });
                }
                else {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // DELETE api/<VehicleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while deleting the vehicle", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(new { Message = $"Vehicle with ID {id} deleted successfully" });
        }
    }
}
