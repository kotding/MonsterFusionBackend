using MonsterFusionBackend.View;
using MonsterFusionBackend.View.MainMenu;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Diagnostics;
using MonsterFusionBackend.View.MainMenu.PVPControllerOption;
using MonsterFusionBackend.View.MainMenu.AviatorCleanerOption;
using MonsterFusionBackend.View.MainMenu.PartyEventOption;
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
        static void Main(string[] args)
        {
            Init();
            while(true)
            {
                WaitUserSelectOption();
            }
        }
        static void Init()
        {
            if (AutoStartup == false) AutoStartup = true;
            listOptions = new List<IMenuOption>();
            listOptions.Add(new AviatorCleanerOptions());
            listOptions.Add(new AviatorResetBoardOption());
            listOptions.Add(new PVPControllerOption());
            listOptions.Add(new PartyEventOption());
        }
        static void DrawMenu()
        {
            Console.Clear();
            for(int i = 0; i < listOptions.Count; i++)
            {
                Console.WriteLine(i + ". " + listOptions[i].Name + " " + (listOptions[i].IsRunning? "[running]" : "[stopped]"));
            }
            Console.WriteLine();
        }
        static void WaitUserSelectOption()
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
                        if (!listOptions[selected].IsRunning)
                        {
                            Task.Run(listOptions[selected].Start);
                            DrawMenu();
                        }
                    }
                }
            }

        }

        public static void ShowMenu()
        {
            Console.Clear();
            WaitUserSelectOption();
        }
    }
}
