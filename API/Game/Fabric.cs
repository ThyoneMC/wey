using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API.Game
{
    public class IFabric
    {
        public class IVersion
        {
            [JsonPropertyName("version")]
            public string Version { get; set; } = string.Empty;

            [JsonPropertyName("stable")]
            public bool IsStable { get; set; }
        }

        public class ILoader
        {
            [JsonPropertyName("loader")]
            public IVersion Loader { get; set; } = new();
        }
    }

    internal static class Fabric
    {
        static readonly RestClient rest;

        static Fabric()
        {
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                UserAgent = "thyonemc/wey (https://github.com/thyonemc/wey)",
                BaseUrl = new Uri("https://meta.fabricmc.net/v2/versions/"),
            };

            rest = new(options);
        }

        public static IFabric.IVersion[]? GetGames()
        {
            RestRequest request = new($"game");
            return rest.Get<IFabric.IVersion[]>(request);
        }

        public static IFabric.ILoader[]? GetLoaders(string gameVersion)
        {
            RestRequest request = new($"loader/{gameVersion}");
            return rest.Get<IFabric.ILoader[]>(request);
        }

        public static IFabric.IVersion[]? GetInstallers()
        {
            RestRequest request = new($"installer");
            return rest.Get<IFabric.IVersion[]>(request);
        }

        public static byte[]? DownloadProfile(string gameVersion, string loaderVersion)
        {
            RestRequest request = new($"loader/{gameVersion}/{loaderVersion}/profile/zip");
            return rest.DownloadData(request);
        }

        public static byte[]? DownloadServer(string gameVersion, string loaderVersion, string installerVersion)
        {
            RestRequest request = new($"loader/{gameVersion}/{loaderVersion}/{installerVersion}/server/jar");
            return rest.DownloadData(request);
        }
    }
}
