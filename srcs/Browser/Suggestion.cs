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
    public class Suggestion : BindableBase
    {
        public string SuggestionTitle;
        public string SuggestionDescription;
        public string SuggestionIcon;
        public string SuggestionExtra;
        public string SuggestionURL;
        public string SuggestionType;
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
        public Suggestion(string url, string title, string description="", string icon = "ms-appx:///Assets/Icons/Windows11/lnk.bmp", string extra = "", string type = "O", int clicks = 0)
        {
            if (url != null)
            {
                SuggestionURL = url;
                SuggestionTitle = title;
                SuggestionDescription = description;
                SuggestionIcon = icon;
                SuggestionExtra = extra;
                SuggestionType = type;
                Clicks = clicks;
            }
        }
        public void UpdateBinding()
        {
            try
            {
                RaisePropertyChanged(nameof(SuggestionIcon));
            }catch(Exception ex)
            {

            }
        }
    }
}
