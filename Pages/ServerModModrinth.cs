using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;
using wey.Mod;
using static wey.Host.Provider.PaperMC;
using static wey.Host.Provider.Vanilla;

namespace wey.Pages
{
    class ServerModModrinth : IPageCommand
    {
        private readonly HostData HostData;
        private readonly HostMod HostMod;

        public ServerModModrinth(HostData host)
        {
            HostData = host;
            HostMod = new HostMod(host);
        }

        public override string GetName()
        {
            return "modrinth";
        }

        public override string GetDescription()
        {
            return "modrinth";
        }

        private void ProjectDownload(HostModType type, string id, string gameVersion, string? loader = null)
        {
            ModrinthVersion project = Modrinth.GetProjectVersions(id, gameVersion, loader).First();
            HostMod.Add(new HostModData
            {
                Type = type,
                Provider = "modrinth",
                ID = project.VersionID,
                HashSHA1 = project.Files.First().Hash.SHA1,
                URL = project.Files.First().URL,
                Name = project.Files.First().FileName
            });

            foreach (ModrinthVersionDependency dependency in project.Dependencies)
            {
                if (dependency.DependencyType != ModrinthVersionDependencyType.Required && dependency.DependencyType != ModrinthVersionDependencyType.Optional) continue;

                if (!string.IsNullOrEmpty(dependency.VersionID))
                {
                    VersionDownload(type, dependency.VersionID, gameVersion, loader);
                }
                else
                {
                    ProjectDownload(type, dependency.ProjectID, gameVersion, loader);
                }
            }
        }

        private void VersionDownload(HostModType type, string id, string gameVersion, string? loader = null)
        {
            ModrinthVersion version = Modrinth.GetVersion(id);
            HostMod.Add(new HostModData
            {
                Type = type,
                Provider = "modrinth",
                ID = version.VersionID,
                HashSHA1 = version.Files.First().Hash.SHA1,
                URL = version.Files.First().URL,
                Name = version.Files.First().FileName
            });

            foreach (ModrinthVersionDependency dependency in version.Dependencies)
            {
                if (dependency.DependencyType != ModrinthVersionDependencyType.Required && dependency.DependencyType != ModrinthVersionDependencyType.Optional) continue;

                if (!string.IsNullOrEmpty(dependency.VersionID))
                {
                    VersionDownload(type, dependency.VersionID, gameVersion, loader);
                }
                else
                {
                    ProjectDownload(type, dependency.ProjectID, gameVersion, loader);
                }
            }
        }

        public override void OnCommand()
        {
            string query = Input.ReadString("Search project", required: true, clear: true);
            ModrinthProject[] search = Modrinth.SearchProjects(query, limit: 25).Hits;
            ModrinthProject project = search[Input.SelectionString(search.Select(v => v.Title)).Index];

            if (project.ProjectType == ModrinthProjectType.ModPack || project.ProjectType == ModrinthProjectType.Shader)
            {
                Logger.Info("Modpack & Shader are not support");
                return;
            }

            IProviderBuild provider = JsonEncryption.Decrypt<IProviderBuild>(HostData.Build);

            ModrinthVersion[] versions = Modrinth.GetProjectVersions(project.Slug, provider.Version);
            if (!versions.Any())
            {
                Logger.Info("game version not found");
                return;
            }

            if (project.ProjectType == ModrinthProjectType.ResourcePack)
            {
                if (!string.IsNullOrWhiteSpace(new HostProperties(HostData).Get("resource-pack")))
                {
                    if (!Input.ReadBoolean("Do you want to replace resource pack?")) return;
                }

                HostMod.Add(new HostModData
                {
                    Type = HostModType.ResourcePack,
                    Provider = "modrinth",
                    ID = versions.First().VersionID,
                    HashSHA1 = versions.First().Files.First().Hash.SHA1,
                    URL = versions.First().Files.First().URL,
                    Name = versions.First().Files.First().FileName
                });

                return;
            }

            ModrinthProjectType[] types = versions.Select(v => v.GetProjectType()).Distinct().ToArray();
            switch (types.Length == 1 ? types[0] : types[Input.SelectionString(types.Select(v => v.ToString()), "What do you want to install?").Index])
            {
                case ModrinthProjectType.Mod:
                    {
                        if (!provider.HasMod)
                        {
                            Logger.Info("mod are not support in this server");
                            return;
                        }

                        ProjectDownload(HostModType.Mod, project.Slug, provider.Version, HostData.Provider);

                        break;
                    }
                case ModrinthProjectType.Plugin:
                    {
                        if (!provider.HasPlugin)
                        {
                            Logger.Info("plugin are not support in this server");
                            return;
                        }

                        VersionDownload(HostModType.Plugin, versions.First(v => v.GetProjectType() == ModrinthProjectType.Plugin).VersionID, provider.Version);

                        break;
                    }
                case ModrinthProjectType.Datapack:
                    {
                        VersionDownload(HostModType.Datapack, versions.First(v => v.GetProjectType() == ModrinthProjectType.Datapack).VersionID, provider.Version);

                        break;
                    }
            }
        }
    }
}
