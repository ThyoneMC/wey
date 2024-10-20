using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.IO;

namespace wey.API.Game
{
    internal class FabricServerHandler : ServerHandler
    {
        public FabricServerHandler(string gameVersion) : base(gameVersion)
        {

        }

        public override ServerHandlerFile Download(string dirPath)
        {
            IFabric.ILoader[]? getLoaders = Fabric.GetLoaders(this.gameVersion);
            if (getLoaders == null) throw new Exception("rest error - Fabric.GetLoaders");

            IFabric.IVersion[]? getInstallers = Fabric.GetInstallers();
            if (getInstallers == null) throw new Exception("rest error - Fabric.GetInstallers");

            string loader = getLoaders.ElementAt(0).Loader.Version;
            string installer = getInstallers.ElementAt(0).Version;

            byte[]? data = Fabric.DownloadServer(this.gameVersion, loader, installer);
            if (data == null) throw new Exception("rest error - Fabric.DownloadServer");

            string fileName = "server.jar";
            FileHelper.UpdateBytes(Path.Join(dirPath, fileName), data);

            return new()
            {
                GameVersion = this.gameVersion,
                FileName = fileName,
            };
        }
    }
}
