using System;
using System.IO;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using WinUniversalTool.WebViewer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinUniversalTool.Views
{
    public enum DialogResultCustom
    {
        Yes = 2,
        No = 1,
        Cancle = 3,
        Nothing
    }
    public sealed partial class ContentDialogCustom : ContentDialog
    {
        public DialogResultCustom Result { get; set; }

        string localTitle = "";
        string localMessage = "";
        public bool devModeActiveDialog = false;
        public ContentDialogCustom(string title, string message, string button1, string button2, string button3, bool telnetOutput = false, string button1Temp = null, string button2Temp = null, string button3Temp = null)
        {
            this.InitializeComponent();
            this.Result = DialogResultCustom.Nothing;
            this.Title = title;
            localTitle = title;
            message = Helpers.escapeSpecialChars(message);
            messageText.Inlines.Clear();
            if (!Helpers.DeviceIsPhone())
            {
                btn1.Margin = new Thickness(5, 1, 5, 10);
                btn2.Margin = new Thickness(5, 1, 5, 10);
                btn3.Margin = new Thickness(5, 1, 5, 10);
            }

            if (telnetOutput)
            {
                messageText.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                DialogContainer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            try
            {
                try
                {
                    StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    message = message.Replace(appInstalledFolder.Path, "");
                }
                catch (Exception ex)
                {

                }
                var textSplit = message.Split('\n');
                if (textSplit != null && textSplit.Length > 1)
                {
                    foreach (var textSplitItem in textSplit)
                    {
                        if (textSplitItem.Trim().Length == 0)
                        {
                            messageText.Inlines.Add(new LineBreak());
                        }
                        else
                        {
                            //var textSplitItemTemp = textSplitItem.Replace("\\","\\\\");
                            var textSplitItemTemp = textSplitItem;
                            try
                            {
                                var itemSplit = textSplitItemTemp.Split(':');
                                if (itemSplit != null && itemSplit.Length > 1)
                                {
                                    var forCounter = 0;
                                    foreach (var itemSplitItem in itemSplit)
                                    {
                                        forCounter++;
                                        var testIfPath = WebHelper.isLinkFile(itemSplitItem);
                                        if (testIfPath)
                                        {
                                            string Name = "";
                                            try
                                            {
                                                Name = Path.GetFileNameWithoutExtension(itemSplitItem);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            if (Name.Length > 0)
                                            {
                                                var Extension = Path.GetExtension(itemSplitItem);
                                                if (Extension != null && Extension.Length > 0 && !Extension.Contains(" ") && !Extension.StartsWith(".0") && !Extension.StartsWith(".1") && !Extension.StartsWith(".2") && !Extension.StartsWith(".4") && !Extension.StartsWith(".5") && !Extension.StartsWith(".6") && !Extension.StartsWith(".8") && !Extension.StartsWith(".9"))
                                                {
                                                    if (Extension.Contains(" "))
                                                    {
                                                        Extension = Extension.Substring(0, Extension.IndexOf(" "));
                                                    }
                                                    var NameRun = new Run { Text = Name, Foreground = new SolidColorBrush(Colors.SeaGreen) };
                                                    var ExtensionRun = new Run { Text = Extension.ToLower(), Foreground = new SolidColorBrush(Colors.SkyBlue), FontWeight = FontWeights.Medium };
                                                    var fullFileName = $"{Name}{Extension}";
                                                    {
                                                        if (itemSplitItem.StartsWith(fullFileName))
                                                        {
                                                            messageText.Inlines.Add(NameRun);
                                                            messageText.Inlines.Add(ExtensionRun);
                                                            var cleanedMessage = itemSplitItem.Replace(fullFileName, "");
                                                            messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                                        }
                                                        else if (itemSplitItem.EndsWith(fullFileName))
                                                        {
                                                            var cleanedMessage = itemSplitItem.Replace(fullFileName, "");
                                                            messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                                            messageText.Inlines.Add(NameRun);
                                                            messageText.Inlines.Add(ExtensionRun);
                                                        }
                                                        else
                                                        {
                                                            var splitMessage = itemSplitItem.Split(new string[] { fullFileName }, StringSplitOptions.None);
                                                            if (splitMessage.Length == 2)
                                                            {
                                                                messageText.Inlines.Add(new Run { Text = $"{splitMessage[0]}" });
                                                                messageText.Inlines.Add(NameRun);
                                                                messageText.Inlines.Add(ExtensionRun);
                                                                messageText.Inlines.Add(new Run { Text = $"{splitMessage[1]}" });

                                                            }
                                                            else
                                                            {
                                                                messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}" });
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}" });
                                                }
                                            }
                                            else
                                            {
                                                messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}" });
                                            }
                                        }
                                        else
                                        {
                                            if (itemSplitItem.Length > 0)
                                            {
                                                if (forCounter == 1)
                                                {
                                                    if (itemSplitItem.ToLower().Contains("important"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Tomato) });
                                                    }
                                                    else
                                                    if (itemSplitItem.ToLower().Contains("be careful"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Tomato) });
                                                    }
                                                    else
                                                    if (itemSplitItem.ToLower().Contains("error"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Red) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("note"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Orange) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("warning"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Orange) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("note 1"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Orange) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("note 2"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Orange) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("remember"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Green) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("failed"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Red) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("report"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Green) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("success"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Green) });
                                                    }
                                                    else if (itemSplitItem.ToLower().Contains("example"))
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.DodgerBlue) });
                                                    }
                                                    else
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:", FontWeight = FontWeights.Bold });
                                                    }
                                                }
                                                else
                                                {
                                                    if (forCounter < itemSplit.Length)
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}:" });
                                                    }
                                                    else
                                                    {
                                                        messageText.Inlines.Add(new Run { Text = $"{itemSplitItem}" });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var testIfPath = WebHelper.isLinkFile(textSplitItemTemp);
                                    if (testIfPath)
                                    {
                                        string Name = "";
                                        try
                                        {
                                            Name = Path.GetFileNameWithoutExtension(textSplitItemTemp);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        if (Name.Length > 0)
                                        {
                                            var Extension = Path.GetExtension(textSplitItemTemp);
                                            if (Extension != null && Extension.Length > 0 && !Extension.Contains(" ") && !Extension.StartsWith(".0") && !Extension.StartsWith(".1") && !Extension.StartsWith(".2") && !Extension.StartsWith(".4") && !Extension.StartsWith(".5") && !Extension.StartsWith(".6") && !Extension.StartsWith(".8") && !Extension.StartsWith(".9"))
                                            {
                                                if (Extension.Contains(" "))
                                                {
                                                    Extension = Extension.Substring(0, Extension.IndexOf(" "));
                                                }
                                                else if (Extension.Contains("?"))
                                                {
                                                    Extension = Extension.Substring(0, Extension.IndexOf("?"));
                                                }
                                                var NameRun = new Run { Text = Name, Foreground = new SolidColorBrush(Colors.SeaGreen) };
                                                var ExtensionRun = new Run { Text = Extension.ToLower(), Foreground = new SolidColorBrush(Colors.SkyBlue), FontWeight = FontWeights.Medium };
                                                var fullFileName = $"{Name}{Extension}";
                                                {
                                                    if (textSplitItemTemp.StartsWith(fullFileName))
                                                    {
                                                        messageText.Inlines.Add(NameRun);
                                                        messageText.Inlines.Add(ExtensionRun);
                                                        var cleanedMessage = textSplitItemTemp.Replace(fullFileName, "");
                                                        messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                                    }
                                                    else if (textSplitItemTemp.EndsWith(fullFileName))
                                                    {
                                                        var cleanedMessage = textSplitItemTemp.Replace(fullFileName, "");
                                                        messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                                        messageText.Inlines.Add(NameRun);
                                                        messageText.Inlines.Add(ExtensionRun);
                                                    }
                                                    else
                                                    {
                                                        var splitMessage = textSplitItemTemp.Split(new string[] { fullFileName }, StringSplitOptions.None);
                                                        if (splitMessage.Length == 2)
                                                        {
                                                            messageText.Inlines.Add(new Run { Text = $"{splitMessage[0]}" });
                                                            messageText.Inlines.Add(NameRun);
                                                            messageText.Inlines.Add(ExtensionRun);
                                                            messageText.Inlines.Add(new Run { Text = $"{splitMessage[1]}" });

                                                        }
                                                        else
                                                        {
                                                            messageText.Inlines.Add(new Run { Text = $"{textSplitItem}" });
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                messageText.Inlines.Add(new Run { Text = $"{textSplitItem}" });
                                            }
                                        }
                                        else
                                        {
                                            messageText.Inlines.Add(new Run { Text = $"{textSplitItem}" });
                                        }
                                    }
                                    else
                                    {
                                        messageText.Inlines.Add(new Run { Text = textSplitItem });
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                messageText.Inlines.Add(new Run { Text = textSplitItem });
                            }
                            messageText.Inlines.Add(new LineBreak());
                        }
                    }
                }
                else
                {
                    try
                    {
                        var testIfPath = WebHelper.isLinkFile(message);
                        if (testIfPath)
                        {
                            string Name = "";
                            try
                            {
                                Name = Path.GetFileNameWithoutExtension(message);
                            }
                            catch (Exception ex)
                            {

                            }
                            if (Name.Length > 0)
                            {
                                var Extension = Path.GetExtension(message);
                                if (Extension != null && Extension.Length > 0 && !Extension.Contains(" ") && !Extension.StartsWith(".0") && !Extension.StartsWith(".1") && !Extension.StartsWith(".2") && !Extension.StartsWith(".4") && !Extension.StartsWith(".5") && !Extension.StartsWith(".6") && !Extension.StartsWith(".8") && !Extension.StartsWith(".9"))
                                {
                                    if (Extension.Contains(" "))
                                    {
                                        Extension = Extension.Substring(0, Extension.IndexOf(" "));
                                    }
                                    var NameRun = new Run { Text = Name, Foreground = new SolidColorBrush(Colors.SeaGreen) };
                                    var ExtensionRun = new Run { Text = Extension.ToLower(), Foreground = new SolidColorBrush(Colors.SkyBlue), FontWeight = FontWeights.Medium };
                                    var fullFileName = $"{Name}{Extension}";
                                    {
                                        if (message.StartsWith(fullFileName))
                                        {
                                            messageText.Inlines.Add(NameRun);
                                            messageText.Inlines.Add(ExtensionRun);
                                            var cleanedMessage = message.Replace(fullFileName, "");
                                            messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                        }
                                        else if (message.EndsWith(fullFileName))
                                        {
                                            var cleanedMessage = message.Replace(fullFileName, "");
                                            messageText.Inlines.Add(new Run { Text = $"{cleanedMessage}" });
                                            messageText.Inlines.Add(NameRun);
                                            messageText.Inlines.Add(ExtensionRun);
                                        }
                                        else
                                        {
                                            var splitMessage = message.Split(new string[] { fullFileName }, StringSplitOptions.None);
                                            if (splitMessage.Length == 2)
                                            {
                                                messageText.Inlines.Add(new Run { Text = $"{splitMessage[0]}" });
                                                messageText.Inlines.Add(NameRun);
                                                messageText.Inlines.Add(ExtensionRun);
                                                messageText.Inlines.Add(new Run { Text = $"{splitMessage[1]}" });

                                            }
                                            else
                                            {
                                                messageText.Inlines.Add(new Run { Text = $"{message}" });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    messageText.Inlines.Add(new Run { Text = $"{message}" });
                                }
                            }
                            else
                            {
                                messageText.Inlines.Add(new Run { Text = $"{message}" });
                            }
                        }
                        else
                        {
                            messageText.Inlines.Add(new Run { Text = $"{message}" });
                        }
                    }
                    catch (Exception e)
                    {
                        //Helpers.Logger(e);
                        messageText.Text = message;
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Logger(e);
                messageText.Text = message;
            }

            localMessage = message;
            if (button1.Length == 0)
            {
                btn1.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn1.Content = button1;
                if (button1Temp != null)
                {
                    try
                    {
                        btn1.Tag = button1Temp;
                    }
                    catch(Exception ex)
                    {

                    }
                }
                else
                {
                    try
                    {
                        btn1.Tag = button1;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                ButtonColor(btn1);
            }
            btn1.Content = button1;
            if (button2.Length == 0)
            {
                btn2.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn2.Content = button2;
                if (button2Temp != null)
                {
                    try
                    {
                        btn2.Tag = button2Temp;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    try
                    {
                        btn2.Tag = button2;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                ButtonColor(btn2);
            }

            btn3.Content = button3;
            if (button3Temp != null)
            {
                try
                {
                    btn3.Tag = button3Temp;
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                try
                {
                    btn3.Tag = button3;
                }
                catch (Exception ex)
                {

                }
            }
            ButtonColor(btn3);
        }
        private void ButtonColor(Button btn)
        {
            try
            {
                try
                {
                    if (btn.Tag.ToString().ToLower().Contains("update"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("yes"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("export"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("package"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("assets"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("download"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("import"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("cancel"))
                    {
                        btn.Background = new SolidColorBrush(Colors.DarkOrange);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("delete"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("+data"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("remove"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("abort"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("ignore"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("unpin"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("clear"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("reload"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().StartsWith("list"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("install"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("open"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("privacy"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("github"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("restart"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("shortcuts"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("create"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("shutdown"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("extract"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("completed"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("all"))
                    {
                        btn.Background = new SolidColorBrush(Colors.DarkOrange);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("disable"))
                    {
                        btn.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("single"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("start"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("pin"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Equals("group"))
                    {
                        //btn.Background = new SolidColorBrush(Colors.GreenYellow);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch(Exception ex)
            {
                try
                {
                    if (btn.Content.ToString().ToLower().Contains("update"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("yes"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("export"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("package"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("assets"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("download"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("import"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("cancel"))
                    {
                        btn.Background = new SolidColorBrush(Colors.DarkOrange);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("delete"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("+data"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("remove"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("abort"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("ignore"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("unpin"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("clear"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("reload"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().StartsWith("list"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("install"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("open"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("privacy"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("github"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("restart"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("shortcuts"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("create"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Tag.ToString().ToLower().Contains("shutdown"))
                    {
                        btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("extract"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Contains("completed"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("all"))
                    {
                        btn.Background = new SolidColorBrush(Colors.DarkOrange);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("disable"))
                    {
                        btn.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("single"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("start"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("pin"))
                    {
                        btn.Background = new SolidColorBrush(Colors.ForestGreen);
                    }
                    else
                    if (btn.Content.ToString().ToLower().Equals("group"))
                    {
                        //btn.Background = new SolidColorBrush(Colors.GreenYellow);
                    }
                }
                catch (Exception exx)
                {

                }
            }
        }
        public ContentDialogCustom(string title, Run[] message, string button1, string button2, string button3, bool telnetOutput = false, string button1Temp = null, string button2Temp = null, string button3Temp = null)
        {
            this.InitializeComponent();
            this.Result = DialogResultCustom.Nothing;
            this.Title = title;
            localTitle = title;
            messageText.Inlines.Clear();
            if (!Helpers.DeviceIsPhone())
            {
                btn1.Margin = new Thickness(5, 1, 5, 10);
                btn2.Margin = new Thickness(5, 1, 5, 10);
                btn3.Margin = new Thickness(5, 1, 5, 10);
            }
            if (telnetOutput)
            {
                messageText.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                DialogContainer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            foreach (var runItem in message)
            {
                runItem.Text = Helpers.escapeSpecialChars(runItem.Text);
                if (runItem.Text.Length == 0)
                {
                    messageText.Inlines.Add(new LineBreak());
                    localMessage += "\n";
                }
                else
                {
                    localMessage += runItem.Text;
                    try
                    {
                        var testIfPath = WebHelper.isLinkFile(runItem.Text);
                        if (testIfPath)
                        {
                            var Name = "";
                            try
                            {
                                Name = Path.GetFileNameWithoutExtension(runItem.Text);
                            }
                            catch (Exception ex)
                            {

                            }
                            if (Name.Length > 0)
                            {
                                var Extension = Path.GetExtension(runItem.Text);
                                if (Extension != null && Extension.Length > 0 && !Extension.Contains(" ") && !Extension.StartsWith(".0") && !Extension.StartsWith(".1") && !Extension.StartsWith(".2") && !Extension.StartsWith(".4") && !Extension.StartsWith(".5") && !Extension.StartsWith(".6") && !Extension.StartsWith(".8") && !Extension.StartsWith(".9"))
                                {
                                    var NameRun = new Run { Text = Name, Foreground = new SolidColorBrush(Colors.SeaGreen) };
                                    var ExtensionRun = new Run { Text = Extension.ToLower(), Foreground = new SolidColorBrush(Colors.SkyBlue), FontWeight = FontWeights.Medium };
                                    messageText.Inlines.Add(NameRun);
                                    messageText.Inlines.Add(ExtensionRun);
                                }
                                else
                                {
                                    messageText.Inlines.Add(runItem);
                                }
                            }
                            else
                            {
                                messageText.Inlines.Add(runItem);
                            }
                        }
                        else
                        {
                            messageText.Inlines.Add(runItem);
                        }
                    }
                    catch (Exception e)
                    {
                        messageText.Inlines.Add(runItem);
                    }

                    if (message.Length > 1)
                    {
                    }
                }
            }

            try
            {
                //localMessage = message.ToString();
            }
            catch (Exception e)
            {

            }
            if (button1.Length == 0)
            {
                btn1.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn1.Content = button1;
                if (button1Temp != null)
                {
                    try
                    {
                        btn1.Tag = button1Temp;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    try
                    {
                        btn1.Tag = button1;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                ButtonColor(btn1);
            }
            btn1.Content = button1;
            if (button2.Length == 0)
            {
                btn2.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn2.Content = button2;
                if (button2Temp != null)
                {
                    try
                    {
                        btn2.Tag = button2Temp;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    try
                    {
                        btn2.Tag = button2;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                ButtonColor(btn2);
            }

            btn3.Content = button3;
            if (button3Temp != null)
            {
                try
                {
                    btn3.Tag = button3Temp;
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                try
                {
                    btn3.Tag = button3;
                }
                catch (Exception ex)
                {

                }
            }
            ButtonColor(btn3);
        }
        public async Task<Enum> ShowAsync2(DialogResultCustom forceReturn = DialogResultCustom.Nothing)
        {
            try
            {
                bool showdevModeCheck = false;
                if (btn2.Content.ToString().Equals("Install"))
                {
                    showdevModeCheck = true;
                }
                if (showdevModeCheck)
                {
                    devmodeGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    devmodeGrid.Visibility = Visibility.Collapsed;
                }
                devmodeCheck.IsChecked = Helpers.devModeActive;
            }
            catch(Exception ex)
            {
                Helpers.Logger(ex);
            }

            try
            {
                switch (Helpers.AppTheme)
                {
                    case "System":
                        RequestedTheme = ElementTheme.Default;
                        break;

                    case "Dark":
                        RequestedTheme = ElementTheme.Dark;
                        break;

                    case "Light":
                        RequestedTheme = ElementTheme.Light;
                        break;
                }
            }
            catch (Exception e)
            {

            }

            Helpers.ChangeDialogBackgroudn(this);
            if (!forceReturn.Equals(DialogResultCustom.Nothing))
            {
                this.Result = forceReturn;
                return forceReturn;
            }
            if (Helpers.DialogInProgress)
            {
                try
                {
                    int totalWait = 0;
                    while (Helpers.DialogInProgress)
                    {
                        if (totalWait > 7)
                        {
                            break;
                        }
                        await Task.Delay(2000);
                        totalWait++;
                    }
                    if (totalWait > 7)
                    {
                        this.Result = DialogResultCustom.Cancle;
                        Helpers.ShowErrorNotification(localTitle, localMessage);
                    }
                    else
                    {
                        Helpers.DialogInProgress = true;
                        await dialog.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    this.Result = DialogResultCustom.Cancle;
                    Helpers.ShowErrorNotification(localTitle, localMessage);
                }
            }
            else
            {
                Helpers.DialogInProgress = true;
                await dialog.ShowAsync();
            }
            Helpers.DialogInProgress = false;
            devModeActiveDialog = devmodeCheck.IsChecked.Value;
            return DialogResultCustom.Cancle;
        }

        // Handle the button clicks from dialog
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this.Result = DialogResultCustom.Yes;
            // Close the dialog
            dialog.Hide();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            this.Result = DialogResultCustom.No;
            // Close the dialog
            dialog.Hide();
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            this.Result = DialogResultCustom.Cancle;
            // Close the dialog
            dialog.Hide();
        }

    }
}
