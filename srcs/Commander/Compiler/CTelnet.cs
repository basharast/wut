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
using WinUniversalTool.Telnet;

namespace Commander.Compiler
{
    public class CTelnet
    {
        public string TelnetID;
        public string Host;
        public int Port;
        public string FinishProcessID;
        public string StartProcessID;
        public string CloseProcessID;
        public string TUser = "";
        public string TPass = "";

        public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient telnetClient = null;

        public CTelnet(string telnetID, string host, string port, bool append = false)
        {
            TelnetID = telnetID;
            Host = host;
            Port = 23;
            try
            {
                Port = Int16.Parse(port);
            }
            catch
            {

            }
            TUser = Helpers.TUser;
            TPass = Helpers.TPass;
            if (append)
            {
                CTelnet outPutTest = null;
                CCache.TelnetsCache.TryGetValue(TelnetID, out outPutTest);
                lock (CCache.TelnetsCache)
                {
                    if (outPutTest == null)
                    {
                        CCache.TelnetsCache.Add(TelnetID, this);
                    }
                    else
                    {
                        CCache.TelnetsCache[TelnetID] = this;
                    }
                }
            }
        }

        public CTelnet(string telnetID, string host, string port, bool append = false, string tuser = "", string tpass = "")
        {
            TelnetID = telnetID;
            Host = host;
            Port = 23;
            try
            {
                Port = Int16.Parse(port);
            }
            catch
            {

            }
            TUser = tuser;
            TPass = tpass;
            if (append)
            {
                CTelnet outPutTest = null;
                CCache.TelnetsCache.TryGetValue(TelnetID, out outPutTest);
                if (outPutTest == null)
                {
                    CCache.TelnetsCache.Add(TelnetID, this);
                }
                else
                {
                    CCache.TelnetsCache[TelnetID] = this;
                }
            }
        }

        public CTelnet(string telnetID, string host, string port, string finishProcessID, bool append = false)
        {
            TelnetID = telnetID;
            Host = host;
            Port = 23;
            try
            {
                Port = Int16.Parse(port);
            }
            catch
            {

            }
            TUser = Helpers.TUser;
            TPass = Helpers.TPass;
            if (append)
            {
                CTelnet outPutTest = null;
                CCache.TelnetsCache.TryGetValue(TelnetID, out outPutTest);
                if (outPutTest == null)
                {
                    CCache.TelnetsCache.Add(TelnetID, this);
                }
                else
                {
                    CCache.TelnetsCache[TelnetID] = this;
                }
            }
            FinishProcessID = finishProcessID;
        }

        public CTelnet(string telnetID, string host, string port, string startProcessID, string finishProcessID, bool append = false)
        {
            TelnetID = telnetID;
            Host = host;
            Port = 23;
            try
            {
                Port = Int16.Parse(port);
            }
            catch
            {

            }
            TUser = Helpers.TUser;
            TPass = Helpers.TPass;
            if (append)
            {
                CTelnet outPutTest = null;
                CCache.TelnetsCache.TryGetValue(TelnetID, out outPutTest);
                if (outPutTest == null)
                {
                    CCache.TelnetsCache.Add(TelnetID, this);
                }
                else
                {
                    CCache.TelnetsCache[TelnetID] = this;
                }
            }
            StartProcessID = startProcessID;
            FinishProcessID = finishProcessID;
        }

        public async Task Create()
        {
            cancellationTokenSource = new CancellationTokenSource();
            telnetClient = new TelnetClient(TimeSpan.FromSeconds(3), cancellationTokenSource.Token);

            telnetClient.ConnectionClosed += HandleConnectionClosed;
            telnetClient.MessageReceived += HandleMessageReceived;
            telnetClient.ErrorReceived += HandleErrorReceived;

            await Log($"Connecting to '{Host}:{Port}'..");

            await telnetClient.Connect(Host, Port);

            await Log($"Telnet '{TelnetID}' started");

            if (TUser.Trim().Length > 0)
            {
                await telnetClient.Send(TUser.Trim());
            }
            if (TPass.Trim().Length > 0)
            {
                await telnetClient.Send(TPass.Trim());
            }
            await onStart();
        }

        public async Task Send(string command)
        {
            await telnetClient.Send(command);
            await Log($"Send: {command}");
        }

        private async void HandleErrorReceived(object sender, string e)
        {
            await Log($"Error: {e}");
        }

        private async void HandleMessageReceived(object sender, string e)
        {
            await Log($"Received: {e}");
            await onFinish();
        }

        private async void HandleConnectionClosed(object sender, EventArgs e)
        {
            await Log("Connection Closed");
        }

        public async void Cancel()
        {
            try
            {
                cancellationTokenSource.Cancel();
                await Log("Cancel request sent");
            }
            catch
            {

            }
        }

        public async void Close()
        {
            try
            {
                telnetClient.ConnectionClosed -= HandleConnectionClosed;
                telnetClient.MessageReceived -= HandleMessageReceived;
                telnetClient.ErrorReceived -= HandleErrorReceived;
                telnetClient.Disconnect();
                telnetClient.Dispose();
                await Log($"Telnet '{TelnetID}' closed");
                await onClose();
            }
            catch
            {

            }
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
        public async Task onClose()
        {
            var targetProcess = CCache.GetProcessCache(CloseProcessID);
            await targetProcess.Execute();

        }

        public async Task Reconnect()
        {
            Cancel();
            Close();
            await Create();
            await Log($"Telnet '{TelnetID}' reconnected");
        }

        public async Task Log(string message)
        {
            Helpers.Logger(message);
        }
    }
}
