using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Host.Provider;
using wey.Interface;

namespace wey.Pages
{
    class ServerCreate : IPageCommand
    {
        public ServerCreate() : base()
        {

        }

        public override string GetName()
        {
            return "create";
        }

        public override string GetDescription()
        {
            return "create the server";
        }

        public override void OnCommand()
        {
            string name = Input.ReadString("server name");

            string providerName = Input.SelectionString(new string[] { "vanilla", "paper", "fabric" }, "provider name");
            switch (providerName)
            {
                case "vanilla":
                    {
                        Vanilla provider = new();

                        string version = Selection<string>
                            .Create(
                                Vanilla.GetVersions().Versions.Select(v => v.ID)
                            )
                            .Render().Value;

                        break;
                    }
                case "paper":
                    {
                        PaperMC provider = new();

                        break;
                    }
                case "fabric":
                    {
                        FabricMC provider = new();

                        break;
                    }
            }
        }
    }
}
