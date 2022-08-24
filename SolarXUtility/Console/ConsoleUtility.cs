
using System;

namespace SolarX.Utility.Console
{
    public static class ConsoleUtility
    {
        public static void WaitForUserAknowledge()
        {
            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
        }

        public static int PrintErrorMessage(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
            return -1;
        }

        public static int PrintSuccessMessage(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Green;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
            return 1;
        }

        public static void PrintWarningMessage(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
        }

        public static void PrintColouredText(string text)
        {
            ColoredText cText = ColoredText.From(text);
            foreach ((ConsoleColor color, string text) pair in cText.NextLine())
            {
                System.Console.ForegroundColor = pair.color;
                System.Console.Write(pair.text);
            }
            System.Console.ResetColor();
        }
    }
}
