using Evacuation.Application.DTOs.Zone;
using Evacuation.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }
        // GET: api/<ZoneController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _zoneService.GetAllZonesAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving zones", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<ZoneController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _zoneService.GetZoneByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving the zone", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // POST api/<ZoneController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateZoneDto zoneDto)
        {
            var result = await _zoneService.AddZoneAsync(zoneDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while adding the zone", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message, Errors = result.Errors });
                }
            }
            return CreatedAtAction(nameof(Get), new { id = result.Data!.ZoneId }, result.Data);
        }

        // PUT api/<ZoneController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateZoneDto zoneDto)
        {
            var result = await _zoneService.UpdateZoneAsync(id, zoneDto);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while updating the zone", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message, Errors = result.Errors });
                }
            }
            return Ok(result.Data);
        }

        // DELETE api/<ZoneController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _zoneService.DeleteZoneAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while deleting the zone", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(new { Message = $"Zone with ID {id} deleted successfully" });
        }
    }
}
