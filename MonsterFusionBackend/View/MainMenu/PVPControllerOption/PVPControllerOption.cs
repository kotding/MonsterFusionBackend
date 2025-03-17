using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PVPControllerOption
{
    
    internal class PVPControllerOption : IMenuOption
    {
        const int TotalRankOpenTime = 7 * 24 * 60; // minute
        const int TotalRankCloseTime = 1 * 24 * 60;// minute


        public string Name => "PVP Controller Option";

        public bool OptionAutoRun => false;

        public bool IsRunning { get; set; }
        string[] allRankNames = new string[]
        {
            "Copper","Silver","Gold","Platinum","Diamond","Ultimate"
        };
        public async Task Start()
        {
            IsRunning = true;
            while(true)
            {
                DateTime now = await DateTimeManager.GetUTCAsync();
                string expiredString = await DBManager.FBClient.Child("PVP/PVP_Config/EndTime").OnceAsJsonAsync();
                expiredString = expiredString.Replace("\"", "");
                DateTime expiredDate = DateTime.ParseExact(expiredString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (now < expiredDate)
                {
                    TimeSpan diff = expiredDate - now;
                    await Task.Delay(diff);
                }
                await RunResetRank();
                await Task.Delay(1000 * 60 * TotalRankCloseTime);
                await DBManager.FBClient.Child("PVP/IsOpen").PutAsync(JsonConvert.SerializeObject(true));
                now = await DateTimeManager.GetUTCAsync();
                now = now.AddMinutes(TotalRankOpenTime);
                string nowString = now.ToString("dd/MM/yyyy HH:mm:ss");
                await DBManager.FBClient.Child("PVP/PVP_Config/EndTime").PutAsync(JsonConvert.SerializeObject(nowString));
            }
        }
        async Task RunResetRank()
        {
            await Task.Delay(100);
            Console.Clear();
            await DBManager.FBClient.Child("PVP/IsOpen").PutAsync(JsonConvert.SerializeObject(false));
            List<AreaRank> listAreaRanks = new List<AreaRank>();
            for (int i = 0; i < allRankNames.Length; i++)
            {
                listAreaRanks.Add(await GetAreaRank(allRankNames[i]));
            }

            for (int i = listAreaRanks.Count - 2; i >= 0; i--)
            {
                AreaRank currArea = listAreaRanks[i];
                AreaRank nextArea = listAreaRanks[i + 1];

                List<PVPRankData> listRankUps = currArea.listAllRanks.Where(x => x.RankIndex < 3).ToList();
                foreach(var rank in listRankUps)
                {
                    await RankRewardSender.SendRewardTo(rank.UserID, rank.RankType, rank.RankIndex);
                    rank.RankType = (RankType)Enum.Parse(typeof(RankType), allRankNames[(i + 1)].ToUpper(), true);
                    rank.RankPoint = 1000;
                }
                if (listRankUps != null)
                {
                    nextArea.listAllRanks.AddRange(listRankUps);
                }
                currArea.listAllRanks.RemoveAll(x => listRankUps.Contains(x));
            }

            foreach(var area in listAreaRanks)
            {
                area.listSubAreaRanks.Clear();
                area.listSubAreaRanks.Add(new SubAreaRank());
                foreach (var rank in area.listAllRanks)
                {
                    SubAreaRank subAreaRank = area.listSubAreaRanks.Find(sub => sub.listRanks.Count < 20);
                    if(subAreaRank == null)
                    {
                        subAreaRank = new SubAreaRank();
                        area.listSubAreaRanks.Add(subAreaRank);
                    }
                    subAreaRank.listRanks.Add(rank);
                    subAreaRank.UserCount = subAreaRank.listRanks.Count;
                }
                area.SubAreaCount = area.listSubAreaRanks.Count;
            }
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

            Console.WriteLine("Pushing board...");
            await PushBoard(listAreaRanks);
            Console.WriteLine("Board is pushed.");

        }

        async Task<AreaRank> GetAreaRank(string currRank)
        {
            string subAreaCountJs = await DBManager.FBClient.Child("PVP").Child("Rankings")
                                .Child(currRank).Child("SubAreaCount").OnceAsJsonAsync();
            int subAreaCount = subAreaCountJs == "null" ? 0 : int.Parse(subAreaCountJs);
            AreaRank areaRank = new AreaRank();
            areaRank.listSubAreaRanks = new List<SubAreaRank>();
            areaRank.SubAreaCount = subAreaCount;
            for (int i = 1; i <= subAreaCount; i++)
            {
                SubAreaRank subAreaRank = new SubAreaRank();
                string userCountJs = await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRank).Child(currRank + "_" + 1).Child("UserCount").OnceAsJsonAsync();
                int userCount = userCountJs == "null" ? 0 : int.Parse(userCountJs);
                subAreaRank.UserCount = userCount;
                string subAreaJs = await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRank).Child(currRank + "_" + i).OnceAsJsonAsync();
                string pattern = "\"UserCount\":\\s*\\d+(?:,)?";
                subAreaJs = Regex.Replace(subAreaJs, pattern, "");
                Dictionary<string, PVPRankData> dictRanks = JsonConvert.DeserializeObject<Dictionary<string, PVPRankData>>(subAreaJs);
                if(dictRanks != null)
                {
                    subAreaRank.listRanks = dictRanks.Values.ToList();
                    areaRank.listAllRanks.AddRange(dictRanks.Values.ToList());
                }
                areaRank.listSubAreaRanks.Add(subAreaRank);
            }
            return areaRank;
        }
        async Task PushBoard(List<AreaRank> listAreas)
        {
            foreach(var rankName in allRankNames)
            {
                await DBManager.FBClient.Child("PVP").Child("Rankings").Child(rankName).DeleteAsync();
            }
            for(int i = 0; i < listAreas.Count; i++)
            {
                var currRankArea = listAreas[i];
                string currRankName = allRankNames[i];
                await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRankName).Child("SubAreaCount").PutAsync(JsonConvert.SerializeObject(currRankArea.SubAreaCount));
                for (int currSubAreaIndex = 0; currSubAreaIndex < currRankArea.SubAreaCount; currSubAreaIndex++)
                {
                    Dictionary<string, PVPRankData> rankDict = new Dictionary<string, PVPRankData>();
                    foreach(var rank in currRankArea.listSubAreaRanks[currSubAreaIndex].listRanks)
                    {
                        await DBManager.FBClient.Child("PVP").Child("Rankings").Child("RankIndexs").Child(rank.UserID).Child("Path").PutAsync(JsonConvert.SerializeObject($"{currRankName}/{currRankName}_{currSubAreaIndex + 1}"));
                        rankDict.Add(rank.UserID, rank);
                    }
                    string js = JsonConvert.SerializeObject(rankDict);
                    await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRankName).Child(currRankName + "_" + (currSubAreaIndex + 1)).PutAsync(js);
                    await DBManager.FBClient.Child("PVP").Child("Rankings").Child(currRankName).Child(currRankName + "_" + (currSubAreaIndex + 1)).Child("UserCount").PutAsync(JsonConvert.SerializeObject(currRankArea.listSubAreaRanks[currSubAreaIndex].UserCount));
                }
            }
        }
        public void Stop()
        {
            IsRunning = false;
            Program.ShowMenu();
        }
    }
}
