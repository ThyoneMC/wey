using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.CLI;
using wey.IO;
using static wey.ILauncher;

namespace wey
{
    public class ILauncher
    {
        public class IProfile
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = "custom";

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("gameDir")]
            public string? MinecraftPath { get; set; } = null;

            [JsonPropertyName("lastVersionId")]
            public string GameVersionID { get; set; } = string.Empty;

            [JsonPropertyName("javaArgs")]
            public string? JavaArguments { get; set; } = null;

            [JsonPropertyName("icon")]
            public string IconString { get; set; } = string.Empty;

            [JsonPropertyName("created")]
            public DateTime Created { get; set; }

            [JsonPropertyName("lastUsed")]
            public DateTime LastUsed { get; set; }
        }

        public class IProfileSettings
        {
            public bool crashAssistance { get; set; }
            public bool enableAdvanced { get; set; }
            public bool enableAnalytics { get; set; }
            public bool enableHistorical { get; set; }
            public bool enableReleases { get; set; }
            public bool enableSnapshots { get; set; }
            public bool keepLauncherOpen { get; set; }
            public string profileSorting { get; set; } = string.Empty;
            public bool showGameLog { get; set; }
            public bool showMenu { get; set; }
            public bool soundOn { get; set; }
        }

        public class ILauncherProfile
        {
            [JsonPropertyName("version")]
            public int Version { get; set; }

            [JsonPropertyName("profiles")]
            public Dictionary<string, IProfile> Profiles { get; set; } = new();

            [JsonPropertyName("settings")]
            public IProfileSettings Settings { get; set; } = new();
        }
    }

    public static class Launcher
    {
        public static readonly string MinecraftPath =
            ApplicationDirectoryHelper.GetDirectoryByOS(windows: ".minecraft", macos: "minecraft", linux: ".minecraft");

        public static void EditProfile(Func<ILauncherProfile, ILauncherProfile> editor)
        {
            string path = Path.Join(MinecraftPath, "launcher_profiles.json");

            ILauncherProfile launcherProfile = FileHelper.ReadJSON<ILauncherProfile>(path) ?? new();

            FileHelper.UpdateJSON(path, editor.Invoke(launcherProfile));
        }

        public class ProfileOptions
        {
            public required string Name;
            public string? MinecraftPath;
            public required string GameVersionID;
            public string? IconPath;
        }

        public static void AddProfile(ProfileOptions options)
        {
            Console.WriteLine($"Create Launcher Profile: {options.Name}");

            EditProfile((launcherProfile) =>
            {
                IProfile profile = new()
                {
                    Name = options.Name,
                    MinecraftPath = options.MinecraftPath ?? MinecraftPath,
                    GameVersionID = options.GameVersionID,
                };

                if (options.IconPath != null)
                {
                    byte[] image = FileHelper.ReadBytes(options.IconPath);
                    profile.IconString = $"data:image/{Path.GetExtension(options.IconPath)};base64,{Convert.ToBase64String(image)}";
                }

                launcherProfile.Profiles[options.Name] = profile;

                return launcherProfile;
            });
        }
    }
}
