using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.TranslateAPI
{
    public class TranslationObject
    {
        public sentences[] sentences;
        public dict[] dict;
        public string src;
        public double confidence;
        public ld_result ld_result;
    }

    public class sentences
    {
        public string trans;
        public string orig;
        public string backend;

        public string translit;
        public string src_translit;
    }

    public class dict
    {
        public string pos;
        public string[] terms;
        public entry[] entry;
        public string base_form;
        public int pos_enum;
    }
    public class entry
    {
        public string word;
        public string[] reverse_translation;
        public double score;
    }
    public class ld_result
    {
        public string[] srclangs;
        public double[] srclangs_confidences;
        public string[] extended_srclangs;
    }
}
