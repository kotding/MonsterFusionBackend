using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View.MainMenu.PVPControllerOption
{
    
    internal class RankRewardSender
    {
        #region DATA
        static List<RankRewardPack> listRewardPack = new List<RankRewardPack>
        {
            new RankRewardPack
            {
                rankType = RankType.COPPER,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 5 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 5 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 5 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 5 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 5 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1000 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 800 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 500 }
                    }} // Top 50-2000
                }
            },
            new RankRewardPack
            {
                rankType = RankType.SILVER,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 2000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 2000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 6 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 2000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1500 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1000 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 800 }
                    }} // Top 50-2000
                }
            },
            new RankRewardPack
            {
                rankType = RankType.GOLD,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 3000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 2 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 3000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 3000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 6 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 3000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 6 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 2000 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 1500 }
                    }} // Top 50-2000
                }
            },
            new RankRewardPack
            {
                rankType = RankType.PLATINUM,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 5000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 3 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 5000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 5000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 5000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 3000 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 2000 }
                    }} // Top 50-2000
                }
            },
            new RankRewardPack
            {
                rankType = RankType.DIAMOND,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 15 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 15 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 4 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 2 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 7000 }
                    }} // Top 50-2000
                }
            },
            new RankRewardPack
            {
                rankType = RankType.ULTIMATE,
                listRewards = new List<RankReward>
                {
                    new RankReward { top = 1, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 20 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 20 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 5 }
                    }},
                    new RankReward { top = 2, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 15 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 15 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 3 }
                    }},
                    new RankReward { top = 3, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 10 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 2 }
                    }},
                    new RankReward { top = 10, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.LEGEND_EGG, NumberReward = 1 }
                    }}, // Top 4-10
                    new RankReward { top = 50, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 8 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RESET_STONE, NumberReward = 8 }
                    }}, // Top 11-50
                    new RankReward { top = 2000, rewards = new List<RewardData>
                    {
                        new RewardData { REWARD_TYPE = REWARD_TYPE.DIAMOND, NumberReward = 10000 },
                        new RewardData { REWARD_TYPE = REWARD_TYPE.RARE_FUSION_BOTTLE, NumberReward = 5 }
                    }} // Top 50-2000
                }
            }
        };
        #endregion
        public static async Task SendRewardTo(string userId, RankType rankType, int top)
        {
            if (top >= 2000) return;
            List<RewardData> rewards = GetReward(rankType,top);
            if (rewards == null) return;
            List<RewardStruct> listRwStructs = new List<RewardStruct>();
            foreach(var rw in  rewards)
            {
                RewardStruct rwStruct = new RewardStruct
                {
                    REWARD_TYPE = rw.REWARD_TYPE,
                    NumberReward = rw.NumberReward,
                };
                listRwStructs.Add(rwStruct);
            }
            string content = $"You have successfully reached the {top+1}st rank in the leaderboard and received a reward.\r\n\r\nTry to maintain this form in the next week.";
            await MailSender.SendRewards(userId, "Ranking rewards", "Congratulations on receiving your ranking reward last week!", content,listRwStructs);
        }
        static List<RewardData> GetReward(RankType rankType, int top)
        {
            Console.WriteLine(rankType + " " + top);
            RankRewardPack pack = listRewardPack.Find(x => x.rankType == rankType);
            if(pack == null) return null;
            RankReward rankRw = pack.listRewards.FindLast(x => x.top <= top + 1);
            if(rankRw == null) return null;
            return rankRw.rewards;
        }
    }
}
