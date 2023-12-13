using Discord;
using Discord.Webhook;
using static YoutubeLiveChatToDiscord.Models.Chat;
using Chat = YoutubeLiveChatToDiscord.Models.Chat.chat;

namespace YoutubeLiveChatToDiscord.Services;

public class DiscordService
{
    private readonly ILogger<DiscordService> _logger;
    private readonly string _id;
    private readonly DiscordWebhookClient _client;

    public DiscordService(
        ILogger<DiscordService> logger,
        DiscordWebhookClient client)
    {
        _logger = logger;
        _client = client;
        _client.Log += DiscordWebhookClient_Log;
        _id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
        if (string.IsNullOrEmpty(_id)) throw new ArgumentException(nameof(_id));
    }

    /// <summary>
    /// 把.NET Core logger對應到Discord內建的logger上面
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private Task DiscordWebhookClient_Log(LogMessage arg)
        => Task.Run(() =>
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical("{message}", arg);
                    break;
                case LogSeverity.Error:
                    _logger.LogError("{message}", arg);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning("{message}", arg);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation("{message}", arg);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace("{message}", arg);
                    break;
                case LogSeverity.Debug:
                default:
                    _logger.LogDebug("{message}", arg);
                    break;
            }
        });

    /// <summary>
    /// 建立Discord embed並送出至Webhook
    /// </summary>
    /// <param name="chat"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">訊息格式未支援</exception>
    public async Task BuildRequestAndSendToDiscord(Chat chat, CancellationToken stoppingToken)
    {
        EmbedBuilder eb = new();
        eb.WithTitle(Environment.GetEnvironmentVariable("TITLE") ?? "")
          .WithUrl($"https://youtu.be/{_id}")
          .WithThumbnailUrl(Helper.GetOriginalImage(Environment.GetEnvironmentVariable("VIDEO_THUMB")));

        var liveChatTextMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
        var liveChatPaidMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidMessageRenderer;
        var liveChatPaidSticker = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidStickerRenderer;
        var liveChatMembershipItemRenderer = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatMembershipItemRenderer;
        var liveChatPurchaseSponsorshipsGift = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatSponsorshipsGiftPurchaseAnnouncementRenderer;

        // ReplaceChat: Treat as a new message
        // This is rare and not easy to test.
        // If it behaves strangely, please open a new issue with more examples.
        var replaceChat = chat.replayChatItemAction?.actions?.FirstOrDefault()?.replaceChatItemAction?.replacementItem?.liveChatTextMessageRenderer;
        if (null != replaceChat)
        {
            liveChatTextMessage = replaceChat;
        }

        string author;
        if (null != liveChatTextMessage)
        {
            BuildNormalMessage(ref eb, liveChatTextMessage, out author);
        }
        else if (null != liveChatPaidMessage)
        // Super Chat
        {
            BuildSuperChatMessage(ref eb, liveChatPaidMessage, out author);
        }
        else if (null != liveChatPaidSticker)
        // Super Chat Sticker
        {
            BuildSuperChatStickerMessage(ref eb, liveChatPaidSticker, out author);
        }
        else if (null != liveChatMembershipItemRenderer)
        // Join Membership
        {
            BuildMemberShipMessage(ref eb, liveChatMembershipItemRenderer, out author);
        }
        else if (null != liveChatPurchaseSponsorshipsGift
                 && null != liveChatPurchaseSponsorshipsGift.header.liveChatSponsorshipsHeaderRenderer)
        // Purchase Sponsorships Gift
        {
            BuildPurchaseSponsorshipsGiftMessage(ref eb, liveChatPurchaseSponsorshipsGift, out author);
        }
        // Discrad known garbage messages.
        else if (IsGarbageMessage(chat)) { return; }
        else
        {
            _logger.LogWarning("Message type not supported, skip sending to discord.");
            throw new ArgumentException("Message type not supported", nameof(chat));
        }

        if (stoppingToken.IsCancellationRequested) return;

        await SendMessage(eb, author, stoppingToken);

        // The rate for Discord webhooks are 30 requests/minute per channel.
        // Be careful when you run multiple instances in the same channel!
        _logger.LogTrace("Wait 2 seconds for discord webhook rate limit");
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
    }

    private static bool IsGarbageMessage(Chat chat) =>
        // Banner Pinned message.
        null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addBannerToLiveChatCommand
        || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.removeBannerForLiveChatCommand
        // Click to show less.
        || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.showLiveChatTooltipCommand
        // Welcome to live chat! Remember to guard your privacy and abide by our community guidelines.
        || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatViewerEngagementMessageRenderer
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
        || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPlaceholderItemRenderer;

    private static EmbedBuilder BuildNormalMessage(ref EmbedBuilder eb, LiveChatTextMessageRenderer liveChatTextMessage, out string author)
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
        return eb;
    }

    private static EmbedBuilder BuildSuperChatMessage(ref EmbedBuilder eb, LiveChatPaidMessageRenderer liveChatPaidMessage, out string author)
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
        Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(Helper.YoutubeColorConverter(liveChatPaidMessage.bodyBackgroundColor));
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
        return eb;
    }

    private static EmbedBuilder BuildSuperChatStickerMessage(ref EmbedBuilder eb, LiveChatPaidStickerRenderer liveChatPaidSticker, out string author)
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
        Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(Helper.YoutubeColorConverter(liveChatPaidSticker.backgroundColor));
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
        return eb;
    }

    private static EmbedBuilder BuildMemberShipMessage(ref EmbedBuilder eb, LiveChatMembershipItemRenderer liveChatMembershipItemRenderer, out string author)
    {
        List<Run> runs = liveChatMembershipItemRenderer.headerSubtext?.runs ?? new List<Run>();

        author = liveChatMembershipItemRenderer.authorName?.simpleText ?? "";
        string authorPhoto = Helper.GetOriginalImage(liveChatMembershipItemRenderer.authorPhoto?.thumbnails?.LastOrDefault()?.url);

        eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
          .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                              .WithUrl($"https://www.youtube.com/channel/{liveChatMembershipItemRenderer.authorExternalChannelId}")
                                              .WithIconUrl(authorPhoto));

        // Membership Background Color
        Color sponsorColor = (Color)System.Drawing.ColorTranslator.FromHtml("#0F9D58");
        eb.WithColor(sponsorColor);

        // Timestamp
        long timeStamp = long.TryParse(liveChatMembershipItemRenderer.timestampUsec, out long l) ? l / 1000 : 0;
        EmbedFooterBuilder ft = new();
        string authorBadgeUrl = Helper.GetOriginalImage(liveChatMembershipItemRenderer.authorBadges?.FirstOrDefault()?.liveChatAuthorBadgeRenderer?.customThumbnail?.thumbnails?.LastOrDefault()?.url);
        ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                  .LocalDateTime
                                  .ToString("yyyy/MM/dd HH:mm:ss"))
          .WithIconUrl(authorBadgeUrl);

        // From Stream Owner
        // I'm not sure if stream owner can join his own membership?
        if (liveChatMembershipItemRenderer.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
        {
            //eb.WithColor(Color.Gold);
            ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
        }

        eb.WithFooter(ft);
        return eb;
    }

    private static EmbedBuilder BuildPurchaseSponsorshipsGiftMessage(ref EmbedBuilder eb, LiveChatSponsorshipsGiftPurchaseAnnouncementRenderer liveChatPurchaseSponsorshipsGift, out string author)
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
        Color sponsorColor = (Color)System.Drawing.ColorTranslator.FromHtml("#0F9D58");
        eb.WithColor(sponsorColor);

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
        return eb;
    }

    private async Task SendMessage(EmbedBuilder eb, string author, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending Request to Discord: {author}: {message}", author, eb.Description);

        try
        {
            await _send();
        }
        catch (TimeoutException) { }
        // System.Net.Http.HttpRequestException: Resource temporarily unavailable (discord.com:443)
        catch (HttpRequestException)
        {
            // Retry once after 5 sec
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await _send();
        }

        Task _send()
            => _client.SendMessageAsync(embeds: new Embed[] { eb.Build() })
               .ContinueWith(p =>
               {
#pragma warning disable AsyncFixer02 // Long-running or blocking operations inside an async method
                   ulong messageId = p.Result;
#pragma warning restore AsyncFixer02 // Long-running or blocking operations inside an async method
                   _logger.LogDebug("Message sent to discord, message id: {messageId}", messageId);
               }, cancellationToken);
    }
}
