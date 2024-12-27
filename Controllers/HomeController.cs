using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using youtubeDowloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace youtubeDowloader.Controllers
{
    public class HomeController(YoutubeClient youtube) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details(string url)
        {
            var video = await youtube.Videos.GetAsync(url);
            var manifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var videoStreams = manifest.GetVideoStreams().Select(s => new StreamInfo
            {
                Quality = s.VideoQuality.Label,
                Container = s.Container.Name,
                FileSize = s.Size.MegaBytes,
                Url = s.Url
            }).ToList();
            var audioStreams = manifest.GetAudioStreams().Select(s => new StreamInfo
            {
                Quality = s.AudioCodec,
                Container = s.Container.Name,
                FileSize = s.Size.MegaBytes,
                Url = s.Url
            }).ToList();

            var videoInfo = new YoutubeVideo
            {
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Duration = video.Duration?.ToString() ?? "Live",
                Thumbnail = video.Thumbnails.FirstOrDefault()!.Url,
                VideoStreams = videoStreams,
                AudioStreams = audioStreams,
                Url = url
            };
            return View(videoInfo);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string url, string quality)
        {
            try
            {
                var video = await youtube.Videos.GetAsync(url);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);

                var streamInfo = streamManifest.GetVideoOnlyStreams()
                    .Where(s => s.VideoQuality.Label == quality).GetWithHighestVideoQuality();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                var fileName = $"{video.Title}.mp4";

                // Sanitize filename
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception)
            {
                TempData["Error"] = "Download failed. Please try again.";
                return RedirectToAction("Index");
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
