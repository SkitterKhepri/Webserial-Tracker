using WT_API.Models;
using WT_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using WT_API.Data;


namespace WT_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly Context _context;


    public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger, Context context)
    {
      _authService = authService;
      _logger = logger;
      _context = context;
    }


    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
      try
      {
        if (!ModelState.IsValid)
          return BadRequest("Invalid payload");
        var (status, user, message) = await _authService.Login(model);
        if (status == 0)
          return BadRequest(message);
        var (success, rolesList) = await _authService.UserClaim(user.Id);
        string[] roles = rolesList.ToArray();
        int[] likes = _context.LikedSerials.Where(l => l.userId == user.Id).Select(l => l.serialId).ToArray();
        return Ok(new LoggedUser(user, message, roles, likes));
      }
      catch (Exception ex)
      {
        _logger.LogError("Some error has occured" + ex.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }



    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegistrationModel model)
    {
      try
      {
        if (!ModelState.IsValid)
          return BadRequest("Invalid payload");
        var (status, message) = await _authService.Register(model);
        if (status == 0)
        {
          Console.WriteLine(BadRequest(message));
          return BadRequest(message);
        }
        if (model.Username != "Admin")
        {
          await _authService.SetClaims(new RolesModel(message, new string[] { UserRoles.User }));
        }
        return CreatedAtAction(nameof(Register), new { model.Username });

      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }

    //[HttpPost]
    //[Route("update")]
    //[Authorize]
    //public async Task<IActionResult> Update(UpdateModel model)
    //{
    //    try
    //    {
    //        var currentUserName = HttpContext.User.Identity.Name;

    //        var user = await _userManager.FindByIdAsync(model.Id);
    //        var currentUser = await _userManager.FindByNameAsync(currentUserName);
    //        var roles = await _userManager.GetRolesAsync(currentUser);
    //        if (user.UserName.Equals(currentUserName) || roles.IndexOf("SAdmin") != -1)
    //        {
    //            if (!ModelState.IsValid)
    //                return BadRequest("Invalid payload");
    //            var (status, message) =
    //                await _authService.Update(model);
    //            if (status == 0)
    //            {
    //                return BadRequest(message);
    //            }
    //            return Ok(message);
    //        }
    //        return BadRequest("Unauthorized");

    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message);
    //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    //    }
    //}  


  }
}
