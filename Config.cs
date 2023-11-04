using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;
using wey.Tool;

namespace wey
{
    class ConfigTunnelData
    {
        [JsonPropertyName("ngrok")]
        public string Ngrok { get; set; } = string.Empty;

        [JsonPropertyName("playit_gg")]
        public string PlayIt { get; set; } = string.Empty;

        [JsonPropertyName("hamachi")]
        public string Hamachi { get; set; } = string.Empty;
    }
    class ConfigData
    {
        [JsonPropertyName("tunnel")]
        public ConfigTunnelData Tunnel { get; set; } = new();

        [JsonPropertyName("auto_restart_delay")]
        public int AutoRestartDelay { get; set; } = 30; // every min
    }

    class Config
    {
        private static readonly FileController<ConfigData> File = new("config");

        public static ConfigData Get()
        {
            ConfigData? read = File.Read();

            if (read == null) return new();

            return read;
        }

        public static void Edit(Func<ConfigData, ConfigData> callback)
        {
            Set(callback(Get()));
        }

        public static void Set(ConfigData data)
        {
            File.Edit(data);

            Logger.Info("config saved");
        }
    }
}
