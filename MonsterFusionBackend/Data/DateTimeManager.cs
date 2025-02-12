using MonsterFusionBackend.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.Data
{
    internal class DateTimeManager
    {
        public static async Task<DateTime> GetUTCAsync()
        {
            string api = "http://104.248.159.111/api/time/global-time";
            var tcs = new TaskCompletionSource<DateTime>();
            string js = await RESTCaller.GET(api);
            var data = JsonConvert.DeserializeObject<DateTime>(js);
            tcs.SetResult(data);
            return await tcs.Task;
        }

    }
}
