using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wey.IO;

namespace wey.API
{
    public static class RestUtils
    {
        static readonly RestClient rest;

        static RestUtils()
        {
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                UserAgent = "thyonemc/wey (https://github.com/thyonemc/wey)"
            };

            rest = new(options);
        }

        public static Uri? ParseUri(string path)
        {
            UriKind fullUrlStructureType = UriKind.Absolute;

            if (
                    Uri.TryCreate(path, fullUrlStructureType, out Uri? uri) &&
                    uri != null &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                )
            {
                return uri;
            }
            else
            {
                return null;
            }
        }

        public static void Download(string filePath, string url)
        {
            Download(filePath, new Uri(url));
        }

        public static void Download(string filePath, Uri uri)
        {
            if (File.Exists(filePath)) return;

            RestRequest request = new(uri);

            byte[]? data = rest.DownloadData(request);
            if (data == null) throw new Exception("downloader error - Downloader.Download");

            FileHelper.UpdateBytes(filePath, data);
        }
    }
}
