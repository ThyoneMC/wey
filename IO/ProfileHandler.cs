using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.API;

namespace wey.IO
{
    public class ISharedProfile
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }

        [JsonPropertyName("launcherIcon")]
        public string? LauncherIcon { get; set; } = null;

        [JsonPropertyName("mods")]
        public List<ModHandlerFile> Mods { get; set; } = new();
    }

    public static class ProfileHandler
    {
        static readonly string dirPath = Path.Join(ApplicationDirectoryHelper.Appdata, "profiles");

        static string GetPath(string name)
        {
            return Path.Join(dirPath, Path.ChangeExtension(name, "json"));
        }

        public static void Create(ISharedProfile profile)
        {
            FileHelper.UpdateJSON(GetPath(profile.Name), profile);
        }

        public static ISharedProfile? Read(string name)
        {
            return FileHelper.ReadJSON<ISharedProfile>(GetPath(name));
        }

        public static void Update(string name, ISharedProfile profile)
        {
            FileHelper.UpdateJSON(GetPath(name), profile);
        }

        public static void Edit(string name, Func<ISharedProfile, ISharedProfile> editor)
        {
            string path = GetPath(name);

            ISharedProfile? profile = FileHelper.ReadJSON<ISharedProfile>(path);
            if (profile == null) return;

            FileHelper.UpdateJSON(path, editor.Invoke(profile));
        }

        public static void Delete(string name)
        {
            FileHelper.Delete(GetPath(name));
        }

        public static bool Exists(string name)
        {
            return Read(name) != null;
        }
    }
}
