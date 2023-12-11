using System.Text.Json;
using System.Text.Json.Serialization;
using YoutubeLiveChatToDiscord.Models;

// Must read:
// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-8-0
[JsonSourceGenerationOptions(WriteIndented = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip)]
[JsonSerializable(typeof(Info.info))]
[JsonSerializable(typeof(Chat.chat))]
[JsonSerializable(typeof(Info.Thumbnail), TypeInfoPropertyName = "InfoThumbnail")]
[JsonSerializable(typeof(Chat.Thumbnail), TypeInfoPropertyName = "ChatThumbnail")]
[JsonSerializable(typeof(List<Chat.Thumbnail>), TypeInfoPropertyName = "ChatThumbnailList")]
[JsonSerializable(typeof(List<Info.Thumbnail>), TypeInfoPropertyName = "InfoThumbnailList")]
internal partial class SourceGenerationContext : JsonSerializerContext { }
