namespace WT_API.Models
{
  public class RolesModel
  {
    public string Id { get; set; }
    public string[] Roles { get; set; }

    public RolesModel(string id, string[] roles) {
      this.Id = id;
      this.Roles = roles;
    }
  }
}
