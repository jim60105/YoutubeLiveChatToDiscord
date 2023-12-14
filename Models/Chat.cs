#pragma warning disable IDE1006 // 命名樣式
#nullable disable

namespace YoutubeLiveChatToDiscord.Models;

public class Chat
{
    public class ContextMenuButton
    {
        public ButtonRenderer buttonRenderer { get; set; }
    }

    public class Icon
    {
        public string iconType { get; set; }
    }

    public class LiveChatBannerHeaderRenderer
    {
        public Icon icon { get; set; }
        public Text text { get; set; }
        public ContextMenuButton contextMenuButton { get; set; }
    }

    public class Header
    {
        public LiveChatBannerHeaderRenderer liveChatBannerHeaderRenderer { get; set; }
        public LiveChatSponsorshipsHeaderRenderer liveChatSponsorshipsHeaderRenderer { get; set; }
        public PollHeaderRenderer pollHeaderRenderer { get; set; }
    }

    public class AccessibilityData
    {
        public string label { get; set; }
        public AccessibilityData accessibilityData { get; set; }
    }

    public class Accessibility
    {
        public string label { get; set; }
        public AccessibilityData accessibilityData { get; set; }
    }

    public class Image
    {
        public List<Thumbnail> thumbnails { get; set; }
        public Accessibility accessibility { get; set; }
        public List<Source> sources { get; set; }
    }

    public class Emoji
    {
        public string emojiId { get; set; }
        public List<string> shortcuts { get; set; }
        public List<string> searchTerms { get; set; }
        public Image image { get; set; }
        public bool isCustomEmoji { get; set; }
        public bool supportsSkinTone { get; set; }
        public List<string> variantIds { get; set; }
    }

    public class Run
    {
        public Emoji emoji { get; set; }
        public string text { get; set; }
        public bool bold { get; set; }
        public bool italics { get; set; }
    }

    public class Message
    {
        public List<Run> runs { get; set; }
    }

    public class HeaderPrimaryText
    {
        public List<Run> runs { get; set; }
    }

    public class HeaderSubtext
    {
        public List<Run> runs { get; set; }
    }

    public class AuthorName
    {
        public string simpleText { get; set; }
    }

    public class AuthorPhoto
    {
        public List<Thumbnail> thumbnails { get; set; }
        public Accessibility accessibility { get; set; }
    }

    public class WebCommandMetadata
    {
        public bool ignoreNavigation { get; set; }
        public string url { get; set; }
        public string webPageType { get; set; }
        public int rootVe { get; set; }
        public string apiUrl { get; set; }
        public bool sendPost { get; set; }
    }

    public class CommandMetadata
    {
        public WebCommandMetadata webCommandMetadata { get; set; }
    }

    public class LiveChatItemContextMenuEndpoint
    {
        public string @params { get; set; }
    }

