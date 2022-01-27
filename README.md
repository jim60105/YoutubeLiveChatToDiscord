# YoutubeLiveChatToDiscord

將Youtube聊天室塞進Discord Webhook

|Youtube Live Chat|| Discord Webhook |
|-|-|-|
|![image](https://user-images.githubusercontent.com/16995691/151437691-d525b724-80b6-4e48-851c-2c38aaf1756f.png) |➡️| ![image](https://user-images.githubusercontent.com/16995691/151438025-d0c4a2de-6845-4d64-93db-89afb2f98e45.png)|


- 不適合用在有大量留言的狀況，此工具是設計來監控 FreeChat\
    此工具最高每兩秒打一次 discord webhook ，可能造成轉送速度跟不上留言速度
    > Discord 方面的限制為，同一頻道中每分鐘可呼叫 Webhook 30次 [ref](https://twitter.com/lolpython/status/967621046277820416)
- 底層使用 yt-dlp 來抓資料，並不是 youtube api ，沒有額度問題
- 此工具在閒置時，讀取 yt-dlp 寫的 json 檔案間隔為10秒

## Docker

需傳入兩個參數

- 影片ID
- Discord Webhook網址

```sh
docker run -rm ghcr.io/jim60105/youtubelivechattodiscord [Video_Id] [Discord_Webhook_Url]
```
