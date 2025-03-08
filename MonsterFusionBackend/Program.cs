using MonsterFusionBackend.View;
using MonsterFusionBackend.View.MainMenu;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Diagnostics;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using MonsterFusionBackend.View.MainMenu.AviatorCleanerOption;

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
        static void Main(string[] args)
        {
            Init();
            StartAutoRunOption();
            while(true)
            {
                ShowMenuSelection();
            }
        }
        static void Init()
        {
            if (AutoStartup == false) AutoStartup = true;
            listOptions = new List<IMenuOption>();
            listOptions.Add(new AviatorCleanerOptions());
            listOptions.Add(new AviatorResetBoardOption());
            listOptions.Add(new PVPControllerOption());

            listOptions.Sort((a,b) => b.OptionAutoRun.CompareTo(a.OptionAutoRun));
        }
        static void StartAutoRunOption()
        {
            foreach(var option in listOptions)
            {
                if(option.OptionAutoRun)
                {
                    option.Start();
                }
            }
        }
        static void DrawMenu()
        {
            Console.Clear();
            Console.WriteLine("                    __  __                  \r\n                   |  \\/  |                 \r\n                   | \\  / | ___ _ __  _   _ \r\n                   | |\\/| |/ _ \\ '_ \\| | | |\r\n                   | |  | |  __/ | | | |_| |\r\n                   |_|  |_|\\___|_| |_|\\__,_|\r\n                                            \r\n                                            ");
            Console.WriteLine();
            for(int i = 0; i < listOptions.Count; i++)
            {
                Console.WriteLine(i + ". " + listOptions[i].Name + " " + (listOptions[i].IsRunning? "[running]" : "[stopped]"));
            }
            Console.WriteLine();
        }
        static void ShowMenuSelection()
        {
            int selected = -1;
            while (true)
            {
                DrawMenu();
                Console.Write("Choose your option [on/off]: ");
                char key = Console.ReadKey().KeyChar;
                if(int.TryParse(key.ToString(),out selected))
                {
                    if(selected >= 0 && selected < listOptions.Count)
                    {
                        break;
                    }
                }
            }
            if (!listOptions[selected].IsRunning)
            {
                listOptions[selected].Start();
            }
            else
            {
                listOptions[selected].Stop();
            }
        }

        public static void ShowMenu()
        {
            Console.Clear();
            ShowMenuSelection();
        }
    }
}
