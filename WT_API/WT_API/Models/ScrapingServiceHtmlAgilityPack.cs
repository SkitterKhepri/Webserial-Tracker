using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace WT_API.Models
{

    namespace WT_API.Models
    {
        public class ScrapingServiceHtmlAgilityPack
        {
            private readonly HttpClient _httpClient;

            public ScrapingServiceHtmlAgilityPack(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<bool> CheckLinkAtXPathAsync(string url, string xpath)
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var pageContents = await response.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageContents);

                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                return node != null;
            }
        }
    }
}
