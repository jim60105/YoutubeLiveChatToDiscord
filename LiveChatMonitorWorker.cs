using Discord;
using Discord.Webhook;
using YoutubeLiveChatToDiscord.Models;
using Newtonsoft.Json;

namespace YoutubeLiveChatToDiscord
{
    public class LiveChatMonitorWorker : BackgroundService
    {
        private readonly ILogger<LiveChatMonitorWorker> logger;
        private readonly string id = Environment.GetEnvironmentVariable("VIDEOID") ?? "";
        private readonly DiscordWebhookClient client;
        private long position = 0;

        public LiveChatMonitorWorker(ILogger<LiveChatMonitorWorker> _logger,
                                     DiscordWebhookClient _client)
        => (logger, client) = (_logger, _client);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await RunAsync();
#if DEBUG
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
#else
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
#endif
            }
        }

        public async Task RunAsync()
        {
            FileInfo fi = new($"{id}.live_chat.json");
            if (fi.Exists)
            {
                using StreamReader sr = new(fi.OpenRead());
                if (sr.BaseStream.Length >= position)
                {
                    sr.BaseStream.Seek(position, SeekOrigin.Begin);
                }

                while (sr.Peek() >= 0)
                {
                    try
                    {
                        Chat? chat = JsonConvert.DeserializeObject<Chat>(await sr.ReadLineAsync() ?? "");

                        if (null != chat)
                        {
                            logger.LogTrace("Get chat {chat}", chat);
                            try
                            {
                                await SendToDiscordAsync(client, chat);
                            }
                            catch (TimeoutException) { }
                        }
                    }
                    catch (JsonSerializationException) { }

                    position = sr.BaseStream.Position;
                }
            }
        }

        private Task SendToDiscordAsync(DiscordWebhookClient client, Chat chat)
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
                eb.WithFooter(new EmbedFooterBuilder().WithText(id));

                return client.SendMessageAsync(embeds: new Embed[] { eb.Build() },
                                               username: author,
                                               avatarUrl: authorPhoto);
            }
            return Task.CompletedTask;
        }
    }
}