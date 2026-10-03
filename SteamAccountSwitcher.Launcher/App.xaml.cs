using System;
using System.IO;
using System.Windows;

namespace SteamAccountSwitcher.Launcher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                LogCrash("AppDomain.UnhandledException", args.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                LogCrash("DispatcherUnhandledException", args.Exception);
                args.Handled = true;
            };

            base.OnStartup(e);

            try
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                LogCrash("MainWindow_Initialization", ex);
            }
        }

        private static void LogCrash(string source, Exception? ex)
        {
            try
            {
                var msg = $"[{DateTime.Now}] Crash in {source}:\n{ex?.ToString()}\n";
                File.AppendAllText("FATAL_STARTUP.txt", msg);
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FATAL_STARTUP.txt"), msg);
            }
            catch { }
        }
    }
}
