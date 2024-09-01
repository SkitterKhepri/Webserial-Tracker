using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WT_API.Models;
using WT_API.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WT_API.Controllers
{
    [Route("api/userlist")]
    [ApiController]
    [Authorize(Roles = "SAdmin,Admin")]
    //[Authorize(Roles = "SAdmin")]
    //[Authorize]
    public class UserListController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserManager<User> _userManager;
        public UserListController(IAuthService authService, ILogger<AuthenticationController> logger, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
           
            //var user = await _userManager.FindByIdAsync(u.id);
            var (status, message) = await _authService.UserList();
            return Ok(message);
        }

      
    }
}

