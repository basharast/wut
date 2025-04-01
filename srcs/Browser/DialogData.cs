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
    public class DialogData : EventArgs
    {
        public string message;
        public string defaultText;
        public string type;

        public DialogData(string message, string type, string defaultText="")
        {
            this.message = message;
            this.defaultText = defaultText;
            this.type = type;
        }
    }
}
