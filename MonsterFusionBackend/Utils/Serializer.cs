using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.Utils
{
    internal class Serializer
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy HH:mm:ss",
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate
        };
        /// <summary>
        /// Serialize một đối tượng thành Json, kiểu DateTime sau chuyển đổi sẽ có format dd/MM/yyyy HH:mm:ss, các trường đang có giá trị mặc định sẽ không thêm vào trong Json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
        /// <summary>
        /// Dessrialize một Json thành Object T, các trường không có trong Json sẽ giữ nguyên giá trị trong khai báo biến. Lưu ý: Json chứ DateTime phải đúng định dạng dd/MM/yyyy HH:mm:ss nếu không thì tỏi.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, settings);
        }
    }
}
