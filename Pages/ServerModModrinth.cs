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
using static wey.Host.Provider.Vanilla;

namespace wey.Pages
{
    class ServerModModrinth : IPageCommand
    {
        private readonly HostData HostData;

        public ServerModModrinth(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "modrinth";
        }

        public override string GetDescription()
        {
            return "modrinth";
        }

        public static void ProjectDownload(string path, string id, string gameVersion, string? loader = null)
        {
            ModrinthVersion project = Modrinth.GetProjectVersions(id, gameVersion, loader).First();
            StaticFileController.Build(Path.Join(path, project.Files.First().FileName), Rest.StaticDownload(project.Files.First().URL));

            foreach (ModrinthVersionDependency dependency in project.Dependencies)
            {
                if (dependency.DependencyType != ModrinthVersionDependencyType.Required && dependency.DependencyType != ModrinthVersionDependencyType.Optional) continue;

                if (!string.IsNullOrEmpty(dependency.VersionID))
                {
                    VersionDownload(path, dependency.VersionID, gameVersion, loader);
                }
                else
                {
                    ProjectDownload(path, dependency.ProjectID, gameVersion, loader);
                }
            }
        }

        public static void VersionDownload(string path, string id, string gameVersion, string? loader = null)
        {
            ModrinthVersion version = Modrinth.GetVersion(id);
            StaticFileController.Build(Path.Join(path, version.Files.First().FileName), Rest.StaticDownload(version.Files.First().URL));

            foreach (ModrinthVersionDependency dependency in version.Dependencies)
            {
                if (dependency.DependencyType != ModrinthVersionDependencyType.Required && dependency.DependencyType != ModrinthVersionDependencyType.Optional) continue;

                if (!string.IsNullOrEmpty(dependency.VersionID))
                {
                    VersionDownload(path, dependency.VersionID, gameVersion, loader);
                }
                else
                {
                    ProjectDownload(path, dependency.ProjectID, gameVersion, loader);
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

            HostProperties property = new(HostData);

            if (project.ProjectType == ModrinthProjectType.ResourcePack)
            {
                if (!string.IsNullOrWhiteSpace(property.Get("resource-pack")))
                {
                    if (!Input.ReadBoolean("Do you want to replace resource pack?")) return;
                }

                property.Set("resource-pack", versions[0].Files[0].URL);
                property.Set("resource-pack-sha1", versions[0].Files[0].Hash.SHA1);

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

                        ProjectDownload(Path.Join(HostData.FolderPath, "mods"), project.Slug, provider.Version, HostData.Provider);

                        break;
                    }
                case ModrinthProjectType.Plugin:
                    {
                        if (!provider.HasPlugin)
                        {
                            Logger.Info("plugin are not support in this server");
                            return;
                        }

                        VersionDownload(Path.Join(HostData.FolderPath, "plugins"), versions.First().VersionID, provider.Version);

                        break;
                    }
                case ModrinthProjectType.Datapack:
                    {
                        string? levelName = property.Get("level-name");
                        if (string.IsNullOrWhiteSpace(levelName))
                        {
                            Logger.Info("world not found");
                            return;
                        }

                        VersionDownload(Path.Join(HostData.FolderPath, levelName, "datapack"), versions.First().VersionID, provider.Version);

                        break;
                    }
            }
        }
    }
}
