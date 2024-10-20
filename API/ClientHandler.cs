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

        protected ClientHandler(string gameVersion)
        {
            this.gameVersion = gameVersion;
        }

        // return gameVersionID
        public abstract string Download();
    }
}
