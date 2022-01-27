using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using YoutubeLiveChatToDiscord.Models;

namespace YoutubeLiveChatToDiscord
{
    public class LiveChatMonitorWorker : BackgroundService
    {
        private readonly ILogger<LiveChatMonitorWorker> logger;
        private readonly string id;
        private readonly DiscordWebhookClient client;
        private readonly FileInfo liveChatFileInfo;
        private long position = 0;

        public LiveChatMonitorWorker(ILogger<LiveChatMonitorWorker> _logger,
                                     DiscordWebhookClient _client)
        {
            (logger, client) = (_logger, _client);
            client.Log += Client_Log;

            id = Environment.GetEnvironmentVariable("VIDEOID") ?? "";
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

            liveChatFileInfo = new($"{id}.live_chat.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!liveChatFileInfo.Exists && !stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{jsonFile} not found.", liveChatFileInfo.FullName);
                logger.LogInformation($"Wait for {nameof(LiveChatDownloadWorker)} to start.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                liveChatFileInfo.Refresh();
            }
            logger.LogInformation("Start Monitoring!");

            using StreamReader sr = new(liveChatFileInfo.OpenRead());
            position = sr.BaseStream.Length;
            logger.LogDebug("Start at position: {position}", position);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (sr.BaseStream.Length <= position)
                {
                    position = sr.BaseStream.Length;
                    logger.LogTrace("No new chat. Wait 10 sec");
                    // 每10秒檢查一次json檔
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                sr.BaseStream.Seek(position, SeekOrigin.Begin);
                while (position < sr.BaseStream.Length)
                {
                    try
                    {
                        string? str = await sr.ReadLineAsync();
                        if (string.IsNullOrEmpty(str)) continue;

                        Chat? chat = JsonConvert.DeserializeObject<Chat>(str);
                        position = sr.BaseStream.Position;
                        if (null == chat) continue;

                        await BuildRequestAndSendToDiscord(chat, stoppingToken);
                    }
                    catch (JsonSerializationException e)
                    {
                        logger.LogError("{error}", e);
                        position = sr.BaseStream.Position;
                    }
                }
            }
        }

        private async Task BuildRequestAndSendToDiscord(Chat chat, CancellationToken stoppingToken)
        {
            LiveChatTextMessageRenderer? lctmr = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
            List<Run>? runs = lctmr?.message?.runs;
            string author = lctmr?.authorName?.simpleText ?? "";
            string authorPhoto = lctmr?.authorPhoto?.thumbnails?.LastOrDefault()?.url ?? "";

            if (null != runs)
            {
                var eb = new EmbedBuilder();
                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))));
                //eb.WithAuthor(author, authorPhoto);
                eb.WithTitle("[點此前往影片]");
                eb.WithUrl($"https://youtu.be/{id}");
                eb.WithFooter(new EmbedFooterBuilder().WithText($"{id}, {lctmr?.timestampUsec}"));

                try
                {
                    logger.LogDebug("Sending Request to Discord: {author}, {message}", author, eb.Description);
                    ulong messageId = await client.SendMessageAsync(embeds: new Embed[] { eb.Build() },
                                                                username: author,
                                                                avatarUrl: authorPhoto);
                    logger.LogDebug("Message sent to discord, message id: {messageId}", messageId);

                    // The rate for Discord webhooks are 30 requests/minute per channel.
                    // Be careful when you run multiple instances in the same channel!
                    logger.LogTrace("Wait 2 seconds for discord webhook rate limit");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (TimeoutException) { }
            }
        }

        private Task Client_Log(LogMessage arg)
        {
            return Task.Run(() =>
            {
                switch (arg.Severity)
                {
                    case LogSeverity.Critical:
                        logger.LogCritical("{message}", arg);
                        break;
                    case LogSeverity.Error:
                        logger.LogError("{message}", arg);
                        break;
                    case LogSeverity.Warning:
                        logger.LogWarning("{message}", arg);
                        break;
                    case LogSeverity.Info:
                        logger.LogInformation("{message}", arg);
                        break;
                    case LogSeverity.Verbose:
                        logger.LogTrace("{message}", arg);
                        break;
                    case LogSeverity.Debug:
                        logger.LogDebug("{message}", arg);
                        break;
                    default:
                        break;
                }
            });
        }

    }
}