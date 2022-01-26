using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeLiveChatToDiscord;

public class LiveChatDownloadWorker : BackgroundService
{
    private readonly ILogger<LiveChatDownloadWorker> logger;
    private readonly string id = Environment.GetEnvironmentVariable("VIDEOID") ?? "";
    public static string YtdlPath { get; set; } = "/usr/bin/yt-dlp";

    public LiveChatDownloadWorker(ILogger<LiveChatDownloadWorker> _logger) => logger = _logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OptionSet optionSet = new()
        {
            IgnoreConfig = true,
            WriteSub = true,
            SubLang = "live_chat",
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s"
        };
        optionSet.AddCustomOption("--ignore-no-formats-error", "");

        YoutubeDLProcess ytdlProc = new(YtdlPath);
        //ytdlProc.OutputReceived += (o, e) => logger.Verbose(e.Data);
        ytdlProc.ErrorReceived += (o, e) => logger.LogError(e.Data);

        return ytdlProc.RunAsync(new string[] { id },
                                 optionSet,
                                 stoppingToken);
    }
}
