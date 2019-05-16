using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( UserForRegisterDto userForRegisterDto)
        {
            //TODO:validate user
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
            return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username,               
            };

            var createdUser = await _repo.Register(userToCreate,userForRegisterDto.Password);
            //return CreatedAtRoute()
            return StatusCode(201);
        }
    }
}