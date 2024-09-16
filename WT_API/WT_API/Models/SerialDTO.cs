namespace WT_API.Models
{
  public class SerialDTO
  {
    public Serial Serial { get; set; }
    public IList<Chapter> Chapters { get; set; }
    public Author Author { get; set; }

  }
}
