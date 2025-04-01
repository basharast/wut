/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using SharedLibrary;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;

namespace WebViewComponents
{
    [AllowForWeb]
    public sealed class WindowOverride
    {
        public void loaded()
        {
            try
            {
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("loaded");
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void menu()
        {
            try
            {
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("menu");
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void tabs()
        {
            try
            {
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("tabs");
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void address(bool show)
        {
            try
            {
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData(show?"showa":"hidea");
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void dblclick()
        {
            try
            {
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("dblclick");
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void save(object data)
        {
            try
            {
                var dataString = (string)data;
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("save", dataString);
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void save(object data, object extra)
        {
            try
            {
                var dataString = (string)data;
                var extraString = (string)extra;
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("save", dataString, extraString);
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void save(object data, object extra, object file)
        {
            try
            {
                var dataString = (string)data;
                var extraString = (string)extra;
                var fileString = (string)file;
                if (StaticHandlers.XEvents != null)
                {
                    XEventData xEventData = new XEventData("save", dataString, extraString, fileString);
                    StaticHandlers.XEvents.Invoke(null, xEventData);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void notify(object message)
        {
            try
            {
                var messageString = (string)message;
                var typeString = "toast";
                if (messageString != null)
                {
                    NotifyData notifyData = new NotifyData(typeString, messageString);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, notifyData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void notify(object title, object message)
        {
            try
            {
                var titleString = (string)title;
                var messageString = (string)message;
                var typeString = "toast";
                if (messageString != null)
                {
                    NotifyData notifyData = new NotifyData(typeString, messageString, titleString);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, notifyData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void notify(object title, object message, int time)
        {
            try
            {
                var titleString = (string)title;
                var messageString = (string)message;
                var typeString = "toast";
                if (messageString != null)
                {
                    NotifyData notifyData = new NotifyData(typeString, messageString, titleString, time);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, notifyData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }  
        
        //Usage notifyl("normal|icon", "message", 5)
        public void notifyl(object type, object message, int time)
        {
            try
            {
                var titleString = (string)type;
                var messageString = (string)message;
                var typeString = "toastl";
                if (messageString != null)
                {
                    NotifyData notifyData = new NotifyData(typeString, messageString, titleString, time);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, notifyData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
