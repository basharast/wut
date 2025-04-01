/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Command
{
    public class CContant
    {
        public string ConstantTag;
        public string ValueID;

        public CContant(string constantTag, string valueID, bool append = false)
        {
            ConstantTag = constantTag;
            ValueID = valueID;
            if (append)
            {
                Append();
            }
        }

        public void Append()
        {
            CConstants.ALLCONSTANTS.Add(this);
        }
    }
}
