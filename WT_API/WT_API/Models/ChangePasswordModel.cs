using System.ComponentModel.DataAnnotations;

namespace WT_API.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }  
        
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } 
        
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
    }
}
