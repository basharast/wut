/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;

namespace WebViewComponents
{
    public class ConsoleOutput : EventArgs
    {
        public string type;
        public string message;
        public string json;
        public ConsoleOutput(string type, string message, string json = "")
        {
            this.type = type;
            this.message = Uri.UnescapeDataString(message);
            this.json = json;
        }
    }
}
