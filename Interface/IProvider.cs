using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Global;

namespace wey.Interface
{
    public class VersionNotFoundException : Exception
    {
        public VersionNotFoundException(string? message = null) : base($"version not found: {message}")
        {

        }
    }

    public class IProviderBuild
    {
        public bool HasMod { get; set; } = false;

        public bool HasPlugin { get; set; } = false;

        public string Version { get; set; } = string.Empty;

        public IProviderBuild(string version)
        {
            Version = version;
        }
    }

    public class IProviderDownload
    {
        public string Build { get; set; } = string.Empty;
        public byte[] ServerJar { get; set; } = Array.Empty<byte>();
    }

    public abstract class IProvider<T> where T : IProviderBuild
    {
        public abstract IProviderDownload GetServerJar(T build);

        public abstract IProviderDownload GetServerJar(string gameVersion);
    }

}
