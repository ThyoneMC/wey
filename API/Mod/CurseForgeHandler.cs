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

            ICurseForge.IFile file = getFiles.Data.OrderBy(x => (int)x.ReleaseType).ElementAt(0);
            ICurseForge.IFileHash hash = file.Hashes.First(x => x.Algorithm == ICurseForge.IFileHashAlgorithm.SHA1);

            List<string> dependencies = new();
            List<string> incompatible = new();

            foreach (ICurseForge.IFileDependency dependency in file.Dependencies)
            {
                if (dependency.RelationType == ICurseForge.IFileRelationType.RequiredDependency)
                {
                    dependencies.Add(dependency.ModID.ToString());
                    continue;
                }

                if (dependency.RelationType == ICurseForge.IFileRelationType.Incompatible)
                {
                    incompatible.Add(dependency.ModID.ToString());
                    continue;
                }
            }

            return new()
            {
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
                DependencyProjectIDs = dependencies.ToArray(),
                IncompatibleProjectIDs = incompatible.ToArray()
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
