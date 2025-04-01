/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.AppInstaller;

namespace Commander.Native
{
    public static class Tasksc
    {
        public static CTask Task(string taskID, string processID)
        {
            CTask cTask = new CTask(taskID, processID, true);
            return cTask;
        }

        public static CTask Task(string taskID, string processID, string finishProcessID)
        {
            CTask cTask = new CTask(taskID, processID, finishProcessID, true);
            return cTask;
        }

        public static CTask Task(string taskID, string processID, string startProcessID, string finishProcessID)
        {
            CTask cTask = new CTask(taskID, processID, startProcessID, finishProcessID, true);
            return cTask;
        }

        public static CTimer Timer(string timerID, string processID, string scriptID, int interval)
        {
            CTimer cTimer = new CTimer(timerID, processID, scriptID, interval, true);
            return cTimer;
        }

        public static CTimer Timer(string timerID, string processID, int interval, string finishProcessID)
        {
            CTimer cTimer = new CTimer(timerID, processID, interval, finishProcessID, true);
            return cTimer;
        }

        public static CTimer Timer(string timerID, string processID, int interval, string startProcessID, string finishProcessID)
        {
            CTimer cTimer = new CTimer(timerID, processID, interval, startProcessID, finishProcessID, true);
            return cTimer;
        }

        public static CProcess Process(string processID, string command, string telnetID, ScriptInstaller scriptInstaller = null)
        {
            CProcess cProcess = new CProcess(processID, command, telnetID, scriptInstaller,  true);
            return cProcess;
        }

        public static CProcess Process(string processID, string command, string telnetID, string finishProcessID)
        {
            CProcess cProcess = new CProcess(processID, command, telnetID, finishProcessID, true);
            return cProcess;
        }

        public static CProcess Process(string processID, string command, string telnetID, string startProcessID, string finishProcessID)
        {
            CProcess cProcess = new CProcess(processID, command, telnetID, startProcessID, finishProcessID, true);
            return cProcess;
        }
    }
}
