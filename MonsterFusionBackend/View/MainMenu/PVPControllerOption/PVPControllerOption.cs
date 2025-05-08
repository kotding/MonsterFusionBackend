using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PVPControllerOption
{
    internal class PVPControllerOption : IMenuOption
    {
        const int TotalRankOpenTime = 7 * 24 * 60; // minute
        const int TotalRankCloseTime = 60;// minute
        public string Name => "PVP Controller Option";

        string[] allRankNames = new string[]
        {
            "Copper","Silver","Gold","Platinum","Diamond","Ultimate"
        };
        public async Task Start()
        {
            while(true)
            {
                try
                {
                    DateTime now = await DateTimeManager.GetUTCAsync();
                    string expiredString = await DBManager.FBClient.Child("PVP").Child("PVP_Config").Child("EndTime").OnceAsJsonAsync();
                    expiredString = expiredString.Replace("\"", "");
                    DateTime expiredDate = DateTime.ParseExact(expiredString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Console.WriteLine();
                    Console.WriteLine("[PVP]: now:" + now.ToString("dd/MM/yyyy HH:mm:ss"));
                    Console.WriteLine("[PVP]: expired:" + expiredString);
                    Console.WriteLine("[PVP]: reset after " + (expiredDate - now).ToString());
                    if (now >= expiredDate)
                    {
                        Console.WriteLine("[PVP]: dowload pvp backup file...");
                        await DowloadBackupPVP();
                        Console.WriteLine("[PVP]: dowload pvp backup file success.");
                        Console.WriteLine("[PVP]: start reset pvp rank...");
                        await RunResetRank();
                        Console.WriteLine("[PVP]: reset rank complete.");
                        Console.WriteLine($"[PVP]: wait re-open pvp.... {60 * TotalRankCloseTime}s");
                        await Task.Delay(1000 * 60 * TotalRankCloseTime);
                        await DBManager.FBClient.Child("PVP/IsOpen").PutAsync(JsonConvert.SerializeObject(true));
                        now = await DateTimeManager.GetUTCAsync();
                        now = now.AddMinutes(TotalRankOpenTime);
                        string nowString = now.ToString("dd/MM/yyyy HH:mm:ss");
                        await DBManager.FBClient.Child("PVP/PVP_Config/EndTime").PutAsync(JsonConvert.SerializeObject(nowString));
                        Console.WriteLine("[PVP]: pvp rank re-opened.");
                        await Task.Delay(1000 * 30);
                    }
                    Console.WriteLine("[PVP]: wait delay 60s...");
                    await Task.Delay(60000);
                }
                catch (Exception ex)
                {
                    LogUtils.LogI(ex.Message);
                    LogUtils.LogI(ex.StackTrace);
                }
            }
        }
        async Task DowloadBackupPVP()
        {
            string js = await DBManager.FBClient.Child("PVP").OnceAsJsonAsync();
            string backUpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PVPBackup_" + DateTime.UtcNow.ToString("dd-MM-yyyy-HH-mm-ss") + ".json");
            File.WriteAllText(backUpFilePath, js);
        }
        async Task RunResetRank()
        {
            await DBManager.FBClient.Child("PVP/IsOpen").PutAsync(JsonConvert.SerializeObject(false));
            List<AreaRank> listAreaRanks = new List<AreaRank>();
            Console.WriteLine("Fetch all data");
            for (int i = 0; i < allRankNames.Length; i++)
            {
                Console.WriteLine(allRankNames[i]);

                var areaRank = await GetAreaRank(allRankNames[i]);
                listAreaRanks.Add(areaRank);
            }


            Console.WriteLine("Run reset rank");
            for (int i = listAreaRanks.Count - 2; i >= 0; i--)
            {
                AreaRank currArea = listAreaRanks[i];
                AreaRank nextArea = listAreaRanks[i + 1];

                List<PVPRankData> listRankUps = currArea.listAllRanks.Where(x => x.RankIndex < 3 && x.RankPoint != 1000).ToList();
                Console.WriteLine("LIST ALL RANKS " + currArea.listAllRanks.Count);
                Console.WriteLine("LIST RANK UP " + listRankUps.Count);
                if (listRankUps != null)
                {
                    foreach (var rank in listRankUps)
                    {
                        Console.Write("Send reward to " + rank.UserID + " top " + rank.RankIndex);
                        await RankRewardSender.SendRewardTo(rank.UserID, rank.RankType, rank.RankIndex);
                        Console.WriteLine("  [sended]");
                        rank.RankType = (RankType)Enum.Parse(typeof(RankType), allRankNames[(i + 1)].ToUpper(), true);
                        rank.RankPoint = 1000;
                        rank.RankIndex = 999;
                    }

                    nextArea.listAllRanks.AddRange(listRankUps);
                    currArea.listAllRanks.Clear();
                    foreach(var rank in currArea.listAllRanks)
                    {
                        rank.RankIndex = 999;
                        rank.RankPoint = 1000;
                    }
                }
            }

            foreach (var area in listAreaRanks)
            {
                area.listSubAreaRanks.Clear();
                area.listSubAreaRanks.Add(new SubAreaRank());
                foreach (var rank in area.listAllRanks)
                {
                    SubAreaRank subAreaRank = area.listSubAreaRanks.Find(sub => sub.listRanks.Count < 100);
                    if (subAreaRank == null)
                    {
                        subAreaRank = new SubAreaRank();
                        area.listSubAreaRanks.Add(subAreaRank);
                    }
                    subAreaRank.listRanks.Add(rank);
                    subAreaRank.UserCount = subAreaRank.listRanks.Count;
                }
                area.SubAreaCount = area.listSubAreaRanks.Count;
            }

            Console.WriteLine("Logging");
            for (int i = listAreaRanks.Count - 1; i >= 0; i--)
            {
                Console.WriteLine("-----" + allRankNames[i] + "_" + listAreaRanks[i].SubAreaCount);
                int subRankIndex = 1;
                foreach (var subRank in listAreaRanks[i].listSubAreaRanks)
                {
                    if (subRank.listRanks.Count > 0)
                        Console.WriteLine("    |   |--------------" + allRankNames[i] + "_Sub_" + subRankIndex);
                    foreach (var rank in subRank.listRanks)
                    {
                        Console.WriteLine("    |   |                  |-----------" + rank.UserName);
                    }
                    subRankIndex++;
                }
            }
            Console.WriteLine("Push all data");
            await PushBoard(listAreaRanks);
            await DBManager.FBClient.Child("PVP/Historys").DeleteAsync();
            Console.WriteLine("All is data pushed");
        }

        async Task<AreaRank> GetAreaRank(string currRank)
        {
            string subAreaCountJs = await DBManager.FBClient.Child("PVP").Child("Rankings")
                                .Child(currRank).Child("SubAreaCount").OnceAsJsonAsync();
            int subAreaCount = subAreaCountJs == "null" ? 0 : int.Parse(subAreaCountJs);
            Console.WriteLine("Sub area count " + subAreaCount);
            AreaRank areaRank = new AreaRank();
            areaRank.listSubAreaRanks = new List<SubAreaRank>();
            areaRank.SubAreaCount = subAreaCount;
            for (int i = 1; i <= subAreaCount; i++)
            {
                SubAreaRank subAreaRank = new SubAreaRank();
                string userCountJs = await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRank).Child(currRank + "_" + i).Child("UserCount").OnceAsJsonAsync();
                int userCount = userCountJs == "null" ? 0 : int.Parse(userCountJs);
                Console.WriteLine(currRank + " sub area " + i + " user count " + userCount);
                subAreaRank.UserCount = userCount;
                try
                {
                    string subAreaJs = await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRank).Child(currRank + "_" + i).OnceAsJsonAsync();
                    string pattern = "\"UserCount\":\\s*\\d+(?:,)?";
                    subAreaJs = Regex.Replace(subAreaJs, pattern, "");
                    Dictionary<string, PVPRankData> dictRanks = JsonConvert.DeserializeObject<Dictionary<string, PVPRankData>>(subAreaJs);
                    if (dictRanks != null && dictRanks.Values != null)
                    {
                        Console.WriteLine("Dict ranks " + dictRanks.Count);
                        subAreaRank.listRanks = dictRanks.Values.ToList();
                        areaRank.listAllRanks.AddRange(subAreaRank.listRanks);
                        areaRank.listSubAreaRanks.Add(subAreaRank);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return areaRank;
        }
        async Task PushBoard(List<AreaRank> listAreas)
        {
            foreach (var rankName in allRankNames)
            {
                await DBManager.FBClient.Child("PVP").Child("Rankings").Child(rankName).DeleteAsync();
            }

            var rankingsData = new Dictionary<string, object>();
            var rankIndexsData = new Dictionary<string, Dictionary<string, string>>();

            for (int i = 0; i < listAreas.Count; i++)
            {
                var currRankArea = listAreas[i];
                string currRankName = allRankNames[i];

                var rankData = new Dictionary<string, object>
        {
            { "SubAreaCount", currRankArea.SubAreaCount }
        };

                for (int currSubAreaIndex = 0; currSubAreaIndex < currRankArea.SubAreaCount; currSubAreaIndex++)
                {
                    var subAreaData = new Dictionary<string, object>
            {
                { "UserCount", currRankArea.listSubAreaRanks[currSubAreaIndex].UserCount }
            };

                    foreach (var rank in currRankArea.listSubAreaRanks[currSubAreaIndex].listRanks)
                    {
                        rankIndexsData[rank.UserID] = new Dictionary<string, string>
                {
                    { "Path", $"{currRankName}/{currRankName}_{currSubAreaIndex + 1}" }
                };

                        subAreaData[rank.UserID] = rank;
                        Console.WriteLine("Add " + rank.UserID);
                    }

                    string subAreaKey = $"{currRankName}_{currSubAreaIndex + 1}";
                    rankData[subAreaKey] = subAreaData;
                }

                rankingsData[currRankName] = rankData;
            }

            rankingsData["RankIndexs"] = rankIndexsData;
            await DBManager.FBClient.Child("PVP/Rankings/RankIndexs").DeleteAsync();
            await DBManager.FBClient.Child("PVP").Child("Rankings").PutAsync(JsonConvert.SerializeObject(rankingsData));
        }
    }
}
