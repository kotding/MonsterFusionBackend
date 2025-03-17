using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace MonsterFusionBackend.Data
{
    internal static class DBManager
    {
        static FirebaseClient fbClient = new FirebaseClient("https://monster-fusion-test-android-default-rtdb.firebaseio.com/", new FirebaseOptions
        {
            JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy HH:mm:ss",
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate
            }
        });

        public static FirebaseClient FBClient => fbClient;
    }
}
