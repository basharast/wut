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
    public static class Areas
    {
        public static CLine Define(string areaID, string areaCommand)
        {
            CLine cLine = new CLine(areaID, areaCommand, true);
            return cLine;
        }

        public static CLine Jump(string areaID)
        {
            CLine cLine = CCache.GetLineCache(areaID);
            return cLine;
        }
    }
}
