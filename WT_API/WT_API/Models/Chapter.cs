namespace WT_API.Models
{
  public class Chapter
  {
    public int id { get; set; }
    public int serialId { get; set; }
    public string title { get; set; }

    public Chapter (int serialId, string title)
    {
      this.serialId = serialId;
      this.title = title;
    }
  }
}
