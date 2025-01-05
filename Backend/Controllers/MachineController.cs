using Laverie.API.Services;
using Laverie.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laverie.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly MachineService _machineService;

        // Inject MachineService into the controller
        public MachineController(MachineService machineService)
        {
            _machineService = machineService;
        }

        // GET: api/Machine
        [HttpGet]
        public async Task<ActionResult<List<Machine>>> GetAllMachines()
        {
            var machines = await _machineService.GetAllMachinesAsync();
            if (machines == null || machines.Count == 0)
            {
                return NotFound(); // Return 404 if no machines are found
            }
            return Ok(machines); // Return the list of machines
        }

        // GET: api/Machine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Machine>> GetMachine(int id)
        {
            var machine = await _machineService.GetMachineByIdAsync(id);
            if (machine == null)
            {
                return NotFound(); // Return 404 if machine not found
            }
            return Ok(machine); // Return the machine details
        }

        // POST: api/Machine
        [HttpPost]
        public async Task<ActionResult<Machine>> CreateMachine([FromBody] Machine machine)
        {
            if (machine == null)
            {
                return BadRequest("Machine data is invalid."); // Handle bad request if no data
            }

            var createdMachine = await _machineService.AddMachineAsync(machine);
            return CreatedAtAction(nameof(GetMachine), new { id = createdMachine.id }, createdMachine); // Return 201 Created with machine details
        }

        // PUT: api/Machine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachine(int id, [FromBody] Machine machine)
        {
            if (id != machine.id)
            {
                return BadRequest("Machine ID mismatch.");
            }

            var updated = await _machineService.UpdateMachineAsync(machine);
            if (!updated)
            {
                return NotFound(); // Return 404 if machine to update is not found
            }
            return NoContent(); // Return 204 No Content on success
        }

        // DELETE: api/Machine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var deleted = await _machineService.DeleteMachineAsync(id);
            if (!deleted)
            {
                return NotFound(); // Return 404 if machine to delete is not found
            }
            return NoContent(); // Return 204 No Content on success
        }
    }
}
