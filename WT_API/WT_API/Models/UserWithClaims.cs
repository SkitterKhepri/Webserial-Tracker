namespace WT_API.Models
{
    public class UserWithClaims: User
    {
        public IList<string>? Claims { get; set; }
    }
}
