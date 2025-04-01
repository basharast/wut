/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.WebViewer
{
    public class FirefoxBookmark
    {
        public string guid;
        public string title;
        public long dateAdded;
        public children[] children;
    }

    public class children
    {
        public string guid;
        public string title;
        public long dateAdded;

        [JsonProperty("children")]
        public children1[] children1;
    }
    public class children1
    {
        public string guid;
        public string title;
        public long dateAdded;
        public string iconuri;
        public string uri;
    }
}
