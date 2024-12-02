namespace WT_API.Models
{
  public class AddedSerial :  Serial
  {
    public string authorName { get; set; }
    public IFormFile? banerUpload { get; set; } = null;
  }
}
