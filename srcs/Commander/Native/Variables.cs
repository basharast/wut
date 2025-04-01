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
    public static class Variables
    {
        public static CVariable Define(string variableID, string variableValue)
        {
            CVariable variable = new CVariable(variableID, variableValue, true);
            return variable;
        }

        public static void Set(string variableID, string variableValue)
        {
            CCache.VariablesCache[variableID].VariableValue = variableValue;
        }

        public static void Set(string variableID, string variableParameter, string variableValue)
        {

        }

        public static string Get(string variableID)
        {
            string variableValue = CCache.VariablesCache[variableID].VariableValue;
            return variableValue;
        }
    }
}
