using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.TranslateAPI
{
    public static class TranslateHelpers
    {
        /// <summary>
        /// An Array of supported languages by google translate
        /// </summary>
        public static Language[] LanguagesSupported { get; set; }

        /// <param name="language">Full name of the required language</param>
        /// <example>GoogleTranslator.GetLanguageByName("English")</example>
        /// <returns>Language object from the LanguagesSupported array</returns>
        public static Language GetLanguageByName(string language)
            => LanguagesSupported.FirstOrDefault(i
                => i.FullName.Equals(language, StringComparison.OrdinalIgnoreCase));

        /// <param name="iso">ISO of the required language</param>
        /// <example>GoogleTranslator.GetLanguageByISO("en")</example>
        /// <returns>Language object from the LanguagesSupported array</returns>
        // ReSharper disable once InconsistentNaming
        public static Language GetLanguageByISO(string iso)
            => LanguagesSupported.FirstOrDefault(i
                => i.ISO639.Equals(iso, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Check is available language to translate
        /// </summary>
        /// <param name="language">Checked <see cref="Language"/> </param>
        /// <returns>Is it available language or not</returns>
        public static bool IsLanguageSupported(Language language)
        {
            if (language.Equals(Language.Auto))
                return true;

            return LanguagesSupported.Contains(language) ||
                         LanguagesSupported.FirstOrDefault(language.Equals) != null;
        }
    }
}
