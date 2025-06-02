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
		if (string.IsNullOrEmpty(userId))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Null user id when send reward " + userId);
			Console.ResetColor();
			return;
		}
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
		try
		{
            await DBManager.FBClient.Child("Mails").Child("User_Mails").Child(userId).Child(mailData.mailId).PutAsync(js);
        }catch(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
			Console.ResetColor();
		}

    }
}