/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Command;
using Commander.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.AppInstaller;
using WinUniversalTool.Models;

namespace Commander.Compiler
{
    public class CProcess
    {
        public string ProcessID;
        public string StartProcessID;
        public string FinishProcessID;
        public string ProcessCommand;
        public string TelnetID;
        public ScriptInstaller scriptInstaller;

        public CProcess(string processID, string processCommand, string telnetID, bool append = false)
        {
            ProcessID = processID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            ProcessCommand = processCommand;
            TelnetID = telnetID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");

            if (append)
            {
                Append();
            }
        }
        public CProcess(string processID, string processCommand, string telnetID, ScriptInstaller ScriptInstaller, bool append = false)
        {
            ProcessID = processID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            ProcessCommand = processCommand;
            TelnetID = telnetID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            scriptInstaller = ScriptInstaller;
            if (append)
            {
                Append();
            }
        }

        public CProcess(string processID, string processCommand, string telnetID, string finishProcessID, bool append = false)
        {
            ProcessID = processID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            ProcessCommand = processCommand;
            TelnetID = telnetID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            FinishProcessID = finishProcessID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");

            if (append)
            {
                Append();
            }
        }

        public CProcess(string processID, string processCommand, string telnetID, string startProcessID, string finishProcessID, bool append = false)
        {
            ProcessID = processID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            ProcessCommand = processCommand;
            TelnetID = telnetID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            StartProcessID = startProcessID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            FinishProcessID = finishProcessID.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");

            if (append)
            {
                Append();
            }
        }

        public void Append()
        {
            CProcess outPutTest = null;
            CCache.ProcessesCache.TryGetValue(ProcessID, out outPutTest);
            lock (CCache.ProcessesCache)
            {
                if (outPutTest == null)
                {
                    CCache.ProcessesCache.Add(ProcessID, this);
                }
                else
                {
                    CCache.ProcessesCache[ProcessID] = this;
                }
            }
        }

        public async Task<bool> Execute()
        {
            try
            {
                await onStart();

                if (TelnetID.Length == 0 || TelnetID.Equals("0"))
                {
                    ProcessCommand = await scriptInstaller?.ReplaceHandlers(ProcessCommand);
                    var executeResult = await scriptInstaller?.FetchHandlerResult(ProcessCommand);
                    if (executeResult)
                    {
                        return executeResult;
                    }
                    else
                    if (ProcessCommand.StartsWith("set "))
                    {
                        CCommand SET = new CCommand("set", new string[] { "variable", "[property]", "value" }, "set variable property, used with objects", true);

                        var Data = SET.ExtractData(ProcessCommand);
                        if (Data.Count > 0)
                        {
                            var variable = Data[SET.CommandOptions[0].OptionName];
                            var value = Data[SET.CommandOptions[2].OptionName];
                            //Helpers.Logger($"{variable} -> {value}");
                            switch (value)
                            {
                                case "++":
                                    try
                                    {
                                        var currentValue = int.Parse(Variables.Get(variable));
                                        currentValue++;
                                        Variables.Set(variable, currentValue.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Variables.Set(variable, value);
                                        Helpers.Logger(ex);
                                    }
                                    break;

                                case "--":
                                    try
                                    {
                                        var currentValue = int.Parse(Variables.Get(variable));
                                        currentValue--;
                                        Variables.Set(variable, currentValue.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Variables.Set(variable, value);
                                        Helpers.Logger(ex);
                                    }
                                    break;

                                default:
                                    try
                                    {
                                        if (value.StartsWith("+"))
                                        {
                                            var targetValue = int.Parse(value.Replace("+", ""));
                                            var currentValue = int.Parse(Variables.Get(variable));
                                            currentValue += targetValue;
                                            Variables.Set(variable, currentValue.ToString());
                                        }
                                        else if (value.StartsWith("-"))
                                        {
                                            var targetValue = int.Parse(value.Replace("-", ""));
                                            var currentValue = int.Parse(Variables.Get(variable));
                                            currentValue -= targetValue;
                                            Variables.Set(variable, currentValue.ToString());
                                        }
                                        else
                                        {
                                            Variables.Set(variable, value);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Helpers.Logger(ex);
                                    }
                                    break;
                            }

                        }
                        else if (ProcessCommand.StartsWith("notify "))
                        {
                            SET = new CCommand("notify", new string[] { "title", "message", "timeout" }, "Push notification with custom timeout, delay true-false will hold the script until notification pressed", true);
                            Data = SET.ExtractData(ProcessCommand);
                            if (Data.Count > 0)
                            {
                                var title = Data[SET.CommandOptions[0].OptionName];
                                var message = Data[SET.CommandOptions[1].OptionName];
                                var timeout = Data[SET.CommandOptions[2].OptionName];
                                var timeOutInt = 15;
                                try
                                {
                                    timeOutInt = int.Parse(timeout);
                                }
                                catch (Exception ex)
                                {

                                }
                                Helpers.ShowToastNotification(title, message, timeOutInt);
                            }

                        }
                        else if (ProcessCommand.StartsWith("info "))
                        {
                            SET = new CCommand("info", new string[] { "info", "message", "button" }, "Show info dialog, will return button value or null on backpressed", true);

                            Data = SET.ExtractData(ProcessCommand);
                            if (Data.Count > 0)
                            {
                                var info = Data[SET.CommandOptions[0].OptionName];
                                var message = Data[SET.CommandOptions[1].OptionName];
                                var button = Data[SET.CommandOptions[2].OptionName];

                                string DialogTitle2 = info;
                                string DialogMessage2 = $"{message}";
                                DialogMessage2 = DialogMessage2.Replace("\\n", "\n");
                                string[] DialogButtons2 = new string[] { button };
                                var DialogTest2 = Helpers.CreateDialog(DialogTitle2, DialogMessage2, DialogButtons2);
                                var DialogResult2 = await DialogTest2.ShowAsync2();
                                //handlerValue = button1Value;
                            }
                        }
                    }
                }
                else
                {
                    var TelnetConnection = CCache.GetTelnetCache(TelnetID);
                    await TelnetConnection.Send(ProcessCommand);

                }

                await onFinish();
                Helpers.CallGCCollect();
                return true;
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
            }
            return false;
        }

        public async Task onStart()
        {
            if (StartProcessID != null && StartProcessID.Length > 0)
            {
                var StartProcess = CCache.GetProcessCache(StartProcessID);
                await StartProcess.Execute();
            }
        }

        public async Task onFinish()
        {
            if (FinishProcessID != null && FinishProcessID.Length > 0)
            {
                var FinsihProcess = CCache.GetProcessCache(FinishProcessID);
                await FinsihProcess.Execute();
            }
        }

        public void Cancel()
        {
            try
            {
                if (TelnetID.Length == 0 || TelnetID.Equals("0"))
                {

                }
                else
                {
                    var TelnetConnection = CCache.GetTelnetCache(TelnetID);
                    TelnetConnection.Cancel();
                }
                Helpers.CallGCCollect();
            }
            catch
            {

            }
        }
    }
}
