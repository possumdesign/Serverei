using System;
using System.Collections.Generic;
using System.Text;

namespace ServerRackSimulator
{
    internal class AppData
    {
        public List<User> Users = new List<User>();
        public List<BaseServer> ServerTemplates = new List<BaseServer>(); // "b" (Admin) + "c" (Custom)
        public List<RackConfig> Configurations = new List<RackConfig>();
        public CostSettings Costs = CostSettings.CreateDefaults();
    }
    public class Robco
    {

        public static void RobcoBG()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            //debugMSG
            // Console.WriteLine("ROB-CO INDUSTRIES (TM) SYSTEMS ONLINE");
        }
        public static void RobcoType(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
        public static void Menu(List<string> options)

        {
            Console.OutputEncoding = Encoding.UTF8;
            // Robco.RobcoBG();       //warscheinlich unnötig hier
            int auswahl = 0;
           /* List<string> options = new List<string>()
        {
            "[1] Eingeloggter User anzeigen/wechseln (Login)",
            "[2] User Auswahl (3 Slots verwalten)",
            "[3] Größenauswahl des Racks (9/15/24/42 HE)",
            "[4] Erstellen Custom Server (1-4 HE)",
            "[5] Konfigurierte Racks (speichern/laden/löschen)",
            "[6] Einstellungen (Namen & Vorlagen & Kosten)",
            "[0] Beenden",
        };   */
            while (true)
            {

                Console.Clear();
                Console.WriteLine("\n================");
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == auswahl)
                    {

                        Console.Write(">> ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(options[i]);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" <<");


                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
                }
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow) auswahl--;
                else if (key == ConsoleKey.DownArrow) auswahl++;
                if (auswahl < 0) auswahl = options.Count - 1;
                if (auswahl >= options.Count) auswahl = 0;
                if (key == ConsoleKey.Enter)
                {
                    if (auswahl == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("1 ");


                    }
                    else if (auswahl == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("2 ");

                    }
                    else if (auswahl == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("3 ");

                    }
                    else if (auswahl == 3)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("4");

                    }
                    else if (auswahl == 4)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("5");

                    }
                    else if (auswahl == 5)
                    {
                        Console.Clear();
                        Console.WriteLine("\n================");
                        Console.WriteLine("6");

                    }
                    auswahl += 1;
                    Console.WriteLine(auswahl + " gewählt.");
                    Console.ReadKey();

                }
            }
        }
    }
}