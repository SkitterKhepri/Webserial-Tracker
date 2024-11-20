using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V123.WebAudio;
using System.Net;
using WT_API.Data;
using WT_API.Models;

namespace WT_API.Services
{
  public class ScrapingService
  {
    private readonly Context _context;

    public ScrapingService(Context context)
    {
      _context = context;
    }
    //Overload 1
    public async Task<(int, int)> AddSerialChapters(Serial serial)
    {

      HttpClient client = new HttpClient();
      //had to add this bc of Ward -- docs
      Uri fullUri = new Uri(serial.firstCh);
      client.BaseAddress = new Uri("http://" + fullUri.Host);
      HttpResponseMessage response = await client.GetAsync(serial.firstCh);
      string pageContent = await CheckUrl(response, serial.firstCh);
      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);
      HtmlNode nextCH;
      string nextCHURL;
      string currentUrl = serial.firstCh;
      int chapterCount = 0;
      Chapter lastChapter = null;

      while (true)
      {
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        string chLink = "";
        HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
        if (chTitleNode != null)
        {
          string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
          chLink = currentUrl;
          chapterCount++;
          lastChapter = new Chapter(serial.id, chTitle, chLink, DateTime.Now, serial.nextChLinkXPath, serial.secondaryNextChLinkXPath, serial.otherNextChLinkXPaths);
          _context.Chapters.Add(lastChapter);
        }
        else
        {
          if(lastChapter == null)
          {
            Console.WriteLine($"{chapterCount} chapters added");
            _context.SaveChanges();
            return (1, chapterCount);
          }
          lastChapter.isLastChapter = true;
          lastChapter.reviewStatus = false;
          _context.SaveChanges();
          return (1, chapterCount);
        }
        if (nextCH == null)
        {
          try
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
          }
          catch (Exception)
          {
            nextCH = null;
          }
          if (nextCH == null)
          {
            try
            {
              nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.otherNextChLinkXPaths);
            }
            catch (Exception)
            {
              nextCH = null;
            }
            if (nextCH == null)
            {
              if (lastChapter != null)
              {
                lastChapter.isLastChapter = true;
                lastChapter.reviewStatus = false;
              }
              else
              {
                Console.WriteLine("No new chapters were added");
              }
              Console.WriteLine($"{chapterCount} chapters added");
              _context.SaveChanges();
              return (1, chapterCount);
            }
          }
        }
        else
        {
          nextCHURL = nextCH.Attributes["href"].Value;
          Console.WriteLine(nextCHURL);
          response = await client.GetAsync(nextCHURL);
          string tbCheckedUrl = nextCHURL;
          if (nextCHURL[0] == '/')
          {
            tbCheckedUrl = client.BaseAddress.ToString().TrimEnd(new Char[] { '/' }) + nextCHURL;
          }
          pageContent = await CheckUrl(response, tbCheckedUrl);
          currentUrl = tbCheckedUrl;
          htmlDoc.LoadHtml(pageContent);
        }
      }
    }


    //Overload 2 used when updating serials
    public async Task<(int, int)> AddSerialChapters(Serial serial, string startUrl)
    {
      Console.WriteLine(startUrl);
      HttpClient client = new HttpClient();
      //had to add this bc of Ward -- docs
      Uri fullUri = new Uri(startUrl);
      client.BaseAddress = new Uri("https://" + fullUri.Host);
      HttpResponseMessage response = await client.GetAsync(startUrl);
      string pageContent = await CheckUrl(response, startUrl);
      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);

      //HtmlNode nextChButton;
      HtmlNode nextCH;
      string nextCHURL;
      string currentUrl = startUrl;
      int chapterCount = 0;
      Chapter lastChapter = null;

      while (true)
      {
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        if (nextCH == null)
        {
          try
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
          }
          catch (Exception)
          {
            nextCH = null;
          }
          if (nextCH == null)
          {
            try
            {
              nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.otherNextChLinkXPaths);
            }
            catch(Exception)
            {
              nextCH = null;
            }
            if (nextCH == null)
            {
              if (lastChapter != null)
              {
                lastChapter.isLastChapter = true;
                lastChapter.reviewStatus = false;
              }
              else
              {
                Console.WriteLine("No new chapters were added");
              }
              Console.WriteLine($"{chapterCount} chapters added");
              _context.SaveChanges();
              return (1, chapterCount);
            }
          }
        }
        else
        {
          nextCHURL = nextCH.Attributes["href"].Value;
          Console.WriteLine(nextCHURL);
          response = await client.GetAsync(nextCHURL);
          string tbCheckedUrl = nextCHURL;
          if (nextCHURL[0] == '/')
          {
            tbCheckedUrl = client.BaseAddress.ToString().TrimEnd(new Char[] { '/' }) + nextCHURL;
          }
          pageContent = await CheckUrl(response, tbCheckedUrl);
          currentUrl = tbCheckedUrl;
          //adding chapter------
          string chLink = "";
          HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
          if (chTitleNode != null)
          {
            string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
            chLink = currentUrl;
            chapterCount++;
            lastChapter = new Chapter(serial.id, chTitle, chLink, DateTime.Now, serial.nextChLinkXPath, serial.secondaryNextChLinkXPath, serial.otherNextChLinkXPaths);
            _context.Chapters.Add(lastChapter);
          }
          else
          {
            if (lastChapter == null)
            {
              Console.WriteLine($"{chapterCount} chapters added");
              _context.SaveChanges();
              return (1, chapterCount);
            }
            lastChapter.isLastChapter = true;
            lastChapter.reviewStatus = false;
            _context.SaveChanges();
            return (1, chapterCount);
          }
          //--------
          htmlDoc.LoadHtml(pageContent);
        }
      }
    }


    public static async Task<string> CheckUrl(HttpResponseMessage response, string link)
    {

      string pageContent = "";
      HttpClient client2 = new HttpClient();

      if (response.IsSuccessStatusCode)
      {
        pageContent = await response.Content.ReadAsStringAsync();
      }
      //this one is for redirects, not a solution for every possible redirect <-TODO, but works for TGAB -- docs
      else if (response.StatusCode == HttpStatusCode.MovedPermanently)
      {
        Uri? redirectLink = response.Headers.Location;
        if (redirectLink != null)
        {
          HttpResponseMessage newResponse = await client2.GetAsync(redirectLink);
          newResponse.EnsureSuccessStatusCode();
          pageContent = await newResponse.Content.ReadAsStringAsync();
        }
      }
      //this one is handling 429 "too many requests" rate limiting issues, probably imperfectly, for Katalepsis -- docs
      else if (response.StatusCode == HttpStatusCode.TooManyRequests)
      {
        Console.WriteLine("Too many requests. Waiting...");

        if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string> values))
        {
          if (int.TryParse(values.FirstOrDefault(), out int retryAfterSeconds))
          {
            await Task.Delay(retryAfterSeconds * 1000);
          }
          else
          {
            await Task.Delay(1000);
          }
        }
        else
        {
          await Task.Delay(1000);
        }
        //response = await client2.GetAsync(request);
        response.EnsureSuccessStatusCode();
        pageContent = await response.Content.ReadAsStringAsync();
      }

      client2.Dispose();
      return pageContent;
    }

    //Update all serials
    public async Task<(int, int)> UpdateSerials()
    {
      int finalChCount = 0;
      List<Serial> serials = await _context.Serials.ToListAsync();

      foreach (Serial serial in serials)
      {
        if (serial.reviewStatus) {
          Chapter? lastChapter = _context.Chapters.Where(ch => ch.isLastChapter).OrderByDescending(ch => ch.id).FirstOrDefault();
          
          string lastLink;
          if (lastChapter == null)
          {
            lastLink = serial.firstCh;
          }
          else
          {
            if (lastChapter.reviewStatus == true && (serial.status == SerialStatuses.Completed || serial.status == SerialStatuses.Abandoned))
            {
              continue;
            }
            lastLink = lastChapter.link;
          }
          var (result, chCount) = await AddSerialChapters(serial, lastLink);
          finalChCount += chCount;
        }
      }
      return (1, finalChCount);
    }

    //Update one specififc serial
    public async Task<(int, int)> UpdateSerial(int id)
    {
      Serial? serial = await _context.Serials.FindAsync(id);

      if (serial == null)
      {
        return (0, 0);
      }
      if (serial.reviewStatus) {

        Chapter? lastChapter = _context.Chapters.Where(ch => ch.isLastChapter).OrderByDescending(ch => ch.id).FirstOrDefault();
        string lastLink;
        if (lastChapter == null)
        {
          lastLink = serial.firstCh;
        }
        else
        {
          lastLink = lastChapter.link;
        }

        return await AddSerialChapters(serial, lastLink);
      }
      else return (2, 0);
    }
  }
}
