using MailKit.Net.Smtp;
using MimeKit;
using WT_API.Models;
using Google.Apis.Auth;
using System.Text.Json;
using MailKit.Security;
using Google.Apis.Auth.OAuth2;

namespace WT_API.Services
{
  public class EmailService
  {

    private readonly string _server = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _emailAddress = "webserialtracker@gmail.com";

    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string? _tokenUri;

    private readonly string _refreshToken = "1//04fvWErbATPqxCgYIARAAGAQSNwF-L9IrJviUFfWGatX1vR-BEwzrE4pcug-1Co5nLMKZdr2HdJI5DbgDsAuV7Qmwta-otIP7YJI";

    private readonly string pathToCredentials = "..\\assets\\cred\\secret.json";
    
    public EmailService()
    {
      var json = File.ReadAllText(pathToCredentials);
      var jsonObject = JsonDocument.Parse(json);

      var secret = jsonObject.RootElement.GetProperty("web");

      _clientId = secret.GetProperty("client_id").GetString();
      _clientSecret = secret.GetProperty("client_secret").GetString();
      _tokenUri = secret.GetProperty("token_uri").GetString();
    }


    private async Task<string> GetAccessTokenAsync()
    {
      using var httpClient = new HttpClient();
      var data = new Dictionary<string, string>
      {
        { "client_id", _clientId },
        { "client_secret", _clientSecret },
        { "refresh_token", _refreshToken },
        { "grant_type", "refresh_token" }
      };

      var response = await httpClient.PostAsync(_tokenUri, new FormUrlEncodedContent(data));
      response.EnsureSuccessStatusCode();

      var responseContent = await response.Content.ReadAsStringAsync();
      var json = JsonDocument.Parse(responseContent);
      if (json.RootElement.TryGetProperty("access_token", out var accessToken))
      {
        return accessToken.GetString();
      }

      throw new Exception("Access token not found in response.");
    }

    public async Task<bool> SendEmail(User user, string subject, string body)
    {
      MimeMessage email = new MimeMessage();
      email.From.Add(new MailboxAddress("Webserial Tracker", _emailAddress));
      email.To.Add(new MailboxAddress(user.UserName, user.Email));
      email.Subject = subject;
      email.Body = new TextPart("html") { Text = body };

      using SmtpClient smtpClient = new SmtpClient();
      smtpClient.CheckCertificateRevocation = false;

      try
      {
        await smtpClient.ConnectAsync(_server, _port, SecureSocketOptions.StartTls);
        var accessToken = await GetAccessTokenAsync();
        await smtpClient.AuthenticateAsync(new SaslMechanismOAuth2(_emailAddress, accessToken));
        await smtpClient.SendAsync(email);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
      finally
      {
        await smtpClient.DisconnectAsync(true);
      }
      return true;
    }


  }
}
