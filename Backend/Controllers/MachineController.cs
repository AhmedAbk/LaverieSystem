using Laverie.API.Infrastructure.repositories;
using Laverie.Domain.DTOS;
using Laverie.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Laverie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineController : ControllerBase
    {
        private readonly MachineRepo _machineRepo;

        public MachineController(MachineRepo machineRepo)
        {
            _machineRepo = machineRepo;
        }
         
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var machines = _machineRepo.GetAll();
                if (machines == null || machines.Count == 0)
                {
                    return NotFound(new { message = "No machines found." });
                }
                return Ok(new { message = "Machines retrieved successfully!", data = machines });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the machines.", error = ex.Message });
            }
        }
         
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var machine = _machineRepo.GetById(id);
                if (machine == null)
                {
                    return NotFound(new { message = $"Machine with ID {id} not found." });
                }
                return Ok(new { message = "Machine retrieved successfully! ", data = machine });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the machine.", error = ex.Message });
            }
        }
         
        [HttpPost("create")]
        public IActionResult Create([FromBody] MachineCreationDTO machine)
        {
            try
            {
                if (machine == null)
                {
                    return BadRequest(new { message = "Invalid machine data." });
                }

                var isCreated = _machineRepo.Create(machine);
                if (isCreated)
                {
                    return Ok(new { message = $"Machine of type '{machine.type}' created successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to create the machine." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the machine.", error = ex.Message });
            }
        }
         
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] MachineUpdateDTO machine)
        {
            try
            {
              

                bool isUpdated = _machineRepo.Update(machine, id);
                if (isUpdated)
                {
                    return Ok(new { message = $"Machine with ID {id} updated successfully! 🎉" });
                }
                else
                {
                    return NotFound(new { message = $"Machine with ID {id} not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the machine.", error = ex.Message });
            }
        }
         
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var isDeleted = _machineRepo.Delete(id);
                if (isDeleted)
                {
                    return Ok(new { message = $"Machine with ID {id} deleted successfully!" });
                }
                else
                {
                    return NotFound(new { message = $"Machine with ID {id} not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the machine.", error = ex.Message });
            }
        }
    }
}
