using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace wey.Global
{
    class JsonEncryptionException : Exception
    {
        public JsonEncryptionException(string? message = null) : base(message)
        {
            
        }
    }

    class JsonEncryption<T>
    {
        public static string Encrypt(T json)
        {
            string str = JsonSerializer.Serialize(json);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static T Decrypt(string json)
        {
            string decrypt = Encoding.UTF8.GetString(Convert.FromBase64String(json));

            T? obj = JsonSerializer.Deserialize<T>(decrypt);
            if (obj == null) throw new JsonEncryptionException();

            return obj;
        }
    }
}
