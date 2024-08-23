namespace WT_API.Models
{
  public class Chapter
  {
    public int id { get; set; }
    public int serialId { get; set; }
    public string title { get; set; }
    public string link {  get; set; }

    public Chapter (int serialId, string title, string link)
    {
      this.serialId = serialId;
      this.title = title;
      this.link = link;
    }
  }
}
