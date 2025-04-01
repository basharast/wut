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
    public class Keywords
    {
        public string Keyword;
        public string FullText;
        public string Description;
        public string URL;
        public string Icon;
        public string KeywordType;
        public int Clicks;
        [JsonIgnore]
        Visibility ItemsIconsBlock
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
        public Keywords(string keyword, string fullText, string description, string url, string icon = "ms-appx:///Assets/Icons/Windows11/browser.bmp", string keywordType="", int clicks = 0)
        {
            if (keyword != null)
            {
                Keyword = keyword;
                FullText = fullText;
                Description = description;
                URL = url;
                Icon = icon;
                KeywordType = keywordType;
                Clicks = clicks;
            }
        }
    }
}
