using Microsoft.AspNetCore.Mvc;
using E_learning.Model.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_learning.Services;
using Microsoft.AspNetCore.Authorization;
using E_learning.DTO.Auth;
using E_learning.Repositories.Auth;
using E_learning.Enums;

namespace E_learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _configuration;
        private readonly GenerateID _generateID;

        public AuthController(IAuthRepository authRepo, IConfiguration configuration, GenerateID generateID)
        {
            _authRepo = authRepo;
            _configuration = configuration;
            _generateID = generateID;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.TryParse<UserRole>(registerDto.UserRole, true, out var userRoleEnum))
            {
                return BadRequest(new { Message = "Invalid user role provided. Valid roles are: Student, Lecturer, Admin." });
            }

            var userExists = await _authRepo.CheckUsernameExistsAsync(registerDto.Username);
            if (userExists)
            {
                return BadRequest(new { Message = "Username already exists" });
            }

            var user = new UserModel
            {
                UserID = _generateID.generateUserID(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                FullName = $"{registerDto.FirstName} {registerDto.LastName}",
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                UserRole = userRoleEnum
            };

            var result = await _authRepo.AddUserAsync(user);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "User creation failed! Please check server logs." });
            }

            return Ok(new { Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _authRepo.GetUserByUsernameAsync(loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user);
        
            return Ok(new { token });
        }

        private string GenerateJwtToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserID", user.UserID),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}