using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wey.IO
{
    public static class DirectoryHelper
    {
        public static string GetRootDirectory(string path)
        {
            return Path.GetDirectoryName(path) ?? string.Empty;
        }

        public static string GetRootDirectoryName(string path)
        {
            return Path.GetFileName(GetRootDirectory(path)) ?? string.Empty;
        }

        public static void Create(string path)
        {
            if (Directory.Exists(path)) return;
            if (string.IsNullOrWhiteSpace(path)) return;

            Directory.CreateDirectory(path);
        }

        public static void Clone(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath)) return;

            CloneFiles(sourcePath, destinationPath);

            foreach (string directory in Directory.GetDirectories(sourcePath))
            {
                string directoryName = GetRootDirectoryName(directory);

                Clone(
                        Path.Join(sourcePath, directoryName),
                        Path.Join(destinationPath, directoryName)
                    );
            }
        }

        public static void CloneFiles(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath)) return;

            Create(destinationPath);

            foreach (string fileName in Directory.GetFiles(sourcePath))
            {
                string destFileName = Path.Join(destinationPath, Path.GetFileName(fileName));

                FileHelper.Clone(fileName, destFileName);
            }
        }

        public static void Delete(string path)
        {
            if (!Directory.Exists(path)) return;

            Directory.Delete(path, true);
        }
    }

    public static class ApplicationDirectoryHelper
    {
        static readonly bool IsWindows;
        static readonly bool IsMacOS;
        static readonly bool IsLinux;

        public static readonly string Appdata;
        public static readonly string Temporary;

        static ApplicationDirectoryHelper()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            Appdata = GetDirectory("thyonemc", "wey");
            Temporary = Path.Join(Path.GetTempPath(), "thyonemc", "wey");

            DirectoryHelper.Create(Appdata);

            DirectoryHelper.Delete(Temporary);
            DirectoryHelper.Create(Temporary);
        }

        public static string GetDirectory(params string[] path)
        {
            string joinpath = Path.Join(path);

            if (IsWindows)
                return Path.GetFullPath(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), joinpath));

            if (IsMacOS)
                return Path.GetFullPath(Path.Join("Library", "Application Support", joinpath));

            if (IsLinux) 
                return Path.GetFullPath(joinpath);

            throw new Exception("unsupported operating system");
        }

        public static string GetDirectoryByOS(string windows, string macos, string linux)
        {
            if (IsWindows) return GetDirectory(windows);
            if (IsMacOS) return GetDirectory(macos);
            if (IsLinux) return GetDirectory(linux);

            throw new Exception("unsupported operating system");
        }
    }
}
