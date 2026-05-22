using System.Diagnostics;
using System.Text.Json;

namespace instagram_downloader.Services;

public class ReelService
{
    public async Task<dynamic> DownloadReelAsync(string reelUrl)
    {
        var tempFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "downloads"
        );

        if (!Directory.Exists(tempFolder))
        {
            Directory.CreateDirectory(tempFolder);
        }

        var outputTemplate = Path.Combine(
            tempFolder,
            "%(id)s.%(ext)s"
        );

        var startInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments =
                $"-f mp4 --get-url \"{reelUrl}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception(error);
        }

        return new
        {
            Success = true,
            VideoUrl = output.Trim()
        };
    }
}