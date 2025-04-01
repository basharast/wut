/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Native
{
    public static class Events
    {
        public static void Start(string targetID, string processID)
        {
            CProcess cProcess = null;
            CTask cTask = null;
            CTimer cTimer = null;
            CTelnet cTelnet = null;

            if (CCache.ProcessesCache.TryGetValue(targetID, out cProcess))
            {
                cProcess.StartProcessID = processID;
            }
            else if (CCache.TasksCache.TryGetValue(targetID, out cTask))
            {
                cTask.StartProcessID = processID;
            }
            else if (CCache.TimersCache.TryGetValue(targetID, out cTimer))
            {
                cTimer.StartProcessID = processID;
            }
            else if (CCache.TelnetsCache.TryGetValue(targetID, out cTelnet))
            {
                cTelnet.StartProcessID = processID;
            }
        }

        public static void Finish(string targetID, string processID)
        {
            CProcess cProcess = null;
            CTask cTask = null;
            CTimer cTimer = null;
            CTelnet cTelnet = null;

            if (CCache.ProcessesCache.TryGetValue(targetID, out cProcess))
            {
                cProcess.FinishProcessID = processID;
            }
            else if (CCache.TasksCache.TryGetValue(targetID, out cTask))
            {
                cTask.FinishProcessID = processID;
            }
            else if (CCache.TimersCache.TryGetValue(targetID, out cTimer))
            {
                cTimer.FinishProcessID = processID;
            }
            else if (CCache.TelnetsCache.TryGetValue(targetID, out cTelnet))
            {
                cTelnet.FinishProcessID = processID;
            }
        }

        public static void Close(string targetID, string processID)
        {
            CCache.TelnetsCache[targetID].CloseProcessID = processID;
        }

        public static void Click(string targetID, string processID)
        {

        }
    }
}
