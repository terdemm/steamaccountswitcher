using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SteamAccountSwitcher.Launcher.Models;

namespace SteamAccountSwitcher.Launcher.Services
{
    public static class VdfParser
    {
        public static List<SteamAccount> ParseLoginUsers(string filePath)
        {
            var accounts = new List<SteamAccount>();
            if (!File.Exists(filePath))
                return accounts;

            try
            {
                var lines = File.ReadAllLines(filePath);
                string? currentSteamId = null;
                SteamAccount? currentAccount = null;

                // Regex patterns for key-value pair and section header
                var keyValueRegex = new Regex(@"^\s*""([^""]+)""\s+""([^""]*)""", RegexOptions.Compiled);
                var headerRegex = new Regex(@"^\s*""([^""]+)""", RegexOptions.Compiled);

                foreach (var rawLine in lines)
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line == "{" || line == "}")
                    {
                        if (line == "}" && currentAccount != null)
                        {
                            accounts.Add(currentAccount);
                            currentAccount = null;
                            currentSteamId = null;
                        }
                        continue;
                    }

                    var kvMatch = keyValueRegex.Match(line);
                    if (kvMatch.Success)
                    {
                        var key = kvMatch.Groups[1].Value;
                        var val = kvMatch.Groups[2].Value;

                        if (currentAccount != null)
                        {
                            switch (key.ToLowerInvariant())
                            {
                                case "accountname":
                                    currentAccount.AccountName = val;
                                    break;
                                case "personaname":
                                    currentAccount.PersonaName = val;
                                    break;
                                case "rememberpassword":
                                    currentAccount.RememberPassword = val == "1";
                                    break;
                                case "mostrecent":
                                    currentAccount.MostRecent = val == "1";
                                    break;
                                case "timestamp":
                                    if (long.TryParse(val, out var ts))
                                        currentAccount.Timestamp = ts;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // Check if it's a section header with 64-bit Steam ID (starts with 7656...)
                        var hMatch = headerRegex.Match(line);
                        if (hMatch.Success)
                        {
                            var header = hMatch.Groups[1].Value;
                            if (header.Length == 17 && header.StartsWith("7656"))
                            {
                                currentSteamId = header;
                                currentAccount = new SteamAccount
                                {
                                    SteamId64 = currentSteamId
                                };
                            }
                        }
                    }
                }

                if (currentAccount != null)
                {
                    accounts.Add(currentAccount);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing VDF: {ex.Message}");
            }

            return accounts;
        }
    }
}
