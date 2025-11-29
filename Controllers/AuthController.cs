using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.DTOs;
using OrderManagementApi.Models;
using OrderManagementApi.Services;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered");
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email // required by Identity
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered successfully");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                false
            );

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");

            var token = await _tokenService.CreateToken(user);

            return Ok(new
            {
                token
                // you can also add expiresIn = 3600 (seconds) if you want
            });
        }
    }
}
