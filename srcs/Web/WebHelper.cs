/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.Security.Cryptography.Certificates;
using Windows.UI;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using WinUniversalTool.Settings;

namespace WinUniversalTool.WebViewer
{
    public static class WebHelper
    {
        public static double DefaultZoom = 100;
        public static string EMPTY_FAVICON = "ms-appx:///Assets/Icons/Windows11/browser.bmp";
    
        public static bool isTextChangedByNavigate = false;
        public static EventHandler SaveSettingsHandler;
        //Cache Files
        public static string BookmarksFile = "bookmarks.db";
        public static string KeywordsFile = "keywords.db";
        public static string AdsFile = "adsfilter.db";
        public static string HistoryFile = "history.db";
        public static string BlockFile = "block.db";
        public static string SettingsFile = "settings.db";
        public static string ExtensionsFolder = "extensions";
        public static SettingsBlock SettingsBlock;

        //Regex Match
        public static string URLPattern = @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.(?<ext>[a-zA-Z0-9()]{1,6}\b)([-a-zA-Z0-9()@:%_\+.~#?&\/\/=]*)";
        public static string URLPatternBatch = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.(?<ext>[a-zA-Z0-9()]{1,6}\b)([-a-zA-Z0-9@:%_\+.~#?&\/\/=]*)";
        public static string URLPatternWithoutHttp = @"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.(?<ext>[a-zA-Z0-9()]{1,6}\b)([-a-zA-Z0-9()@:%_\+.~#?&\/\/=]*)$";
        public static string URLFilePattern = @"https?:\/\/.*\/(?<file>[^\/\\&\?:]+\.[a-z3-9]\w{1,10}(?=([\?&].*$|$)))";
        public static string[] DomainsExtenstions = { ".com", ".online", ".edu", ".gov", ".int", ".mil", ".net", ".org", ".biz", ".cat", ".cam", ".ac", ".ad", ".ae", ".af", ".ag", ".ai", ".al", ".am", ".ao", ".aq", ".ar", ".as", ".at", ".au", ".aw", ".axz", ".az", ".ba", ".bb", ".bd", ".be", ".bf", ".bg", ".bh", ".bi", ".bj", ".bm", ".bn", ".bo", ".bq", ".br", ".bs", ".bt", ".bv", ".bw", ".by", ".bz", ".ca", ".cc", ".cd", ".cf", ".cg", ".ch", ".ci", ".ck", ".cl", ".cm", ".cn", ".co", ".cr", ".cu", ".cv", ".cw", ".cx", ".cy", ".cz", ".de", ".dj", ".dk", ".dm", ".do", ".dz", ".ec", ".ee", ".eg", ".er", ".es", ".et", ".eu", ".fi", ".fj", ".fk", ".fm", ".fo", ".fr", ".ga", ".gb", ".gd", ".ge", ".gf", ".gg", ".gh", ".gi", ".gl", ".gm", ".gn", ".gp", ".gq", ".gr", ".gs", ".gt", ".gw", ".gy", ".hk", ".hm", ".hn", ".hr", ".ht", ".hu", ".id", ".ie", ".il", ".im", ".in", ".io", ".iq", ".ir", ".is", ".it", ".je", ".jm", ".jo", ".jp", ".ke", ".kg", ".kh", ".ki", ".km", ".kn", ".kp", ".kr", ".kw", ".ky", ".kz", ".la", ".lb", ".lc", ".li", ".lk", ".lr", ".ls", ".lt", ".lu", ".lv", ".ly", ".ma", ".mc", ".md", ".me", ".mg", ".mh", ".mk", ".ml", ".mm", ".mn", ".mo", ".mp", ".mq", ".mr", ".ms", ".mt", ".mu", ".mv", ".mw", ".mx", ".my", ".mz", ".na", ".nc", ".ne", ".nf", ".ng", ".ni", ".nl", ".no", ".np", ".nr", ".nu", ".nz", ".om", ".pa", ".pe", ".pf", ".pg", ".ph", ".pk", ".pl", ".pm", ".pn", ".pr", ".ps", ".pt", ".pw", ".py", ".qa", ".re", ".ro", ".rs", ".ru", ".rw", ".sa", ".sb", ".sc", ".sd", ".se", ".sh", ".si", ".sj", ".sk", ".sl", ".sm", ".sn", ".so", ".sr", ".ss", ".st", ".su", ".sv", ".sx", ".sy", ".sz", ".tc", ".td", ".tf", ".tg", ".th", ".tj", ".tk", ".tl", ".tm", ".tn", ".to", ".tr", ".tt", ".tv", ".tw", ".tz", ".ua", ".ug", ".uk", ".us", ".uy", ".uz", ".va", ".vc", ".ve", ".vg", ".vi", ".vn", ".vu", ".wf", ".ws", ".ye", ".yt", ".za", ".zm", ".zw" };
        public static string[] ExcludedExtentions = { ".html", ".htm", ".php", ".phpx", ".asp", ".aspx", ".jsp", ".srf" };

        public static bool isLinkFile(string url, bool NoHttpCheck = true)
        {
            try
            {
                Match m = Regex.Match(url, NoHttpCheck ? BasicFileName : URLPatternWithoutHttp, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }
        public static string SolveURL(string url)
        {
            try
            {
                if (url == null)
                {
                    return "blank:page";
                }
                url = url.Replace(@"\r", "").Trim();
                var DomainsExtenstion = "-";
                try
                {
                    Match m = Regex.Match(url, URLPatternWithoutHttp, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            DomainsExtenstion = m.Groups["ext"].ToString();
                        }
                    }
                }
                catch (Exception e)
                {

                }
                
                url = url.Replace("http//", "http://");
                url = url.Replace("http/", "http://");
                url = url.Replace("http/", "http://");
                if (!url.Contains("http://"))
                {
                    url = url.Replace("http:/", "http://");
                }
                url = url.Replace("htp:", "http:");
                url = url.Replace("htt:", "http:");

                url = url.Replace("https//", "https://");
                url = url.Replace("https/", "https://");
                url = url.Replace("https/", "https://");
                if (!url.Contains("https://"))
                {
                    url = url.Replace("https:/", "https://");
                }
                url = url.Replace("htps:", "https:");
                url = url.Replace("htts:", "https:");

                if (!url.EndsWith("/"))
                {
                    if (url.EndsWith(DomainsExtenstion))
                    {
                        url = $"{url}/";
                    }
                }
            }
            catch (Exception e)
            {

            }
            return url;
        }

        public static string IPMatch = @"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        public static bool isIP(string url)
        {
            try
            {
                var mb = Regex.Match(url, IPMatch, RegexOptions.IgnoreCase);
                if (mb.Success)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public static string ExtractIP(string url)
        {
            try
            {
                var mb = Regex.Match(url, IPMatch, RegexOptions.IgnoreCase);
                if (mb.Success)
                {
                    return mb.Value;
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        public static bool IsAllowedUri(string url, bool noHttpCheck = false)
        {
            try
            {
                if (url != null)
                {
                    Match mb = Regex.Match(url, URLPattern, RegexOptions.IgnoreCase);
                    if (noHttpCheck)
                    {
                        mb = Regex.Match(url, URLPatternWithoutHttp, RegexOptions.IgnoreCase);
                    }
                    if (mb.Success)
                    {
                        return true;
                    }
                    else if (url.StartsWith("blank:") || url.StartsWith("error:") || url.StartsWith("security:"))
                    {
                        return true;
                    }
                    else
                    {
                        mb = Regex.Match(url, IPMatch, RegexOptions.IgnoreCase);
                        if (mb.Success)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }
        public static Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
        public static string UpdatesResponses = "";
        public static async Task<T> GetResultRequest<T>(string apiLink, bool updateRespose = false)
        {

            try
            {
                try
                {
                    client.Dispose();
                    client = new Windows.Web.Http.HttpClient();
                }
                catch (Exception e)
                {

                }
                Uri uri = null;
                try
                {
                    uri = new Uri(apiLink);
                }
                catch (Exception e)
                {

                }
                if (uri != null)
                {
                    Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        var dictionaryList = JsonConvert.DeserializeObject<T>(result);
                        if (updateRespose)
                        {
                            UpdatesResponses += result;
                        }
                        return dictionaryList;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return default(T);
        }

        public static async Task<T> GetResultRequest2<T>(string apiLink)
        {

            try
            {
                try
                {
                    client.Dispose();
                    client = new Windows.Web.Http.HttpClient();
                }
                catch (Exception e)
                {

                }
                Uri uri = null;
                try
                {
                    uri = new Uri(apiLink);
                }
                catch (Exception e)
                {

                }
                if (uri != null)
                {
                    Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        var dictionaryList = JsonConvert.DeserializeObject<T>(result);
                        return dictionaryList;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return default(T);
        }

        public static async Task<T> GetResultRequest<T>(string apiLink, HttpCredentialsHeaderValue authenticationHeaderValue, List<KeyValuePair<string, string>> postValues = null, string acceptHeader = "application/json")
        {

            try
            {
                try
                {
                    client.Dispose();
                    client = new Windows.Web.Http.HttpClient();
                    if (authenticationHeaderValue != null)
                    {
                        client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                    }
                }
                catch (Exception e)
                {

                }
                Uri uri = null;
                try
                {
                    uri = new Uri(apiLink);
                }
                catch (Exception e)
                {

                }
                if (uri != null)
                {
                    Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        //Helpers.Logger($"Response for {apiLink}:\n{result}");
                        var dictionaryList = JsonConvert.DeserializeObject<T>(result);
                        return dictionaryList;
                    }
                    else
                    {
                        Helpers.Logger($"Response for {apiLink}:\n{response.StatusCode}");
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Logger($"Response for {apiLink}:\n{e.Message}");
            }
            return default(T);
        }
        public static async Task<string> GetResultRequestString(string apiLink)
        {

            try
            {
                try
                {
                    client.Dispose();
                    client = new Windows.Web.Http.HttpClient();
                }
                catch (Exception e)
                {

                }
                Uri uri = null;
                try
                {
                    uri = new Uri(apiLink);
                }
                catch (Exception e)
                {

                }
                if (uri != null)
                {
                    Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public static async Task<long> GetFileSize(string url)
        {
            long FileSize = 0;
            try
            {
                var fileLink = new Uri(url);
                var _client = new Windows.Web.Http.HttpClient();
                var response = await _client.GetAsync(fileLink, Windows.Web.Http.HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    var FileName = response.Content.Headers?.ContentDisposition?.FileName;
                    try
                    {
                        FileSize = (long)response.Content.Headers.ContentLength.GetValueOrDefault();
                    }
                    catch (Exception e)
                    {
                        FileSize = 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return FileSize;
        }
        public static async Task<Windows.Web.Http.HttpResponseMessage> GetResponse(string url, CancellationToken cancellationToken, HttpCredentialsHeaderValue authenticationHeaderValue = null, bool showError = true, bool returnResponseAnyway = false)
        {
            var _client = new Windows.Web.Http.HttpClient();
            if (authenticationHeaderValue != null)
            {
                _client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            }
            Windows.Web.Http.HttpResponseMessage response = null;
            try
            {
                url = url.Replace("wut::", "");
                if (IsAllowedUri(url))
                {
                    Uri uri = null;
                    try
                    {
                        uri = new Uri(url);
                    }
                    catch (Exception e)
                    {

                    }
                    if (uri != null)
                    {
                        response = await _client.GetAsync(uri, Windows.Web.Http.HttpCompletionOption.ResponseHeadersRead).AsTask(cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            if (!returnResponseAnyway)
                            {
                                return null;
                            }
                            else
                            {
                                return response;
                            }
                        }

                        if (response.Content.Headers.ContentDisposition == null)
                        {
                            //IEnumerable<string> contentDisposition;
                            /*if (response.Content.Headers.TryGetValues("Content-Disposition", out contentDisposition))
                            {
                                response.Content.Headers.ContentDisposition = Windows.Web.Http.ContentDispositionHeaderValue.Parse(contentDisposition.ToArray()[0].TrimEnd(';').Replace("\"", ""));
                            }*/
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    var HostName = "";
                    url = url.Replace("wut::", "");
                    if (IsAllowedUri(url))
                    {
                        Uri uri = null;
                        try
                        {
                            uri = new Uri(url);
                            HostName = uri.Host;
                        }
                        catch (Exception e)
                        {

                        }

                    }
                    var messageError = ex.Message;
                    if (HostName.Length > 0)
                    {
                        messageError = $"{messageError}\nHost: {HostName}";
                    }
                    try
                    {
                        messageError = String.Join("\n", messageError.Split('\n').Distinct(StringComparer.CurrentCultureIgnoreCase));
                        messageError = messageError.Replace("\n\r\n\r", "\n\r");
                        messageError = messageError.Replace("\n\n", "\n");
                    }
                    catch (Exception exx)
                    {

                    }
                    try
                    {
                        Helpers.PlayNotificationSoundDirect("error.mp3");
                    }
                    catch (Exception ee)
                    {

                    }
                    LocalNotificationData localNotificationData = new LocalNotificationData();
                    localNotificationData.icon = SegoeMDL2Assets.Error;
                    localNotificationData.type = Colors.Tomato;
                    localNotificationData.message = messageError;
                    localNotificationData.time = 7;
                    Helpers.pushLocalNotification(null, localNotificationData);

                    //Helpers.ShowToastNotification("Error", ex.Message);
                }
            }
            return response;
        }
        public static async Task WaitAsync<T>(this TaskCompletionSource<T> tcs, CancellationToken ctok)
        {

            CancellationTokenSource cts = null;
            CancellationTokenSource linkedCts = null;

            try
            {
                cts = new CancellationTokenSource();
                linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctok);

                var exitTok = linkedCts.Token;

                Func<Task> listenForCancelTaskFnc = async () =>
                {
                    await Task.Delay(-1, exitTok).ConfigureAwait(false);
                };

                var cancelTask = listenForCancelTaskFnc();

                await Task.WhenAny(new Task[] { tcs.Task, cancelTask }).ConfigureAwait(false);

                cts.Cancel();

            }
            finally
            {

                if (linkedCts != null) linkedCts.Dispose();

            }

        }

        public static string UserAgentMobile = "Mozilla/5.0 (Android 10.0; Mobile; rv:84.0) Gecko/20100101 Firefox/88.0";
        public static async Task<Windows.Web.Http.HttpResponseMessage> GetResponse(Uri uri, CancellationToken cancellationToken, List<KeyValuePair<string, string>> postValues = null, string acceptHeader = "application/json", HttpCredentialsHeaderValue authenticationHeaderValue = null)
        {
            try
            {
                Windows.Web.Http.HttpRequestMessage request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Post, uri);
                request.Headers.UserAgent.Clear();
                HttpProductInfoHeaderValue userAgent = new HttpProductInfoHeaderValue(UserAgentMobile);
                request.Headers.UserAgent.Add(userAgent);

                if (postValues?.Count > 0)
                {
                    HttpFormUrlEncodedContent httpFormUrlEncodedContent = new HttpFormUrlEncodedContent(postValues.ToArray());
                    request.Content = httpFormUrlEncodedContent;
                    postValues = null;
                }
                HttpMediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue = new HttpMediaTypeWithQualityHeaderValue(acceptHeader);
                request.Headers.Accept.Add(mediaTypeWithQualityHeaderValue);
                if (authenticationHeaderValue != null)
                {
                    request.Headers.Authorization = authenticationHeaderValue;
                }
                var _httpClient = new Windows.Web.Http.HttpClient();
                var result = await _httpClient.SendRequestAsync(request);

                return result;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public static async Task<long> GetFileSizeAsync(Uri url)
        {
            try
            {
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<Stream> GetStreamForLink(string url, CancellationToken cancellationToken, HttpCredentialsHeaderValue authenticationHeaderValue = null)
        {
            Stream stream = null;
            try
            {
                var response = await WebHelper.GetResponse(url, cancellationToken, authenticationHeaderValue);
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
            }
            return stream;
        }

        public static string YoutubeFileName = @"(?<fileName>videoplayback).*(mime=video%2F(?<type>\w+))\&";
        public static string YoutubeFileType = @"(?<fileName>\w+).*(mime=video%2F(?<type>\w+))\&";
        public static string GeneralFileName = @"(?<fileName>[\w+\s+_\d+.\-()!@#$%^&_+=';~]+)\?.*";
        public static string BasicFileName = @"(?<fileName>[\w+\s+_\d+.\-()!@#$%^&_+=';~]+)\.\w+";
        public static string VerifyFileName = @"^[\w+\s+_\d+.\-()!@#$%^&_+=';~]+\.\w+$";
        public static string TypeSelector = @".*(?<type>\.\w+)$";
        public static string YoutubeShort = @"youtu\.be\/(?<id>.*)";

        public static string checkYoutubeLink(string url)
        {
            string fullLink = "";
            try
            {

                /*else if (url.ToLower().StartsWith("https://vimeo.com/")) LATER
                {
                    fullLink = url;
                }*/
                if (url.ToLower().Contains("youtube.com/watch"))
                {
                    fullLink = url;
                }
                else
                {
                    try
                    {
                        Match m = Regex.Match(url, YoutubeShort, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            if (m.Groups != null && m.Groups.Count > 0)
                            {
                                fullLink = $"https://www.youtube.com/watch?v={m.Groups["id"]}";
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {

            }
            return fullLink;
        }
        public static string FileNameExtractor(string NameData, string ForceType = "", bool dontValidate = false)
        {
            var timeAdd = "";
            try
            {
                timeAdd = DateTime.Now.ToString().Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_");
            }
            catch (Exception ex)
            {

            }

            if (NameData.EndsWith("/"))
            {
                NameData = NameData.Substring(0, NameData.Length - 1);
            }

            string fixedName = GetFixedName(ForceType);


            if (NameData.Length == 0)
            {
                NameData = $"{fixedName}_{timeAdd}{ForceType}";
                if (dontValidate)
                {
                    return NameData;
                }
            }
            try
            {
                bool FoundMatch = false;
                Match m = Regex.Match(NameData, YoutubeFileName, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (m.Groups != null && m.Groups.Count > 0)
                    {
                        NameData = $"{m.Groups["fileName"]}.{m.Groups["type"]}";
                        if (NameData.Equals($"videoplayback.{m.Groups["type"]}"))
                        {
                            NameData = $"{m.Groups["fileName"]}_{timeAdd}.{m.Groups["type"]}";
                        }
                        FoundMatch = true;
                    }
                }
                else
                {
                    m = Regex.Match(NameData, YoutubeFileType, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            NameData = $"videoplayback_{timeAdd}.{m.Groups["type"]}";
                            FoundMatch = true;
                        }
                    }
                }

                if (!FoundMatch)
                {
                    m = Regex.Match(NameData, GeneralFileName, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            NameData = $"{m.Groups["fileName"]}";
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            try
            {
                Match m = Regex.Match(NameData, VerifyFileName, RegexOptions.IgnoreCase);
                if (!m.Success)
                {
                    m = Regex.Match(NameData, TypeSelector, RegexOptions.IgnoreCase);
                    if (ForceType.Length == 0 && m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            fixedName = GetFixedName(m.Groups["type"].Value);
                            NameData = $"{fixedName}_{timeAdd}{m.Groups["type"]}";
                        }
                        else
                        {
                            NameData = $"{fixedName}_{timeAdd}{ForceType}";
                        }
                    }
                    else
                    {
                        NameData = $"{fixedName}_{timeAdd}{ForceType}";
                    }
                }
                else
                {
                    m = Regex.Match(NameData, TypeSelector, RegexOptions.IgnoreCase);
                    if (ForceType.Length > 0 && m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            if (DomainsExtenstions.Contains($"{m.Groups["type"]}"))
                            {
                                NameData = $"{fixedName}_{timeAdd}{ForceType}";
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
            }
            return NameData;
        }

        public static string GetFixedName(string ForceType)
        {
            string fixedName = "file";
            try
            {
                if (new string[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".ico", ".svg" }.Contains(ForceType))
                {
                    fixedName = "image";
                }
                else if (new string[] { ".mp4", ".mkv", ".avi", ".mpeg", ".web4", ".wmv" }.Contains(ForceType))
                {
                    fixedName = "video";
                }
                else if (new string[] { ".mp3", ".wav", ".aif", ".ogg", ".wma" }.Contains(ForceType))
                {
                    fixedName = "audio";
                }
                else if (new string[] { ".html", ".htm", ".asp", ".aspx", ".php" }.Contains(ForceType))
                {
                    fixedName = "page";
                }
                else
                {
                    fixedName = "file";
                }
            }
            catch (Exception e)
            {
                fixedName = "file";
            }
            return fixedName;
        }
        public static string isYoutubeDownloadLink(string url)
        {
            string NameData = null;
            try
            {
                Match m = Regex.Match(url, YoutubeFileName, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (m.Groups != null && m.Groups.Count > 0)
                    {
                        NameData = $"{m.Groups["fileName"]}.{m.Groups["type"]}";
                    }
                }
            }
            catch (Exception e)
            {

            }
            return NameData;
        }


        public static IDictionary<string, string> MIMETypes = new Dictionary<string, string>() {
        #region Big freaking list of mime types
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion
         };
        public static string GetExtensionByMIMEType(string mime)
        {
            try
            {
                foreach (var MIMIItem in MIMETypes)
                {
                    if (MIMIItem.Value.Equals(mime))
                    {
                        return MIMIItem.Key;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return "";
        }

        //Get file name using HTTP Response
        public static async Task<string> getFileNameWithResponse(string link)
        {
            var FileName = "test.ffu";
            var cancellationTokenSource = new CancellationTokenSource();
            Uri fileLink = new Uri(link);
            var _client = new Windows.Web.Http.HttpClient();
            var response = await _client.GetAsync(fileLink, Windows.Web.Http.HttpCompletionOption.ResponseHeadersRead).AsTask(cancellationTokenSource.Token);
            if (response.IsSuccessStatusCode)
            {
                FileName = response.Content.Headers?.ContentDisposition?.FileName;
            }
            return FileName;
        }

        //Get file name from link using Regex
        public static string getFileNameWithRegex(string link)
        {
            var FileName = "test.ffu";
            var pattern = @"\w+\/(?<fileName>[\w+\s+_\d+.\-()!@#$%^&_+=';]+\.\w+)";
            var m = Regex.Match(link, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                if (m.Groups != null && m.Groups.Count > 0)
                {
                    FileName = $"{m.Groups["fileName"]}";
                }
            }
            return FileName;
        }

    }
}
