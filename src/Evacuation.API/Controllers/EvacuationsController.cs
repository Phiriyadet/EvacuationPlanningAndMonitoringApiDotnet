using Evacuation.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evacuation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvacuationsController : ControllerBase
    {
        private readonly IEvacuationService _evacuationService;
        public EvacuationsController(IEvacuationService evacuationService)
        {
            _evacuationService = evacuationService;
        }

        // GET: api/evacuation/plan
        [HttpGet("plan")]
        [Authorize]
        public async Task<IActionResult> GetAllPlans()
        {
            var result = await _evacuationService.GetAllPlansAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving evacuation plans", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET: api/evacuation/status
        [HttpGet("status")]
        [Authorize]
        public async Task<IActionResult> GetAllStatuses()
        {
            var result = await _evacuationService.GetAllStatusAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving evacuation statuses", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<EvacuationController>/plan/5
        [HttpGet("plan/zoneId/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPlanByIdAsync(int id)
        {
            var result = await _evacuationService.GetPlanByZoneIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving the evacuation plan", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // GET api/<EvacuationController>/status/5
        [HttpGet("status/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStatusByIdAsync(int id)
        {
            var result = await _evacuationService.GetStatusByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while retrieving the evacuation status", Error = result.Exception.Message });
                }
                else
                {
                    return NotFound(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // POST api/<EvacuationController>
        [HttpPost("plan")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> PostPlan(double distanceKm=100)
        {
            var result = await _evacuationService.CreatePlanAsync(distanceKm);
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while creating the evacuation plan", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // PUT api/<EvacuationController>/5
        [HttpPut("update")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> UpdateStatus()
        {
            var result = await _evacuationService.UpdateStatusByPlanAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while updating the evacuation status", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(result.Data);
        }

        // DELETE api/<EvacuationController>/5
        [HttpDelete("clear")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> ClearEvacuations()
        {
            var result = await _evacuationService.ClearAllPlanAndStatusAsync();
            if (!result.IsSuccess)
            {
                if (result.Exception != null)
                {
                    return StatusCode(500, new { Message = "An error occurred while clearing evacuations", Error = result.Exception.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            return Ok(new { Message = "All evacuation plans and statuses cleared successfully" });

        }
    }
}
