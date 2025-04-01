/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.UI.Core;

namespace Commander.Compiler
{
    public class CTimer
    {
        public string TimerID;
        public int Interval;
        public string ProcessID;
        public string FinishProcessID;
        public string StartProcessID;
        public string ScriptID;
        public Timer ProcessTimer;

        public CTimer(string timerID, string processID, string scriptID, int interval, bool append = false)
        {
            TimerID = timerID;
            Interval = interval;
            ProcessID = processID;
            ScriptID = scriptID;

            if (append)
            {
                Append();
            }
        }

        public CTimer(string timerID, string processID, int interval, string finishProcessID, bool append = false)
        {
            TimerID = timerID;
            Interval = interval;
            ProcessID = processID;
            FinishProcessID = finishProcessID;

            if (append)
            {
                Append();
            }
        }

        public CTimer(string timerID, string processID, int interval, string startProcessID, string finishProcessID, bool append = false)
        {
            TimerID = timerID;
            Interval = interval;
            ProcessID = processID;
            StartProcessID = startProcessID;
            FinishProcessID = finishProcessID;

            if (append)
            {
                Append();
            }
        }

        public async void Start()
        {
            try
            {
                TimerInProgress = false;
                ProcessTimer?.Dispose();
                await onStart();
                ProcessTimer = new Timer(delegate { TimerTick(null, EventArgs.Empty); }, null, 0, Interval);
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
            }
        }

        CProcess targetProcess;
        bool TimerInProgress = false;
        public async void TimerTick(object sender, EventArgs e)
        {
            if (!TimerInProgress)
            {
                TimerInProgress = true;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        targetProcess = CCache.GetProcessCache(ProcessID);
                        await targetProcess.Execute();
                    }
                    catch
                    {

                    }
                    TimerInProgress = false;
                });
            }
        }

        public void Cancel()
        {
            if (targetProcess != null)
            {
                targetProcess.Cancel();
            }
        }

        public async void Stop(bool deleteRequest = false)
        {
            try
            {
                ProcessTimer?.Dispose();
                TimerInProgress = false;
                if (!deleteRequest)
                {
                    await onFinish();
                    Cancel();
                }
                else
                {
                    Cancel();
                }
            }
            catch
            {
            }
        }

        public async Task onStart()
        {
            if (StartProcessID != null && StartProcessID.Length > 0)
            {
                var targetProcess = CCache.GetProcessCache(StartProcessID);
                await targetProcess.Execute();
            }
        }

        public async Task onFinish()
        {
            if (FinishProcessID != null && FinishProcessID.Length > 0)
            {
                var targetProcess = CCache.GetProcessCache(FinishProcessID);
                await targetProcess.Execute();
            }
        }

        public void Append()
        {
            CTimer outPutTest = null;
            CCache.TimersCache.TryGetValue(TimerID, out outPutTest);
            lock (CCache.TimersCache)
            {
                if (outPutTest == null)
                {
                    CCache.TimersCache.Add(TimerID, this);
                }
                else
                {
                    CCache.TimersCache[TimerID] = this;
                }
            }
        }
    }
}
