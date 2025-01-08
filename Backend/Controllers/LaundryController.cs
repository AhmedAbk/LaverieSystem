using Microsoft.AspNetCore.Mvc;
using Laverie.API.Services;
using Laverie.Domain.Entities;
using Laverie.Domain.DTOS;
using MySqlX.XDevAPI.Common;

namespace Laverie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaundryController : ControllerBase
    {
        private readonly LaundryService _laundryService;

        public LaundryController(LaundryService laundryService)
        {
            _laundryService = laundryService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_laundryService.GetAllLaundries());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var laundry = _laundryService.GetLaundryById(id);
            if (laundry == null) return NotFound();
            return Ok(laundry);
        }



        [HttpPost("create")]
        public IActionResult Create([FromBody] LaundryCreationDTO laundry)
        {
            try
            { 
                bool rowsAffected = _laundryService.AddLaundry(laundry);


                if (rowsAffected)
                {
                    return Ok($"Laundry '{laundry.nomLaverie}' created successfully! 🎉");
                }
                else
                {
                    return NotFound($" Laundry not found. Please check the ID and try again.");
                }

            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new
                {
                    Message = "An error occurred while creating the laundry.",
                    Status = "Failed",
                    Error = ex.Message
                });
            }
        }


        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] LaundryUpdateDTO laundry)
        {
            
        
            bool result = _laundryService.UpdateLaundry(laundry, id);
           

            if (result)
            {
                return Ok($"Laundry '{laundry.nomLaverie}' updated successfully");
            }
            else
            {
                return NotFound($" Laundry with ID {id} not found. Please check the ID and try again.");
            }
        }



        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        { 
            bool result = _laundryService.DeleteLaundry(id);

            if (result)
            {
                return Ok($" Laundry with ID {id} deleted successfully!");
            }
            else
            {
                return NotFound($" Laundry with ID {id} not found. Please verify the ID and try again.");
            }
        }

    }
}