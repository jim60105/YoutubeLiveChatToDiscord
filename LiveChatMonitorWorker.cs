using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using YoutubeLiveChatToDiscord.Models;
using YoutubeLiveChatToDiscord.Services;

namespace YoutubeLiveChatToDiscord
{
    public class LiveChatMonitorWorker : BackgroundService
    {
        private readonly ILogger<LiveChatMonitorWorker> _logger;
        private readonly string _id;
        private readonly DiscordWebhookClient _client;
        private readonly FileInfo _liveChatFileInfo;
        private long _position = 0;
        private readonly LiveChatDownloadService _liveChatDownloadService;

        public LiveChatMonitorWorker(
            ILogger<LiveChatMonitorWorker> logger,
            DiscordWebhookClient client,
            LiveChatDownloadService liveChatDownloadService
            )
        {
            (_logger, _client, _liveChatDownloadService) = (logger, client, liveChatDownloadService);
            _client.Log += Helper.DiscordWebhookClient_Log;

            _id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
            if (string.IsNullOrEmpty(_id)) throw new ArgumentException(nameof(_id));

            _liveChatFileInfo = new($"{_id}.live_chat.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_liveChatDownloadService.downloadProcess.IsCompleted)
                    {
                        _ = _liveChatDownloadService.ExecuteAsync(stoppingToken)
                                                   .ContinueWith((_) => _logger.LogInformation("yt-dlp is stopped."), stoppingToken);
                    }

                    _logger.LogInformation("Wait 10 seconds.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    _liveChatFileInfo.Refresh();

                    try
                    {
                        if (!_liveChatFileInfo.Exists)
                        {
                            throw new FileNotFoundException(null, _liveChatFileInfo.FullName);
                        }

                        await Monitoring(stoppingToken);
                    }
                    catch (FileNotFoundException e)
                    {
                        _logger.LogWarning("Json file not found. {FileName}", e.FileName);
                    }
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                _logger.LogError("Wait 10 seconds before closing the program. This is to prevent a restart loop from hanging the machine.");
#pragma warning disable CA2016 // 將 'CancellationToken' 參數轉送給方法
                await Task.Delay(TimeSpan.FromSeconds(10));
#pragma warning restore CA2016 // 將 'CancellationToken' 參數轉送給方法
            }
        }

        /// <summary>
        /// Monitoring
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns></returns>
        private async Task Monitoring(CancellationToken stoppingToken)
        {
            await GetVideoInfo(stoppingToken);

#if !DEBUG
            if (null == Environment.GetEnvironmentVariable("SKIP_STARTUP_WAITING"))
            {
                _logger.LogInformation("Wait 1 miunute to skip old chats");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                _liveChatFileInfo.Refresh();
            }
#endif

            _position = _liveChatFileInfo.Length;
            _logger.LogInformation("Start at position: {position}", _position);
            _logger.LogInformation("Start Monitoring!");

            while (!stoppingToken.IsCancellationRequested)
            {
                _liveChatFileInfo.Refresh();
                if (_liveChatFileInfo.Length > _position)
                {
                    await ProcessChats(stoppingToken);
                }
                else if (_liveChatDownloadService.downloadProcess.IsCompleted)
                {
                    _logger.LogInformation("Download process is stopped. Restart monitoring.");
                    return;
                }
                else
                {
                    _position = _liveChatFileInfo.Length;
                    _logger.LogTrace("No new chat. Wait 10 seconds.");
                    // 每10秒檢查一次json檔
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        /// <summary>
        /// GetVideoInfo
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private async Task GetVideoInfo(CancellationToken stoppingToken)
        {
            FileInfo videoInfo = new($"{_id}.info.json");
            if (!videoInfo.Exists)
            {
                // Chat json file 在 VideoInfo json file之後被產生，理論上這段不會進來
                throw new FileNotFoundException(null, videoInfo.FullName);
            }

            Info? info = JsonConvert.DeserializeObject<Info>(await new StreamReader(videoInfo.OpenRead()).ReadToEndAsync(stoppingToken));
            string? Title = info?.title;
            string? ChannelId = info?.channel_id;
            string? thumb = info?.thumbnail;

            Environment.SetEnvironmentVariable("TITLE", Title);
            Environment.SetEnvironmentVariable("CHANNEL_ID", ChannelId);
            Environment.SetEnvironmentVariable("VIDEO_THUMB", thumb);
        }

        private async Task ProcessChats(CancellationToken stoppingToken)
        {
            // Notice: yt-dlp在Linux會使用lock鎖定此檔案，在Windows不鎖定。
            // 實作: https://github.com/yt-dlp/yt-dlp/commit/897376719871279eef89426b1452abb89051f0dc
            // Issue: https://github.com/yt-dlp/yt-dlp/issues/3124
            // 不像Windows是獨占鎖，Linux上是諮詢鎖，程式可以自行決定是否遵守鎖定。
            // FileStream「會」遵守鎖定，所以此處會在開啟檔案時報錯。
            // 詳細說明請參考這個issue，其中的討論過程非常清楚: https://github.com/dotnet/runtime/issues/34126
            // 這是.NET Core在Linux、Windows上關於鎖定設計的描述: https://github.com/dotnet/runtime/pull/55256
            // 如果要繞過這個問題，從.NET 6開始，可以加上環境變數「DOTNET_SYSTEM_IO_DISABLEFILELOCKING」讓FileStream「不」遵守鎖定。
            // (本專案已在Dockerfile加上此環境變數)
            using FileStream fs = new(_liveChatFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader sr = new(fs);

            sr.BaseStream.Seek(_position, SeekOrigin.Begin);
            while (_position < sr.BaseStream.Length)
            {
                string? str = "";
                try
                {
                    str = await sr.ReadLineAsync(stoppingToken);
                    _position = sr.BaseStream.Position;
                    if (string.IsNullOrEmpty(str)) continue;

                    Chat? chat = JsonConvert.DeserializeObject<Chat>(str);
                    if (null == chat) continue;

                    await BuildRequestAndSendToDiscord(chat, stoppingToken);
                }
                catch (JsonSerializationException e)
                {
                    _logger.LogError("{error}", e.Message);
                    _logger.LogError("{originalString}", str);
                }
                catch (ArgumentException e)
                {
                    _logger.LogError("{error}", e.Message);
                    _logger.LogError("{originalString}", str);
                }
                catch (IOException e)
                {
                    _logger.LogError("{error}", e.Message);
                    break;
                }
            }
        }

        /// <summary>
        /// 建立Discord embed並送出至Webhook
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">訊息格式未支援</exception>
        private async Task BuildRequestAndSendToDiscord(Chat chat, CancellationToken stoppingToken)
        {
            EmbedBuilder eb = new();
            eb.WithTitle(Environment.GetEnvironmentVariable("TITLE") ?? "")
              .WithUrl($"https://youtu.be/{_id}")
              .WithThumbnailUrl(Helper.GetOriginalImage(Environment.GetEnvironmentVariable("VIDEO_THUMB")));
            string author = "";

            var liveChatTextMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
            var liveChatPaidMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidMessageRenderer;
            var liveChatPaidSticker = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidStickerRenderer;
            var liveChatPurchaseSponsorshipsGift = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatSponsorshipsGiftPurchaseAnnouncementRenderer;

            // ReplaceChat: Treat as a new message
            // This is rare and not easy to test.
            // If it behaves strangely, please open a new issue with more examples.
            var replaceChat = chat.replayChatItemAction?.actions?.FirstOrDefault()?.replaceChatItemAction?.replacementItem?.liveChatTextMessageRenderer;
            if (null != replaceChat)
            {
                liveChatTextMessage = replaceChat;
            }

            // Normal Message
            if (null != liveChatTextMessage)
            {
                List<Run> runs = liveChatTextMessage.message?.runs ?? new List<Run>();
                author = liveChatTextMessage.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatTextMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatTextMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Timestamp
                long timeStamp = long.TryParse(liveChatTextMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                string authorBadgeUrl = Helper.GetOriginalImage(liveChatTextMessage.authorBadges?.FirstOrDefault()?.liveChatAuthorBadgeRenderer?.customThumbnail?.thumbnails?.LastOrDefault()?.url);
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl(authorBadgeUrl);

                // From Stream Owner
                //if (liveChatTextMessage.authorBadges?[0].liveChatAuthorBadgeRenderer?.icon?.iconType == "OWNER")
                if (liveChatTextMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            else if (null != liveChatPaidMessage)
            // Super Chat
            {
                List<Run> runs = liveChatPaidMessage.message?.runs ?? new List<Run>();

                author = liveChatPaidMessage.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatPaidMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidMessage.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidMessage.bodyBackgroundColor));
                eb.WithColor(bgColor);

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/wallet.png");

                // From Stream Owner
                if (liveChatPaidMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            else if (null != liveChatPaidSticker)
            // Super Chat Sticker
            {
                author = liveChatPaidSticker.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatPaidSticker.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription("")
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidSticker.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidSticker.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidSticker.backgroundColor));
                eb.WithColor(bgColor);

                // Super Chat Sticker Picture
                string stickerThumbUrl = Helper.GetOriginalImage("https:" + liveChatPaidSticker.sticker?.thumbnails?.LastOrDefault()?.url);
                eb.WithThumbnailUrl(stickerThumbUrl);

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidSticker.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/wallet.png");

                // From Stream Owner
                if (liveChatPaidSticker.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            else if (null != liveChatPurchaseSponsorshipsGift && null != liveChatPurchaseSponsorshipsGift?.header?.liveChatSponsorshipsHeaderRenderer)
            // Purchase Sponsorships Gift
            {
                LiveChatSponsorshipsHeaderRenderer header = liveChatPurchaseSponsorshipsGift.header.liveChatSponsorshipsHeaderRenderer;
                author = header.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(header.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription("")
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPurchaseSponsorshipsGift?.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Gift Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(header?.primaryText?.runs?[1].text) });

                // Gift Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml("#0f9d58");
                eb.WithColor(bgColor);

                // Gift Picture
                string? giftThumbUrl = header?.image?.thumbnails?.LastOrDefault()?.url;
                if (null != giftThumbUrl) eb.WithThumbnailUrl(giftThumbUrl);

                // Timestamp
                long timeStamp = long.TryParse(liveChatPurchaseSponsorshipsGift?.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/wallet.png");

                // From Stream Owner
                if (liveChatPurchaseSponsorshipsGift?.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    //eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            // Discrad known garbage messages.
            else if (
                // Banner Pinned message.
                null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addBannerToLiveChatCommand
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.removeBannerForLiveChatCommand
                // Click to show less.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.showLiveChatTooltipCommand
                // Welcome to live chat! Remember to guard your privacy and abide by our community guidelines.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatViewerEngagementMessageRenderer
                // Membership messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatMembershipItemRenderer
                // SC Ticker messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addLiveChatTickerItemAction
                // Delete messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.markChatItemAsDeletedAction
                // Remove Chat Item. Not really sure what this is.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.removeChatItemAction
                // Live chat mode change.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatModeChangeMessageRenderer
                // Poll
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.updateLiveChatPollAction
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.closeLiveChatActionPanelAction
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.showLiveChatActionPanelAction
                // Sponsorships Gift redemption
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatSponsorshipsGiftRedemptionAnnouncementRenderer
                // Have no idea what this is
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPlaceholderItemRenderer
            ) { return; }
            else
            {
                _logger.LogWarning("Message type not supported, skip sending to discord.");
                throw new ArgumentException("Message type not supported", nameof(chat));
            }

            if (stoppingToken.IsCancellationRequested) return;

            _logger.LogDebug("Sending Request to Discord: {author}: {message}", author, eb.Description);

            try
            {
                await SendMessage();
            }
            catch (TimeoutException) { }
            // System.Net.Http.HttpRequestException: Resource temporarily unavailable (discord.com:443)
            catch (HttpRequestException)
            {
                // Retry once after 5 sec
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                await SendMessage();
            }

            // The rate for Discord webhooks are 30 requests/minute per channel.
            // Be careful when you run multiple instances in the same channel!
            _logger.LogTrace("Wait 2 seconds for discord webhook rate limit");
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

            Task SendMessage()
               => _client.SendMessageAsync(embeds: new Embed[] { eb.Build() })
                   .ContinueWith(async p =>
                   {
                       ulong messageId = await p;
                       _logger.LogDebug("Message sent to discord, message id: {messageId}", messageId);
                   }, stoppingToken);
        }
    }
}