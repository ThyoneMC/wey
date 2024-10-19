using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.API.Mod
{
    public class IModrinthHelper
    {
        public class IDependencies
        {
            public readonly List<string> dependencies = new();
            public readonly List<string> incompatible = new();
        }
    }

    internal class ModrinthHelper : ModHelper
    {
        public ModrinthHelper(string gameVersion) : base(gameVersion)
        {

        }

        public static IModrinthHelper.IDependencies GetDependencies(IModrinth.IVersionDependency[] dependencies)
        {
            IModrinthHelper.IDependencies data = new();

            foreach (IModrinth.IVersionDependency dependency in dependencies)
            {
                switch (dependency.DependencyType)
                {
                    case IModrinth.IVersionDependencyType.Required:
                        {
                            data.dependencies.Add(dependency.ProjectID);
                            break;
                        }
                    case IModrinth.IVersionDependencyType.Incompatible:
                        {
                            data.incompatible.Add(dependency.ProjectID);
                            break;
                        }
                }
            }

            return data;
        }

        public override ModHelperFile Get(string id)
        {
            IModrinth.IProject? getProject = Modrinth.GetProject(id);
            if (getProject == null) throw new Exception("rest error - Modrinth.GetProject");

            IModrinth.IVersion[]? getVersions = Modrinth.GetVersionsByProject(getProject.ID, this.gameVersion);
            if (getVersions == null) throw new Exception("rest error - Modrinth.GetVersionsByProject");

            IModrinth.IVersion version = getVersions.First(x => x.VersionType == IModrinth.IVersionType.Release);
            if (version == null) version = getVersions.ElementAt(0);

            IModrinth.IVersionFile file = version.Files.ElementAt(0);

            IModrinthHelper.IDependencies dependency = ModrinthHelper.GetDependencies(version.Dependencies);

            return new()
            {
                Provider = ModHelperProvider.Modrinth,
                ID = getProject.ID,
                FileName = file.FileName,
                FileID = version.ID,
                Hash = new ModHelperFileHash()
                {
                    Algorithm = "sha1",
                    Value = file.Hashes.SHA1,
                },
                URL = file.URL,
                ClientSide = IModrinth.IsDownloadableProjectEnvironment(getProject.ClientSide),
                ServerSide = IModrinth.IsDownloadableProjectEnvironment(getProject.ServerSide),
                DependencyProjectIDs = dependency.dependencies.ToArray(),
                IncompatibleProjectIDs = dependency.incompatible.ToArray()
            };
        }

        public override ModHelperFile[] Update(ModHelperFile[] ids)
        {
            if (ids.Length == 0) return ids;

            IModrinth.IVersion[]? getVersions = Modrinth.GetLatestVersionsByHashes(ids.Select(x => x.Hash.Value).ToArray(), this.gameVersion, "sha1");
            if (getVersions == null) throw new Exception("rest error - Modrinth.GetLatestVersionsByHashes");

            for (int i = 0; i < ids.Length; i++)
            {
                IModrinth.IVersionFile file = getVersions[i].Files.ElementAt(0);
                IModrinthHelper.IDependencies dependency = ModrinthHelper.GetDependencies(getVersions[i].Dependencies);

                ids[i].FileName = file.FileName;
                ids[i].FileID = getVersions[i].ID;
                ids[i].Hash.Value = file.Hashes.SHA1;
                ids[i].URL = file.URL;
                ids[i].DependencyProjectIDs = dependency.dependencies.ToArray();
                ids[i].IncompatibleProjectIDs = dependency.incompatible.ToArray();
            }

            return ids;
        }
    }
}
