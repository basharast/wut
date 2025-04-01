using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.TranslateAPI
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class LanguageAttribute: Attribute
	{
		public string Iso639 { get; }
		public string FullName { get; }
		public LanguageAttribute(string iso, [CallerMemberName] string fullName = "")
		{
			if (string.IsNullOrWhiteSpace(iso))
				throw new ArgumentException(nameof(iso));

			Iso639 = iso;
			FullName = fullName;
		}
	}
}
