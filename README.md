# YoutubeLiveChatToDiscord

將Youtube聊天室塞進Discord Webhook，每5分鐘檢查一次\
此工具設計來監控FreeChat，不適合用在用大量留言的狀況\
可能會卡到Discord Webhook的冷卻間隔，造成轉送速度跟不上留言速度

## Docker

```sh
docker run -rm ghcr.io/jim60105/youtubelivechattodiscord [VideoId] [Discord_Webhook_Url]
```
