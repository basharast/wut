/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using WinUniversalTool.WebViewer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace WinUniversalTool.Settings
{
    public class SettingsObject : BindableBase
    {
        public string Tab;
        public string Name;
        public object Value;
        public object ReturnValue;
        public string Description;
        public string Hint;
        public int Type;
        public string Trigger;
        public bool ScriptRequest;
        public string getLocalString(string name)
        {
            var value = name;
            try
            {
                var temp = Helpers.getLocalString(name.Replace("...", "").Replace("..", "").Replace("!", ""));
                if (temp.Length > 0) { value = temp; }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        [JsonIgnore]
        ToggleSwitch checkBox;
        [JsonIgnore]
        ComboBox comboBox;
        [JsonIgnore]
        TextBlock textBlock;
        [JsonIgnore]
        TextBox textBox;

        [JsonIgnore]
        public object OutoutObject;

        [JsonIgnore]
        public Type OutputType;

        [JsonConstructor]
        public SettingsObject(string name, object value, string description, int type, string trigger = null, string hint = "", bool scriptRequest = false)
        {
            description = getLocalString(description);
            try
            {
                if (name == null)
                {
                    return;
                }
                ScriptRequest = scriptRequest;
                if (scriptRequest)
                {
                    Name = name;
                }
                else
                {
                    Name = name.ToLower();
                }
                Value = value;
                Hint = hint;
                Description = description;
                Type = type;
                Trigger = trigger;
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }

        public void Build()
        {
            try
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);
                border.HorizontalAlignment = HorizontalAlignment.Left;
                border.CornerRadius = new CornerRadius(2);
                switch (Type)
                {
                    case ObjectTypes.CheckBox:
                        checkBox = new ToggleSwitch();
                        checkBox.Tag = Tab;
                        checkBox.Name = Name;
                        checkBox.Header = Description;
                        checkBox.OnContent = "";
                        checkBox.OffContent = "";
                        var convertedValue = false;
                        try
                        {
                            convertedValue = (bool)Value;
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                convertedValue = JsonConvert.DeserializeObject<bool>((string)Value);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        checkBox.IsOn = convertedValue;
                        ReturnValue = Value;
                        if (ScriptRequest)
                        {
                            Variables.Define(Name, ReturnValue.ToString());
                        }
                        OutoutObject = checkBox;
                        checkBox.Toggled += CheckBox_Tapped;
                        OutputType = typeof(ToggleSwitch);
                        break;

                    case ObjectTypes.ComboBox:
                        var items = new string[] { "Empty" };
                        try
                        {
                            items = JsonConvert.DeserializeObject<string[]>((string)Value);
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                items = (string[])Value;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        comboBox = new ComboBox();
                        comboBox.Tag = Tab;
                        comboBox.Name = Name;
                        comboBox.ItemsSource = items;
                        if (ReturnValue != null)
                        {
                            comboBox.SelectedItem = ReturnValue;
                        }
                        else
                        {
                            if (Hint != null && Hint.Length > 0)
                            {
                                comboBox.SelectedItem = Hint;
                                ReturnValue = Hint;
                            }
                            else
                            {
                                comboBox.SelectedIndex = 0;
                                ReturnValue = comboBox.SelectedItem;
                            }
                        }
                        if (ScriptRequest)
                        {
                            Variables.Define(Name, ReturnValue.ToString());
                        }
                        comboBox.SelectionChanged += ComboBox_SelectionChanged;
                        comboBox.BorderThickness = new Thickness(0);

                        
                        border.Child = comboBox;
                        OutoutObject = border;
                        OutputType = typeof(ComboBox);
                        break;

                    case ObjectTypes.TextBlock:
                        textBlock = new TextBlock();
                        textBlock.Tag = Tab;
                        textBlock.Name = Name;
                        textBlock.Text = (string)Value;
                        ReturnValue = Value;
                        border.Child = textBlock;
                        OutoutObject = border;
                        OutputType = typeof(TextBlock);
                        break;

                    case ObjectTypes.TextBox:
                        textBox = new TextBox();
                        textBox.Tag = Tab;
                        textBox.Name = Name;
                        textBox.Text = (string)Value;
                        ReturnValue = Value;
                        if (ScriptRequest)
                        {
                            Variables.Define(Name, ReturnValue.ToString());
                        }
                        textBox.PlaceholderText = Hint;
                        textBox.BorderThickness = new Thickness(0);
                        textBox.TextChanged += TextBox_TextChanged;
                        textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                        textBox.Padding = new Thickness(5, 5, 0, 0);
                        border.Margin = new Thickness(10, 0, 10, 0);
                        border.HorizontalAlignment = HorizontalAlignment.Stretch;
                        border.Child = textBox;
                        OutoutObject = border;
                        OutputType = typeof(TextBox);
                        break;

                    case ObjectTypes.Slider:
                        break;

                }

            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }

        bool UpdatedByTrigger = false;
        public void SetValue(object value)
        {
            try
            {
                switch (Type)
                {
                    case ObjectTypes.CheckBox:
                        checkBox.Header = Description;
                        Value = value;
                        ReturnValue = value;
                        break;

                    case ObjectTypes.ComboBox:
                        ReturnValue = value;
                        break;

                    case ObjectTypes.TextBlock:
                        Value = value;
                        ReturnValue = value;
                        break;

                    case ObjectTypes.TextBox:
                        Value = value;
                        ReturnValue = value;
                        break;

                    case ObjectTypes.Slider:
                        Value = value;
                        ReturnValue = value;
                        break;

                }
                UpdatedByTrigger = true;
            }
            catch (Exception e)
            {

            }
        }
        public object GetValue()
        {
            try
            {
                switch (Type)
                {
                    case ObjectTypes.CheckBox:
                        return ReturnValue;

                    case ObjectTypes.ComboBox:
                        return ReturnValue;

                    case ObjectTypes.TextBlock:
                        return ReturnValue;

                    case ObjectTypes.TextBox:
                        return ReturnValue;

                    case ObjectTypes.Slider:
                        return ReturnValue;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!ScriptRequest)
                {
                    if (!UpdatedByTrigger)
                    {
                        var TriggerLocal = SettingsHelper.GetHandlerByName(Trigger);
                        if (TriggerLocal != null)
                        {
                            TriggerLocal.Invoke(sender, EventArgs.Empty);
                        }
                    }
                }
                else if (!UpdatedByTrigger)
                {
                    var TriggerLocal = SettingsHelper.GetHandlerByNameScript(Trigger);
                    if (TriggerLocal != null)
                    {
                        TriggerLocal.Invoke(sender, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            UpdatedByTrigger = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!ScriptRequest)
                {
                    if (!UpdatedByTrigger)
                    {
                        var TriggerLocal = SettingsHelper.GetHandlerByName(Trigger);
                        if (TriggerLocal != null)
                        {
                            TriggerLocal.Invoke(sender, EventArgs.Empty);
                            WebHelper.SaveSettingsHandler.Invoke(null, EventArgs.Empty);
                            if (Name.Equals("translateto"))
                            {
                                var ValueSe = GetValue().ToString();
                                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Name, ValueSe);
                            }
                            else
                            if (Name.Equals("DownloadBoostExtra".ToLower()))
                            {
                                var ValueSe = GetValue().ToString();
                                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Name, ValueSe);
                            }
                        }
                    }
                }
                else if (!UpdatedByTrigger)
                {
                    var TriggerLocal = SettingsHelper.GetHandlerByNameScript(Trigger);
                    if (TriggerLocal != null)
                    {
                        TriggerLocal.Invoke(sender, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            UpdatedByTrigger = false;
        }

        private void CheckBox_Tapped(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ScriptRequest)
                {
                    if (!UpdatedByTrigger)
                    {
                        var TriggerLocal = SettingsHelper.GetHandlerByName(Trigger);
                        if (TriggerLocal != null)
                        {
                            TriggerLocal.Invoke(sender, EventArgs.Empty);
                            WebHelper.SaveSettingsHandler.Invoke(null, EventArgs.Empty);
                            try
                            {
                                RaisePropertyChanged(Name);
                            }
                            catch (Exception ex)
                            {

                            }
                            if (Name.Equals("BoostDownloadSpeed".ToLower()))
                            {
                                var ValueSe = (bool)GetValue();
                                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Name, ValueSe);
                            }
                            else
                            if (Name.Equals("ConfirmTranlate".ToLower()))
                            {
                                var ValueSe = (bool)GetValue();
                                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Name, ValueSe);
                            }
                            else
                            if (Name.Equals("WideTranslateButton".ToLower()))
                            {
                                var ValueSe = (bool)GetValue();
                                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(Name, ValueSe);
                            }
                        }
                    }
                }
                else if (!UpdatedByTrigger)
                {
                    var TriggerLocal = SettingsHelper.GetHandlerByNameScript(Trigger);
                    if (TriggerLocal != null)
                    {
                        TriggerLocal.Invoke(sender, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            UpdatedByTrigger = false;
        }
    }
}
