namespace WT_API.Models
{
  public class LoggedUser
  {
    public User User { get; set; }
    public string Token { get; set; }
    public string[] Roles { get; set; }
    public int[] Likes { get; set; }

    public LoggedUser(User user, string token, string[] roles, int[] likes)
    {
      User = user;
      Token = token;
      Roles = roles;
      Likes = likes;
    }
  }
}
