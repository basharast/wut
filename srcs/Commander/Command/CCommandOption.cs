/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Command
{
    public class CCommandOption
    {
        public string OptionName;
        public string RegexRule = ".*";
        public CCommandOption(string optionName, string regexRule = ".*")
        {
            OptionName = optionName;
            RegexRule = regexRule;
        }

    }
}
