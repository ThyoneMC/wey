using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Model
{
    public abstract class TunnelBase
    {
        public string Path = string.Empty;

        public TunnelBase(string path)
        {
            this.Path = path;
        }

        public abstract string GetName();

        public abstract int Start();

        public abstract void Stop();
    }
}
