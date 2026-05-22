using instagram_downloader.Models;
using instagram_downloader.Services;
using Microsoft.AspNetCore.Mvc;

namespace instagram_downloader.Controllers;

[ApiController]
[Route("api/reels")]
public class ReelController : ControllerBase
{
    private readonly ReelService _reelService;

    public ReelController(ReelService reelService)
    {
        _reelService = reelService;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            success = true,
            message = "API working"
        });
    }

    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] ReelRequest request)
    {
        try
        {
            var result = await _reelService.DownloadReelAsync(request.Url);

            return Ok(new
            {
                success = true,
                videoUrl = result.VideoUrl
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
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
            var result = await _reelService.DownloadReelAsync(request.Url);

            if (result == null || string.IsNullOrEmpty(result.VideoUrl))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Video URL not found"
                });
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0"
            );

            var bytes = await client.GetByteArrayAsync(result.VideoUrl);

            return File(
                bytes,
                "video/mp4",
                "instagram-reel.mp4"
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}