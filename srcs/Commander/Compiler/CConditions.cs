/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace Commander.Compiler
{
    public class CConditions
    {
        public string Condition;
        public string WhenCommand;
        public string ElseCommand;

        public CConditions(string condition, string whenCommand)
        {
            Condition = condition;
            WhenCommand = whenCommand;
        }

        public CConditions(string condition, string whenCommand, string elseCommand)
        {
            Condition = condition;
            WhenCommand = whenCommand;
            ElseCommand = elseCommand;
        }

        public bool CheckCondition()
        {
            string Operator = "";
            string variable = "";
            string testValue = "";
            string testValue2 = "";
            string testValue3 = "";
          
            string ConditionCleaned = Condition.Replace(" ", "");
            //Get Operator
            var regexTest = @"(?<variable>.*)\s{0,10}(?<Operator>\b>=|<=|<|>|equ|==|neq|!=|lss|leq|gtr|geq|!~|~~|@|#\b)\s{0,10}(?<testValue>.*)";
            Match ms = Regex.Match(Condition, regexTest, RegexOptions.IgnoreCase);
            if (ms.Success)
            {
                if (ms.Groups != null && ms.Groups.Count > 0)
                {
                    Operator = ms.Groups["Operator"].Value;
                    variable = ms.Groups["variable"].Value;
                    bool lowerRequest = false;
                    if (variable.StartsWith("lower:"))
                    {
                        variable = variable.Replace("lower:", "");
                        lowerRequest = true;
                    }
                    //Try to find if defined before using the actual result
                    try
                    {
                        foreach(var vItem in CCache.VariablesCache)
                        {
                            if (vItem.Key.Equals(variable))
                            {
                                variable = Commander.Native.Variables.Get(variable);
                                break;
                            }
                        }
                    }catch(Exception ex)
                    {

                    }
                    if (lowerRequest)
                    {
                        variable = variable.ToLower();
                    }
                    testValue = ms.Groups["testValue"].Value;
                    if (testValue.Contains("|"))
                    {
                        var values = testValue.Split('|');
                        testValue = values[0];
                        testValue2 = values[1];
                        if(values.Length == 3)
                        {
                            testValue3 = values[2];
                        }
                    }
                }
            }

            
            //Helpers.Logger($"is {variable} ({Operator}) from {testValue}?");
            switch (Operator)
            {
                case "equ":
                case "==":
                    if (variable.Equals(testValue) || (testValue2.Length>0 && variable.Equals(testValue2))|| (testValue3.Length>0 && variable.Equals(testValue3)))
                    {
                        return true;
                    }
                    break;

                case "neq":
                case "!=":
                    if (!variable.Equals(testValue) || (testValue2.Length > 0 && !variable.Equals(testValue2))|| (testValue3.Length > 0 && !variable.Equals(testValue3)))
                    {
                        return true;
                    }
                    break;

                case "lss":
                case "<":
                    try
                    {
                        var variableInt = int.Parse(variable);
                        var testValueInt = int.Parse(testValue);
                        if (variableInt < testValueInt)
                        {
                            return true;
                        }
                        if(testValue2.Length > 0)
                        {
                             variableInt = int.Parse(variable);
                             testValueInt = int.Parse(testValue2);
                            if (variableInt < testValueInt)
                            {
                                return true;
                            }
                        }
                        if(testValue3.Length > 0)
                        {
                             variableInt = int.Parse(variable);
                             testValueInt = int.Parse(testValue3);
                            if (variableInt < testValueInt)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    break;

                case "leq":
                case "<=":
                    try
                    {
                        var variableInt = int.Parse(variable);
                        var testValueInt = int.Parse(testValue);
                        if (variableInt <= testValueInt)
                        {
                            return true;
                        }
                        if (testValue2.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue2);
                            if (variableInt <= testValueInt)
                            {
                                return true;
                            }
                        }
                        if (testValue3.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue3);
                            if (variableInt <= testValueInt)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    break;

                case "gtr":
                case ">":
                    try
                    {
                        var variableInt = int.Parse(variable);
                        var testValueInt = int.Parse(testValue);
                        if (variableInt > testValueInt)
                        {
                            return true;
                        }
                        if (testValue2.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue2);
                            if (variableInt > testValueInt)
                            {
                                return true;
                            }
                        }
                        if (testValue3.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue3);
                            if (variableInt > testValueInt)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    break;

                case "geq":
                case ">=":
                    try
                    {
                        var variableInt = int.Parse(variable);
                        var testValueInt = int.Parse(testValue);
                        if (variableInt >= testValueInt)
                        {
                            return true;
                        }
                        if (testValue2.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue2);
                            if (variableInt >= testValueInt)
                            {
                                return true;
                            }
                        }
                        if (testValue3.Length > 0)
                        {
                            variableInt = int.Parse(variable);
                            testValueInt = int.Parse(testValue3);
                            if (variableInt >= testValueInt)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    break; 
                
                case "~~":
                    if (variable.Contains(testValue) || (testValue2.Length > 0 && variable.Contains(testValue2)) || (testValue3.Length > 0 && variable.Contains(testValue3)))
                    {
                        return true;
                    }
                    break;

                case "!~":
                    if (!variable.Contains(testValue) || (testValue2.Length > 0 && !variable.Contains(testValue2)) || (testValue3.Length > 0 && !variable.Contains(testValue3)))
                    {
                        return true;
                    }
                    break;

                case "@":
                    if (variable.StartsWith(testValue) || (testValue2.Length > 0 && variable.StartsWith(testValue2)) || (testValue3.Length > 0 && variable.StartsWith(testValue3)))
                    {
                        return true;
                    }
                    break;

                case "#":
                    if (variable.EndsWith(testValue) || (testValue2.Length > 0 && variable.EndsWith(testValue2))|| (testValue3.Length > 0 && variable.EndsWith(testValue3)))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
