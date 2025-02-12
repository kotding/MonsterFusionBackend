using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.Utils
{
    internal class RESTCaller
    {
        public static async Task<string> GET(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string responseText = await reader.ReadToEndAsync();
                        return responseText;
                    }
                }
            }
        }

    }
}
