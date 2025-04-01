/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using WinUniversalTool.WebViewer;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WinUniversalTool.TranslateAPI
{
    public static class YandexTranslate
    {
        public static async Task<List<Run>> Translate(string text, CancellationToken cancellationToken, TranslateAPI.Language from, TranslateAPI.Language to)
        {
            List<Run> translatedText = new List<Run>();

            try
            {
                string languageRequest = to.ISO639;
                if (!from.ISO639.Equals("auto"))
                {
                    languageRequest = $"{from.ISO639}-{to.ISO639}";
                }
                var YandexKey = WebHelper.TranslateRequestYandexKey;
                List<KeyValuePair<string, string>> postValues = new List<KeyValuePair<string, string>>()
                {
                   new KeyValuePair<string, string>("format", "plain"),
                   new KeyValuePair<string, string>("lang", $"{languageRequest}"),
                   new KeyValuePair<string, string>("text", text),
                   new KeyValuePair<string, string>("key", YandexKey),
                   new KeyValuePair<string, string>("options", "0")
                };

                var response = await WebHelper.GetResponse(new Uri(WebHelper.TranslateRequestYandexURL), cancellationToken, postValues);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var result = "";

                    var output = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var translateObject = JsonConvert.DeserializeObject<YandexTranslateObject>(output);
                        if (translateObject == null)
                        {
                            throw new Exception("Empty result");
                        }
                        if (translateObject.code != 200)
                        {
                            Helpers.ShowMessage("Yandex Translate", $"Code: {translateObject.code}\nMessage: {translateObject.message}");
                        }
                        else
                        {
                            ParseTranslate(ref translatedText, translateObject);
                        }
                    }
                    catch (Exception e)
                    {
                        var translateObject = JsonConvert.DeserializeObject<dynamic>(output);
                        if (translateObject == null)
                        {
                            throw new Exception("Empty result");
                        }
                        if (translateObject.code != 200)
                        {
                            Helpers.ShowMessage("Yandex Translate", $"Code: {translateObject.code}\nMessage: {translateObject.message}");
                        }
                        else
                        {
                            ParseTranslate(ref translatedText, translateObject);
                        }
                    }
                    //translatedText = result;
                }
                else
                {
                    string extraData = "";
                    if (response != null)
                    {
                        extraData = $"\n{response.Headers.ToString()}";
                    }
                    Helpers.Logger($"Yandex Translate -> Cannot translate at the moment\n{extraData}");
                }
            }
            catch (Exception e)
            {

            }

            return translatedText;
        }

        public static void ParseTranslate(ref List<Run> result, dynamic translateObject)
        {
            try
            {
                foreach (var translateItem in translateObject.text)
                {
                    if (translateItem != null)
                    {
                        //result += $"{ Uri.UnescapeDataString(translateItem)}";
                        result.Add(new Run { Text = $"{Uri.UnescapeDataString(translateItem)}" });
                    }
                }

                try
                {
                    var langs = translateObject.lang.Split('-');
                    TranslateAPI.Language SourceLanguage = TranslateHelpers.GetLanguageByISO(langs[0]);
                    //result = $"{result}\n\nTranslated from ({SourceLanguage.FullName}) by Yandex";
                    result.Add(new Run { Text = $"\n\nTranslated from ({SourceLanguage.FullName}) by Yandex", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.Green) });
                }
                catch (Exception e)
                {
                    result.Add(new Run { Text = $"\n\nTranslated by Yandex", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.Green) });
                    //result = $"{result}\n\nTranslated by Yandex";
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
