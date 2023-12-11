using System.Text.Json.Serialization;

namespace YoutubeLiveChatToDiscord.Models;

/* These POCOs are generated from the results by the code generator.
 * https://json2csharp.com/
 */

// Info myDeserializedClass = JsonConvert.DeserializeObject<Info>(myJsonResponse);

#pragma warning disable IDE1006 // 命名樣式
public class HttpHeaders
{
    [JsonPropertyName("User-Agent")]
    public string? UserAgent { get; set; }
    public string? Accept { get; set; }

    [JsonPropertyName("Accept-Encoding")]
    public string? AcceptEncoding { get; set; }

    [JsonPropertyName("Accept-Language")]
    public string? AcceptLanguage { get; set; }

    [JsonPropertyName("Sec-Fetch-Mode")]
    public string? SecFetchMode { get; set; }
}

public class Fragment
{
    public string? path { get; set; }
    public double duration { get; set; }
}

public class DownloaderOptions
{
    public int http_chunk_size { get; set; }
}

public class Format
{
    public string? format_id { get; set; }
    public string? url { get; set; }
    public string? manifest_url { get; set; }
    public double tbr { get; set; }
    public string? ext { get; set; }
    public double fps { get; set; }
    public string? protocol { get; set; }
    public int quality { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string? vcodec { get; set; }
    public string? acodec { get; set; }
    public string? dynamic_range { get; set; }
    public string? video_ext { get; set; }
    public string? audio_ext { get; set; }
    public double vbr { get; set; }
    public double abr { get; set; }
    public string? format { get; set; }
    public string? resolution { get; set; }
    public HttpHeaders? http_headers { get; set; }
    public string? format_note { get; set; }
    public List<Fragment>? fragments { get; set; }
    public int? asr { get; set; }
    public long? filesize { get; set; }
    public int? source_preference { get; set; }
    public string? language { get; set; }
    public int? language_preference { get; set; }
    public DownloaderOptions? downloader_options { get; set; }
    public string? container { get; set; }
    public double? filesize_approx { get; set; }
}

public class Thumbnail
{
    public string? url { get; set; }
    public int preference { get; set; }
    public string? id { get; set; }
    public int? height { get; set; }
    public int? width { get; set; }
    public string? resolution { get; set; }
}

public class LiveChat
{
    public string? url { get; set; }
    public string? video_id { get; set; }
    public string? ext { get; set; }
    public string? protocol { get; set; }
}

public class Subtitles
{
    public List<LiveChat>? live_chat { get; set; }
}

public class Af
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sq
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Am
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ar
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Hy
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Az
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Bn
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Eu
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Be
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class B
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Bg
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class My
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ca
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ceb
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class ZhHan
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class ZhHant
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Co
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Hr
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class C
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Da
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Nl
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class En
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Eo
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Et
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Fil
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Fi
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Fr
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Gl
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ka
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class De
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class El
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Gu
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ht
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ha
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Haw
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Iw
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Hi
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Hmn
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Hu
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Is
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ig
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Id
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ga
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class It
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ja
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Jv
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Kn
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Kk
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Km
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Rw
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ko
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ku
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ky
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Lo
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class La
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Lv
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Lt
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Lb
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mk
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mg
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class M
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ml
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mt
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mi
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mr
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Mn
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ne
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class No
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ny
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Or
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class P
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Fa
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Pl
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Pt
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Pa
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ro
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ru
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sm
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Gd
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sr
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sn
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sd
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Si
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sk
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sl
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class So
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class St
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class E
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Su
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sw
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Sv
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Tg
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ta
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Tt
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Te
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Th
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Tr
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Tk
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Uk
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ur
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Ug
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Uz
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Vi
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Cy
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Fy
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Xh
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Yi
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Yo
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class Zu
{
    public string? ext { get; set; }
    public string? url { get; set; }
    public string? name { get; set; }
}

public class AutomaticCaptions
{
    public List<Af>? af { get; set; }
    public List<Sq>? sq { get; set; }
    public List<Am>? am { get; set; }
    public List<Ar>? ar { get; set; }
    public List<Hy>? hy { get; set; }
    public List<Az>? az { get; set; }
    public List<Bn>? bn { get; set; }
    public List<Eu>? eu { get; set; }
    public List<Be>? be { get; set; }
    public List<B>? bs { get; set; }
    public List<Bg>? bg { get; set; }
    public List<My>? my { get; set; }
    public List<Ca>? ca { get; set; }
    public List<Ceb>? ceb { get; set; }

    [JsonPropertyName("zh-Hans")]
    public List<ZhHan>? ZhHans { get; set; }

