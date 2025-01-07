using Laverie.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laverie.Domain.Entities;
using Laverie.Domain.DTOS;

namespace Laverie.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ConfigurationService _configurationService;

        public ConfigurationController(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        // GET: api/configuration
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetConfiguration()
        {
            var configurations = await _configurationService.GetConfig();
            if (configurations == null || configurations.Count == 0)
            {
                return NotFound("No configurations found.");
            }
            return Ok(configurations);
        }


        [HttpPost("startMachine")]
        public async Task<IActionResult> StartMachine([FromBody] StartMachineRequest request)
        {
            var success = await _configurationService.starteMachineAsync(request.MachineId, request.IdCycle);

            if (!success)
            {
                return NotFound(new { Message = "Machine not found or failed to toggle status." });
            }

            return Ok(new { Message = "Machine status toggled successfully." });
        }


        [HttpPost("stopMachine")]
        public async Task<IActionResult> StopMachine([FromBody] int machineId)
        {
            var success = await _configurationService.stopeMachineAsync(machineId);

            if (!success)
            {
                return NotFound(new { Message = "Machine not found or failed to toggle status." });
            }

            return Ok(new { Message = "Machine status toggled successfully." });
        }


        [HttpPost("addCycle")]
        public async Task<IActionResult> AddCycle([FromBody] CycleCreationDTO cycle)
        {
            try
            {
                // Call AddCycleAsync to insert the cycle and get the ID
                int cycleId = await _configurationService.AddCycle(cycle);

                if (cycleId > 0)
                {
                    // If the cycle was successfully created, return the cycle ID
                    return Ok(new { CycleId = cycleId });
                }
                else
                {
                    // Return a failure response if cycle insertion failed
                    return BadRequest("Failed to add the new cycle.");
                }
            }
            catch (Exception ex)
            {
                // Return an error response if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }


}
