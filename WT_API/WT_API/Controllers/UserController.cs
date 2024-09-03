using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WT_API.Models;
using WT_API.Services;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WT_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "SAdmin,Admin")]
    //[Authorize(Roles = "SAdmin")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserManager<User> _userManager;
        public UserController(IAuthService authService, ILogger<AuthenticationController> logger, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;

        }

        [HttpGet]
        [Route("userlist")]
        [Authorize(Roles = "SAdmin,Admin")]
        public async Task<IActionResult> Get()
        {

            //var user = await _userManager.FindByIdAsync(u.id);
            var (status, message) = await _authService.UserList();
            return Ok(message);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "SAdmin,Admin")]
        public async Task<IActionResult> Get(string id)
        {
            //var currentUserName = HttpContext.User.Identity.Name;
            //var currentUser = await _userManager.FindByNameAsync(currentUserName);
            var currentUser = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(currentUser);
           
            UserWithClaims userWithClaims = new UserWithClaims();
            var serialized = JsonConvert.SerializeObject(currentUser);
            userWithClaims = JsonConvert.DeserializeObject<UserWithClaims>(serialized);
                   
            userWithClaims.Claims = roles;
            return Ok(userWithClaims);            
                    
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "SAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByIdAsync(id);
            var currentUser = await _userManager.FindByNameAsync(currentUserName);
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!user.UserName.Equals(currentUserName))
            {
                var (status, message) =
                 await _authService.DeleteUser(id);
                if (status == 0)
                {
                    return BadRequest(message);
                }
                return Ok(new { message });
            }
            return BadRequest("Unauthorized");

        }
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(UpdateModel model)
        {
            try
            {
                var currentUserName = HttpContext.User.Identity.Name;

                var user = await _userManager.FindByIdAsync(model.Id);
                var currentUser = await _userManager.FindByNameAsync(currentUserName);
                var roles = await _userManager.GetRolesAsync(currentUser);
                if (user.UserName.Equals(currentUserName) || roles.IndexOf("SAdmin") != -1)
                {
                    if (!ModelState.IsValid)
                        return BadRequest("Invalid payload");
                    var (status, message) =
                        await _authService.Update(model);
                    if (status == 0)
                    {
                        return BadRequest(message);
                    }
                    return Ok(new { message });
                }
                return BadRequest("Unauthorized");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("changePassword/{id}")]
        [Authorize(Roles = "SAdmin,Admin")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model, string id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var currentUserName = HttpContext.User.Identity.Name;

                var user = await _userManager.FindByIdAsync(id);
                var currentUser = await _userManager.FindByNameAsync(currentUserName);
                var roles = await _userManager.GetRolesAsync(currentUser);
                if (user.UserName.Equals(currentUserName) || roles.IndexOf("SAdmin") != -1)
                {

                    var (status, message) =
                    await _authService.ChangePassword(model);
                    if (status == 0)
                    {
                        return BadRequest(message);
                    }
                    return Ok(message);
                }
                return BadRequest("Unauthorized");
                //return CreatedAtAction(nameof(Register), model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("changeMyPassword")]
        [Authorize]
        public async Task<IActionResult> ChangeMyPassword(ChangePasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var currentUserName = HttpContext.User.Identity.Name;

                var user = await _userManager.FindByIdAsync(model.Id);
                var currentUser = await _userManager.FindByNameAsync(currentUserName);
                var roles = await _userManager.GetRolesAsync(currentUser);
                if (user.UserName.Equals(currentUserName) || roles.IndexOf("SAdmin") != -1)
                {

                    var (status, message) =
                    await _authService.ChangePassword(model);
                    if (status == 0)
                    {
                        return BadRequest(message);
                    }
                    return Ok(message);
                }
                return BadRequest("Unauthorized");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}


