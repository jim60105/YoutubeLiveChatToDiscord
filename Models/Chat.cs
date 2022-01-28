﻿namespace YoutubeLiveChatToDiscord.Models;

/* These POCOs are generated from the results by the code generator.
 * https://json2csharp.com/
 */

// Chat myDeserializedClass = JsonConvert.DeserializeObject<Chat>(myJsonResponse);

#pragma warning disable IDE1006 // 命名樣式
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
//public class Thumbnail
//{
//    public string url { get; set; }
//    public int width { get; set; }
//    public int height { get; set; }
//}

public class AccessibilityData
{
    public string? label { get; set; }
    public AccessibilityData? accessibilityData { get; set; }
}

public class Accessibility
{
    public AccessibilityData? accessibilityData { get; set; }
}

public class Image
{
    public List<Thumbnail>? thumbnails { get; set; }
    public Accessibility? accessibility { get; set; }
}

public class Emoji
{
    public string? emojiId { get; set; }
    public List<string>? shortcuts { get; set; }
    public List<string>? searchTerms { get; set; }
    public Image? image { get; set; }
    public bool isCustomEmoji { get; set; }
    public bool? supportsSkinTone { get; set; }
    public List<string>? variantIds { get; set; }
}

public class Run
{
    public Emoji? emoji { get; set; }
    public string? text { get; set; }
}

public class Message
{
    public List<Run>? runs { get; set; }
}

public class AuthorName
{
    public string? simpleText { get; set; }
}

public class AuthorPhoto
{
    public List<Thumbnail>? thumbnails { get; set; }
    public Accessibility? accessibility { get; set; }
}

public class WebCommandMetadata
{
    public bool ignoreNavigation { get; set; }
    public string? url { get; set; }
    public string? webPageType { get; set; }
    public int rootVe { get; set; }
}

public class CommandMetadata
{
    public WebCommandMetadata? webCommandMetadata { get; set; }
}

public class LiveChatItemContextMenuEndpoint
{
    public string? @params { get; set; }
}

public class ContextMenuEndpoint
{
    public string? clickTrackingParams { get; set; }
    public CommandMetadata? commandMetadata { get; set; }
    public LiveChatItemContextMenuEndpoint? liveChatItemContextMenuEndpoint { get; set; }
}

public class CustomThumbnail
{
    public List<Thumbnail>? thumbnails { get; set; }
}

public class Icon
{
    public string? iconType { get; set; }
}

public class LiveChatAuthorBadgeRenderer
{
    public CustomThumbnail? customThumbnail { get; set; }
    public string? tooltip { get; set; }
    public Accessibility? accessibility { get; set; }
    public Icon? icon { get; set; }
}

public class AuthorBadge
{
    public LiveChatAuthorBadgeRenderer? liveChatAuthorBadgeRenderer { get; set; }
}

public class ContextMenuAccessibility
{
    public AccessibilityData? accessibilityData { get; set; }
}

public class LiveChatTextMessageRenderer
{
    public Message? message { get; set; }
    public AuthorName? authorName { get; set; }
    public AuthorPhoto? authorPhoto { get; set; }
    public ContextMenuEndpoint? contextMenuEndpoint { get; set; }
    public string? id { get; set; }
    public string? timestampUsec { get; set; }
    public List<AuthorBadge>? authorBadges { get; set; }
    public string? authorExternalChannelId { get; set; }
    public ContextMenuAccessibility? contextMenuAccessibility { get; set; }
}

public class PurchaseAmountText
{
    public string? simpleText { get; set; }
}

public class LiveChatPaidMessageRenderer
{
    public string? id { get; set; }
    public string? timestampUsec { get; set; }
    public AuthorName? authorName { get; set; }
    public AuthorPhoto? authorPhoto { get; set; }
    public PurchaseAmountText? purchaseAmountText { get; set; }
    public Message? message { get; set; }
    public object? headerBackgroundColor { get; set; }
    public object? headerTextColor { get; set; }
    public object? bodyBackgroundColor { get; set; }
    public object? bodyTextColor { get; set; }
    public string? authorExternalChannelId { get; set; }
    public object? authorNameTextColor { get; set; }
    public ContextMenuEndpoint? contextMenuEndpoint { get; set; }
    public object? timestampColor { get; set; }
    public ContextMenuAccessibility? contextMenuAccessibility { get; set; }
    public string? trackingParams { get; set; }
}

public class Text
{
    public string? simpleText { get; set; }
}

public class UrlEndpoint
{
    public string? url { get; set; }
    public string? target { get; set; }
}

