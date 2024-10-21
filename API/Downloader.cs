using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.IO;

namespace wey.API
{
    public static class Downloader
    {
        static readonly RestClient rest;

        static Downloader()
        {
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                UserAgent = "thyonemc/wey (https://github.com/thyonemc/wey)"
            };

            rest = new(options);
        }

        // return "filePath"
        public static string Download(string url, string fileName)
        {
            RestRequest request = new(url);

            byte[]? data = rest.DownloadData(request);
            if (data == null) throw new Exception("downloader error - Downloader.Download");

            string path = Path.Join(ApplicationDirectoryHelper.Temporary, fileName);
            FileHelper.UpdateBytes(path, data);

            return path;
        }
    }
}
