/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace WinUniversalTool.Models
{
    public class Queue
    {
        public ObservableCollection<QueueItem> Downloads;
        public ObservableCollection<QueueItem> DownloadsGroup;
        public int maxTasksAtSameTime = 1;
        public int MaxTasksAtSameTime
        {
            get
            {
                return maxTasksAtSameTime;
            }
            set
            {
                maxTasksAtSameTime = value;
            }
        }
        public int CurrentActiveTasks
        {
            get
            {
                int currentActiveTasks = 0;
                foreach (var QueueTask in Downloads)
                {
                    if (!QueueTask.isDownloadDone && !QueueTask.isDownloadFailed && !QueueTask.isDownloadCanceled && !QueueTask.isCancelingRequestInProgress && QueueTask.isStarted)
                    {
                        currentActiveTasks++;
                    }
                }
                foreach (var QueueTask in DownloadsGroup)
                {
                    if (!QueueTask.isDownloadDone && !QueueTask.isDownloadFailed && !QueueTask.isDownloadCanceled && !QueueTask.isCancelingRequestInProgress && QueueTask.isStarted)
                    {
                        currentActiveTasks++;
                    }
                }
                return currentActiveTasks;
            }
        }
        public Queue()
        {
            Downloads = new ObservableCollection<QueueItem>();
            DownloadsGroup = new ObservableCollection<QueueItem>();
        }

        public async void AddQueue(QueueItem queueItem, bool FolderDownload = false, bool ShowAddMessage = true, string CertificateName = "")
        {
            bool ignoreGroupDownlodsState = true;
            if (FolderDownload && !ignoreGroupDownlodsState)
            {
                try
                {
                    foreach (var DownloadItem in DownloadsGroup)
                    {
                        if (queueItem.FileName.Equals(DownloadItem.FileName) && (!DownloadItem.isDownloadCanceled && !DownloadItem.isDownloadFailed && !DownloadItem.isDownloadDone && !DownloadItem.isCancelingRequestInProgress && !DownloadItem.isDownloading))
                        {
                            return;
                        }
                    }
                }
                catch (Exception e)
                {

                }
                try
                {
                    DownloadsGroup.Add(queueItem);
                    if (!FolderDownload && ShowAddMessage)
                    {
                        Helpers.PlayNotificationSoundDirect("alert.mp3");

                        string DialogTitle = "Downloads Queue";
                        string DialogMessage = $"{queueItem.FileName} added to the queue";
                        string[] DialogButtons = new string[] { "Close" };

                        if (CertificateName.Length > 0)
                        {
                            DialogMessage = $"{queueItem.FileName}\n{CertificateName}\nAdded to the queue";
                        }

                        LocalNotificationData localNotificationData = new LocalNotificationData();
                        localNotificationData.icon = SegoeMDL2Assets.DownloadLegacy;
                        localNotificationData.type = Colors.DodgerBlue;
                        localNotificationData.time = 2;
                        localNotificationData.message = DialogMessage;
                        Helpers.pushLocalNotification(null, localNotificationData);
                        /*var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var QuestionResult = QuestionDialog.ShowAsync2();*/

                    }
                }
                catch (Exception e)
                {
                    _ = Helpers.ShowCatchedErrorAsync(e);
                }
            }
            else
            {
                try
                {
                    foreach (var DownloadItem in Downloads)
                    {
                        if (queueItem.FileName.Equals(DownloadItem.FileName) && (!DownloadItem.isDownloadCanceled && !DownloadItem.isDownloadFailed && !DownloadItem.isDownloadDone && !DownloadItem.isCancelingRequestInProgress && !DownloadItem.isDownloading))
                        {
                            return;
                        }
                    }
                }
                catch (Exception e)
                {

                }
                try
                {
                    Downloads.Insert(0, queueItem);
                    if (!FolderDownload && ShowAddMessage)
                    {
                        Helpers.PlayNotificationSoundDirect("alert.mp3");
                        string DialogTitle = "Downloads Queue";
                        string DialogMessage = $"{queueItem.FileName} added to the queue";
                        string[] DialogButtons = new string[] { "Close" };

                        if (CertificateName.Length > 0)
                        {
                            DialogMessage = $"{queueItem.FileName}\n{CertificateName}\nAdded to the queue";
                        }
                        LocalNotificationData localNotificationData = new LocalNotificationData();
                        localNotificationData.icon = SegoeMDL2Assets.DownloadLegacy;
                        localNotificationData.type = Colors.DodgerBlue;
                        localNotificationData.time = 2;
                        localNotificationData.message = DialogMessage;
                        Helpers.pushLocalNotification(null, localNotificationData);
                        /*var QuestionDialog = Helpers.CreateDialog(DialogTitle, DialogMessage, DialogButtons);
                        var QuestionResult = QuestionDialog.ShowAsync2();*/

                    }
                }
                catch (Exception e)
                {
                    _ = Helpers.ShowCatchedErrorAsync(e);
                }
            }
        }
    }
}
