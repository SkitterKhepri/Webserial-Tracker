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

      while (true)
      {
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        int chapterCount = 0;
        //debug
        //Console.WriteLine(nextCH.InnerText);
        string chLink = "";
        HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
        //probably dont need this if?
        if (chTitleNode != null)
        {
          string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
          chLink = currentUrl;
          //TODO maybe this doesnt need to happen every time, just first maybe?
          if (!(_context.Chapters.Where(ch => ch.title == chTitle).Any()))
          {
            _context.Chapters.Add(new Chapter(serial.id, chTitle, chLink, serial.nextChLinkXPath, serial.secondaryNextChLinkXPath, serial.otherNextChLinkXPaths));
          }
        }
        else
        {
          var lastChapter = _context.Chapters.OrderByDescending(ch => ch.id).FirstOrDefault();
          lastChapter.isLastChapter = true;
          _context.Entry(lastChapter).Property(ch => ch.isLastChapter).IsModified = true;
          _context.SaveChanges();
          return (1, chapterCount);
        }
        if (nextCH == null)
        {
          try
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
          }
          catch (Exception ex)
          {
            nextCH = null;
          }
          if (nextCH == null)
          {
            try
            {
              nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.otherNextChLinkXPaths);
            }
            catch (Exception ex)
            {
              nextCH = null;
            }
            if (nextCH == null)
            {
              var lastChapter = _context.Chapters.OrderByDescending(ch => ch.id).FirstOrDefault();
              if (lastChapter == null)
              {
                Console.WriteLine("Should be zero: " + chapterCount);
                return (0, 0);
              }
              lastChapter.isLastChapter = true;
              _context.Entry(lastChapter).Property(ch => ch.isLastChapter).IsModified = true;
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


    //Overload 2
    public async Task<(int, int)> AddSerialChapters(Serial serial, string startUrl)
    {
      Console.WriteLine(startUrl);
      int chapterCount = 0;
      HttpClient client = new HttpClient();
      //had to add this bc of Ward -- docs
      Uri fullUri = new Uri(startUrl);
      client.BaseAddress = new Uri("https://" + fullUri.Host);
      HttpResponseMessage response = await client.GetAsync(startUrl);
      string pageContent = await CheckUrl(response, startUrl);
      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);
      HtmlNode nextCH;
      string nextCHURL;
      string currentUrl = startUrl;

      while (true)
      {
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);

        //debug
        //Console.WriteLine(nextCH.InnerText);
        HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
        //probably dont need this if?
        if (chTitleNode != null)
        {
          string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
          string chLink = currentUrl;
          //TODO maybe this doesnt need to happen every time, just first maybe?
          if (!(_context.Chapters.Where(ch => ch.title == chTitle).Any()))
          {
            chapterCount++;
            _context.Chapters.Add(new Chapter(serial.id, chTitle, chLink, serial.nextChLinkXPath, serial.secondaryNextChLinkXPath, serial.otherNextChLinkXPaths));
          }
        }
        else {
          var lastChapter = _context.Chapters.OrderByDescending(ch => ch.id).FirstOrDefault();
          if (lastChapter == null) {
            Console.WriteLine("Should be zero: " + chapterCount);
            return (0, 0);
          }
          lastChapter.isLastChapter = true;
          _context.Entry(lastChapter).Property(ch => ch.isLastChapter).IsModified = true;
          _context.SaveChanges();
          return (1, chapterCount);
        }
        if (nextCH == null)
        {
          try
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
          }
          catch (Exception ex)
          {
            nextCH = null;
          }
          if (nextCH == null)
          {
            try
            {
              nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.otherNextChLinkXPaths);
            }
            catch(Exception ex)
            {
              nextCH = null;
            }
            if (nextCH == null)
            {
              var lastChapter = _context.Chapters.OrderByDescending(ch => ch.id).FirstOrDefault();
              if (lastChapter == null)
              {
                Console.WriteLine("Should be zero: " + chapterCount);
                return (0, 0);
              }
              lastChapter.isLastChapter = true;
              _context.Entry(lastChapter).Property(ch => ch.isLastChapter).IsModified = true;
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


    public static async Task<string> CheckUrl(HttpResponseMessage response, string link)
    {

      string pageContent = "";
      HttpClient client2 = new HttpClient();
      //Uri fullUri = new Uri(link);

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

      if (true)
      {
          
      }

      foreach (Serial serial in serials)
      {
        Chapter? lastChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
        
        string lastLink;
        if (lastChapter == null)
        {
          lastLink = serial.firstCh;
        }
        else
        {
          if (lastChapter.isLastChapter == true && lastChapter.reviewStatus == true && (serial.status == SerialStatuses.Completed
              || serial.status == SerialStatuses.Abandoned))
          {
            continue;
          }
          lastLink = lastChapter.link;
        }
        var (result, chCount) = await AddSerialChapters(serial, lastLink);
        finalChCount += chCount;
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

      Chapter? lastChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
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
  }
}
