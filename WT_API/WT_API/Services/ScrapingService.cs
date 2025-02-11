using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V123.WebAudio;
using System.Collections;
using System.Net;
using System.Web;
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

    public async Task<(int, int)> AddSerialChapters(Serial serial, string? startUrl = null, string? prevChURL = null)
    {

      if (startUrl == null)
      {
        startUrl = serial.firstCh;
      }

      Console.WriteLine(startUrl);
      HttpClient client = new HttpClient();
      //had to add this bc of Ward -- docs
      Uri fullUri = new Uri(startUrl);
      client.BaseAddress = new Uri("https://" + fullUri.Host);
      HttpResponseMessage response = await client.GetAsync(startUrl);

      string pageContent = "";
      string? redirectedLink = null;

      (pageContent, redirectedLink) = await CheckUrl(response, startUrl);

      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);

      HtmlNode? nextCH = null;
      string nextCHURL;
      int chapterCount = 0;
      Chapter? lastChapter = null;

      string currentUrl = redirectedLink == null ? startUrl : redirectedLink;
      string? prevUrl = prevChURL == null ? null : prevChURL;

      string? prev2Url = null;
      string? prev3Url = null;
      bool firstLoop = true;

      List<string> possibleNextNodesXpath = new List<string>();
      List<string> wrongNextNodes = new List<string>();


      Chapter addedCh = new Chapter();
      bool success = false;

      //add first chapter
      Chapter? lastStoredChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();

      (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);
      if (success)
      {
        lastChapter = addedCh;
        chapterCount++;
      }
      else
      {
        Console.WriteLine("No new chapters were added");
        return (0, chapterCount);
      }

      //Next ch link's inner-text, later updated to last working links'
      string? nextChInnerText;
      try
      {
        HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        nextChInnerText = node?.InnerText;
      }
      catch(Exception)
      {
        nextChInnerText = null;
      }

      while (true)
      {
        success = false;
        foreach(string possibleNextChXPath in possibleNextNodesXpath)
        {
          string? nextUrl = htmlDoc.DocumentNode.SelectSingleNode(possibleNextChXPath)?.Attributes["href"]?.Value;
          if ((nextUrl != prevUrl && nextUrl != prev2Url && nextUrl != prev3Url) && !wrongNextNodes.Contains(possibleNextChXPath))
          {
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(possibleNextChXPath);
            break;
          }
        }

        if (nextCH == null)
        {
          Console.WriteLine("Trying primary nexch xpath...");
          nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
          if (nextCH != null)
          {
            if (wrongNextNodes.Contains(nextCH.XPath) || nextCH.Attributes["href"].Value == prevUrl || nextCH.Attributes["href"].Value == prev2Url || nextCH.Attributes["href"].Value == prev3Url)
            {
              nextCH = null;
            }
          }
          if (nextCH == null)
          {
            if (serial.secondaryNextChLinkXPath != null && serial.secondaryNextChLinkXPath != "")
            {
              if(serial.secondaryNextChLinkXPath == "") { Console.WriteLine("nem null..."); }
              Console.WriteLine("Trying secondary XPath...");
              nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
              if (nextCH != null)
              {
                if (wrongNextNodes.Contains(nextCH.XPath) && ((nextCH.Attributes["href"].Value == prevUrl && nextCH.Attributes["href"].Value == prev2Url && nextCH.Attributes["href"].Value == prev3Url) && !firstLoop))
                {
                  nextCH = null;
                }
              }
            }

            if (nextCH == null)
            {
              if (serial.otherNextChLinkXPaths != null && serial.otherNextChLinkXPaths.Count() != 0)
              {
                Console.WriteLine("Trying other XPaths...");
                foreach (string xpath in serial.otherNextChLinkXPaths)
                {
                  if(xpath != null) {
                    nextCH = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                  }
                  if (nextCH != null)
                  {
                    if (!wrongNextNodes.Contains(nextCH.XPath) && nextCH.Attributes.Contains("href"))
                    {
                      if (nextCH.Attributes["href"].Value != prevUrl && nextCH.Attributes["href"].Value != prev2Url && nextCH.Attributes["href"].Value != prev3Url &&
                      nextCH.Attributes["href"].Value != currentUrl)
                      {
                        Console.WriteLine($"Found next ch in otherXpaths, leads to: {HttpUtility.UrlDecode(nextCH.Attributes["href"].Value)}");
                        break;
                      }
                    }
                  }
                  nextCH = null;
                }
              }
              if (nextCH == null)
              {
                Console.WriteLine("Trying inner-text and rel search...");
                List<HtmlNode> allNodes = htmlDoc.DocumentNode.Descendants().Where(nd => nd.Attributes.Contains("href")).ToList();
                foreach (HtmlNode node in allNodes)
                {
                  if (node.InnerText?.Trim() == nextChInnerText?.Trim() || node.Attributes["rel"]?.Value == "next")
                  {
                    if(node.Attributes["href"].Value != prevUrl && node.Attributes["href"].Value != prev2Url && node.Attributes["href"].Value != prev3Url)
                    {
                      possibleNextNodesXpath.Add(node.XPath);
                      nextCH = node;
                      break;
                    }
                  }
                } 
                if (nextCH == null)
                {
                  //adding last chapter before returning
                  //making sure first chapter is not added again
                  if (!firstLoop)
                  {
                    (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);
                    //we care about succes here, in case the last page we navigated to wasnt a valid chapter (defined by finding chapter title at xpath, in AddChapter())
                    if (success)
                    {
                      lastChapter = addedCh;
                      chapterCount++;
                    }
                    else { Console.WriteLine("Adding chapter failed - 1"); }
                  }
                  lastChapter.isLastChapter = true;
                  lastChapter.reviewStatus = false;
                  Console.WriteLine($"{chapterCount} new chapter(s) added");
                  //adding dynamically found xpaths to serial
                  if (possibleNextNodesXpath.Count > 0)
                  {
                    Serial modifiedSerial = await _context.Serials.FindAsync(serial.id);
                    (modifiedSerial.otherNextChLinkXPaths ??= new List<string>()).AddRange(possibleNextNodesXpath);
                  }
                  _context.SaveChanges();
                  return (1, chapterCount);
                }
              }
            }
          }
        }

        //retriveing next chapter page
        try
        {
          nextCHURL = HttpUtility.UrlDecode(nextCH.Attributes["href"].Value);
        }
        catch (NullReferenceException)
        {
          wrongNextNodes.Add(nextCH.XPath);
          continue;
        }
        Console.WriteLine($"Next ch url found: ------{nextCHURL}");
        nextChInnerText = nextCH.InnerText;
        response = await client.GetAsync(nextCHURL);
        string nextFullUrl = "";
        if (nextCHURL[0] == '/')
        {
          nextFullUrl = client.BaseAddress.ToString().TrimEnd(new Char[] { '/' }) + nextCHURL;
        }
        else
        {
          nextFullUrl = nextCHURL;
        }

        (pageContent, redirectedLink) = await CheckUrl(response, nextFullUrl);
        nextFullUrl = redirectedLink != null ? redirectedLink : nextFullUrl;
        if(redirectedLink != null) { Console.WriteLine("Redirect happened"); }
        //After the first loop, adding current chapter
        if (!firstLoop)
        {
          (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);

          //we care about success, in case navigating to next page is successful, but that page isnt a valid chapter (defined by finding chapter title at xpath, in AddChapter)
          if (success)
          {
            lastChapter = addedCh;
            chapterCount++;
          }
          else
          {
            Console.WriteLine("Adding chapter failed - 2");
            lastChapter.isLastChapter = true;
            lastChapter.reviewStatus = false;
            //adding dynamically found xpaths to serial
            if (possibleNextNodesXpath.Count > 0)
            {
              Serial modifiedSerial = await _context.Serials.FindAsync(serial.id);
              (modifiedSerial.otherNextChLinkXPaths ??= new List<string>()).AddRange(possibleNextNodesXpath);
            }
            _context.SaveChanges();
            return (1, chapterCount);
          }
        }
        
        //load next chapter page
        htmlDoc.LoadHtml(pageContent);
        //really fucking hate this, but need it b/c of twig 5-9
        prev3Url = prev2Url;
        prev2Url = prevUrl;
        prevUrl = currentUrl;
        currentUrl = nextFullUrl;
        firstLoop = false;
        redirectedLink = null;
        nextCH = null;
        if(currentUrl == prevUrl)
        {
          Console.WriteLine("Not moved, prev and current urls match, terminating...");
          return (0, 999);
        }
      }
    }


    public static async Task<(string, string?)> CheckUrl(HttpResponseMessage response, string link)
    {
      string pageContent = "";
      HttpClient client2 = new HttpClient();
      int retrys = 0;
      string? redirectLink = null;

      while (true)
      {
        if(retrys > 10) { break; }
        if (response.IsSuccessStatusCode)
        {
          pageContent = await response.Content.ReadAsStringAsync();
          break;
        }
        //this one is for redirects, works for TGAB -- docs
        else if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
        {
          Uri? redirectLinkUri = response.Headers.Location;
          if (redirectLinkUri != null)
          {
            HttpResponseMessage newResponse = await client2.GetAsync(redirectLinkUri);
            response = newResponse;
            redirectLink = redirectLinkUri.ToString();
            retrys++;
            continue;
          }
        }
        //this one is handling 429 "too many requests" rate limiting issues, probably imperfectly, for Katalepsis -- docs
        else if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
          Console.WriteLine("Too many requests. Waiting...");
          bool rateLimited = true;
          int rateOfLimit = 1;

          while (rateLimited)
          {
            if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string> values))
            {
              if (int.TryParse(values.FirstOrDefault(), out int retryAfterSeconds))
              {
                Console.WriteLine("Rate limit exceeded, retrying after provided amount of time...");
                await Task.Delay(retryAfterSeconds * 1000);
              }
              else
              {
                Console.WriteLine($"invalid Retry-After value, waiting {rateOfLimit} seconds");
                await Task.Delay(1000 * rateOfLimit);
                rateOfLimit++;
              }
            }
            else
            {
              Console.WriteLine($"no retry-after, waiting {rateOfLimit} second");
              await Task.Delay(1000 * rateOfLimit);
              rateOfLimit++;
            }
            response = await client2.GetAsync(link);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
              Console.WriteLine("Still rate-limited. Waiting...");
            }
            else
            {
              rateLimited = false;
            }
          }

          try
          {
            response.EnsureSuccessStatusCode();
            pageContent = await response.Content.ReadAsStringAsync();
            break;
          }
          catch
          {
            retrys++;
            continue;
          }
        }
      }
      client2.Dispose();
      if(redirectLink != null)
      {
        Console.WriteLine("redirect happened!");
      }
      return (pageContent, redirectLink);
    }

    //Update all serials
    public async Task<(int, int)> UpdateSerials()
    {
      int finalChCount = 0;
      List<Serial> serials = await _context.Serials.ToListAsync();

      foreach (Serial serial in serials)
      {
        if (serial.reviewStatus) {
          Chapter? lastChapter = _context.Chapters.Where(ch => ch.isLastChapter == true && ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
          Chapter? prevChapter = _context.Chapters.Where(ch => ch.isLastChapter == false && ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();

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
          var (result, chCount) = await AddSerialChapters(serial, lastLink, prevChapter?.link);
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

        Chapter? lastChapter = _context.Chapters.Where(ch => ch.isLastChapter == true && ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();
        Chapter? prevChapter = _context.Chapters.Where(ch => ch.isLastChapter == false && ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();

        string lastLink;
        if (lastChapter == null)
        {
          lastLink = serial.firstCh;
        }
        else
        {
          lastLink = lastChapter.link;
        }

        return await AddSerialChapters(serial, lastLink, prevChapter?.link);
      }
      else return (2, 0);
    }

    //Add chapter, as chapter of serial
    private (bool, Chapter) AddChapter(HtmlDocument htmlDoc, Serial serial, string currentUrl)
    {
      Chapter tbAddedCh = new Chapter();
      try
      {
        HtmlNode chTitleNode = htmlDoc.DocumentNode.SelectSingleNode(serial.titleXPath);
        string chTitle = WebUtility.HtmlDecode(chTitleNode.InnerText);
        string chLink = currentUrl;
        tbAddedCh = new Chapter(serial.id, chTitle, chLink, DateTime.Now, serial.nextChLinkXPath, serial.secondaryNextChLinkXPath, serial.otherNextChLinkXPaths);
        _context.Chapters.Add(tbAddedCh);
        return (true, tbAddedCh);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error adding chapter at {currentUrl}. Exception: {ex}");
        return (false, tbAddedCh);
      }
    }
  }
}
