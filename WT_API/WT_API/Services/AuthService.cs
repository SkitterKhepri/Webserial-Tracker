using WT_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore;

namespace WT_API.Services
{
  public class AuthService : IAuthService
  {
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration _configuration;
    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
      this.userManager = userManager;
      this.roleManager = roleManager;
      _configuration = configuration;


    }

    public async Task<(int, List<UserWithClaims>)> UserList()
    {
      var userList = await userManager.Users.ToListAsync();
      List<UserWithClaims> userListWithClaims = new List<UserWithClaims>();
      for (int i = 0; i < userList.Count; i++)
      {
        UserWithClaims userWithClaims = new UserWithClaims();
        var serialized = JsonConvert.SerializeObject(userList[i]);
        userWithClaims = JsonConvert.DeserializeObject<UserWithClaims>(serialized);

        var roles = await userManager.GetRolesAsync(userList[i]);
        userWithClaims.Claims = roles;
        userListWithClaims.Add(userWithClaims);
      }
      return (1, userListWithClaims);
    }


    public async Task<(int, IList<string>)> UserClaim(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      var roles = await userManager.GetRolesAsync(user);
      return (1, roles);
    }

    public async Task<(int, string)> SetClaims(RolesModel model)
    {
      var user = await userManager.FindByIdAsync(model.Id);
      var roles = await userManager.GetRolesAsync(user);
      await userManager.RemoveFromRolesAsync(user, roles);

      foreach (var role in model.Roles)
      {
        if (!await roleManager.RoleExistsAsync(role))
          await roleManager.CreateAsync(new IdentityRole(role));

        if (await roleManager.RoleExistsAsync(role))
          await userManager.AddToRoleAsync(user, role);

      }
      return (1, "Claims/Roles set!");
    }

    public async Task<(int, string)> DeleteUser(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      if (user == null)
        return (0, "User not found");
      var result = await userManager.DeleteAsync(user);
      if (!result.Succeeded)
        return (0, "Delete user failed! Please check user details and try again.");
      return (1, "Deleted user successfully!");
    }
    public async Task<(int, string)> ChangePassword(ChangePasswordModel model)
    {
      var user = await userManager.FindByIdAsync(model.Id);
      if (user == null)
        return (0, "User not found");

      //await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

      await userManager.RemovePasswordAsync(user);
      var result = await userManager.AddPasswordAsync(user, model.NewPassword);
      if (!result.Succeeded)
        return (0, "Password update failed! Please check user details and try again.");
      return (1, "Password update successfully!");
    }

    public async Task<(int, string)> ChangeMyPassword(ChangePasswordModel model)
    {
      var user = await userManager.FindByIdAsync(model.Id);
      if (user == null)
        return (0, "User not found");

      var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

      //await userManager.RemovePasswordAsync(user);
      //var result = await userManager.AddPasswordAsync(user, model.NewPassword);
      if (!result.Succeeded)
        return (0, "Password update failed! Please check user details and try again.");
      return (1, "Password update successfully!");
    }
    public async Task<(int, string)> Update(UpdateModel model)
    {
      var user = await userManager.FindByIdAsync(model.Id);
      if (user == null)
        return (0, "User not found");
      user.Email = model.Email;
      user.UserName = model.Username;
      var result = await userManager.UpdateAsync(user);
      if (!result.Succeeded)
        return (0, "User update failed! Please check user details and try again.");
      return (1, "User update successfully!");
    }
    public async Task<(int, string)> Register(RegistrationModel model)
    {
      var userExists = await userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return (0, "User already exists");

      User user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var createUserResult = await userManager.CreateAsync(user, model.Password);
      if (!createUserResult.Succeeded)
        return (0, "User creation failed! Please check user details and try again.");

      /*if (!await roleManager.RoleExistsAsync(role))
          await roleManager.CreateAsync(new IdentityRole(role));

      if (await roleManager.RoleExistsAsync(role))
          await userManager.AddToRoleAsync(user, role);*/

      if (user.UserName.Equals("Admin"))
      {
        if (!await roleManager.RoleExistsAsync(UserRoles.SAdmin))
          await roleManager.CreateAsync(new IdentityRole(UserRoles.SAdmin));
        if (await roleManager.RoleExistsAsync(UserRoles.SAdmin))
          await userManager.AddToRoleAsync(user, UserRoles.SAdmin);
      }
      return (1, user.Id);
    }

    public async Task<(int, User?, string)> Login(LoginModel model)
    {
      var user = await userManager.FindByNameAsync(model.Username);
      user.LastLoggedOn = DateTime.Now;
      if (user == null)
        return (0, null, "Invalid username");
      if (!await userManager.CheckPasswordAsync(user, model.Password))
        return (0, null, "Invalid password");

      var userRoles = await userManager.GetRolesAsync(user);
      var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

      foreach (var userRole in userRoles)
      {
        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
      }
      string token = GenerateToken(authClaims);
      return (1, user, token);
    }


    private string GenerateToken(IEnumerable<Claim> claims)
    {
      var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
      var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Issuer = _configuration["JWTKey:ValidIssuer"],
        Audience = _configuration["JWTKey:ValidAudience"],
        Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
        SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
        Subject = new ClaimsIdentity(claims)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
