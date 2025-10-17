using server;
internal class Program
{
    
    public static void Main(string[] args)
    {
    //MUSSWEG    SpeichernLaden.Laden(ref tagebuch, ref todos, ref einkauf);
    //MUSSWEG    UhrThread = new Thread(Uhr.AnzeigeUhr);
        Menu();
    }
    public static void Menu()
    {
        int auswahl = 0;
        List<string> options = new List<string>()
        {
            "1. Tagebucheintrag erstellen",
            "2. To-Do-Eintrag erstellen",
            "3. Einkaufsliste erstellen",
            "4. Tagebucheinträge anzeigen",
            "5. To-Do-Einträge anzeigen",
            "6. Einkaufslisten anzeigen",
        };
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n\n\n\n");
            for (int i = 0; i < options.Count; i++)
            {
                if(i == auswahl)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(options[i]);
                }
            }
            if(!UhrThread.IsAlive) UhrThread.Start();

            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.UpArrow) auswahl--;
            else if (key == ConsoleKey.DownArrow) auswahl++;
            if (auswahl < 0) auswahl = options.Count - 1;
            if (auswahl >= options.Count) auswahl = 0;
            if(key == ConsoleKey.Enter)
            {
                if(auswahl == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("Geben Sie den Titel des Tagebucheintrags ein: ");
                    string titel = Console.ReadLine() ?? "";
                    if(titel.Trim() == "") titel = "Ohne Titel";
                    var tagebucheintrag = new TagebuchEintrag(titel);
                    tagebuch.Add(tagebucheintrag);
                    SpeichernLaden.Speichern(tagebuch, todos, einkauf);

                }
                else if(auswahl == 1)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("Geben Sie denTitel des To-Do eintrags ein: ");
                    string titel = Console.ReadLine() ?? "";
                    if (titel.Trim() == "") titel = "Ohne Titel";

                    int auswahl2 = 0;
                    List<string> options2 = new List<string>()
                    {
                        "1. Mit Erinnerung",
                        "2. Ohne Erinnerung"
                    };
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("\n\n\n\n");
                        Console.WriteLine("Soll eine Erinnerung gesetzt werden? ");
                        for (int i = 0; i < options2.Count; i++)
                        {
                            if (i == auswahl2)
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine(options2[i]);
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine(options2[i]);
                            }
                        }
                        var key2 = Console.ReadKey().Key;
                        if (key2 == ConsoleKey.UpArrow) auswahl2--;
                        else if (key2 == ConsoleKey.DownArrow) auswahl2++;
                        if (auswahl2 < 0) auswahl2 = options2.Count - 1;
                        if (auswahl2 >= options2.Count) auswahl2 = 0;
                        if (key2 == ConsoleKey.Enter) break;
                    }
                    if(auswahl2 == 0)
                    {
                        var todoeintrag = new ToDoEintrag(titel, ToDoEintrag.eingabeErinnerungsdatum());
                        todos.Add(todoeintrag);

                    }
                    else
                    {
                        var todoeintrag = new ToDoEintrag(titel);
                        todos.Add(todoeintrag);
                    }
                    SpeichernLaden.Speichern(tagebuch, todos, einkauf);
                }
                else if(auswahl == 2)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("Geben Sie den Titel der Einkaufsliste ein: ");
                    string titel = Console.ReadLine() ?? "";
                    if (titel.Trim() == "") titel = "Ohne Titel";
                    var liste = new Einkaufsliste(titel);
                    einkauf.Add(liste);
                    SpeichernLaden.Speichern(tagebuch, todos, einkauf);
                }
                else if(auswahl == 3)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("Tagebucheinträge\n\n");
                    foreach (var eintrag in tagebuch)
                    {
                        eintrag.Anzeigen();
                    }
                    Console.ReadKey();
                }
                else if (auswahl == 4)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("To-Do Notizen\n\n");
                    foreach (var eintrag in todos)
                    {
                        eintrag.Anzeigen();
                    }
                    Console.ReadKey();
                }
                else if (auswahl == 5)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n");
                    Console.WriteLine("Einkaufslisten\n\n");
                    foreach (var eintrag in einkauf)
                    {
                        eintrag.Anzeigen();
                    }
                    Console.ReadKey();
                }


            }
        }
    }
}