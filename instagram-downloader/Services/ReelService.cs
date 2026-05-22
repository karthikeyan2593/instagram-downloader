using System.Text.Json;

namespace instagram_downloader.Services
{
    public class ReelService
    {
        private readonly HttpClient _httpClient;

        public ReelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> DownloadReelAsync(string reelUrl)
        {
            var apiUrl =
                $"https://fastvideosave.net/wp-json/aio-dl/video-data/?url={Uri.EscapeDataString(reelUrl)}";

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch reel.");
            }

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (!root.TryGetProperty("medias", out var medias))
            {
                throw new Exception("Video not found.");
            }

            var firstVideo = medias[0];

            var videoUrl = firstVideo.GetProperty("url").GetString();

            if (string.IsNullOrEmpty(videoUrl))
            {
                throw new Exception("Video URL empty.");
            }

            return videoUrl;
        }
    }
}