using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteamAccountSwitcher.Launcher.Models
{
    public class SteamAccount : INotifyPropertyChanged
    {
        public string SteamId64 { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string PersonaName { get; set; } = string.Empty;
        public bool RememberPassword { get; set; }
        public bool MostRecent { get; set; }
        public long Timestamp { get; set; }

        private bool _isCurrentlyActive;
        public bool IsCurrentlyActive
        {
            get => _isCurrentlyActive;
            set { _isCurrentlyActive = value; OnPropertyChanged(); }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }

        private string _customNote = string.Empty;
        public string CustomNote
        {
            get => _customNote;
            set { _customNote = value; OnPropertyChanged(); }
        }

        private string _customTag = string.Empty;
        public string CustomTag
        {
            get => _customTag;
            set { _customTag = value; OnPropertyChanged(); }
        }

        private string _tagColor = "#66c0f4";
        public string TagColor
        {
            get => _tagColor;
            set { _tagColor = value; OnPropertyChanged(); }
        }

        private string _avatarUrl = string.Empty;
        public string AvatarUrl
        {
            get => _avatarUrl;
            set { _avatarUrl = value; OnPropertyChanged(); }
        }

        private int _level = -1;
        public int Level
        {
            get => _level;
            set { _level = value; OnPropertyChanged(); }
        }

        private bool _isVacBanned;
        public bool IsVacBanned
        {
            get => _isVacBanned;
            set { _isVacBanned = value; OnPropertyChanged(); }
        }

        public string DisplayName => string.IsNullOrWhiteSpace(PersonaName) ? AccountName : PersonaName;
        
        public string LastLoginFormatted
        {
            get
            {
                if (Timestamp <= 0) return "Bilinmiyor";
                try
                {
                    var dt = DateTimeOffset.FromUnixTimeSeconds(Timestamp).LocalDateTime;
                    return dt.ToString("dd.MM.yyyy HH:mm");
                }
                catch
                {
                    return "Bilinmiyor";
                }
            }
        }

        public string AvatarInitial
        {
            get
            {
                var name = DisplayName;
                return string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpperInvariant();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
