using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wey.Console;

namespace wey.Tool
{
    class Rest
    {
        // static

        private static readonly RestClient StaticClient = new(
                new RestClientOptions()
                {
                    ThrowOnAnyError = true,
                    ThrowOnDeserializationError = true,
                }
            );

        public static T StaticGet<T>(string url)
        {
            Task<T?> Data = StaticClient.GetAsync<T>(new RestRequest(url));

            if (Data == null) throw new NullReferenceException();
            if (Data.Result == null) throw new NullReferenceException();

            return Data.Result;
        }

        public static byte[] StaticDownload(string url)
        {
            Task<byte[]?> Data = StaticClient.DownloadDataAsync(new RestRequest(url));

            if (Data == null) throw new NullReferenceException();
            if (Data.Result == null) throw new NullReferenceException();

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
