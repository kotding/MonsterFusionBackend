using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PVPControllerOption
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RankType
    {
        COPPER,
        SILVER,
        GOLD,
        PLATINUM,
        DIAMOND,
        ULTIMATE,
        NONE
    }
    public class PVPRankData
    {
        public string UserID;
        public int RankPoint;
        public string UserName;
        public string AvatarUrl;
        public int Power;
        public RankType RankType;
        public int RankIndex;
    }
    public class SubAreaRank
    {
        public int UserCount = 0;
        public List<PVPRankData> listRanks = new List<PVPRankData>();
    }
    public class AreaRank
    {
        public int SubAreaCount;
        public List<SubAreaRank> listSubAreaRanks = new List<SubAreaRank>();
        public List<PVPRankData> listAllRanks = new List<PVPRankData>();
    }
    public interface IRewardStruct
    {
        REWARD_TYPE GetRewardType();
        int GetRewardAmount();
    }
    [Serializable]
    public struct RewardStruct : IRewardStruct
    {
        public REWARD_TYPE REWARD_TYPE;
        public int NumberReward;
        public RewardStruct(REWARD_TYPE type, int num)
        {
            this.REWARD_TYPE = type;
            this.NumberReward = num;
        }
        public RewardStruct(RewardStruct DataConfig)
        {
            this.REWARD_TYPE = DataConfig.REWARD_TYPE;
            this.NumberReward = DataConfig.NumberReward;
        }

        public int GetRewardAmount()
        {
            return NumberReward;
        }

        public REWARD_TYPE GetRewardType()
        {
            return REWARD_TYPE;
        }
    }
    public enum MailType
    {
        GlobalMail,
        UserMail
    }
    public enum MailStatus
    {
        None,
        Viewed,
        Claimed
    }
    [Serializable]
    public class MailData
    {
        public string mailId;
        public MailStatus mailStatus;
        public string title;
        public string shortContent;
        public string content;
        public DateTime date;
        public bool canTransleMail;
        public List<RewardStruct> listRewards;
        [JsonIgnore] public MailType mailType = MailType.UserMail;
    }
    [Serializable]
    public enum REWARD_TYPE
    {
        DIAMOND = 0,
        GOLD = 1,
        STAR = 2,
        MONSTER = 4,
        RESET_STONE = 7,
        ARTIFACT = 8,
        ARTIFACT_NORMAL = 9,
        ARTIFACT_UNCOMMON = 10,
        ARTIFACT_RARE = 11,
        ARTIFACT_EPIC = 12,
        ARTIFACT_LEGEND = 13,
        ARTIFACT_DIVINE = 14,



        GOLD_LEVEL = 1001,

        NORMAL_FUSION_BOTTLE = 1003,
        RARE_FUSION_BOTTLE = 1004,
        NORMAL_EGG = 1002,
        RARE_EGG = 1009,
        ELEMENTAL_EGG = 1010,
        LEGEND_EGG = 1005,

        STAR_FUSION_EVENT = 1006,
        DICE = 1007,
        REMOVE_ADS = 1008,

        GALAXY_EGG = 1011,
        STAR_GALAXY_EVENT = 1012,
        CHRISTMAS_TICKET = 1013,
        CHRISTMAS_BELL = 1014,
        MUTANT_BOTTLE = 1015,

        HAMMER = 1016,
        CHRISTMAS_7Days_X2 = 1017,
        CHRISTMAS_7Days_X4 = 1018,
        VIP_PACK = 1019,
        CHALLENGE_TICKET = 1020,
        FISH_BAIT = 1021,
        AVIATOR_STONE = 1022,
        CHALLENGE_STONE = 1023,
        PURCHASE_PACK
    }
    public class RewardData
    {
        public REWARD_TYPE REWARD_TYPE;
        public int NumberReward;
    }
    public class RankReward
    {
        public int top;
        public List<RewardData> rewards;
    }
    public class RankRewardPack
    {
        public RankType rankType;
        public List<RankReward> listRewards;
    }
}
