namespace WT_API.Models
{
  public class NewSerial
  {
    public string title { get; set; }
    public string authorName { get; set; }
    public string home {  get; set; }
    public string firstCh {  get; set; }
    public SerialStatuses status { get; set; }
    public IFormFile? banerUpload { get; set; } = null;

    public NewSerial() { }
  }
}
