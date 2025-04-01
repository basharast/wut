using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.TranslateAPI
{
	class LanguageIsNotSupportedException: Exception
	{
		public readonly Language Language;

		public LanguageIsNotSupportedException(Language language)
			:base("Language is not supported by GoogleTranslate:")
		{
			Language = language;
		}

		public override string Message => base.Message + " " + Language;
	}
}
