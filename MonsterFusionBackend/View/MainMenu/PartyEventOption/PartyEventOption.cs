using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PartyEventOption
{
    internal class PartyEventOption : IMenuOption
    {
        //static int TotalRankOpenTime = 3 * 24 * 60; // minute
        //static int OffsetResetTime = -5; // minute

        static int TotalRankOpenTime = 5; // minute
        static int OffsetResetTime = -5; // minute

        public string Name => "Party event";

        public bool IsRunning { get; set; }

        public async Task Start()
        {
            IsRunning = true;
            //Program.ShowMenu();
            while(true)
            {
                DateTime now = await DateTimeManager.GetUTCAsync();

                string expiredString = await DBManager.FBClient.Child("PartyRank/TimeExpired").OnceAsJsonAsync();
                expiredString = expiredString.Replace("\"", "");
                long longExpired = long.Parse(expiredString);
                DateTime expiredDate = longExpired.ToDate().AddMinutes(OffsetResetTime);
                Console.WriteLine();
                Console.WriteLine("[Party] now: " + now);
                Console.WriteLine("[Party] expired: " + expiredDate);
                Console.WriteLine($"[Party] reset rank in {(expiredDate - now).TotalSeconds}s.");
                if (now >= expiredDate)
                {
                    Console.WriteLine("[Party] Dowload party rank backup file...");
                    await DowloadBackupParty();
                    Console.WriteLine("[Party] Dowload party rank back up file success.");
                    Console.WriteLine("[Party] Run reset rank party rank...");
                    await ResetPartyRank();
                    Console.WriteLine("[Party] Reset party rank success.");
                }
                await Task.Delay(60000);
            }
        }
        async Task DowloadBackupParty()
        {
            string js = await DBManager.FBClient.Child("PartyRank").OnceAsJsonAsync();
            string backUpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PartyRank_" + DateTime.UtcNow.ToString("dd-MM-yyyy-HH-mm-ss") + ".json");
            File.WriteAllText(backUpFilePath, js);
        }
        async Task ResetPartyRank()
        {
            var partyRank = await DBManager.FBClient.Child("PartyRank").OnceAsync<object>();
            foreach(var child in partyRank)
            {
                if(child.Key.Contains("PartyGroup_"))
                {
                    var group = await DBManager.FBClient.Child("PartyRank").Child(child.Key).OrderBy("RankPoint").LimitToLast(50).OnceAsync<object>();
                    int index = 0;
                    foreach(var user in group)
                    {
                        await RankRewardSender.SendPartyRewardTo(user.Key, index);
                        index++;
                    }
                    await DBManager.FBClient.Child("PartyRank").Child(child.Key).DeleteAsync();
                }
            }
            await DBManager.FBClient.Child("PartyRank/TotalUserCount").PutAsync(0);

            DateTime now = await DateTimeManager.GetUTCAsync();
            DateTime nextExpiredDate = now.AddMinutes(TotalRankOpenTime).AddMinutes(-OffsetResetTime);
            Console.WriteLine("[Party] set next expired:" + nextExpiredDate);
            await DBManager.FBClient.Child("PartyRank/TimeExpired").PutAsync(nextExpiredDate.ToLong());
        }
        public void Stop()
        {
            IsRunning = false;
            Program.ShowMenu();
        }
    }
}
