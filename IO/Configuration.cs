using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace wey.IO
{
    public class IConfiguration : Dictionary<string, string>
    {
        
    }

    /*
     * Known Config
     * 
     * 1. curseforgeApi
     * 2. minecraftDir
     */

    public static class Configuration
    {
        static string filePath;

        static IConfiguration data;

        static Configuration()
        {
            filePath = Path.Join(ApplicationDirectoryHelper.Appdata, "config.json");

            IConfiguration? config = FileHelper.ReadJSON<IConfiguration>(filePath);
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

        public static string? Read(string key)
        {
            if (!data.TryGetValue(key, out string? value))
            {
                return null;
            }

            return value;
        }

        public static void Update(string key, string value)
        {
            data[key] = value;

            FileHelper.UpdateJSON(filePath, data);
        }

        public static void Delete(string key)
        {
            data.Remove(key);

            FileHelper.UpdateJSON(filePath, data);
        }
    }
}
