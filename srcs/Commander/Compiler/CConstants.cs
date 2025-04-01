/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Compiler
{
    public static class CConstants
    {
        public static List<CContant> ALLCONSTANTS = new List<CContant>();

        //Constants (Locations)
        public static CContant BASELOCALTION = new CContant("$BaseLocation", "", true);
        public static CContant SCRIPTLOCATION = new CContant("$ScriptLocation", "", true);
        public static CContant CHACHEFOLDER = new CContant("$CacheFolder", "", true);
        public static CContant LOCALFOLDER = new CContant("$LocalFolder", "", true);

        //Constants (Miscs)
        public static CContant CURRENTDATE = new CContant("$CurrentDate", "", true);
    }
}
