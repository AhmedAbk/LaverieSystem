using Microsoft.AspNetCore.Mvc;
using Laverie.API.Services;
using Laverie.Domain.Entities;

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

        [HttpPost]
        public IActionResult Create([FromBody] Laundry laundry)
        {
            _laundryService.AddLaundry(laundry);
            return CreatedAtAction(nameof(GetById), new { id = laundry.id }, laundry);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Laundry laundry)
        {
            if (id != laundry.id) return BadRequest();
            _laundryService.UpdateLaundry(laundry);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _laundryService.DeleteLaundry(id);
            return NoContent();
        }
    }
}