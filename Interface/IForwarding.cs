using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using wey.Console;
using wey.Global;

namespace wey.Interface
{
    
    abstract class IForwarding
    {
        private static readonly JsonFileController<Dictionary<string, string>> File = new("forwarding.json");

        static IForwarding()
        {
            File.Build();
        }

        private string Name = string.Empty;
        protected string FilePath = string.Empty;

        

        public IForwarding(string name)
        {
            Name = name;

            FilePath = Get();
        }

        public string Get()
        {
            return File.ReadRequired().GetValueOrDefault(Name, string.Empty);
        }

        public bool IsRegistered()
        {
            return IForwarding.File.ReadRequired().ContainsKey(Name);
        }

        public void Register(string path)
        {
            Logger.Log($"Register executable path of {Name} as {path}");

            FilePath = path;

            IForwarding.File.Edit(data =>
            {
                data[Name] = FilePath;

                return data;
            });
        }

        public void Unregister()
        {
            Logger.Log($"Unregister executable path of {Name}");

            FilePath = string.Empty;

            IForwarding.File.Edit(data =>
            {
                data.Remove(Name);

                return data;
            });
        }
    }
}
