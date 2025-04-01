/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace WebViewComponents
{
    [AllowForWeb]
    public sealed class WebComponents
    {
        public WebComponents(object message)
        {
            var messageConverted = (string)message;
            if (messageConverted != null)
            {

            }
        }
        public void callMe()
        {
            object message = null;
            var messageConverted = (string)message;
            if (messageConverted != null)
            {

            }
        }
    }
}
