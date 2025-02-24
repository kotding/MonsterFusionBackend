using Firebase.Database.Query;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.Utils;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MailSender
{
	public static async Task SendRewards(string userId, string title, string shortContent, string content, List<RewardStruct> listReward)
	{
		MailData mailData = new MailData
		{
			canTransleMail = true,
			shortContent = shortContent,
			content = content,
			date = await DateTimeManager.GetUTCAsync(),
			mailId = RandomUtils.RandomId(),
			mailStatus = MailStatus.None,
			mailType = MailType.UserMail,
			title = title,
			listRewards = listReward
		};
		string js = Serializer.SerializeObject(mailData);
		await DBManager.FBClient.Child("Mails").Child(userId).PutAsync(js);
	}
}