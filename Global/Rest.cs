using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using wey.Console;

namespace wey.Global
{
    class RestNullException : Exception
    {
        public RestNullException(string? message = null) : base(message)
        {

        }
    }

    class Rest
    {
        // static

        private static readonly RestClient Client = new(
                new RestClientOptions()
                {
                    ThrowOnAnyError = true,
                    ThrowOnDeserializationError = true,
                }
            );

        public static T StaticGet<T>(string url)
        {
            Logger.Log($"GET: {url}");

            Task<T?> Data = Client.GetAsync<T>(new RestRequest(url));

            if (Data == null) throw new RestNullException();
            if (Data.Result == null) throw new RestNullException();

            return Data.Result;
        }

        public static byte[] StaticDownload(string url)
        {
            Logger.Log($"DOWNLOAD: {url}");

            Task<byte[]?> Data = Client.DownloadDataAsync(new RestRequest(url));

            if (Data == null) throw new RestNullException();
            if (Data.Result == null) throw new RestNullException();

            return Data.Result;
        }

        // class

        private readonly string baseUrl;

        public Rest(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public T Get<T>(string url)
        {
            return StaticGet<T>($"{baseUrl}{url}");
        }

        public byte[] Download(string url)
        {
            return StaticDownload($"{baseUrl}{url}");
        }
    }
}
