using System;
using System.Threading;

namespace MonsterFusionBackend.View
{
    public class AnimatedText
    {
        public static void WriteLineAnimated(string text, int delay = 50)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        public static void WriteBounceText(string text, int bounceHeight = 1, int delay = 50)
        {
            int originalTop = Console.CursorTop;
            
            for (int i = 0; i < text.Length; i++)
            {
                // Bounce up
                Console.SetCursorPosition(Console.CursorLeft, originalTop - bounceHeight);
                Console.Write(text[i]);
                Thread.Sleep(delay);

                // Bounce down
                Console.SetCursorPosition(Console.CursorLeft - 1, originalTop);
                Console.Write(text[i]);
                Thread.Sleep(delay/2);
            }
            Console.WriteLine();
        }
    }
} 