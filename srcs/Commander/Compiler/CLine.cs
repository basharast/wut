/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Compiler
{
    public class CLine
    {
        public string LineID;
        public string LineCommand;

        public CLine(string lineID, string lineCommand, bool append = false)
        {
            LineID = lineID;
            LineCommand = lineCommand;

            if (append)
            {
                Append();
            }
        }

        public void Append()
        {
            CLine outPutTest = null;
            CCache.ScriptCache.TryGetValue(LineID, out outPutTest);
            lock (CCache.ScriptCache)
            {
                if (outPutTest == null)
                {
                    CCache.ScriptCache.Add(LineID, this);
                }
                else
                {
                    CCache.ScriptCache[LineID] = this;
                }
            }
        }
    }
}
