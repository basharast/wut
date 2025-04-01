/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinUniversalTool.WebViewer
{
    public class ContextMenuData : EventArgs
    {
        public string ItemType;
        public string Data;
        public string Text;
        public string HTML;
        public double PositionX;
        public double PositionY;
        public ContextMenuData(string item, string data, string text, string x, string y, string HTML = "")
        {
            try
            {
                ItemType = item;
                Data = data;
                Text = text;
                try
                {
                    Text = Regex.Replace(text, "<[^>]*>", "").Trim();
                }catch(Exception e)
                {

                }
                this.HTML = HTML;
                try
                {
                    PositionX = Double.Parse(x);
                }
                catch (Exception e)
                {
                    PositionX = 0;
                }
                try
                {
                    PositionY = Double.Parse(y);
                }
                catch (Exception e)
                {
                    PositionY = 0;
                }

            }
            catch (Exception e)
            {

            }
        }
    }
}
