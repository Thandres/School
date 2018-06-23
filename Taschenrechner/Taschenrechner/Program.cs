using System;
using System.Text.RegularExpressions;
namespace Taschenrechner
{
    class Program
    {
        static void Main(string[] args)
        {
            Berechner berechner = new Berechner();
            EingabenParser parser = new EingabenParser(ref berechner);
            userMessage();
            bool quit = false;
            while (!quit)
            {
                String eingabe = Console.ReadLine();
                switch (eingabe)
                {
                    case "q":
                        quit = true;
                        break;
                    case "p":
                        parser.watchHistoryChannel();
                        userMessage();
                        break;
                    default:
                        Console.WriteLine(parser.returnSolution(eingabe));
                        Console.WriteLine("Nächste Rechnung:");
                        break;
                }
             
            }
        }

        private static void userMessage() {
            Console.WriteLine("Bitte geben Sie eine Rechnung ein.");
            Console.WriteLine("Postfixe: Binär=b Hexadezimal=h Octal=o");
            Console.WriteLine("Um alle bisherigen Rechnungen zu sehen geben sie 'p' ein");
            Console.WriteLine("Zum beenden q eingeben");
        }
    }
}
