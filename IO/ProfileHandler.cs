using Quickenshtein;
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
    public class ModHandlerFileList : List<ModHandlerFile>
    {
        // return -1 when not found
        public new int IndexOf(ModHandlerFile file)
        {
            float MaxSimilarityScore = 80;

            for (int i = 0; i < Count; i++)
            {
                ModHandlerFile mod = this[i];

                if (file.FileName == mod.FileName) return i;
                if (string.Equals(file.Hash.Algorithm, mod.Hash.Algorithm, StringComparison.OrdinalIgnoreCase) && file.Hash.Value == mod.Hash.Value) return i;
                if (file.Provider == mod.Provider && file.ID == mod.ID) return i;

                // likely to be char difference
                int distance = Levenshtein.GetDistance(file.Name, mod.Name);
                float distanceRatio = (float)distance / Math.Max(file.Name.Length, mod.Name.Length);
                float score = (1 - distanceRatio) * 100;
                if (score >= MaxSimilarityScore) return i;
            }

            return -1;
        }

        public new void Add(ModHandlerFile file)
        {
            int notFound = -1;

            foreach (ModHandlerFile mod in file.Incompatibles)
            {
                int indxErr = IndexOf(mod);
                if (indxErr != notFound)
                {
                    Console.WriteLine($"mod \"{this[indxErr].Name}\" expect incompatible by \"{file.Name}\"");
                }
            }

            int indx = IndexOf(file);
            if (indx == notFound)
            {
                base.Add(file);
            }
            else
            {
                this[indx] = file;

                Console.WriteLine($"mod \"{file.Name}\" has been replaced by \"{this[indx].Name}\"");
            }
        }

        public new void AddRange(IEnumerable<ModHandlerFile> files)
        {
            foreach (ModHandlerFile mod in files)
            {
                Add(mod);
            }
        }
    }

    public class ModHandlerFileExternal
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = $"mod-external-{DateTime.Now.ToFileTimeUtc()}.jar";

        [JsonPropertyName("url")]
        public required string URL { get; set; }

        [JsonPropertyName("replaceId")]
        public string? ReplacementID { get; set; } = null;

        [JsonPropertyName("clientSide")]
        public bool ClientSide { get; set; } = true;

        [JsonPropertyName("serverSide")]
        public bool ServerSide { get; set; } = true;
    }

    public class ISharedProfile
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }

        [JsonPropertyName("launcherIcon")]
        public string? LauncherIconString { get; set; } = null;

        [JsonPropertyName("mods")]
        public ModHandlerFileList Mods { get; set; } = new();

        [JsonPropertyName("externalMods")]
        public List<ModHandlerFileExternal> ExternalMods { get; set; } = new();
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

        public static ISharedProfile[] ReadAll()
        {
            List<ISharedProfile> profiles = new();

            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                try
                {
                    ISharedProfile? pf = Read(Path.GetFileName(filePath));
                    if (pf == null) throw new Exception();

                    profiles.Add(pf);
                }
                catch { }
            }

            return profiles.ToArray();
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
