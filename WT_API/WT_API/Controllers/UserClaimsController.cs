using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WT_API.Models;
using WT_API.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WT_API.Controllers
{ 
    [Route("api/userClaims")]
    [ApiController]
    [Authorize(Roles = "SAdmin")]
    [Authorize]
    public class UserClaimsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserManager<User> _userManager;

        public UserClaimsController(IAuthService authService, ILogger<AuthenticationController> logger, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
        }

              
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            
            var currentUserName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByIdAsync(id);
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (user.UserName.Equals(currentUserName) || roles.IndexOf("SAdmin") != -1)
            {
                var (status, message) = await _authService.UserClaim(id);
                return Ok(message);
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "SAdmin")]
        public async Task<IActionResult> Post(RolesModel model)
        {

            var (status, message) = await _authService.SetClaims(model);
            return Ok(message);
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

    }
}

