using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WT_API.Models
{
  public class Chapter
  {
    public int id { get; set; }
    public int serialId { get; set; }
    public string title { get; set; }
    public string link {  get; set; }
    public DateTime added { get; set; }
    public string nextChLinkXPath { get; set; }
    public string? secondaryNextChLinkXPath { get; set; }
    public string? otherNextChLinkXPathsJSONString { get; set; }
    [DefaultValue(false)]
    public bool isLastChapter { get; set; }
    [DefaultValue(true)]
    public bool reviewStatus { get; set; }

    public Chapter() { }

    public Chapter (int serialId, string title, string link, DateTime added, string xp1, string xp2, List<string> xp3)
    {
      this.serialId = serialId;
      this.title = title;
      this.link = link;
      this.added = added;
      this.nextChLinkXPath = xp1;
      this.secondaryNextChLinkXPath = xp2;
      this.otherNextChLinkXPaths = xp3;
    }

    [NotMapped]
    public List<string>? otherNextChLinkXPaths
    {
      get => otherNextChLinkXPathsJSONString == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(otherNextChLinkXPathsJSONString);
      set => otherNextChLinkXPathsJSONString = JsonSerializer.Serialize(value);
    }
  }
}
