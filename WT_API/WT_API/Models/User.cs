using Microsoft.AspNetCore.Identity;

namespace WT_API.Models
{
    public class User : IdentityUser
    {
      public DateTime LastLoggedOn { get; set; }
    }
}
