using MonsterFusionBackend.Data;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using System.IO;
using System;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Collections.Generic;
using Firebase.Database;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MonsterFusionBackend.View.MainMenu.SoloBattleOption
{
    internal class SoloBattleOption : IMenuOption
    {
        public string Name => "Solo battle";
        // Chay vong lap de check thoi gian reset (1p/lan)
        
        public async Task Start()
        {
            while (true)
            {
                // Call thoi gian hien tai & tgian ket thuc
                DateTime now = await DateTimeManager.GetUTCAsync();
                string expiredString = await DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/TimeExpired").OnceAsJsonAsync();

                expiredString = expiredString.Replace("\"", "");
                long longExpired = long.Parse(expiredString);
                DateTime expiredDate = longExpired.ToDate();

                Console.WriteLine();
                Console.WriteLine("[SoloBattle] expired: " + expiredDate);
                Console.WriteLine($"[SoloBattle] reset rank in {(expiredDate - now)}");
                
                if (now >= expiredDate)
                {
                    // dowload backup file truoc khi reset
                    Console.WriteLine("[SoloBattle] Dowload backup file...");
                    await DownloadBackupFile();
                    Console.WriteLine("[SoloBattle] Dowload back up file success.");
                    Console.WriteLine("[SoloBattle] Run reset rank rank...");

                    // tien hanh reset
                    await ResetSoloBattle();
                    Console.WriteLine("[SoloBattle] Reset rank success.");
                }
                await Task.Delay(60000);
            }
        }
        async Task DownloadBackupFile()
        {
            string js = await DBManager.FBClient.Child("SoloBattleRank").OnceAsJsonAsync();
            string backUpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoloBattleRank_" + DateTime.UtcNow.ToString("dd-MM-yyyy-HH-mm-ss") + ".json");
            File.WriteAllText(backUpFilePath, js);
        }
        async Task ResetSoloBattle()
        {
            var allUser = await DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/AllUserRank").OnceAsync<object>();
            Dictionary<string, List<FirebaseObject<object>>> dict = new Dictionary<string, List<FirebaseObject<object>>>();

            // lay toan bo user, sau do chia thanh cac nhom khac nhau
            foreach (var child in allUser)
            {
                List<FirebaseObject<object>> listUserInGroup = null;
                string group = DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/AllUserRank").Child(child.Key).Child("IndexOfRankgroup").OnceSingleAsync<string>().Result.ToString();
                Console.WriteLine("Group " + group);
                if(dict.ContainsKey(group))
                {
                    listUserInGroup = dict[group];
                }
                else
                {
                    listUserInGroup = new List<FirebaseObject<object>>();
                    dict[group] = listUserInGroup;
                }
                listUserInGroup.Add(child);
            }
            Console.WriteLine("Group : " + dict.Keys.Count);
            // sap xep lai cac nhom va gui thuong
            foreach(var key in dict.Keys)
            {
                var list = dict[key];
                list.Sort((a, b) =>
                {
                    int aPoint = int.Parse(JsonConvert.DeserializeObject<SoloRank>(a.Object.ToString()).DailyRankPoint);
                    int bPoint = int.Parse(JsonConvert.DeserializeObject<SoloRank>(b.Object.ToString()).DailyRankPoint);
                    if (aPoint > bPoint) return 1;
                    if(bPoint > aPoint) return -1;
                    return 0;
                });

                for(int i = 0; i < list.Count; i++)
                {
                    await RankRewardSender.SendSoloBattleReward(list[i].Key, i);
                }
            }
            //DateTime now = await DateTimeManager.GetUTCAsync();
            //DateTime nextExpiredDate = now.AddMinutes(24 * 60).AddMinutes(5);
            //await DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/TimeExpired").PutAsync(nextExpiredDate.ToLong());
            //Console.WriteLine("[SolotBattle] set next expired:" + nextExpiredDate);
        }
    }
    internal class SoloRank
    {
        public string DailyRankPoint;
    }
}
