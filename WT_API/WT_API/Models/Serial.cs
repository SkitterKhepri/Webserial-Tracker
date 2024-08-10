using System.ComponentModel.DataAnnotations;

namespace WT_API.Models
{
  public class Serial
  {
    [Key]
    public int id { get; set; }
    public string title { get; set; }
    public string status { get; set; }
    public string author { get; set; }
    public string link { get; set; }
    public string nextChLinkXPath { get; set; }
    public string secondaryNextChLinkXPath { get; set; }
    public string otherNextChLinkXPaths { get; set; }
    public string titleXPath { get; set; }

    //public bool reviewStatus { get; set; }

    public Serial() { }
    public Serial(Serial newSer)
    {
      this.title = newSer.title;
      this.status = newSer.status;
      this.author = newSer.author;
      this.link = newSer.link;
      this.nextChLinkXPath = newSer.nextChLinkXPath;
      this.secondaryNextChLinkXPath = newSer.secondaryNextChLinkXPath;
      this.otherNextChLinkXPaths = newSer.otherNextChLinkXPaths;
      this.titleXPath = newSer.titleXPath;
    }
  }
}
