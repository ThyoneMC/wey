using wey.API.Mod;

namespace wey.API
{
    public enum ModHandlerProvider
    {
        Modrinth,
        CurseForge
    }

    public static class ModHandlerFactory
    {
        public static ModHandler Get(ModHandlerProvider? provider, string gameVersion)
        {
            switch (provider)
            {
                case ModHandlerProvider.Modrinth:
                    {
                        return new ModrinthHandler(gameVersion);
                    }
                case ModHandlerProvider.CurseForge:
                    {
                        return new CurseForgeHandler(gameVersion);
                    }
                default:
                    {
                        throw new Exception($"provider {provider} not found");
                    }
            }
        }
    }
}
