using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WT_API.Models
{
    public class User
    {
        [Key]
        public int id {  get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
  }
}
