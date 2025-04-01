/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedLibrary;
using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace WebViewComponents
{
    [AllowForWeb]
    public sealed class ConsoleOverride
    {

        public void notify(object type, object message)
        {
            try
            {
                var messageString = (string)message;
                var typeString = (string)type;
                if (messageString != null)
                {
                    NotifyData notifyData = new NotifyData(typeString, messageString);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, notifyData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void alert(object message)
        {
            try
            {
                var messageString = (string)message;
                if (messageString != null)
                {
                    DialogData dialogData = new DialogData(messageString, "alert");
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, dialogData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void confirm(object message)
        {
            try
            {
                var messageString = (string)message;
                if (messageString != null)
                {
                    DialogData dialogData = new DialogData(messageString, "confirm");
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, dialogData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void prompt(object message, object defaultText)
        {
            try
            {
                var messageString = (string)message;
                var defaultTextString = (string)defaultText;
                if (messageString != null)
                {
                    DialogData dialogData = new DialogData(messageString, "prompt", defaultTextString);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, dialogData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void prompt(object message)
        {
            try
            {
                var messageString = (string)message;
                var defaultTextString = "";
                if (messageString != null)
                {
                    DialogData dialogData = new DialogData(messageString, "prompt", defaultTextString);
                    if (StaticHandlers.Notify != null)
                    {
                        StaticHandlers.Notify.Invoke(null, dialogData);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void log(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("output", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void xhr(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("xhr", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void error(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("error", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void html(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("html", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void info(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("info", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void warn(object message)
        {
            try
            {
                var messageString = GetMessageResult(message);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("warn", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        internal static int countValue = 0;
        public void count(string message)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("output", $"{message}: {countValue}");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
                countValue++;
            }
            catch (Exception e)
            {

            }
        }
        public void count()
        {
            try
            {
                countValue++;
                string message = "default";
                ConsoleOutput consoleOutput = new ConsoleOutput("output", $"{message}: {countValue}");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void assert(bool condition, object message)
        {
            try
            {
                if (!condition)
                {
                    var messageString = GetMessageResult(message);
                    if (messageString != null)
                    {
                        ConsoleOutput consoleOutput = new ConsoleOutput("error", messageString);
                        if (StaticHandlers.AddToConsole != null)
                        {
                            StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void clear()
        {
            try
            {
                if (StaticHandlers.ClearConsole != null)
                {
                    StaticHandlers.ClearConsole.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void table(object tabledata, object tablecolumns)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void table(object tabledata)
        {
            try
            {
                var messageString = GetMessageResult(tabledata);
                if (messageString != null)
                {
                    ConsoleOutput consoleOutput = new ConsoleOutput("output", "", messageString);
                    if (StaticHandlers.AddToConsole != null)
                    {
                        StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public string tableBuilder(string header, string rows)
        {
            try
            {
                var headers = JsonConvert.DeserializeObject<string[]>(header);
                var rows2d = JsonConvert.DeserializeObject<string[][]>(rows);

                var table = new Table()
                {
                    Padding = 1,
                    HeaderTextAlignRight = false,
                    RowTextAlignRight = false
                };
                table.SetHeaders(headers);
                foreach (var row in rows2d)
                {
                    table.AddRow(row);
                }
                return table.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public void group(string label)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void group()
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void groupEnd()
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void groupCollapsed(string label)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void groupCollapsed()
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void timeEnd(string label)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void timeEnd()
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void trace(string label)
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }
        public void trace()
        {
            try
            {
                ConsoleOutput consoleOutput = new ConsoleOutput("warn", $"Sorry, {GetCurrentMethodName()} is not supported!");
                if (StaticHandlers.AddToConsole != null)
                {
                    StaticHandlers.AddToConsole.Invoke(null, consoleOutput);
                }
            }
            catch (Exception e)
            {

            }
        }

        internal static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal static string GetMessageResult(object message)
        {
            string output = "";
            try
            {
                if (message == null)
                {
                    output = "Null value";
                }
                else
                if (message.GetType() == typeof(string))
                {
                    output = message.ToString();
                }
                else
                {
                    output = JsonConvert.SerializeObject(message);
                }
                try
                {
                    if (output != null && output.Length > 0 && IsValidJson(output))
                    {
                        JToken parsedJson = JToken.Parse(output);
                        output = parsedJson.ToString(Formatting.Indented);
                        output = output.Replace("\\\"", "\"");
                    }
                }
                catch (Exception e)
                {

                }
            }
            catch (Exception e)
            {
                try
                {
                    output = message.ToString();
                }
                catch (Exception ex)
                {
                    output = ex.Message;
                }
            }
            return output;
        }

        internal static string GetCurrentMethodName([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                return memberName;
            }
            catch (Exception e)
            {
                return "?";
            }
        }
    }
}