public class NavigationEndpoint
{
    public string? clickTrackingParams { get; set; }
    public CommandMetadata? commandMetadata { get; set; }
    public UrlEndpoint? urlEndpoint { get; set; }
}

public class ButtonRenderer
{
    public string? style { get; set; }
    public string? size { get; set; }
    public bool isDisabled { get; set; }
    public Text? text { get; set; }
    public NavigationEndpoint? navigationEndpoint { get; set; }
    public string? trackingParams { get; set; }
    public AccessibilityData? accessibilityData { get; set; }
}

public class ActionButton
{
    public ButtonRenderer? buttonRenderer { get; set; }
}

public class LiveChatViewerEngagementMessageRenderer
{
    public string? id { get; set; }
    public string? timestampUsec { get; set; }
    public Icon? icon { get; set; }
    public Message? message { get; set; }
    public ActionButton? actionButton { get; set; }
}

public class Sticker
{
    public List<Thumbnail>? thumbnails { get; set; }
    public Accessibility? accessibility { get; set; }
}

public class LiveChatPaidStickerRenderer
{
    public string? id { get; set; }
    public ContextMenuEndpoint? contextMenuEndpoint { get; set; }
    public ContextMenuAccessibility? contextMenuAccessibility { get; set; }
    public string? timestampUsec { get; set; }
    public AuthorPhoto? authorPhoto { get; set; }
    public AuthorName? authorName { get; set; }
    public string? authorExternalChannelId { get; set; }
    public Sticker? sticker { get; set; }
    public long moneyChipBackgroundColor { get; set; }
    public long moneyChipTextColor { get; set; }
    public PurchaseAmountText? purchaseAmountText { get; set; }
    public int stickerDisplayWidth { get; set; }
    public int stickerDisplayHeight { get; set; }
    public long backgroundColor { get; set; }
    public long authorNameTextColor { get; set; }
    public string? trackingParams { get; set; }
}

public class Item
{
    public LiveChatTextMessageRenderer? liveChatTextMessageRenderer { get; set; }
    public LiveChatPaidMessageRenderer? liveChatPaidMessageRenderer { get; set; }
    public LiveChatViewerEngagementMessageRenderer? liveChatViewerEngagementMessageRenderer { get; set; }
    public LiveChatPaidStickerRenderer? liveChatPaidStickerRenderer { get; set; }
    public LiveChatTickerPaidMessageItemRenderer? liveChatTickerPaidMessageItemRenderer { get; set; }
}

public class AddChatItemAction
{
    public Item? item { get; set; }
    public string? clientId { get; set; }
}

public class DeletedStateMessage
{
    public List<Run>? runs { get; set; }
}

public class MarkChatItemAsDeletedAction
{
    public DeletedStateMessage? deletedStateMessage { get; set; }
    public string? targetItemId { get; set; }
}

public class Amount
{
    public string? simpleText { get; set; }
}

public class Renderer
{
    public LiveChatPaidMessageRenderer? liveChatPaidMessageRenderer { get; set; }
}

public class ShowLiveChatItemEndpoint
{
    public Renderer? renderer { get; set; }
    public string? trackingParams { get; set; }
}

public class ShowItemEndpoint
{
    public string? clickTrackingParams { get; set; }
    public CommandMetadata? commandMetadata { get; set; }
    public ShowLiveChatItemEndpoint? showLiveChatItemEndpoint { get; set; }
}

public class LiveChatTickerPaidMessageItemRenderer
{
    public string? id { get; set; }
    public Amount? amount { get; set; }
    public long amountTextColor { get; set; }
    public long startBackgroundColor { get; set; }
    public long endBackgroundColor { get; set; }
    public AuthorPhoto? authorPhoto { get; set; }
    public int durationSec { get; set; }
    public ShowItemEndpoint? showItemEndpoint { get; set; }
    public string? authorExternalChannelId { get; set; }
    public int fullDurationSec { get; set; }
    public string? trackingParams { get; set; }
}

public class AddLiveChatTickerItemAction
{
    public Item? item { get; set; }
    public string? durationSec { get; set; }
}

public class Action
{
    public string? clickTrackingParams { get; set; }
    public AddChatItemAction? addChatItemAction { get; set; }
    public MarkChatItemAsDeletedAction? markChatItemAsDeletedAction { get; set; }
    public AddLiveChatTickerItemAction? addLiveChatTickerItemAction { get; set; }
}

public class ReplayChatItemAction
{
    public List<Action>? actions { get; set; }
}

public class Chat
{
    public ReplayChatItemAction? replayChatItemAction { get; set; }
    public string? videoOffsetTimeMsec { get; set; }
    public bool isLive { get; set; }
}

#pragma warning restore IDE1006 // 命名樣式
