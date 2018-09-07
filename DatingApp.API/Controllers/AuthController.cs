using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            // var userToCreate = new User
            // {
            //     Username = userForRegisterDto.Username
            // };

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            //return StatusCode(201);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            // mapping the userFromRepo to the USerFromListDto for the photoUrl link
            var user = _mapper.Map<USerForListDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}

// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using DatingApp.API.Data;
// using DatingApp.API.Dtos;
// using DatingApp.API.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;

// namespace DatingApp.API.Controllers
// {
//     [Route("api/[controller]")]
//     public class AuthController : Controller
//     {
//         private readonly IAuthRepository _repo;
//         //Inject the reference of which ever Interface Repo you want to implement
//         public readonly IConfiguration _config;
//         public AuthController(IAuthRepository repo, IConfiguration config)
//         {
//             this._config = config;
//             _repo = repo;
//         }

//         [HttpPost("register")]
//         public async Task<IActionResult> Register([FromBody]UserForRegisterDto UserForRegisterDto)
//         {
//             //converting the username input to lowercase
//             if(!string.IsNullOrEmpty(UserForRegisterDto.UserName))
//             UserForRegisterDto.UserName = UserForRegisterDto.UserName.ToLower();

//             if (await _repo.UserExists(UserForRegisterDto.UserName))
//                 ModelState.AddModelError("Username", "Username Already Exist");
//             //validate request
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);


//             var userToCreate = new User
//             {
//                 Username = UserForRegisterDto.UserName
//             };

//             var createUser = await _repo.Register(userToCreate, UserForRegisterDto.Password);
//             return StatusCode(201);
//         }

//         [HttpPost("login")]
//         public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
//         {
//             //getting user information from _repo
//             var userFromRepo = await _repo.LoginAsync(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

//             if (userFromRepo == null)
//                 return Unauthorized();

//             //Now generating Tokens
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
//             //The following is the token discriptor which contains all the payload for the token
//             var tokenDescriptor = new SecurityTokenDescriptor
//             {
//                 Subject = new ClaimsIdentity(new Claim[]
//                 {
//                     new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
//                     new Claim(ClaimTypes.Name, userFromRepo.Username)
//                 }),

//                 Expires = DateTime.Now.AddDays(1),

//                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
//                                                     SecurityAlgorithms.HmacSha512Signature)
//             };

//             var token = tokenHandler.CreateToken(tokenDescriptor);
//             var tokenString = tokenHandler.WriteToken(token);

//             return Ok(new { tokenString });

//         }

//     }
// }