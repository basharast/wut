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
using WinUniversalTool.TranslateAPI;
using WinUniversalTool.WebViewer;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WinUniversalTool.TranslateAPI
{
    public static class GoogleTranslate
    {
        public static bool isArabic = false;
        public static async Task<List<Run>> Translate(string text, CancellationToken cancellationToken, TranslateAPI.Language from, TranslateAPI.Language to)
        {
            List<Run> translatedText = new List<Run>();
            try
            {
                List<KeyValuePair<string, string>> postValues = new List<KeyValuePair<string, string>>()
                {
                   new KeyValuePair<string, string>("sl", from.ISO639),
                   new KeyValuePair<string, string>("tl", to.ISO639),
                   new KeyValuePair<string, string>("q", text)
                };
                if (to.ISO639.Equals("ar"))
                {
                    isArabic = true;
                }
                else
                {
                    isArabic = false;
                }
                var response = await WebHelper.GetResponse(new Uri(WebHelper.TranslateRequestGoogleURL), cancellationToken, postValues);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var output = await response.Content.ReadAsStringAsync();
                    var result = "";
                    try
                    {
                        var translateObject = JsonConvert.DeserializeObject<TranslationObject>(output);
                        if (translateObject == null)
                        {
                            throw new Exception("Empty result");
                        }
                        ParseTranslate(ref translatedText, translateObject);
                    }
                    catch (Exception e)
                    {
                        var translateObject = JsonConvert.DeserializeObject<dynamic>(output);
                        if (translateObject == null)
                        {
                            throw new Exception("Empty result");
                        }
                        ParseTranslate(ref translatedText, translateObject);
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
                    Helpers.Logger($"Google Translate -> Cannot translate at the moment\n{extraData}");
                }
            }
            catch(Exception e)
            {

            }
            return translatedText;
        }

        public static void ParseTranslate(ref List<Run> result, dynamic translateObject)
        {
            try
            {
                foreach (var translateItem in translateObject.sentences)
                {
                    if (translateItem.trans != null)
                    {
                        result.Add(new Run { Text = $"{ Uri.UnescapeDataString(translateItem.trans)}" });
                    }
                }
                var dictionaryExtra = "";
                if (translateObject.dict != null && translateObject.dict.Length > 0)
                {
                    foreach (var dictionaryItem in translateObject.dict)
                    {
                        if (dictionaryItem.terms != null && dictionaryItem.terms.Length > 0)
                        {
                            foreach (var termItem in dictionaryItem.terms)
                            {
                                dictionaryExtra += $"\n{Uri.UnescapeDataString(termItem)}";
                                result.Add(new Run { Text = $"\n{Uri.UnescapeDataString(termItem)}", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.Gray) });

                            }
                        }

                        if (dictionaryItem.entry != null && dictionaryItem.entry.Length > 0)
                        {
                            dictionaryExtra += "\n------------\n";
                            result.Add(new Run { Text = $"\n------------\n", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Orange) });
                            foreach (var entryItem in dictionaryItem.entry)
                            {
                                result.Add(new Run { Text = $"\n{Uri.UnescapeDataString(entryItem.word)}\n{Uri.UnescapeDataString(string.Join(", ", entryItem.reverse_translation))}\n", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.DodgerBlue) });
                                dictionaryExtra += $"\n{Uri.UnescapeDataString(entryItem.word)}\n{Uri.UnescapeDataString(string.Join(", ", entryItem.reverse_translation))}\n";
                            }
                        }
                    }
                }
                if (dictionaryExtra.Length > 0)
                {
                    if (isArabic)
                    {
                        result.Insert(1, new Run { Text = $"\n\nإنظر أيضاً", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Tomato) });
                    }
                    else
                    {
                        result.Insert(1, new Run { Text = $"\n\nSee Also", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Tomato) });
                    }
                    //result = $"{result}\n\nSee Also:{dictionaryExtra}";
                }
                try
                {
                    TranslateAPI.Language SourceLanguage = TranslateHelpers.GetLanguageByISO(translateObject.src);
                    result.Add(new Run { Text = $"\n\nTranslated from ({SourceLanguage.FullName}) by Google", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.Green) });
                    //result = $"{result}\n\nTranslated from ({SourceLanguage.FullName}) by Google";
                }
                catch (Exception e)
                {
                    result.Add(new Run { Text = $"\n\nTranslated by Google", FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(Colors.Green) });
                    //result = $"{result}\n\nTranslated by Google";
                }
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }
    }
}
