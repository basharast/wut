/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinUniversalTool.Settings
{
    public class SettingsTab
    {
        public string Name;
        public string Title;
        public Symbol Icon;
        public ObservableCollection<SettingsObject> SettingsObjects;

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
        public PivotItem TabObject;

        [JsonIgnore]
        public StackPanel contents;

        [JsonIgnore]
        public ScrollViewer scrollViewer;

        [JsonConstructor]
        public SettingsTab(string name, string title, Symbol icon, ObservableCollection<SettingsObject> settingsObjects)
        {
            title = getLocalString(title);
            try
            {
                if (name == null)
                {
                    return;
                }
                Name = name.ToLower();
                Title = title;
                Icon = icon;
                SettingsObjects = settingsObjects;
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

                TabObject = new PivotItem();
                TabObject.Name = Name;

                int left = 0;
                int top = 3;
                int right = 0;
                int bottom = 10;

                var stackPanel = new StackPanel();
                SymbolIcon symbolIcon = new SymbolIcon(Icon);
                TextBlock textBlock = new TextBlock();

                textBlock.Text = Title;

                //stackPanel.Children.Add(symbolIcon);
                stackPanel.Children.Add(textBlock);

                TabObject.Header = stackPanel;

                scrollViewer = new ScrollViewer();
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
                //scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                contents = new StackPanel();
                contents.VerticalAlignment = VerticalAlignment.Stretch;
                contents.Margin = new Thickness(left, 15, right, bottom);
                foreach (var objectItem in SettingsObjects)
                {
                    objectItem.Tab = Name;
                    objectItem.Build();
                    if (objectItem.Type != ObjectTypes.CheckBox)
                    {
                        if (objectItem.Description.Length > 0)
                        {
                            TextBlock descriptions = new TextBlock();
                            descriptions.Text = objectItem.Description;
                            contents.Children.Add(descriptions);
                        }
                    }
                    switch (objectItem.Type)
                    {
                        case ObjectTypes.CheckBox:
                            var checkbox = (ToggleSwitch)objectItem.OutoutObject;
                            checkbox.Margin = new Thickness(left, top, right, 10);
                            if (objectItem.Description.Length > 0)
                            {
                                checkbox.Header = objectItem.Description;
                            }
                            checkbox.OnContent = "";
                            checkbox.OffContent = "";
                            contents.Children.Add(checkbox);
                            break;

                        case ObjectTypes.ComboBox:
                            var comboBox = ((Border)objectItem.OutoutObject);
                            comboBox.Margin = new Thickness(left, top, right, bottom);
                            contents.Children.Add(comboBox);
                            break;

                        case ObjectTypes.TextBlock:
                            var textBlock2 = ((Border)objectItem.OutoutObject);
                            textBlock2.Margin = new Thickness(left, top, right, bottom);
                            contents.Children.Add(textBlock2);
                            break;

                        case ObjectTypes.TextBox:
                            var textBox = ((Border)objectItem.OutoutObject);
                            textBox.Margin = new Thickness(left, top, right, bottom);
                            contents.Children.Add(textBox);
                            break;

                        case ObjectTypes.Slider:
                            break;
                    }
                }
                scrollViewer.Content = contents;
                TabObject.Content = scrollViewer;
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }
        public void SetValue(string name, object value)
        {
            try
            {
                foreach (var objectItem in SettingsObjects)
                {
                    if (objectItem.Name.Equals(name))
                    {
                        objectItem.SetValue(value);
                        break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public T GetValue<T>(string name)
        {
            try
            {
                foreach (var objectItem in SettingsObjects)
                {
                    if (objectItem.Name.Equals(name))
                    {
                        return (T)objectItem.GetValue();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return default(T);
        }
    }
}
