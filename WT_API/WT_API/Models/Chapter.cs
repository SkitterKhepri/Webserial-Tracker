using System.ComponentModel;

namespace WT_API.Models
{
  public class Chapter
  {
    public int id { get; set; }
    public int serialId { get; set; }
    public string title { get; set; }
    public string link {  get; set; }
    public string nextChLinkXPath { get; set; }
    public string secondaryNextChLinkXPath { get; set; }
    public string otherNextChLinkXPaths { get; set; }
    [DefaultValue(false)]
    public bool isLastChapter { get; set; }
    [DefaultValue(false)]
    public bool reviewStatus { get; set; }

    public Chapter() { }

    public Chapter (int serialId, string title, string link, string xp1, string xp2, string xp3)
    {
      this.serialId = serialId;
      this.title = title;
      this.link = link;
      this.nextChLinkXPath = xp1;
      this.secondaryNextChLinkXPath = xp2;
      this.otherNextChLinkXPaths = xp3;

    }
  }
}
