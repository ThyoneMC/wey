using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.API;

namespace wey.IO
{
    public static class ProfileModHandler
    {
        static string dirPath = Path.Join(ApplicationDirectoryHelper.Appdata, "mods");

        public static void Download(ModHandlerFile[] files, ModHandlerFileExternal[] externals)
        {
            if (files.Length == 0) return;

            ModHandlerFileExternal[] replacementMods = externals.Where(x => !string.IsNullOrWhiteSpace(x.ReplacementID)).ToArray();
            ModHandlerFileExternal[] externalMods = externals.Where(x => !replacementMods.Contains(x)).ToArray();

            foreach (ModHandlerFile mod in files)
            {
                ModHandlerFileExternal? modReplace = replacementMods.FirstOrDefault(x => x != null && x.ReplacementID == mod.ID, null);
                if (modReplace != null)
                {
                    Console.WriteLine($"download {mod.FileName} -> {modReplace.FileName}");

                    string dirEx = Path.Join(dirPath, modReplace.FileName);
                    RestUtils.Download(dirEx, modReplace.URL);

                    continue;
                }

                Console.WriteLine($"download {mod.FileName}");

                string filePath = Path.Join(dirPath, mod.FileName);
                RestUtils.Download(filePath, mod.URL);

                Download(mod.Dependencies, externals);
            }

            foreach (ModHandlerFileExternal mod in externalMods)
            {
                Console.WriteLine($"download {mod.FileName}");

                string dirEx = Path.Join(dirPath, mod.FileName);
                RestUtils.Download(dirEx, mod.URL);
            }
        }

        public static void Load(string dstPath, ModHandlerFile[] files, ModHandlerFileExternal[] externals)
        {
            ModHandlerFileExternal[] replacementMods = externals.Where(x => !string.IsNullOrWhiteSpace(x.ReplacementID)).ToArray();
            ModHandlerFileExternal[] externalMods = externals.Where(x => !replacementMods.Contains(x)).ToArray();

            foreach (ModHandlerFile mod in files)
            {
                string fileName = mod.FileName;

                ModHandlerFileExternal? modReplace = replacementMods.FirstOrDefault(x => x != null && x.ReplacementID == mod.ID, null);
                if (modReplace != null)
                {
                    fileName = modReplace.FileName;
                }

                string srcFile = Path.Combine(dirPath, fileName);
                string dstFile = Path.Combine(dstPath, fileName);

                FileHelper.Clone(srcFile, dstFile);

                Load(dstPath, mod.Dependencies, externals);
            }

            foreach (ModHandlerFileExternal mod in externalMods)
            {
                string srcFile = Path.Combine(dirPath, mod.FileName);
                string dstFile = Path.Combine(dstPath, mod.FileName);

                FileHelper.Clone(srcFile, dstFile);
            }
        }
    }
}
