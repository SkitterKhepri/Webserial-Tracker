using System.Security.Claims;

namespace WT_API.Models
{
    public class UserClaimsModel
    {
        public User User;
        public IList<Claim> Claims;
    }
}
