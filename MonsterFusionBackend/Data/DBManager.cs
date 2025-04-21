using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;

namespace MonsterFusionBackend.Data
{
    internal static class DBManager
    {
        const string firebaseIOS = "https://monster-fusion-ios-default-rtdb.firebaseio.com/";
        const string firebaseAndroid = "https://monsterfusion-c0e4e-default-rtdb.firebaseio.com/PartyRank";
        const string firebaseTest = "https://monster-fusion-test-android-default-rtdb.firebaseio.com/";
        static FirebaseClient fbClient;
        public static void SetFBDatabaseUrl(string param)
        {
            if (string.IsNullOrEmpty(param)) return;
            string url = firebaseTest;
            if (param == "test") url = firebaseTest;
            else if (param == "android") url = firebaseAndroid;
            else if (param == "ios") url = firebaseIOS;
            else
            {
                Console.WriteLine("Parrem invalid");
            }

            fbClient = new FirebaseClient(url, new FirebaseOptions
            {
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    DateFormatString = "dd/MM/yyyy HH:mm:ss",
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Populate
                }
            });
        }
        public static FirebaseClient FBClient => fbClient;
    }
}
