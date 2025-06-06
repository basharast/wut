﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.TranslateAPI
{
    public static class LanguagesJSON
    {
        public static string LanguagesList = @"[
  {
    ""FullName"": ""Automatic"",
    ""ISO639"": ""auto""
  },
  {
    ""FullName"": ""Afrikaans"",
    ""ISO639"": ""af""
  },
  {
    ""FullName"": ""Albanian"",
    ""ISO639"": ""sq""
  },
  {
    ""FullName"": ""Amharic"",
    ""ISO639"": ""am""
  },
  {
    ""FullName"": ""Arabic"",
    ""ISO639"": ""ar""
  },
  {
    ""FullName"": ""Armenian"",
    ""ISO639"": ""hy""
  },
  {
    ""FullName"": ""Azerbaijani"",
    ""ISO639"": ""az""
  },
  {
    ""FullName"": ""Basque"",
    ""ISO639"": ""eu""
  },
  {
    ""FullName"": ""Belarusian"",
    ""ISO639"": ""be""
  },
  {
    ""FullName"": ""Bengali"",
    ""ISO639"": ""bn""
  },
  {
    ""FullName"": ""Bosnian"",
    ""ISO639"": ""bs""
  },
  {
    ""FullName"": ""Bulgarian"",
    ""ISO639"": ""bg""
  },
  {
    ""FullName"": ""Catalan"",
    ""ISO639"": ""ca""
  },
  {
    ""FullName"": ""Cebuano"",
    ""ISO639"": ""ceb""
  },
  {
    ""FullName"": ""Chichewa"",
    ""ISO639"": ""ny""
  },
  {
    ""FullName"": ""Chinese Simplified"",
    ""ISO639"": ""zh-cn""
  },
  {
    ""FullName"": ""Chinese Traditional"",
    ""ISO639"": ""zh-tw""
  },
  {
    ""FullName"": ""Corsican"",
    ""ISO639"": ""co""
  },
  {
    ""FullName"": ""Croatian"",
    ""ISO639"": ""hr""
  },
  {
    ""FullName"": ""Czech"",
    ""ISO639"": ""cs""
  },
  {
    ""FullName"": ""Danish"",
    ""ISO639"": ""da""
  },
  {
    ""FullName"": ""Dutch"",
    ""ISO639"": ""nl""
  },
  {
    ""FullName"": ""English"",
    ""ISO639"": ""en""
  },
  {
    ""FullName"": ""Esperanto"",
    ""ISO639"": ""eo""
  },
  {
    ""FullName"": ""Estonian"",
    ""ISO639"": ""et""
  },
  {
    ""FullName"": ""Filipino"",
    ""ISO639"": ""tl""
  },
  {
    ""FullName"": ""Finnish"",
    ""ISO639"": ""fi""
  },
  {
    ""FullName"": ""French"",
    ""ISO639"": ""fr""
  },
  {
    ""FullName"": ""Frisian"",
    ""ISO639"": ""fy""
  },
  {
    ""FullName"": ""Galician"",
    ""ISO639"": ""gl""
  },
  {
    ""FullName"": ""Georgian"",
    ""ISO639"": ""ka""
  },
  {
    ""FullName"": ""German"",
    ""ISO639"": ""de""
  },
  {
    ""FullName"": ""Greek"",
    ""ISO639"": ""el""
  },
  {
    ""FullName"": ""Gujarati"",
    ""ISO639"": ""gu""
  },
  {
    ""FullName"": ""Haitian Creole"",
    ""ISO639"": ""ht""
  },
  {
    ""FullName"": ""Hausa"",
    ""ISO639"": ""ha""
  },
  {
    ""FullName"": ""Hawaiian"",
    ""ISO639"": ""haw""
  },
  {
    ""FullName"": ""Hebrew"",
    ""ISO639"": ""iw""
  },
  {
    ""FullName"": ""Hindi"",
    ""ISO639"": ""hi""
  },
  {
    ""FullName"": ""Hmong"",
    ""ISO639"": ""hmn""
  },
  {
    ""FullName"": ""Hungarian"",
    ""ISO639"": ""hu""
  },
  {
    ""FullName"": ""Icelandic"",
    ""ISO639"": ""is""
  },
  {
    ""FullName"": ""Igbo"",
    ""ISO639"": ""ig""
  },
  {
    ""FullName"": ""Indonesian"",
    ""ISO639"": ""id""
  },
  {
    ""FullName"": ""Irish"",
    ""ISO639"": ""ga""
  },
  {
    ""FullName"": ""Italian"",
    ""ISO639"": ""it""
  },
  {
    ""FullName"": ""Japanese"",
    ""ISO639"": ""ja""
  },
  {
    ""FullName"": ""Javanese"",
    ""ISO639"": ""jw""
  },
  {
    ""FullName"": ""Kannada"",
    ""ISO639"": ""kn""
  },
  {
    ""FullName"": ""Kazakh"",
    ""ISO639"": ""kk""
  },
  {
    ""FullName"": ""Khmer"",
    ""ISO639"": ""km""
  },
  {
    ""FullName"": ""Korean"",
    ""ISO639"": ""ko""
  },
  {
    ""FullName"": ""Kurdish (Kurmanji)"",
    ""ISO639"": ""ku""
  },
  {
    ""FullName"": ""Kyrgyz"",
    ""ISO639"": ""ky""
  },
  {
    ""FullName"": ""Lao"",
    ""ISO639"": ""lo""
  },
  {
    ""FullName"": ""Latin"",
    ""ISO639"": ""la""
  },
  {
    ""FullName"": ""Latvian"",
    ""ISO639"": ""lv""
  },
  {
    ""FullName"": ""Lithuanian"",
    ""ISO639"": ""lt""
  },
  {
    ""FullName"": ""Luxembourgish"",
    ""ISO639"": ""lb""
  },
  {
    ""FullName"": ""Macedonian"",
    ""ISO639"": ""mk""
  },
  {
    ""FullName"": ""Malagasy"",
    ""ISO639"": ""mg""
  },
  {
    ""FullName"": ""Malay"",
    ""ISO639"": ""ms""
  },
  {
    ""FullName"": ""Malayalam"",
    ""ISO639"": ""ml""
  },
  {
    ""FullName"": ""Maltese"",
    ""ISO639"": ""mt""
  },
  {
    ""FullName"": ""Maori"",
    ""ISO639"": ""mi""
  },
  {
    ""FullName"": ""Marathi"",
    ""ISO639"": ""mr""
  },
  {
    ""FullName"": ""Mongolian"",
    ""ISO639"": ""mn""
  },
  {
    ""FullName"": ""Myanmar (Burmese)"",
    ""ISO639"": ""my""
  },
  {
    ""FullName"": ""Nepali"",
    ""ISO639"": ""ne""
  },
  {
    ""FullName"": ""Norwegian"",
    ""ISO639"": ""no""
  },
  {
    ""FullName"": ""Pashto"",
    ""ISO639"": ""ps""
  },
  {
    ""FullName"": ""Persian"",
    ""ISO639"": ""fa""
  },
  {
    ""FullName"": ""Polish"",
    ""ISO639"": ""pl""
  },
  {
    ""FullName"": ""Portuguese"",
    ""ISO639"": ""pt""
  },
  {
    ""FullName"": ""Punjabi"",
    ""ISO639"": ""ma""
  },
  {
    ""FullName"": ""Romanian"",
    ""ISO639"": ""ro""
  },
  {
    ""FullName"": ""Russian"",
    ""ISO639"": ""ru""
  },
  {
    ""FullName"": ""Samoan"",
    ""ISO639"": ""sm""
  },
  {
    ""FullName"": ""Scots Gaelic"",
    ""ISO639"": ""gd""
  },
  {
    ""FullName"": ""Serbian"",
    ""ISO639"": ""sr""
  },
  {
    ""FullName"": ""Sesotho"",
    ""ISO639"": ""st""
  },
  {
    ""FullName"": ""Shona"",
    ""ISO639"": ""sn""
  },
  {
    ""FullName"": ""Sindhi"",
    ""ISO639"": ""sd""
  },
  {
    ""FullName"": ""Sinhala"",
    ""ISO639"": ""si""
  },
  {
    ""FullName"": ""Slovak"",
    ""ISO639"": ""sk""
  },
  {
    ""FullName"": ""Slovenian"",
    ""ISO639"": ""sl""
  },
  {
    ""FullName"": ""Somali"",
    ""ISO639"": ""so""
  },
  {
    ""FullName"": ""Spanish"",
    ""ISO639"": ""es""
  },
  {
    ""FullName"": ""Sundanese"",
    ""ISO639"": ""su""
  },
  {
    ""FullName"": ""Swahili"",
    ""ISO639"": ""sw""
  },
  {
    ""FullName"": ""Swedish"",
    ""ISO639"": ""sv""
  },
  {
    ""FullName"": ""Tajik"",
    ""ISO639"": ""tg""
  },
  {
    ""FullName"": ""Tamil"",
    ""ISO639"": ""ta""
  },
  {
    ""FullName"": ""Telugu"",
    ""ISO639"": ""te""
  },
  {
    ""FullName"": ""Thai"",
    ""ISO639"": ""th""
  },
  {
    ""FullName"": ""Turkish"",
    ""ISO639"": ""tr""
  },
  {
    ""FullName"": ""Ukrainian"",
    ""ISO639"": ""uk""
  },
  {
    ""FullName"": ""Urdu"",
    ""ISO639"": ""ur""
  },
  {
    ""FullName"": ""Uzbek"",
    ""ISO639"": ""uz""
  },
  {
    ""FullName"": ""Vietnamese"",
    ""ISO639"": ""vi""
  },
  {
    ""FullName"": ""Welsh"",
    ""ISO639"": ""cy""
  },
  {
    ""FullName"": ""Xhosa"",
    ""ISO639"": ""xh""
  },
  {
    ""FullName"": ""Yiddish"",
    ""ISO639"": ""yi""
  },
  {
    ""FullName"": ""Yoruba"",
    ""ISO639"": ""yo""
  },
  {
    ""FullName"": ""Zulu"",
    ""ISO639"": ""zu""
  }
]";

    }
}
