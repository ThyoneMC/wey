using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    class ConfigData
    {
        [JsonPropertyName("ngrok")]
        public string Ngrok { get; set; } = string.Empty;

        [JsonPropertyName("playit_gg")]
        public string PlayIt { get; set; } = string.Empty;

        [JsonPropertyName("hamachi")]
        public string Hamachi { get; set; } = string.Empty;
    }

    class Config
    {
        private static readonly FileController File = new("config");

        public static ConfigData Get()
        {
            ConfigData? read = File.ReadFile<ConfigData>();

            if (read == null) return new();

            return read;
        }

        public static void Edit(Func<ConfigData, ConfigData> callback)
        {
            Set(callback(Get()));
        }

        public static void Set(ConfigData data)
        {
            File.EditFile(data);

            Logger.Info("config saved");
        }
    }
}
