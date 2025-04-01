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
    public class NotifyData : EventArgs
    {
        public string type;
        public string data;
        public string extra;
        public int time;

        public NotifyData(string type, string data, string extra = "", int time = 30)
        {
            this.type = type;
            this.data = data;
            this.extra = extra;
            this.time = time;
        }
    }
}