    [JsonPropertyName("zh-Hant")]
    public List<ZhHant>? ZhHant { get; set; }
    public List<Co>? co { get; set; }
    public List<Hr>? hr { get; set; }
    public List<C>? cs { get; set; }
    public List<Da>? da { get; set; }
    public List<Nl>? nl { get; set; }
    public List<En>? en { get; set; }
    public List<Eo>? eo { get; set; }
    public List<Et>? et { get; set; }
    public List<Fil>? fil { get; set; }
    public List<Fi>? fi { get; set; }
    public List<Fr>? fr { get; set; }
    public List<Gl>? gl { get; set; }
    public List<Ka>? ka { get; set; }
    public List<De>? de { get; set; }
    public List<El>? el { get; set; }
    public List<Gu>? gu { get; set; }
    public List<Ht>? ht { get; set; }
    public List<Ha>? ha { get; set; }
    public List<Haw>? haw { get; set; }
    public List<Iw>? iw { get; set; }
    public List<Hi>? hi { get; set; }
    public List<Hmn>? hmn { get; set; }
    public List<Hu>? hu { get; set; }
    public List<Is>? @is { get; set; }
    public List<Ig>? ig { get; set; }
    public List<Id>? id { get; set; }
    public List<Ga>? ga { get; set; }
    public List<It>? it { get; set; }
    public List<Ja>? ja { get; set; }
    public List<Jv>? jv { get; set; }
    public List<Kn>? kn { get; set; }
    public List<Kk>? kk { get; set; }
    public List<Km>? km { get; set; }
    public List<Rw>? rw { get; set; }
    public List<Ko>? ko { get; set; }
    public List<Ku>? ku { get; set; }
    public List<Ky>? ky { get; set; }
    public List<Lo>? lo { get; set; }
    public List<La>? la { get; set; }
    public List<Lv>? lv { get; set; }
    public List<Lt>? lt { get; set; }
    public List<Lb>? lb { get; set; }
    public List<Mk>? mk { get; set; }
    public List<Mg>? mg { get; set; }
    public List<M>? ms { get; set; }
    public List<Ml>? ml { get; set; }
    public List<Mt>? mt { get; set; }
    public List<Mi>? mi { get; set; }
    public List<Mr>? mr { get; set; }
    public List<Mn>? mn { get; set; }
    public List<Ne>? ne { get; set; }
    public List<No>? no { get; set; }
    public List<Ny>? ny { get; set; }
    public List<Or>? or { get; set; }
    public List<P>? ps { get; set; }
    public List<Fa>? fa { get; set; }
    public List<Pl>? pl { get; set; }
    public List<Pt>? pt { get; set; }
    public List<Pa>? pa { get; set; }
    public List<Ro>? ro { get; set; }
    public List<Ru>? ru { get; set; }
    public List<Sm>? sm { get; set; }
    public List<Gd>? gd { get; set; }
    public List<Sr>? sr { get; set; }
    public List<Sn>? sn { get; set; }
    public List<Sd>? sd { get; set; }
    public List<Si>? si { get; set; }
    public List<Sk>? sk { get; set; }
    public List<Sl>? sl { get; set; }
    public List<So>? so { get; set; }
    public List<St>? st { get; set; }
    public List<E>? es { get; set; }
    public List<Su>? su { get; set; }
    public List<Sw>? sw { get; set; }
    public List<Sv>? sv { get; set; }
    public List<Tg>? tg { get; set; }
    public List<Ta>? ta { get; set; }
    public List<Tt>? tt { get; set; }
    public List<Te>? te { get; set; }
    public List<Th>? th { get; set; }
    public List<Tr>? tr { get; set; }
    public List<Tk>? tk { get; set; }
    public List<Uk>? uk { get; set; }
    public List<Ur>? ur { get; set; }
    public List<Ug>? ug { get; set; }
    public List<Uz>? uz { get; set; }
    public List<Vi>? vi { get; set; }
    public List<Cy>? cy { get; set; }
    public List<Fy>? fy { get; set; }
    public List<Xh>? xh { get; set; }
    public List<Yi>? yi { get; set; }
    public List<Yo>? yo { get; set; }
    public List<Zu>? zu { get; set; }
}

public class Info
{
    public string? id { get; set; }
    public string? title { get; set; }
    public List<Format>? formats { get; set; }
    public List<Thumbnail>? thumbnails { get; set; }
    public string? thumbnail { get; set; }
    public string? description { get; set; }
    public string? upload_date { get; set; }
    public string? uploader { get; set; }
    public string? uploader_id { get; set; }
    public string? uploader_url { get; set; }
    public string? channel_id { get; set; }
    public string? channel_url { get; set; }
    public int view_count { get; set; }
    public int age_limit { get; set; }
    public string? webpage_url { get; set; }
    public List<string>? categories { get; set; }
    public List<string>? tags { get; set; }
    public bool playable_in_embed { get; set; }
    public bool is_live { get; set; }
    public bool was_live { get; set; }
    public string? live_status { get; set; }
    public int release_timestamp { get; set; }
    public Subtitles? subtitles { get; set; }
    public int like_count { get; set; }
    public string? channel { get; set; }
    public int channel_follower_count { get; set; }
    public string? availability { get; set; }
    public string? webpage_url_basename { get; set; }
    public string? extractor { get; set; }
    public string? extractor_key { get; set; }
    public string? display_id { get; set; }
    public string? release_date { get; set; }
    public string? fulltitle { get; set; }
    public int epoch { get; set; }
    public string? format_id { get; set; }
    public string? url { get; set; }
    public string? manifest_url { get; set; }
    public double? tbr { get; set; }
    public string? ext { get; set; }
    public double? fps { get; set; }
    public string? protocol { get; set; }
    public int? quality { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public string? vcodec { get; set; }
    public string? acodec { get; set; }
    public string? dynamic_range { get; set; }
    public string? video_ext { get; set; }
    public string? audio_ext { get; set; }
    public double? vbr { get; set; }
    public double? abr { get; set; }
    public string? format { get; set; }
    public string? resolution { get; set; }
    public HttpHeaders? http_headers { get; set; }
    public int? duration { get; set; }
    public AutomaticCaptions? automatic_captions { get; set; }
    public string? duration_string { get; set; }
    public string? format_note { get; set; }
    public int? filesize_approx { get; set; }
    public int? asr { get; set; }
}
#pragma warning restore IDE1006 // 命名樣式

