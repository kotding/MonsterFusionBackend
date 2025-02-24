using System;

public static class RandomUtils
{
    /// <summary>
    /// Ham nay random 1 so, theo mac dinh tu 1-100, neu random vao <= tham so truyen vao thi return true, nguoc lai false
    /// </summary>
    /// <param name="luckyRange"></param>
    /// <param name="max"> default = 100</param>
    /// <returns></returns>
    public static bool IsLucky(int luckyRange, int max = 100)
    {
        int luckyNumber = Range(1, max + 1);
        return luckyNumber <= luckyRange;
    }
    /// <summary>
    /// Ham nay random 1 so, theo mac dinh tu 1-100, neu random vao <= tham so truyen vao thi return true, nguoc lai false
    /// </summary>
    /// <param name="luckyRange"></param>
    /// <param name="max">default = 100</param>
    /// <returns></returns>
    public static bool IsLucky(float luckyRange, float max = 100)
    {
        float luckyNumber = Range(1, max + 1);
        return luckyNumber <= luckyRange;
    }

    static string alphabet = "abcdefghijklmnouzxy0123456789";
    public static string RandomId(int length = 12)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += alphabet.GetRandom();
        }
        return result;
    }
    static string[] baseNames = {
    "Shadow", "Dragon", "Phoenix", "Knight", "Ninja", "Legend", "Hunter", "Wolf", "Ghost", "Demon",
    "Dark", "Angel", "Reaper", "Storm", "Rogue", "Venom", "Blaze", "Thunder", "Frost", "Inferno",
    "God", "Devil", "Slayer", "Titan", "Warrior", "Sniper", "Zombie", "Beast", "Overlord", "Destroyer",
    "Psycho", "Chaos", "Lucifer", "Joker", "Assassin", "Havoc", "Doom", "Satan", "MadDog", "Hellfire",
    "Savage", "Viper", "Rage", "Anarchy", "Brutal", "Fearless", "Mercenary", "Berserker", "Outlaw", "Rebel"
};

    static string[] vietnameseNames = {
    "Long", "Hùng", "Phúc", "Dũng", "Quân", "Sơn", "Linh", "Hải", "Nam", "Bảo",
    "Phong", "Minh", "Đức", "Anh", "Vinh", "Thịnh", "Tuấn", "Trung", "Khánh", "Khoa",
    "Thành", "Trang", "Miu", "Hân", "Nonn", "Tý", "Đạt", "Toàn", "Lộc", "Tèo", "Tý",
    "Đồng", "Hưng", "Lợn", "Cáo", "Gấu", "Mập", "Bựa", "Xàm", "Đếch", "Cú", "Chó",
    "Mlem", "Trẩu", "Vãi", "Húp", "Dâm", "Kẹo", "Hề", "Nhây","Ngốc","Ebe","Nam","Hacker"
};

    static string[] suffixes = {
    "99", "X", "Pro", "Gamer", "VN", "xX", "King", "Lord", "Master", "_YT", "_TV", "xxx",
    "VIP", "007", "420", "Elite", "Ultra", "Slayer", "GG", "FTW", "Demon", "Ghost",
    "Boss", "Dead", "OP", "Killer", "Omega", "Lover", "Hardcore", "Xtreme", "666", "Noob",
    "TryHard", "Smurf", "Toxic", "D4rk", "1337", "Godly", "Simp", "Trap", "R34", "UwU", "OniiChan","zz","zzz","FF","GG","Hunter",
    "Killer","XXX","Cute","Girl","Bad","Sad","Loly","Chubi","Fat","Oh"
};

    static string[] specialChars = {
    "_", ".", "-", "~", "#","?"
};


    public static string GetRandomName()
    {
        int nameType = Range(0, 10);
        string baseName;

        if (nameType < 7)
        {
            baseName = baseNames.GetRandom();
        }
        else if (nameType < 9)
        {
            baseName = vietnameseNames.GetRandom();
        }
        else
        {
            baseName = baseNames.GetRandom() + vietnameseNames.GetRandom();
        }

        bool hasSpecialChar = Range(0, 4) == 0;
        bool hasSuffix = Range(0, 3) == 0;

        string specialChar = hasSpecialChar ? specialChars.GetRandom() : "";
        string suffix = hasSuffix ? suffixes.GetRandom() : "";

        int specialPosition = Range(0, 3);
        if (specialPosition == 0)
            return specialChar + baseName + suffix;
        else if (specialPosition == 1)
            return baseName.Insert(baseName.Length / 2, specialChar) + suffix;
        else
            return baseName + specialChar + suffix;
    }
    static Random random = new Random();
    public static int Range(int min, int max)
    {
        return random.Next(min, max);
    }
    public static float Range(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }
}


