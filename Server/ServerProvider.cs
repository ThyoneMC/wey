using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Model;
using wey.Provider;

namespace wey.Server
{
    public static class ServerProvider
    {
        public const string Vanilla = "vanilla";
        public const string PaperMC = "paper";
        public const string FabricMC = "fabric";

        public static ProviderBase GetProvider(string provider)
        {
            switch (provider)
            {
                case ServerProvider.Vanilla:
                    {
                        return new Vanilla();
                    }
                case ServerProvider.PaperMC:
                    {
                        return new PaperMC();
                    }
                case ServerProvider.FabricMC:
                    {
                        return new FabricMC();
                    }
                default:
                    {
                        throw new ArgumentException("Provider Not Found");
                    }
            }
        }
    }
}
