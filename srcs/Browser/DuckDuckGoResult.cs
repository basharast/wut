/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.WebViewer
{
    public class DuckDuckGoResult
    {
        public string Abstract;
        public string AbstractSource;
        public string AbstractText;
        public string AbstractURL;
        public string Answer;
        public string Heading;
        public string Entity;
        public Infobox Infobox;
        public RelatedTopics[] RelatedTopics;
    }
    public class Infobox
    {
        public content[] content;
        public meta meta;
    }
    public class content
    {
        public string data_type;
        public string label;
        public string value;
        public int wiki_order;
    }
    public class meta
    {
        public string data_type;
        public string label;
        public string value;
    }
    public class RelatedTopics
    {
        public string FirstURL;
        public Icon Icon;
        public string Result;
        public string Text;
        public string Name;
        public Topics[] Topics;
    }
    public class Icon
    {
        public string Height;
        public string URL;
        public string Width;
    }
    public class Topics
    {
        public string FirstURL;
        public Icon Icon;
        public string Result;
        public string Text;
    }
}
