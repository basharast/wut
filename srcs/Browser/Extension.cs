/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.Storage;
using Windows.UI.Xaml;

namespace WinUniversalTool.WebViewer
{
    public class Extension
    {
        public StorageFile ExtensionFile;
        public string ExtensionLocation;
        public string ExtensionShortLocation;
        public string ExtensionType;
        public string ExtensionIcon;
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
        public Extension(StorageFile extensionFile)
        {
            ExtensionFile = extensionFile;
            ExtensionLocation = extensionFile.Path;

            var fileExtension = Path.GetExtension(extensionFile.Name);
            switch (fileExtension)
            {
                case ".js":
                    ExtensionType = "JavaScript";
                    ExtensionIcon = "ms-appx:///Assets/Icons/Windows11/js.bmp";
                    break;

                case ".dll":
                    ExtensionType = "Runtime";
                    ExtensionIcon = "ms-appx:///Assets/Icons/Windows11/dll.bmp";
                    break;
            }
            ExtensionShortLocation = "WUTExts"+ExtensionLocation.Replace(ApplicationData.Current.LocalFolder.Path,"");
        }
    }
}
