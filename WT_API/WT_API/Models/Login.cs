using System.ComponentModel.DataAnnotations;

namespace WT_API.Models
{
  public class Login
  {
    [Required(ErrorMessage = "Username is required")]
    [StringLength(10, ErrorMessage ="Username must be shorter than 10 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
    public string Password { get; set; }
  }
}
