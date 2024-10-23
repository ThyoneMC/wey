using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.API.Mod
{
    internal class CurseForgeHandler : ModHandler
    {
        public CurseForgeHandler(string gameVersion) : base(gameVersion)
        {

        }

        public override ModHandlerFile Get(string id)
        {
            ICurseForge.IData<ICurseForge.IMod>? getMod = CurseForge.GetMod(int.Parse(id));
            if (getMod == null) throw new Exception("rest error - CurseForge.GetMod");

            ICurseForge.IData<ICurseForge.IFile[]>? getFiles = CurseForge.GetModFilesByGameVersion(getMod.Data.ID, this.gameVersion);
            if (getFiles == null) throw new Exception("rest error - CurseForge.GetModFilesByGameVersion");
            if (getFiles.Data.Length == 0) throw new Exception($"mod {getMod.Data.Name} ({getMod.Data.ID}) not found for {this.gameVersion}");

            ICurseForge.IFile file = getFiles.Data.OrderBy(x => (int)x.ReleaseType).ElementAt(0);
            ICurseForge.IFileHash hash = file.Hashes.First(x => x.Algorithm == ICurseForge.IFileHashAlgorithm.SHA1);

            List<ModHandlerFile> dependencies = new();
            List<ModHandlerFile> incompatible = new();

            foreach (ICurseForge.IFileDependency dependency in file.Dependencies)
            {
                if (dependency.RelationType == ICurseForge.IFileRelationType.RequiredDependency)
                {
                    dependencies.Add(Get(dependency.ModID.ToString()));
                    continue;
                }

                if (dependency.RelationType == ICurseForge.IFileRelationType.Incompatible)
                {
                    incompatible.Add(Get(dependency.ModID.ToString()));
                    continue;
                }
            }

            return new()
            {
                Name = getMod.Data.Name,
                Provider = ModHandlerProvider.CurseForge,
                ID = getMod.Data.ID.ToString(),
                FileName = file.FileName,
                FileID = file.ID.ToString(),
                Hash = new ModHandlerFileHash()
                {
                    Algorithm = "sha1",
                    Value = hash.Value,
                },
                URL = file.DownloadURL,
                ClientSide = true,
                ServerSide = true,
                Dependencies = dependencies.ToArray(),
                Incompatibles = incompatible.ToArray()
            };
        }

        public override ModHandlerFile[] Update(ModHandlerFile[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = Get(ids[i].ID);
            }

            return ids;
        }
    }
}
