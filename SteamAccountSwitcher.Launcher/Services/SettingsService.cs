using System;
using System.IO;
using System.Text.Json;
using SteamAccountSwitcher.Launcher.Models;

namespace SteamAccountSwitcher.Launcher.Services
{
    public class SettingsService
    {
        private readonly string _settingsFolder;
        private readonly string _settingsPath;
        private AppSettings _settings;

        public AppSettings Settings => _settings;

        public SettingsService()
        {
            _settingsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SteamAccountSwitcherPro"
            );
            _settingsPath = Path.Combine(_settingsFolder, "settings.json");
            _settings = LoadSettings();
        }

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null)
                        return loaded;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }

            return new AppSettings();
        }

        public void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(_settingsFolder);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        public AccountCustomData GetAccountData(string steamId)
        {
            if (_settings.AccountsData.TryGetValue(steamId, out var data))
                return data;

            var newData = new AccountCustomData();
            _settings.AccountsData[steamId] = newData;
            return newData;
        }

        public void SetAccountData(string steamId, AccountCustomData data)
        {
            _settings.AccountsData[steamId] = data;
            SaveSettings();
        }
    }
}
