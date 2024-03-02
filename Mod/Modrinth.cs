using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using static wey.Host.Provider.PaperMC;

namespace wey.Mod
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModrinthProjectEnvironment
    {
        [EnumMember(Value = "required")]
        Required,

        [EnumMember(Value = "optional")]
        Optional,

        [EnumMember(Value = "unsupported")]
        Unsupported
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModrinthProjectType
    {
        [EnumMember(Value = "mod")]
        Mod,

        //sub-mod
        [EnumMember(Value = "plugin")]
        Plugin,

        //sub-mod
        [EnumMember(Value = "datapack")]
        Datapack,

        //unsupport
        [EnumMember(Value = "shader")]
        Shader,

        [EnumMember(Value = "resourcepack")]
        ResourcePack,

        //unsupport
        [EnumMember(Value = "modpack")]
        ModPack
    }

    public class ModrinthProject
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("client_side")]
        public ModrinthProjectEnvironment ClientSide { get; set; }

        [JsonPropertyName("server_side")]
        public ModrinthProjectEnvironment ServerSide { get; set; }

        [JsonPropertyName("project_type")]
        public ModrinthProjectType ProjectType { get; set; }
    }

    public class ModrinthProjectSearch
    {
        [JsonPropertyName("hits")]
        public ModrinthProject[] Hits { get; set; } = Array.Empty<ModrinthProject>();
    }

    public class ModrinthVersionFileHash
    {
        [JsonPropertyName("sha512")]
        public string SHA512 { get; set; } = string.Empty;

        [JsonPropertyName("sha1")]
        public string SHA1 { get; set; } = string.Empty;
    }

    public class ModrinthVersionFile
    {
        [JsonPropertyName("hashes")]
        public ModrinthVersionFileHash Hash { get; set; } = new();

        [JsonPropertyName("url")]
        public string URL { get; set; } = string.Empty;

        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModrinthVersionDependencyType
    {
        [EnumMember(Value = "required")]
        Required,

        [EnumMember(Value = "optional")]
        Optional,

        [EnumMember(Value = "incompatible")]
        Incompatible,

        [EnumMember(Value = "embedded")]
        Embedded
    }

    public class ModrinthVersionDependency
    {
        [JsonPropertyName("version_id")]
        public string VersionID { get; set; } = string.Empty;

        [JsonPropertyName("project_id")]
        public string ProjectID { get; set; } = string.Empty;

        [JsonPropertyName("dependency_type")]
        public ModrinthVersionDependencyType DependencyType { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModrinthVersionType
    {
        [EnumMember(Value = "release")]
        Release,

        [EnumMember(Value = "beta")]
        Beta,

        [EnumMember(Value = "alpha")]
        Alpha,
    }

    public class ModrinthVersion
    {
        [JsonPropertyName("dependencies")]
        public ModrinthVersionDependency[] Dependencies { get; set; } = Array.Empty<ModrinthVersionDependency>();

        [JsonPropertyName("game_versions")]
        public string[] GameVersions { get; set; } = Array.Empty<string>();

        [JsonPropertyName("version_type")]
        public ModrinthVersionType VersionType { get; set; }

        [JsonPropertyName("loaders")]
        public string[] Loaders { get; set; } = Array.Empty<string>();

        [JsonPropertyName("id")]
        public string VersionID { get; set; } = string.Empty;

        [JsonPropertyName("files")]
        public ModrinthVersionFile[] Files { get; set; } = Array.Empty<ModrinthVersionFile>();

        public ModrinthProjectType GetProjectType()
        {
            if (this.Loaders.Contains("minecraft"))
            {
                return ModrinthProjectType.ResourcePack;
            }

            if (this.Loaders.Contains("datapack"))
            {
                return ModrinthProjectType.Datapack;
            }

            if (this.Loaders.Any(v =>
            {
                return
                v == "fabric" ||
                v == "forge" ||
                v == "quilt";
            }))
            {
                return ModrinthProjectType.Mod;
            }

            return ModrinthProjectType.Plugin;
        }
    }

    public static class Modrinth
    {
        public static readonly FileController File;
        private static readonly Rest Client = new("https://api.modrinth.com/v2/");

        static Modrinth()
        {
            File = new(Path.Join(StaticFolderController.AppdataPath, "mod"), "modrinth");
            if (!File.Exists()) File.Build(string.Empty);
        }

        public static ModrinthProject GetProject(string id)
        {
            return Client.Get<ModrinthProject>($"project/{id}");
        }

        public static ModrinthProject[] GetProject(params string[] id)
        {
            return Client.Get<ModrinthProject[]>($"projects?ids={JsonSerializer.Serialize(id)}");
        }

        public static ModrinthProjectSearch SearchProjects(string query, int limit = 10)
        {
            return Client.Get<ModrinthProjectSearch>($"search?query=\"{query}\"&limit={limit}");
        }

        public static ModrinthVersion[] GetProjectVersions(string id, string gameVersion, string? loader = null)
        {
            string request = $"project/{id}/version?game_versions=[\"{gameVersion}\"]";
            if (!string.IsNullOrWhiteSpace(loader)) request += $"&loaders=[\"{loader}\"]";

            return Client.Get<ModrinthVersion[]>(request);
        }

        public static ModrinthVersion GetVersion(string id)
        {
            return Client.Get<ModrinthVersion>($"version/{id}");
        }

        public static ModrinthVersion[] GetVersion(params string[] id)
        {
            return Client.Get<ModrinthVersion[]>($"versions?ids={JsonSerializer.Serialize(id)}");
        }
    }
}
