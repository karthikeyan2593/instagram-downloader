using instagram_downloader.Models;
using instagram_downloader.Services;
using Microsoft.AspNetCore.Mvc;

namespace instagram_downloader.Controllers
{
    [ApiController]
    [Route("api/reels")]
    public class ReelController : ControllerBase
    {
        private readonly ReelService _reelService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReelController(
            ReelService reelService,
            IHttpClientFactory httpClientFactory)
        {
            _reelService = reelService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { success = true, message = "API Running!" });
        }

        // Preview-ku videoUrl return pannurom
        [HttpPost("download")]
        public async Task<IActionResult> Download(
            [FromBody] ReelRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Url))
                    return BadRequest(new { success = false, message = "URL required" });

                string videoUrl = await _reelService.GetVideoUrlAsync(request.Url);

                return Ok(new { success = true, videoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Extract + Stream — one shot! URL expire problem illai
        [HttpPost("download-and-stream")]
        public async Task<IActionResult> DownloadAndStream(
            [FromBody] ReelRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Url))
                    return BadRequest(new { success = false, message = "URL required" });

                // Fresh URL extract
                string videoUrl = await _reelService.GetVideoUrlAsync(request.Url);

                // Udane stream pannurom
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                client.DefaultRequestHeaders.Add("Referer",
                    "https://www.instagram.com/");

                var response = await client.GetAsync(
                    videoUrl, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = $"CDN error: {(int)response.StatusCode}"
                    });

                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "video/mp4", "instagram-reel.mp4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}