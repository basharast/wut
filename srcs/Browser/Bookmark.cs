/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.UI.Xaml;
using WinUniversalTool.DirectStorage;
using Windows.UI.StartScreen;

namespace WinUniversalTool.WebViewer
{
    public class Bookmark : BindableBase
    {
        [JsonIgnore]
        public bool Collapsed = false;

        [JsonIgnore]
        public string Key
        {
            get
            {
                return BookmarkFolder;
            }
        }
        public string bookmarkFolder;

        [JsonIgnore]
        public bool isPinned {
            get
            {
                var tileID = "B_"+LocalToData.crc32(BookmarkURL);
                bool pinned = SecondaryTile.Exists(tileID);
                return pinned;
            }
        }

        [JsonIgnore]
        public string PinButtonText
        {
            get
            {
                return isPinned ? "UnPin" : "Pin";
            }
        }
        public void UpdatePinnedText()
        {
            try
            {
                RaisePropertyChanged(nameof(PinButtonText));
            }catch(Exception ex)
            {

            }
        }

        [JsonIgnore]
        public string BookmarkFolder
        {
            get
            {
                if (bookmarkFolder == null)
                {
                    return "General";
                }
                else
                {
                    return bookmarkFolder;
                }
            }
            set
            {
                bookmarkFolder = value;
                RaisePropertyChanged(nameof(BookmarkFolder));
            }
        }

        public string bookmarkTitle;
        public string BookmarkTitle
        {
            get
            {
                return bookmarkTitle;
            }
            set
            {
                bookmarkTitle = value;
                RaisePropertyChanged(nameof(BookmarkTitle));
            }
        }

        [JsonIgnore]
        public string BookmarkTitleShort
        {
            get
            {
                if (BookmarkTitle.Length > 50)
                {
                    return $"{BookmarkTitle.Substring(0, 35)}..";
                }
                else
                {
                    return BookmarkTitle;
                }
            }
        }
        public string bookmarkURL;
        public string BookmarkURL
        {
            get
            {
                return bookmarkURL;
            }
            set
            {
                bookmarkURL = value;
                RaisePropertyChanged(nameof(BookmarkURL));
            }
        }

        string bookmarkIcon;
        public string BookmarkIcon
        {
            get
            {
                if(bookmarkIcon != null && bookmarkIcon.Length> 0 || bookmarkIcon.Contains(".svg") || bookmarkIcon.Contains(WebHelper.EMPTY_FAVICON))
                {
                    return bookmarkIcon;
                }
                else
                {
                    return "ms-appx:///Assets/Icons/Windows11/fav.bmp";
                }
            }
            set
            {
                if (value==null || value.Length == 0 || value.Contains(".svg") || value.Contains(WebHelper.EMPTY_FAVICON))
                {
                    bookmarkIcon = "ms-appx:///Assets/Icons/Windows11/fav.bmp";
                }
                else
                {
                    bookmarkIcon = value;
                }
                RaisePropertyChanged(nameof(BookmarkIcon));
            }
        }
        public string bookmarkDateText;
        public string BookmarkDateText
        {
            get
            {
                return bookmarkDateText;
            }
            set
            {
                bookmarkDateText = value;
                RaisePropertyChanged(nameof(BookmarkDateText));
            }
        }
        public long BookmarkDate;
        public int Clicks;

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
        public Bookmark(string title, string url, string icon = "ms-appx:///Assets/Icons/Windows11/fav.bmp")
        {
            try
            {
                if (title != null)
                {
                    BookmarkTitle = title.Trim();
                    BookmarkURL = url.Trim();
                    if (icon == null || icon.Length == 0)
                    {
                        BookmarkIcon = "ms-appx:///Assets/Icons/Windows11/fav.bmp";
                    }
                    else
                    {
                        BookmarkIcon = icon;
                    }

                    long date = DateTime.Now.Ticks;
                    BookmarkDate = date;
                    BookmarkDateText = DateTime.Now.ToLocalTime().ToString();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
