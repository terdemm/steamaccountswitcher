using System.Collections.Generic;

namespace SteamAccountSwitcher.Launcher.Models
{
    public class AccountCustomData
    {
        public bool IsFavorite { get; set; }
        public string Note { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string TagColor { get; set; } = "#66c0f4";
        public string CustomLaunchArgs { get; set; } = string.Empty;
        public string AutoLaunchGameAppId { get; set; } = string.Empty;
    }

    public class AppSettings
    {
        public string SteamPath { get; set; } = string.Empty;
        public string SteamApiKey { get; set; } = string.Empty;
        public bool MinimizeToTray { get; set; } = true;
        public bool CloseToTray { get; set; } = false;
        public bool StartWithWindows { get; set; } = false;
        public bool LaunchBigPicture { get; set; } = false;
        public bool LaunchSilent { get; set; } = false;
        public string GlobalLaunchArgs { get; set; } = string.Empty;
        public Dictionary<string, AccountCustomData> AccountsData { get; set; } = new();
    }
}
