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
    public static class Conditions
    {
        public static CConditions When(string condition, string command)
        {
            CConditions cConditions = new CConditions(condition, command);
            return cConditions;
        }

        public static CConditions Else(string condition, string command)
        {
            CConditions cConditions = new CConditions(condition, command);
            return cConditions;
        }
    }
}
