using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu
{
    [Serializable]
    public class AvivatorLeaderBoardItemData
    {
        public string id;
        public string username;
        public string urlImage;
        public float mul;
        public int diamond;
        public int lastDiamond;
        public long lastRunTime;
        public LeaderBoardUpdateType LeaderBoardUpdateType;
    }
    [Serializable]
    public enum LeaderBoardUpdateType
    {
        UploadNew,
        UpdateTime,
        UpdateScore
    }
}
