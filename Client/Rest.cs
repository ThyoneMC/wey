using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wey.Client
{
    class Rest
    {
        // static

        private static readonly RestClient StaticClient = new();

        public static Task<T?> StaticGet<T>(string url)
        {
            return StaticClient.GetAsync<T>(new RestRequest(url));
        }

        public static Task<byte[]?> StaticDownload(string url)
        {
            return StaticClient.DownloadDataAsync(new RestRequest(url));
        }

        // class

        private string baseUrl;

        public Rest(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Task<T?> Get<T>(string url)
        {
            return StaticClient.GetAsync<T>(new RestRequest($"{baseUrl}{url}"));
        }

        public Task<byte[]?> Download(string url)
        {
            return StaticClient.DownloadDataAsync(new RestRequest($"{baseUrl}{url}"));
        }
    }
}
