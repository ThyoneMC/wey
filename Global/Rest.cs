using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using wey.Console;
using System.Net.Http.Headers;

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

        private static readonly RestClient Client;

        static Rest()
        {
            Client = new(
                new RestClientOptions()
                {
                    ThrowOnAnyError = true,
                    ThrowOnDeserializationError = true,
                    UserAgent = "thyonemc/wey",
                }
            );

            Client.AddDefaultHeader("Accept", "application/json");
        }

        public static T StaticGet<T>(string url)
        {
            Logger.Log($"GET: {url}");

            T? request = Client.Get<T>(new RestRequest(url));
            if (request == null) throw new RestNullException();

            return request;
        }

        public static byte[] StaticDownload(string url)
        {
            Logger.Log($"DOWNLOAD: {url}");

            byte[]? request = Client.DownloadData(new RestRequest(url));
            if (request == null) throw new RestNullException();

            return request;
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
