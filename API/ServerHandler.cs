﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API
{
    public class ServerHandlerFile
    {
        [JsonPropertyName("gameVersion")]
        public string GameVersion { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
    }

    public abstract class ServerHandler
    {
        protected string gameVersion;

        protected ServerHandler(string gameVersion)
        {
            this.gameVersion = gameVersion;
        }

        public abstract ServerHandlerFile Download(string jarPath);
    }
}
