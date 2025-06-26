using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.Utils;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PartyEventOption
{
    internal class PartyEventOption : IMenuOption
    {
        static int TotalRankOpenTime = 3 * 24 * 60;
        public string Name => "Party event";
        public async Task Start()
        {
            while(true)
            {
                try
                {
                    DateTime now = await DateTimeManager.GetUTCAsync();

                    string expiredString = await DBManager.FBClient.Child("PartyRank/TimeExpired").OnceAsJsonAsync();
                    expiredString = expiredString.Replace("\"", "");
                    long longExpired = long.Parse(expiredString);
                    DateTime expiredDate = longExpired.ToDate().AddMinutes(-5);
                    Console.WriteLine($"[Party] reset rank in {(expiredDate - now)}");
                    if (now >= expiredDate)
                    {
                        Console.WriteLine("[Party] Dowload party rank backup file...");
                        Console.WriteLine("[Party] Run reset rank party rank...");
                        await ResetPartyRank();
                        Console.WriteLine("[Party] Reset party rank success.");
                    }
                    await Task.Delay(60000);
                }
                catch (Exception ex)
                {
                    LogUtils.LogI(ex.Message);
                    LogUtils.LogI(ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
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
                        //await RankRewardSender.SendPartyRewardTo(user.Key, index);
                        index++;
                    }
                    await DBManager.FBClient.Child("PartyRank").Child(child.Key).DeleteAsync();
                }
            }
            await DBManager.FBClient.Child("PartyRank/TotalUserCount").PutAsync(0);

            DateTime now = await DateTimeManager.GetUTCAsync();
            DateTime nextExpiredDate = now.AddMinutes(TotalRankOpenTime).AddMinutes(5).Date;
            await DBManager.FBClient.Child("PartyRank/TimeExpired").PutAsync(nextExpiredDate.ToLong());
            Console.WriteLine("[Party] set next expired:" + nextExpiredDate);
        }
    }
}
