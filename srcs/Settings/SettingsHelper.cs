/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.Settings
{
    public static class SettingsHelper
    {
        public static EventHandler TextBoxChanged;
        public static EventHandler ComboBoxChanged;
        public static EventHandler CheckBoxChanged;

        public static EventHandler TextBoxChangedScript;
        public static EventHandler ComboBoxChangedScript;
        public static EventHandler CheckBoxChangedScript;
        public static EventHandler GetHandlerByNameScript(string name)
        {
            switch (name)
            {
                case "TextBox":
                    return TextBoxChangedScript;

                case "ComboBox":
                    return ComboBoxChangedScript;

                case "CheckBox":
                    return CheckBoxChangedScript;
            }
            return null;
        } 
        public static EventHandler GetHandlerByName(string name)
        {
            switch (name)
            {
                case "TextBox":
                    return TextBoxChanged;

                case "ComboBox":
                    return ComboBoxChanged;

                case "CheckBox":
                    return CheckBoxChanged;
            }
            return null;
        }
    }
}
