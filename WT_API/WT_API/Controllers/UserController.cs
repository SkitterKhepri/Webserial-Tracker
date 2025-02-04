using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WT_API.Models;
using WT_API.Services;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Web;

namespace WT_API.Controllers
{
  [Route("api/user")]
  [ApiController]
  //[Authorize(Roles = "SAdmin,Admin")]
  //[Authorize(Roles = "SAdmin")]
  //[Authorize]
  public class UserController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService = new EmailService();
    public UserController(IAuthService authService, ILogger<AuthenticationController> logger, UserManager<User> userManager, EmailService emailService)
    {
      _authService = authService;
      _logger = logger;
      _userManager = userManager;
      _emailService = emailService;

    }

    [HttpGet]
    [Route("userlist")]
    //TODO need this
    //[Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> Get()
    {
      var (status, message) = await _authService.UserList();
      return Ok(message);
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "SAdmin,Admin")]
    public async Task<IActionResult> Get(string id)
    {
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
    //TODO need this
    //[Authorize]
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

    [HttpHead]
    [Route("password/resetReq/{email}")]
    //TODO need this
    //[Authorize]
    public async Task<IActionResult> ResetPasswordRequest(string email)
    {
      User currentUser = await _userManager.FindByEmailAsync(email);
      if (currentUser == null)
      {
        //TODO have this, not reveal no email caveman
        //return StatusCode(StatusCodes.Status500InternalServerError, "Email service couldn't send email");
        return NotFound("User doesn't exist");
      }

      string urlToken = HttpUtility.UrlEncode(await _authService.GenerateResetToken(currentUser));
      string resetLink = $"http://localhost:4200/resetPassword?token={urlToken}&id={currentUser.Id}";
      string htmlTemplate = System.IO.File.ReadAllText("..\\assets\\email\\passReset.html");
      string emailBody = htmlTemplate
        .Replace("{{name}}", currentUser.UserName)
        .Replace("{{action_url}}", resetLink);

      bool success = await _emailService.SendEmail(currentUser, "Password Reset", emailBody);
      if (success)
      {
        return Ok("Password reset email sent successfully");
      }
      return StatusCode(StatusCodes.Status500InternalServerError, "Email service couldn't send email");
    }

    [HttpPost]
    [Route("password/reset/{id}")]
    //TODO need this
    //[Authorize]
    public async Task<IActionResult> ResetPassword(ResetPassDTO resetDTO, string id)
    {
      User currentUser = await _userManager.FindByIdAsync(id);
      if (currentUser == null)
      {
        return NotFound("Invalid user id");
      }
      var (result, message) = await _authService.ResetPassword(currentUser, resetDTO);

      if (result == 1)
      {
        return Ok(new {message = $"{message}" });
      }
      return BadRequest(message);
    }

    [HttpPut]
    [Route("changePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
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
            return BadRequest(new { message = $"{message}" });
          }
          return Ok(new { message = $"{message}" });
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


