using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace WinUniversalTool.WebViewer
{
    public static class DispatcherHelper
    {
        public static Task RunOnUIThreadAsync(Action action)
        {
            return RunOnUIThreadAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, action);
        }

        public static async Task RunOnUIThreadAsync(Windows.UI.Core.CoreDispatcherPriority priority, Action action)
        {
            try
            {
                await returnDispatcher().RunAsync(priority, () =>
                {
                    action();
                });
            }
            catch (Exception ex)
            {
               
            }
        }

        public static Windows.UI.Core.CoreDispatcher returnDispatcher()
        {
            return (Windows.UI.Xaml.Window.Current == null) ?
                CoreApplication.MainView.CoreWindow.Dispatcher :
                CoreApplication.GetCurrentView().CoreWindow.Dispatcher;
        }
    }
}
