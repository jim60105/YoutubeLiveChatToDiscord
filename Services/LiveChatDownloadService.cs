using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeLiveChatToDiscord.Services;

public class LiveChatDownloadService
{
    private readonly ILogger<LiveChatDownloadService> _logger;
    private readonly string _id;
    public Task<int> downloadProcess = Task.FromResult(0);

    public LiveChatDownloadService(ILogger<LiveChatDownloadService> logger)
    {
        _logger = logger;
        _id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
        if (string.IsNullOrEmpty(_id)) throw new ArgumentException(nameof(_id));
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        downloadProcess = ExecuteAsyncInternal(stoppingToken);
        return downloadProcess;
    }

    private Task<int> ExecuteAsyncInternal(CancellationToken stoppingToken)
    {
        OptionSet live_chatOptionSet = new()
        {
            IgnoreConfig = true,
            WriteSubs = true,
            SubLangs = "live_chat",
            SkipDownload = true,
            NoPart = true,
            NoContinue = true,
            Output = "%(id)s",
            IgnoreNoFormatsError = true
        };

        OptionSet info_jsonOptionSet = new()
        {
            IgnoreConfig = true,
            WriteInfoJson = true,
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s",
            IgnoreNoFormatsError = true
        };

        FileInfo cookies = new("cookies.txt");
        if (cookies.Exists)
        {
            _logger.LogInformation("Detected {cookies}, use it for yt-dlp", cookies.FullName);
            var bak = cookies.CopyTo("cookies.copy.txt", true);
            live_chatOptionSet.Cookies = bak.FullName;
            info_jsonOptionSet.Cookies = bak.FullName;
        }

        YoutubeDLProcess ytdlProc = new(Helper.WhereIsYt_dlp());
        ytdlProc.OutputReceived += (o, e) => _logger.LogTrace("{message}", e.Data);
        ytdlProc.ErrorReceived += (o, e) => _logger.LogError("{error}", e.Data);

        string url = $"https://www.youtube.com/watch?v={_id}";
        _logger.LogInformation("Start yt-dlp with url: {url}", url);
        return ytdlProc.RunAsync(new string[] { url },
                                 info_jsonOptionSet,
                                 stoppingToken)
                       .ContinueWith((e) => ytdlProc.RunAsync(new string[] { url },
                                                              live_chatOptionSet,
                                                              stoppingToken))
                       .Unwrap();
    }
}
