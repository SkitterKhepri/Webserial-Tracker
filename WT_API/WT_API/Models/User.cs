using System.ComponentModel.DataAnnotations;

namespace WT_API.Models
{
    public class User
    {
        [Key]
        public string id {  get; set; }
        public string userName { get; set; }
        public string email { get; set; }
    }
}
