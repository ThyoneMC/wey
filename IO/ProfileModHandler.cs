using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;

namespace wey.IO
{
    public class IProfileModHandler : List<ModHandlerFile>
    {

    }

    public static class ProfileModHandler
    {
        static readonly string filePath;

        static readonly IProfileModHandler data;

        static ProfileModHandler()
        {
            filePath = Path.Join(ApplicationDirectoryHelper.Appdata, "mods.json");

            IProfileModHandler? config = FileHelper.ReadJSON<IProfileModHandler>(filePath);
            if (config == null)
            {
                data = new();
                FileHelper.UpdateJSON(filePath, data);
            }
            else
            {
                data = config;
            }
        }

        static bool Contains(ModHandlerFile file)
        {
            foreach (ModHandlerFile mod in data)
            {
                if (file.FileName == mod.FileName) return true;
                if (string.Equals(file.Hash.Algorithm, mod.Hash.Algorithm, StringComparison.OrdinalIgnoreCase) && file.Hash.Value == mod.Hash.Value) return true;
            }

            return false;
        }
        
        public static void Download(string dirPath, ModHandlerFile[] files)
        {
            if (files.Length == 0) return;

            foreach (ModHandlerFile mod in files)
            {
                if (Contains(mod)) continue;

                string filePath = Path.Join(dirPath, mod.FileName);

                RestUtils.Download(filePath, mod.URL);
                data.Add(mod);

                Download(dirPath, mod.Dependencies);
            }

            FileHelper.UpdateJSON(filePath, data);
        }
    }
}
