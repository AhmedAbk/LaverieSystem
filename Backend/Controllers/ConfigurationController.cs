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
               
                int cycleId = await _configurationService.AddCycle(cycle);

                if (cycleId > 0)
                {
                    
                    return Ok(new { CycleId = cycleId });
                }
                else
                {
                    
                    return BadRequest("Failed to add the new cycle.");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }


}
