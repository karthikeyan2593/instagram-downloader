using System.Diagnostics;

namespace instagram_downloader.Services
{
    public class ReelService
    {
        private const string YtDlpPath =
            @"C:\yt-dlp\yt-dlp.exe";

        private const string CookiesPath =
            @"C:\Users\HP\Desktop\instagram-downloader\instagram-downloader\cookies.txt";

        public async Task<string> GetVideoUrlAsync(string reelUrl)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = YtDlpPath,

                Arguments = $"--get-url " +
                            $"--no-warnings " +
                            $"--socket-timeout 30 " +
                            $"--retries 3 " +
                            $"--cookies \"{CookiesPath}\" " +
                            $"{reelUrl}",

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // 60 second timeout
            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(60));

            string output = await process.StandardOutput
                .ReadToEndAsync();

            string error = await process.StandardError
                .ReadToEndAsync();

            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                process.Kill();
                throw new Exception("yt-dlp timeout — 60 seconds exceed aaguthu!");
            }

            // First line mattum — clean URL
            string videoUrl = output
                .Split('\n')
                .FirstOrDefault(line => line.StartsWith("http"))
                ?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(videoUrl))
                throw new Exception(
                    string.IsNullOrEmpty(error)
                        ? "yt-dlp URL extract failed"
                        : error.Trim()
                );

            return videoUrl;
        }
    }
}