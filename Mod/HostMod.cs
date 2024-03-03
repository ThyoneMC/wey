using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;

namespace wey.Mod
{
    public enum HostModType
    {
        Mod,
        Plugin,
        Datapack,
        ResourcePack
    }

    public class HostModData
    {
        [JsonPropertyName("type")]
        public HostModType Type { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("sha1")]
        public string HashSHA1 { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string URL { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class HostMod
    {
        public readonly JsonFileController<List<HostModData>> File;

        private readonly HostData HostData;

        public HostMod(HostData host)
        {
            HostData = host;

            File = new(Path.Join(host.FolderPath, ".wey", "mod.json"));
            if (!File.Exists()) File.Build();
        }

        public void Add(HostModData mod)
        {
            List<HostModData> read = File.ReadRequired();
            if (read.Any(v => v.HashSHA1 == mod.HashSHA1)) return;

            HostProperties property = new(HostData);

            if (mod.Type == HostModType.ResourcePack)
            {
                read = read.Where(a => a.Type != HostModType.ResourcePack).ToList();

                property.Set("resource-pack", mod.URL);
                property.Set("resource-pack-sha1", mod.HashSHA1);
            }
            else
            {
                string path = mod.Type switch
                {
                    HostModType.Mod => "mods",
                    HostModType.Plugin => "plugins",
                    HostModType.Datapack => Path.Join(property.Get("level-name") ?? "world", "datapack"),
                    _ => throw new NotImplementedException(),
                };

                StaticFileController.Build(Path.Join(HostData.FolderPath, path, mod.Name), Rest.StaticDownload(mod.URL));
            }

            read.Add(mod);
            File.Edit(read);
        }

        public void Remove(HostModData mod)
        {
            HostProperties property = new(HostData);

            if (mod.Type == HostModType.ResourcePack)
            {
                property.Set("resource-pack", string.Empty);
                property.Set("resource-pack-sha1", string.Empty);
            }
            else
            {
                string path = mod.Type switch
                {
                    HostModType.Mod => "mods",
                    HostModType.Plugin => "plugins",
                    HostModType.Datapack => Path.Join(property.Get("level-name") ?? "world", "datapacks"),
                    _ => throw new NotImplementedException(),
                };

                StaticFileController.Delete(Path.Join(HostData.FolderPath, path, mod.Name));
            }

            File.Edit(data => data.Where(v => v.HashSHA1 != mod.HashSHA1).ToList());
        }
    }
}
