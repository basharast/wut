/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Text.RegularExpressions;
using System.Threading;
using WinUniversalTool.DirectStorage;
using WinUniversalTool.WebViewer;

namespace WinUniversalTool.Models
{
    public class BrowseListModel : BindableBase
    {
        public string displayName
        {
            get
            {
                try
                {
                    if (Helpers.HideKnownExtensions && isFile)
                    {
                        try
                        {
                            return Path.GetFileNameWithoutExtension(fileName);
                        }
                        catch (Exception e)
                        {
                            return fileName;
                        }
                    }
                    else
                    {
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    return fileName;
                }
            }
        }
        public string filename;
        [JsonIgnore]
        public string filename_cache = null;
        [JsonIgnore]
        public string fileName
        {
            get
            {
                if(filename_cache != null)
                {
                    return filename_cache;
                }
                //filename_cache = AstifanHelper.Base64Decode(filename);
                filename_cache = filename;
                return filename_cache;
            }
            set
            {
                filename_cache = null;
                //filename = AstifanHelper.Base64Encode(value);
                filename = value;
            }
        }
        public string fileSize;
        public long fileSizeLong = 0;
        public string fileIcon2;
        public string fileIcon
        {
            get
            {
                return fileIcon2;
            }
            set
            {
                fileIcon2 = value;
                RaisePropertyChanged(nameof(fileIcon));
            }
        }
        public string fileType;
        public int fileTypeOrder = 0;
        public string fileid;
        [JsonIgnore]
        public string fileid_cache = null;
        [JsonIgnore]
        public string fileID
        {
            get
            {
                if (fileid_cache != null)
                {
                    return fileid_cache;
                }
                fileid_cache = fileid.ScrambleBack();
                return fileid_cache;
            }
            set
            {
                fileid_cache = null;
                fileid = MenuItem ? value : value.Scramble();
            }
        }
        public string parentid;
        [JsonIgnore]
        public string parentid_cache = null;
        [JsonIgnore]
        public string ParentID
        {
            get
            {
                if(parentid_cache != null)
                {
                    return parentid_cache;
                }
                parentid_cache = parentid.ScrambleBack();
                return parentid_cache;
            }
            set
            {
                parentid_cache = null;
                parentid = value.Scramble();
            }
        }
        public string SerializedFingerprint;
        public bool isRoot = false;
        public bool isFolder = false;
        public bool isFile = false;
        public bool ThumbLoaded = false;
        public MegaFolder Folder;
        public string filesnode;
        [JsonIgnore]
        public string filesnode_cache = null;
        [JsonIgnore]
        public string FilesNode
        {
            get
            {
                if(filesnode_cache != null) {
                    return filesnode_cache;
                }
                filesnode_cache = Helpers.decrypt(filesnode);
                return filesnode_cache;
            }
            set
            {
                filesnode_cache = null;
                filesnode = Helpers.encrypt(value);
            }
        }
        public bool isDownloadFolderTrigger = false;
        public bool ShareOnly = false;
        public bool UpdateFile = false;
        public bool isMergeRequest = false;
        public bool preRoot = false;
        public bool LocatedFile = false;
        public bool LocalRepo = false;
        public bool UnsplashRepo = false;
        public bool ShareState = true;


        //UPDATE: it's now used in MEGARoot not from here
        public bool ProtectedRepo = false;

        public string UpdateFileVersion = Helpers.AppVersionNumber;
        public string UpdateFileName = "";
        public string URIDownloadAuthor = "";
        public string Group = "";


        //UPDATE: There is new function to get this value now
        public string RootId = "";

        public bool IndexerItem = false;
        public bool isGitHub = false;
        public bool isGitHubDirect = false;
        public bool MenuItem = false;
        public bool SearchItem = false;
        public bool NoticeItem
        {
            get
            {
                return (noticeID != null && noticeID.Length > 0) || noticeItem != null;
            }
        }
        public string noticeID = "";
        public BrowseListModel noticeItem = null;
        public string githublink = "";
        [JsonIgnore]
        public string githublink_cache = null;
        [JsonIgnore]
        public string GitHubLink
        {
            get
            {
                if(githublink_cache != null)
                {
                    return githublink_cache;
                }
                githublink_cache = Helpers.decrypt(githublink);
                return githublink_cache;
            }
            set
            {
                githublink_cache = null;
                githublink = Helpers.encrypt(value);
            }
        }

        public string key = "";
        [JsonIgnore]
        public string key_cache = null;
        [JsonIgnore]
        public string Key
        {
            get
            {
                if (key_cache != null)
                {
                    return key_cache;
                }
                key_cache = Helpers.decrypt(key);
                return key_cache;
            }
            set
            {
                key_cache = null;
                key = Helpers.encrypt(value);
            }
        }
		
        [JsonIgnore]
        public bool Collapsed = false;

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
        [JsonIgnore]
        public Visibility itemVisibilityState = Visibility.Visible;

        [JsonIgnore]
        public string queueMonitor = "";

        [JsonIgnore]
        public string QueueMonitor2
        {
            get
            {
                return queueMonitor;
            }
            set
            {
                queueMonitor = value.ToUpper();
                RaisePropertyChanged(nameof(QueueMonitor2));
                if (QueueMonitorTextState == Visibility.Collapsed)
                {
                    QueueMonitorTextState = Visibility.Visible;
                    RaisePropertyChanged(nameof(QueueMonitorTextState));
                }
            }
        }

        [JsonIgnore]
        public Visibility QueueMonitorTextState = Visibility.Collapsed;
        public Visibility ItemVisibilityState
        {
            get
            {
                return itemVisibilityState;
            }
            set
            {
                itemVisibilityState = value;
                RaisePropertyChanged(nameof(ItemVisibilityState));
            }
        }
        [JsonIgnore]
        public Visibility ItemsIconsBlock
        {
            get
            {
                if (Helpers.DisableAllIcons && !MenuItem)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        [JsonIgnore]
        public Visibility ItemsProgressBlock = Visibility.Collapsed;

        [JsonIgnore]
        public bool ItemsProgressBlockActive = true;

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
        public void updateProgressState(Visibility itemsProgressBlock, bool itemsProgressBlockActive)
        {
            try
            {
                if (Helpers.DisableAllIcons)
                {
                    ItemsProgressBlock = Visibility.Collapsed;
                    ItemsProgressBlockActive = false;
                    RaisePropertyChanged(nameof(ItemsProgressBlock));
                    RaisePropertyChanged(nameof(ItemsProgressBlockActive));
                }
                else
                {
                    ItemsProgressBlock = itemsProgressBlock;
                    ItemsProgressBlockActive = itemsProgressBlockActive;
                    RaisePropertyChanged(nameof(ItemsProgressBlock));
                    RaisePropertyChanged(nameof(ItemsProgressBlockActive));
                }
            }
            catch (Exception e)
            {

            }
        }

        [JsonIgnore]
        public Visibility QuickViewAllowed
        {
            get
            {
                if (!Helpers.ShowQuickView || !Helpers.CheckIsFileMedia(fileName))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        [JsonIgnore]
        public Visibility QuickDownloadAllowed
        {
            get
            {
                if (!Helpers.ShowQuickDownload || isFolder)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
		
		[JsonIgnore]
        public Visibility QuickLocateAllowed
        {
            get
            {
                if (!SearchItem)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
		
		[JsonIgnore]
        public Visibility QuickNoticeAllowed
        {
            get
            {
                if (!NoticeItem)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        [JsonIgnore]
        public Visibility isMoreButtonVisible
        {
            get
            {
                if ((QuickShareAllowed == Visibility.Collapsed && QuickDownloadAllowed == Visibility.Collapsed && QuickViewAllowed == Visibility.Collapsed && QuickLocateAllowed == Visibility.Collapsed) || QuickShareAllowed2 == Visibility.Visible)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        [JsonIgnore]
        public Visibility QuickShareAllowed
        {
            get
            {
                if (!Helpers.ShowQuickShare || !ShareState)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        [JsonIgnore]
        public Visibility QuickShareAllowed2
        {
            get
            {
                if (!Helpers.ShowQuickShare || !ShareState || QuickDownloadAllowed == Visibility.Visible || QuickLocateAllowed == Visibility.Visible || QuickViewAllowed == Visibility.Visible)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        public void updateIconsState()
        {
            try
            {
                RaisePropertyChanged(nameof(ItemsIconsBlock));
                RaisePropertyChanged(nameof(QuickViewAllowed));
                RaisePropertyChanged(nameof(QuickDownloadAllowed));
				RaisePropertyChanged(nameof(QuickLocateAllowed));
                RaisePropertyChanged(nameof(QuickShareAllowed));
				RaisePropertyChanged(nameof(QuickNoticeAllowed));
                RaisePropertyChanged(nameof(BorderState));
            }
            catch (Exception e)
            {

            }
        }

        [JsonIgnore]
        public SolidColorBrush backgroundColor
        {
            get
            {
                if (LocatedFile)
                {
                    return new SolidColorBrush(Color.FromArgb(75, 10, 230, 10));
                }
                else
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        public void updateBackgroundColor()
        {
            try
            {
                RaisePropertyChanged(nameof(backgroundColor));
            }
            catch (Exception e)
            {

            }
        }

        [JsonIgnore]
        public Visibility RootTagVisible
        {
            get
            {
                try
                {
                    if (RootId != null && RootId.Length > 0 && (IndexerItem || Helpers.ShowRootName) && !ParentID.Equals("preRootID"))
                    {
                        RootId = RootId.ToUpper();
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                catch (Exception e)
                {
                    return Visibility.Collapsed;
                }
            }
        }

        [JsonConstructor]
        public BrowseListModel(MegaRoot megaRoot)
        {
            if (megaRoot != null)
            {
                fileName = megaRoot.Name;
                fileSize = GetSize(megaRoot.Size);
                string Type = megaRoot.Type.ToString();
                fileIcon = GetIconLocation("root.bmp");
                fileType = "Home".ToUpper();
                isRoot = true;
                fileID = megaRoot.Id;
                ParentID = megaRoot.ParentId;
                fileTypeOrder = 0;
                LocalRepo = megaRoot.LocalRepo;
                if (LocalRepo)
                {
                    fileIcon = GetIconLocation("repolocal.bmp");
                }
                else
                if (megaRoot != null && megaRoot.DirectRepo)
                {
                    fileIcon = GetIconLocation("repodirect.bmp");
                }
                else
                {
                    fileIcon = GetIconLocation("repo.bmp");
                }
            }
        }

        public BrowseListModel(string none)
        {

        }

        public string GetIconLocation(string iconName)
        {
            var iconFullPath = $"ms-appx:///Assets/Icons/Windows11/{iconName}";
            try
            {
                if (Helpers.AppIcons.Equals("CustomIcons"))
                {
                    iconFullPath = $"ms-appdata:///local/csIcons/{iconName}";
                }
                else
                {
                    iconFullPath = $"ms-appx:///Assets/Icons/{Helpers.AppIcons}/{iconName}";
                }
                return iconFullPath;
            }
            catch (Exception e)
            {

                return iconFullPath;
            }
        }
        public string GetIconLocation2(string iconName)
        {
            var iconFullPath = $"ms-appx:///Assets/Icons/Menus/{iconName}.bmp";
            return iconFullPath;
        }
        public void UpdateItemIcon()
        {
            try
            {
                var itemIconName = Path.GetFileName(fileIcon);
                fileIcon = GetIconLocation(itemIconName);
            }
            catch (Exception e)
            {

            }
        }

        [JsonIgnore]
        string tempSize
        {
            get
            {
                return fileSizeLong.ToFileSize();
            }
        }

        public BrowseListModel(MegaFolder megaFolder, bool isdownloadFolderTrigger = false, bool MediaOnly = false, bool isMenuItem = false, bool isQueueMonitor = false)
        {
            if (megaFolder != null)
            {
                fileTypeOrder = 1;
                fileName = megaFolder.Name;
                RootId = megaFolder.rootID;
                isDownloadFolderTrigger = isdownloadFolderTrigger;
                fileSizeLong = megaFolder.Size;
                LocalRepo = megaFolder.LocalRepo;
                ShareState = megaFolder.ShareState;
                Group = megaFolder.Group;
                if (isQueueMonitor)
                {
                    QueueMonitorTextState = Visibility.Visible;
                    RaisePropertyChanged(nameof(QueueMonitorTextState));
                }
                if (Helpers.CaclulateFolderSize)
                {
                    if (fileSizeLong > 0)
                    {
                        fileSize = GetSize(megaFolder.Size);
                    }
                    else
                    {
                        fileSize = "";
                    }
                }
                else
                {
                    fileSize = "";
                }
                string Type = megaFolder.Type.ToString();
                fileIcon = GetIconLocation("folder.bmp");
                if (megaFolder.preRoot)
                {
                    if (megaFolder.LocalRepo)
                    {
                        fileIcon = GetIconLocation("repolocal.bmp");
                    }
                    else
                    if (megaFolder.DirectRepo)
                    {
                        fileIcon = GetIconLocation("repodirect.bmp");
                    }
                    else
                    {
                        fileIcon = GetIconLocation("repo.bmp");
                    }
                }
                string fileNameTest = fileName.ToLower();
                fileType = "Folder".ToUpper();
                MenuItem = isMenuItem;
                if (isMenuItem)
                {
                    fileType = "";
                }
                bool sizeOverided = false;
                if (!megaFolder.IndexerItem)
                {
                    if (megaFolder.Folders != null && megaFolder.Folders.Count > 0)
                    {
                        var TotalFoldersCount = megaFolder.Folders.Count;
                        foreach (var folderItem in megaFolder.Folders)
                        {
                            if ((folderItem.Name.Contains(Helpers.FolderImageIcon)) && !folderItem.preRoot)
                            {
                                TotalFoldersCount--;
                            }
                        }
                        if (TotalFoldersCount > 0)
                        {
                            if (megaFolder.Folders.Count > 1 && megaFolder.Folders.Count < 11)
                            {
                                fileSize = $"{TotalFoldersCount} folders";
                            }
                            else
                            {
                                fileSize = $"{TotalFoldersCount} folder";
                            }
                            sizeOverided = true;
                        }
                    }
                    if (megaFolder.Files != null && megaFolder.Files.Count > 0)
                    {
                        var FilesCount = megaFolder.Files.Count;
                        string[] certs = { ".cer", ".cert", ".crt", ".pfx" };
                        foreach (var fileItem in megaFolder.Files)
                        {
                            if (fileItem.Name.Contains(Helpers.FolderImageIcon))
                            {
                                FilesCount--;
                            }
                            if (!LocalRepo && Helpers.HideCertificateState && certs.Contains(Path.GetExtension(fileItem.Name).ToLower()))
                            {
                                FilesCount--;
                            }
                        }
                        if (FilesCount > 0)
                        {
                            if (FilesCount > 1 && FilesCount < 11)
                            {
                                if (fileSize.Length > 0)
                                {
                                    fileSize = $"{fileSize} / {FilesCount} files";
                                }
                                else
                                {
                                    fileSize = $"{FilesCount} files";
                                }
                            }
                            else
                            {
                                if (fileSize.Length > 0)
                                {
                                    fileSize = $"{fileSize} / {FilesCount} file";
                                }
                                else
                                {
                                    fileSize = $"{FilesCount} file";
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (megaFolder.ParentName != null && megaFolder.ParentName.Length > 0)
                    {
                        fileSize = megaFolder.ParentName;
                    }
                }

                if (sizeOverided)
                {
                    if (fileSizeLong > 0 && Helpers.CaclulateFolderSize)
                    {
                        fileSize = $"{fileSize} / {tempSize}";
                    }
                }

                bool repoIconHandled = true;
                if (isdownloadFolderTrigger)
                {
                    fileTypeOrder = 0;
                    fileName = $"{megaFolder.Name}";
                    fileIcon = GetIconLocation("download.bmp");
                    fileType = "Options".ToUpper();
                    if (megaFolder.Files != null && megaFolder.Files.Count > 0)
                    {
                        var indexerCount = 0;
                        string[] certs = { ".cer", ".cert", ".crt", ".pfx" };
                        foreach (var FileItem in megaFolder.Files)
                        {
                            if (MediaOnly && Helpers.CheckIsFileMedia(FileItem) && !FileItem.Name.Contains(Helpers.FolderImageIcon))
                            {
                                if (!LocalRepo && Helpers.HideCertificateState && certs.Contains(Path.GetExtension(FileItem.Name).ToLower()))
                                {
                                    continue;
                                }
                                indexerCount++;
                            }
                            else if (!MediaOnly && !FileItem.Name.Contains(Helpers.FolderImageIcon))
                            {
                                if (!LocalRepo && Helpers.HideCertificateState && certs.Contains(Path.GetExtension(FileItem.Name).ToLower()))
                                {
                                    continue;
                                }
                                indexerCount++;
                            }
                        }

                        if (indexerCount > 1)
                        {
                            if (megaFolder.Files.Count == indexerCount)
                            {
                                if (LocalRepo)
                                {
                                    if (indexerCount > 1 && indexerCount < 11)
                                    {
                                        fileSize = $"Queue {indexerCount} Files";
                                    }
                                    else
                                    {
                                        fileSize = $"Queue {indexerCount} File";
                                    }
                                }
                                else
                                {
                                    if (indexerCount > 1 && indexerCount < 11)
                                    {
                                        fileSize = $"Download {indexerCount} Files";
                                    }
                                    else
                                    {
                                        fileSize = $"Download {indexerCount} File";
                                    }
                                }
                            }
                            else
                            {
                                if (LocalRepo)
                                {
                                    if (indexerCount > 1 && indexerCount < 11)
                                    {
                                        fileSize = $"Queue {indexerCount} of {megaFolder.Files.Where(item => !item.Name.Contains(Helpers.FolderImageIcon)).Count()} Files";
                                    }
                                    else
                                    {
                                        fileSize = $"Queue {indexerCount} of {megaFolder.Files.Where(item => !item.Name.Contains(Helpers.FolderImageIcon)).Count()} File";
                                    }
                                }
                                else
                                {
                                    if (indexerCount > 1 && indexerCount < 11)
                                    {
                                        fileSize = $"Download {indexerCount} of {megaFolder.Files.Where(item => !item.Name.Contains(Helpers.FolderImageIcon)).Count()} Files";
                                    }
                                    else
                                    {
                                        fileSize = $"Download {indexerCount} of {megaFolder.Files.Where(item => !item.Name.Contains(Helpers.FolderImageIcon)).Count()} File";
                                    }
                                }
                            }
                        }
                        else
                        {
                            fileIcon = GetIconLocation("share.bmp");
                            fileSize = "Folder Actions";
                            ShareOnly = true;
                        }
                    }
                    else
                    {
                        fileIcon = GetIconLocation("share.bmp");
                        fileSize = "Folder Actions";
                        ShareOnly = true;
                    }
                }
                else if (megaFolder.isRepo)
                {
                    if (fileNameTest.Equals("wp games") || fileNameTest.Equals("w10m games") || fileNameTest.Equals("wp8.1 games") || fileNameTest.Equals("wpgames"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/wpgames.bmp";
                    }
                    else if (fileNameTest.Equals("wp xbox games") || fileNameTest.Equals("xbox games") || fileNameTest.Equals("wp xbl games") || fileNameTest.Equals("wpxblgames") || fileNameTest.Equals("wpxboxgames"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/wpxgames.bmp";
                    }
                    else if (fileNameTest.Equals("wut home") || fileNameTest.Equals("wuthome") || fileNameTest.Equals("wut mini"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/wut.bmp";
                    }
                    else if (fileNameTest.Equals("w10m group") || fileNameTest.Equals("w10mgroup") || fileNameTest.Equals("w10m store"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/w10m.bmp";
                    }
                    else if (fileNameTest.StartsWith("projecta") || fileNameTest.Contains("astoria") || fileNameTest.Equals("android"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/astoria.bmp";
                    }
                    else if (fileNameTest.StartsWith("emulation") || fileNameTest.Contains("retro") || fileNameTest.Contains("emulators"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/retro.bmp";
                    }
                    else if (fileNameTest.StartsWith("dimas") || fileNameTest.Contains("pc mods"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/dimas.bmp";
                    }
                    else if (fileNameTest.StartsWith("penguin"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/penguin.bmp";
                    }
                    else if (fileNameTest.StartsWith("cuztomization") || fileNameTest.StartsWith("personalization") || fileNameTest.StartsWith("theme"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/paint.bmp";
                    }
                    else if (fileNameTest.StartsWith("empyreal") || fileNameTest.StartsWith("andromeda") || fileNameTest.StartsWith("leaks"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/andro.bmp";
                    }
                    else if (fileNameTest.StartsWith("photography"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/photos.bmp";
                    }
                    else if (fileNameTest.StartsWith("symbian"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/symbian.bmp";
                    }
                    else if ((fileNameTest.Contains("ms band") && fileNameTest.Contains("8.1")) || (fileNameTest.Contains("ms band") && fileNameTest.Contains("8.1")) || (fileNameTest.Contains("ms band repo") && fileNameTest.Contains("8.1")) || (fileNameTest.Contains("ms band group") && fileNameTest.Contains("8.1")))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/band81.bmp";
                    }
                    else if (fileNameTest.Contains("ms band") || fileNameTest.Contains("ms band") || fileNameTest.Contains("ms band repo") || fileNameTest.Contains("ms band group"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/msband.bmp";
                    }
                    else if (fileNameTest.StartsWith("telegram w10m group") || fileNameTest.Contains("w10m telegram") || fileNameTest.Contains("w10m repo telegram") || fileNameTest.Contains("w10m telegram group"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/telegram.bmp";
                    }
                    else if (fileNameTest.Equals("w8.1m group") || fileNameTest.Equals("w8.1m store") || fileNameTest.Equals("w8.1mgroup") || fileNameTest.Equals("wp8.1 group") || fileNameTest.Equals("wp8.1 store") || fileNameTest.Equals("wp 8.1 group"))
                    {
                        fileIcon = "ms-appx:///Assets/Icons/w81m.bmp";
                    }
                    else
                    {
                        repoIconHandled = false;
                    }
                }
                else
                {
                    repoIconHandled = false;
                }

                if (repoIconHandled)
                {

                }
                else
                if (fileNameTest.Contains("browser"))
                {
                    fileIcon = GetIconLocation("folder-Browser.bmp");
                }
                else if (fileNameTest.Contains("emulators"))
                {
                    fileIcon = GetIconLocation("folder-games.bmp");
                }
                else if (fileNameTest == "apps")
                {
                    fileIcon = GetIconLocation("folder-apps.bmp");
                }
                else if (fileNameTest.Contains("microsoft apps"))
                {
                    fileIcon = GetIconLocation("folder-microsoft.bmp");
                }
                else if (fileNameTest.Contains("extension"))
                {
                    fileIcon = GetIconLocation("folder-extensions.bmp");
                }
                else if (fileNameTest.Contains("pc") && fileNameTest.Contains("tools"))
                {
                    fileIcon = GetIconLocation("folder-pctools.bmp");
                }
                else if (fileNameTest.Contains("microsoft") && fileNameTest.Contains("band"))
                {
                    fileIcon = GetIconLocation("folder-band.bmp");
                }
                else if (fileNameTest.Contains("camera") || fileNameTest.Contains("background") || fileNameTest.Contains("pictures") || fileNameTest.Contains("photos"))
                {
                    fileIcon = GetIconLocation("folder-camera.bmp");
                }
                else if (fileNameTest.Contains("customization"))
                {
                    fileIcon = GetIconLocation("folder-customize.bmp");
                }
                else if (fileNameTest.Contains("dependencies"))
                {
                    fileIcon = GetIconLocation("folder-deps.bmp");
                }
                else if (fileNameTest.Contains("microsoft"))
                {
                    fileIcon = GetIconLocation("folder-microsoft.bmp");
                }
                else if (fileNameTest.Contains("multimedia"))
                {
                    fileIcon = GetIconLocation("folder-media.bmp");
                }
                else if (fileNameTest.Contains("memories"))
                {
                    fileIcon = GetIconLocation("folder-memo.bmp");
                }
                else if (fileNameTest.Contains("productivity"))
                {
                    fileIcon = GetIconLocation("folder-products.bmp");
                }
                else if (fileNameTest.Contains("social"))
                {
                    fileIcon = GetIconLocation("Folder-Social.bmp");
                }
                else if (fileNameTest.Contains("tweaks") || fileNameTest.Contains("tools"))
                {
                    fileIcon = GetIconLocation("folder-tools.bmp");
                }
                else if (fileNameTest.Contains("tools") || fileNameTest.Contains("settings"))
                {
                    fileIcon = GetIconLocation("folder-wtools.bmp");
                }
                else if (fileNameTest.Contains("music") || fileNameTest.Contains("musical") || fileNameTest.Contains("audio") || (fileNameTest.Contains("ring") && !fileNameTest.Contains("ering") && !fileNameTest.Contains("uring") && !fileNameTest.Contains("tring")))
                {
                    fileIcon = GetIconLocation("folder-music.bmp");
                }
                else if (fileNameTest.Contains("xbox"))
                {
                    fileIcon = GetIconLocation("folder-XboxLive.bmp");
                }
                else if (fileNameTest.Contains("guides") || fileNameTest.Contains("how to"))
                {
                    fileIcon = GetIconLocation("folder-guides.bmp");
                }
                else if (fileNameTest.Contains("hp exclusive"))
                {
                    fileIcon = GetIconLocation("folder-hp.bmp");
                }
                else if (fileNameTest.Contains("security"))
                {
                    fileIcon = GetIconLocation("folder-security.bmp");
                }
                else if (fileNameTest.Contains("office"))
                {
                    fileIcon = GetIconLocation("folder-office.bmp");
                }
                else if (fileNameTest.Contains("mail"))
                {
                    fileIcon = GetIconLocation("folder-mail.bmp");
                }
                else if (fileNameTest.Contains("transfer"))
                {
                    fileIcon = GetIconLocation("folder-transfer.bmp");
                }
                else if (fileNameTest.Contains("clock") || fileNameTest.Contains("alarm"))
                {
                    fileIcon = GetIconLocation("folder-clock.bmp");
                }
                else if (fileNameTest.Contains("exclusive"))
                {
                    fileIcon = GetIconLocation("folder-exclusive.bmp");
                }
                else if (fileNameTest.Contains("gps") || fileNameTest.Contains("navigation") || fileNameTest.Contains("travel"))
                {
                    fileIcon = GetIconLocation("Folder-GPS,Navigation.bmp");
                }
                else if (fileNameTest.Contains("health"))
                {
                    fileIcon = GetIconLocation("Folder-Health.bmp");
                }
                else if (fileNameTest.Contains("health"))
                {
                    fileIcon = GetIconLocation("Folder-Health.bmp");
                }
                else if (fileNameTest.Contains("weather"))
                {
                    fileIcon = GetIconLocation("Folder-Weather, News.bmp");
                }
                else if (fileNameTest.Contains("msn"))
                {
                    fileIcon = GetIconLocation("Folder-MSN.bmp");
                }
                else if (fileNameTest.Contains("memories"))
                {
                    fileIcon = GetIconLocation("Folder-Old Memories.bmp");
                }
                else if (fileNameTest.Contains("games"))
                {
                    fileIcon = GetIconLocation("folder-games.bmp");
                }

                if (!isDownloadFolderTrigger && LocalRepo && megaFolder.remoteImageIcon != null && megaFolder.remoteImageIcon.Length > 0)
                {
                    fileIcon = megaFolder.remoteImageIcon;
                }
                else
                if (!isdownloadFolderTrigger && megaFolder.remoteIcon)
                {
                    CheckFolderIcon(megaFolder);
                }

                isFolder = true;
                fileID = megaFolder.Id;
                Folder = megaFolder;
                ParentID = megaFolder.ParentId;
                FilesNode = megaFolder.FilesNode;
                IndexerItem = megaFolder.IndexerItem;
            }
        }
        public async void FindRemoteIcon()
        {
            try
            {
                var IconPathTest = "";
                Helpers.RemoteIconsCache.TryGetValue(fileID, out IconPathTest);
                if (IconPathTest != null && IconPathTest.Length > 0)
                {
                    fileIcon = IconPathTest;
                    return;
                }
                string RemoteIconName = $"i{fileID}.png";
                var localFolder = ApplicationData.Current.LocalFolder;
                var testFolder = (StorageFolder)await localFolder.TryGetItemAsync("Remote Icons");
                if (testFolder != null)
                {
                    var testIcon = (StorageFile)await testFolder.TryGetItemAsync(RemoteIconName);
                    if (testIcon != null)
                    {
                        fileIcon = testIcon.Path;
                        try
                        {
                            Helpers.RemoteIconsCache.Add(fileID, fileIcon);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        string TempIcon = "";
        public void CheckFolderIcon(MegaFolder megaFolder)
        {
            try
            {
                if (TempIcon.Length == 0)
                {
                    TempIcon = fileIcon;
                }
                //await megaFolder.CheckRemoteIcon();
                if (megaFolder.remoteIcon)
                {
                    fileIcon = megaFolder.remoteImageIcon;
                    try
                    {
                        if (!File.Exists(fileIcon))
                        {
                            fileIcon = TempIcon;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    Helpers.UpdateBindings.Invoke(null, null);
                }
            }
            catch (Exception e)
            {

            }
        }

        public MegaFile megaFileNode;
        public string OriginalFileSize = "";
        public BrowseListModel(MegaFile megaFile, CancellationToken cancellationToken, string forceIcon = "")
        {
            try
            {
                if (megaFile != null)
                {
                    fileTypeOrder = 2;
                    fileName = megaFile.Name;
                    try
                    {
                        string GitHubPattern = @"(?<name>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<host>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<user>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<repo>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<tag>[\w\s_\d.\-()!@#$^&_+=';]+)";
                        Match m = Regex.Match(fileName, GitHubPattern, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            if (m.Groups != null && m.Groups.Count > 0)
                            {
                                var name = m.Groups["name"].Value;
                                var host = m.Groups["host"].Value;
                                var username = m.Groups["user"].Value;
                                var reponame = m.Groups["repo"].Value;
                                var tag = m.Groups["tag"].Value;
                                if (host.Trim().ToLower().Equals("github"))
                                {
                                    isGitHub = true;
                                    if (tag.Trim().ToLower().Equals("latest"))
                                    {
                                        GitHubLink = $"https://github.com/{username}/{reponame}/releases/{tag}";
                                    }
                                    else
                                    {
                                        GitHubLink = $"https://github.com/{username}/{reponame}/releases/tag/{tag}";
                                    }
                                    fileName = $"{name}.git";
                                }
                            }
                        }
                        else
                        {
                            GitHubPattern = @"(?<host>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<user>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<repo>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<tag>[\w\s_\d.\-()!@#$^&_+=';]+)";
                            m = Regex.Match(fileName, GitHubPattern, RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                if (m.Groups != null && m.Groups.Count > 0)
                                {
                                    var host = m.Groups["host"].Value;
                                    var username = m.Groups["user"].Value;
                                    var reponame = m.Groups["repo"].Value;
                                    var tag = m.Groups["tag"].Value;
                                    if (host.Trim().ToLower().Equals("github"))
                                    {
                                        isGitHub = true;
                                        if (tag.Trim().ToLower().Equals("latest"))
                                        {
                                            GitHubLink = $"https://github.com/{username}/{reponame}/releases/{tag}";
                                        }
                                        else
                                        {
                                            GitHubLink = $"https://github.com/{username}/{reponame}/releases/tag/{tag}";
                                        }
                                        fileName = $"{reponame} ({tag}).git";
                                    }
                                }
                            }
                            else
                            {
                                GitHubPattern = @"(?<host>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<user>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<repo>[\w\s_\d.\-()!@#$%^&_+=';]+)#(?<tag>\%[\w\s_\d.\-()!@#$%^&_+=';]+)";
                                m = Regex.Match(fileName, GitHubPattern, RegexOptions.IgnoreCase);
                                if (m.Success)
                                {
                                    if (m.Groups != null && m.Groups.Count > 0)
                                    {
                                        var host = m.Groups["host"].Value;
                                        var username = m.Groups["user"].Value;
                                        var reponame = m.Groups["repo"].Value;
                                        var tag = m.Groups["tag"].Value;
                                        tag = tag.Replace("%", "/");
                                        isGitHubDirect = true;
                                        if (host.ToLower().Trim().Equals("github"))
                                        {
                                            host = host.Trim() + ".io";
                                        }
                                        if (username.Trim().Length > 0)
                                        {
                                            GitHubLink = $"https://{username}.{host}/{reponame}{tag}";
                                            fileName = Path.GetFileName(tag);
                                        }
                                        else
                                        {
                                            GitHubLink = $"https://{host}/{reponame}{tag}";
                                            fileName = Path.GetFileName(tag);
                                        }
                                        try
                                        {
                                            //TODO Size should be cached
                                            if (megaFile.Size == 0)
                                            {
                                                //megaFile.Size = WebHelper.GetFileSize(GitHubLink).Result;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    RootId = megaFile.rootID;
                    fileSize = GetSize(megaFile.Size);
                    OriginalFileSize = fileSize;
                    LocalRepo = megaFile.LocalRepo;
                    if (megaFile.IndexerItem && megaFile.ParentName != null && megaFile.ParentName.Length > 0)
                    {
                        fileSize = $"{fileSize} / {megaFile.ParentName}";
                    }
                    fileSizeLong = megaFile.Size;
                    IndexerItem = megaFile.IndexerItem;
                    string Type = megaFile.Type.ToString();
                    fileIcon = GetFileIcon(forceIcon);
                    if (LocalRepo && megaFile.remoteImageIcon != null && megaFile.remoteImageIcon.Length > 0)
                    {
                        fileIcon = megaFile.remoteImageIcon;
                    }
                    else
                    {
                        CheckFileIcon(megaFile, cancellationToken);
                    }

                    isFile = true;
                    fileID = megaFile.Id;
                    FilesNode = megaFile.FilesNode;
                    ParentID = megaFile.ParentId;
                    megaFileNode = megaFile;
                    SerializedFingerprint = megaFile.GetFileUniqueName();
                }
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
            }

        }
        public void CheckFileIcon(MegaFile megaFile, CancellationToken cancellationToken, bool updateIcon = false)
        {
            if (!Helpers.EnableRemoteIcons)
            {
                return;
            }
            try
            {
                if (TempIcon.Length == 0)
                {
                    TempIcon = fileIcon;
                }
                if (megaFile.remoteIcon)
                {
                    fileIcon = megaFile.remoteImageIcon;
                    try
                    {
                        if (!File.Exists(fileIcon))
                        {
                            fileIcon = TempIcon;
                            if (updateIcon)
                            {
                                RaisePropertyChanged(nameof(fileIcon));
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    Helpers.UpdateBindings.Invoke(null, null);
                }
            }
            catch (Exception e)
            {

            }
        }
        public bool processInProgress = false;
        public async Task<bool> GetThumbnail(CancellationToken cancellationToken)
        {
            if (processInProgress)
            {
                return false;
            }
            processInProgress = true;
            bool iconState = false;
            try
            {
                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        if (TempIcon.Length == 0)
                        {
                            TempIcon = fileIcon;
                        }

                        if (Helpers.CheckIsImageVideo(fileName))
                        {
                            //updateProgressState(Visibility.Visible, true);
                            fileIcon = "ms-appx:///Assets/loader.bmp";
                            iconState = await megaFileNode.GetThumbnail(cancellationToken);
                            if (iconState)
                            {
                                fileIcon = megaFileNode.remoteImageIcon;
                            }
                            else
                            {
                                fileIcon = TempIcon;
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {

                    }
                });
                await taskCompletionSource.WaitAsync(cancellationToken);
                //updateProgressState(Visibility.Collapsed, false);
            }
            catch (Exception ex)
            {
                fileIcon = TempIcon;
                updateProgressState(Visibility.Collapsed, false);
                Helpers.Logger(ex);
            }
            ThumbLoaded = iconState;
            downloadThumbTries++;
            processInProgress = false;
            return iconState;
        }
        public int downloadThumbTries = 0;
        public void UpdateIcon()
        {
            try
            {
                fileIcon = megaFileNode.remoteImageIcon;
            }
            catch (Exception ex)
            {

            }
        }
        public string GetSize(long size)
        {
            if (size > 0)
            {
                return size.ToFileSize();
            }
            return "-";
        }
        public string GetFileIcon(string forceIcon = "")
        {
            string FileIcon = GetIconLocation("default.bmp");
            string FileExtension = Path.GetExtension(fileName).ToLower();
            fileType = FileExtension.Replace(".", "").ToUpper();
            string fileNameTest = fileName.ToLower();
            if (
                !FileExtension.Contains("mp3") && !FileExtension.Contains("wav") && !FileExtension.Contains("mp4") && !FileExtension.Contains("avi") && !FileExtension.Contains("wma") && !FileExtension.Contains("ogg") &&
                !FileExtension.Contains("crt") && !FileExtension.Contains("cer") && !FileExtension.Contains("pfx") && !FileExtension.Contains("cert") && !FileExtension.Contains("reg") &&
                !FileExtension.Contains("pdf") && !FileExtension.Contains("doc") && !FileExtension.Contains("txt") && !FileExtension.Contains("rtf") && (!FileExtension.Contains("zip") || fileNameTest.Contains("retroarch")) && !FileExtension.Contains("7z") && !FileExtension.Contains("tar") && !FileExtension.Contains("gz") && !FileExtension.Contains("wuts") && !FileExtension.Contains("wutc") && !FileExtension.Contains("wutz") &&
                !FileExtension.Contains("png") && !FileExtension.Contains("bmp") && !FileExtension.Contains("jpg") && !FileExtension.Contains("jpeg") && !FileExtension.Contains("jpe") && !FileExtension.Contains("gif") && !FileExtension.Contains("ico"))
            {
                if (forceIcon.Length > 0)
                {
                    FileIcon = GetIconLocation(forceIcon);
                }
                else
                if (fileNameTest.Contains("calendar"))
                {
                    FileIcon = GetIconLocation("calendar.bmp");
                }
                else if (fileNameTest.Contains("interop"))
                {
                    FileIcon = GetIconLocation("interop.bmp");
                }
                else if (fileNameTest.Contains("scummvm"))
                {
                    FileIcon = GetIconLocation("scummvm.bmp");
                }
                else if (fileNameTest.Contains("injector"))
                {
                    FileIcon = GetIconLocation("cmdi.bmp");
                }
                else if (fileNameTest.Contains("easy") && fileNameTest.Contains("fetch"))
                {
                    FileIcon = GetIconLocation("easy.bmp");
                }
                else if (fileNameTest.Contains("calculator"))
                {
                    FileIcon = GetIconLocation("calculator.bmp");
                }
                else if (fileNameTest.Contains("alarm"))
                {
                    FileIcon = GetIconLocation("alarm.bmp");
                }
                else if (fileNameTest.Contains("camera"))
                {
                    FileIcon = GetIconLocation("camera.bmp");
                }
                else if (fileNameTest.Contains("movies"))
                {
                    FileIcon = GetIconLocation("movies.bmp");
                }
                else if (fileNameTest.Contains("feedback"))
                {
                    FileIcon = GetIconLocation("feedback.bmp");
                }
                else if (fileNameTest.Contains("email") || fileNameTest.Contains("outlook") || fileNameTest.Contains("mail"))
                {
                    FileIcon = GetIconLocation("email.bmp");
                }
                else if (fileNameTest.Contains("paint"))
                {
                    FileIcon = GetIconLocation("paint.bmp");
                }
                else if (fileNameTest.Contains("audacity"))
                {
                    FileIcon = GetIconLocation("audacity.bmp");
                }
                else if (fileNameTest.Contains("blender"))
                {
                    FileIcon = GetIconLocation("blender.bmp");
                }
                else if (fileNameTest.Contains("dropbox"))
                {
                    FileIcon = GetIconLocation("dropbox.bmp");
                }
                else if (fileNameTest.Contains("edge") && (fileNameTest.Contains("microsoft") || fileNameTest.Contains("ms")))
                {
                    FileIcon = GetIconLocation("edge.bmp");
                }
                else if (fileNameTest.Contains("firefox"))
                {
                    FileIcon = GetIconLocation("firefox.bmp");
                }
                else if (fileNameTest.Contains("github"))
                {
                    FileIcon = GetIconLocation("git.bmp");
                }
                else if (fileNameTest.Contains("chromium"))
                {
                    FileIcon = GetIconLocation("chromium.bmp");
                }
                else if (fileNameTest.Contains("chrome"))
                {
                    FileIcon = GetIconLocation("chrome.bmp");
                }
                else if (fileNameTest.Contains("vlc"))
                {
                    FileIcon = GetIconLocation("vlc.bmp");
                }
                else if (fileNameTest.Contains("onenote") || fileNameTest.Contains("one note") || fileNameTest.Contains("one-note") || fileNameTest.Contains("one.note") || fileNameTest.Contains("one_note"))
                {
                    FileIcon = GetIconLocation("onenote.bmp");
                }
                else if ((fileNameTest.Contains("microsoft") || fileNameTest.Contains("office") || fileNameTest.Contains("ms ")) && fileNameTest.Contains("word"))
                {
                    FileIcon = GetIconLocation("word.bmp");
                }
                else if (fileNameTest.Contains("powerpoint"))
                {
                    FileIcon = GetIconLocation("powerpoint.bmp");
                }
                else if ((fileNameTest.Contains("microsoft") || fileNameTest.Contains("office") || fileNameTest.Contains("ms ")) && fileNameTest.Contains("excel"))
                {
                    FileIcon = GetIconLocation("excel.bmp");
                }
                else if ((fileNameTest.Contains("microsoft") || fileNameTest.Contains("360") || fileNameTest.Contains("ms ")) && fileNameTest.Contains("office"))
                {
                    FileIcon = GetIconLocation("office.bmp");
                }
                else if (fileNameTest.Contains("netflix"))
                {
                    FileIcon = GetIconLocation("netflix.bmp");
                }
                else if (fileNameTest.Contains("notepad"))
                {
                    FileIcon = GetIconLocation("notepad.bmp");
                }
                else if (fileNameTest.StartsWith("opera") || fileNameTest.EndsWith("opera") || fileNameTest.Contains("opera "))
                {
                    FileIcon = GetIconLocation("opera.bmp");
                }
                else if (fileNameTest.Contains("python"))
                {
                    FileIcon = GetIconLocation("python.bmp");
                }
                else if (fileNameTest.Contains("adobe") && fileNameTest.Contains("reader"))
                {
                    FileIcon = GetIconLocation("reader.bmp");
                }
                else if (fileNameTest.Contains("spotify"))
                {
                    FileIcon = GetIconLocation("spotify.bmp");
                }
                else if (fileNameTest.Contains("steam"))
                {
                    FileIcon = GetIconLocation("steam.bmp");
                }
                else if (fileNameTest.Contains("atom"))
                {
                    FileIcon = GetIconLocation("atom.bmp");
                }
                else if (fileNameTest.Contains("vegas"))
                {
                    FileIcon = GetIconLocation("vegas.bmp");
                }
                else if (fileNameTest.Contains("xbox"))
                {
                    FileIcon = GetIconLocation("xbox.bmp");
                }
                else if (fileNameTest.Contains("store"))
                {
                    FileIcon = GetIconLocation("store.bmp");
                }
                else if (fileNameTest.Contains("audio"))
                {
                    FileIcon = GetIconLocation("audio.bmp");
                }
                else if (fileNameTest.Contains("battery"))
                {
                    FileIcon = GetIconLocation("battery.bmp");
                }
                else if (fileNameTest.Contains("record") && !fileNameTest.Contains("screenrecorder") && !fileNameTest.Contains("recordfixer"))
                {
                    FileIcon = GetIconLocation("record.bmp");
                }
                else if (fileNameTest.Contains("firewall"))
                {
                    FileIcon = GetIconLocation("firewall.bmp");
                }
                else if (fileNameTest.Contains("font"))
                {
                    FileIcon = GetIconLocation("font.bmp");
                }
                else if (fileNameTest.Contains("groove"))
                {
                    FileIcon = GetIconLocation("groove.bmp");
                }
                else if (fileNameTest.Contains("heart"))
                {
                    FileIcon = GetIconLocation("heart.bmp");
                }
                else if (fileNameTest.Contains("search"))
                {
                    FileIcon = GetIconLocation("search.bmp");
                }
                else if (fileNameTest.Contains("translate"))
                {
                    FileIcon = GetIconLocation("translate.bmp");
                }
                else if (fileNameTest.Contains("network"))
                {
                    FileIcon = GetIconLocation("network.bmp");
                }
                else if (fileNameTest.Contains("onedrive") || fileNameTest.Contains("one drive"))
                {
                    FileIcon = GetIconLocation("onedrive.bmp");
                }
                else if (fileNameTest.Contains("people"))
                {
                    FileIcon = GetIconLocation("people.bmp");
                }
                else if (fileNameTest.Contains("performance"))
                {
                    FileIcon = GetIconLocation("performance.bmp");
                }
                else if (fileNameTest.Contains("sdcard") || fileNameTest.Contains("storage") || (fileNameTest.Contains("sd ") && fileNameTest.Contains("card")))
                {
                    FileIcon = GetIconLocation("sdcard.bmp");
                }
                else if (fileNameTest.Contains("security"))
                {
                    FileIcon = GetIconLocation("security.bmp");
                }
                else if (fileNameTest.Contains("tools"))
                {
                    FileIcon = GetIconLocation("tools.bmp");
                }
                else if (fileNameTest.Contains("retrix"))
                {
                    FileIcon = GetIconLocation("retrix.bmp");
                }
                else if (fileNameTest.Contains("retroarch"))
                {
                    FileIcon = GetIconLocation("RetroArch.bmp");
                }
                else if (fileNameTest.Contains("ppsspp"))
                {
                    FileIcon = GetIconLocation("ppsspp.bmp");
                }
                else if (fileNameTest.Contains("authenticator"))
                {
                    FileIcon = GetIconLocation("auth.bmp");
                }
                else if (fileNameTest.Contains("browser"))
                {
                    FileIcon = GetIconLocation("browser.bmp");
                }
                else if (fileNameTest.Contains("skype"))
                {
                    FileIcon = GetIconLocation("skype.bmp");
                }
                else if (fileNameTest.Contains("photoshop"))
                {
                    FileIcon = GetIconLocation("photoshop.bmp");
                }
                else if (fileNameTest.Contains("autocad"))
                {
                    FileIcon = GetIconLocation("autocad.bmp");
                }
                else if (fileNameTest.Contains("telegram"))
                {
                    FileIcon = GetIconLocation("telg.bmp");
                }
                else if (fileNameTest.Contains("unigram"))
                {
                    FileIcon = GetIconLocation("unig.bmp");
                }
                else if (fileNameTest.Contains("chip-8") || fileNameTest.StartsWith("chip8"))
                {
                    FileIcon = GetIconLocation("ch8.bmp");
                }
                else
                {
                    FileIcon = getIconByExtention(FileExtension);
                }
            }
            else
            {
                FileIcon = getIconByExtention(FileExtension);
            }
            return FileIcon;
        }
        private string getIconByExtention(string FileExtension)
        {
            var fileTypeLocal = FileExtension.Replace(".", "").ToLower();
            string FileIcon = GetIconLocation("default.bmp");
            string fileNameTest = fileName.ToLower();
            switch (FileExtension)
            {
                case ".txt":
                    if (fileNameTest.Contains("guide"))
                    {
                        FileIcon = GetIconLocation("info.bmp");
                    }
                    else if (fileNameTest.Contains("notice"))
                    {
                        FileIcon = GetIconLocation("info.bmp");
                    }
                    else if (fileNameTest.Contains("note"))
                    {
                        FileIcon = GetIconLocation("info.bmp");
                    }
                    else if (fileNameTest.Contains("readme"))
                    {
                        FileIcon = GetIconLocation("info.bmp");
                    }
                    else
                    {
                        FileIcon = GetIconLocation($"{fileTypeLocal}.bmp");
                    }
                    break;
                case ".rdp":
                case ".sln":
                case ".py":
                case ".ppt":
                case ".pptx":
                case ".doc":
                case ".docx":
                case ".adb":
                case ".git":
                case ".blend":
                case ".blender":
                case ".3gp":
                case ".7z":
                case ".aif":
                case ".appx":
                case ".appxbundle":
                case ".msix":
                case ".msixbundle":
                case ".avi":
                case ".bat":
                case ".bin":
                case ".cer":
                case ".cert":
                case ".cs":
                case ".exe":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".json":
                case ".lnk":
                case ".mkv":
                case ".mp3":
                case ".mp4":
                case ".msi":
                case ".ogg":
                case ".pdf":
                case ".pfx":
                case ".png":
                case ".rar":
                case ".sql":
                case ".tif":
                case ".tmp":
                case ".ttf":
                case ".unigram-theme":
                case ".uni-theme":
                case ".wav":
                case ".wma":
                case ".wmv":
                case ".xap":
                case ".xls":
                case ".xlsx":
                case ".zip":
                case ".tar":
                case ".gz":
                case ".js":
                case ".css":
                case ".wuts":
                case ".wutc":
                case ".wutz":
                case ".wut":
                case ".w10ms":
                case ".w10mc":
                case ".w10mz":
                case ".w10m":
                case ".reg":
                case ".svg":
                case ".html":
                case ".htm":
                case ".webm":
                case ".aac":
                case ".opus":
                case ".php":
                case ".apk":
                case ".xapk":
                case ".aab":
                    FileIcon = GetIconLocation($"{fileTypeLocal}.bmp");
                    break;

                case ".rtf":
                    FileIcon = GetIconLocation($"notepad.bmp");
                    break;

                case ".ffu":
                    FileIcon = GetIconLocation($"runtime.bmp");
                    break;

                case ".cab":
                    FileIcon = GetIconLocation($"zip.bmp");
                    break;

                case ".ch8":
                    FileIcon = GetIconLocation($"ch8.bmp");
                    break;

                case ".jpe":
                    FileIcon = GetIconLocation($"jpeg.bmp");
                    break;

                case ".spkg":
                case ".spku":
                case ".spkr":
                    FileIcon = GetIconLocation($"msi.bmp");
                    break;
            }

            return FileIcon;
        }
    }
}
