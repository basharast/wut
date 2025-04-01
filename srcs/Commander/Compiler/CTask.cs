/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Commander.Compiler
{
    public class CTask
    {
        public string TaskID;
        public string ProcessID;
        public string StartProcessID;
        public string FinishProcessID;

        public CTask(string taskID, string processID, bool append = false)
        {
            TaskID = taskID;
            ProcessID = processID;

            if (append)
            {
                Append();
            }
        }

        public CTask(string taskID, string processID, string finsihProcessID, bool append = false)
        {
            TaskID = taskID;
            ProcessID = processID;
            FinishProcessID = finsihProcessID;

            if (append)
            {
                Append();
            }
        }

        public CTask(string taskID, string processID, string startProcessID, string finsihProcessID, bool append = false)
        {
            TaskID = taskID;
            ProcessID = processID;
            FinishProcessID = finsihProcessID;
            StartProcessID = startProcessID;

            if (append)
            {
                Append();
            }
        }

        CProcess targetProcess;
        public void Start()
        {
            targetProcess = CCache.GetProcessCache(ProcessID);
            Task.Factory.StartNew(async () =>
            {
                await onStart();
                await targetProcess.Execute();
                await onFinish();
            });
        }

        public void Cancel()
        {
            targetProcess.Cancel();
        }

        public async Task onStart()
        {
            var targetProcess = CCache.GetProcessCache(StartProcessID);
            await targetProcess.Execute();

        }
        public async Task onFinish()
        {
            var targetProcess = CCache.GetProcessCache(FinishProcessID);
            await targetProcess.Execute();

        }
        public void Append()
        {
            CTask outPutTest = null;
            CCache.TasksCache.TryGetValue(TaskID, out outPutTest);
            lock (CCache.TasksCache)
            {
                if (outPutTest == null)
                {
                    CCache.TasksCache.Add(TaskID, this);
                }
                else
                {
                    CCache.TasksCache[TaskID] = this;
                }
            }
        }
    }
}
