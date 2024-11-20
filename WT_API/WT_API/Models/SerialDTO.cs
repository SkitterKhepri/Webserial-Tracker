namespace WT_API.Models
{
  public class SerialDTO : Serial
  {
    public IList<Chapter> chapters { get; set; }
    public Author author { get; set; }

    public SerialDTO(Serial serial)
    {
      this.id = serial.id;
      this.title = serial.title;
      this.status = serial.status;
      this.authorId = serial.authorId;
      this.firstCh = serial.firstCh;
      this.home = serial.home;
      this.nextChLinkXPath = serial.nextChLinkXPath;
      this.secondaryNextChLinkXPath = serial.secondaryNextChLinkXPath;
      this.otherNextChLinkXPaths = serial.otherNextChLinkXPaths;
      this.titleXPath = serial.titleXPath;
      this.reviewStatus = serial.reviewStatus;
    }

  }
}
