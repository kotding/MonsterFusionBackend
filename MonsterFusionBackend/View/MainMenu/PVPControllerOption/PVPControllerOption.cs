using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PVPControllerOption
{
    internal class PVPControllerOption : IMenuOption
    {
        public string Name => "PVP Controller Option";

        string[] allRankNames = new string[]
        {
            "Copper","Silver","Gold","Platinum","Diamond","Ultimate"
        };
        const string DATE_TIME_PATTERN = "yyyy-MM-dd HH:mm:ss";
        const string DATE_TIME_PATTERN2 = "dd/MM/yyyy HH:mm:ss";
        public async Task Start()
        {
            while (true)
            {
                try
                {
                    DateTime now = await DateTimeManager.GetUTCAsync();
                    string pvpEventJson = await DBManager.FBClient.Child("PVP").Child("RemoteEventData").OnceAsJsonAsync();
                    PVP_Event_Data pvpEventData = JsonConvert.DeserializeObject<PVP_Event_Data>(pvpEventJson);
                    DateTime expiredDate = DateTime.ParseExact(pvpEventData.time_expired, DATE_TIME_PATTERN, CultureInfo.InvariantCulture);
                    Console.WriteLine();
                    Console.WriteLine("[PVP]: now:" + now.ToString(DATE_TIME_PATTERN));
                    Console.WriteLine("[PVP]: expired:" + pvpEventJson);
                    Console.WriteLine("[PVP]: reset after " + (expiredDate - now).ToString());
                    if (now >= expiredDate)
                    {
                        Console.WriteLine("[PVP]: dowload pvp backup file...");
                        Console.WriteLine("[PVP]: dowload pvp backup file success.");
                        Console.WriteLine("[PVP]: start reset pvp rank...");
                        await RunResetRank();
                        Console.WriteLine("[PVP]: reset rank complete.");
                        Console.WriteLine($"[PVP]: wait re-open pvp.... {60}s");
                        await Task.Delay(1000 * 20);
                        await DBManager.FBClient.Child("PVP/IsOpen").PutAsync(JsonConvert.SerializeObject(true));
                        now = await DateTimeManager.GetUTCAsync();
                        int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                        if (daysUntilNextMonday == 0)
                            daysUntilNextMonday = 7;
                        DateTime expired = now.Date.AddDays(daysUntilNextMonday);
                        string expiredString = expired.ToString(DATE_TIME_PATTERN);
                        pvpEventData.time_expired = expiredString;
                        pvpEventData.time_start = now.ToString(DATE_TIME_PATTERN);
                        pvpEventData.uid_event += 1;
                        await DBManager.FBClient.Child("PVP/RemoteEventData").PutAsync(JsonConvert.SerializeObject(pvpEventData));
                        await DBManager.FBClient.Child("PVP/PVP_Config/EndTime").PutAsync(JsonConvert.SerializeObject(expired.ToString(DATE_TIME_PATTERN2)));
                        Console.WriteLine("[PVP]: pvp rank re-opened.");
                        await Task.Delay(1000 * 30);
                    }
                    Console.WriteLine("[PVP]: wait delay 60s...");
                }
                catch (Exception ex)
                {
                    LogUtils.LogI(ex.Message);
                    LogUtils.LogI(ex.StackTrace);
                }
                await Task.Delay(20 * 60000);
            }
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
                List<PVPRankData> listRankClaimRewards = currArea.listAllRanks.Where(x => x.RankIndex < 50 && x.RankPoint != 1000).ToList();
                Console.WriteLine("LIST ALL RANKS " + currArea.listAllRanks.Count);
                Console.WriteLine("LIST RANK UP " + listRankUps.Count);
                if (listRankUps != null)
                {
                    foreach (var rank in listRankClaimRewards)
                    {
                        //Console.Write("Send reward to " + rank.UserID + " top " + rank.RankIndex);
                        //await RankRewardSender.SendRewardTo(rank.UserID, rank.RankType, rank.RankIndex);
                        //Console.WriteLine("  [sended]");
                        rank.RankType = (RankType)Enum.Parse(typeof(RankType), allRankNames[(i + 1)].ToUpper(), true);
                        rank.RankPoint = 1000;
                        rank.RankIndex = 999;
                    }

                    nextArea.listAllRanks.AddRange(listRankUps);
                    currArea.listAllRanks.RemoveAll(x => listRankUps.Contains(x));
                    currArea.listAllRanks.Confuse();
                    if (i == 0) currArea.listAllRanks.Clear();
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
                        if (rank == null || string.IsNullOrEmpty(rank.UserID)) continue;
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
[Serializable]
public class PVP_Event_Data
{
    public string time_expired;
    public string time_start;
    public uint uid_event;
}
