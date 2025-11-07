using Evacuation.Application.DTOs.Zone;
using Evacuation.Application.Services.Factory.Interfaces;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers.V2
{
    [Route("api/v2/zones")]
    [ApiController]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        public ZonesController(IServiceFactory serviceFactory)
        {
            _zoneService = serviceFactory.CreateZoneService(DatabaseType.Postgres);
        }
        // GET: api/<ZoneController>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
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
        public async Task<IActionResult> GetByIdAsync(int id)
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
        public async Task<IActionResult> AddAsync([FromBody] CreateZoneDto zoneDto)
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
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // PUT api/<ZoneController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateZoneDto zoneDto)
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
        public async Task<IActionResult> DeleteAsync(int id)
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
