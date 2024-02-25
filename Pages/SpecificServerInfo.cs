using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Global;
using wey.Host;
using wey.Host.Provider;
using wey.Interface;
using static System.Net.Mime.MediaTypeNames;

namespace wey.Pages
{
    class SpecificServerInfo : IPageView
    {
        private readonly HostData HostData;

        public SpecificServerInfo(HostData host)
        {
            HostData = host;
        }

        public override string GetName()
        {
            return "info";
        }

        public override string GetDescription()
        {
            return "server info";
        }

        public override void OnLoad()
        {
            StringBuilder ProviderInfo = new();

            switch (HostData.Provider)
            {
                case "vanilla":
                    {
                        VanillaBuild ProviderBuildInfo = JsonEncryption.Decrypt<VanillaBuild>(HostData.Build); ;

                        ProviderInfo.AppendLine($"\t- Version: {ProviderBuildInfo.Version}");
                        ProviderInfo.AppendLine($"\t- Mod: {ProviderBuildInfo.HasMod}");
                        ProviderInfo.Append($"\t- Plugin: {ProviderBuildInfo.HasPlugin}");

                        break;
                    }
                case "paper":
                    {
                        PaperMCBuild ProviderBuildInfo = JsonEncryption.Decrypt<PaperMCBuild>(HostData.Build); ;

                        ProviderInfo.AppendLine($"\t- Version: {ProviderBuildInfo.Version}");
                        ProviderInfo.AppendLine($"\t- Mod: {ProviderBuildInfo.HasMod}");
                        ProviderInfo.AppendLine($"\t- Plugin: {ProviderBuildInfo.HasPlugin}");
                        ProviderInfo.Append($"\t- Build: {ProviderBuildInfo.Build}");

                        break;
                    }
                case "fabric":
                    {
                        FabricMCBuild ProviderBuildInfo = JsonEncryption.Decrypt<FabricMCBuild>(HostData.Build); ;

                        ProviderInfo.AppendLine($"\t- Version: {ProviderBuildInfo.Version}");
                        ProviderInfo.AppendLine($"\t- Mod: {ProviderBuildInfo.HasMod}");
                        ProviderInfo.AppendLine($"\t- Plugin: {ProviderBuildInfo.HasPlugin}");
                        ProviderInfo.AppendLine($"\t- Loader: {ProviderBuildInfo.Loader}");
                        ProviderInfo.Append($"\t- Installer: {ProviderBuildInfo.Installer}");

                        break;
                    }
            }

            Logger.WriteMultiple(
                    $"Name: {HostData.Name}",
                    $"Provider: {HostData.Provider}",
                    ProviderInfo.ToString(),
                    $"Create At: {HostData.CreateAt}",
                    $"Path: {HostData.FolderPath}"
                );
        }

        public override void OnViewing()
        {

        }

        public override void OnExit()
        {

        }
    }
}
