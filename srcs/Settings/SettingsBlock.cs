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
using Windows.UI.Xaml.Controls;

namespace WinUniversalTool.Settings
{
    public class SettingsBlock
    {
        public ObservableCollection<SettingsTab> SettingsTabs;

        [JsonIgnore]
        public Pivot Settings;

        [JsonConstructor]
        public SettingsBlock(ObservableCollection<SettingsTab> settingsTabs)
        {
            try
            {
                if (settingsTabs == null)
                {
                    return;
                }
                SettingsTabs = settingsTabs;
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
                Settings = new Pivot();
                foreach (var tabItem in SettingsTabs)
                {
                    tabItem.Build();
                    Settings.Items.Add(tabItem.TabObject);
                }
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }

        public void SetValue(object tab, string name, object value, bool scriptRequests = false)
        {
            try
            {
                foreach (var tabItem in SettingsTabs)
                {
                    if (tabItem.Name.Equals((string)tab))
                    {
                        if (scriptRequests)
                        {
                            tabItem.SetValue(name, value);
                        }
                        else
                        {
                            tabItem.SetValue(name.ToLower(), value);
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public void SetValue(string name, object value, bool scriptRequests = false)
        {
            try
            {
                foreach (var tabItem in SettingsTabs)
                {
                    foreach (var objectItem in tabItem.SettingsObjects)
                    {
                        if (objectItem.Name.Equals(name.ToLower()))
                        {
                            if (scriptRequests)
                            {
                                tabItem.SetValue(name, value);
                            }
                            else
                            {
                                tabItem.SetValue(name.ToLower(), value);
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        public T GetValue<T>(object tab, string name)
        {
            try
            {
                foreach (var tabItem in SettingsTabs)
                {
                    if (tabItem.Name.Equals((string)tab))
                    {
                        return tabItem.GetValue<T>(name.ToLower());
                    }
                }
            }
            catch (Exception e)
            {

            }
            return default(T);
        }
        public T GetValue<T>(string name, T defaultReturn = default(T))
        {
            try
            {
                foreach (var tabItem in SettingsTabs)
                {
                    foreach (var objectItem in tabItem.SettingsObjects)
                    {
                        if (objectItem.Name.Equals(name.ToLower()))
                        {
                            return tabItem.GetValue<T>(name.ToLower());
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return defaultReturn;
        }
    }
}
