using Microsoft.AspNetCore.Mvc;
using Laverie.API.Services;
using Laverie.Domain.Entities;
using Laverie.Domain.DTOS;

namespace Laverie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var proprietaire = _service.GetById(id);
            if (proprietaire == null) return NotFound();
            return Ok(proprietaire);
        }

        [HttpPost("register")]
        public IActionResult Create([FromBody] UserCreationDTO proprietaire)
        {
            try
            {
               
                _service.Create(proprietaire);

              
                return StatusCode(201, new
                {
                    message = "User created successfully!",
                    data = new
                    {
                        Name = proprietaire.Name,
                        Password = proprietaire.Password,
                        Email = proprietaire.Email,
                        Age = proprietaire.Age
                    }
                });
            }
            catch (Exception ex)
            {
                // Return 400 Bad Request with error details
                return BadRequest(new
                {
                    message = "Failed to create user.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] UserCreationDTO proprietaire)
        {
            try
            {
                _service.Update(proprietaire, id);
                return Ok(new
                {
                    Message = "User updated successfully.",
                    UserId = id,
                    UpdatedData = proprietaire
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Failed to update the user.",
                    Error = ex.Message
                });
            }
        }


        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = _service.Delete(id);

                if (isDeleted)
                {
                    return Ok(new
                    {
                        Message = "User deleted successfully.",
                        UserId = id
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Message = "User not found.",
                        UserId = id
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the user.",
                    Error = ex.Message
                });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO proprietaire)
        {
            try
            {
                var result = _service.Login(proprietaire);

                if (result == null)
                {
                    return Unauthorized(new
                    {
                        Message = "Invalid email or password.",
                        Status = "Failed"
                    });
                }

                return Ok(new
                {
                    Message = "Login successful.",
                    Status = "Success",
                    User = new
                    { 
                        Password = result.password,
                        Email = result.email,
                        
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred during login.",
                    Error = ex.Message
                });
            }
        }

    }
}
