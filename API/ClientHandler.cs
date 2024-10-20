using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wey.API
{
    public abstract class ClientHandler
    {
        protected string gameVersion;
        protected string minecraftPath;

        protected ClientHandler(string gameVersion, string? minecraftPath = null)
        {
            this.gameVersion = gameVersion;
            this.minecraftPath = minecraftPath ?? Launcher.GameDirectoryPath;
        }

        // return gameVersionID
        public abstract string Download();
    }
}
