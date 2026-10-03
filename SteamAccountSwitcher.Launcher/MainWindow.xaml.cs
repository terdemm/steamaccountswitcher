using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using SteamAccountSwitcher.Launcher.Services;
using SteamAccountSwitcher.Launcher.ViewModels;

namespace SteamAccountSwitcher.Launcher
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private MainViewModel? ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            SetupTrayIcon();
        }

        private void SetupTrayIcon()
        {
            try
            {
                _notifyIcon = new NotifyIcon
                {
                    Text = "Steam Account Switcher Pro",
                    Visible = true
                };

                // Load icon from Assets if exists, or system icon
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico");
                if (File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                }
                else
                {
                    _notifyIcon.Icon = SystemIcons.Application;
                }

                _notifyIcon.DoubleClick += (s, e) => ShowAndRestore();

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Göster / Öne Getir", null, (s, e) => ShowAndRestore());
                contextMenu.Items.Add("Hesapları Yenile", null, async (s, e) =>
                {
                    if (ViewModel != null) await ViewModel.LoadAccountsAsync();
                });
                contextMenu.Items.Add(new ToolStripSeparator());
                contextMenu.Items.Add("Steam'i Kapat", null, async (s, e) =>
                {
                    if (ViewModel != null) await ViewModel.ForceKillSteamAsync();
                });
                contextMenu.Items.Add(new ToolStripSeparator());
                contextMenu.Items.Add("Uygulamadan Çık", null, (s, e) =>
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    System.Windows.Application.Current.Shutdown();
                });

                _notifyIcon.ContextMenuStrip = contextMenu;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to init NotifyIcon: {ex.Message}");
            }
        }

        private void ShowAndRestore()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            Focus();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    ToggleMaximize();
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.MinimizeToTray == true)
            {
                Hide();
            }
            else
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.CloseToTray == true)
            {
                Hide();
            }
            else
            {
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel?.CloseToTray == true)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            base.OnClosing(e);
        }

        private void FilterAll_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.SelectedFilterIndex = 0;
        }

        private void FilterFav_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.SelectedFilterIndex = 1;
        }

        private void FilterActive_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.SelectedFilterIndex = 2;
        }

        private void SteamApiKeyLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SteamService.OpenUrl("https://steamcommunity.com/dev/apikey");
        }
    }
}
