using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu
{
    internal class AviatorCleanerOptions : IMenuOption
    {
        public string Name => "Aviator Leaderboard Cleaner";


        public async Task Start()
        {
            await ClearErrorDataInLeaderBoard();
        }
        async Task ClearErrorDataInLeaderBoard()
        {
            while(true)
            {
                try
                {
                    DateTime now = await DateTimeManager.GetUTCAsync();
                    long nowLong = long.Parse(now.ToString("yyMMddHHmmss"));
                    string js = await DBManager.FBClient.Child("LeaderBoards").Child("AviatorEvent").OnceAsJsonAsync();
                    List<AvivatorLeaderBoardItemData> listData = JsonConvert.DeserializeObject<Dictionary<string, AvivatorLeaderBoardItemData>>(js).Values.ToList();
                    if (listData != null)
                    {
                        foreach (var item in listData)
                        {
                            if (item.lastRunTime == 0 || item.lastRunTime > nowLong + 120)
                            {
                                await DBManager.FBClient.Child("LeaderBoards").Child("AviatorEvent").Child(item.id).DeleteAsync();
                                LogUtils.LogI($"Removed {item.username}   {item.diamond}   {item.lastRunTime}");
                            }
                        }
                    }
                    await Task.Delay(1000 * 60 * 60);
                }
                catch (Exception ex)
                {
                    LogUtils.LogI(ex.Message);
                    LogUtils.LogI(ex.StackTrace);
                }
            }
        }
    }
}
