using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.Console;
using wey.Provider;

namespace wey.Model
{
    public class ProviderBaseDownload
    {
        public string[] BuildInfo { get; set; } = Array.Empty<string>();
        public byte[] ServerJar { get; set; } = Array.Empty<byte>();
    }

    public abstract class ProviderBase
    {
        public abstract bool GetIsMod();

        public abstract ProviderBaseDownload GetServerJar(string[] buildInfo);

        public abstract ProviderBaseDownload GetServerJar(string gameVersion);
    }
}
