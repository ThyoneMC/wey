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
            if (getVersions.Length == 0) throw new Exception($"mod {getProject.Title} ({getProject.ID}) not found for {this.gameVersion}");

            IModrinth.IVersion version = getVersions[0];

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

        public override ModHandlerFile[] Update(ModHandlerFile[] files)
        {
            if (files.Length == 0) return files;

            Dictionary<string, IModrinth.IVersion>? getVersions
                = Modrinth.GetLatestVersionsByHashes(files.Select(x => x.Hash.Value).ToArray(), this.gameVersion, "sha1");
            if (getVersions == null) throw new Exception("rest error - Modrinth.GetLatestVersionsByHashes");

            List<ModHandlerFile> fileList = new();

            foreach (ModHandlerFile mod in files)
            {
                if (!getVersions.TryGetValue(mod.Hash.Value, out IModrinth.IVersion? version) || version == null)
                {
                    Console.WriteLine($"mod {mod.Name} ({mod.ID}) not found for {this.gameVersion}");
                    continue;
                }

                IModrinth.IVersionFile file = version.Files.ElementAt(0);
                IModrinthHandler.IDependencies dependency = GetDependencies(version.Dependencies);

                mod.FileName = file.FileName;
                mod.FileID = version.ID;
                mod.Hash.Value = file.Hashes.SHA1;
                mod.URL = file.URL;
                mod.Dependencies = dependency.dependencies.Select(Get).ToArray();
                mod.Incompatibles = dependency.incompatible.Select(Get).ToArray();

                fileList.Add(mod);
            }

            return fileList.ToArray();
        }
    }
}
