using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeLiveChatToDiscord.Services;

public class LiveChatDownloadService
{
    private readonly ILogger<LiveChatDownloadService> logger;
    private readonly string id;
    public Task<int> DownloadProcess = Task.FromResult(0);

    public LiveChatDownloadService(ILogger<LiveChatDownloadService> _logger)
    {
        logger = _logger;

        id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
        if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DownloadProcess = ExecuteAsyncInternal(stoppingToken);
        return DownloadProcess;
    }

    private Task<int> ExecuteAsyncInternal(CancellationToken stoppingToken)
    {
        OptionSet live_chatOptionSet = new()
        {
            IgnoreConfig = true,
            WriteSub = true,
            SubLang = "live_chat",
            SkipDownload = true,
            NoPart = true,
            NoContinue = true,
            Output = "%(id)s"
        };
        live_chatOptionSet.AddCustomOption("--ignore-no-formats-error", true);

        OptionSet info_jsonOptionSet = new()
        {
            IgnoreConfig = true,
            WriteInfoJson = true,
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s"
        };
        info_jsonOptionSet.AddCustomOption("--ignore-no-formats-error", true);

        if (File.Exists("cookies.txt"))
        {
            live_chatOptionSet.Cookies = "cookies.txt";
            info_jsonOptionSet.Cookies = "cookies.txt";
        }

        YoutubeDLProcess ytdlProc = new(Helper.WhereIsYt_dlp());
        ytdlProc.OutputReceived += (o, e) => logger.LogTrace("{message}", e.Data);
        ytdlProc.ErrorReceived += (o, e) => logger.LogError("{error}", e.Data);

        string url = $"https://www.youtube.com/watch?v={id}";
        logger.LogInformation("Start yt-dlp with url: {url}", url);
        return ytdlProc.RunAsync(new string[] { url },
                                    info_jsonOptionSet,
                                    stoppingToken)
                          .ContinueWith((e) => ytdlProc.RunAsync(new string[] { url },
                                                                 live_chatOptionSet,
                                                                 stoppingToken))
                          .Unwrap();

    }
}
