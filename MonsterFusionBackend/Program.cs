
using MonsterFusionBackend.View;
using MonsterFusionBackend.View.MainMenu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonsterFusionBackend
{
    internal class Program
    {
        static List<IMenuOption> listOptions;
        static void Main(string[] args)
        {
            Init();
            StartMenu();
        }
        static void Init()
        {
            listOptions = new List<IMenuOption>();
            listOptions.Add(new AviatorCleanerOption());
        }
        static void DrawMenu()
        {
            Console.WriteLine("==========Menu===========\n");
            for(int i = 0; i < listOptions.Count; i++)
            {
                Console.WriteLine(i + ". " + listOptions[i].Name);
            }
            Console.WriteLine();
        }
        static void StartMenu()
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
            listOptions[selected].Execute().Wait();
        }

        public static void ShowMenu()
        {
            StartMenu();
        }
    }
}
