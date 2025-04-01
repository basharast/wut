/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using DamienG.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using WinUniversalTool.Views;
using WebViewComponents;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;
using Windows.UI;
using Windows.UI.Xaml.Markup;
using Windows.Security.Cryptography.Certificates;
using Windows.Web;
using Microsoft.Tools.WindowsDevicePortal;
using Octokit;

namespace WinUniversalTool.WebViewer
{
    public class WebObject : BindableBase
    {
        public ObservableCollection<string> History;
        public string SessionID = "";
        public string FirstCallURL = "";
        public string CurrentURL = "";
        public string RealCurrentURL = "";
        public string CurrentURLTemp = "";
        public string CurrentHost = "";

        //should I remove this?
        public bool xamarinWebView = false;
        public bool XamarinWebView
        {
            get
            {
                return false;
            }
            set
            {
                xamarinWebView = value;
            }
        }
        double zoomLevel = WebHelper.DefaultZoom;
        int blocks = 0;
        int adblocks = 0;
        public int totalBlocked
        {
            get
            {
                return blocks;
            }
            set
            {
                blocks = value;
                RaisePropertyChanged(nameof(totalBlocked));
                RaisePropertyChanged("ShowTotalBlocked");
                Helpers.UpdateBindings(null, EventArgs.Empty);
            }
        }
        public int totalAdBlocked
        {
            get
            {
                return adblocks;
            }
            set
            {
                adblocks = value;
                RaisePropertyChanged(nameof(totalAdBlocked));
                RaisePropertyChanged("ShowTotalAdBlocked");
                Helpers.UpdateBindings(null, EventArgs.Empty);
            }
        }
        public double ZoomLevel
        {
            get
            {
                return zoomLevel;
            }
            set
            {
                zoomLevel = value;
                _ = ChangeZoom(zoomLevel);
            }
        }
        string certificateProvider = "SSL Certificate";
        public string CertificateProvider
        {
            get
            {
                if (certificateProvider != null)
                {
                    return certificateProvider;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                certificateProvider = value;
            }
        }
        public List<KeyValuePair<string, string>> postValues;

        public string MobileAgent = WebHelper.UserAgentMobile;
        public string DesktopAgent = WebHelper.UserAgentDesktop;

        ObservableCollection<Extension> Extensions = null;
        [JsonIgnore]
        public Visibility ItemsIconsBlock
        {
            get
            {
                if (Helpers.DisableAllIcons)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        public string ViewVersion
        {
            get
            {
                string version = "Mobile";
                if (!isMobileVersion)
                {
                    version = "Desktop";
                }
                return version;
            }
        }

        //Console Options
        public bool ConsoleErrors = true;
        public bool ConsoleWarnings = false;
        public bool ConsoleLog = false;
        public bool ConsoleInfo = false;
        public bool ConsoleXHR = false;
        bool lockDown = false;
        public bool LockDown
        {
            get
            {
                return lockDown;
            }

            set
            {
                lockDown = value;
                RaisePropertyChanged(nameof(LockDown));
                RaisePropertyChanged("LockDownCommandState");
                if (lockDown)
                {
                    var LockDownHandler = "window.lockdown = true";
                    _ = SendCommand(LockDownHandler, false);
                    AddToConsoleList("warn", "Lock Down Active");
                }
                else
                {
                    var LockDownHandler = "window.lockdown = false";
                    _ = SendCommand(LockDownHandler, false);
                    AddToConsoleList("warn", "Lock Down Disabled");
                }
            }
        }

        public bool TurboMode = WebHelper.TurboMode;

        public bool LoadRequestCanceled = false;
        public bool isLoading = false;
        public bool isActive = false;
        public bool isTranslateMode = false;
        public bool isConsoleOpen = false;
        public bool FileDownloadInProgress = false;
        public bool ConsoleStringify = WebHelper.ConsoleStringify;
        public bool ConsoleAutoScroll = WebHelper.ConsoleAutoScroll;
        public bool isJavascriptEnabled = WebHelper.EnableJavascript;
        public bool isIndexedDB = WebHelper.IndexedDB;
        public bool mobileVersion = WebHelper.UseMobileVersion;
        public bool HandleAllErrors = WebHelper.HandleAllErrors;
        public bool gPUCacheModeState = Helpers.GPUCacheMode;
        public bool GPUCacheModeState
        {
            get
            {
                return gPUCacheModeState;
            }
            set
            {
                gPUCacheModeState = value;
                if (gPUCacheModeState)
                {
                    webView.CacheMode = new BitmapCache();
                }
                else
                {
                    webView.CacheMode = null;
                }
            }
        }
        public bool isMobileVersion
        {
            get
            {
                return mobileVersion;
            }
            set
            {
                try
                {
                    mobileVersion = value;
                    if (!mobileVersion && (CurrentURL.StartsWith("https://m.") || CurrentURL.StartsWith("https://mobile.") || CurrentURL.StartsWith("http://m.") || CurrentURL.StartsWith("http://mobile.")))
                    {
                        Navigate(CurrentURL.Replace("//mobile.", "//www.").Replace("//m.", "//www."));
                    }
                    else
                    {
                        ReloadCurrentWebWithQuestion();
                        //Reload();
                    }
                    RaisePropertyChanged(nameof(isMobileVersion));
                }
                catch (Exception e)
                {

                }
            }
        }
        public string PageHttp = "";
        public string pageTitle = "Loading..";
        public string PageTitle
        {
            get
            {
                return pageTitle;
            }
            set
            {
                pageTitle = value;
            }
        }
        public string pageIcon = "ms-appx:///Assets/Icons/Windows11/browser.bmp";
        public string PageIcon
        {
            get
            {
                return pageIcon;
            }
            set
            {
                pageIcon = value;
                BitmapIconSource bitmapIconSource = new BitmapIconSource();
                bitmapIconSource.UriSource = new Uri(PageIcon);
            }
        }
        public string PagePreivew = "ms-appx:///Assets/Icons/Windows11/browser.bmp";
        public WriteableBitmap TabPreview = new WriteableBitmap(92, 92);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        Visibility LoadingVisible = Visibility.Collapsed;
        public Visibility isLoadingVisible
        {
            get
            {
                return LoadingVisible;
            }
            set
            {
                LoadingVisible = value;
                RaisePropertyChanged(nameof(isLoadingVisible));
            }
        }

        Visibility FrameLoadingVisible = Visibility.Collapsed;
        public Visibility isFrameLoadingVisible
        {
            get
            {
                return FrameLoadingVisible;
            }
            set
            {
                FrameLoadingVisible = value;
                RaisePropertyChanged(nameof(isFrameLoadingVisible));
            }
        }

        Visibility DownloadPossible = Visibility.Collapsed;
        public Visibility isDownloadPossible
        {
            get
            {
                return DownloadPossible;
            }
            set
            {
                DownloadPossible = value;
                RaisePropertyChanged(nameof(isDownloadPossible));
            }
        }

        bool validCertificateProvided = true;
        public Visibility isHTTPS
        {
            get
            {
                Visibility state = Visibility.Collapsed;
                if (CurrentURL.StartsWith("https:") && validCertificateProvided)
                {
                    state = Visibility.Visible;
                }
                else if (CurrentURL.StartsWith("ftps:"))
                {
                    state = Visibility.Visible;
                }
                return state;
            }
        }
        public Visibility isHTTP
        {
            get
            {
                Visibility state = Visibility.Collapsed;
                if (CurrentURL.StartsWith("http:") || !validCertificateProvided)
                {
                    state = Visibility.Visible;
                }
                else if (CurrentURL.StartsWith("ftp:"))
                {
                    state = Visibility.Visible;
                }
                return state;
            }
        }
        public async void RaiseAllChanged()
        {
            try
            {
                RaisePropertyChanged(nameof(CurrentURL));
                RaisePropertyChanged(nameof(CurrentHost));
                RaisePropertyChanged(nameof(CertificateProvider));
                RaisePropertyChanged(nameof(isLoading));
                RaisePropertyChanged(nameof(isActive));
                RaisePropertyChanged(nameof(isMobileVersion));
                RaisePropertyChanged(nameof(PageHttp));
                RaisePropertyChanged(nameof(PageTitle));
                RaisePropertyChanged(nameof(PageIcon));
                RaisePropertyChanged(nameof(PagePreivew));
                RaisePropertyChanged(nameof(ViewVersion));
                RaisePropertyChanged(nameof(isJavascriptEnabled));
                RaisePropertyChanged(nameof(isIndexedDB));
                RaisePropertyChanged(nameof(isDownloadPossible));
                Helpers.UpdateBindings(null, EventArgs.Empty);
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        public Dictionary<string, string[]> ConsoleList = new Dictionary<string, string[]>();

        public WebView webView;
        public IProgress<double> WebProgress;

        public WebObject()
        {
            try
            {
                ConsoleList = new Dictionary<string, string[]>();
                webView = new WebView();
                if (Helpers.GPUCacheMode)
                {
                    webView.CacheMode = new BitmapCache();
                }
            }
            catch (Exception e)
            {

            }

        }
        public WebObject(string url, ObservableCollection<Extension> extensions = null, bool isStringContent = false)
        {
            try
            {
                InitialWebView(url, extensions, isStringContent);
            }
            catch (Exception e)
            {
                Helpers.ShowErrorMessage(e);
            }
        }

        public FormsWebView XWebView = null;

        public void InitialWebView(string url, ObservableCollection<Extension> extensions = null, bool isStringContent = false)
        {
            try
            {
                ConsoleList = new Dictionary<string, string[]>();
                if (XamarinWebView)
                {
                    XWebView = new FormsWebView();
                    XWebView.OnNavigationError += XWebView_OnNavigationError;
                    XWebView.OnNavigationCompleted += XWebView_OnNavigationCompleted; ;
                    XWebView.OnNavigationStarted += XWebView_OnNavigationStarted;
                    XWebView.OnContentLoaded += XWebView_OnContentLoaded; ;
                }
                else
                {
                    if (WebHelper.UseSeparateThreadNew)
                    {
                        webView = new WebView(WebViewExecutionMode.SeparateThread);
                    }
                    else
                    {
                        webView = new WebView();
                    }
                    webView.ContentLoading += ContentLoading;
                    webView.LoadCompleted += LoadingDone;
                    webView.NavigationStarting += NavigationStarting;
                    webView.DOMContentLoaded += WebView_DOMContentLoaded;
                    webView.ContainsFullScreenElementChanged += ContainsFullScreenElementChanged;
                    webView.PermissionRequested += PermissionRequested;
                    webView.ScriptNotify += WebView_ScriptNotify;
                    webView.NewWindowRequested += WebView_NewWindowRequested;
                    webView.FrameContentLoading += WebView_FrameContentLoading;
                    webView.FrameDOMContentLoaded += WebView_FrameDOMContentLoaded;
                    webView.LongRunningScriptDetected += WebView_LongRunningScriptDetected;
                    webView.NavigationFailed += WebView_NavigationFailed;
                    webView.UnsafeContentWarningDisplaying += WebView_UnsafeContentWarningDisplaying;
                    webView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
                    webView.UnviewableContentIdentified += WebView_UnviewableContentIdentified;
                    webView.FrameNavigationStarting += WebView_FrameNavigationStarting;
                    webView.FrameNavigationCompleted += WebView_FrameNavigationCompleted;
                }

                History = new ObservableCollection<string>();
                SessionID = $"{Path.GetRandomFileName()}_{Path.GetRandomFileName()}";

                Extensions = extensions;

                /*double scale = 0.75;
                webView.RenderTransform = new TransformGroup()
                {
                    Children = {new ScaleTransform() { ScaleX = scale, ScaleY = scale }}
                };*/

                callConsoledMonitorTimer(true);
                callThumbnailMonitorTimer(true);
                Navigate(url, false, isStringContent);
            }
            catch (Exception ex)
            {

            }
        }

        private void XWebView_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

      
        private void XWebView_OnNavigationError(object sender, int e)
        {
            AddToConsoleList("error", $"Navigation error: {e}");
        }

        private void XWebView_OnContentLoaded(object sender, EventArgs e)
        {
            //Nothing Here
        }

        private void XWebView_OnNavigationCompleted(object sender, string e)
        {
            //Nothing Here
        }

        private void XWebView_OnNavigationStarted(object sender, object args)
        {
            try
            {
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public void AddToConsoleHandler(object sender, EventArgs args)
        {
            try
            {
                var ConsoleData = (ConsoleOutput)args;
                bool isMyLog = false;

                if (ConsoleData != null)
                {
                    var consoleType = ConsoleData.type;
                    if (consoleType.Equals("error") && (ConsoleData.message.Contains("blacklist") || ConsoleData.message.Contains("Lock Down")))
                    {
                        totalBlocked++;
                        isMyLog = true;
                    }
                    if (consoleType.Equals("error") && ConsoleData.message.Contains("adblock"))
                    {
                        totalAdBlocked++;
                        isMyLog = true;
                    }
                }
                if (!isConsoleOpen)
                {
                    return;
                }

                if (ConsoleData != null)
                {
                    var consoleType = ConsoleData.type;
                    switch (consoleType)
                    {
                        case "error":

                            if (!ConsoleErrors)
                            {
                                return;
                            }
                            break;

                        case "output":
                            if (!ConsoleLog)
                            {
                                return;
                            }
                            break;

                        case "info":
                            if (!ConsoleInfo)
                            {
                                return;
                            }
                            break;

                        case "warn":
                            if (!ConsoleWarnings)
                            {
                                return;
                            }
                            break;

                        case "xhr":
                            if (!ConsoleXHR)
                            {
                                return;
                            }
                            break;
                    }
                    if (ConsoleData.json.Length > 0)
                    {
                        ConsoleTable(ConsoleData.json);
                    }
                    else
                    {
                        AddToConsoleList(ConsoleData.type, ConsoleData.message, isMyLog);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        public async void ConsoleTable(string jsonObject)
        {
            try
            {
                var jsTableScript = await PathIO.ReadTextAsync("ms-appx:///Assets/JSHelpers/table.js");
                jsTableScript = jsTableScript.Replace("_JSON_OBJECT_", jsonObject);
                await SendCommand(jsTableScript, false);
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        public void ClearConsoleHandler(object sender, EventArgs args)
        {
            try
            {
                ClearConsole();
            }
            catch (Exception e)
            {

            }
        }
        public void ShowAlertHandler(object sender, EventArgs args)
        {
            try
            {
                var Message = (string)sender;
                if (Message != null)
                {
                    Helpers.ShowMessage(PageTitle, Message);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void XEventHandler(object sender, EventArgs args)
        {
            try
            {
                var xEventData = (XEventData)args;
                if (xEventData != null)
                {
                    switch (xEventData.type)
                    {
                        case "loaded":
                            PageLoaded();
                            break;

                        case "dblclick":
                            WebHelper.AppBarsHidderHandler.Invoke(null, EventArgs.Empty);
                            break;

                        case "menu":
                            WebHelper.ShowMenuHandler.Invoke(null, EventArgs.Empty);
                            break;

                        case "tabs":
                            WebHelper.ShowTabsHandler.Invoke(null, EventArgs.Empty);
                            break;

                        case "showa":
                            WebHelper.ShowAddressHandler.Invoke(null, EventArgs.Empty);
                            break;

                        case "hidea":
                            WebHelper.HideAddressHandler.Invoke(null, EventArgs.Empty);
                            break;

                        case "save":
                            WebHelper.SaveToFileHandler(null, args);
                            break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        bool PageLoadedState = false;
        public async void PageLoaded()
        {
            try
            {
                isFrameLoadingVisible = Visibility.Collapsed;
                isLoading = false;
                PageLoadedState = true;
                isLoadingVisible = Visibility.Collapsed;
                if (isActive && WebProgress != null)
                {
                    WebProgress.Report(100);
                }
                if (WebHelper.SuggestionsHideHandler != null)
                {
                    WebHelper.SuggestionsHideHandler.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {

            }
        }
        private void InjectComponents()
        {
            try
            {
                if (XamarinWebView)
                {
                }
                else
                {
                    webView.AddWebAllowedObject("console", new ConsoleOverride());
                    webView.AddWebAllowedObject("xevents", new WindowOverride());
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        private async void WebView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            try
            {
                CompletePageLoad();
                AddToConsoleList("warn", $"Unviewable Content: {args.MediaType}\nSource: {args.Referrer.AbsoluteUri}");
                if (args.MediaType.Contains("application/json"))
                {
                    try
                    {
                        try
                        {
                            cancellationTokenSource = new CancellationTokenSource();
                        }
                        catch (Exception e)
                        {

                        }
                        var result = await WebHelper.GetResponse(args.Uri, cancellationTokenSource.Token, postValues);
                        if (result != null)
                        {
                            string output = await result.Content.ReadAsStringAsync();
                            try
                            {
                                JToken parsedJson = JToken.Parse(output);
                                output = parsedJson.ToString(Formatting.Indented);
                                output = output.Replace("\\\"", "\"");
                            }
                            catch (Exception e)
                            {

                            }
                            AddToConsoleList("output", output);
                        }
                        else
                        {
                            AddToConsoleList("output", "JavaScript return null result");
                        }
                        WebHelper.OpenConsoleHandler.Invoke(null, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        AddToConsoleList(e);
                    }
                }
                else
                {
                    CheckURIForDownload(args.Uri);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void WebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            try
            {
                CompletePageLoad();
                AddToConsoleList("warn", $"Unsupported Uri: {args.Uri.AbsoluteUri}");
                CheckURIForDownload(args.Uri);
            }
            catch (Exception ex)
            {

            }
        }

        public async void CheckURIForDownload(Uri uri)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var testContent = await WebHelper.GetResponse(uri.AbsoluteUri, cancellationTokenSource.Token);
                if (testContent != null)
                {
                    string FileURL = uri.AbsoluteUri.Replace("wut::", "");
                    FileURL = FileURL.Replace("?dl=0", "?dl=1");
                    var tempName = "";
                    try
                    {
                        tempName = Path.GetFileName(FileURL);
                    }
                    catch (Exception e)
                    {

                    }
                    if (tempName.Length == 0 || tempName.Contains("?"))
                    {
                        tempName = WebHelper.FileNameExtractor(FileURL);
                    }
                    MegaFile megaFile = new MegaFile(testContent, tempName, true);
                    string DialogTitle = "File Detected";
                    //string DialogMessage = $"A file detected\ndo you want to start download?\nFile: {megaFile.Name}\nSize: {megaFile.Size.ToFileSize()}";
                    Run[] runMessage = new Run[] {
                        new Run{ Text = "A file detected " },
                        new Run { Text= ""},
                        new Run { Text = "File: ", FontWeight = FontWeights.Bold}, new Run { Text = $"{megaFile.Name}"},
                        new Run { Text = ""},
                         new Run { Text = "Size: ", FontWeight = FontWeights.Bold}, new Run { Text = $"{megaFile.Size.ToFileSize()}"},
                        new Run { Text = ""},
                        new Run { Text = "Do you want to download?", FontWeight= FontWeights.Medium},
                        new Run{Text = ""},
                        new Run{Text = ""},
                        new Run {Text = "If the app failed to download this file, you can try send it to Edge", Foreground = new SolidColorBrush(Colors.Orange)}
                    };
                    string[] DialogButtons = new string[] { $"Download", "Edge", "Cancel" };
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, runMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        string TargetHost = CurrentHost;
                        try
                        {
                            if (TargetHost.Length == 0)
                            {
                                TargetHost = uri.Host;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        Helpers.URIDownloadLink = uri.AbsoluteUri;
                        if (!Helpers.URIDownloadLink.Contains("&author="))
                        {
                            Helpers.URIDownloadLink += $"&author={TargetHost}";
                        }
                        Helpers.CheckIfScriptOnOpen.Invoke(null, null);

                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                        else
                        {
                            WebHelper.isTextChangedByNavigate = true;
                            CurrentURL = CurrentURLTemp;
                            WebHelper.AddressHandler.Invoke(CurrentURLTemp, EventArgs.Empty);
                            AddToConsoleList("info", $"Sent to downloads: {uri}");
                        }
                        FileDownloadInProgress = true;
                    }
                }
                else
                {
                    AddToConsoleList("error", $"Failed to get response from the server!");
                }
            }
            catch (Exception e)
            {

            }
        }
        public string getLocalString(string name)
        {
            var value = name;
            try
            {
                var temp = Helpers.getLocalString(name.Replace("...", "").Replace("..", "").Replace("!", ""));
                if (temp.Length > 0) { value = temp; }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        private void WebView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            try
            {
                CompletePageLoad();
                AddToConsoleList("warn", "SmartScreen Filter Detected Unsafe Content");
                //Helpers.ShowToastNotification(PageTitle, "SmartScreen Filter Detected Unsafe Content");
                Helpers.PlayNotificationSoundDirect("error.mp3");
                LocalNotificationData localNotificationData = new LocalNotificationData();
                localNotificationData.icon = SegoeMDL2Assets.Package;
                localNotificationData.type = Colors.OrangeRed;
                localNotificationData.time = 5;
                localNotificationData.message = "SmartScreen Filter Detected Unsafe Content";
                Helpers.pushLocalNotification(null, localNotificationData);
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateOptions()
        {
            try
            {
                if (XamarinWebView)
                {
                }
                else
                {
                    webView.Settings.IsJavaScriptEnabled = isJavascriptEnabled;
                    webView.Settings.IsIndexedDBEnabled = isIndexedDB;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void WebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            bool errorHandled = false;
            try
            {
                try
                {
                    if (e != null && e.Uri != null && e.WebErrorStatus.ToString().ToLower().Contains("certificate") && WebHelper.isIP(e.Uri.ToString()))
                    {
                        string DialogTitle = "Issue Detected";
                        string DialogMessage = $"Issue: {e.WebErrorStatus.ToString()}\nDo you want to load this page anyway?\n\nImportant: You could face some security issues if the website is not trusted!";
                        string[] DialogButtons = new string[] { $"Yes", "No" };
                        int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                        var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var result = await locationPromptDialog.ShowAsync2();

                        if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                        {
                            var IPAddress = WebHelper.ExtractIP(e.Uri.ToString());
                            var portal = new DevicePortal(new DefaultDevicePortalConnection(e.Uri.ToString(), "", ""));
                            var certificate = await portal.GetRootDeviceCertificateAsync(true);
                            portal.SetManualCertificate(certificate);
                            Navigate(e.Uri.ToString());
                            errorHandled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SwitchToStaticPage("error", ex.Message, e.Uri);
                    errorHandled = true;
                }

                if (HandleAllErrors && !errorHandled)
                {
                    SwitchToStaticPage("error", e.WebErrorStatus.ToString(), e.Uri);
                }
                AddToConsoleList("error", $"Error recieved: {e.WebErrorStatus.ToString()}\nURL: {e.Uri}");
            }
            catch (Exception ex)
            {
                AddToConsoleList(ex);
            }
        }

        //Check if the time not reset on page referesh
        double tempScriptTimer = 0;
        bool LongScriptDialogInProgress = false;
        private async void WebView_LongRunningScriptDetected(WebView sender, WebViewLongRunningScriptDetectedEventArgs args)
        {
            try
            {
                var currentTime = args.ExecutionTime.TotalMilliseconds;
                var maxTime = WebHelper.MaxScriptTimeout * 1000;
                if (!LongScriptDialogInProgress && WebHelper.DetectLongRunningScripts && currentTime > (maxTime + tempScriptTimer))
                {
                    LongScriptDialogInProgress = true;
                    string DialogTitle = "Script Detected";
                    string DialogMessage = $"Long Running Script Detected\ndo you want to stop script execution?";
                    string[] DialogButtons = new string[] { $"Yes", "No" };
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        args.StopPageScriptExecution = true;
                    }
                    tempScriptTimer += currentTime;
                }
                LongScriptDialogInProgress = false;
            }
            catch (Exception e)
            {
                LongScriptDialogInProgress = false;
            }
        }

        private void WebView_FrameNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            try
            {
                isFrameLoadingVisible = Visibility.Collapsed;
            }
            catch (Exception e)
            {

            }
        }

        private void WebView_FrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                if (args == null || args.Uri == null)
                {
                    args.Cancel = true;
                    isFrameLoadingVisible = Visibility.Collapsed;
                    return;
                }
                isFrameLoadingVisible = Visibility.Visible;
                if (!WebHelper.AllowIframe || LockDown)
                {
                    args.Cancel = true;
                    isFrameLoadingVisible = Visibility.Collapsed;
                    totalBlocked++;
                    if (LockDown)
                    {
                        AddToConsoleList("warn", $"Iframe blocked due 'Lock Down' mode: {args.Uri.Host}");
                    }
                    else
                    {
                        AddToConsoleList("warn", $"Iframe blocked due 'Iframes Allowed: Off': {args.Uri.Host}");
                    }
                    if (ConsoleLog)
                    {
                        AddToConsoleList("log", $"Full URL: {args.Uri.AbsoluteUri}");
                    }
                }
                else if (BlackList.InSecureDomains.Contains(args.Uri.Host) || BlackList.InSecureDomains.Contains(ExtractHostName(args.Uri.Host)))
                {

                    args.Cancel = true;
                    isFrameLoadingVisible = Visibility.Collapsed;
                    totalBlocked++;
                    AddToConsoleList("error", $"Iframe blocked due blacklist: {args.Uri.Host}", true);
                    if (ConsoleLog)
                    {
                        AddToConsoleList("log", $"Full URL: {args.Uri.AbsoluteUri}");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void WebView_FrameDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                isFrameLoadingVisible = Visibility.Collapsed;
            }
            catch (Exception e)
            {

            }
        }

        private void WebView_FrameContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            try
            {
                if (args != null && args.Uri != null && !args.Uri.AbsoluteUri.Contains("blank:") && !args.Uri.AbsoluteUri.Contains(":blank") && args.Uri.AbsoluteUri.Length > 0)
                {
                    isFrameLoadingVisible = Visibility.Visible;
                    AddToConsoleList("warn", $"Load iframe: {args.Uri.Host}");
                    if (ConsoleLog)
                    {
                        AddToConsoleList("log", $"Full URL: {args.Uri.AbsoluteUri}");
                    }
                }
                //CheckPageLoaded();
            }
            catch (Exception e)
            {

            }
        }

        bool NewTabHandled = false;
        TaskCompletionSource<bool> newWindowHandler = new TaskCompletionSource<bool>();
        private async void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            try
            {
                if (args != null && args.Uri != null && args.Uri.AbsoluteUri != null && !args.Uri.AbsoluteUri.Contains(":blank"))
                {
                    if (BlackList.InSecureDomains.Contains(args.Uri.Host) || BlackList.InSecureDomains.Contains(ExtractHostName(args.Uri.Host)))
                    {
                        args.Handled = true;
                        NewTabHandled = true;
                        totalBlocked++;
                        AddToConsoleList("error", $"New Tab request blocked due blacklist: {args.Uri.Host}", true);
                        if (ConsoleLog)
                        {
                            AddToConsoleList("log", $"Full URL': {args.Uri.AbsoluteUri}");
                        }
                        return;
                    }
                    newWindowHandler = new TaskCompletionSource<bool>();
                    args.Handled = true;
                    NewTabHandled = true;
                    string DialogTitle = "New Tab";
                    string DialogMessage = $"This website request new tab?\nTarget: {args.Uri.AbsoluteUri}";
                    string[] DialogButtons = new string[] { $"New Tab", "Same Tab", "Cancel" };
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        WebHelper.NewTabHandler.Invoke(args.Uri.AbsoluteUri, EventArgs.Empty);
                    }
                    else if (Helpers.DialogResultCheck(locationPromptDialog, 1))
                    {
                        Navigate(args.Uri.AbsoluteUri);
                    }
                    NewTabHandled = false;
                }
                else
                {
                    if (args != null && args.Uri != null && args.Uri.AbsoluteUri != null && args.Uri.AbsoluteUri.Contains(":blank"))
                    {
                        args.Handled = true;
                    }
                }
            }
            catch (Exception e)
            {
                NewTabHandled = false;
            }
            try
            {
                newWindowHandler.SetResult(true);
            }
            catch (Exception e)
            {

            }
        }

        public void WebViewNotify(object sender, EventArgs args)
        {
            try
            {
                if (args.GetType() == typeof(NotifyData))
                {
                    NotifyData notifyData = (NotifyData)args;
                    if (notifyData != null)
                    {
                        CheckNotifyData(notifyData);
                    }
                }
                else if (args.GetType() == typeof(DialogData))
                {
                    DialogData dialogData = (DialogData)args;
                    if (dialogData != null)
                    {
                        CheckDialogData(dialogData);
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        private async void CheckNotifyData(NotifyData notifyData)
        {
            try
            {
                switch (notifyData.type)
                {
                    case "longclick":
                        string[] seprator = { "|#|" };
                        var SplitData = notifyData.data.Split(seprator, StringSplitOptions.None);
                        ContextMenuData contextMenuData = new ContextMenuData(SplitData[0].Trim(), SplitData[1].Trim(), SplitData[2].Trim(), SplitData[3].Trim(), SplitData[4].Trim(), SplitData[5].Trim());
                        WebHelper.HoldingHandler.Invoke(null, contextMenuData);
                        break;

                    case "toast":
                        var title = PageTitle;
                        if (notifyData.extra.Length > 0)
                        {
                            title = notifyData.extra;
                        }
                        Helpers.ShowToastNotification(title, notifyData.data, notifyData.time);
                        break;

                    case "toastl":
                        WebHelper.ToastLocal.Invoke(null, notifyData);
                        break;

                    default:
                        Helpers.ShowToastNotification(PageTitle, notifyData.data, 30);
                        AddToConsoleList("command", notifyData.data);
                        break;
                }

            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        private void CheckDialogData(DialogData dialogData)
        {
            try
            {
                var NotifyData = dialogData.message;
                if (isActive)
                {
                    WebHelper.AlertHandler.Invoke(null, dialogData);
                }
                else
                {
                    Helpers.ShowToastNotification(PageTitle, NotifyData, 15);
                    AddToConsoleList("command", NotifyData);
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }


        public async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.StartsWith("confirm:"))
            {
                try
                {
                    var dialog = new Windows.UI.Popups.MessageDialog(e.Value.Replace("confirm:", ""));
                    dialog.Commands.Add(new UICommand("Yes", this.CommandInvokedHandler));

                    dialog.Commands.Add(new UICommand("Cancel", this.CommandInvokedHandler));

                    dialog.DefaultCommandIndex = 0;

                    dialog.CancelCommandIndex = 1;
                    var result = await dialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    AddToConsoleList(ex);
                }
            }
            else
            {
                try
                {
                    Helpers.ShowToastNotification(PageTitle, e.Value, 15);
                    AddToConsoleList("command", e.Value);
                }
                catch (Exception ex)
                {
                    AddToConsoleList(ex);
                }
            }
        }

        private async void CommandInvokedHandler(IUICommand command)
        {
            if (XamarinWebView)
            {
                if (command.Label == "Yes")
                {
                    await XWebView.InjectJavascriptAsync("_confirmResult = true;");
                }
                else
                {
                    await XWebView.InjectJavascriptAsync("_confirmResult = false;");
                }
            }
            else
            {
                if (command.Label == "Yes")
                {
                    await webView.InvokeScriptAsync("eval", new string[] { "_confirmResult = true;" });
                }
                else
                {
                    await webView.InvokeScriptAsync("eval", new string[] { "_confirmResult = false;" });
                }
            }
        }

        bool ScriptsInjected = false;
        public async Task InjectJavaScriptWrappers()
        {
            if (ScriptsInjected)
            {
                return;
            }
            try
            {
                ScriptsInjected = true;
                try
                {
                    var windowErrorHandler = "window.onerror = function(message, file, lineNumber){console.log(file + '\\n' + message + ' at line: ' + lineNumber); return false;}";
                    await SendCommand(windowErrorHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                await CheckPageLoaded();

                try
                {
                    var windowAlertHandler = "window.alert = function(message){console.alert(message);}";
                    await SendCommand(windowAlertHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var windowConfirmHandler = @"
                                                 var _confirmReceived = false;
                                                 if(false){
                                                    window.confirm = function(message){ window.external.notify('confirm:' + message) };
                                                  }else{
                                                 var confirmCallerName = null; var _confirmResult = false; window.confirm = function(message){_confirmReceived = false; _confirmResult = false; console.confirm(message); return _confirmResult;} 
                                                 }
                                                ";
                    await SendCommand(windowConfirmHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var windowConfirmHandler = "var _promptReceived = false; var _promptResult = null; window.prompt = function(message, defaultText){_promptReceived = false; _promptResult = false; if(defaultText){ console.prompt(message, defaultText);}else{ console.prompt(message);} return _promptResult;}";
                    await SendCommand(windowConfirmHandler, false);

                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var imagesScript = @"function MountContextMenuToImages(){ var imgs = document.images;
                                    for(var i=0; i<imgs.length; i++)
                                    {
                                        if(!imgs[i].hasAttribute('xevent')){
                                        imgs[i].setAttribute('xevent', 'ready');
	                                    imgs[i].oncontextmenu = function(e) {var elementHTML=e.target.outerHTML; console.notify('longclick', 'image|#|' + (this.hasAttribute('src')?this.src:'') + '|#|' + (this.hasAttribute('alt')?this.alt:'') + '|#|' + e.clientX + '|#|' + e.clientY+ '|#|' + elementHTML); return false;}
                                        }
                                    }} MountContextMenuToImages();";
                    await SendCommand(imagesScript, false);
                }
                catch (Exception e)
                {
                    Helpers.Logger(e);
                    AddToConsoleList(e);
                }


                try
                {
                    var linksScript = @"function MountContextMenuToLinks(){ var links = document.links;
                                    for(var i=0; i<links.length; i++)
                                    {
                                        var innerImages=links[i].getElementsByTagName('img');
                                        if(!links[i].hasAttribute('xevent') && (innerImages.length == 0 || window.location.hostname.includes('youtube.com'))){
                                        links[i].setAttribute('xevent', 'ready');
	                                    links[i].oncontextmenu = function(e) {var elementHTML=e.target.outerHTML; console.notify('longclick', 'link|#|' + (this.hasAttribute('href')?this.href:'') + '|#|' + this.innerHTML + '|#|' + e.clientX + '|#|' + e.clientY + '|#|' + elementHTML); return false;}
                                        }
                                    }} MountContextMenuToLinks();";
                    await SendCommand(linksScript, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var videosScript = @"function MountContextMenuToVideos(){var videos = document.getElementsByTagName('video');
                                    for (var i=0; i<videos.length; i++)
                                    {
                                        if(!videos[i].hasAttribute('xevent')){
                                        videos[i].setAttribute('xevent', 'ready');
                                        videos[i].oncontextmenu = function(e) {var sources=this.getElementsByTagName('source'); var sourceLink = sources.length>0 ? sources[0].src:(this.hasAttribute('src')?this.src:''); var elementHTML=e.target.outerHTML; console.notify('longclick', 'video|#|' + sourceLink + '|#|' + '' + '|#|' + e.clientX + '|#|' + e.clientY + '|#|' + elementHTML); return false;}
                                        }
                                    }} MountContextMenuToVideos();";
                    await SendCommand(videosScript, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var documentScript = @"function getSelectionText() { var text = ''; if (window.getSelection) { text = window.getSelection().toString(); } else if (document.selection && document.selection.type != 'Control') { text = document.selection.createRange().text; }return text; }
                                        document.oncontextmenu = function(e) {MountContextMenuToVideos(); MountContextMenuToLinks(); MountContextMenuToImages(); var targetText = getSelectionText(); var elementHTML=e.target.outerHTML; if(targetText.length > 0){ console.notify('longclick', 'text|#|' + '' + '|#|' + targetText + '|#|' + e.clientX + '|#|' + e.clientY + '|#|' + elementHTML); }else{ if(e.target.nodeName != 'IMG' && e.target.nodeName != 'VIDEO' && e.target.nodeName != 'A'){ console.notify('longclick', 'element|#|' + '' + '|#|' + targetText + '|#|' + e.clientX + '|#|' + e.clientY + '|#|' + elementHTML); } } return false;}
                                       ";
                    await SendCommand(documentScript, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var prototypeScript = @"if (typeof NodeList.prototype.forEach !== 'function') { NodeList.prototype.forEach = Array.prototype.forEach;}";
                    await SendCommand(prototypeScript, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    var scrollHandler = "var W10MScrollPreState = false, scrollState = false; var ScrollLoaderMonitor = setInterval(function () { if(W10MScrollPreState!=scrollState){ W10MScrollPreState = scrollState; xevents.address(scrollState); } },500); window.onscroll = function(e){ MountContextMenuToVideos(); MountContextMenuToLinks(); MountContextMenuToImages();  scrollState = this.oldScroll > this.scrollY; this.oldScroll = this.scrollY; }";
                    await SendCommand(scrollHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }
                try
                {
                    var arrayString = JsonConvert.SerializeObject(ADSProtection.ADSProtectionList);
                    var adsHandler = @"
                        var adsprotectArray = " + arrayString + @";
                        var AdsProtectionLoaderMonitor = setInterval(function () {   
                        for (var a = 0; a < adsprotectArray.length; a++){
                        try{
                        var selectorBlock = adsprotectArray[a];
                        if(selectorBlock.indexOf('!')==0 || selectorBlock.indexOf('|')==0){
                         return;
                        }
                        if(selectorBlock.indexOf('##')!=-1){
                         var myArr = selectorBlock.split('##');
                         if(myArr.length>1){
                          var HostCheck = myArr[0];
                          selectorBlock = myArr[1];
                          if(!window.location.hostname.includes(HostCheck)){
                            return;
                          }
                          }else{
                          selectorBlock = myArr[0];
                          }
                        }
                        try{
                        var rem = document.querySelectorAll(selectorBlock)
                        try{
                        for (var i = 0; i < rem.length; i++){
                        rem[i].parentNode.removeChild(rem[i]); 
                        if(typeof rem[i].id!='undefined' && rem[i].id.length > 0){
                         console.error('Element '+rem[i].id+' removed due adblock filter');
                        }else if(typeof rem[i].nodeName !='undefined'){
                         console.error('Element ('+rem[i].nodeName+') removed due adblock filter');
                        }else{
                         console.error('Element ? removed due adblock filter');
                        }
                        }
                        }catch(e){
                        }
                        }catch(e){
                        }
                        }catch(e){
                        }
                        }
                        },1500);
                        ";
                    await SendCommand(adsHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }


                try
                {
                    var xhrHandler = @"var lockdown = " + (LockDown ? "true" : "false") + "; var blacklist = " + JsonConvert.SerializeObject(BlackList.InSecureDomains) + @"; var xhrbody=false; const origSend = window.XMLHttpRequest.prototype.send; const origOpen = window.XMLHttpRequest.prototype.open; XMLHttpRequest.prototype.open = function (method, url, async, user, password) { this._url = url; var testurl = new URL((this._url.startsWith('http')?this._url:'http://'+this._url)); if(lockdown){console.error('Request blocked due Lock Down mode: '+testurl.hostname); return;} if(blacklist.indexOf(testurl.hostname)!=-1){ console.error('XHR request blocked due blacklist, Target: '+testurl.hostname); console.xhr(`XHR -> blocked to ${this._url}`); return; }  origOpen.apply(this, arguments); };
                                       XMLHttpRequest.prototype.send = function () { var testurl = new URL((this._url.startsWith('http')?this._url:'http://'+this._url)); if(lockdown){console.error('Request blocked due Lock Down mode: '+testurl.hostname); return;} if(blacklist.indexOf(testurl.hostname)!=-1){ console.error('XHR request blocked due blacklist, Target: '+testurl.hostname); console.xhr(`XHR -> blocked to ${this._url}`); return; } if(xhrbody){ console.xhr(`XHR -> ${this._url} body:`, arguments[0]); }else{ console.xhr(`XHR -> sending to ${this._url}`); } origSend.apply(this, arguments); }";
                    await SendCommand(xhrHandler, false);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                try
                {
                    await SolveLinks();
                }
                catch (Exception e)
                {

                }

                try
                {
                    await ChangeZoom(ZoomLevel);
                }
                catch (Exception e)
                {

                }

                try
                {
                    if (TurboMode)
                    {
                        await InjectLiteModeScript();
                    }
                }
                catch (Exception e)
                {

                }

            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public async Task InjectLiteModeScript()
        {
            try
            {
                var liteHandler = @"var LiteModeLoaderMonitor = setInterval(function () {
                                      LiteModeCall();
                                   },750);
                                  function DisableImagesByCSS(){
                                    document.head.insertAdjacentHTML('beforeend', '<style type=""text / css"">* { background-image: unset !important; } .video-thumbnail-container-large::before { display: none; padding-top: 0px !important; }</style>');
                                    var divs = document.getElementsByTagName('div');
                                        if(divs!=null){
                                            for (var i=0; i<divs.length; i++)
                                            {
                                               try{
                                                    divs[i].style['background-image']= 'unset';
                                                  }catch(e){}
                                            }
                                            divs = null;
                                            }
                                        var links = document.getElementsByTagName('a');
                                        if(links!=null){
                                            for (var i=0; i<links.length; i++)
                                            {
                                               try{
                                                    links[i].style['background-image']= 'unset';
                                                  }catch(e){}
                                            }
                                            divs = null;
                                        }
                                        var spans = document.getElementsByTagName('span');
                                        if(spans!=null){
                                            for (var i=0; i<spans.length; i++)
                                            {
                                               try{
                                                    spans[i].style['background-image']= 'unset';
                                                  }catch(e){}
                                            }
                                            divs = null;
                                        }
                                  }
                                    var CSSImagesLite = false;
                                  function LiteModeCall(){
                                    if(!CSSImagesLite){
                                    DisableImagesByCSS();   
                                    CSSImagesLite=true;
                                    }
                                    var imgs = document.images;
                                          if(imgs!=null){
                                            for (var i = 0; i < imgs.length; i++)
                                            {
                                              ReplaceElementForLiteMode(imgs[i]);
                                            }
                                            imgs = null;
                                        }
                                        var videos = document.getElementsByTagName('video');
                                        if(videos!=null){
                                            for (var i=0; i<videos.length; i++)
                                             {
                                              ReplaceElementForLiteMode(videos[i]);
                                             }
                                        }
                                        var iframes = document.getElementsByTagName('iframe');
                                        if(iframes!=null){
                                            for (var i=0; i<iframes.length; i++)
                                            {
                                                ReplaceElementForLiteMode(iframes[i]);
                                            }
                                            iframes = null;
                                        }
                                        var figures = document.getElementsByTagName('figure');
                                        if(figures!=null){
                                            for (var i=0; i<figures.length; i++)
                                            {
                                                ReplaceElementForLiteMode(figures[i]);
                                            }
                                            figures = null;
                                        }
                                  }
                                   function ReplaceElementForLiteMode(element){
                                         var cwidth = element.width;
                                         var cheight = element.height;
                                            const newItem = document.createElement('div');
                                            newItem.width = cwidth;
                                            newItem.height = cheight;
                                            var litemodetext = cwidth > 100?'LITE MODE':'';
                                            newItem.innerHTML = '<tdiv style=""width:'+cwidth+'px;height:'+cheight+'px;background: lightgreen;display: inline-block;text-align: center;margin: auto;opacity: 0.5;""><sdiv style=""display: block;margin: auto;position: relative;top: 45%;"">'+litemodetext+'</sdiv></tdiv>';
                                            element.parentNode.replaceChild(newItem, element);
                                    }
                                  LiteModeCall();
                ";
                await SendCommand(liteHandler, false);
            }
            catch (Exception e)
            {

            }
        }
        private async Task CheckPageLoaded()
        {
            try
            {
                var DocumentLoadedHandler = "var xeventsPageLoaderMonitor = setInterval(function () { if (document.readyState === 'complete'){ clearInterval(xeventsPageLoaderMonitor); xevents.loaded(); } },1000);";
                await SendCommand(DocumentLoadedHandler, false);

                try
                {
                    //Link Sweet Alert
                    //var jsSweetScript = await PathIO.ReadTextAsync("ms-appx:///Assets/JSHelpers/sweetalert.js");
                    //await SendCommand(jsSweetScript, false);
                }
                catch (Exception ex)
                {

                }
                /*if (WebHelper.TabsSwipeLeft || WebHelper.MenuSwipeRight)
                {
                    var jsSwipeScript = await PathIO.ReadTextAsync("ms-appx:///Assets/JSHelpers/swipe.js");
                    await SendCommand(jsSwipeScript, false);

                    if (WebHelper.TabsSwipeLeft)
                    {
                        var swipLeftScript = "w10mg_swipedocument.on('swipeleft', function (ev) { xevents.tabs(); });";
                        await SendCommand(swipLeftScript, false);
                    }

                    if (WebHelper.MenuSwipeRight)
                    {
                        var swipRightScript = "w10mg_swipedocument.on('swiperight', function (ev) { xevents.menu(); });";
                        await SendCommand(swipRightScript, false);
                    }
                }*/

                if (WebHelper.HideOnDouble)
                {
                    var DoubleClickHandler = "window.document.ondblclick = function() { xevents.dblclick(); }";
                    await SendCommand(DoubleClickHandler, false);
                }
                var jsFindScript = await PathIO.ReadTextAsync("ms-appx:///Assets/JSHelpers/search.js");
                await SendCommand(jsFindScript, false);
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        private async Task SolveLinks()
        {
            try
            {
                var resolveScript = "var allLinks = document.links; for (var i = 0; i < allLinks.length; i++) { allLinks[i].href = allLinks[i].href.replace('wut::', ''); }";
                await SendCommand(resolveScript, false);
            }
            catch (Exception e)
            {

            }
        }
        /*public static readonly DependencyProperty HTMLProperty =
           DependencyProperty.RegisterAttached("HTML", typeof(string), typeof(WebViewHTML), new PropertyMetadata("", new PropertyChangedCallback(OnHTMLChanged)));
        private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;

            if (wv != null)
            {
                wv.NavigateToString((string)e.NewValue);
            }
        }*/

        private async void PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            try
            {
                var permissionType = args.PermissionRequest.PermissionType;
                string DialogTitle = "Permission Request";
                string DialogMessage = $"Do you allow to ({args.PermissionRequest.Uri.Host})\nto use your location?";
                string[] DialogButtons = new string[] { $"Allow", "Deny", "Later" };
                int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                ContentDialogCustom permissionPromptDialog = null;
                bool requestFound = false;
                switch (permissionType)
                {
                    case WebViewPermissionType.Geolocation:
                        if (WebHelper.AllowLocation)
                        {
                            DialogMessage = $"This website ({args.PermissionRequest.Uri.Host})\nRequest access to your Geolocation";
                            requestFound = true;
                        }
                        break;

                    case WebViewPermissionType.Media:
                        if (WebHelper.AllowMedia)
                        {
                            DialogMessage = $"This website ({args.PermissionRequest.Uri.Host})\nRequest access to your Media";
                            requestFound = true;
                        }
                        break;

                    case WebViewPermissionType.PointerLock:
                        if (WebHelper.PointerLock)
                        {
                            DialogMessage = $"This website ({args.PermissionRequest.Uri.Host})\nRequest access to your PointerLock";
                            requestFound = true;
                        }
                        break;

                    case WebViewPermissionType.UnlimitedIndexedDBQuota:
                        if (WebHelper.UnlimitedIndexedDBQuota)
                        {
                            DialogMessage = $"This website ({args.PermissionRequest.Uri.Host})\nRequest access to your UnlimitedIndexedDBQuota";
                            requestFound = true;
                        }
                        break;

                    case WebViewPermissionType.WebNotifications:
                        if (WebHelper.WebNotifications)
                        {
                            DialogMessage = $"This website ({args.PermissionRequest.Uri.Host})\nRequest access to your WebNotifications";
                            requestFound = true;
                        }
                        break;
                }
                if (requestFound)
                {
                    permissionPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                    var result = await permissionPromptDialog.ShowAsync2();
                    if (permissionPromptDialog != null)
                    {
                        if (Helpers.DialogResultCheck(permissionPromptDialog, 2))
                        {
                            args.PermissionRequest.Allow();
                        }
                        else if (Helpers.DialogResultCheck(permissionPromptDialog, 1))
                        {
                            args.PermissionRequest.Deny();
                        }
                        else
                        {
                            args.PermissionRequest.Defer();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        private void ContainsFullScreenElementChanged(WebView sender, object args)
        {
            try
            {
                var applicationView = ApplicationView.GetForCurrentView();

                if (sender.ContainsFullScreenElement)
                {
                    applicationView.TryEnterFullScreenMode();
                }
                else if (applicationView.IsFullScreenMode)
                {
                    applicationView.ExitFullScreenMode();
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        bool CustomPageLoading = false;
        public async void SwitchToStaticPage(string pageType, string extraData = "", Uri targetURL = null)
        {
            try
            {
                var html = "";
                switch (pageType)
                {
                    case "blank":
                        html = await PathIO.ReadTextAsync("ms-appx:///Assets/Pages/blank.html");
                        CurrentHost = "Blank";
                        if (targetURL != null)
                        {
                            CurrentURL = targetURL.ToString();
                        }
                        RealCurrentURL = "blank:page";
                        break;

                    case "offline":
                        html = await PathIO.ReadTextAsync("ms-appx:///Assets/Pages/offline.html");
                        CurrentHost = "Offline";
                        if (targetURL != null)
                        {
                            CurrentURL = targetURL.ToString();
                        }
                        RealCurrentURL = "offline:page";
                        break;

                    case "error":
                        html = await PathIO.ReadTextAsync("ms-appx:///Assets/Pages/error.html");
                        CurrentHost = "Error";
                        if (targetURL != null)
                        {
                            CurrentURL = targetURL.ToString();
                        }
                        RealCurrentURL = "error:page";
                        html = html.Replace("_error_num_", extraData);
                        break;

                    case "security":
                        html = await PathIO.ReadTextAsync("ms-appx:///Assets/Pages/security.html");
                        CurrentHost = "Security";
                        if (targetURL != null)
                        {
                            CurrentURL = targetURL.ToString();
                        }
                        RealCurrentURL = "security:page";
                        html = html.Replace("_message_", extraData);
                        break;
                }
                if (html != null && html.Length > 0)
                {
                    WebHelper.isTextChangedByNavigate = true;
                    WebHelper.AddressHandler.Invoke(CurrentURL, EventArgs.Empty);
                    CustomPageLoading = true;
                    if (XamarinWebView)
                    {
                        //XWebView.ContentType = "StringData";
                        //XWebView.Source = html;
                    }
                    else
                    {
                        webView.NavigateToString(html);
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        bool isYoutubeLink = false;
        private string CheckIfFileAsync(Uri uri)
        {
            try
            {
                if (uri != null && !CustomPageLoading)
                {
                    var URLToTest = uri.AbsoluteUri;
                    URLToTest = URLToTest.Replace("?dl=0", "?dl=1");
                    try
                    {
                        Match m = Regex.Match(URLToTest, WebHelper.URLFilePattern, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            if (m.Groups != null && m.Groups.Count > 0)
                            {
                                string file = m.Groups["file"].ToString();
                                string fileExtenstion = Path.GetExtension(file);
                                if (!WebHelper.ExcludedExtentions.Contains(fileExtenstion) && !WebHelper.DomainsExtenstions.Contains(fileExtenstion) && !URLToTest.Contains("#"))
                                {
                                    return file;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        if (!CustomPageLoading)
                        {
                            var file = WebHelper.isYoutubeDownloadLink(uri.ToString());
                            if (file != null)
                            {
                                string fileExtenstion = Path.GetExtension(file);
                                if (!WebHelper.ExcludedExtentions.Contains(fileExtenstion) && !URLToTest.Contains("#"))
                                {
                                    isYoutubeLink = true;
                                    return file;
                                }
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

            return null;
        }
        async void OpenWithEdge(Uri uri)
        {
            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }
        public async void OpenWithEdge()
        {
            var options = new Windows.System.LauncherOptions();
            options.PreferredApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe";
            options.PreferredApplicationDisplayName = "Microsoft Edge";
            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri(CurrentURL), options);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }

        public bool ForceLoadByBrowser = false;
        public bool RequestHandledByNewUserAgent = false;
        public bool isStringContentLoaded = false;
        public List<Certificate> serverCertificates = new List<Certificate>();
        private async void NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {

            try
            {
                /*if (!RequestHandledByNewUserAgent && args.Uri!=null && !RealCurrentURL.StartsWith("blank:") && !RealCurrentURL.StartsWith("error:") && !RealCurrentURL.StartsWith("security:"))
                {
                    args.Cancel = true;
                    NavigateWithCustomMode(args.Uri);
                    return;
                }
                else
                {
                    RequestHandledByNewUserAgent = false;
                }*/
                try
                {
                    WebHelper.ShowAddressHandler.Invoke(null, EventArgs.Empty);
                }
                catch (Exception ex)
                {

                }
                if ((args == null || args.Uri == null) && (CurrentURL.StartsWith("about:") || CurrentURL.StartsWith("blank:") || CurrentURL.StartsWith("offline:") || CurrentURL.StartsWith("error:") || CurrentURL.StartsWith("security:")))
                {
                    CustomPageLoading = true;
                }

                LoadRequestCanceled = false;
                if (isStringContentLoaded)
                {
                    return;
                }

                try
                {
                    cancellationTokenSource.Cancel();
                    isFrameLoadingVisible = Visibility.Collapsed;
                }
                catch (Exception e)
                {

                }
                UpdateOptions();
                try
                {
                    if (XamarinWebView)
                    {
                        XWebView.Focus();
                    }
                    else
                    {
                        webView.Focus(FocusState.Programmatic);
                    }
                }
                catch (Exception ex)
                {

                }

                try
                {
                    if (NewTabHandled)
                    {
                        await newWindowHandler.Task;
                    }
                }
                catch (Exception e)
                {

                }

                if ((Helpers.OfflineMode || !Helpers.isInternetActive) && !CustomPageLoading)
                {
                    args.Cancel = true;
                    LoadRequestCanceled = true;
                    Helpers.ShowOfflineModeMessage();
                    try
                    {
                        SwitchToStaticPage("offline", "", args.Uri);
                    }
                    catch (Exception e)
                    {

                    }
                    return;
                }

                if (!CustomPageLoading && (args == null || args.Uri == null) && (!CurrentURL.StartsWith("about:") && !CurrentURL.StartsWith("blank:") && !CurrentURL.StartsWith("offline:") && !CurrentURL.StartsWith("error:") && !CurrentURL.StartsWith("security:")))
                {
                    args.Cancel = true;
                    SwitchToStaticPage("blank");
                    return;
                }

                CheckCookiesSettings(args.Uri, isClearCookiesRequest);

                //Check if file
                string file = CheckIfFileAsync(args.Uri);
                if (!ForceLoadByBrowser && file != null)
                {
                    args.Cancel = true;
                    LoadRequestCanceled = true;
                    try
                    {
                        if (args != null && args.Uri != null)
                        {
                            AddToConsoleList("nav", $"Loading: {args.Uri}");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    string DialogTitle = "File Detected";
                    //string DialogMessage = $"A file ({file}) was detected\ndo you want to start download?";
                    Run[] runMessage = new Run[] {
                    new Run{Text = "File name detected," }, new Run{ Text=$" ({file}) ", FontWeight=FontWeights.Bold},
                    new Run { Text = ""},
                    new Run {Text = "Do you want to start download?", FontWeight = FontWeights.Medium },
                    new Run { Text = "" },
                    new Run { Text = "If you sure it's not a file, choose 'Browse'", Foreground = new SolidColorBrush(Colors.Orange)}
                    };
                    string[] DialogButtons = new string[] { $"Download", "Browse", "Cancel" };
                    if (isYoutubeLink)
                    {
                        //DialogMessage = $"A video ({file}) was detected\ndo you want to start download?";
                        runMessage = new Run[] {
                    new Run{Text = "A video " }, new Run{ Text=$"{file} ", FontWeight=FontWeights.Bold}, new Run { Text= " detected"},
                    new Run { Text = ""},
                    new Run {Text = "Do you want to start download?", FontWeight = FontWeights.Medium },
                    new Run { Text = "" },
                    new Run { Text = "If you want to view in browser, choose 'View'", Foreground = new SolidColorBrush(Colors.Orange)}
                    };
                        DialogButtons = new string[] { $"Download", "View", "Cancel" };
                    }
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, runMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        string TargetHost = CurrentHost;
                        try
                        {
                            if (TargetHost.Length == 0)
                            {
                                TargetHost = args.Uri.Host;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        isYoutubeLink = false;
                        Helpers.URIDownloadLink = args.Uri.AbsoluteUri;
                        if (!Helpers.URIDownloadLink.Contains("&author="))
                        {
                            Helpers.URIDownloadLink += $"&author={TargetHost}";
                        }
                        Helpers.CheckIfScriptOnOpen.Invoke(null, null);

                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                        else
                        {
                            WebHelper.isTextChangedByNavigate = true;
                            CurrentURL = CurrentURLTemp;
                            WebHelper.AddressHandler.Invoke(CurrentURLTemp, EventArgs.Empty);
                            AddToConsoleList("info", $"Sent to downloads: {args.Uri}");
                        }
                        FileDownloadInProgress = true;
                    }
                    else if (Helpers.DialogResultCheck(locationPromptDialog, 1))
                    {
                        /*if (isYoutubeLink)
                        {*/
                        ForceLoadByBrowser = true;
                        Navigate(args.Uri.ToString());
                        AddToConsoleList("info", $"Request to View: {args.Uri}");
                        /*}
                        else
                        {
                            OpenWithEdge(args.Uri);
                            if (CurrentURLTemp.Length == 0)
                            {
                                WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                            }
                            else
                            {
                                CurrentURL = CurrentURLTemp;
                                WebHelper.AddressHandler.Invoke(CurrentURLTemp, EventArgs.Empty);
                                AddToConsoleList("info", $"Sent to Edge: {args.Uri}");
                            }
                        }*/

                        isYoutubeLink = false;
                    }
                    else
                    {
                        isYoutubeLink = false;
                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                    }
                    return;
                }

                //Check if MEGA link
                if (!ForceLoadByBrowser && Helpers.CheckIfMegaLink(args.Uri))
                {
                    args.Cancel = true;
                    LoadRequestCanceled = true;
                    try
                    {
                        if (args != null && args.Uri != null)
                        {
                            AddToConsoleList("nav", $"Loading: {args.Uri}");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    string DialogTitle = "MEGA Detected";
                    string DialogMessage = $"A MEGA link detected\nHow do you want to open this link?";
                    string[] DialogButtons = new string[] { $"WUT", "Browser", "Cancel" };
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        Helpers.URIDownloadLink = args.Uri.AbsoluteUri;
                        Helpers.CheckIfScriptOnOpen.Invoke(null, null);

                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                        else
                        {
                            WebHelper.isTextChangedByNavigate = true;
                            CurrentURL = CurrentURLTemp;
                            WebHelper.AddressHandler.Invoke(CurrentURLTemp, EventArgs.Empty);
                            AddToConsoleList("info", $"Opening: {args.Uri}");
                        }
                    }
                    else if (Helpers.DialogResultCheck(locationPromptDialog, 1))
                    {
                        ForceLoadByBrowser = true;
                        Navigate(args.Uri.AbsoluteUri);
                    }
                    else
                    {
                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                    }
                    return;
                }

                //Check if MEGA direct link
                if (!ForceLoadByBrowser && args.Uri != null && args.Uri.AbsoluteUri.Contains("https://mega.nz/file/"))
                {
                    args.Cancel = true;
                    LoadRequestCanceled = true;
                    try
                    {
                        if (args != null && args.Uri != null)
                        {
                            AddToConsoleList("nav", $"Loading: {args.Uri}");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    string DialogTitle = "MEGA Detected";
                    string DialogMessage = $"A MEGA file detected\nHow do you want to open this link?";
                    string[] DialogButtons = new string[] { $"WUT", "Browser", "Cancel" };
                    int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                    var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                    var result = await locationPromptDialog.ShowAsync2();

                    if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                    {
                        Helpers.URIDownloadLink = args.Uri.AbsoluteUri;
                        Helpers.CheckIfScriptOnOpen.Invoke(null, null);

                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                        else
                        {
                            WebHelper.isTextChangedByNavigate = true;
                            CurrentURL = CurrentURLTemp;
                            WebHelper.AddressHandler.Invoke(CurrentURLTemp, EventArgs.Empty);
                            AddToConsoleList("info", $"Opening: {args.Uri}");
                        }
                    }
                    else if (Helpers.DialogResultCheck(locationPromptDialog, 1))
                    {
                        ForceLoadByBrowser = true;
                        Navigate(args.Uri.AbsoluteUri);
                    }
                    else
                    {
                        if (CurrentURLTemp.Length == 0)
                        {
                            WebHelper.CloseTabHandler($"#switch|{SessionID}", EventArgs.Empty);
                        }
                    }
                    return;
                }

                ForceLoadByBrowser = false;
                //Check if not static
                /*if (!RealCurrentURL.StartsWith("blank:") && !RealCurrentURL.StartsWith("error:") && !RealCurrentURL.StartsWith("security:"))
                {
                    CustomPageLoading = false;
                }*/


                if (!CustomPageLoading && args != null && args.Uri != null && !IsAllowedUri(args.Uri))
                {
                    if (!args.Uri.ToString().EndsWith("blank"))
                    {
                        args.Cancel = true;
                        LoadRequestCanceled = true;
                    }
                    else
                    {
                        try
                        {
                            args.Cancel = true;
                            LoadRequestCanceled = true;
                            SwitchToStaticPage("blank", "", args.Uri);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    isLoading = false;
                    isLoadingVisible = Visibility.Collapsed;
                    if (isActive && WebProgress != null)
                    {
                        try
                        {
                            //WebProgress.Report(100);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else
                {

                    if (!CustomPageLoading && args.Uri != null && (!WebHelper.CheckSecurity(args.Uri) || BlackList.InSecureDomains.Contains(ExtractHostName(args.Uri.Host))))
                    {
                        args.Cancel = true;
                        LoadRequestCanceled = true;
                        SwitchToStaticPage("security", "Insecure Website", args.Uri);
                    }
                    else
                    //if (!CustomPageLoading)
                    {
                        try
                        {
                            totalBlocked = 0;
                            totalAdBlocked = 0;
                            ClearConsole();
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            if (args != null && args.Uri != null)
                            {
                                AddToConsoleList("nav", $"Loading: {args.Uri}");
                                cancellationTokenSource = new CancellationTokenSource();
                                try
                                {
                                    var testURL = args.Uri.ToString(); ;
                                    if (testURL.StartsWith("https:"))
                                    {
                                        validCertificateProvided = true;
                                    }
                                    else if (testURL.StartsWith("ftps:"))
                                    {
                                        validCertificateProvided = true;
                                    }
                                    serverCertificates = new List<Certificate>();
                                    var webResponse = await WebHelper.GetResponse(testURL, cancellationTokenSource.Token, null, false);
                                    if (webResponse != null)
                                    {
                                        try
                                        {
                                            serverCertificates = webResponse.RequestMessage.TransportInformation.ServerIntermediateCertificates.ToList();
                                            //validCertificateProvided = WebHelper.ValidateCertificates(serverCertificates);
                                            List<string> certs = new List<string>();
                                            if (serverCertificates.Count > 0)
                                            {
                                                foreach (var cItem in serverCertificates)
                                                {
                                                    certs.Add(cItem.Issuer);
                                                }
                                                CertificateProvider = String.Join("\n", certs.Distinct());
                                            }
                                            else
                                            {
                                                CertificateProvider = "";
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        ScriptsInjected = false;

                        WebHelper.isTextChangedByNavigate = true;
                        if (!CustomPageLoading)
                        {
                            CurrentURL = args.Uri.ToString();
                            CurrentURLTemp = CurrentURL;
                            WebHelper.AddressHandler.Invoke(CurrentURL, EventArgs.Empty);
                        }

                        try
                        {
                            callLoadingMonitorTimer(true);
                        }
                        catch (Exception e)
                        {

                        }

                        PageIcon = WebHelper.EMPTY_FAVICON;
                        Helpers.UpdateBindings.Invoke(null, EventArgs.Empty);

                        CurrentHost = GetHostName(args.Uri);
                        if (CurrentURL.Equals(FirstCallURL))
                        {
                            isTranslateMode = false;
                        }
                        if (!isTranslateMode)
                        {
                            FirstCallURL = CurrentURL;
                        }

                        try
                        {
                            if (CurrentURL.ToLower().Contains("youtube.com/watch"))
                            {
                                isDownloadPossible = Visibility.Visible;
                            }
                            else
                            {
                                isDownloadPossible = Visibility.Collapsed;
                            }
                        }
                        catch (Exception e)
                        {

                        }

                        InjectComponents();

                        try
                        {
                            if (TurboMode)
                            {
                                /*cancellationTokenSource = new CancellationTokenSource();
                                var requestURL = args.Uri.AbsoluteUri;
                                args.Cancel = true;
                                LoadRequestCanceled = true;
                                var pageContent = await WebHelper.GetResponse(requestURL, cancellationTokenSource.Token);
                                if (pageContent != null)
                                {
                                    var testStream = new TurboStream(pageContent, WebProgress, cancellationTokenSource.Token);
                                    var SourceStream = await testStream.FetchStream();
                                    if (SourceStream != null)
                                    {
                                        isStringContentLoaded = true;
                                        webView.NavigateToString(SourceStream);
                                    }
                                }
                                else
                                {
                                    Helpers.ShowMessage("Turbo Mode","Failed to get the page contents\nIf this message keep appearing, switch off Turbo Mode");
                                    AddToConsoleList("warn",$"Turbo Mode (Failed) to load:\n{requestURL}");
                                }*/
                                //return;
                            }
                        }
                        catch (Exception e)
                        {
                            AddToConsoleList(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        bool userAgentReset = false;
        public void CheckAgentMode()
        {
            try
            {
                if (!WebHelper.DisableCustomWebAgent && WebHelper.UseURLDemon)
                {
                    userAgentReset = false;
                    if (isMobileVersion && MobileAgent.Length > 0)
                    {
                        if (TurboMode)
                        {
                            var TurboModeAgent = "Mozilla/4.0 (Windows CE; PPC; 240x320) Trident/3.1; IEMobile/7.0";
                            UserAgentHelper.SetDefaultUserAgent(TurboModeAgent);
                        }
                        else
                        {
                            UserAgentHelper.SetDefaultUserAgent(MobileAgent);
                        }
                    }
                    else if (DesktopAgent.Length > 0)
                    {
                        UserAgentHelper.SetDefaultUserAgent(DesktopAgent);
                    }
                }
                else
                {
                    if (!userAgentReset)
                    {
                        UserAgentHelper.SetDefaultUserAgent("");
                        userAgentReset = true;
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        bool isClearCookiesRequest = false;
        public void CheckCookiesSettings(Uri gotouri, bool clearCookiesRequest = false)
        {
            try
            {
                isClearCookiesRequest = false;
                if (WebHelper.BlockCookies || clearCookiesRequest)
                {
                    HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
                    HttpCookieManager cookieManager = myFilter.CookieManager;
                    HttpCookieCollection myCookieJar = cookieManager.GetCookies(gotouri);
                    foreach (HttpCookie cookie in myCookieJar)
                    {
                        cookieManager.DeleteCookie(cookie);
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        public void ClearCookies()
        {
            try
            {
                isClearCookiesRequest = true;
                Reload();
            }
            catch (Exception e)
            {

            }
        }
        private bool IsAllowedUri(Uri uri)
        {
            try
            {
                if (uri != null)
                {
                    Match mb = Regex.Match(uri.ToString(), WebHelper.URLPattern, RegexOptions.IgnoreCase);
                    if (mb.Success)
                    {
                        return true;
                    }
                    else if (CurrentURL.StartsWith("blank:") || CurrentURL.StartsWith("offline:") || CurrentURL.StartsWith("error:") || CurrentURL.StartsWith("security:"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }

        private bool IsAllowedUri(string url)
        {
            try
            {
                if (url != null)
                {
                    Match mb = Regex.Match(url, WebHelper.URLPattern, RegexOptions.IgnoreCase);
                    if (mb.Success)
                    {
                        return true;
                    }
                    else if (CurrentURL.StartsWith("blank:") || CurrentURL.StartsWith("offline:") || CurrentURL.StartsWith("error:") || CurrentURL.StartsWith("security:"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }

        public string ParseURL(string url)
        {
            try
            {
                if (WebHelper.IsAllowedUri(url))
                {
                    string parsedURL = url;
                    parsedURL = parsedURL.Trim();
                    return parsedURL;
                }
                else
                {
                    AddToConsoleList("error", "URL not allowed or not valid!");
                    return WebHelper.StartupURL;
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
                return WebHelper.StartupURL;
            }
        }
        public void Reload()
        {
            try
            {
                CheckAgentMode();
                if (XamarinWebView)
                {
                    XWebView.Refresh();
                }
                else
                {
                    webView.Refresh();
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        private async void ReloadCurrentWebWithQuestion(bool showMessage = false, string title = "", string message="")
        {
            try
            {
                Helpers.PlayNotificationSoundDirect("alert.mp3");
                string DialogTitle = "Reload Page";
                string DialogMessage = $"Do you want to reload the current page?";
                string[] DialogButtons = new string[] { $"Reload", "Cancel" };
                int[] DialogButtonsIds = new int[] { 2, 1, 3 };
                var locationPromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                var result = await locationPromptDialog.ShowAsync2();

                if (Helpers.DialogResultCheck(locationPromptDialog, 2))
                {
                   Reload();
                }
                if (showMessage)
                {
                    if (title.Length > 0)
                    {
                        Helpers.ShowMessage(title, message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void Stop()
        {
            try
            {
                if (WebHelper.SuggestionsHideHandler != null)
                {
                    WebHelper.SuggestionsHideHandler.Invoke(null, EventArgs.Empty);
                }
                DOMLoaded = true;
                LoadingDoneChangingProgress = true;
                /*try
                {
                    CancellationTokenProgressEmulate.Cancel();
                }
                catch (Exception e)
                {
                }*/

                callLoadingMonitorTimer();
                while (LoadingProgress < 100)
                {
                    await Task.Delay(1);
                    Interlocked.Increment(ref LoadingProgress);
                    if (isActive && WebProgress != null)
                    {
                        WebProgress.Report(LoadingProgress);
                    }
                }
                if (XamarinWebView)
                {
                    //TO-DO XWebView Stop
                }
                else
                {
                    webView.Stop();
                }
                isLoading = false;
                //isLoading = true;
                isLoadingVisible = Visibility.Collapsed;
                isFrameLoadingVisible = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        public bool GoBack()
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {

            }
            try
            {
                if (XamarinWebView)
                {
                    if (XWebView.CanGoBack)
                    {
                        CheckAgentMode();
                        XWebView.GoBack();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (webView.CanGoBack)
                    {
                        CheckAgentMode();
                        webView.GoBack();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception e)
            {
                AddToConsoleList(e);
                return false;
            }
        }
        public bool GoForward()
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {

            }
            try
            {
                if (XamarinWebView)
                {
                    if (XWebView.CanGoForward)
                    {
                        CheckAgentMode();
                        XWebView.GoForward();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (webView.CanGoForward)
                    {
                        CheckAgentMode();
                        webView.GoForward();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception e)
            {
                AddToConsoleList(e);
                return true;
            }
        }
        
        public void Navigate(string url, bool Translate = false, bool isStringContent = false)
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {

            }

            RealCurrentURL = "";

            if (url.Equals("blank:"))
            {
                SwitchToStaticPage("blank");
                WebHelper.isTextChangedByNavigate = true;
                CurrentURL = "blank:";
                CurrentURLTemp = CurrentURL;
                PageIcon = WebHelper.EMPTY_FAVICON;
                WebHelper.AddressHandler.Invoke(CurrentURL, EventArgs.Empty);
                return;
            }

            if (isStringContent)
            {
                isStringContentLoaded = true;
                if (XamarinWebView)
                {
                    //XWebView.ContentType = "StringData";
                    //XWebView.Source = url;
                }
                else
                {
                    webView.NavigateToString(url);
                }
                return;
            }
            try
            {
                isStringContentLoaded = false;
                url = WebHelper.SolveURL(url);
                WebHelper.isTextChangedByNavigate = true;
                WebHelper.AddressHandler.Invoke(url, EventArgs.Empty);
                if (!IsAllowedUri(url))
                {
                    var SearchURL = WebHelper.SearchEngine + url;
                    Navigate(SearchURL);
                }
                else
                {
                    CheckAgentMode();
                    CurrentURL = url;
                    if (!Translate)
                    {
                        FirstCallURL = CurrentURL;
                        isTranslateMode = false;
                    }
                    else
                    {
                        isTranslateMode = true;
                    }
                    NavigateWithCustomMode(new Uri(ParseURL(url)));
                    History.Add(url);
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public void NavigateWithCustomMode(Uri uri)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
                try
                {
                    if (!WebHelper.DisableCustomWebAgent)
                    {
                        HttpProductInfoHeaderValue userAgent = new HttpProductInfoHeaderValue(MobileAgent);
                        if (!isMobileVersion)
                        {
                            userAgent = new HttpProductInfoHeaderValue(DesktopAgent);
                        }
                        if ((isMobileVersion && MobileAgent.Length > 0) || (!isMobileVersion && DesktopAgent.Length > 0))
                        {
                            httpRequestMessage.Headers.UserAgent.Clear();
                            httpRequestMessage.Headers.UserAgent.Add(userAgent);
                        }
                    }
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }
                if (XamarinWebView)
                {
                    //TO-DO Try to navigate with http message
                    //XWebView.ContentType = "Internet";
                    //XWebView.Source = uri.ToString();
                    AddToConsoleList("warn", "User agent ignored because of XaWebView", true);
                }
                else
                {
                    webView.NavigateWithHttpRequestMessage(httpRequestMessage);
                }
                RequestHandledByNewUserAgent = true;
            }
            catch (Exception e)
            {
                if (XamarinWebView)
                {
                    //XWebView.ContentType = "Internet";
                    //XWebView.Source = uri.ToString();
                }
                else
                {
                    webView.Navigate(uri);
                }
                AddToConsoleList(e);
            }
        }

        public async Task<string> CaptureCurrentPage(StorageFolder storageFolder, bool isFull = false)
        {
            if (XamarinWebView)
            {
                //TO-DO make capture possible
                LocalNotificationData localNotificationData = new LocalNotificationData();
                localNotificationData.icon = SegoeMDL2Assets.Error;
                localNotificationData.type = Colors.Tomato;
                localNotificationData.message = "Capture is not supported in XWebView!";
                localNotificationData.time = 3;
                Helpers.pushLocalNotification(null, localNotificationData);
                return "";
            }
            else
            {
                try
                {
                    var widthTemp = webView.ActualWidth;
                    var heightTemp = webView.ActualHeight;
                    int width = (int)webView.ActualWidth;
                    int height = (int)webView.ActualHeight;

                    var fileName = DateTime.Now.ToString().Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_") + ".png";
                    StorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    if (storageFile != null)
                    {
                        using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            if (!isFull)
                            {
                                var size = new Size(width, height);
                                webView.Width = size.Width;
                                webView.Height = size.Height;
                                await Task.Delay(500);
                            }
                            await webView.CapturePreviewToStreamAsync(stream.AsStreamForWrite().AsRandomAccessStream());
                        }
                    }
                    if (!isFull)
                    {
                        webView.Width = widthTemp;
                        webView.Height = heightTemp;
                        await Task.Delay(500);
                    }
                    return storageFile.Path;
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                    return "";
                }
            }
        }
        public async Task<string> CaptureFullPage(StorageFolder storageFolder)
        {
            if (XamarinWebView)
            {
                //TO-DO make capture possible
                LocalNotificationData localNotificationData = new LocalNotificationData();
                localNotificationData.icon = SegoeMDL2Assets.Error;
                localNotificationData.type = Colors.Tomato;
                localNotificationData.message = "Capture is not supported in XWebView!";
                localNotificationData.time = 3;
                Helpers.pushLocalNotification(null, localNotificationData);
                return "";
            }
            else
            {
                try
                {
                    var widthTemp = webView.ActualWidth;
                    var heightTemp = webView.ActualHeight;
                    int width = (int)webView.ActualWidth;
                    int height = (int)webView.ActualHeight;

                    string jsString = "Math.max(document.body.scrollHeight, document.documentElement.scrollHeight, document.body.offsetHeight, document.documentElement.offsetHeight, document.body.clientHeight, document.documentElement.clientHeight)";

                    string Command = $"JSON.stringify({jsString})";
                    var executedScript = await webView.InvokeScriptAsync("eval", new string[] { Command });

                    height = Int32.Parse(executedScript);
                    if (height > 10000)
                    {
                        height = 10000;
                    }
                    var size = new Size(width, height);

                    webView.Width = size.Width;
                    webView.Height = size.Height;
                    await Task.Delay(500);
                    var FileLocation = await CaptureCurrentPage(storageFolder, true);

                    webView.Width = widthTemp;
                    webView.Height = heightTemp;
                    await Task.Delay(500);
                    return FileLocation;
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                    return "";
                }
            }
        }

        public async Task CreateScaledBitmapFromStreamAsync(int width, int height, IRandomAccessStream source)
        {
            TabPreview = new WriteableBitmap(width, height);
            try
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);
                BitmapTransform transform = new BitmapTransform();
                transform.ScaledHeight = (uint)height;
                transform.ScaledWidth = (uint)width;
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);
                pixelData.DetachPixelData().CopyTo(TabPreview.PixelBuffer);
            }
            catch (Exception e)
            {

            }
        }
        public async Task CreateTabPreview()
        {
            if (XamarinWebView)
            {
                //TO-DO make tab preview possible
                return;
            }
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (!Helpers.DisableAllIcons && webView != null && ConsoleList != null && !isLoading && DOMLoaded && PageLoadedState)
                        {
                            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                            await webView.CapturePreviewToStreamAsync(stream);
                            double thumbnailWidth = 92;
                            double thumbnailHeight = 92;
                            double webViewControlWidth = webView.ActualWidth;
                            double webViewControlHeight = webView.ActualHeight;
                            if (thumbnailWidth == 0 || thumbnailHeight == 0 || webViewControlWidth == 0 || webViewControlHeight == 0)
                            {
                                // Avoid 0x0 bitmaps, which cause all sorts of problems.
                                return;
                            }
                            double horizontalScale = thumbnailWidth / webViewControlWidth;
                            double verticalScale = thumbnailHeight / webViewControlHeight;
                            double scale = Math.Min(horizontalScale, verticalScale);
                            int width = (int)(webViewControlWidth * scale);
                            int height = (int)(webViewControlHeight * scale);

                            await CreateScaledBitmapFromStreamAsync(width, height, stream);

                            RaisePropertyChanged(nameof(TabPreview));
                            //Helpers.UpdateBindings(null, EventArgs.Empty);
                        }
                    }
                    catch (Exception e)
                    {
                        Helpers.Logger(e);
                    }
                });
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public bool DOMLoaded = false;
        public int LoadingProgress = 0;
        private void ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            if (WebHelper.isFirstLoad)
            {
                //Reload();
                WebHelper.isFirstLoad = false;
            }
            try
            {
                if (WebHelper.SuggestionsHideHandler != null)
                {
                    if (!CurrentURL.Contains("google.com/maps"))
                    {
                        WebHelper.SuggestionsHideHandler.Invoke(null, EventArgs.Empty);
                    }
                }

                /*try
                {
                    CancellationTokenProgressEmulate.Cancel();
                }
                catch (Exception e)
                {
                }*/
                //CancellationTokenProgressEmulate = new CancellationTokenSource();
                CheckAgentMode();
                if (!CustomPageLoading)
                {
                    CheckCookiesSettings(args.Uri, isClearCookiesRequest);
                }

                try
                {
                    WebHelper.isTextChangedByNavigate = true;
                    if (!CustomPageLoading)
                    {
                        if (args.Uri != null)
                        {
                            CurrentURL = args.Uri.ToString();
                        }
                    }
                    if (CurrentURL != CurrentURLTemp)
                    {
                        try
                        {
                            ClearConsole();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    WebHelper.isTextChangedByNavigate = true;
                    CurrentURLTemp = CurrentURL;
                    if (!CurrentURL.Contains("google.com/maps"))
                    {
                        WebHelper.AddressHandler.Invoke(CurrentURL, EventArgs.Empty);
                    }
                    try
                    {
                        if (CurrentURL.ToLower().Contains("youtube.com/watch"))
                        {
                            isDownloadPossible = Visibility.Visible;
                        }
                        else
                        {
                            isDownloadPossible = Visibility.Collapsed;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                catch (Exception e)
                {

                }
                //ProgressEmulate();
            }
            catch (Exception ex)
            {
                Helpers.ShowErrorMessage(ex);
            }
            //CheckPageLoaded();
        }

        /*CancellationTokenSource CancellationTokenProgressEmulate = new CancellationTokenSource();
        public async void ProgressEmulate()
        {
            try
            {
                if (isActive && WebProgress != null)
                {
                    while (!LoadingDoneChangingProgress && !DOMLoaded && LoadingProgress < 95)
                    {
                        CancellationTokenProgressEmulate.Token.ThrowIfCancellationRequested();
                        Interlocked.Increment(ref LoadingProgress);
                        Random random = new Random();
                        await Task.Delay(random.Next(1, 10), CancellationTokenProgressEmulate.Token);
                        WebProgress.Report(LoadingProgress);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }*/
        private void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }

        public bool LoadingDoneChangingProgress = false;
        public async Task CompletePageLoad()
        {
            try
            {
                if (WebHelper.SuggestionsHideHandler != null)
                {
                    WebHelper.SuggestionsHideHandler.Invoke(null, EventArgs.Empty);
                }
                LoadingDoneChangingProgress = true;
                while (LoadingProgress < 100)
                {
                    await Task.Delay(1);
                    Interlocked.Increment(ref LoadingProgress);
                    if (isActive && WebProgress != null)
                    {
                        WebProgress.Report(LoadingProgress);
                    }
                }
                isLoading = false;
                PageLoadedState = true;
                isLoadingVisible = Visibility.Collapsed;
                try
                {
                    _ = ExtensionsLoader();
                    _ = GetFavIconAsync();
                    _ = FetchPageInfo();
                    //_ = CreateTabPreview();
                }
                catch (Exception ec)
                {
                    AddToConsoleList(ec);
                }
            }
            catch (Exception ex)
            {
                AddToConsoleList(ex);
            }
        }
        private void LoadingDone(object sender, NavigationEventArgs e)
        {
            try
            {
                
              

                //"document.documentElement.outerHTML"
            }
            catch (Exception ex)
            {
                AddToConsoleList(ex);
            }
        }

        public async Task ExtensionsLoader(StorageFile singleFile = null)
        {
            try
            {
                if (singleFile == null)
                {
                    if (Extensions != null)
                    {
                        foreach (var ExtensionItem in Extensions)
                        {
                            try
                            {
                                //content = SolveScriptContent(content);
                                switch (ExtensionItem.ExtensionType)
                                {
                                    case "JavaScript":
                                        var content = await PathIO.ReadTextAsync(ExtensionItem.ExtensionLocation);
                                        if (content.Length > 0)
                                        {
                                            AddToConsoleList("warn", $"Invoke: {ExtensionItem.ExtensionFile.Name}");
                                            string output = await SendCommand(content, false);
                                        }
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                AddToConsoleList(e);
                            }
                        }
                    }
                }
                else
                {
                    var content = await PathIO.ReadTextAsync(singleFile.Path);
                    //content = SolveScriptContent(content);
                    AddToConsoleList("warn", $"Invoke: {singleFile.Name}");
                    string output = await SendCommand(content, false);
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public string SolveScriptContent(string content)
        {
            try
            {
                content = content.Replace("\r\n", " ");
                content = content.Replace("\r", "");
                content = content.Replace("\n", "");
                content = content.Replace("\\\"", "\"");
                content = content.Replace("\\'", "'");
            }
            catch (Exception e)
            {

            }
            return content;
        }
        private async Task FetchPageInfo()
        {
            //Moved to create preview timer
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (XamarinWebView)
                        {
                            string title = await XWebView.InjectJavascriptAsync("document.title");
                            if (title != null && title.Length > 0)
                            {
                                PageTitle = title;
                            }
                            else
                            {
                                PageTitle = "Web Page";
                            }
                        }
                        else
                        {
                            string title = webView.DocumentTitle;
                            if (title != null && title.Length > 0)
                            {
                                PageTitle = title;
                            }
                            else
                            {
                                PageTitle = "Web Page";
                            }
                        }
                        RaiseAllChanged();
                    }
                    catch (Exception e)
                    {
                        AddToConsoleList(e);
                    }
                });
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        private async Task GetFavIconAsync()
        {
            try
            {
                /*if (CustomPageLoading)
                {
                    PageIcon = WebHelper.EMPTY_FAVICON;
                }
                else*/
                {
                    string fileName = $"{CurrentHost.Replace(".", "_")}.png";
                    var TestIcon = (StorageFile)await ApplicationData.Current.RoamingFolder.TryGetItemAsync(fileName);
                    if (TestIcon != null)
                    {
                        PageIcon = TestIcon.Path;
                    }
                    else if (SearchForInternalLogo().Length > 0)
                    {
                        PageIcon = SearchForInternalLogo();
                    }
                    else
                    {
                        var WebIcon = "";
                        if (XamarinWebView)
                        {
                            WebIcon = await XWebView.InjectJavascriptAsync("JSON.stringify(Array.from(document.getElementsByTagName('link')).filter(link => link.rel.includes('icon')).map(link => link.href));");
                        }
                        else
                        {
                            WebIcon = await webView.InvokeScriptAsync("eval", new string[] { "JSON.stringify(Array.from(document.getElementsByTagName('link')).filter(link => link.rel.includes('icon')).map(link => link.href))" });
                        }
                        if (WebIcon != null && WebIcon.Length > 5)
                        {
                            var IconsArray = JsonConvert.DeserializeObject<string[]>(WebIcon);
                            if (IconsArray.Length > 0)
                            {
                                var TempPageIcon = IconsArray[0];
                                foreach (var iconItem in IconsArray)
                                {
                                    if (iconItem.Contains(".ico"))
                                    {
                                        TempPageIcon = iconItem;
                                        break;
                                    }
                                    else if (iconItem.Contains(".png"))
                                    {
                                        TempPageIcon = iconItem;
                                    }
                                }
                                if (TempPageIcon.Contains("base64"))
                                {
                                    TempPageIcon = TempPageIcon.Replace("data:image/png;base64,", "");
                                    TempPageIcon = TempPageIcon.Replace("data:image/jpg;base64,", "");
                                    TempPageIcon = TempPageIcon.Replace("data:image/jpeg;base64,", "");
                                    byte[] data = Convert.FromBase64String(TempPageIcon);

                                    StorageFile storageFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                    if (storageFile != null)
                                    {
                                        using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                                        {
                                            await stream.WriteAsync(data.AsBuffer());
                                            await stream.FlushAsync();
                                            stream.Dispose();
                                        }
                                    }
                                    PageIcon = storageFile.Path;
                                }
                                else if (!TempPageIcon.StartsWith("http") && WebHelper.isLinkFile(TempPageIcon))
                                {
                                    if (CurrentURL.Contains("https"))
                                    {
                                        TempPageIcon = $"https://{CurrentHost}/{TempPageIcon}";
                                    }
                                    else
                                    {
                                        TempPageIcon = $"http://{CurrentHost}/{TempPageIcon}";
                                    }
                                }
                                else if (!TempPageIcon.StartsWith("http"))
                                {
                                    PageIcon = WebHelper.EMPTY_FAVICON;
                                }
                                else
                                {
                                    PageIcon = TempPageIcon;
                                }
                            }
                            else
                            {
                                PageIcon = WebHelper.EMPTY_FAVICON;
                            }
                        }
                        else
                        {
                            PageIcon = WebHelper.EMPTY_FAVICON;
                        }
                    }
                    try
                    {
                        if (CurrentURL != null && CurrentURL.Length > 0 && !CurrentURL.StartsWith("blank:") && !CurrentURL.StartsWith("offline:") && !CurrentURL.StartsWith("error:") && !CurrentURL.StartsWith("security:") && !CurrentURL.EndsWith(":blank") && !isStringContentLoaded && !CurrentURL.ToLower().Equals(WebHelper.StartupURL.ToLower()))
                        {
                            await Task.Delay(1500);
                            string testTitle = await SendCommand("document.title");
                            if (testTitle != null && testTitle.Length > 0)
                            {
                                if (!PageTitle.Equals(testTitle))
                                {
                                    PageTitle = testTitle;
                                }
                            }
                            WebHelper.AddToHistory(this, EventArgs.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddToConsoleList(ex);
                    }
                }
                RaiseAllChanged();
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public async Task ChangeZoom(double zoomLevel)
        {
            try
            {
                var MinZoom = 0;
                var MaxZoom = 500;
                var CurrentZoom = zoomLevel;
                var ScaleValue = Math.Max(Math.Min(CurrentZoom, MaxZoom), MinZoom) / 100;
                //['height', 'calc(' + ({ScaleValue}) + 'px + ' + (100 / {ScaleValue}) + '%)'],
                string zoomCommand = $@"document.body.setAttribute('style', [
                ['width', (100 / {ScaleValue}) + '%'],
                ['transform', 'scale(' + {ScaleValue} + ')'],
                ['transform-origin', '0 0']
                ].map(pair => pair[0] + ': ' + pair[1]).join('; '));";
                if (zoomLevel == 100)
                {
                    zoomCommand = $@"document.body.setAttribute('style', [
                ['width', 'unset'],
                ['transform', 'unset'],
                ['transform-origin', 'unset']
                ].map(pair => pair[0] + ': ' + pair[1]).join('; '));";
                }
                if (XamarinWebView)
                {
                    await XWebView.InjectJavascriptAsync(zoomCommand);
                }
                else
                {
                    await webView.InvokeScriptAsync("eval", new string[] { zoomCommand });
                }
            }
            catch (Exception e)
            {

            }
        }

        public async Task SearchForText(string text)
        {
            try
            {
                text = text.Trim();
                await SendCommand($"W10MSearchInPage('{text}')");
            }
            catch (Exception e)
            {

            }
        }
        public async Task<string> SendCommand(string command, bool Stringify = true)
        {
            try
            {
                string output = "";
                var taskCompletionSource = new TaskCompletionSource<bool>();
                await DispatcherHelper.returnDispatcher().RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        string Command = $"JSON.stringify({command})";
                        if (!Stringify || !ConsoleStringify)
                        {
                            Command = command;
                        }
                        try
                        {
                            if (XamarinWebView)
                            {
                                output = await XWebView.InjectJavascriptAsync(Command);
                            }
                            else
                            {
                                output = await webView.InvokeScriptAsync("eval", new string[] { Command });
                            }
                        }
                        catch (Exception e)
                        {
                            output = $"{e.Message}";
                        }
                        try
                        {
                            if (output.Length > 0 && Helpers.IsValidJson(output))
                            {
                                JToken parsedJson = JToken.Parse(output);
                                output = parsedJson.ToString(Formatting.Indented);
                                output = output.Replace("\\\"", "\"");
                            }
                        }
                        catch (Exception e)
                        {
                            //AddToConsoleList(e);
                        }
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception e)
                    {
                        AddToConsoleList(e);
                    }
                });

                await taskCompletionSource.Task;
                return output;
            }
            catch (Exception e)
            {
                return $"{e.Message}";
            }
        }
        public void PostData(string data)
        {
            try
            {
                var PostCount = 0;
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(CurrentURL));
                try
                {
                    httpRequestMessage.Headers.UserAgent.Clear();
                    HttpProductInfoHeaderValue userAgent = new HttpProductInfoHeaderValue(WebHelper.UserAgentMobile);
                    if (!isMobileVersion)
                    {
                        userAgent = new HttpProductInfoHeaderValue(WebHelper.UserAgentDesktop);
                    }
                    httpRequestMessage.Headers.UserAgent.Add(userAgent);
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }
                try
                {
                    var dataArray = data.Split('&');
                    if (dataArray.Length > 0)
                    {
                        postValues = new List<KeyValuePair<string, string>>();
                        foreach (var arrayItem in dataArray)
                        {
                            var currentParam = arrayItem.Split('=');
                            if (currentParam.Length > 1)
                            {
                                string key = currentParam[0].Trim();
                                string value = currentParam[1].Trim();
                                if (key.Length > 0 && value.Length > 0)
                                {
                                    httpRequestMessage.Headers.Add(key, value);
                                    postValues.Add(new KeyValuePair<string, string>(key, value));
                                    AddToConsoleList("info", $"POST-> key: {key} = value: {value}");
                                    PostCount++;
                                }
                                else
                                {
                                    AddToConsoleList("warn", $"Key:[{key}], Value:[{value}] rejected due incorrect format!");
                                }
                            }
                        }
                        HttpFormUrlEncodedContent httpFormUrlEncodedContent = new HttpFormUrlEncodedContent(postValues.ToArray());
                        httpRequestMessage.Content = httpFormUrlEncodedContent;
                        httpRequestMessage.Method = HttpMethod.Post;
                    }
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }
                if (PostCount > 0)
                {
                    if (XamarinWebView)
                    {
                        //TO-DO make post possible
                        LocalNotificationData localNotificationData = new LocalNotificationData();
                        localNotificationData.icon = SegoeMDL2Assets.Error;
                        localNotificationData.type = Colors.Tomato;
                        localNotificationData.message = "Post data is not supported in XWebView!";
                        localNotificationData.time = 3;
                        Helpers.pushLocalNotification(null, localNotificationData);
                    }
                    else
                    {
                        webView.NavigateWithHttpRequestMessage(httpRequestMessage);
                    }
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        bool requestScrollToEnd = false;
        public void AddToConsoleList(string type, string data, bool isMyLog = false)
        {
            //resolve language
            data = getLocalString(data);

            try
            {
                if (!isConsoleOpen)
                {
                    return;
                }

                if (ConsoleList != null)
                {
                    var currentTime = DateTime.Now.ToLocalTime().ToString();
                    var uniqID = $"{currentTime}_{Path.GetRandomFileName()}_{Path.GetRandomFileName()}";

                    ConsoleList.Add(uniqID, new string[] { $">{currentTime}", "time" });
                    if (data.Trim().Length == 0)
                    {
                        data = "Empty";
                    }
                    if (!isMyLog && data.Length > 95 && type.Equals("error") && !ConsoleLog)
                    {
                        var fullData = data;
                        var shortData = $"{data.Substring(0, 95)}\n(Enable 'log' to see full error)";
                        ConsoleList.Add($"{uniqID}_", new string[] { shortData, type });
                    }
                    else
                    {
                        ConsoleList.Add($"{uniqID}_", new string[] { data, type });
                    }

                    requestScrollToEnd = true;
                }
            }
            catch (Exception e)
            {

            }
        }
        public void AddToConsoleList(Exception e, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                AddToConsoleList("error", $"Function:{memberName}, Line:{sourceLineNumber} \n{ e.Message}");
            }
            catch (Exception ex)
            {

            }
        }
        public async void ClearConsole()
        {
            try
            {
                ConsoleList.Clear();
                Helpers.GCCollectForList(ConsoleList);
            }
            catch (Exception e)
            {

            }
        }

        public Timer LoadingMonitorTimer;
        private async void callLoadingMonitorTimer(bool startState = false)
        {
            try
            {
                try
                {
                    LoadingMonitorTimer?.Dispose();
                    if (startState)
                    {
                        DOMLoaded = false;
                        LoadingDoneChangingProgress = false;
                        LoadingProgress = 0;
                        isLoading = true;
                        PageLoadedState = false;
                        isLoadingVisible = Visibility.Visible;
                        LoadingMonitorTimer = new Timer(async delegate
                        {
                            try
                            {
                                await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    try
                                    {
                                        bool pageLoadedState = false;
                                        if (CustomPageLoading)
                                        {
                                            LoadingMonitor();
                                            LoadingMonitorTimer?.Dispose();
                                        }
                                        else
                                        {
                                            try
                                            {
                                                string testURL = await SendCommand("window.location.href");
                                                if (testURL.Equals(CurrentURL))
                                                {
                                                    pageLoadedState = true;
                                                    //AddToConsoleList("info", PageTitle);
                                                }
                                            }
                                            catch (Exception e)
                                            {

                                            }
                                            if (pageLoadedState)
                                            {
                                                LoadingMonitorTimer?.Dispose();
                                                LoadingMonitor();
                                            }
                                            else
                                            {
                                                Interlocked.Increment(ref LoadingProgress);
                                                if (isActive && WebProgress != null)
                                                {
                                                    if (LoadingProgress <= 100 && !LoadRequestCanceled)
                                                    {
                                                        WebProgress.Report(LoadingProgress);
                                                    }
                                                    else
                                                    {
                                                        LoadingMonitor();
                                                        LoadingMonitorTimer?.Dispose();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        AddToConsoleList(e);
                                    }
                                });
                            }
                            catch (Exception e)
                            {
                                AddToConsoleList(e);
                            }
                        }, null, 0, 50);
                    }
                }
                catch (Exception e)
                {
                    AddToConsoleList(e);
                }

                /*try
                {
                    CancellationTokenProgressEmulate.Cancel();
                }
                catch (Exception e)
                {

                }*/
                /*if (startState)
                {
                    while (!LoadingDoneChangingProgress && LoadingProgress < 95)
                    {
                        await Task.Delay(1);
                        Interlocked.Increment(ref LoadingProgress);
                        if (isActive && WebProgress != null)
                        {
                            WebProgress.Report(LoadingProgress);
                        }
                    }
                }*/
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }

        }
        private async void LoadingMonitor()
        {
            try
            {
                if (WebHelper.SuggestionsHideHandler != null)
                {
                    WebHelper.SuggestionsHideHandler.Invoke(null, EventArgs.Empty);
                }

                try
                {
                    /*if (isActive && (CurrentURL.StartsWith("blank:") || CurrentURL.StartsWith("error:") || CurrentURL.StartsWith("security:") || CurrentURL.EndsWith(":blank")))
                    {
                        if (isLoadingVisible != Visibility.Collapsed)
                        {
                            webView.Stop();
                            isLoading = false;
                            PageLoadedState = true;
                            if (isActive && WebProgress != null)
                            {
                                WebProgress.Report(100);
                            }
                            isLoadingVisible = Visibility.Collapsed;
                            RaiseAllChanged();
                        }
                    }
                    else*/
                    {
                        if (CurrentURL.ToLower().Contains("youtube.com/watch"))
                        {
                            if (isDownloadPossible != Visibility.Visible)
                            {
                                isDownloadPossible = Visibility.Visible;
                            }
                        }
                        else
                        {
                            if (isDownloadPossible != Visibility.Collapsed)
                            {
                                isDownloadPossible = Visibility.Collapsed;
                            }
                        }

                        try
                        {
                            await CompletePageLoad();
                            await InjectJavaScriptWrappers();
                            DOMLoaded = true;
                            PageLoadedState = true;
                            CustomPageLoading = false;
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }
                catch (Exception ec)
                {
                    AddToConsoleList(ec);
                }

            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }
        public Timer ConsoleMonitorTimer;
        private void callConsoledMonitorTimer(bool startState = false)
        {
            try
            {
                ConsoleMonitorTimer?.Dispose();
                if (startState)
                {
                    ConsoleMonitorTimer = new Timer(delegate
                    {
                        if (isActive && isConsoleOpen)
                        {
                            if (requestScrollToEnd)
                            {
                                WebHelper.ReloadConsoleOutput("", EventArgs.Empty);
                            }
                            else
                            {
                                WebHelper.ReloadConsoleOutput(null, EventArgs.Empty);
                            }
                        }
                        requestScrollToEnd = false;
                    }, null, 0, 250);
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public Timer ThumbnailMonitorTimer;
        private void callThumbnailMonitorTimer(bool startState = false)
        {
            try
            {
                ThumbnailMonitorTimer?.Dispose();
                if (startState)
                {
                    ThumbnailMonitorTimer = new Timer(async delegate
                    {
                        if (isActive && !WebHelper.isPanelOpen && Helpers.isTabsPageOpened)
                        {
                            await CreateTabPreview();

                            try
                            {
                                await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    try
                                    {
                                        string title = "";
                                        if (XamarinWebView)
                                        {
                                            title = await XWebView.InjectJavascriptAsync("document.title");
                                        }
                                        else
                                        {
                                            title = webView.DocumentTitle;
                                        }
                                        if (title != null && title.Length > 0)
                                        {
                                            PageTitle = title;
                                        }
                                        else
                                        {
                                            PageTitle = "Web Page";
                                        }
                                        RaiseAllChanged();
                                    }
                                    catch (Exception e)
                                    {
                                        AddToConsoleList(e);
                                    }
                                });
                            }
                            catch (Exception e)
                            {
                                AddToConsoleList(e);
                            }
                        }
                    }, null, 0, 1000);
                }
            }
            catch (Exception e)
            {
                AddToConsoleList(e);
            }
        }

        public string GetHostName(string url)
        {
            try
            {
                if (WebHelper.IsAllowedUri(url))
                {
                    Uri myUri = new Uri(url);
                    string host = myUri.Host;
                    return host;
                }
                else
                {
                    return "None";
                }

            }
            catch (Exception e)
            {
                return url;
            }
        }
        public string GetHostName(Uri uri)
        {
            try
            {
                if (!CustomPageLoading)
                {
                    string host = uri.Host;
                    return host;
                }
                else
                {
                    return "Static Page";
                }
            }
            catch (Exception e)
            {
                return uri.ToString();
            }
        }
        public string GetStringCRC32(string InputText)
        {
            try
            {
                Encoding unicode = Encoding.Unicode;
                var InputTextBytes = unicode.GetBytes(InputText);

                Crc32 crc32 = new Crc32();
                var FinalCRC32 = crc32.ComputeHash(InputTextBytes);

                var FinalText = unicode.GetString(FinalCRC32);
                return FinalText;
            }
            catch (Exception e)
            {
                return InputText;
            }
        }

        public string ExtractHostName(string host)
        {
            string hostName = host;
            try
            {
                var HostExtract = @"\w+\.(?<host>.*)(?<ext>\.\w+)";
                var m = Regex.Match(host, HostExtract, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (m.Groups != null && m.Groups.Count > 0)
                    {
                        hostName = $"{m.Groups["host"]}{m.Groups["ext"]}";
                    }
                }
            }
            catch (Exception e)
            {

            }

            return hostName;
        }
        public string SearchForInternalLogo()
        {
            string logo = "";
            try
            {
                string hostName = Path.GetFileNameWithoutExtension(CurrentHost);
                try
                {
                    var HostExtract = @"\w+\.(?<host>.*)(?<ext>\.\w+)";
                    var m = Regex.Match(CurrentHost, HostExtract, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            hostName = $"{m.Groups["host"]}";
                        }
                    }
                }
                catch (Exception e)
                {

                }
                switch (hostName)
                {
                    case "google":
                    case "microsoft":
                    case "mega":
                    case "duckduckgo":
                    case "twitter":
                    case "apple":
                        logo = $"ms-appx:///Assets/Sites/{hostName}.bmp";
                        break;
                }
            }
            catch (Exception e)
            {

            }
            return logo;
        }

        public void PrepareToRemove()
        {
            try
            {
                try
                {
                    cancellationTokenSource.Cancel();
                }
                catch (Exception e)
                {

                }
                webView = null;
                ConsoleList = null;
                History = null;
                callConsoledMonitorTimer();
                callThumbnailMonitorTimer();
                callLoadingMonitorTimer();
                ConsoleMonitorTimer = null;
                ConsoleMonitorTimer = null;
            }
            catch (Exception e)
            {

            }
        }

        public void ClearCache()
        {
            try
            {
                WebHelper.NewTabHandler(CurrentURL, EventArgs.Empty);
                WebHelper.CloseTabHandler(SessionID, EventArgs.Empty);
            }
            catch (Exception e)
            {

            }
        }
    }
    public sealed class BadHttpsStreamResolver : IUriToStreamResolver
    {
        private readonly string baseUri;
        private readonly string localStreamUri;
        private readonly HttpClient hc;

        public BadHttpsStreamResolver(Uri baseUri, Uri localStreamUri)
        {
            this.baseUri = baseUri.ToString();
            this.localStreamUri = localStreamUri.ToString();
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            //Specify here which certificate errors should we ignore
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidCertificateAuthorityPolicy);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidSignature);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.BasicConstraintsError);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationFailure);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationInformationMissing);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.OtherErrors);
            hc = new HttpClient(filter);
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            // TODO better uri validation and conversion
            Uri targetUri = new Uri(uri.ToString().Replace(localStreamUri, baseUri));
            return GetInputStream(targetUri).AsAsyncOperation();
        }

        public async Task<IInputStream> GetInputStream(Uri targetUri)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, targetUri);
                HttpResponseMessage response = await hc.SendRequestAsync(request);
                IInputStream stream = await response.Content.ReadAsInputStreamAsync();
                return stream;
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
                return null;
            }
        }
    }
}
