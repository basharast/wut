/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace Commander.Command
{
    public class CCommand
    {
        public string CommandTag;
        public string CommandDescription;
        public List<CCommandOption> CommandOptions = new List<CCommandOption>();
        public CCommand(string commandTag, string[] commandOptions, string commandDescription = "-", bool appendScript = false)
        {
            CommandTag = commandTag;
            CommandDescription = commandDescription;
            foreach (var OptionItem in commandOptions)
            {
                var CommandOption = new CCommandOption(OptionItem);
                CommandOptions.Add(CommandOption);
            }

            if (appendScript)
            {
                Append();
            }
        }

        public Dictionary<string, string> ExtractData(string CodeLine)
        {
            if (!CodeLine.ToLower().StartsWith($"definec ") && !CodeLine.ToLower().StartsWith($"func ")&& !CodeLine.ToLower().StartsWith($"process "))
            {
                CodeLine = CodeLine.Trim().Replace("[", "").Replace("]", "");
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            var regexTest = $@"{CommandTag}[\s]" + "{0,10}";
            try
            {
                foreach (var CommandItem in CommandOptions)
                {
                    if (CommandItem.OptionName.Contains("["))
                    {
                        continue;
                    }
                    try
                    {
                        if (CommandOptions.Last().Equals(CommandItem))
                        {
                            if (CommandItem.OptionName.Equals("command"))
                            {
                                regexTest += $@"\((?<{CommandItem.OptionName}>{CommandItem.RegexRule})\)";
                            }
                            else
                            {
                                regexTest += $"(?<{CommandItem.OptionName}>{CommandItem.RegexRule})";
                            }
                        }
                        else
                        {
                            if (CommandItem.OptionName.Equals("command"))
                            {
                                regexTest += $@"\((?<{CommandItem.OptionName}>{CommandItem.RegexRule})\);[\s]" + "{0,10}" + @",[\s]{0,10}";
                            }
                            else
                            {
                                regexTest += $@"(?<{CommandItem.OptionName}>{CommandItem.RegexRule});[\s]" + "{0,10}" + @",[\s]{0,10}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.Logger(ex);
                    }
                }
                //Helpers.Logger(regexTest);
                try
                {
                    var CommandPatternStatus = regexTest;
                    Match ms = Regex.Match(CodeLine, CommandPatternStatus, RegexOptions.IgnoreCase);
                    if (ms.Success)
                    {
                        if (ms.Groups != null && ms.Groups.Count > 0)
                        {
                            foreach (var CommandItem in CommandOptions)
                            {
                                if (CommandItem.OptionName.Contains("["))
                                {
                                    continue;
                                }
                                var tag = CommandItem.OptionName;
                                var value = ms.Groups[CommandItem.OptionName].ToString().Trim();
                                //Helpers.Logger($"{tag} -> {value}");
                                data.Add(tag, value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.Logger(ex);
                }
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
            }

            return data;
        }
        public CCommand(string commandTag, List<CCommandOption> commandOptions, string commandDescription = "-", bool appendScript = false)
        {
            CommandTag = commandTag;
            CommandDescription = commandDescription;
            CommandOptions = commandOptions;

            if (appendScript)
            {
                Append();
            }
        }

        public void AddOption(string commandOption)
        {
            var CommandOption = new CCommandOption(commandOption);
            CommandOptions.Add(CommandOption);
        }

        public void AddOption(CCommandOption commandOption)
        {
            CommandOptions.Add(commandOption);
        }

        public void Append()
        {
            //Append command to ALLCOMMANDS list
            CCommands.ALLCOMMANDS.Add(this);
        }
    }
}
