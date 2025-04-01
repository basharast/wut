/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices.Gpio;
using WinUniversalTool.Settings;

namespace Commander.Compiler
{
    public static class CCache
    {
        //Script
        public static Dictionary<string, CLine> ScriptCache = new Dictionary<string, CLine>();

        //Storage
        public static Dictionary<string, CFolder> FoldersCache = new Dictionary<string, CFolder>();
        public static Dictionary<string, CFile> FilesCache = new Dictionary<string, CFile>();
        public static Dictionary<string, CFiles> MFilesCache = new Dictionary<string, CFiles>();

        //Values
        public static Dictionary<string, CVariable> VariablesCache = new Dictionary<string, CVariable>();
       
        //Forms
        public static Dictionary<string, ObservableCollection<SettingsTab>> ContainerCache = new Dictionary<string, ObservableCollection<SettingsTab>>();
        public static Dictionary<string, SettingsTab> TabsCache = new Dictionary<string, SettingsTab>();

        //Tasks
        public static Dictionary<string, CTask> TasksCache = new Dictionary<string, CTask>();
        public static Dictionary<string, CTimer> TimersCache = new Dictionary<string, CTimer>();
        public static Dictionary<string, CProcess> ProcessesCache = new Dictionary<string, CProcess>();

        //Telnet
        public static Dictionary<string, CTelnet> TelnetsCache = new Dictionary<string, CTelnet>();

        //Getters
        public static CLine GetLineCache(string lineID)
        {
            CLine LineCache = null;
            ScriptCache.TryGetValue(lineID, out LineCache);
            return LineCache;
        }

        public static CFolder GetFolderCache(string folderID)
        {
            //folderID = folderID.Replace("\"","");
            CFolder FolderCache = null;
            FoldersCache.TryGetValue(folderID, out FolderCache);
            return FolderCache;
        }

        public static CFile GetFileCache(string fileID)
        {
            CFile FileCache = null;
            FilesCache.TryGetValue(fileID, out FileCache);
            return FileCache;
        }

        public static CFiles GetFilesCache(string filesID)
        {
            CFiles FilesCache = null;
            MFilesCache.TryGetValue(filesID, out FilesCache);
            return FilesCache;
        }

        public static CVariable GetVariableCache(string variableID)
        {
            CVariable VariableCache = null;
            VariablesCache.TryGetValue(variableID, out VariableCache);
            return VariableCache;
        }
        public static void SetTabsCache(string variableID, SettingsTab settingsTab)
        {
            SettingsTab outPutTest = null;
            CCache.TabsCache.TryGetValue(variableID, out outPutTest);
            lock (CCache.TabsCache)
            {
                if (outPutTest == null)
                {
                    CCache.TabsCache.Add(variableID, settingsTab);
                }
                else
                {
                    CCache.TabsCache[variableID] = settingsTab;
                }
            }
        }
        public static void SetContainerCache(string variableID, ObservableCollection<SettingsTab> settingsTabs)
        {
            ObservableCollection<SettingsTab> outPutTest = null;
            CCache.ContainerCache.TryGetValue(variableID, out outPutTest);
            lock (CCache.ContainerCache)
            {
                if (outPutTest == null)
                {
                    CCache.ContainerCache.Add(variableID, settingsTabs);
                }
                else
                {
                    CCache.ContainerCache[variableID] = settingsTabs;
                }
            }
        }
        public static ObservableCollection<SettingsTab> GetContainerCache(string variableID)
        {
            ObservableCollection<SettingsTab> VariableCache = null;
            ContainerCache.TryGetValue(variableID, out VariableCache);
            return VariableCache;
        }

        public static SettingsTab GetTabCache(string variableID)
        {
            SettingsTab VariableCache = null;
            TabsCache.TryGetValue(variableID, out VariableCache);
            return VariableCache;
        }

        public static CTask GetTaskCache(string taskID)
        {
            CTask TaskCache = null;
            TasksCache.TryGetValue(taskID, out TaskCache);
            return TaskCache;
        }

        public static CTimer GetTimerCache(string timerID)
        {
            CTimer TimerCache = null;
            TimersCache.TryGetValue(timerID, out TimerCache);
            return TimerCache;
        }

        public static CProcess GetProcessCache(string processID)
        {
            CProcess ProcessCache = null;
            ProcessesCache.TryGetValue(processID, out ProcessCache);
            return ProcessCache;
        }

        public static CTelnet GetTelnetCache(string telnetID)
        {
            CTelnet TelnetCache = null;
            TelnetsCache.TryGetValue(telnetID, out TelnetCache);
            return TelnetCache;
        }
    }
}
