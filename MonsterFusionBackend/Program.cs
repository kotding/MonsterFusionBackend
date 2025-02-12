using MonsterFusionBackend.View;
using MonsterFusionBackend.View.MainMenu;
using System;
using System.Collections.Generic;

namespace MonsterFusionBackend
{
    internal class Program
    {
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
            listOptions = new List<IMenuOption>();
            listOptions.Add(new AviatorCleanerOption());
        }
        static void StartAutoRunOption()
        {
            foreach(var option in listOptions)
            {
                if(option.OptionAutoRun)
                {
                    option.Execute();
                }
            }
        }
        static void DrawMenu()
        {
            Console.WriteLine("==========Menu===========\n");
            for(int i = 0; i < listOptions.Count; i++)
            {
                Console.WriteLine(i + ". " + listOptions[i].Name + " " + (listOptions[i].IsRunning? "[running]" : "[stoped]"));
            }
            Console.WriteLine();
        }
        static void ShowMenuSelection()
        {
            int selected = -1;
            while (true)
            {
                DrawMenu();
                Console.Write("Choose your option: ");
                char key = Console.ReadKey().KeyChar;
                if(int.TryParse(key.ToString(),out selected))
                {
                    if(selected >= 0 && selected < listOptions.Count)
                    {
                        break;
                    }
                }
            }
            listOptions[selected].Execute();
        }

        public static void ShowMenu()
        {
            Console.Clear();
            ShowMenuSelection();
        }
    }
}
