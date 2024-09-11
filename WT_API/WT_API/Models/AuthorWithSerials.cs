namespace WT_API.Models
{
  public class AuthorWithSerials : Author
  {
    public IList<Serial>? Serials { get; set; }
  }
}
