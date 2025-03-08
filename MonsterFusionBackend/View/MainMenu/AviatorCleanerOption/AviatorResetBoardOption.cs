using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using System;

namespace MonsterFusionBackend.View.MainMenu.AviatorCleanerOption
{
    internal class AviatorResetBoardOption : IMenuOption
    {
        public string Name => "Reset aviator board";

        public bool OptionAutoRun => false;

        public bool IsRunning { get; set; }

        public void Start()
        {
            DBManager.FBClient.Child("LeaderBoards").Child("AviatorEvent").DeleteAsync();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
