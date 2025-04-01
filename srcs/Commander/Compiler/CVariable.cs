/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Compiler
{
    public class CVariable
    {
        public string VariableValue;
        public string VariableID;

        public CVariable(string variableID, string variableValue, bool append = false)
        {
            VariableValue = variableValue;
            VariableID = variableID;

            if (append)
            {
                Append();
            }
        }

        public void Append()
        {
            CVariable outPutTest = null;
            CCache.VariablesCache.TryGetValue(VariableID, out outPutTest);
            lock (CCache.VariablesCache)
            {
                if (outPutTest == null)
                {
                    CCache.VariablesCache.Add(VariableID, this);
                }
                else
                {
                    CCache.VariablesCache[VariableID] = this;
                }
            }
        }
    }
}
