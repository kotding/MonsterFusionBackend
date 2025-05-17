using MonsterFusionBackend.Data;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using System.IO;
using System;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;
using MonsterFusionBackend.Utils;

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
                try
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
                }catch (Exception ex)
                {
                    LogUtils.LogI("[SoloBattle] " + ex.Message);
                }
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
            var allUser = await DBManager.FBClient
                .Child("SoloBattleRank/Solo1vs1Rank/AllUserRank")
                .OnceAsync<object>();

            List<FirebaseObject<object>> allUserList = new List<FirebaseObject<object>>(allUser);
            Console.WriteLine($"Total users before cleanup: {allUserList.Count}");

            Dictionary<string, List<FirebaseObject<object>>> groupDict = new Dictionary<string, List<FirebaseObject<object>>>();
            List<FirebaseObject<object>> activeUsers = new List<FirebaseObject<object>>();

            foreach (var user in allUserList)
            {
                try
                {
                    SoloRank userData = JsonConvert.DeserializeObject<SoloRank>(user.Object.ToString());
                    if (int.TryParse(userData.DailyRankPoint, out int point) && point > 0)
                    {
                        string group = await DBManager.FBClient
                            .Child("SoloBattleRank/Solo1vs1Rank/AllUserRank")
                            .Child(user.Key)
                            .Child("IndexOfRankgroup")
                            .OnceSingleAsync<string>();

                        if (!groupDict.ContainsKey(group))
                            groupDict[group] = new List<FirebaseObject<object>>();
                        groupDict[group].Add(user);

                        activeUsers.Add(user); // giữ lại user hoạt động
                    }
                    else
                    {
                        // xóa user đã nghỉ chơi (DailyRankPoint = 0)
                        await DBManager.FBClient
                            .Child("SoloBattleRank/Solo1vs1Rank/AllUserRank")
                            .Child(user.Key)
                            .DeleteAsync();
                        Console.WriteLine("Solo battle : delete user " + user.Key);
                    }
                }catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[SoloBattle] " + ex.Message + " " + ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                
            }
            Console.WriteLine("[SoloBattle] Send reward");
            // Gửi phần thưởng theo group
            foreach (var kvp in groupDict)
            {
                var list = kvp.Value;
                list.Sort((a, b) =>
                {
                    int aPoint = int.Parse(JsonConvert.DeserializeObject<SoloRank>(a.Object.ToString()).DailyRankPoint);
                    int bPoint = int.Parse(JsonConvert.DeserializeObject<SoloRank>(b.Object.ToString()).DailyRankPoint);
                    return bPoint.CompareTo(aPoint);
                });

                for (int i = 0; i < list.Count; i++)
                {
                    string userId = JsonConvert.DeserializeObject<SoloRank>(list[i].Object.ToString()).UserId;
                    await RankRewardSender.SendSoloBattleReward(userId, i);
                    Console.WriteLine("[SoloBattle] Send reward to " + userId);
                }
            }
            Console.WriteLine("[SoloBattle] Reset rankpoint for active user");
            // Reset điểm về 0 cho user còn hoạt động
            foreach (var user in activeUsers)
            {
                await DBManager.FBClient
                    .Child("SoloBattleRank/Solo1vs1Rank/AllUserRank")
                    .Child(user.Key)
                    .Child("DailyRankPoint")
                    .PutAsync("0");
                Console.WriteLine("[SoloBattle] reset rank point " + user.Key);
            }

            // Chia lại group cho user còn hoạt động (mỗi 100 người)
            Shuffle(activeUsers);
            for (int i = 0; i < activeUsers.Count; i++)
            {
                int newGroup = i / 100;
                await DBManager.FBClient
                    .Child("SoloBattleRank/Solo1vs1Rank/AllUserRank")
                    .Child(activeUsers[i].Key)
                    .Child("IndexOfRankgroup")
                    .PutAsync(newGroup.ToString());
            }
            Console.WriteLine("[SoloBattle] Update total user " + activeUsers.Count);
            await DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/TotalUser").PutAsync(activeUsers.Count);
            await Task.Delay(60 * 1000);
            DateTime now = await DateTimeManager.GetUTCAsync();
            DateTime nextExpired = now.AddDays(1);
            await DBManager.FBClient.Child("SoloBattleRank/Solo1vs1Rank/TimeExpired").PutAsync(nextExpired.ToLong());
            Console.WriteLine("[SoloBattle] Reset + reward + regroup completed.");
        }

        static void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
    internal class SoloRank
    {
        public string DailyRankPoint;
        public string UserId;
    }
}
