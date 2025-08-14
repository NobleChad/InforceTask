using InforceTask.Server.Constants;
using InforceTask.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InforceTask.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="model">User registration details including email, password, and role.</param>
        /// <returns>Status of the registration process.</returns>
        /// /// <response code="200">User registered successfully.</response>
        /// <response code="400">Validation failed or invalid role provided.</response>
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parsedRole = Enum.TryParse<Roles>(model.Role, true, out var role) ? role : Roles.User;

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _accountRepository.CreateUserAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _accountRepository.AddToRoleAsync(user, parsedRole.ToString());

            return Ok("User registered successfully");
        }



        /// <summary>
        /// Logs in a user and creates a session.
        /// </summary>
        /// <param name="model">User login details including email and password.</param>
        /// <returns>Status of the login process.</returns>
        /// /// <response code="200">Login successful.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="401">Invalid credentials.</response>
        [HttpPost("/api/sessions")]
        public async Task<IActionResult> CreateSession([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _accountRepository.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Sign in using repository (already validates password)
            var result = await _accountRepository.SignInAsync(user, model.Password);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { message = "Login successful" });
        }

        /// <summary>
        /// Logs out the current authenticated user.
        /// </summary>
        /// /// <response code="200">Logout successful.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpDelete("/api/sessions/current")]
        [Authorize]
        public async Task<IActionResult> DeleteCurrentSession()
        {
            await _accountRepository.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        /// <summary>
        /// Gets the currently authenticated user's email.
        /// </summary>
        /// <returns>User email if authenticated.</returns>
        /// /// <response code="200">Returns the authenticated user's email.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpGet("current")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            return Ok(email);
        }

        /// <summary>
        /// Checks if the current session is authenticated.
        /// </summary>
        /// <returns>Authentication status.</returns>
        /// /// <response code="200">Returns authentication status.</response>
        [HttpGet("/api/sessions/current")]
        public IActionResult GetCurrentSessionStatus()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            return Ok(new { isAuthenticated });
        }
    }

    public class RegisterModel : IValidatableObject
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        public string Role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Role))
            {
                bool roleIsValid = Enum.TryParse<Roles>(Role, ignoreCase: true, out _);
                if (!roleIsValid)
                {
                    var allowedRoles = string.Join(", ", Enum.GetNames(typeof(Roles)));
                    yield return new ValidationResult(
                        $"Role '{Role}' is invalid. Allowed roles: {allowedRoles}",
                        new[] { nameof(Role) }
                    );
                }
            }
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
