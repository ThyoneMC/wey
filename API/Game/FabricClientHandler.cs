using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.IO;

namespace wey.API.Game
{
    internal class FabricClientHandler : ClientHandler
    {
        public FabricClientHandler(string gameVersion) : base(gameVersion)
        {

        }

        public override void Download()
        {
            IFabric.ILoader[]? getLoaders = Fabric.GetLoaders(this.gameVersion);
            if (getLoaders == null) throw new Exception("rest error - Fabric.GetLoaders");

            IFabric.ILoader loader = getLoaders.ElementAt(0);

            byte[]? zipData = Fabric.DownloadProfile(this.gameVersion, loader.Loader.Version);
            if (zipData == null) throw new Exception("rest error - Fabric.DownloadProfile");

            string extractDir = Path.Combine(ApplicationDirectoryHelper.Temporary, $"fabric-loader-{loader.Loader.Version}-{this.gameVersion}");
            FileHelper.UnzipBytes(extractDir, zipData);

            string[] dirArr = Directory.GetDirectories(extractDir);
            if (dirArr.Length != 1) throw new Exception("unzip error");

            DirectoryHelper.Clone(dirArr[0], Path.Join(Launcher.GameDirectoryPath, "versions", DirectoryHelper.GetRootDirectoryName(dirArr[0])));
        }

        public override bool ContainsGameVersion()
        {
            IFabric.IVersion[]? getGameVersions = Fabric.GetGames();
            if (getGameVersions == null) throw new Exception("rest error - Fabric.GetGames");

            foreach (IFabric.IVersion version in getGameVersions)
            {
                if (!version.IsStable) continue;

                if (version.Version == this.gameVersion)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
