using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SteamAccountSwitcher.Launcher.Models;
using SteamAccountSwitcher.Launcher.Services;

namespace SteamAccountSwitcher.Launcher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SteamService _steamService;
        private readonly SettingsService _settingsService;
        private readonly SteamApiService _apiService;
        private readonly DispatcherTimer _pollTimer;

        public ObservableCollection<SteamAccount> Accounts { get; } = new();

        private List<SteamAccount> _rawAccounts = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        private int _selectedFilterIndex = 0; // 0: All, 1: Favorites, 2: Active
        public int SelectedFilterIndex
        {
            get => _selectedFilterIndex;
            set
            {
                if (_selectedFilterIndex != value)
                {
                    _selectedFilterIndex = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        private bool _isSteamRunning;
        public bool IsSteamRunning
        {
            get => _isSteamRunning;
            set { _isSteamRunning = value; OnPropertyChanged(); }
        }

        private int _steamProcessId;
        public int SteamProcessId
        {
            get => _steamProcessId;
            set { _steamProcessId = value; OnPropertyChanged(); }
        }

        private string _currentAutoLoginUser = string.Empty;
        public string CurrentAutoLoginUser
        {
            get => _currentAutoLoginUser;
            set { _currentAutoLoginUser = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isStatusMessageVisible;
        public bool IsStatusMessageVisible
        {
            get => _isStatusMessageVisible;
            set { _isStatusMessageVisible = value; OnPropertyChanged(); }
        }

        private bool _isSettingsOpen;
        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set { _isSettingsOpen = value; OnPropertyChanged(); }
        }

        private SteamAccount? _editingAccount;
        public SteamAccount? EditingAccount
        {
            get => _editingAccount;
            set { _editingAccount = value; OnPropertyChanged(); }
        }

        // Account edit temporary properties
        private string _editNote = string.Empty;
        public string EditNote
        {
            get => _editNote;
            set { _editNote = value; OnPropertyChanged(); }
        }

        private string _editTag = string.Empty;
        public string EditTag
        {
            get => _editTag;
            set { _editTag = value; OnPropertyChanged(); }
        }

        private string _editTagColor = "#66c0f4";
        public string EditTagColor
        {
            get => _editTagColor;
            set { _editTagColor = value; OnPropertyChanged(); }
        }

        private string _editLaunchArgs = string.Empty;
        public string EditLaunchArgs
        {
            get => _editLaunchArgs;
            set { _editLaunchArgs = value; OnPropertyChanged(); }
        }

        private string _editFastLaunchAppId = string.Empty;
        public string EditFastLaunchAppId
        {
            get => _editFastLaunchAppId;
            set { _editFastLaunchAppId = value; OnPropertyChanged(); }
        }

        // Settings Proxy Properties
        public string SteamApiKey
        {
            get => _settingsService.Settings.SteamApiKey;
            set
            {
                _settingsService.Settings.SteamApiKey = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        public bool MinimizeToTray
        {
            get => _settingsService.Settings.MinimizeToTray;
            set
            {
                _settingsService.Settings.MinimizeToTray = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        public bool CloseToTray
        {
            get => _settingsService.Settings.CloseToTray;
            set
            {
                _settingsService.Settings.CloseToTray = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        public bool LaunchBigPicture
        {
            get => _settingsService.Settings.LaunchBigPicture;
            set
            {
                _settingsService.Settings.LaunchBigPicture = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        public bool LaunchSilent
        {
            get => _settingsService.Settings.LaunchSilent;
            set
            {
                _settingsService.Settings.LaunchSilent = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        public string GlobalLaunchArgs
        {
            get => _settingsService.Settings.GlobalLaunchArgs;
            set
            {
                _settingsService.Settings.GlobalLaunchArgs = value;
                _settingsService.SaveSettings();
                OnPropertyChanged();
            }
        }

        // Commands
        public RelayCommand SwitchAccountCommand { get; }
        public RelayCommand ToggleFavoriteCommand { get; }
        public RelayCommand EditAccountCommand { get; }
        public RelayCommand SaveAccountEditCommand { get; }
        public RelayCommand CancelAccountEditCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand KillSteamCommand { get; }
        public RelayCommand RestartSteamCommand { get; }
        public RelayCommand OpenSteamFolderCommand { get; }
        public RelayCommand OpenUserDataFolderCommand { get; }
        public RelayCommand AddNewAccountCommand { get; }
        public RelayCommand OpenProfileCommand { get; }
        public RelayCommand OpenSteamRepCommand { get; }
        public RelayCommand OpenSteamDbCommand { get; }
        public RelayCommand CopySteamIdCommand { get; }
        public RelayCommand OpenSteamStatCommand { get; }
        public RelayCommand ToggleSettingsCommand { get; }

        public MainViewModel()
        {
            _steamService = new SteamService();
            _settingsService = new SettingsService();
            _apiService = new SteamApiService();

            SwitchAccountCommand = new RelayCommand(async obj =>
            {
                if (obj is SteamAccount acc)
                    await SwitchToAccountAsync(acc);
            });

            ToggleFavoriteCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc)
                    ToggleFavorite(acc);
            });

            EditAccountCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc)
                    BeginEditAccount(acc);
            });

            SaveAccountEditCommand = new RelayCommand(_ => SaveAccountEdit());
            CancelAccountEditCommand = new RelayCommand(_ => EditingAccount = null);

            RefreshCommand = new RelayCommand(async () => await LoadAccountsAsync());
            KillSteamCommand = new RelayCommand(async () => await ForceKillSteamAsync());
            RestartSteamCommand = new RelayCommand(async () => await RestartSteamAsync());
            OpenSteamFolderCommand = new RelayCommand(() => _steamService.OpenSteamFolder());
            OpenUserDataFolderCommand = new RelayCommand(() => _steamService.OpenUserDataFolder());
            AddNewAccountCommand = new RelayCommand(async () => await AddNewAccountAsync());

            OpenProfileCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc && !string.IsNullOrEmpty(acc.SteamId64))
                    SteamService.OpenUrl($"https://steamcommunity.com/profiles/{acc.SteamId64}");
            });

            OpenSteamRepCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc && !string.IsNullOrEmpty(acc.SteamId64))
                    SteamService.OpenUrl($"https://steamrep.com/profiles/{acc.SteamId64}");
            });

            OpenSteamDbCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc && !string.IsNullOrEmpty(acc.SteamId64))
                    SteamService.OpenUrl($"https://steamdb.info/calculator/{acc.SteamId64}/");
            });

            CopySteamIdCommand = new RelayCommand(obj =>
            {
                if (obj is SteamAccount acc && !string.IsNullOrEmpty(acc.SteamId64))
                {
                    try
                    {
                        Clipboard.SetText(acc.SteamId64);
                        ShowNotification($"SteamID64 Kopyalandı: {acc.SteamId64}");
                    }
                    catch { }
                }
            });

            OpenSteamStatCommand = new RelayCommand(() => SteamService.OpenUrl("https://steamstat.us/"));
            ToggleSettingsCommand = new RelayCommand(() => IsSettingsOpen = !IsSettingsOpen);

            // Timer to check Steam process status
            _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _pollTimer.Tick += (s, e) => CheckSteamStatus();
            _pollTimer.Start();

            // Initial load
            _ = LoadAccountsAsync();
        }

        private void CheckSteamStatus()
        {
            var running = _steamService.IsSteamRunning(out var pid);
            IsSteamRunning = running;
            SteamProcessId = pid;

            var autoLogin = _steamService.GetCurrentAutoLoginUser() ?? "";
            if (autoLogin != CurrentAutoLoginUser)
            {
                CurrentAutoLoginUser = autoLogin;
                foreach (var a in _rawAccounts)
                {
                    a.IsCurrentlyActive = string.Equals(a.AccountName, autoLogin, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public async Task LoadAccountsAsync()
        {
            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    _rawAccounts = _steamService.GetAccounts();

                    // Apply stored metadata
                    foreach (var acc in _rawAccounts)
                    {
                        var data = _settingsService.GetAccountData(acc.SteamId64);
                        acc.IsFavorite = data.IsFavorite;
                        acc.CustomNote = data.Note;
                        acc.CustomTag = data.Tag;
                        acc.TagColor = string.IsNullOrWhiteSpace(data.TagColor) ? "#66c0f4" : data.TagColor;
                    }
                });

                CheckSteamStatus();
                ApplyFilter();

                // If API Key is present, enrich in background
                if (!string.IsNullOrWhiteSpace(SteamApiKey))
                {
                    _ = Task.Run(async () =>
                    {
                        await _apiService.EnrichAccountsAsync(_rawAccounts, SteamApiKey);
                        Application.Current?.Dispatcher.Invoke(ApplyFilter);
                    });
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Hesaplar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ApplyFilter()
        {
            var query = _rawAccounts.AsEnumerable();

            // Filter tabs
            if (SelectedFilterIndex == 1) // Favorites
                query = query.Where(a => a.IsFavorite);
            else if (SelectedFilterIndex == 2) // Currently Active
                query = query.Where(a => a.IsCurrentlyActive);

            // Text search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.Trim().ToLowerInvariant();
                query = query.Where(a =>
                    a.AccountName.ToLowerInvariant().Contains(s) ||
                    a.PersonaName.ToLowerInvariant().Contains(s) ||
                    a.SteamId64.Contains(s) ||
                    a.CustomNote.ToLowerInvariant().Contains(s) ||
                    a.CustomTag.ToLowerInvariant().Contains(s)
                );
            }

            // Ordering: Active first, then favorites, then most recent, then name
            var sorted = query
                .OrderByDescending(a => a.IsCurrentlyActive)
                .ThenByDescending(a => a.IsFavorite)
                .ThenByDescending(a => a.Timestamp)
                .ThenBy(a => a.DisplayName)
                .ToList();

            Accounts.Clear();
            foreach (var item in sorted)
            {
                Accounts.Add(item);
            }
        }

        public async Task SwitchToAccountAsync(SteamAccount account)
        {
            IsLoading = true;
            ShowNotification($"'{account.DisplayName}' hesabına geçiliyor...");

            try
            {
                var customData = _settingsService.GetAccountData(account.SteamId64);

                // Build launch arguments
                var args = new List<string>();
                if (LaunchBigPicture) args.Add("-bigpicture");
                if (LaunchSilent) args.Add("-silent");
                if (!string.IsNullOrWhiteSpace(GlobalLaunchArgs)) args.Add(GlobalLaunchArgs.Trim());
                if (!string.IsNullOrWhiteSpace(customData.CustomLaunchArgs)) args.Add(customData.CustomLaunchArgs.Trim());

                var finalArgs = string.Join(" ", args);
                var gameId = string.IsNullOrWhiteSpace(customData.AutoLaunchGameAppId) ? null : customData.AutoLaunchGameAppId.Trim();

                await _steamService.SwitchAccountAsync(account.AccountName, finalArgs, gameId);

                // Update active state
                CurrentAutoLoginUser = account.AccountName;
                foreach (var a in _rawAccounts)
                {
                    a.IsCurrentlyActive = string.Equals(a.AccountName, account.AccountName, StringComparison.OrdinalIgnoreCase);
                }
                ApplyFilter();

                ShowNotification($"Başarıyla '{account.DisplayName}' hesabına geçildi!");
            }
            catch (Exception ex)
            {
                ShowNotification($"Hesap değiştirilirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ForceKillSteamAsync()
        {
            ShowNotification("Steam kapatılıyor...");
            var success = await _steamService.KillSteamAsync();
            CheckSteamStatus();
            ShowNotification(success ? "Steam başarıyla kapatıldı." : "Steam kapatılamadı.");
        }

        public async Task RestartSteamAsync()
        {
            ShowNotification("Steam yeniden başlatılıyor...");
            await _steamService.KillSteamAsync();
            await Task.Delay(500);
            _steamService.StartSteam();
            ShowNotification("Steam başlatıldı.");
        }

        public async Task AddNewAccountAsync()
        {
            ShowNotification("Yeni hesap eklemek için Steam giriş ekranı açılıyor...");
            await _steamService.KillSteamAsync();
            _steamService.SetAutoLoginUser("");
            await Task.Delay(500);
            _steamService.StartSteam();
            ShowNotification("Steam açıldı. Lütfen hesabınızı girin ve 'Beni Hatırla'yı işaretleyin.");
        }

        public void ToggleFavorite(SteamAccount account)
        {
            account.IsFavorite = !account.IsFavorite;
            var data = _settingsService.GetAccountData(account.SteamId64);
            data.IsFavorite = account.IsFavorite;
            _settingsService.SetAccountData(account.SteamId64, data);
            ApplyFilter();
        }

        public void BeginEditAccount(SteamAccount account)
        {
            var data = _settingsService.GetAccountData(account.SteamId64);
            EditNote = data.Note;
            EditTag = data.Tag;
            EditTagColor = string.IsNullOrWhiteSpace(data.TagColor) ? "#66c0f4" : data.TagColor;
            EditLaunchArgs = data.CustomLaunchArgs;
            EditFastLaunchAppId = data.AutoLaunchGameAppId;
            EditingAccount = account;
        }

        public void SaveAccountEdit()
        {
            if (EditingAccount != null)
            {
                var data = _settingsService.GetAccountData(EditingAccount.SteamId64);
                data.Note = EditNote;
                data.Tag = EditTag;
                data.TagColor = EditTagColor;
                data.CustomLaunchArgs = EditLaunchArgs;
                data.AutoLaunchGameAppId = EditFastLaunchAppId;
                _settingsService.SetAccountData(EditingAccount.SteamId64, data);

                EditingAccount.CustomNote = EditNote;
                EditingAccount.CustomTag = EditTag;
                EditingAccount.TagColor = EditTagColor;

                EditingAccount = null;
                ApplyFilter();
                ShowNotification("Hesap detayları kaydedildi.");
            }
        }

        public void ShowNotification(string message)
        {
            StatusMessage = message;
            IsStatusMessageVisible = true;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            timer.Tick += (s, e) =>
            {
                IsStatusMessageVisible = false;
                timer.Stop();
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
