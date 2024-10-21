using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API.Mod
{
    public class IModrinth
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IProjectEnvironment
        {
            [EnumMember(Value = "required")]
            Required,

            [EnumMember(Value = "optional")]
            Optional,

            [EnumMember(Value = "unsupported")]
            Unsupported,

            [EnumMember(Value = "unknown")]
            Unknown
        }

        public static bool IsDownloadableProjectEnvironment(IProjectEnvironment environment)
        {
            return
                environment == IProjectEnvironment.Required ||
                environment == IProjectEnvironment.Optional;
        }

        public class IProject
        {
            [JsonPropertyName("client_side")]
            public IProjectEnvironment ClientSide { get; set; }

            [JsonPropertyName("server_side")]
            public IProjectEnvironment ServerSide { get; set; }

            [JsonPropertyName("id")]
            public string ID { get; set; } = string.Empty;

            [JsonPropertyName("slug")]
            public string Slug { get; set; } = string.Empty;

            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
        }

        public class IVersionFileHash
        {
            [JsonPropertyName("sha1")]
            public string SHA1 { get; set; } = string.Empty;

            [JsonPropertyName("sha512")]
            public string SHA512 { get; set; } = string.Empty;
        }

        public class IVersionFile
        {
            [JsonPropertyName("hashes")]
            public IVersionFileHash Hashes { get; set; } = new();

            [JsonPropertyName("url")]
            public string URL { get; set; } = string.Empty;

            [JsonPropertyName("filename")]
            public string FileName { get; set; } = string.Empty;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IVersionDependencyType
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

        public class IVersionDependency
        {
            [JsonPropertyName("dependency_type")]
            public IVersionDependencyType DependencyType { get; set; }

            [JsonPropertyName("project_id")]
            public string ProjectID { get; set; } = string.Empty;

            [JsonPropertyName("version_id")]
            public string VersionID { get; set; } = string.Empty;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IVersionType
        {
            [EnumMember(Value = "release")]
            Release,

            [EnumMember(Value = "beta")]
            Beta,

            [EnumMember(Value = "alpha")]
            Alpha
        }

        public class IVersion
        {
            [JsonPropertyName("id")]
            public string ID { get; set; } = string.Empty;

            [JsonPropertyName("version_type")]
            public IVersionType VersionType { get; set; }

            [JsonPropertyName("files")]
            public IVersionFile[] Files { get; set; } = Array.Empty<IVersionFile>();

            [JsonPropertyName("dependencies")]
            public IVersionDependency[] Dependencies { get; set; } = Array.Empty<IVersionDependency>();
        }
    }

    internal static class Modrinth
    {
        static readonly RestClient rest;

        static Modrinth()
        {
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                UserAgent = "thyonemc/wey (https://github.com/thyonemc/wey)",
                BaseUrl = new Uri("https://api.modrinth.com/v2/"),
            };

            rest = new(options);
        }

        public static IModrinth.IProject? GetProject(string id)
        {
            RestRequest request = new($"project/{id}");
            return rest.Get<IModrinth.IProject>(request);
        }

        public static IModrinth.IProject[]? GetProjects(string[] id)
        {
            RestRequest request = new($"projects");

            request.AddQueryParameter("ids", JsonSerializer.Serialize(id));

            return rest.Get<IModrinth.IProject[]>(request);
        }

        public static IModrinth.IVersion[]? GetVersionsByProject(string projectId, string gameVersion)
        {
            RestRequest request = new($"project/{projectId}/version");

            string[] loaders = { "fabric" };
            request.AddQueryParameter("loaders", JsonSerializer.Serialize(loaders));

            string[] gameVersions = { gameVersion };
            request.AddQueryParameter("game_versions", JsonSerializer.Serialize(gameVersions));

            return rest.Get<IModrinth.IVersion[]>(request);
        }

        public static IModrinth.IVersion[]? GetVersions(string[] id)
        {
            RestRequest request = new($"versions");

            request.AddQueryParameter("ids", JsonSerializer.Serialize(id));

            return rest.Get<IModrinth.IVersion[]>(request);
        }

        public static Dictionary<string, IModrinth.IVersion>? GetLatestVersionsByHashes(string[] hash, string gameVersion, string algorithm = "sha512")
        {
            RestRequest request = new($"version_files/update");

            var body = new
            {
                hashes = hash,
                algorithm = algorithm,
                loaders = new string[] { "fabric" },
                game_versions = new string[] { gameVersion }
            };

            request.AddJsonBody(body);

            return rest.Post<Dictionary<string, IModrinth.IVersion>>(request);
        }
    }
}
