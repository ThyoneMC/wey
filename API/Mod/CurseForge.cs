using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.IO;

namespace wey.API.Mod
{
    public class ICurseForge
    {
        public class IMod
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("slug")]
            public string Slug { get; set; } = string.Empty;
        }

        public enum IFileReleaseType
        {
            Release = 1,
            Beta = 2,
            Alpha = 3
        }

        public enum IFileHashAlgorithm
        {
            SHA1 = 1,
            MD5 = 2
        }

        public class IFileHash
        {
            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("algo")]
            public IFileHashAlgorithm Algorithm { get; set; }
        }

        public enum IFileRelationType
        {
            EmbeddedLibrary = 1,
            OptionalDependency = 2,
            RequiredDependency = 3,
            Tool = 4,
            Incompatible = 5,
            Include = 6
        }

        public class IFileDependency
        {
            [JsonPropertyName("modId")]
            public int ModID { get; set; }

            [JsonPropertyName("relationType")]
            public IFileRelationType RelationType { get; set; }
        }

        public class IFile
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("fileName")]
            public string FileName { get; set; } = string.Empty;

            [JsonPropertyName("releaseType")]
            public IFileReleaseType ReleaseType { get; set; }

            [JsonPropertyName("downloadUrl")]
            public string DownloadURL { get; set; } = string.Empty;

            [JsonPropertyName("hashes")]
            public IFileHash[] Hashes { get; set; } = Array.Empty<IFileHash>();

            [JsonPropertyName("dependencies")]
            public IFileDependency[] Dependencies { get; set; } = Array.Empty<IFileDependency>();
        }

        public class IData<T>
        {
            [JsonPropertyName("data")]
            public required T Data { get; set; }
        }
    }

    internal static class CurseForge
    {
        static readonly RestClient rest;

        static CurseForge()
        {
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                UserAgent = "thyonemc/wey (https://github.com/thyonemc/wey)",
                BaseUrl = new Uri("https://api.curseforge.com/v1/"),
            };

            rest = new(options);

            string API_KEY = Configuration.Read("curseforgeApi") ?? string.Empty;
            rest.AddDefaultHeader("x-api-key", API_KEY);
        }

        public static ICurseForge.IData<ICurseForge.IMod>? GetMod(int id)
        {
            RestRequest request = new($"mods/{id}");
            return rest.Get<ICurseForge.IData<ICurseForge.IMod>>(request);
        }

        public static ICurseForge.IData<ICurseForge.IMod[]>? GetMods(int[] id)
        {
            RestRequest request = new($"mods");

            request.AddBody("modIds", JsonSerializer.Serialize(id));

            return rest.Post<ICurseForge.IData<ICurseForge.IMod[]>>(request);
        }

        public static ICurseForge.IData<ICurseForge.IFile>? GetModFile(int id, int fileId)
        {
            RestRequest request = new($"mods/{id}/files/{fileId}");
            return rest.Get<ICurseForge.IData<ICurseForge.IFile>>(request);
        }

        public static ICurseForge.IData<ICurseForge.IFile[]>? GetModFilesByGameVersion(int id, string gameVersion)
        {
            RestRequest request = new($"mods/{id}/files");

            request.AddQueryParameter("gameVersion", gameVersion);

            int Fabric = 4;
            request.AddQueryParameter("modLoaderType", Fabric);

            request.AddQueryParameter("pageSize", 20);

            return rest.Get<ICurseForge.IData<ICurseForge.IFile[]>>(request);
        }
    }
}