    public class ContextMenuEndpoint
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public LiveChatItemContextMenuEndpoint liveChatItemContextMenuEndpoint { get; set; }
    }

    public class CustomThumbnail
    {
        public List<Thumbnail> thumbnails { get; set; }
    }

    public class LiveChatAuthorBadgeRenderer
    {
        public CustomThumbnail customThumbnail { get; set; }
        public string tooltip { get; set; }
        public Accessibility accessibility { get; set; }
        public Icon icon { get; set; }
    }

    public class AuthorBadge
    {
        public LiveChatAuthorBadgeRenderer liveChatAuthorBadgeRenderer { get; set; }
    }

    public class ContextMenuAccessibility
    {
        public AccessibilityData accessibilityData { get; set; }
    }

    public class LiveChatTextMessageRenderer
    {
        public Message message { get; set; }
        public AuthorName authorName { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public List<AuthorBadge> authorBadges { get; set; }
        public string authorExternalChannelId { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
    }

    public class PurchaseAmountText
    {
        public string simpleText { get; set; }
    }

    public class LiveChatPaidMessageRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public AuthorName authorName { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public PurchaseAmountText purchaseAmountText { get; set; }
        public Message message { get; set; }
        public long headerBackgroundColor { get; set; }
        public long headerTextColor { get; set; }
        public long bodyBackgroundColor { get; set; }
        public long bodyTextColor { get; set; }
        public string authorExternalChannelId { get; set; }
        public long authorNameTextColor { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public long timestampColor { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
        public string trackingParams { get; set; }
        public HeaderOverlayImage headerOverlayImage { get; set; }
        public int textInputBackgroundColor { get; set; }
        public LowerBumper lowerBumper { get; set; }
        public CreatorHeartButton creatorHeartButton { get; set; }
        public bool isV2Style { get; set; }
        public PdgPurchasedNoveltyLoggingDirectives pdgPurchasedNoveltyLoggingDirectives { get; set; }
    }

    public class Text
    {
        public string simpleText { get; set; }
        public List<Run> runs { get; set; }
        public string content { get; set; }
        public List<StyleRun> styleRuns { get; set; }
    }

    public class UrlEndpoint
    {
        public string url { get; set; }
        public string target { get; set; }
        public bool nofollow { get; set; }
    }

    public class NavigationEndpoint
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public UrlEndpoint urlEndpoint { get; set; }
    }

    public class Command
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public LiveChatItemContextMenuEndpoint liveChatItemContextMenuEndpoint { get; set; }
    }

    public class ButtonRenderer
    {
        public string style { get; set; }
        public string size { get; set; }
        public bool isDisabled { get; set; }
        public Text text { get; set; }
        public NavigationEndpoint navigationEndpoint { get; set; }
        public string trackingParams { get; set; }
        public AccessibilityData accessibilityData { get; set; }
        public Icon icon { get; set; }
        public Command command { get; set; }
        public Accessibility accessibility { get; set; }
        public string targetId { get; set; }
    }

    public class ActionButton
    {
        public ButtonRenderer buttonRenderer { get; set; }
    }

    public class LiveChatViewerEngagementMessageRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public Icon icon { get; set; }
        public Message message { get; set; }
        public ActionButton actionButton { get; set; }
    }

    public class Sticker
    {
        public List<Thumbnail> thumbnails { get; set; }
        public Accessibility accessibility { get; set; }
    }

    public class LiveChatPaidStickerRenderer
    {
        public string id { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
        public string timestampUsec { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public AuthorName authorName { get; set; }
        public string authorExternalChannelId { get; set; }
        public Sticker sticker { get; set; }
        public long moneyChipBackgroundColor { get; set; }
        public long moneyChipTextColor { get; set; }
        public PurchaseAmountText purchaseAmountText { get; set; }
        public int stickerDisplayWidth { get; set; }
        public int stickerDisplayHeight { get; set; }
        public long backgroundColor { get; set; }
        public long authorNameTextColor { get; set; }
        public string trackingParams { get; set; }
        // Actually I doesn't find the 1st purchase sticker. These properties are copied from the LiveChatPaidMessageRenderer
        public HeaderOverlayImage headerOverlayImage { get; set; }
        public int textInputBackgroundColor { get; set; }
        public LowerBumper lowerBumper { get; set; }
        public CreatorHeartButton creatorHeartButton { get; set; }
        public bool isV2Style { get; set; }
        public PdgPurchasedNoveltyLoggingDirectives pdgPurchasedNoveltyLoggingDirectives { get; set; }
    }

    public class LiveChatMembershipItemRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public string authorExternalChannelId { get; set; }
        public HeaderPrimaryText headerPrimaryText { get; set; }
        public HeaderSubtext headerSubtext { get; set; }
        public Message message { get; set; }
        public AuthorName authorName { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public List<AuthorBadge> authorBadges { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
        public string trackingParams { get; set; }
    }

    public class Item
    {
        public LiveChatTextMessageRenderer liveChatTextMessageRenderer { get; set; }
        public LiveChatPaidMessageRenderer liveChatPaidMessageRenderer { get; set; }
        public LiveChatViewerEngagementMessageRenderer liveChatViewerEngagementMessageRenderer { get; set; }
        public LiveChatPaidStickerRenderer liveChatPaidStickerRenderer { get; set; }
        public LiveChatTickerPaidMessageItemRenderer liveChatTickerPaidMessageItemRenderer { get; set; }
        public LiveChatMembershipItemRenderer liveChatMembershipItemRenderer { get; set; }
        public LiveChatModeChangeMessageRenderer liveChatModeChangeMessageRenderer { get; set; }
        public LiveChatSponsorshipsGiftPurchaseAnnouncementRenderer liveChatSponsorshipsGiftPurchaseAnnouncementRenderer { get; set; }
        public LiveChatSponsorshipsGiftRedemptionAnnouncementRenderer liveChatSponsorshipsGiftRedemptionAnnouncementRenderer { get; set; }
        public LiveChatPlaceholderItemRenderer liveChatPlaceholderItemRenderer { get; set; }
    }

    public class AddChatItemAction
    {
        public Item item { get; set; }
        public string clientId { get; set; }
    }

    public class DeletedStateMessage
    {
        public List<Run> runs { get; set; }
    }

    public class MarkChatItemAsDeletedAction
    {
        public DeletedStateMessage deletedStateMessage { get; set; }
        public string targetItemId { get; set; }
    }

    public class Amount
    {
        public string simpleText { get; set; }
    }

    public class Renderer
    {
        public LiveChatPaidMessageRenderer liveChatPaidMessageRenderer { get; set; }
    }

    public class ShowLiveChatItemEndpoint
    {
        public Renderer renderer { get; set; }
        public string trackingParams { get; set; }
    }

    public class ShowItemEndpoint
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public ShowLiveChatItemEndpoint showLiveChatItemEndpoint { get; set; }
    }

    public class LiveChatTickerPaidMessageItemRenderer
    {
        public string id { get; set; }
        public Amount amount { get; set; }
        public long amountTextColor { get; set; }
        public long startBackgroundColor { get; set; }
        public long endBackgroundColor { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public int durationSec { get; set; }
        public ShowItemEndpoint showItemEndpoint { get; set; }
        public string authorExternalChannelId { get; set; }
        public int fullDurationSec { get; set; }
        public string trackingParams { get; set; }
    }

    public class AddLiveChatTickerItemAction
    {
        public Item item { get; set; }
        public string durationSec { get; set; }
    }

    public class UiActions
    {
        public bool hideEnclosingContainer { get; set; }
    }

    public class FeedbackEndpoint
    {
        public string feedbackToken { get; set; }
        public UiActions uiActions { get; set; }
    }

    public class ImpressionEndpoint
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public FeedbackEndpoint feedbackEndpoint { get; set; }
    }

    public class AcceptCommand
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public FeedbackEndpoint feedbackEndpoint { get; set; }
    }

    public class DismissCommand
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public FeedbackEndpoint feedbackEndpoint { get; set; }
    }

    public class PromoConfig
    {
        public string promoId { get; set; }
        public List<ImpressionEndpoint> impressionEndpoints { get; set; }
        public AcceptCommand acceptCommand { get; set; }
        public DismissCommand dismissCommand { get; set; }
    }

    public class DetailsText
    {
        public List<Run> runs { get; set; }
    }

    public class SuggestedPosition
    {
        public string type { get; set; }
    }

    public class DismissStrategy
    {
        public string type { get; set; }
    }

    public class TooltipRenderer
    {
        public PromoConfig promoConfig { get; set; }
        public string targetId { get; set; }
        public DetailsText detailsText { get; set; }
        public SuggestedPosition suggestedPosition { get; set; }
        public DismissStrategy dismissStrategy { get; set; }
        public string dwellTimeMs { get; set; }
        public string trackingParams { get; set; }
    }

    public class Tooltip
    {
        public TooltipRenderer tooltipRenderer { get; set; }
    }

    public class ShowLiveChatTooltipCommand
    {
        public Tooltip tooltip { get; set; }
    }

    public class Contents
    {
        public LiveChatTextMessageRenderer liveChatTextMessageRenderer { get; set; }
        public PollRenderer pollRenderer { get; set; }
    }

    public class LiveChatBannerRenderer
    {
        public Header header { get; set; }
        public Contents contents { get; set; }
        public string actionId { get; set; }
        public bool viewerIsCreator { get; set; }
        public string targetId { get; set; }
        public bool isStackable { get; set; }
        public string backgroundType { get; set; }
    }

    public class BannerRenderer
    {
        public LiveChatBannerRenderer liveChatBannerRenderer { get; set; }
    }

    public class AddBannerToLiveChatCommand
    {
        public BannerRenderer bannerRenderer { get; set; }
    }

    public class RemoveBannerForLiveChatCommand
    {
        public string targetActionId { get; set; }
    }

    public class UpdateLiveChatPollAction
    {
        public PollToUpdate pollToUpdate { get; set; }
    }

    public class CloseLiveChatActionPanelAction
    {
        public string targetPanelId { get; set; }
        public bool skipOnDismissCommand { get; set; }
    }

    public class Action
    {
        public string clickTrackingParams { get; set; }
        public AddChatItemAction addChatItemAction { get; set; }
        public MarkChatItemAsDeletedAction markChatItemAsDeletedAction { get; set; }
        public AddLiveChatTickerItemAction addLiveChatTickerItemAction { get; set; }
        public ShowLiveChatTooltipCommand showLiveChatTooltipCommand { get; set; }
        public AddBannerToLiveChatCommand addBannerToLiveChatCommand { get; set; }
        public ReplaceChatItemAction replaceChatItemAction { get; set; }
        public RemoveChatItemAction removeChatItemAction { get; set; }
        public RemoveBannerForLiveChatCommand removeBannerForLiveChatCommand { get; set; }
        public UpdateLiveChatPollAction updateLiveChatPollAction { get; set; }
        public CloseLiveChatActionPanelAction closeLiveChatActionPanelAction { get; set; }
        public ShowLiveChatActionPanelAction showLiveChatActionPanelAction { get; set; }
    }

    public class RemoveChatItemAction
    {
        public string targetItemId { get; set; }
    }

    public class ReplayChatItemAction
    {
        public List<Action> actions { get; set; }
        public string videoOffsetTimeMsec { get; set; }
    }

#pragma warning disable CS8981 // 類型名稱只包含小寫的 ASCII 字元。此類名稱可能保留供此語言使用。
    public class chat
#pragma warning restore CS8981 // 類型名稱只包含小寫的 ASCII 字元。此類名稱可能保留供此語言使用。
    {
        public ReplayChatItemAction replayChatItemAction { get; set; }
        public string clickTrackingParams { get; set; }
        public string videoOffsetTimeMsec { get; set; }
        public bool isLive { get; set; }
    }

    public class LiveChatModeChangeMessageRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public Icon icon { get; set; }
        public Text text { get; set; }
        public Subtext subtext { get; set; }
    }

    public class Subtext
    {
        public List<Run> runs { get; set; }
    }

    public class ReplaceChatItemAction
    {
        public string targetItemId { get; set; }
        public ReplacementItem replacementItem { get; set; }
    }

    public class ReplacementItem
    {
        public LiveChatTextMessageRenderer liveChatTextMessageRenderer { get; set; }
    }

    public class Choice
    {
        public Text text { get; set; }
        public bool selected { get; set; }
        public double voteRatio { get; set; }
        public VotePercentage votePercentage { get; set; }
        public SelectServiceEndpoint selectServiceEndpoint { get; set; }
    }

    public class MetadataText
    {
        public List<Run> runs { get; set; }
    }

    public class PollHeaderRenderer
    {
        public PollQuestion pollQuestion { get; set; }
        public Thumbnail thumbnail { get; set; }
        public MetadataText metadataText { get; set; }
        public string liveChatPollType { get; set; }
        public ContextMenuButton contextMenuButton { get; set; }
    }

    public class PollQuestion
    {
        public List<Run> runs { get; set; }
    }

    public class PollRenderer
    {
        public List<Choice> choices { get; set; }
        public string liveChatPollId { get; set; }
        public Header header { get; set; }
        public string trackingParams { get; set; }
    }

    public class PollToUpdate
    {
        public PollRenderer pollRenderer { get; set; }
    }

    public class SelectServiceEndpoint
    {
        public string clickTrackingParams { get; set; }
        public CommandMetadata commandMetadata { get; set; }
        public SendLiveChatVoteEndpoint sendLiveChatVoteEndpoint { get; set; }
    }

    public class SendLiveChatVoteEndpoint
    {
        public string @params { get; set; }
    }

    public class VotePercentage
    {
        public string simpleText { get; set; }
    }

    public class LiveChatActionPanelRenderer
    {
        public Contents contents { get; set; }
        public string id { get; set; }
        public string targetId { get; set; }
    }

    public class PanelToShow
    {
        public LiveChatActionPanelRenderer liveChatActionPanelRenderer { get; set; }
    }

    public class ShowLiveChatActionPanelAction
    {
        public PanelToShow panelToShow { get; set; }
    }

    public class LiveChatSponsorshipsGiftPurchaseAnnouncementRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public string authorExternalChannelId { get; set; }
        public Header header { get; set; }
    }

    public class LiveChatSponsorshipsGiftRedemptionAnnouncementRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
        public string authorExternalChannelId { get; set; }
        public AuthorName authorName { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public Message message { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
        public string trackingParams { get; set; }
    }

    public class LiveChatSponsorshipsHeaderRenderer
    {
        public AuthorName authorName { get; set; }
        public AuthorPhoto authorPhoto { get; set; }
        public PrimaryText primaryText { get; set; }
        public List<AuthorBadge> authorBadges { get; set; }
        public ContextMenuEndpoint contextMenuEndpoint { get; set; }
        public ContextMenuAccessibility contextMenuAccessibility { get; set; }
        public Image image { get; set; }
    }

    public class PrimaryText
    {
        public List<Run> runs { get; set; }
    }

    public class LiveChatPlaceholderItemRenderer
    {
        public string id { get; set; }
        public string timestampUsec { get; set; }
    }

    public class Thumbnail
    {
        public string url { get; set; }
        public int preference { get; set; }
        public string id { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string resolution { get; set; }
    }

    public class BorderImageProcessor
    {
        public ImageTint imageTint { get; set; }
    }

    public class BumperUserEduContentViewModel
    {
        public Text text { get; set; }
        public string trackingParams { get; set; }
        public Image image { get; set; }
    }

    public class ClientResource
    {
        public string imageName { get; set; }
        public long imageColor { get; set; }
    }

    public class Content
    {
        public BumperUserEduContentViewModel bumperUserEduContentViewModel { get; set; }
    }

    public class CreatorHeartButton
    {
        public CreatorHeartViewModel creatorHeartViewModel { get; set; }
    }

    public class CreatorHeartViewModel
    {
        public CreatorThumbnail creatorThumbnail { get; set; }
        public HeartedIcon heartedIcon { get; set; }
        public UnheartedIcon unheartedIcon { get; set; }
        public string heartedHoverText { get; set; }
        public string heartedAccessibilityLabel { get; set; }
        public string unheartedAccessibilityLabel { get; set; }
        public string engagementStateKey { get; set; }
        public Gradient gradient { get; set; }
        public LoggingDirectives loggingDirectives { get; set; }
    }

    public class CreatorThumbnail
    {
        public List<Source> sources { get; set; }
    }

    public class Gradient
    {
        public List<Source> sources { get; set; }
        public Processor processor { get; set; }
    }

    public class HeaderOverlayImage
    {
        public List<Thumbnail> thumbnails { get; set; }
    }

    public class HeartedIcon
    {
        public List<Source> sources { get; set; }
    }

    public class ImageTint
    {
        public long color { get; set; }
    }

    public class LiveChatItemBumperViewModel
    {
        public Content content { get; set; }
        public PdgPurchasedBumperLoggingDirectives pdgPurchasedBumperLoggingDirectives { get; set; }
    }

    public class LoggingDirectives
    {
        public string trackingParams { get; set; }
        public Visibility visibility { get; set; }
        public bool enableDisplayloggerExperiment { get; set; }
    }

    public class LowerBumper
    {
        public LiveChatItemBumperViewModel liveChatItemBumperViewModel { get; set; }
    }

    public class PdgPurchasedBumperLoggingDirectives
    {
        public LoggingDirectives loggingDirectives { get; set; }
    }

    public class PdgPurchasedNoveltyLoggingDirectives
    {
        public LoggingDirectives loggingDirectives { get; set; }
    }

    public class Processor
    {
        public BorderImageProcessor borderImageProcessor { get; set; }
    }

    public class Source
    {
        public ClientResource clientResource { get; set; }
        public string url { get; set; }
    }

    public class StyleRun
    {
        public int startIndex { get; set; }
        public int length { get; set; }
    }

    public class TimestampText
    {
        public string simpleText { get; set; }
    }

    public class UnheartedIcon
    {
        public List<Source> sources { get; set; }
        public Processor processor { get; set; }
    }

    public class Visibility
    {
        public string types { get; set; }
    }
}
