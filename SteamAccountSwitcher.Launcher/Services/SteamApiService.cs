using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SteamAccountSwitcher.Launcher.Models;

namespace SteamAccountSwitcher.Launcher.Services
{
    public class SteamApiService
    {
        private readonly HttpClient _http;
        private readonly string _avatarCacheFolder;

        public SteamApiService()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            _avatarCacheFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SteamAccountSwitcherPro",
                "avatars"
            );
            Directory.CreateDirectory(_avatarCacheFolder);
        }

        public async Task EnrichAccountsAsync(List<SteamAccount> accounts, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || accounts.Count == 0)
                return;

            try
            {
                var steamIds = string.Join(",", accounts.Select(a => a.SteamId64));
                var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamIds}";

                var res = await _http.GetStringAsync(url);
                using var doc = JsonDocument.Parse(res);
                if (doc.RootElement.TryGetProperty("response", out var resp) &&
                    resp.TryGetProperty("players", out var players) &&
                    players.ValueKind == JsonValueKind.Array)
                {
                    foreach (var p in players.EnumerateArray())
                    {
                        var sid = p.GetProperty("steamid").GetString();
                        if (string.IsNullOrEmpty(sid)) continue;
                        var acc = accounts.FirstOrDefault(a => a.SteamId64 == sid);
                        if (acc != null)
                        {
                            if (p.TryGetProperty("avatarfull", out var av))
                            {
                                var remoteAvatarUrl = av.GetString();
                                if (!string.IsNullOrEmpty(remoteAvatarUrl))
                                {
                                    var localAvatar = await CacheAvatarAsync(sid, remoteAvatarUrl);
                                    acc.AvatarUrl = localAvatar ?? remoteAvatarUrl;
                                }
                            }
                            if (p.TryGetProperty("personaname", out var pn))
                            {
                                acc.PersonaName = pn.GetString() ?? acc.PersonaName;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching player summaries: {ex.Message}");
            }

            // Bans
            try
            {
                var steamIds = string.Join(",", accounts.Select(a => a.SteamId64));
                var url = $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={apiKey}&steamids={steamIds}";

                var res = await _http.GetStringAsync(url);
                using var doc = JsonDocument.Parse(res);
                if (doc.RootElement.TryGetProperty("players", out var bans) &&
                    bans.ValueKind == JsonValueKind.Array)
                {
                    foreach (var b in bans.EnumerateArray())
                    {
                        var sid = b.GetProperty("SteamId").GetString();
                        var acc = accounts.FirstOrDefault(a => a.SteamId64 == sid);
                        if (acc != null)
                        {
                            var vac = b.TryGetProperty("VACBanned", out var vb) && vb.GetBoolean();
                            var gameBans = b.TryGetProperty("NumberOfGameBans", out var gb) ? gb.GetInt32() : 0;
                            var comm = b.TryGetProperty("CommunityBanned", out var cb) && cb.GetBoolean();
                            acc.IsVacBanned = vac || gameBans > 0 || comm;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching bans: {ex.Message}");
            }
        }

        private async Task<string?> CacheAvatarAsync(string steamId, string url)
        {
            try
            {
                var localFile = Path.Combine(_avatarCacheFolder, $"{steamId}.jpg");
                if (File.Exists(localFile))
                    return localFile;

                var bytes = await _http.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(localFile, bytes);
                return localFile;
            }
            catch
            {
                return null;
            }
        }
    }
}
