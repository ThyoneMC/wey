using Quickenshtein;
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

        static ModHandlerFile? Find(ModHandlerFile file)
        {
            float MaxSimilarityScore = 80;

            foreach (ModHandlerFile mod in data)
            {
                if (file.FileName == mod.FileName) return mod;
                if (file.Hash.Algorithm.ToLower() == mod.Hash.Algorithm.ToLower() && file.Hash.Value == mod.Hash.Value) return mod;
                if (file.Provider == mod.Provider && file.ID == mod.ID) return mod;

                // likely to be char difference
                int distance = Levenshtein.GetDistance(file.Name, mod.Name);
                float distanceRatio = distance / Math.Max(file.Name.Length, mod.Name.Length);
                float score = (1 - distanceRatio) * 100;
                if (score >= MaxSimilarityScore) return mod;
            }

            return null;
        }

        // file will save in "ApplicationDirectoryHelper.Temporary"
        public static void Download(ModHandlerFile[] files)
        {
            if (files.Length == 0) return;

            List<ModHandlerFile> incompatibles = new();

            foreach (ModHandlerFile mod in files)
            {
                ModHandlerFile? findMod = Find(mod);
                if (findMod != null)
                {
                    Console.WriteLine($"expect duplicate mod ({findMod.ID})");
                    continue;
                }

                Downloader.Download(mod.URL, mod.FileName);
                data.Add(mod);

                Download(mod.Dependencies);
                incompatibles.AddRange(mod.Incompatibles);
            }

            foreach (ModHandlerFile mod in incompatibles)
            {
                ModHandlerFile? findMod = Find(mod);
                if (findMod != null)
                {
                    // #! Add what mod is incompatible with?
                    Console.WriteLine($"expect incompatible mod ({findMod.ID})");
                }
            }

            FileHelper.UpdateJSON(filePath, data);
        }
    }
}
