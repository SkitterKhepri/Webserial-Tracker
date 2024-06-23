using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WT_API.Models;
using WT_API.Models.WT_API.Models;

namespace WT_API.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class ScrapingController : ControllerBase
    {
        private readonly ScrapingServiceHtmlAgilityPack _scrapingService;

        public ScrapingController(ScrapingServiceHtmlAgilityPack scrapingService)
        {
            _scrapingService = scrapingService;
        }

        [HttpGet("check-link")]
        public async Task<IActionResult> CheckLinkAtXPath(string url, string xpath)
        {
            var exists = await _scrapingService.CheckLinkAtXPathAsync(url, xpath);
            return Ok(new { linkExists = exists });
        }
    }

}
