using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V123.WebAudio;
using System.Collections;
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
    
    public async Task<(int, int)> AddSerialChapters(Serial serial, string startUrl = null)
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

      string pageContent = await CheckUrl(response, startUrl);

      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(pageContent);

      HtmlNode? nextCH;
      string nextCHURL;
      string currentUrl = startUrl;
      int chapterCount = 0;
      Chapter lastChapter = null;

      string prevUrl = startUrl;
      bool firstLoop = true;

      List<HtmlNode> possibleNextNodes = new List<HtmlNode>();
      List<HtmlNode> wrongNextNodes = new List<HtmlNode>();


      Chapter addedCh = new Chapter();
      //TODO check if this is needed at all, on 1) new serial, and 2) serial missing 1 chapter
      //add first chapter
      //Chapter? lastStoredChapter = _context.Chapters.Where(ch => ch.serialId == serial.id).OrderByDescending(ch => ch.id).FirstOrDefault();

      //var (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);
      //if (success)
      //{
      //  lastChapter = addedCh;
      //  chapterCount++;
      //}
      //else
      //{
      //  return (0, chapterCount);
      //}

      //last working link's inner-text
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
        bool success = false;
        nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.nextChLinkXPath);
        if (nextCH != null)
        {
          if (wrongNextNodes.Contains(nextCH) || (nextCH.Attributes["href"].Value == prevUrl && !firstLoop))
          {
            nextCH = null;
          }
        }

        if (nextCH == null)
        {
          if (serial.secondaryNextChLinkXPath != null)
          {
            Console.WriteLine("Trying secondary XPath...");
            nextCH = htmlDoc.DocumentNode.SelectSingleNode(serial.secondaryNextChLinkXPath);
            if (nextCH != null)
            {
              if (wrongNextNodes.Contains(nextCH) || (nextCH.Attributes["href"].Value == prevUrl && !firstLoop))
              {
                nextCH = null;
              }
            }
          }

          if (nextCH == null)
          {
            if (serial.otherNextChLinkXPaths != null)
            {
              Console.WriteLine("Trying other XPaths...");
              foreach (string xpath in serial.otherNextChLinkXPaths)
              {
                nextCH = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                if (nextCH != null)
                {
                  if (!wrongNextNodes.Contains(nextCH) && nextCH.Attributes.Contains("href"))
                  {
                    if (nextCH.Attributes["href"].Value != prevUrl)
                    {
                      break;
                    }
                  }
                }
              }
            }
            if (nextCH == null)
            {
              if (nextChInnerText != null)
              {
                Console.WriteLine("Trying inner-text search...");
                List<HtmlNode> allNodes = htmlDoc.DocumentNode.Descendants().Where(nd => nd.Attributes.Contains("href")).ToList();
                foreach (HtmlNode node in allNodes)
                {
                  if (node.InnerText.Trim() == nextChInnerText.Trim())
                  {
                    possibleNextNodes.Add(node);
                    nextCH = node;
                    break;
                  }
                }
              }
              if (nextCH == null)
              {
                //adding last chapter
                //making sure last chapter is not added again
                if (currentUrl != startUrl)
                {
                  (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);
                }
                //we care about succes, in case the last page we navigated to wasnt a valid chapter (defined by finding chapter title at xpath, in AddChapter())
                if (success)
                {
                  lastChapter = addedCh;
                  chapterCount++;
                }
                if (lastChapter != null)
                {
                  lastChapter.isLastChapter = true;
                  lastChapter.reviewStatus = false;
                  Console.WriteLine($"{chapterCount} new chapter(s) added");
                  //adding dynamically found xpaths to serial
                  if (possibleNextNodes.Count > 0)
                  {
                    Serial modifiedSerial = await _context.Serials.FindAsync(serial.id);
                    List<string> possibleXPaths = new List<string>();
                    foreach (HtmlNode node in possibleNextNodes)
                    {
                      possibleXPaths.Add(node.XPath);
                    }
                    modifiedSerial.otherNextChLinkXPaths.Concat(possibleXPaths);
                  }
                  _context.SaveChanges();
                }
                else
                {
                  Console.WriteLine("No new chapters were added");
                }
                return (1, chapterCount);
              }
            }
          }
        }

        //retriveing next chapter page
        try
        {
          nextCHURL = nextCH.Attributes["href"].Value;
        }
        catch (NullReferenceException)
        {
          wrongNextNodes.Add(nextCH);
          continue;
        }
        Console.WriteLine(nextCHURL);
        nextChInnerText = nextCH.InnerText;
        response = await client.GetAsync(nextCHURL);
        string tbCheckedUrl = "";
        if (nextCHURL[0] == '/')
        {
          tbCheckedUrl = client.BaseAddress.ToString().TrimEnd(new Char[] { '/' }) + nextCHURL;
        }
        else
        {
          tbCheckedUrl = nextCHURL;
        }
        pageContent = await CheckUrl(response, tbCheckedUrl);
        currentUrl = tbCheckedUrl;

        //adding chapter
        //making sure if there are no more chapters, the last chapter is not added again (might be unnecessary, cant be fucked rn)
        if (currentUrl != startUrl)
        {
          (success, addedCh) = AddChapter(htmlDoc, serial, currentUrl);
        }
        //we care about succes, in case navigating to next page is successful, but that page isnt a valid chapter (defined by finding chapter title at xpath)
        if (success)
        {
          lastChapter = addedCh;
          chapterCount++;
        }
        else
        {
          lastChapter.isLastChapter = true;
          lastChapter.reviewStatus = false;
          //adding dynamically found xpaths to serial
          if (possibleNextNodes.Count > 0)
          {
            Serial modifiedSerial = await _context.Serials.FindAsync(serial.id);
            List<string> possibleXPaths = new List<string>();
            foreach (HtmlNode node in possibleNextNodes)
            {
              possibleXPaths.Add(node.XPath);
            }
            modifiedSerial.otherNextChLinkXPaths.Concat(possibleXPaths);
          }
          _context.SaveChanges();
          return (1, chapterCount);
        }

        //load next chapter page
        htmlDoc.LoadHtml(pageContent);
        firstLoop = false;
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
        bool rateLimited = true;
        int rateOfLimit = 1;

        while(rateLimited) {
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

        Chapter? lastChapter = _context.Chapters.OrderByDescending(ch => ch.id).FirstOrDefault(ch => ch.serialId == serial.id);
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
