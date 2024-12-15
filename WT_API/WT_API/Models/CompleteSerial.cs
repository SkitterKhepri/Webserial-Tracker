namespace WT_API.Models
{
  public class CompleteSerial :  Serial
  {
    public string authorName { get; set; }
    public IFormFile? banerUpload { get; set; } = null;
  }
}
