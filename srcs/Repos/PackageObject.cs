using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.Storage;
using Windows.UI.Xaml;
/**

Copyright (c) 2021-2024  Bashar Astifan

*/
namespace WinUniversalTool.DirectStorage
{
    public class PackageObject : BindableBase
    {
        public string PackageIcon = "ms-appx:///Assets/Icons/Windows11/appx.png";
        public string PackageTag;
        public string PackageName;
        public string PackageID;
        public string PackageType = "";
        public string Description = "";
        public string Publisher = "";
        public string Version = "";
        public string SizeText = "";
        public long Size = 0;

        [JsonIgnore]
        public string NotAllowedChars = "[\\/\\:*?\"<>|]";
        [JsonIgnore]
        public Visibility isRunPossible = Visibility.Visible;
        public string PackageLocation = "";
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
        /*[JsonIgnore]
        public List<StorageFolder> storageFolders = new List<StorageFolder>();*/

        public PackageObject(string packageID, string packageName, string packageIcon/*, StorageFolder packageLocation*/, string packageTag = "")
        {
            if (packageID != null)
            {
                PackageID = packageID;
                PackageName = packageName;
                try
                {
                    PackageName = Regex.Replace(PackageName, NotAllowedChars, "_");
                }
                catch (Exception e)
                {

                }
                if (packageIcon?.Length > 0)
                {
                    PackageIcon = packageIcon;
                }
                else
                {
                    PackageIcon = $"ms-appx:///Assets/Icons/{Helpers.AppIcons}/appx.bmp";
                }
                PackageTag = packageTag;
                //PackageLocation = packageLocation;
            }
        }

        public async Task ReloadIcon()
        {
            try
            {
                var tempIcon = PackageIcon;
                PackageIcon = $"ms-appx:///Assets/Icons/{Helpers.AppIcons}/appx.bmp";
                await RaisePropertyChanged(nameof(PackageIcon));
                await Task.Delay(50);
                PackageIcon = tempIcon;
                await RaisePropertyChanged(nameof(PackageIcon));
            }
            catch (Exception e)
            {

            }
        }
    }
}
