using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PartyEventOption
{
    internal class PartyEventOption : IMenuOption
    {
        //static int TotalRankOpenTime = 3 * 24 * 60; // minute
        static int TotalRankOpenTime = 2; // minute

        public string Name => "Party event";

        public bool OptionAutoRun => true;

        public bool IsRunning { get; set; }

        public async Task Start()
        {
            IsRunning = true;
            while(true)
            {
                DateTime now = await DateTimeManager.GetUTCAsync();
                string expiredString = await DBManager.FBClient.Child("PartyRank/TimeExpired").OnceAsJsonAsync();
                expiredString = expiredString.Replace("\"", "");
                DateTime expiredDate = DateTime.ParseExact(expiredString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (now < expiredDate)
                {
                    TimeSpan diff = expiredDate - now;
                    await Task.Delay(diff);
                }
                await ResetPartyRank();

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
                        await RankRewardSender.SendPartyRewardTo(user.Key, index);
                        index++;
                    }
                    await DBManager.FBClient.Child("PartyRank").Child(child.Key).DeleteAsync();
                }
            }
            await DBManager.FBClient.Child("PartyRank/TotalUserCount").PutAsync(0);
            DateTime now = await DateTimeManager.GetUTCAsync();
            DateTime nextExpiredDate = now.AddMinutes(TotalRankOpenTime);
            string nextExpiredString = nextExpiredDate.ToString("dd/MM/yyyy HH:mm:ss");
            await DBManager.FBClient.Child("PartyRank/TimeExpired").PutAsync(JsonConvert.SerializeObject(nextExpiredString));

        }
        public void Stop()
        {
            IsRunning = false;
            Program.ShowMenu();
        }
    }
}
