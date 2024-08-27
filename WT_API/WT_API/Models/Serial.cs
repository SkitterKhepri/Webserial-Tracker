using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WT_API.Models
{
  public class Serial
  {
    [Key]
    public int id { get; set; }
    public string title { get; set; }
    public string status { get; set; }
    public int? authorId { get; set; }
    public string firstCh { get; set; }
    public string home {  get; set; }
    public string nextChLinkXPath { get; set; }
    public string secondaryNextChLinkXPath { get; set; }
    public string otherNextChLinkXPaths { get; set; }
    public string titleXPath { get; set; }
    [DefaultValue(false)]
    public bool reviewStatus { get; set; }

    public Serial() { }
    public Serial(Serial newSer)
    {
      this.title = newSer.title;
      this.status = newSer.status;
      this.authorId = newSer.authorId;
      this.firstCh = newSer.firstCh;
      this.home = newSer.home;
      this.nextChLinkXPath = newSer.nextChLinkXPath;
      this.secondaryNextChLinkXPath = newSer.secondaryNextChLinkXPath;
      this.otherNextChLinkXPaths = newSer.otherNextChLinkXPaths;
      this.titleXPath = newSer.titleXPath;
    }
  }
}
