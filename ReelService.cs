using System.Diagnostics;

namespace instagram_downloader.Services;

public class ReelService
{
    public async Task<string> GetVideoUrlAsync(string reelUrl)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = $"-g \"{reelUrl}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string output =
            await process.StandardOutput.ReadToEndAsync();

        string error =
            await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new Exception(error);
        }

        return output.Trim();
    }
}