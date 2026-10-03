using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using SteamAccountSwitcher.Launcher.Models;

namespace SteamAccountSwitcher.Launcher.Services
{
    public class SteamService
    {
        private const string SteamRegistrySubKey = @"Software\Valve\Steam";

        public string GetSteamPath()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(SteamRegistrySubKey);
                if (key?.GetValue("SteamPath") is string path && !string.IsNullOrWhiteSpace(path))
                {
                    return path.Replace('/', '\\');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get SteamPath from Registry: {ex.Message}");
            }

            // Fallbacks
            string[] commonPaths =
            {
                @"C:\Program Files (x86)\Steam",
                @"C:\Program Files\Steam",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam")
            };

            foreach (var cp in commonPaths)
            {
                if (Directory.Exists(cp))
                    return cp;
            }

            return string.Empty;
        }

        public string GetSteamExecutablePath()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(SteamRegistrySubKey);
                if (key?.GetValue("SteamExe") is string exe && !string.IsNullOrWhiteSpace(exe))
                {
                    return exe.Replace('/', '\\');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get SteamExe from Registry: {ex.Message}");
            }

            var steamPath = GetSteamPath();
            if (!string.IsNullOrEmpty(steamPath))
            {
                var candidate = Path.Combine(steamPath, "steam.exe");
                if (File.Exists(candidate))
                    return candidate;
            }

            return string.Empty;
        }

        public string? GetCurrentAutoLoginUser()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(SteamRegistrySubKey);
                return key?.GetValue("AutoLoginUser") as string;
            }
            catch
            {
                return null;
            }
        }

        public bool SetAutoLoginUser(string accountName)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(SteamRegistrySubKey, writable: true);
                if (key != null)
                {
                    key.SetValue("AutoLoginUser", accountName);
                    key.SetValue("RememberPassword", 1, RegistryValueKind.DWord);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to set AutoLoginUser: {ex.Message}");
            }
            return false;
        }

        public string GetLoginUsersVdfPath()
        {
            var steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
                return string.Empty;

            return Path.Combine(steamPath, "config", "loginusers.vdf");
        }

        public List<SteamAccount> GetAccounts()
        {
            var vdfPath = GetLoginUsersVdfPath();
            var accounts = VdfParser.ParseLoginUsers(vdfPath);
            var activeUser = GetCurrentAutoLoginUser();

            foreach (var acc in accounts)
            {
                acc.IsCurrentlyActive = !string.IsNullOrEmpty(activeUser) &&
                    string.Equals(acc.AccountName, activeUser, StringComparison.OrdinalIgnoreCase);
            }

            return accounts;
        }

        public bool IsSteamRunning(out int processId)
        {
            processId = 0;
            var processes = Process.GetProcessesByName("steam");
            if (processes.Length > 0)
            {
                processId = processes[0].Id;
                return true;
            }
            return false;
        }

        public async Task<bool> KillSteamAsync()
        {
            return await Task.Run(() =>
            {
                var processes = Process.GetProcessesByName("steam");
                if (processes.Length == 0)
                    return true;

                foreach (var p in processes)
                {
                    try
                    {
                        p.CloseMainWindow();
                        if (!p.WaitForExit(2000))
                        {
                            p.Kill(entireProcessTree: true);
                            p.WaitForExit(3000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to kill steam process: {ex.Message}");
                    }
                }

                // Verify
                var remaining = Process.GetProcessesByName("steam");
                return remaining.Length == 0;
            });
        }

        public void StartSteam(string arguments = "")
        {
            var exePath = GetSteamExecutablePath();
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                throw new FileNotFoundException("Steam.exe bulunamadı!");

            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(exePath) ?? ""
            };

            Process.Start(psi);
        }

        public async Task<bool> SwitchAccountAsync(string accountName, string extraArgs = "", string? runGameAppId = null)
        {
            // 1. Kill steam if running
            await KillSteamAsync();

            // 2. Set registry auto-login
            SetAutoLoginUser(accountName);

            // Wait a tiny bit for registry flush
            await Task.Delay(300);

            // 3. Build launch arguments
            var args = extraArgs.Trim();
            if (!string.IsNullOrWhiteSpace(runGameAppId))
            {
                args = $"-applaunch {runGameAppId.Trim()} {args}".Trim();
            }

            // 4. Start Steam
            StartSteam(args);
            return true;
        }

        public void OpenSteamFolder()
        {
            var path = GetSteamPath();
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
            }
        }

        public void OpenUserDataFolder()
        {
            var path = GetSteamPath();
            if (!string.IsNullOrEmpty(path))
            {
                var userData = Path.Combine(path, "userdata");
                if (Directory.Exists(userData))
                {
                    Process.Start("explorer.exe", userData);
                    return;
                }
            }
            OpenSteamFolder();
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }
    }
}
