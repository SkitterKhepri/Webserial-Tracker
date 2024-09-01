using WT_API.Models;
using System.Security.Claims;

namespace WT_API.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Register(RegistrationModel model);
        Task<(int, string)>Update(UpdateModel model); 
        Task<(int, string)>ChangePassword(ChangePasswordModel model);
        Task<(int, string)>ChangeMyPassword(ChangePasswordModel model);
        Task<(int, string)>DeleteUser(string id);
        Task<(int, User?, string)> Login(LoginModel model);

        Task<(int, List<UserWithClaims>)> UserList();
       
        Task<(int, IList<string>)> UserClaim(string id);

        Task<(int, string)> SetClaims(RolesModel id);



    }
}
