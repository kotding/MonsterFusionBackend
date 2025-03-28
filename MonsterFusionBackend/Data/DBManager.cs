using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace MonsterFusionBackend.Data
{
    internal static class DBManager
    {
        const string firebaseIOS = "https://monster-fusion-ios-default-rtdb.firebaseio.com/";
        const string firebaseAndroid = "https://monsterfusion-c0e4e-default-rtdb.firebaseio.com/PartyRank";
        const string firebaseTest = "https://monster-fusion-test-android-default-rtdb.firebaseio.com/";
        static FirebaseClient fbClient = new FirebaseClient(firebaseTest, new FirebaseOptions
        {
            JsonSerializerSettings = new JsonSerializerSettings
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
