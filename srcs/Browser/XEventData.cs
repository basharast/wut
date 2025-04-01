/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class XEventData : EventArgs
    {
        public string type;
        public string data;
        public string extra;
        public string file;

        public XEventData(string type, string data = "", string extra = "web", string file = "txt")
        {
            this.type = type;
            this.data = data;
            this.extra = extra;
            this.file = file;
        }
    }
}
