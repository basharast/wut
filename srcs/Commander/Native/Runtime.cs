/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace Commander.Native
{
    public static class Runtime
    {
        public static async Task Delay(int delayTime)
        {
            await Task.Delay(delayTime * 1000);
        }

        public static void Break()
        {

        }

        public static void Stop(string targetID)
        {
            CProcess cProcess = null;
            CTask cTask = null;
            CTimer cTimer = null;
            CTelnet cTelnet = null;

            if(CCache.ProcessesCache.TryGetValue(targetID ,out cProcess))
            {
                cProcess.Cancel();
            }
            else if (CCache.TasksCache.TryGetValue(targetID, out cTask))
            {
                cTask.Cancel();
            }
            else if (CCache.TimersCache.TryGetValue(targetID, out cTimer))
            {
                cTimer.Stop();
            }
            else if (CCache.TelnetsCache.TryGetValue(targetID, out cTelnet))
            {
                cTelnet.Close();
            }
        }

        public static async Task Start(string targetID)
        {
            CProcess cProcess = null;
            CTask cTask = null;
            CTimer cTimer = null;
            CTelnet cTelnet = null;
            //Helpers.Logger(targetID);
            if (CCache.ProcessesCache.TryGetValue(targetID, out cProcess))
            {
                await cProcess.Execute();
            }
            else if (CCache.TasksCache.TryGetValue(targetID, out cTask))
            {
                cTask.Start();
            }
            else if (CCache.TimersCache.TryGetValue(targetID, out cTimer))
            {
                //Helpers.Logger($"{targetID} Timer");
                cTimer.Start();
            }
            else if (CCache.TelnetsCache.TryGetValue(targetID, out cTelnet))
            {
                await cTelnet.Reconnect();
            }
            else
            {
                Helpers.Logger($"{targetID} Not Found");
            }
        }

        public static void Close(string telnetID)
        {
            CTelnet cTelnet = CCache.GetTelnetCache(telnetID);
            cTelnet.Close();
        }
    }
}
