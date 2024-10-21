using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.API.Mod
{
    public class IModrinthHandler
    {
        public class IDependencies
        {
            public readonly List<string> dependencies = new();
            public readonly List<string> incompatible = new();
        }
    }

    internal class ModrinthHandler : ModHandler
    {
        public ModrinthHandler(string gameVersion) : base(gameVersion)
        {

        }

        public static IModrinthHandler.IDependencies GetDependencies(IModrinth.IVersionDependency[] dependencies)
        {
            IModrinthHandler.IDependencies data = new();

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

        public override ModHandlerFile Get(string id)
        {
            IModrinth.IProject? getProject = Modrinth.GetProject(id);
            if (getProject == null) throw new Exception("rest error - Modrinth.GetProject");

            IModrinth.IVersion[]? getVersions = Modrinth.GetVersionsByProject(getProject.ID, this.gameVersion);
            if (getVersions == null) throw new Exception("rest error - Modrinth.GetVersionsByProject");

            IModrinth.IVersion version = getVersions.First(x => x.VersionType == IModrinth.IVersionType.Release);
            if (version == null) version = getVersions.ElementAt(0);

            IModrinth.IVersionFile file = version.Files.ElementAt(0);

            IModrinthHandler.IDependencies dependency = GetDependencies(version.Dependencies);

            return new()
            {
                Name = getProject.Title,
                Provider = ModHandlerProvider.Modrinth,
                ID = getProject.ID,
                FileName = file.FileName,
                FileID = version.ID,
                Hash = new ModHandlerFileHash()
                {
                    Algorithm = "sha1",
                    Value = file.Hashes.SHA1,
                },
                URL = file.URL,
                ClientSide = IModrinth.IsDownloadableProjectEnvironment(getProject.ClientSide),
                ServerSide = IModrinth.IsDownloadableProjectEnvironment(getProject.ServerSide),
                Dependencies = dependency.dependencies.Select(Get).ToArray(),
                Incompatibles = dependency.incompatible.Select(Get).ToArray()
            };
        }

        public override ModHandlerFile[] Update(ModHandlerFile[] ids)
        {
            if (ids.Length == 0) return ids;

            Dictionary<string, IModrinth.IVersion>? getVersionsDictionary
                = Modrinth.GetLatestVersionsByHashes(ids.Select(x => x.Hash.Value).ToArray(), this.gameVersion, "sha1");
            if (getVersionsDictionary == null) throw new Exception("rest error - Modrinth.GetLatestVersionsByHashes");

            IModrinth.IVersion[] getVersions = getVersionsDictionary.Values.ToArray();

            for (int i = 0; i < ids.Length; i++)
            {
                IModrinth.IVersionFile file = getVersions[i].Files.ElementAt(0);
                IModrinthHandler.IDependencies dependency = ModrinthHandler.GetDependencies(getVersions[i].Dependencies);

                ids[i].FileName = file.FileName;
                ids[i].FileID = getVersions[i].ID;
                ids[i].Hash.Value = file.Hashes.SHA1;
                ids[i].URL = file.URL;
                ids[i].Dependencies = dependency.dependencies.Select(Get).ToArray();
                ids[i].Incompatibles = dependency.incompatible.Select(Get).ToArray();
            }

            return ids;
        }
    }
}
