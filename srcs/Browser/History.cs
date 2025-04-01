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

namespace WinUniversalTool.WebViewer
{
    public class History
    {
        public string HistoryTitle;
        public string HistoryTitleShort {
            get
            {
                if (HistoryTitle.Length > 50)
                {
                    return $"{HistoryTitle.Substring(0, 35)}..";
                }
                else
                {
                    return HistoryTitle;
                }
            }
        }
        public string HistoryURL;
        string historyIcon;
        public string HistoryIcon
        {
            get
            {
                if (historyIcon != null && historyIcon.Length > 0 || historyIcon.Contains(".svg") || historyIcon.Contains(WebHelper.EMPTY_FAVICON))
                {
                    return historyIcon;
                }
                else
                {
                    return "ms-appx:///Assets/Icons/Windows11/history.bmp";
                }
            }
            set
            {
                if (value == null || value.Length == 0 || value.Contains(".svg") || value.Contains(WebHelper.EMPTY_FAVICON))
                {
                    historyIcon = "ms-appx:///Assets/Icons/Windows11/history.bmp";
                }
                else
                {
                    historyIcon = value;
                }
            }
        }

        [JsonIgnore]
        public string HistoryGroup
        {
            get
            {
                try
                {
                    var currentDay = new DateTime(HistoryDate);
                    var currentDayString = $"{currentDay.Day}-{currentDay.Month}-{currentDay.Year}";
                    return currentDayString;
                }
                catch(Exception ex)
                {
                    return "Now";
                }
            }
        }
        [JsonIgnore]
        public bool Collapsed = false;
        [JsonIgnore]
        public string Key
        {
            get
            {
                return HistoryGroup;
            }
        }
        public string HistoryDateText;
        public long HistoryDate;
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
        public History(string title, string url, string icon = "ms-appx:///Assets/Icons/Windows11/history.png")
        {
            try
            {
                if (title != null)
                {
                    HistoryTitle = title;
                    HistoryURL = url;
                    if (icon.Length == 0)
                    {
                        HistoryIcon = "ms-appx:///Assets/Icons/Windows11/history.png";
                    }
                    else
                    {
                        HistoryIcon = icon;
                    }
                    long date = DateTime.Now.Ticks;
                    HistoryDate = date;
                    HistoryDateText = DateTime.Now.ToLocalTime().ToString();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
