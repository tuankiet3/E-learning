using Azure.Core;
using System.Diagnostics;
using E_learning.Model.Courses;
using E_learning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace E_learning.Services.Lesson
{
    public class ConvertURL
    {
        private readonly ILogger<ConvertURL> _logger;
        public ConvertURL(ILogger<ConvertURL> logger)
        {
            _logger = logger;
        }
        public async Task UploadVideo(IFormFile videoFile, string lessonID)
        {
            if (videoFile == null || videoFile.Length == 0)
                throw new ArgumentException("Video file is missing or empty.");

            var lessonPath = Path.Combine("private_videos", "videos", lessonID);
            if (!Directory.Exists(lessonPath))
                Directory.CreateDirectory(lessonPath);

            var savePath = Path.Combine(lessonPath, "input.mp4");
            using var stream = new FileStream(savePath, FileMode.Create);
            await videoFile.CopyToAsync(stream);

            _logger.LogInformation("📥 Uploaded video for lesson {lessonId} to {path}", lessonID, savePath);
        }
        public async Task<string> convertToHLS(string lessonId)
        {
            var videoDir = Path.Combine("private_videos", "videos", lessonId);
            var inputPath = Path.Combine(videoDir, "input.mp4");
            var outputPath = Path.Combine(videoDir, "index.m3u8");

            if (!System.IO.File.Exists(inputPath))
            {
                _logger.LogError("❌ Input video not found at {path}", inputPath);
                return "Input video not found.";
            }

            var ffmpegPath = @"C:\tool\ffmpeg-7.1.1-essentials_build\bin\ffmpeg.exe";
            if (!System.IO.File.Exists(ffmpegPath))
            {
                _logger.LogError("❌ FFmpeg not found at {path}", ffmpegPath);
                return "FFmpeg executable not found.";
            }

            var args = $"-i \"{inputPath}\" -preset ultrafast -start_number 0 -hls_time 10 -hls_list_size 0 -f hls \"{outputPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = args,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                _logger.LogInformation("🎬 Starting FFmpeg conversion for lesson {lessonId}", lessonId);
                process.Start();

                var logTask = Task.Run(async () =>
                {
                    string? line;
                    while ((line = await process.StandardError.ReadLineAsync()) != null)
                    {
                        _logger.LogInformation("[FFmpeg] " + line);
                    }
                });

                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(2));
                var waitForExitTask = Task.Run(() => process.WaitForExit());

                var completedTask = await Task.WhenAny(waitForExitTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    try { process.Kill(); } catch { }
                    _logger.LogError("⚠️ FFmpeg conversion timed out for {lessonId}", lessonId);
                    return "Conversion timed out and was terminated.";
                }

                if (process.ExitCode != 0)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    _logger.LogError("❌ FFmpeg exited with error: {error}", error);
                    return $"FFmpeg failed: {error}";
                }

                _logger.LogInformation("✅ FFmpeg conversion completed for lesson {lessonId}", lessonId);
                return $"/videos/{lessonId}/index.m3u8";
            }
            catch (Exception ex)
            {
                try { process.Kill(); } catch { }
                _logger.LogError(ex, "❌ Exception during FFmpeg conversion");
                return "Video processing failed: " + ex.Message;
            }
        }
        public async Task<string> TryConvertWithRetry(string lessonId, int maxRetries = 2)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var result = await convertToHLS(lessonId);
                if (!result.StartsWith("FFmpeg failed") && !result.StartsWith("Conversion timed out"))
                    return result;

                _logger.LogWarning("⚠️ Convert attempt {attempt} failed: {result}", attempt, result);
                await Task.Delay(1000); // Wait 1s before retry
            }

            return "Failed to convert video after multiple attempts.";
        }

        public async Task DeleteVideo(string lessonId)
        {
            var videoDir = Path.Combine("private_videos", "videos", lessonId);
            if (Directory.Exists(videoDir))
            {
                try
                {
                    Directory.Delete(videoDir, true);
                    _logger.LogInformation("🗑️ Deleted video directory for lesson {lessonId}", lessonId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to delete video directory for lesson {lessonId}", lessonId);
                }
            }
            else
            {
                _logger.LogWarning("⚠️ Video directory for lesson {lessonId} does not exist", lessonId);
            }
        }
    }
}
