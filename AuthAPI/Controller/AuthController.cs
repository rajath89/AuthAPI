using AuthAPI.Models;
using AuthAPI.Models.Request;
using AuthAPI.Models.Response;
using AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controller;

[ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(CreateErrorResponse("Invalid request data"));
                }
                
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(CreateErrorResponse("Invalid credentials"));
                }
                
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                {
                    return BadRequest(CreateErrorResponse("Invalid credentials"));
                }
                
                var token = _jwtTokenService.GenerateToken(user, request.TraceId);
                
                return Ok(new AuthResponse
                {
                    Completion = new Completion { Code = "SUCCESS", Value = "0" },
                    Token = token,
                    TokenType = "SSOTOKEN"
                });
            }
            catch (Exception)
            {
                return BadRequest(CreateErrorResponse("System error occurred"));
            }
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(CreateErrorResponse("Invalid request data"));
                }
                
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(CreateErrorResponse("User already exists"));
                }
                
                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedDate = DateTime.UtcNow
                };
                
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(CreateErrorResponse("Registration failed"));
                }
                
                var token = _jwtTokenService.GenerateToken(user, request.TraceId);
                
                return Ok(new AuthResponse
                {
                    Completion = new Completion { Code = "SUCCESS", Value = "0" },
                    Token = token,
                    TokenType = "SSOTOKEN"
                });
            }
            catch (Exception)
            {
                return BadRequest(CreateErrorResponse("System error occurred"));
            }
        }
        
        private ErrorResponse CreateErrorResponse(string description)
        {
            return new ErrorResponse
            {
                ErrorInfo = new List<ErrorInfo>
                {
                    new ErrorInfo
                    {
                        Code = "system_error",
                        Description = "We're sorry, but we can't complete your request at this time. Please try again later."
                    }
                }
            };
        }
    }