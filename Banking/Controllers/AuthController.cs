using Banking.DTO;
using Banking.Entity;
using Banking.Services;
using Banking.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IAccountRepository _accountRepository;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService, IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new LoginResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid email or password."
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new LoginResponseDto
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid email or password."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);
            var isAdmin = roles.Contains("Admin");

            return Ok(new LoginResponseDto
            {
                IsSuccessful = true,
                Token = token,
                IsAdmin = isAdmin
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            var user = new User
            {
                UserName = userRegisterDto.Email,
                Email = userRegisterDto.Email,
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRegisterDto.Password!);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new RegistrationResponseDto
                {
                    IsSuccessfulRegistration = false,
                    Errors = errors
                });
            }

            // Создаем банковский аккаунт для пользователя
            var account = new Account
            {
                UserId = user.Id,
                AccountNumber = Guid.NewGuid().ToString("N")[..12].ToUpper(), // Генерируем короткий номер счета
                Balance = userRegisterDto.InitialBalance > 0 ? userRegisterDto.InitialBalance : 0
            };

            await _accountRepository.CreateAccountAsync(account);

            return Ok(new RegistrationResponseDto
            {
                IsSuccessfulRegistration = true,
                AccountNumber = account.AccountNumber
            });
        }

        [HttpPost("assign-admin")]
        public async Task<IActionResult> AssignAdmin([FromBody] AssignRoleDto assignRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }

            var user = await _userManager.FindByEmailAsync(assignRoleDto.Email);
            if (user == null)
            {
                return NotFound(new { Message = $"User with email '{assignRoleDto.Email}' not found." });
            }

            var roleExists = await _userManager.GetRolesAsync(user);
            if (roleExists.Contains(assignRoleDto.Role))
            {
                return BadRequest(new { Message = $"User already has the '{assignRoleDto.Role}' role." });
            }

            var result = await _userManager.AddToRoleAsync(user, assignRoleDto.Role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = $"Role '{assignRoleDto.Role}' successfully assigned to user '{assignRoleDto.Email}'." });
        }

        [HttpGet("user-roles/{email}")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { Message = $"User with email '{email}' not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new 
            { 
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            });
        }
    }
}
