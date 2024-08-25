namespace WT_API.Models
{
  public class Author
  {
    public int id {  get; set; }
    public string name { get; set; }

    public Author() { }
    public Author(string name)
    {
      this.name = name;
    }
  }
}
