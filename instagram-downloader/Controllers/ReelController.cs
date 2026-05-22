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

        public ReelController(ReelService reelService)
        {
            _reelService = reelService;
        }

        [HttpPost("download")]
        public async Task<IActionResult> Download([FromBody] ReelRequest request)
        {
            try
            {
                var videoUrl = await _reelService.DownloadReelAsync(request.Url);

                return Ok(new
                {
                    success = true,
                    videoUrl = videoUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("download-and-stream")]
        public async Task<IActionResult> DownloadAndStream([FromBody] ReelRequest request)
        {
            try
            {
                var videoUrl = await _reelService.DownloadReelAsync(request.Url);

                using var client = new HttpClient();

                var bytes = await client.GetByteArrayAsync(videoUrl);

                return File(bytes, "video/mp4", "instagram-reel.mp4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}