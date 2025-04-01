/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WinUniversalTool.AppInstaller;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using WinUniversalTool.WebViewer;
using WinUniversalTool.MegaApiClientSource;
using WinUniversalTool.Views;
using Windows.Media.Editing;
using Windows.Media.Transcoding;
using Windows.Media.MediaProperties;
using WinUniversalTool.DirectStorage;
using Windows.Web.Http.Headers;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using System.IO.Packaging;
using System.Xml.Linq;
using System.IO.Compression;
using System.Linq;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Commander.Compiler;

namespace WinUniversalTool.Models
{
    public class QueueItem : BindableBase
    {

        [JsonIgnore]
        public Thickness BorderState
        {

            get
            {
                if (Helpers.ShowSeparator)
                {
                    return new Thickness(0, 0, 0, 1);
                }
                else
                {
                    return new Thickness(0, 0, 0, 0);
                }
            }
        }

        public void updateIconsState()
        {
            try
            {
                RaisePropertyChanged(nameof(BorderState));
            }
            catch (Exception e)
            {

            }
        }

        public bool isDeleted = false;
        public async Task PrepareForDelete()
        {
            try
            {
                if (UniqID.Length > 0)
                {
                    Helpers.DownloadsExecptions.Remove(UniqID);
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                DownloadReport = "Removing (please wait)..";
                isDeleted = true;
                callDownloadMonitorTimer();
                try
                {
                    if (chunks != null)
                    {
                        foreach (var chunk in chunks.ChunkItems)
                        {
                            chunk.ChunkFile = null;
                            chunk.RemoteStream = null;
                            chunks.ChunkItems.Remove(chunk);
                        }
                        chunks.ChunkItems = null;
                    }
                }
                catch (Exception e)
                {

                }
                chunks = null;
                try
                {
                    if (Installer != null)
                    {
                        Installer.DownloadsFolder = null;
                        Installer.packageInContext = null;
                        Installer.ParentFolder = null;
                        Installer.dependencies.Clear();
                    }
                }
                catch (Exception e)
                {

                }
                Installer = null;

                try
                {
                    if (XAPInstaller != null)
                    {
                        try
                        {
                            XAPInstaller.cancellationTokenSource.Cancel();
                        }
                        catch (Exception ex)
                        {

                        }

                        XAPInstaller.isCancellationRequest = true;
                        XAPInstaller.isTelnetTerminated = true;
                        int countss = 0;
                        while (countss < 10 && XAPInstaller.isXAPInProgress)
                        {
                            await Task.Delay(1000);
                            countss++;
                        }

                        XAPInstaller.DownloadsFolder = null;
                        XAPInstaller.LogFile = null;
                        if (XAPInstaller.appxInstaller != null)
                        {
                            XAPInstaller.appxInstaller.DownloadsFolder = null;
                            XAPInstaller.appxInstaller.packageInContext = null;
                            XAPInstaller.appxInstaller.ParentFolder = null;
                            XAPInstaller.appxInstaller.dependencies.Clear();
                        }
                        XAPInstaller.appxInstaller = null;
                    }
                }
                catch (Exception e)
                {

                }
                XAPInstaller = null;
                try
                {
                    if (ScriptInstaller != null)
                    {
                        try
                        {
                            ScriptInstaller.isCancellationRequest = true;
                            ScriptInstaller.isTelnetTerminated = true;
                            int counts = 0;
                            while (counts < 10 && ScriptInstaller.isScriptInProgress)
                            {
                                await Task.Delay(1000);
                                counts++;
                            }
                            foreach (var tItem in CCache.TimersCache.Keys)
                            {
                                try
                                {
                                    var timerRow = CCache.TimersCache[tItem];
                                    if (timerRow.ScriptID.Equals(ScriptInstaller))
                                    {
                                        try
                                        {
                                            timerRow.Stop(true);
                                            CCache.TimersCache.Remove(tItem);
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
                        if (ScriptInstaller.appxInstaller != null)
                        {
                            ScriptInstaller.appxInstaller.DownloadsFolder = null;
                            ScriptInstaller.appxInstaller.packageInContext = null;
                            ScriptInstaller.appxInstaller.ParentFolder = null;
                            ScriptInstaller.appxInstaller.dependencies.Clear();
                        }
                        ScriptInstaller.appxInstaller = null;
                        ScriptInstaller.BreakHandlers.Clear();
                        ScriptInstaller.DownloadsFolder = null;
                        ScriptInstaller.FilesHandlers.Clear();
                        ScriptInstaller.FoldersHandlers.Clear();
                        ScriptInstaller.LogFile = null;
                        ScriptInstaller.ResultHandlers.Clear();

                        if (ScriptInstaller.xapInstaller != null)
                        {
                            ScriptInstaller.xapInstaller.DownloadsFolder = null;
                            ScriptInstaller.xapInstaller.LogFile = null;
                            if (ScriptInstaller.xapInstaller.appxInstaller != null)
                            {
                                ScriptInstaller.xapInstaller.appxInstaller.DownloadsFolder = null;
                                ScriptInstaller.xapInstaller.appxInstaller.packageInContext = null;
                                ScriptInstaller.xapInstaller.appxInstaller.ParentFolder = null;
                                ScriptInstaller.xapInstaller.appxInstaller.dependencies.Clear();
                            }
                            ScriptInstaller.xapInstaller.appxInstaller = null;
                        }
                        ScriptInstaller.xapInstaller = null;
                    }
                }
                catch (Exception e)
                {

                }
                ScriptInstaller = null;
                testFile = null;
                StorageFolder = null;
                SelectedFile = null;
                SourceStream = null;
            }
            catch (Exception e)
            {

            }
        }

        public Symbol SymbolIcon
        {
            get
            {
                return ScriptInstaller.SymbolIcon;
            }
            set
            {
                ScriptInstaller.SymbolIcon = value;
                RaisePropertyChanged(nameof(SymbolIcon));
            }
        }
        public Visibility isSymbolVisible = Visibility.Collapsed;
        public void updateSymbol(Symbol symbolInput)
        {
            try
            {
                SymbolIcon = symbolInput;
            }
            catch (Exception ex)
            {

            }
        }

        public string ScriptID
        {
            get
            {
                return scriptID;
            }

            set
            {
                if (value.Contains("|"))
                {
                    var data = value.Split('|');
                    scriptID = data[0];
                    downloadID = data[1];
                    if (data.Length >= 3)
                    {
                        var extra = data[2];
                        if (extra.Equals("Cache"))
                        {
                            StorageFolder = ApplicationData.Current.LocalCacheFolder;
                        }
                    }
                    if (data.Length >= 4)
                    {
                        var extra = data[3];
                        if (extra.Equals("ScriptUpdate"))
                        {
                            isScriptUpdate = true;
                        }
                    }
                }
                if (scriptID.Length > 0)
                {
                    DownloadItem downloadItem = new DownloadItem();
                    downloadItem.ScriptID = scriptID;
                    downloadItem.FileID = downloadID;
                    downloadItem.fileLink = DirectURL;
                    downloadItem.fileName = FileName;
                    downloadItem.isDownloaded = false;
                    downloadItem.isFailed = false;
                    Helpers.DownloadsMonitor.Add(downloadItem);
                    Commander.Native.Variables.Define(downloadID, FileName);
                }
            }
        }
        public Chunks chunks;
        public AppxInstaller Installer = new AppxInstaller();
        public XAPInstaller XAPInstaller = new XAPInstaller();
        public ScriptInstaller ScriptInstaller = new ScriptInstaller();

        public bool SupportBackground = false;
        public bool IsSessionStarted = false;
        public bool DirectDownload = false;
        public bool UpdateFile = false;
        public bool AutoOpen = false;
        public string UpdateFileVersion = "";
        public string UpdateFileName = "";
        public string DirectURL = "";
        private string scriptID = "";
        public string downloadID = "";
        public bool scriptApprovedToRun = false;
        public bool devModeActiveDialog = false;

        public bool isScriptUpdate = false;
        private void updateMonitorState(bool state, bool failed = false)
        {
            try
            {
                foreach (var dItem in Helpers.DownloadsMonitor)
                {
                    if (dItem.FileID.Equals(downloadID) && dItem.ScriptID.Equals(ScriptID))
                    {
                        if (state)
                        {
                            if (!dItem.isDownloaded)
                            {
                                dItem.isDownloaded = state;
                                dItem.isFailed = failed;
                                break;
                            }
                        }
                        else
                        {
                            dItem.isDownloaded = state;
                            dItem.isFailed = failed;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public long TotalDownloadedForDirectFile = 0;

        public StorageFile testFile = null;
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
        public Timer DownloadMonitorTimer;
        private void callDownloadMonitorTimer(bool startState = false)
        {
            try
            {
                DownloadMonitorTimer?.Dispose();
                if (startState)
                {
                    DownloadMonitorTimer = new Timer(delegate { DownloadMonitor(null, EventArgs.Empty); }, null, 0, 1000);
                }
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }
        public async void DownloadMonitor(object sender, EventArgs e)
        {
            try
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        if (chunks.CurrentProgress >= 100)
                        {
                            if (EstimatedTime.Length > 0)
                            {
                                EstimatedTime = "";
                            }
                            DownloadReport = "Finish";
                            isDownloading = false;
                            ForceRaise = true;
                            try
                            {
                                SourceStream.Dispose();
                            }
                            catch (Exception ee)
                            {

                            }
                            FilesCombine();
                            callDownloadMonitorTimer();

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                _ = Helpers.ShowCatchedErrorAsync(ex);
            }
        }
        public double currentProgress;
        public double CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                currentProgress = value;
                RaisePropertyChanged2(nameof(CurrentProgress));
            }
        }
        public bool downloadDone = false;
        public bool isDownloadDone
        {
            get
            {
                return downloadDone;
            }

            set
            {
                downloadDone = value;
                try
                {
                    if (downloadDone)
                    {
                        var ext = Path.GetExtension(FileName);
                        switch (ext)
                        {
                            case ".wutc":
                            case ".wuts":
                            case ".wutz":
                            case ".w10mc":
                            case ".w10ms":
                            case ".w10mz":
                                isSymbolVisible = Visibility.Visible;
                                RaisePropertyChanged(nameof(isSymbolVisible));
                                break;

                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public bool isDownloadFailed = false;
        public bool isDownloadCanceled = false;
        public bool isFolderDownload = false;
        public bool isFirstDownload = false;
        public bool ForceRaise = false;

        public string FileName;
        public string FileNameDisplay
        {
            get
            {
                var FileNameNew = FileName.Replace("NoTelnet", "");
                return FileNameNew;
            }
        }
        public string FileIcon
        {
            get
            {
                return ScriptInstaller.FileIcon;
            }

            set
            {
                ScriptInstaller.FileIcon = value;
            }
        }
        //private string fileType;
        public string fileType
        {
            get
            {
                return ScriptInstaller.FileType;
            }

            set
            {
                ScriptInstaller.FileType = value;
            }
        }
        private string estimatedTime = "";
        public string EstimatedTime
        {
            get
            {
                return ScriptInstaller.EstimatedTime;
            }
            set
            {
                ScriptInstaller.EstimatedTime = value;
                //RaisePropertyChanged2(nameof(FileType), true);
            }
        }
        public string FileType
        {
            get
            {
                if (EstimatedTime.Length > 0)
                {
                    return $"{EstimatedTime} / {ScriptInstaller.FileType}";
                }
                else
                {
                    return ScriptInstaller.FileType;
                }
            }
            set
            {
                ScriptInstaller.FileType = value;
                RaisePropertyChanged2(nameof(FileType), true);
            }
        }
        public string FileSize;
        public string DownloadFullLocation;
        public string downloadReport = "";
        public string DownloadReport
        {
            get
            {
                return ScriptInstaller.DownloadReport;
            }
            set
            {
                if (SelectedFile == null || SelectedFile.URIDownloadAuthor.Length == 0)
                {
                    ScriptInstaller.DownloadReport = value;
                }
                else
                {
                    var fromTag = $"From {SelectedFile.URIDownloadAuthor.Replace("%20", "")}";
                    if (SelectedFile.URIDownloadAuthor.Contains("Publisher:"))
                    {
                        fromTag = $"{SelectedFile.URIDownloadAuthor.Replace("%20", "")}";
                    }
                    if (value.Contains(fromTag))
                    {
                        ScriptInstaller.DownloadReport = $"{value}";
                    }
                    else
                    {
                        if (value.Length > 0)
                        {
                            ScriptInstaller.DownloadReport = $"{value}\n{fromTag}";
                        }
                        else
                        {
                            ScriptInstaller.DownloadReport = $"{fromTag}";
                        }
                    }
                }
                RaisePropertyChanged2(nameof(DownloadReport));
            }
        }
        public long FileSizeLong;
        public StorageFolder StorageFolder;
        public bool isStarted = false;
        public bool isDownloading = false;
        public BrowseListModel SelectedFile;

        public async void updateFileData(StorageFile storageFile = null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (testFile != null || storageFile != null)
                    {
                        var fileExt = Path.GetExtension((storageFile != null ? storageFile : testFile).Name);
                        Stream packageStream = null;
                        var stream = (await (storageFile != null ? storageFile : testFile).OpenReadAsync()).AsStream();
                        bool isAppxBundle = false;
                        switch (fileExt.ToLower())
                        {
                            case ".appxbundle":
                            case ".msixbundle":
                                ZipArchive zip = new ZipArchive(stream);
                                foreach (var item in zip.Entries)
                                {
                                    if (item.Name.Contains(".appx") || item.Name.Contains(".msix"))
                                    {
                                        if (!item.Name.ToLower().Contains("_language-") && !item.Name.ToLower().Contains("_scale-"))
                                        {
                                            packageStream = item.Open();
                                            isAppxBundle = true;
                                            break;
                                        }
                                    }
                                }
                                break;

                            case ".appx":
                            case ".msix":
                                packageStream = stream;
                                break;
                        }
                        if (packageStream != null)
                        {
                            // NuGet: System.IO.Packaging
                            var package = Package.Open(packageStream);
                            var part = package.GetPart(new Uri("/AppxManifest.xml", UriKind.Relative));
                            var manifest = XElement.Load(part.GetStream());

                            var properties = manifest.Element(manifest.Name.Namespace + "Properties");
                            var packageName = properties.Element(manifest.Name.Namespace + "DisplayName").Value;
                            var packagePublisher = properties.Element(manifest.Name.Namespace + "PublisherDisplayName").Value;
                            var packageLogo = properties.Element(manifest.Name.Namespace + "Logo").Value;

                            var MinBuild = "8.1";
                            try
                            {
                                var dependencies = manifest.Element(manifest.Name.Namespace + "Dependencies");
                                var targetDevice = dependencies.Element(manifest.Name.Namespace + "TargetDeviceFamily");
                                MinBuild = targetDevice.Attribute("MinVersion").Value;
                            }
                            catch (Exception ex)
                            {

                            }
                            var deviceBuild = "";
                            try
                            {
                                string familyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                                ulong v = ulong.Parse(familyVersion);
                                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                                ulong v4 = (v & 0x000000000000FFFFL);
                                string deviceVersion = $"{v1}.{v2}.{v3}.{v4}";
                                deviceBuild = $"\nDevice: {deviceVersion}";
                            }
                            catch (Exception ex)
                            {

                            }
                            SelectedFile.URIDownloadAuthor = $"Package: {packageName}\nPublisher: {packagePublisher}\nMBuild: {MinBuild}{deviceBuild}";
                            DownloadReport = downloadReport;

                            //Extract Icon
                            if (packageLogo != null && packageLogo.Length > 0)
                            {
                                try
                                {
                                    var packageLogoResolved = packageLogo.Replace("\\", "/");
                                    var logoExt = Path.GetExtension(packageLogoResolved);

                                    //We have to remove the extension and test with 'StartsWith'
                                    //Logo name will contains at the end .scale-100 ..etc
                                    var logoName = packageLogoResolved.Replace(logoExt, "");
                                    var packageParts = package.GetParts();
                                    foreach (var packagePart in packageParts)
                                    {
                                        if (packagePart.Uri.ToString().StartsWith($"/{logoName}"))
                                        {
                                            var logoPartStream = packagePart.GetStream();
                                            var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetFileName(packageLogo), CreationCollisionOption.GenerateUniqueName);
                                            var readStream = logoPartStream.AsInputStream().AsStreamForRead();
                                            byte[] buffer = new byte[readStream.Length];
                                            await readStream.ReadAsync(buffer, 0, buffer.Length);
                                            await FileIO.WriteBufferAsync(tempFile, CryptographicBuffer.CreateFromByteArray(buffer));
                                            FileIcon = tempFile.Path;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            try
                            {
                                package.Close();
                            }
                            catch (Exception ex) { }
                        }

                        try
                        {
                            if (packageStream != null)
                            {
                                packageStream.Dispose();
                            }
                            if (stream != null)
                            {
                                stream.Dispose();
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
            });
        }

        public Visibility DoneVisible = Visibility.Collapsed;
        public Visibility isDoneVisible
        {
            get
            {
                return DoneVisible;
            }
            set
            {
                DoneVisible = value;
                RaisePropertyChanged2(nameof(isDoneVisible), true);
                if (value == Visibility.Visible)
                {
                    updateMonitorState(true);
                }
            }
        }

        public string getLocalString(string name)
        {
            var test = Helpers.getLocalString(name);
            if (test.Length == 0)
            {
                return name;
            }
            return test;
        }

        public string DoneText = "Downloaded";
        public string isDoneText
        {
            get
            {
                return DoneText;
            }
            set
            {
                DoneText = getLocalString(value);
                RaisePropertyChanged2(nameof(isDoneText), true);
            }
        }
        public Visibility FailedVisible = Visibility.Collapsed;
        public Visibility isFailedVisible
        {
            get
            {
                return FailedVisible;
            }
            set
            {
                FailedVisible = value;
                if (value == Visibility.Visible)
                {
                    isCancelingVisible = Visibility.Collapsed;
                    RaisePropertyChanged2(nameof(isCancelingVisible), true);
                    isCanceledVisible = Visibility.Collapsed;
                    RaisePropertyChanged2(nameof(isCanceledVisible), true);
                }
                RaisePropertyChanged2(nameof(isFailedVisible), true);
                if (value == Visibility.Visible)
                {
                    updateMonitorState(true, true);
                }
            }
        }
        public Visibility ProgressVisible = Visibility.Collapsed;
        public Visibility isProgressVisible
        {
            get
            {
                return ProgressVisible;
            }
            set
            {
                ProgressVisible = value;
                RaisePropertyChanged2(nameof(isProgressVisible), true);
            }
        }
        public Visibility CanceledVisible = Visibility.Collapsed;
        public Visibility isCanceledVisible
        {
            get
            {
                return CanceledVisible;
            }
            set
            {
                CanceledVisible = value;
                if (value == Visibility.Visible)
                {
                    isFailedVisible = Visibility.Collapsed;
                    RaisePropertyChanged2(nameof(isFailedVisible), true);
                }
                RaisePropertyChanged2(nameof(isCanceledVisible), true);
                if (value == Visibility.Visible)
                {
                    updateMonitorState(true);
                }
            }
        }
        public Visibility CancelingVisible = Visibility.Collapsed;
        public Visibility isCancelingVisible
        {
            get
            {
                return CancelingVisible;
            }
            set
            {
                CancelingVisible = value;
                RaisePropertyChanged2(nameof(isCancelingVisible), true);
                if (value == Visibility.Visible)
                {
                    updateMonitorState(true, true);
                }
            }

        }

        public void RaisePropertyChanged2(string name, bool updateRequired = false)
        {
            if (Helpers.DownloadsListVisible || ForceRaise || updateRequired)
            {
                RaisePropertyChanged(name);
            }
        }
        public EventHandler DownloadErrorCatched;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool noClean;
        public bool NoClean
        {
            get
            {
                return ScriptInstaller.NoClean;
            }
            set
            {
                ScriptInstaller.NoClean = value;
            }
        }

        public string UniqID = "";
        public QueueItem(BrowseListModel selectedFile, StorageFolder storageFolder, bool FolderDownload = false, bool isFirst = false)
        {
            SelectedFile = selectedFile;
            FileName = SelectedFile.fileName;
            UpdateFile = SelectedFile.UpdateFile;
            UpdateFileName = SelectedFile.UpdateFileName;
            UpdateFileVersion = SelectedFile.UpdateFileVersion;
            DownloadReport = "Queued (starting soon)..";
            try
            {
                UniqID = Helpers.sha256_hash(DateTime.Now.Ticks.ToString());
                if (UniqID.Length > 0)
                {
                    Helpers.DownloadsExecptions.Add(UniqID, "");
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                Installer.UpdateFile = UpdateFile;
            }
            catch (Exception ex)
            {

            }
            if (ScriptInstaller != null)
            {
                ScriptID = $"{ScriptInstaller.ScriptID}|{ScriptInstaller.ScriptID}";
            }
            try
            {
                FileName = FileName.Replace("?", "_");
            }
            catch (Exception e)
            {

            }
            ScriptInstaller.FileIcon = SelectedFile.fileIcon;
            FileType = SelectedFile.fileType;
            if (SelectedFile.OriginalFileSize.Length > 0)
            {
                FileSize = SelectedFile.OriginalFileSize;
            }
            else
            {
                FileSize = SelectedFile.fileSize;
            }
            FileSizeLong = SelectedFile.fileSizeLong;
            if (storageFolder != null)
            {
                StorageFolder = storageFolder;
            }
            isFolderDownload = FolderDownload;
            isFirstDownload = isFirst;
            DownloadErrorCatched += DownloadErrorCatchedHandler;
            UnzipComplete += UnzipCompleteEvent;
            Installer.DownloadsFolder = StorageFolder;
            XAPInstaller.DownloadsFolder = StorageFolder;
        }


        public void ReDownloadRequest(bool initialRequeut = false)
        {
            isProgressVisible = Visibility.Visible;
            isFailedVisible = Visibility.Collapsed;
            isCanceledVisible = Visibility.Collapsed;
            isCancelingVisible = Visibility.Collapsed;
            CurrentProgress = 0;
            isDownloadCanceled = false;
            isDownloadDone = false;
            isDownloadFailed = false;
            isStarted = false;
            isDownloading = false;
            isCancelingRequestInProgress = false;
            cancellationTokenSource = new CancellationTokenSource();
            if (!initialRequeut)
            {
                isRetry = true;
                DownloadReport = $"{FileSize} / Retrying..";
            }
        }

        public Stream SourceStream;
        bool isRetry = false;

        public async Task StartDownload()
        {
            try
            {
                ReDownloadRequest(true);
                bool streamCanSeek = false;
                ForceRaise = false;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     try
                     {
                         double timeStart = 0;
                         double timeNow = 0;
                         double downloadedPart = 0;
                         double totalSize = 0;
                         double averageSpeed = 0;
                         double timeLeft = 0;
                         IProgress<double> progress = new Progress<double>(value =>
                         {
                             if (value == 100)
                             {
                                 ForceRaise = true;
                                 if (EstimatedTime.Length > 0)
                                 {
                                     EstimatedTime = "";
                                 }
                             }
                             else
                             {
                                 try
                                 {

                                     if (SelectedFile.fileSizeLong > 0)
                                     {
                                         long SizeDownloaded = (long)Math.Round(SelectedFile.fileSizeLong * value / 100);
                                         DownloadReport = $"{Math.Round(value)}% / {SizeDownloaded.ToFileSize()} of {FileSize}";


                                         try
                                         {
                                             if (timeStart == 0)
                                             {
                                                 try
                                                 {
                                                     timeStart = DateTime.Now.Ticks;
                                                     totalSize = SelectedFile.fileSizeLong;
                                                 }
                                                 catch (Exception e)
                                                 {

                                                 }
                                             }
                                             try
                                             {
                                                 timeNow = DateTime.Now.Ticks;
                                                 downloadedPart = SizeDownloaded;
                                             }
                                             catch (Exception e)
                                             {

                                             }
                                             if (timeStart > 0)
                                             {
                                                 averageSpeed = downloadedPart / (timeNow - timeStart);
                                                 timeLeft = totalSize / averageSpeed - (timeNow - timeStart);
                                                 if (timeLeft > 0)
                                                 {
                                                     TimeSpan timeSpan = new TimeSpan((long)timeLeft);
                                                     var hours = timeSpan.Hours;
                                                     var minutes = timeSpan.Minutes;
                                                     var seconds = timeSpan.Seconds;
                                                     var averageSpeedString = "~";
                                                     try
                                                     {
                                                         averageSpeedString = ((long)Math.Round(10000000.0 * averageSpeed)).ToFileSize();
                                                     }
                                                     catch (Exception e)
                                                     {

                                                     }
                                                     if (hours > 0)
                                                     {
                                                         EstimatedTime = $"{hours} H, {minutes} M, {seconds} S ({averageSpeedString}/s)";
                                                     }
                                                     else if (minutes > 0)
                                                     {
                                                         EstimatedTime = $"{minutes} Min., {seconds} Sec. ({averageSpeedString}/s)";
                                                     }
                                                     else
                                                     {
                                                         EstimatedTime = $"{seconds} Sec. ({averageSpeedString}/s)";
                                                     }
                                                 }
                                                 else
                                                 {
                                                     if (EstimatedTime.Length > 0)
                                                     {
                                                         EstimatedTime = "";
                                                     }
                                                 }

                                             }
                                             else
                                             {
                                                 if (EstimatedTime.Length > 0)
                                                 {
                                                     EstimatedTime = "";
                                                 }
                                             }

                                         }
                                         catch (Exception e)
                                         {
                                             if (EstimatedTime.Length > 0)
                                             {
                                                 Helpers.Logger(e);
                                                 EstimatedTime = "";
                                             }
                                         }

                                     }
                                     else
                                     {
                                         var valueLong = (long)value;
                                         DownloadReport = $"Downloading {valueLong.ToFileSize()}";
                                         TotalDownloadedForDirectFile = valueLong;
                                         if (EstimatedTime.Length > 0)
                                         {
                                             EstimatedTime = "";
                                         }
                                     }

                                 }
                                 catch (Exception e)
                                 {

                                 }
                                 isDownloading = true;
                             }
                             if (SelectedFile.fileSizeLong > 0)
                             {
                                 CurrentProgress = value;
                             }
                             else
                             {
                                 CurrentProgress = (value / 100000000) * 100;
                             }
                         });
                         try
                         {
                             await RequestSessionAsync(ExtendedExecutionReason.Unspecified, SessionRevoked, $"{FileName} Download Session");
                         }
                         catch (Exception e)
                         {

                         }

                         isStarted = true;
                         DownloadReport = "Getting file info..";
                         if (!DirectDownload)
                         {
                             if (SelectedFile.megaFileNode.LocalRepo)
                             {
                                 DownloadReport = "Preparing file..";
                                 var fetchedFiled = await LocalToData.GetFileForToken(SelectedFile.megaFileNode, cancellationTokenSource);
                                 if (fetchedFiled != null)
                                 {
                                     testFile = fetchedFiled;
                                     isDownloadDone = true;
                                     if (Helpers.FinishHandler != null)
                                     {
                                         //Helpers.FinishHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload));
                                         ClearSession();
                                     }
                                     LocalFileDone();
                                 }
                                 else
                                 {
                                     isDownloadFailed = true;
                                     if (Helpers.FailedHandler != null)
                                     {
                                         //Helpers.FailedHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload));
                                         ClearSession();
                                     }
                                     isFailedVisible = Visibility.Visible;
                                     isCancelingVisible = Visibility.Collapsed;
                                     isProgressVisible = Visibility.Collapsed;
                                     DownloadReport = $"{FileSize} / Failed";
                                     EstimatedTime = "";
                                 }
                                 return;
                             }
                             else
                             if (SelectedFile.megaFileNode.DirectRepo)
                             {
                                 DownloadReport = "Getting file info..";
                                 await SelectedFile.megaFileNode.ReloadResponse(SelectedFile.megaFileNode.DirectRepoLink, cancellationTokenSource.Token, new HttpCredentialsHeaderValue("Bearer", SelectedFile.megaFileNode.AuthKey));
                                 //await Task.Delay(500);
                                 var testStream = new StreamProgress(SelectedFile, progress, cancellationTokenSource.Token);
                                 DownloadReport = "Prepare to download..";
                                 SourceStream = await testStream.FetchStream();
                                 //await Task.Delay(1000);
                                 if (SourceStream.CanSeek)
                                 {
                                     streamCanSeek = true;
                                 }
                             }
                             else
                             {
                                 var FileNode = await Helpers.GetFileNode(SelectedFile);
                                 if (FileNode == null)
                                 {
                                     ForceCancelDowload();
                                     return;
                                 }
                                 SourceStream = await Helpers.MegaClient.DownloadAsync(FileNode, progress, cancellationTokenSource.Token, false, UniqID);
                                 if (SourceStream == null)
                                 {
                                     Helpers.Logger($"SourceStream is null");
                                 }
                             }
                         }
                         else
                         {
                             try
                             {

                                 if (SelectedFile.megaFileNode.responseMessage != null)
                                 {
                                     try
                                     {
                                         if (isRetry)
                                         {
                                             isRetry = false;
                                             DownloadReport = "Getting file info..";
                                             await SelectedFile.megaFileNode.ReloadResponse(DirectURL, cancellationTokenSource.Token);
                                         }
                                     }
                                     catch (Exception e)
                                     {

                                     }
                                     //await Task.Delay(500);
                                     var testStream = new StreamProgress(SelectedFile, progress, cancellationTokenSource.Token);
                                     DownloadReport = "Prepare to download..";
                                     SourceStream = await testStream.FetchStream();
                                     //await Task.Delay(1000);
                                     if (SourceStream.CanSeek)
                                     {
                                         streamCanSeek = true;
                                     }
                                 }
                                 else if (SelectedFile.megaFileNode.DirectStream != null)
                                 {
                                     //cancellationTokenSource = new CancellationTokenSource();
                                     if (isRetry)
                                     {
                                         isRetry = false;
                                         DownloadReport = "Getting file info..";
                                         await SelectedFile.megaFileNode.ReloadDirectStream(DirectURL, cancellationTokenSource.Token, progress);
                                     }
                                     await Task.Delay(500);
                                     var testStream = new StreamProgress(SelectedFile, progress, cancellationTokenSource.Token);
                                     DownloadReport = "Prepare to download..";
                                     SourceStream = await testStream.FetchStream();
                                     await Task.Delay(1000);
                                     try
                                     {
                                         if (SourceStream.CanSeek)
                                         {
                                             streamCanSeek = true;
                                         }
                                     }
                                     catch (Exception e)
                                     {

                                     }
                                 }
                                 else
                                 {
                                     DownloadReport = "Preparing for download..";
                                     SourceStream = await GetStream(DirectURL);
                                     if (SourceStream == null)
                                     {
                                         ForceCancelDowload("Stream is null / empty");
                                         return;
                                     }
                                     try
                                     {
                                         if (SourceStream.Length > 0)
                                         {
                                             FileSize = SourceStream.Length.ToFileSize();
                                             FileSizeLong = SourceStream.Length;
                                         }
                                     }
                                     catch (Exception ee)
                                     {

                                     }
                                     try
                                     {
                                         SourceStream = new StreamBlind(SourceStream, progress, cancellationTokenSource.Token);
                                     }
                                     catch (Exception e)
                                     {

                                     }
                                 }
                             }
                             catch (Exception e)
                             {
                                 ForceCancelDowload(e.Message);
                                 return;
                             }
                         }

                         if (SupportBackground)
                         {
                             try
                             {
                                 if (!FileType.Contains("[B]"))
                                 {
                                     FileType = $"{FileType} [B]";
                                 }
                             }
                             catch (Exception ee)
                             {

                             }
                         }
                         else
                         {
                             FileType = FileType.Replace(" [B]", "");
                         }


                         bool isSkipped = false;
                         StorageFile targetFile = null;
                         try
                         {
                             targetFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                         }
                         catch
                         {

                         }
                         if (targetFile != null)
                         {
                             try
                             {
                                 string DialogTitle = "File Actions";
                                 string DialogMessage = $"File already exists, overwrite?\nFile: {FileName}";
                                 string[] DialogButtons = new string[] { $"Yes", "No" };
                                 int[] DialogButtonsIds = new int[] { 2, 3 };
                                 var ReplacePromptDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                                 Views.DialogResultCustom dialogResultCustom = DialogResultCustom.Nothing;
                                 if (isScriptUpdate)
                                 {
                                     dialogResultCustom = DialogResultCustom.Yes;
                                 }
                                 var ReplaceResult = await ReplacePromptDialog.ShowAsync2(dialogResultCustom);
                                 if (Helpers.DialogResultCheck(ReplacePromptDialog, 2))
                                 {
                                     await targetFile.DeleteAsync();
                                 }
                                 else
                                 {
                                     isSkipped = true;
                                 }
                             }
                             catch (Exception e)
                             {
                                 isSkipped = true;
                                 ForceCancelDowload(e.Message, isSkipped);
                                 return;
                             }
                         }
                         if (SourceStream != null && !isSkipped)
                         {
                             DownloadReport = "Downloading..";
                             chunks = new Chunks(SourceStream, StorageFolder, FileName, FileSizeLong, ref DownloadErrorCatched, streamCanSeek);
                             callDownloadMonitorTimer(true);
                         }
                         else
                         {
                             ForceCancelDowload("", isSkipped);
                             return;
                         }
                     }
                     catch (Exception e)
                     {
                         ForceCancelDowload(e.Message);
                         return;
                     }
                 });
            }
            catch (Exception e)
            {
                ForceCancelDowload(e.Message);
            }
        }
        protected async Task<Stream> GetStream(string fileLink)
        {
            var webClient = new WebClient();
            var response = webClient.GetRequestRaw(new Uri(fileLink));
            return response;
        }

        private async void FilesCombine()
        {
            try
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {

                        callDownloadMonitorTimer();
                        DownloadReport = "Resolving files..";
                        bool NeedCombine = chunks.MaxChunks > 1;

                        StorageFile testFile = null;
                        if (NeedCombine)
                        {
                            testFile = await StorageFolder.CreateFileAsync(FileName);
                            using (var targetStream = await testFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                var output = targetStream.AsStreamForWrite();
                                foreach (var ChunkItem in chunks.ChunkItems)
                                {
                                    try
                                    {
                                        var ChunkFile = ChunkItem.ChunkFile;
                                        using (var sourceStream = await ChunkFile.OpenAsync(FileAccessMode.Read))
                                        {
                                            var source = sourceStream.AsStreamForRead();
                                            await source.CopyToAsync(output);
                                            source.Dispose();
                                        }
                                        await ChunkFile.DeleteAsync();
                                    }
                                    catch (Exception e)
                                    {
                                        isDownloadFailed = true;
                                        _ = Helpers.ShowCatchedErrorAsync(e);
                                        if (Helpers.FailedHandler != null)
                                        {
                                            Helpers.FailedHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                            ClearSession();
                                        }
                                        break;
                                    }
                                }
                                output.Dispose();
                            }
                            if (!isDownloadFailed)
                            {
                                isDownloadDone = true;
                                if (Helpers.FinishHandler != null)
                                {
                                    if (!isScriptUpdate)
                                    {
                                        Helpers.FinishHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                    }
                                    ClearSession();
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                            }
                            catch
                            {

                            }
                            if (testFile != null)
                            {
                                var basicProperties = await testFile.GetBasicPropertiesAsync();
                                var fileSize = (long)basicProperties.Size;
                                if (fileSize < SelectedFile.fileSizeLong)
                                {
                                    isDownloadFailed = true;
                                    if (Helpers.FailedHandler != null)
                                    {
                                        Helpers.FailedHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                        ClearSession();
                                    }
                                }
                                else
                                {
                                    isDownloadDone = true;
                                    if (Helpers.FinishHandler != null)
                                    {
                                        if (!isScriptUpdate)
                                        {
                                            Helpers.FinishHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                        }
                                        ClearSession();
                                    }

                                }
                            }
                            else
                            {
                                isDownloadFailed = true;
                                if (Helpers.FailedHandler != null)
                                {
                                    Helpers.FailedHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                    ClearSession();
                                }
                            }
                        }

                        if (isDownloadDone)
                        {
                            isDoneVisible = Visibility.Visible;
                            isProgressVisible = Visibility.Collapsed;
                            isCancelingVisible = Visibility.Collapsed;
                            updateFileData(testFile);
                            if (SelectedFile.fileSizeLong > 0)
                            {
                                DownloadReport = $"{FileSize} / Done";
                            }
                            else
                            {
                                var size = TotalDownloadedForDirectFile.ToFileSize();
                                if (size.Equals("0.00 B"))
                                {
                                    size = "~ 1.00 - 100 K";
                                }
                                DownloadReport = $"{size} / Done";
                            }

                            DownloadFullLocation = testFile.Path;
                            var FileType = Path.GetExtension(FileName).ToLower();
                            if (FileType.ToLower().Equals(".wut") && AutoOpen)
                            {
                                if (testFile == null)
                                {
                                    try
                                    {
                                        testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                                    }
                                    catch
                                    {

                                    }
                                }
                                if (testFile != null)
                                {
                                    Helpers.NewRepoRequest = testFile;
                                    if (Helpers.AppAlreadyStarted)
                                    {
                                        if (Helpers.ReloadNewRepo != null)
                                        {
                                            Helpers.ReloadNewRepo.Invoke(null, EventArgs.Empty);
                                        }
                                        else
                                        {
                                            OpenFile();
                                        }
                                    }
                                    else
                                    {
                                        OpenFile();
                                    }
                                }
                                else
                                {
                                    OpenFile();
                                }
                            }
                            else
                            if (!Helpers.isX64() || Helpers.CheckIsZip(FileName) || Helpers.CheckIsDesktopApp(FileName))
                            {
                                switch (FileType)
                                {
                                    case ".xap":
                                        await InstallApp();
                                        if (!devModeActiveDialog)
                                        {
                                            if (Helpers.XAPTelnetDefault)
                                            {
                                                if (Helpers.SDCardByDefault)
                                                {
                                                    Helpers.XAPInSDCardGlobal = true;
                                                }
                                                else if (Helpers.InternalByDefault)
                                                {
                                                    Helpers.XAPInSDCardGlobal = false;
                                                }
                                                InstallXAP(Helpers.LogsFolder, true);
                                            }
                                        }
                                        break;

                                    case ".appx":
                                    case ".msix":
                                    case ".appxbundle":
                                    case ".msixbundle":
                                        if (Helpers.AutoAppx)
                                        {
                                            await InstallApp();
                                        }
                                        break;

                                    case ".zip":
                                    case ".rar":
                                    case ".7z":
                                    case ".tar":
                                    case ".gz":
                                        if (Helpers.AutoExtract || UpdateFile)
                                        {
                                            await InstallApp();
                                        }
                                        break;

                                    case ".wuts":
                                    case ".wutc":
                                    case ".wutz":
                                    case ".w10ms":
                                    case ".w10mc":
                                    case ".w10mz":
                                        if (Helpers.XAPTelnetDefault)
                                        {
                                            await InstallApp();
                                        }
                                        break;
                                }

                            }
                            else
                            {
                                Installer.isDoneVisible = Visibility.Visible;
                                try
                                {
                                    ClearSession();
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                        else if (isDownloadFailed)
                        {
                            isFailedVisible = Visibility.Visible;
                            isCancelingVisible = Visibility.Collapsed;
                            isProgressVisible = Visibility.Collapsed;
                            DownloadReport = $"{FileSize} / Failed";
                            EstimatedTime = "";
                        }
                    }
                    catch (Exception e)
                    {
                        //_ = Helpers.ShowCatchedErrorAsync(e);
                    }
                });
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }

        public bool localRepo = false;
        public bool LocalRepo
        {
            get
            {
                if (SelectedFile != null)
                {
                    return SelectedFile.LocalRepo || localRepo;
                }
                return localRepo;
            }
            set
            {
                localRepo = value;
            }
        }
        private async void LocalFileDone()
        {
            try
            {
                LocalRepo = true;
                isDoneVisible = Visibility.Visible;
                isProgressVisible = Visibility.Collapsed;
                isCancelingVisible = Visibility.Collapsed;
                isDoneText = "Queued";
                if (SelectedFile.fileSizeLong > 0)
                {
                    DownloadReport = $"{FileSize} / Done";
                }
                else
                {
                    var size = TotalDownloadedForDirectFile.ToFileSize();
                    if (size.Equals("0.00 B"))
                    {
                        size = "~ 1.00 - 100 K";
                    }
                    DownloadReport = $"{size} / Done";
                }

                DownloadFullLocation = testFile.Path;
                if (!Helpers.isX64() || Helpers.CheckIsZip(FileName) || Helpers.CheckIsDesktopApp(FileName))
                {
                    var FileType = Path.GetExtension(FileName).ToLower();
                    switch (FileType)
                    {
                        case ".xap":
                            await InstallApp();
                            if (!devModeActiveDialog)
                            {
                                if (Helpers.XAPTelnetDefault)
                                {
                                    if (Helpers.SDCardByDefault)
                                    {
                                        Helpers.XAPInSDCardGlobal = true;
                                    }
                                    else if (Helpers.InternalByDefault)
                                    {
                                        Helpers.XAPInSDCardGlobal = false;
                                    }
                                    InstallXAP(Helpers.LogsFolder, true);
                                }
                            }
                            break;

                        case ".appx":
                        case ".msix":
                        case ".appxbundle":
                        case ".msixbundle":
                            if (Helpers.AutoAppx)
                            {
                                await InstallApp();
                            }
                            break;

                        case ".zip":
                        case ".rar":
                        case ".7z":
                        case ".tar":
                        case ".gz":
                            if (Helpers.AutoExtract || UpdateFile)
                            {
                                await InstallApp();
                            }
                            break;

                        case ".wuts":
                        case ".wutc":
                        case ".wutz":
                        case ".w10ms":
                        case ".w10mc":
                        case ".w10mz":
                            if (Helpers.XAPTelnetDefault)
                            {
                                await InstallApp();
                            }
                            break;
                    }

                }
                else
                {
                    Installer.isDoneVisible = Visibility.Visible;
                    try
                    {
                        ClearSession();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
            }
        }

        public EventHandler UnzipComplete = null;
        public void UnzipCompleteEvent(object sender, EventArgs args)
        {
            try
            {
                if (ScriptID.Length > 0)
                {
                    foreach (var dItem in Helpers.DownloadsMonitor)
                    {
                        if (dItem.FileID.Equals(downloadID) && dItem.ScriptID.Equals(ScriptID))
                        {
                            dItem.isExtracted = true;
                            break;
                        }
                    }
                }
                if (sender != null)
                {
                    var TargetFolder = (StorageFolder)sender;
                    if (TargetFolder != null)
                    {
                        if (UpdateFile)
                        {
                            OpenFile(true, TargetFolder, UpdateFileName, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public async Task InstallApp()
        {
            try
            {
                if (testFile == null)
                {
                    try
                    {
                        testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                    }
                    catch
                    {

                    }
                }
                if (testFile != null)
                {
                    var basicProperties = await testFile.GetBasicPropertiesAsync();
                    var fileSize = (long)basicProperties.Size;
                    if (fileSize < SelectedFile.fileSizeLong)
                    {
                        string DialogTitle = "Install App";
                        string DialogMessage = $"The file didn't pass the verification!, try to install manually";
                        string[] DialogButtons = new string[] { "Close" };
                        var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var QuestionResult = QuestionDialog.ShowAsync2();
                    }
                    else
                    {
                        try
                        {
                            Installer.UpdateFile = UpdateFile;
                        }
                        catch (Exception ex)
                        {

                        }
                        await Installer.InstallApp(testFile, UnzipComplete, ScriptID, downloadID, null, devModeActiveDialog);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public async void InstallCert(bool ShowError = false)
        {
            try
            {

                if (testFile == null)
                {
                    try
                    {
                        testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                    }
                    catch
                    {

                    }
                }
                if (testFile != null)
                {
                    var basicProperties = await testFile.GetBasicPropertiesAsync();
                    var fileSize = (long)basicProperties.Size;
                    if (fileSize < SelectedFile.fileSizeLong)
                    {
                        string DialogTitle = "Install App";
                        string DialogMessage = $"The certificate didn't pass the verification!, try to install manually";
                        string[] DialogButtons = new string[] { "Close" };
                        var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var QuestionResult = QuestionDialog.ShowAsync2();
                    }
                    else
                    {
                        DownloadReport = "Installing...";
                        var InstallState = await Helpers.SolveCertificateIssue(testFile, ShowError, true);
                        if (InstallState)
                        {
                            Installer.ShowInstalled();
                            DownloadReport = "Done";
                        }
                        else
                        {
                            DownloadReport = "Unable to install";
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        bool InternalXAPInstallationInProgress = false;
        public async void InstallXAP(StorageFolder storageFolder, bool GroupInstall = false)
        {
            try
            {
                if (InternalXAPInstallationInProgress)
                {
                    return;
                }
                InternalXAPInstallationInProgress = true;
                if (testFile == null)
                {
                    try
                    {
                        testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                    }
                    catch
                    {

                    }
                }
                if (testFile != null)
                {
                    XAPInstaller.SuperWriteLine("");
                    var basicProperties = await testFile.GetBasicPropertiesAsync();
                    var fileSize = (long)basicProperties.Size;
                    if (fileSize < SelectedFile.fileSizeLong)
                    {
                        string DialogTitle = "Install App";
                        string DialogMessage = $"The xap didn't pass the verification!, try to install manually";
                        string[] DialogButtons = new string[] { "Close" };
                        var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var QuestionResult = QuestionDialog.ShowAsync2();
                    }
                    else
                    {
                        if (GroupInstall)
                        {
                            XAPInstaller.XAPPath = testFile.Path;
                            XAPInstaller.XAPFile = testFile;
                            XAPInstaller.XAPSize = fileSize;
                            XAPInstaller.InstallInSDCard = Helpers.XAPInSDCardGlobal;
                            StorageFile LogFile = await GetLogFile(storageFolder);
                            await XAPInstaller.InstallXAP(LogFile, Installer);
                        }
                        else
                        {
                            //Installation Way
                            string DialogTitle3 = "Install Options";
                            string DialogMessage3 = $"How do you want to install\n{testFile.Name}?\n\nRequirements: \n-CMD configured via CMD Injector app";
                            string[] DialogButtons3 = new string[] { "Install", "CMDI", "Cancel" };
                            var QuestionDialog3 = Helpers.CreateDialog(DialogTitle3, DialogMessage3, DialogButtons3);
                            var QuestionResult3 = DialogResultCustom.Nothing;
                            if (Helpers.XAPCMDDefault)
                            {
                                QuestionResult3 = (DialogResultCustom)await QuestionDialog3.ShowAsync2(DialogResultCustom.No);
                            }
                            else
                            {
                                QuestionResult3 = (DialogResultCustom)await QuestionDialog3.ShowAsync2();
                            }
                            if (Helpers.DialogResultCheck(QuestionDialog3, 1))
                            {
                                try
                                {
                                    bool CMDIState = await XAPInstaller.SendToCMDInjector(testFile);
                                    if (!CMDIState)
                                    {
                                        var LocationCommand = "-di";
                                        if (Helpers.SDCardByDefault)
                                        {
                                            LocationCommand = "-dis";
                                        }
                                        else if (Helpers.InternalByDefault)
                                        {
                                            LocationCommand = "-di";
                                        }
                                        else
                                        {
                                            string DialogTitle4 = "Install Location";
                                            string DialogMessage4 = $"Where do you want to install\n{testFile.Name}?";
                                            string[] DialogButtons4 = new string[] { "SD Card", "Internal", "Cancel" };
                                            var QuestionDialog4 = Helpers.CreateDialog(DialogTitle4, DialogMessage4, DialogButtons4);
                                            var QuestionResult4 = await QuestionDialog4.ShowAsync2();

                                            if (Helpers.DialogResultCheck(QuestionDialog4, 2))
                                            {
                                                LocationCommand = "-dis";
                                            }
                                            else if (Helpers.DialogResultCheck(QuestionDialog4, 3))
                                            {
                                                InternalXAPInstallationInProgress = false;
                                                try
                                                {
                                                    ClearSession();
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                                return;
                                            }

                                        }
                                        string InstallerPath = XAPInstaller.GetInstallationFileByXAP(testFile);

                                        string command = $"\"{InstallerPath}\" {LocationCommand}";
                                        DataPackage dataPackage = new DataPackage();
                                        dataPackage.RequestedOperation = DataPackageOperation.Copy;
                                        dataPackage.SetText(command);
                                        Clipboard.SetContent(dataPackage);

                                        //Helpers.ShowToastNotification("Install Command", $"Command for {testFile.Name} copied to clipboard");
                                        Helpers.PlayNotificationSoundDirect("alert.mp3");
                                        LocalNotificationData localNotificationData = new LocalNotificationData();
                                        localNotificationData.icon = SegoeMDL2Assets.CommandPrompt;
                                        localNotificationData.type = Colors.DodgerBlue;
                                        localNotificationData.message = $"{testFile.Name}: Installation command copied to clipboard";
                                        localNotificationData.time = 3;
                                        Helpers.pushLocalNotification(null, localNotificationData);

                                        /*string DialogTitle = "Install Command";
                                        string DialogMessage = $"Command sent to clipboard, paste this command in any Telnet or CMD client and enjoy";
                                        string[] DialogButtons = new string[] { "Close" };
                                        var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                                        var QuestionResult = QuestionDialog.ShowAsync2();*/
                                    }
                                }
                                catch (Exception ee)
                                {

                                }
                            }
                            else if (Helpers.DialogResultCheck(QuestionDialog3, 2))
                            {
                                //Try to use XAP Installer
                                try
                                {
                                    XAPInstaller.XAPPath = testFile.Path;
                                    XAPInstaller.XAPFile = testFile;
                                    XAPInstaller.XAPSize = fileSize;
                                    if (Helpers.SDCardByDefault)
                                    {
                                        XAPInstaller.InstallInSDCard = true;
                                        StorageFile LogFile = await GetLogFile(storageFolder);
                                        await XAPInstaller.InstallXAP(LogFile, Installer);
                                    }
                                    else if (Helpers.InternalByDefault)
                                    {
                                        XAPInstaller.InstallInSDCard = false;
                                        StorageFile LogFile = await GetLogFile(storageFolder);
                                        await XAPInstaller.InstallXAP(LogFile, Installer);
                                    }
                                    else
                                    {
                                        string DialogTitle2 = "Install Location";
                                        string DialogMessage2 = $"Where do you want to install\n{testFile.Name}?\nYou can set default storage from settings";
                                        string[] DialogButtons2 = new string[] { "SD Card", "Internal", "Cancel" };
                                        var QuestionDialog2 = Helpers.CreateDialog(DialogTitle2, DialogMessage2, DialogButtons2);
                                        var QuestionResult2 = await QuestionDialog2.ShowAsync2();
                                        if (Helpers.DialogResultCheck(QuestionDialog2, 2))
                                        {
                                            XAPInstaller.InstallInSDCard = true;
                                        }


                                        if (!Helpers.DialogResultCheck(QuestionDialog2, 3))
                                        {
                                            StorageFile LogFile = await GetLogFile(storageFolder);
                                            await XAPInstaller.InstallXAP(LogFile, Installer);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                }
                InternalXAPInstallationInProgress = false;
            }
            catch (Exception e)
            {
                InternalXAPInstallationInProgress = false;
            }
        }
        public async void OpenFile(bool InternalFile = false, StorageFolder TargetFolder = null, string FileNameEx = "", bool CloseAfterOpen = false)
        {
            try
            {
                if (InternalFile)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        try
                        {

                            if (testFile != null)
                            {
                                if (TargetFolder != null && FileNameEx.Length > 0)
                                {
                                    StorageFile TargetInternalFile = null;
                                    try
                                    {
                                        TargetInternalFile = (StorageFile)await TargetFolder.TryGetItemAsync(FileNameEx);
                                    }
                                    catch
                                    {

                                    }
                                    if (TargetInternalFile == null)
                                    {
                                        StorageFolder InnerFolder = null;
                                        try
                                        {
                                            InnerFolder = (StorageFolder)await TargetFolder.TryGetItemAsync(TargetFolder.Name);
                                        }
                                        catch
                                        {

                                        }
                                        if (InnerFolder != null)
                                        {
                                            try
                                            {
                                                TargetInternalFile = (StorageFile)await InnerFolder.TryGetItemAsync(FileNameEx);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    try
                                    {
                                        if (TargetInternalFile == null)
                                        {
                                            FileNameEx = FileNameEx.Replace(".appx", ".msix");
                                            try
                                            {
                                                TargetInternalFile = (StorageFile)await TargetFolder.TryGetItemAsync(FileNameEx);
                                            }
                                            catch
                                            {

                                            }
                                            if (TargetInternalFile == null)
                                            {
                                                StorageFolder InnerFolder = null;
                                                try
                                                {
                                                    InnerFolder = (StorageFolder)await TargetFolder.TryGetItemAsync(TargetFolder.Name);
                                                }
                                                catch
                                                {

                                                }
                                                if (InnerFolder != null)
                                                {
                                                    try
                                                    {
                                                        TargetInternalFile = (StorageFile)await InnerFolder.TryGetItemAsync(FileNameEx);
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Helpers.Logger(ex);
                                    }
                                    if (TargetInternalFile != null)
                                    {
                                        Helpers.Logger($"Opening Update at: {TargetInternalFile.Path}");
                                        var options = new Windows.System.LauncherOptions();
                                        options.PreferredApplicationPackageFamilyName = "Microsoft.DesktopAppInstaller_8wekyb3d8bbwe";
                                        options.PreferredApplicationDisplayName = "App Installer";

                                        //var uriBing = new Uri($"ms-appinstaller:?source={TargetInternalFile.Path}");
                                        var state = await Windows.System.Launcher.LaunchFileAsync(TargetInternalFile, options);
                                        // Launch the URI
                                        //var state = await Windows.System.Launcher.LaunchUriAsync(uriBing);
                                        Helpers.Logger("App Installer State ? " + (state ? "Started" : "Failed"));
                                        if (state)
                                        {
                                            if (CloseAfterOpen)
                                            {
                                                //Helpers.Logger("Closing the app");
                                                //Application.Current.Exit();
                                            }
                                        }
                                        else
                                        {
                                            var successfolder = await Windows.System.Launcher.LaunchFolderAsync(TargetFolder);
                                            if (successfolder)
                                            {
                                                if (CloseAfterOpen)
                                                {
                                                    //Application.Current.Exit();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Helpers.Logger("Update file couldn't be found!");
                                        var successfolder = await Windows.System.Launcher.LaunchFolderAsync(TargetFolder);
                                        if (successfolder)
                                        {
                                            if (CloseAfterOpen)
                                            {
                                                //Application.Current.Exit();
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                var success = await Windows.System.Launcher.LaunchFolderAsync(TargetFolder);
                                if (success)
                                {
                                    if (CloseAfterOpen)
                                    {
                                        Application.Current.Exit();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Helpers.Logger(e);
                        }
                    });
                }
                else
                {
                    if (testFile == null)
                    {
                        try
                        {
                            testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                        }
                        catch
                        {

                        }
                    }
                    if (testFile != null)
                    {
                        var basicProperties = await testFile.GetBasicPropertiesAsync();
                        var fileSize = (long)basicProperties.Size;
                        if (fileSize < SelectedFile.fileSizeLong)
                        {
                            string DialogTitle = "Install App";
                            string DialogMessage = $"The file didn't pass the verification!";
                            string[] DialogButtons = new string[] { "Close" };
                            var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                            var QuestionResult = QuestionDialog.ShowAsync2();
                        }
                        else
                        {
                            if (testFile != null)
                            {
                                var success = await Windows.System.Launcher.LaunchFileAsync(testFile);

                                if (success)
                                {
                                    // File launched
                                }
                                else
                                {
                                    string DialogTitle = "Install File";
                                    string DialogMessage = $"Failed to open the file\nPlease go to downloads folder and install it manually";
                                    string[] DialogButtons = new string[] { "Close" };
                                    var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                                    var QuestionResult = QuestionDialog.ShowAsync2();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
            }
        }
        public async void OpenFolder()
        {
            try
            {
                if (testFile != null)
                {
                    if (testFile != null)
                    {
                        var FolderName = Path.GetFileNameWithoutExtension(testFile.Name);
                        StorageFolder TargetFolder = null;
                        try
                        {
                            TargetFolder = (StorageFolder)await StorageFolder.TryGetItemAsync(FolderName);
                        }
                        catch
                        {

                        }
                        if (TargetFolder == null)
                        {
                            try
                            {
                                TargetFolder = (StorageFolder)await Installer.ParentFolder.TryGetItemAsync(FolderName); ;
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        if (TargetFolder != null)
                        {
                            var success = await Windows.System.Launcher.LaunchFolderAsync(TargetFolder);
                            if (success)
                            {
                                // File launched
                            }
                            else
                            {
                                string DialogTitle = "Install File";
                                string DialogMessage = $"Failed to open the file\nPlease go to downloads folder and install it manually";
                                string[] DialogButtons = new string[] { "Close" };
                                var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                                var QuestionResult = QuestionDialog.ShowAsync2();
                            }
                        }
                        else
                        {
                            string DialogTitle2 = "Open Folder";
                            string DialogMessage2 = $"Unable to find the folder\n{FolderName}?\nYou can try to extract or open again";
                            string[] DialogButtons2 = new string[] { "Extract", "Open", "Cancel" };
                            var QuestionDialog2 = Helpers.CreateDialog(DialogTitle2, DialogMessage2, DialogButtons2);
                            var QuestionResult2 = await QuestionDialog2.ShowAsync2();
                            if (Helpers.DialogResultCheck(QuestionDialog2, 2))
                            {
                                await InstallApp();
                            }
                            else if (Helpers.DialogResultCheck(QuestionDialog2, 1))
                            {
                                OpenFolder();
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {

            }
        }


        public async void InstallScript(StorageFolder storageFolder)
        {
            try
            {
                if (testFile == null)
                {
                    testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                }
                StorageFile LogFile = await GetLogFile(storageFolder);
                await ScriptInstaller.RunScript(testFile, LogFile, Installer, XAPInstaller, StorageFolder);

            }
            catch (Exception e)
            {

            }
        }
        public void ChangeIcon(string newIcon)
        {
            FileIcon = newIcon;
            RaisePropertyChanged2(nameof(FileIcon), true);
        }

        private async Task<StorageFile> GetLogFile(StorageFolder storageFolder)
        {
            StorageFile LogFile = null;
            try
            {
                if (storageFolder != null && Helpers.TelnetLog)
                {
                    var fileName = DateTime.Now.ToString().Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace(" ", "_");
                    StorageFile testFile = null;
                    try
                    {
                        testFile = (StorageFile)await storageFolder.TryGetItemAsync($"{fileName}.txt");
                    }
                    catch
                    {

                    }
                    if (testFile != null)
                    {
                        await testFile.DeleteAsync();
                    }
                    LogFile = await storageFolder.CreateFileAsync($"{fileName}.txt");
                }
            }
            catch (Exception e)
            {

            }
            return LogFile;
        }
        public bool isCancelingRequestInProgress = false;
        public async void ForceCancelDowload(string CustomMessage = "", bool isSkipped = false, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                //Helpers.Logger($"{memberName}  |  {sourceLineNumber}  |  {sourceFilePath}");
                if (!isDownloadDone && !isDownloadFailed)
                {
                    isDownloadCanceled = true;
                    isFailedVisible = Visibility.Collapsed;
                    isProgressVisible = Visibility.Collapsed;
                    isCancelingVisible = Visibility.Collapsed;
                    isCanceledVisible = Visibility.Visible;
                    if (CustomMessage.Length > 0)
                    {
                        DownloadReport = $"{FileSize} / {getLocalString(CustomMessage)}";
                    }
                    else
                    {
                        DownloadReport = $"{FileSize} / Canceled";
                        EstimatedTime = "";
                    }
                    isCancelingRequestInProgress = false;
                    callDownloadMonitorTimer();
                    try
                    {
                        SourceStream.Dispose();
                    }
                    catch (Exception e)
                    {

                    }
                    try
                    {
                        StorageFile testFile = null;
                        try
                        {
                            testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                        }
                        catch
                        {

                        }
                        if (testFile != null)
                        {
                            if (Helpers.CanceledHandler != null)
                            {
                                if (isSkipped)
                                {
                                    Helpers.CanceledHandler.Invoke(null, null);
                                }
                                else
                                {
                                    Helpers.CanceledHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                }
                                ClearSession();
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
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }
        public void CancelDowload()
        {
            try
            {
                ForceRaise = true;
                if (!isDownloadDone && !isDownloadFailed)
                {
                    isCancelingRequestInProgress = true;
                    cancellationTokenSource.Cancel();
                    isCancelingVisible = Visibility.Visible;
                    DownloadReport = $"{FileSize} / Cancelling..";
                    if (!isDownloading)
                    {
                        isDownloadCanceled = true;
                        isFailedVisible = Visibility.Collapsed;
                        isProgressVisible = Visibility.Collapsed;
                        isCancelingVisible = Visibility.Collapsed;
                        isCanceledVisible = Visibility.Visible;
                        DownloadReport = $"{FileSize} / Canceled";
                        EstimatedTime = "";
                    }
                }
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }

        private async void DownloadErrorCatchedHandler(object sender, EventArgs eventArgs)
        {
            try
            {
                ForceRaise = true;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        callDownloadMonitorTimer();
                        try
                        {
                            SourceStream.Dispose();
                        }
                        catch (Exception ee)
                        {

                        }
                        var exceptionMessage = "...";
                        try
                        {
                            Exception exception = (Exception)sender;
                            exceptionMessage = exception.Message;
                        }
                        catch (Exception e)
                        {

                        }
                        StorageFile testFile = null;
                        try
                        {
                            testFile = (StorageFile)await StorageFolder.TryGetItemAsync(FileName);
                        }
                        catch
                        {

                        }
                        if (exceptionMessage.Equals("OperationCanceled"))
                        {
                            isDownloadCanceled = true;
                            isFailedVisible = Visibility.Collapsed;
                            isProgressVisible = Visibility.Collapsed;
                            isCancelingVisible = Visibility.Collapsed;
                            isCanceledVisible = Visibility.Visible;
                            DownloadReport = $"{FileSize} / Canceled";
                            EstimatedTime = "";
                            if (testFile != null)
                            {
                                if (Helpers.CanceledHandler != null)
                                {
                                    Helpers.CanceledHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                    ClearSession();
                                }
                            }
                        }
                        else
                        {
                            isFailedVisible = Visibility.Visible;
                            isProgressVisible = Visibility.Collapsed;
                            isCancelingVisible = Visibility.Collapsed;
                            try
                            {
                                if (UniqID.Length > 0 && Helpers.DownloadsExecptions[UniqID].Length > 0)
                                {
                                    DownloadReport = getLocalString(Helpers.DownloadsExecptions[UniqID]);
                                }
                                else
                                {
                                    DownloadReport = $"{FileSize} / Failed";
                                    if (!exceptionMessage.Contains("Object reference not"))
                                    {
                                        DownloadReport += $"\n{getLocalString(exceptionMessage)}";
                                    }
                                }
                            }
                            catch (Exception ee)
                            {
                                DownloadReport = $"{FileSize} / Failed";
                                if (!exceptionMessage.Contains("Object reference not"))
                                {
                                    DownloadReport += $"\n{getLocalString(exceptionMessage)}";
                                }
                            }
                            EstimatedTime = "";
                            isDownloadFailed = true;

                            if (testFile != null)
                            {
                                if (Helpers.FailedHandler != null)
                                {
                                    Helpers.FailedHandler.Invoke(null, new DownloadArgs(testFile, isFolderDownload, StorageFolder));
                                    ClearSession();
                                }
                            }
                        }
                        isDownloading = false;
                        isCancelingRequestInProgress = false;
                    }
                    catch (Exception e)
                    {
                        Helpers.Logger(e);
                    }
                });
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
            }
        }


        private ExtendedExecutionSession session = null;
        private int taskCount = 0;

        public bool IsRunning
        {
            get
            {
                if (session != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task RequestSessionAsync(ExtendedExecutionReason reason, TypedEventHandler<object, ExtendedExecutionRevokedEventArgs> revoked, String description)
        {
            try
            {
                if (!Helpers.BackgroundDownload)
                {
                    //return ExtendedExecutionResult.Denied;
                    return;
                }
                // The previous Extended Execution must be closed before a new one can be requested.       
                ClearSession();
                IsSessionStarted = true;
                var newSession = new ExtendedExecutionSession();
                newSession.Reason = reason;
                newSession.Description = description;
                newSession.Revoked += SessionRevoked;

                // Add a revoked handler provided by the app in order to clean up an operation that had to be halted prematurely
                if (revoked != null)
                {
                    newSession.Revoked += revoked;
                }

                ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

                switch (result)
                {
                    case ExtendedExecutionResult.Allowed:
                        session = newSession;
                        SupportBackground = true;
                        break;
                    default:
                    case ExtendedExecutionResult.Denied:
                        newSession.Dispose();
                        SupportBackground = false;
                        break;
                }
                //return result;
            }
            catch (Exception e)
            {
                //return new ExtendedExecutionResult();
            }
        }

        public void ClearSession()
        {
            try
            {
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }

                taskCount = 0;

            }
            catch (Exception e)
            {

            }
            IsSessionStarted = false;
        }
        public Deferral GetExecutionDeferral()
        {
            try
            {
                if (session == null)
                {
                    //Helpers.ShowErrorMessage(new InvalidOperationException("No extended execution session is active"));
                    Helpers.PlayNotificationSoundDirect("error.mp3");
                    LocalNotificationData localNotificationData = new LocalNotificationData();
                    localNotificationData.icon = SegoeMDL2Assets.Package;
                    localNotificationData.type = Colors.OrangeRed;
                    localNotificationData.time = 5;
                    localNotificationData.message = "No extended execution session is active";
                    Helpers.pushLocalNotification(null, localNotificationData);
                }

                taskCount++;
            }
            catch (Exception e)
            {

            }
            return new Deferral(OnTaskCompleted);
        }
        private void OnTaskCompleted()
        {
            try
            {
                if (taskCount > 0)
                {
                    taskCount--;
                }

                //If there are no more running tasks than end the extended lifetime by clearing the session
                if (taskCount == 0 && session != null)
                {
                    ClearSession();
                }
            }
            catch (Exception e)
            {

            }
        }
        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            try
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        switch (args.Reason)
                        {
                            case ExtendedExecutionRevokedReason.Resumed:
                                //Helpers.ShowToastNotification("Background Session", "Extended execution revoked due to returning to foreground.");
                                break;

                            case ExtendedExecutionRevokedReason.SystemPolicy:
                                //Helpers.ShowToastNotification("Background Session", "Extended execution revoked due to system policy.");
                                break;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                });
            }
            catch (Exception e)
            {

            }
            try
            {
                //The session has been prematurely revoked due to system constraints, ensure the session is disposed
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }

                taskCount = 0;

            }
            catch (Exception e)
            {

            }
            IsSessionStarted = false;
        }

    }



    public class DownloadArgs : EventArgs
    {
        public StorageFile DownloadedFile;
        public StorageFolder DownloadsFolder;
        public bool FolderDownload;
        public DownloadArgs(StorageFile downloadedFile, bool folderDownload, StorageFolder downloadsFolder)
        {
            this.DownloadedFile = downloadedFile;
            this.FolderDownload = folderDownload;
            this.DownloadsFolder = downloadsFolder;
        }
    }
    public class ChunkdArgs : EventArgs
    {
        public bool NeedCombine = false;
        public ChunkdArgs(bool needCombine)
        {
            this.NeedCombine = needCombine;
        }
    }
}
