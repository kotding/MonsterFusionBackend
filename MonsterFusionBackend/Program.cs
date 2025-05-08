using Microsoft.Win32;
using MonsterFusionBackend.Data;
using MonsterFusionBackend.View;
using MonsterFusionBackend.View.MainMenu;
using MonsterFusionBackend.View.MainMenu.PartyEventOption;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using MonsterFusionBackend.View.MainMenu.SoloBattleOption;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MonsterFusionBackend
{
    internal class Program
    {
        #region REGIST STARTUP
        const string _autoStartKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const string _appName = "MonsterFusionBackend";
        public static bool AutoStartup
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_autoStartKey, false))
                {
                    return key.GetValue(_appName, null) != null;
                }
            }
            set
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_autoStartKey, true))
                {
                    if (value)
                    {
                        key.SetValue(_appName, $"\"{Process.GetCurrentProcess().MainModule.FileName}\"");
                    }
                    else
                    {
                        key.DeleteValue(_appName, false);
                    }
                }
            }
        }
        #endregion

        static List<IMenuOption> listOptions;
        const string url = "test";
        static void Main(string[] args)
        {
            Init();
            StartAllOptions();
            DrawMenu();
            while (true)
            {
                Console.ReadKey();
            }
        }
        static void Init()
        {
            if (AutoStartup == false) AutoStartup = true;
            DBManager.SetFBDatabaseUrl(url);
        }
        static void DrawMenu()
        {
            Console.WriteLine("For " + url);
            for(int i = 0; i < listOptions.Count; i++)
            {
                Console.WriteLine($"{i}. {listOptions[i].Name}");
            }
            Console.WriteLine();
        }
        static void StartAllOptions()
        {
            listOptions = new List<IMenuOption>();

            listOptions.Add(new AviatorCleanerOptions());
            listOptions.Add(new PVPControllerOption());
            listOptions.Add(new PartyEventOption());
            //listOptions.Add(new SoloBattleOption());

            foreach (var  option in listOptions)
            {
                Task task = Task.Run(option.Start);
            }
        }
    }
}
