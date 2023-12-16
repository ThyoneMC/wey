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
        public VersionNotFoundException(string? message = null) : base(message)
        {

        }
    }

    public class IProviderBuild
    {
        public string Version { get; set; }

        public IProviderBuild(string version)
        {
            Version = version;
        }
    }

    public class IProviderDownload<T> where T : IProviderBuild
    {
        public T Build { get; set; } = FileController<T>.CreateTypeInstance();
        public byte[] ServerJar { get; set; } = Array.Empty<byte>();
    }

    public abstract class IProvider<T> where T : IProviderBuild
    {
        public abstract bool IsMod();

        public abstract IProviderDownload<T> GetServerJar(T build);

        public abstract IProviderDownload<T> GetServerJar(string gameVersion);
    }

}
