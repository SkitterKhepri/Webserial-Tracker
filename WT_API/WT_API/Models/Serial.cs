using HtmlAgilityPack;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json;

namespace WT_API.Models
{
  public class Serial
  {
    [Key]
    public int id { get; set; }
    public string title { get; set; }
    public SerialStatuses? status { get; set; }
    public int authorId { get; set; }
    public string? firstCh { get; set; }
    public string home {  get; set; }
    public string? bannerPath { get; set; }
    public string? nextChLinkXPath { get; set; }
    public string? secondaryNextChLinkXPath { get; set; }
    public string? otherNextChLinkXPathsJSONString { get; set; }
    public string? titleXPath { get; set; }
    public string? description { get; set; }
    [DefaultValue(false)]
    public bool reviewStatus { get; set; }

    public Serial() { }

    public Serial(ProposedSerial newSer)
    {
      this.title = newSer.title;
      this.status = newSer.status;
      this.firstCh = newSer.firstCh;
      this.home = newSer.home;
    }
    //public Serial(CompleteSerial newSer)
    //{
    //  this.title = newSer.title;
    //  this.status = newSer.status;
    //  this.firstCh = newSer.firstCh;
    //  this.home = newSer.home;
    //  this.nextChLinkXPath = newSer.nextChLinkXPath;
    //  this.secondaryNextChLinkXPath = newSer.secondaryNextChLinkXPath;
    //  this.otherNextChLinkXPaths = newSer.otherNextChLinkXPaths;
    //  this.titleXPath = newSer.titleXPath;
    //  this.reviewStatus = newSer.reviewStatus;
    //}

    public void CompleteSerialMapper(CompleteSerial newSerial)
    {
      this.title = newSerial.title;
      this.status = newSerial.status;
      this.firstCh = newSerial.firstCh;
      this.home = newSerial.home;
      this.nextChLinkXPath = newSerial.nextChLinkXPath;
      this.secondaryNextChLinkXPath = newSerial.secondaryNextChLinkXPath;
      this.otherNextChLinkXPaths = newSerial.otherNextChLinkXPaths;
      this.titleXPath = newSerial.titleXPath;
      this.description = newSerial.description;
      this.reviewStatus = newSerial.reviewStatus;
    }

    [NotMapped]
    public List<string>? otherNextChLinkXPaths
    {
      get => otherNextChLinkXPathsJSONString == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(otherNextChLinkXPathsJSONString);
      set => otherNextChLinkXPathsJSONString = JsonSerializer.Serialize(value);
    }
  }
}
