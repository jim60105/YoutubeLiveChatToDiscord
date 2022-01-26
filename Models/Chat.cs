namespace LiveChatToDiscord.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
#pragma warning disable IDE1006 // 命名樣式
public class Thumbnail
{
    public string? url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

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

public class Item
{
    public LiveChatTextMessageRenderer? liveChatTextMessageRenderer { get; set; }
    public LiveChatViewerEngagementMessageRenderer? liveChatViewerEngagementMessageRenderer { get; set; }
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

public class Action
{
    public string? clickTrackingParams { get; set; }
    public AddChatItemAction? addChatItemAction { get; set; }
    public MarkChatItemAsDeletedAction? markChatItemAsDeletedAction { get; set; }
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
