using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu
{
    internal class AviatorCleanerOption : IMenuOption
    {
        public string Name => "Aviator Leaderboard Cleaner";

        public bool OptionAutoRun => true;

        public bool IsRunning { get; set; }

        public async Task Execute()
        {
            await ClearErrorDataInLeaderBoard();
            Console.ReadKey();
            Kill();
        }
        async Task ClearErrorDataInLeaderBoard()
        {
            IsRunning = true;
            while(IsRunning)
            {
                DateTime now = await DateTimeManager.GetUTCAsync();
                long nowLong = long.Parse(now.ToString("yyMMddHHmmss"));
                string js = await DBManager.FBClient.Child("LeaderBoards").Child("AviatorEvent").OnceAsJsonAsync();
                List<AvivatorLeaderBoardItemData> listData = JsonConvert.DeserializeObject<Dictionary<string, AvivatorLeaderBoardItemData>>(js).Values.ToList();
                if (listData == null)
                {
                }
                else
                {
                    int count = 0;
                    foreach (var item in listData)
                    {
                        if (count >= 100) break;
                        if (item.lastRunTime == 0 || item.lastRunTime > nowLong + 120)
                        {
                            Console.WriteLine("Deleting " + item.username + "  " + item.lastRunTime);
                            await DBManager.FBClient.Child("LeaderBoards").Child("AviatorEvent").Child(item.id).DeleteAsync();
                            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/AviatorRemovedLogText.txt";
                            if(!File.Exists(filePath))
                            {
                                using (File.Create(filePath)) ;
                            }
                            File.AppendAllText(filePath, $"Removed {item.username}   {item.diamond}   {item.lastRunTime} - {nowLong} \n");
                            count++;
                        }
                    }
                }
                await Task.Delay(1000 * 120);
            }
        }
        public void Kill()
        {
            IsRunning = false;
            Program.ShowMenu();
        }
    }
}
