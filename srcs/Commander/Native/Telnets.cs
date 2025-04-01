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
    public static class Telnets
    {
        public static CTelnet Telnet(string telnetID, string host, string port)
        {
            CTelnet cTelnet = new CTelnet(telnetID, host, port, true);
            return cTelnet;
        }

        public static CTelnet Telnet(string telnetID, string host, string port, string finishProcessID)
        {
            CTelnet cTelnet = new CTelnet(telnetID, host, port, finishProcessID, true);
            return cTelnet;
        }

        public static CTelnet Telnet(string telnetID, string host, string port, string startProcessID, string finishProcessID)
        {
            CTelnet cTelnet = new CTelnet(telnetID, host, port, startProcessID, finishProcessID, true);
            return cTelnet;
        }
        public static CTelnet Telnet(string telnetID, string host, string port, string tUser, string tPassword, bool login)
        {
            CTelnet cTelnet = new CTelnet(telnetID, host, port, true, tUser, tPassword);
            return cTelnet;
        }
    }
}
