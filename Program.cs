using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
// ServerRackSimulator
// Aufgeteilt in mehrere Dateien (+ Kostenmodell, sort of...)

namespace ServerRackSimulator
{
    internal class Program
    {
        private const string DataFile = "data.json"; //immer im Debug Dir, wo das Programm läuft. fool me once...

        private static AppData _app = new AppData();
        private static User _currentUser = null; //Ohne gehts nicht
        private static Rack _currentRack = null; //Ohne gehts nicht
        private static int _nextPlacedId = 1;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;


            Robco.RobcoBG();


            Console.Clear();
            Robco.RobcoType("ROB-CO INDUSTRIES (TM) SYSTEMS ONLINE", 15); // eigene classe für den "Robcotype" Effekt, interchangeable mit normalen Console.Writeline
            Robco.RobcoType("Initialisiere...", 25);
            Robco.RobcoType("ZAX Mainframe.................Verbunden", 30);



            LoadOrInit();

            Action_Login();

            

            bool check = true;
            while (check == true)
            {
                ShowHeader();
                ShowMainMenu();

                Console.Write("Auswahl: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": Action_Login();
                        break;                              // Eingeloggter User / Login
                    case "2": Action_UserSelect();
                        break;                              // User Auswahl / Umbenennen
                    case "3": Action_RackSelect();
                        break;                              // Rackgrößen
                    case "4": Action_CreateCustomServer();
                        break;                              // Custom 1-4HE (c)
                    case "5": Action_ConfigRacksMenu();
                        break;                              // Konfigurationen verwalten
                    case "6": Action_Settings();
                        break;                              // Einstellungen inkl. Kosten
                    case "0": check = false;
                        break;                              // Beenden
                    default: Message("Ungültige Auswahl.");
                        break;
                }
            }

            Save();
            Robco.RobcoType("Programm beendet.");
        }

        //Persistenz 0.2
        private static void LoadOrInit()
        {
            if (File.Exists(DataFile))
            {
                try
                {
                    string json = File.ReadAllText(DataFile, Encoding.UTF8);
                    var options = new JsonSerializerOptions { IncludeFields = true };
                    _app = JsonSerializer.Deserialize<AppData>(json, options);
                    if (_app == null) _app = new AppData();
                    ReconstructAfterLoad();
                }
                catch
                {
                    _app = new AppData();
                    InitDefaults();
                    Save();
                }
            }
            else
            {
                _app = new AppData();
                InitDefaults();
                Save();
            }
        }

        private static void InitDefaults()
        {
            if (_app.Users == null) _app.Users = new List<User>();
            if (_app.ServerTemplates == null) _app.ServerTemplates = new List<BaseServer>();
            if (_app.Configurations == null) _app.Configurations = new List<RackConfig>();
            if (_app.Costs == null) _app.Costs = CostSettings.CreateDefaults();

            if (_app.Users.Count == 0)
            {
                _app.Users.Add(new User("Admin", Role.Admin));
                _app.Users.Add(new User("User1", Role.User));
                _app.Users.Add(new User("User2", Role.User));
                _app.Users.Add(new User("User3", Role.User));
            }

            if (_app.ServerTemplates.Count == 0)
            {
                // Base (b) Vorlagen mit Lüfteranzahl, Netzteilen, SSD, HDD, GPU

                _app.ServerTemplates.Add(new He1("1HE Router", 40, 4, 1, 1, 0, 0, "b"));
                _app.ServerTemplates.Add(new He2("2HE Compute", 60, 6, 2, 2, 4, 1, "b"));
                _app.ServerTemplates.Add(new He3("3HE Storage", 80, 8, 2, 1, 12, 0, "b"));
                _app.ServerTemplates.Add(new He4("4HE Data Storage", 120, 8, 2, 2, 8, 0, "b"));
            }
        }

        private static void ReconstructAfterLoad()
        {
            if (_app.Users == null) _app.Users = new List<User>();
            if (_app.ServerTemplates == null) _app.ServerTemplates = new List<BaseServer>();
            if (_app.Configurations == null) _app.Configurations = new List<RackConfig>();
            if (_app.Costs == null) _app.Costs = CostSettings.CreateDefaults();
        }

        private static void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            string json = JsonSerializer.Serialize(_app, options);
            File.WriteAllText(DataFile, json, Encoding.UTF8);
        }

        // GUI, sort of TODO
        private static void ShowHeader()
        {
            Console.Clear();
            Console.WriteLine("===============================================");
            Robco.RobcoType("        SERVER RACK SIMULATOR....Online ");
            Console.WriteLine("===============================================");
            Console.WriteLine("Eingeloggt: {0}", _currentUser != null ? _currentUser.Name + " (" + _currentUser.Role + ")" : "—");
            Console.WriteLine("Aktives Rack: {0}", _currentRack != null ? _currentRack.Name : "—");
            Console.WriteLine();
        }

        private static void ShowMainMenu() //Menü alt , v0.2
        {
            Console.WriteLine("[1] Eingeloggter User anzeigen/wechseln (Login)");
            Console.WriteLine("[2] User Auswahl (3 Slots verwalten)");
            Console.WriteLine("[3] Größenauswahl des Racks (9/15/24/42 HE)");
            Console.WriteLine("[4] Erstellen Custom Server (1-4 HE)");
            Console.WriteLine("[5] Konfigurierte Racks (speichern/laden/löschen)");
            Console.WriteLine("[6] Einstellungen (Namen & Vorlagen & Kosten)");
            Console.WriteLine("[0] Beenden");
            Console.WriteLine();
        }

        private static void Message(string msg)
        {
            Console.WriteLine("> " + msg);
            Console.WriteLine("(Weiter mit Enter)");
            Console.ReadLine();
        }

        //Aktionen 0.2
        private static void Action_Login()
        {
            Console.Clear();
            Console.WriteLine("LOGIN");
            Console.WriteLine("-----");

            for (int i = 0; i < _app.Users.Count; i++)
            {
                Console.WriteLine("[{0}] {1} ({2})", i + 1, _app.Users[i].Name, _app.Users[i].Role);
            }
            Console.Write("Auswahl (Zahl): ");
            string s = Console.ReadLine();
            int idx;

            if (int.TryParse(s, out idx))
            {
                idx = idx - 1;
                if (idx >= 0 && idx < _app.Users.Count)
                {
                    _currentUser = _app.Users[idx];
                    Message("Eingeloggt als: " + _currentUser.Name + " (" + _currentUser.Role + ")");
                    return;
                }
            }
            Message("Login fehlgeschlagen.");
        }

        private static void Action_UserSelect()
        {
            Console.Clear();
            Console.WriteLine("USER AUSWAHL / VERWALTUNG");
            Console.WriteLine("--------------------------");
            Console.WriteLine("[1] Nutzer umbenennen (User1-3)");
            Console.WriteLine("[0] Zurück");
            Console.Write("Auswahl: ");
            string a = Console.ReadLine();
            if (a == "1") RenameUserSlots();
        }

        private static void RenameUserSlots()
        {
            for (int i = 0; i < _app.Users.Count; i++)
            {
                var u = _app.Users[i];
                if (u.Role == Role.User)
                {
                    Console.Write("Neuer Name für {0} (Enter für unverändert): ", u.Name);
                    string neu = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(neu)) u.Name = neu.Trim();
                }
            }
            Save();
            Message("Usernamen aktualisiert.");
        }

        private static void Action_RackSelect()
        {
            Console.Clear();
            Console.WriteLine("RACK-AUSWAHL");
            Console.WriteLine("-----------");
            Console.WriteLine("[1]  9 HE");
            Console.WriteLine("[2] 15 HE");
            Console.WriteLine("[3] 24 HE");
            Console.WriteLine("[4] 42 HE");
            Console.Write("Auswahl: ");
            string s = Console.ReadLine();
            int choice; int height = 0;
            if (int.TryParse(s, out choice))
            {
                if (choice == 1) height = 9;
                else if (choice == 2) height = 15;
                else if (choice == 3) height = 24;
                else if (choice == 4) height = 42;
            }
            if (height == 0) { Message("Ungültige Auswahl."); return; }

            _currentRack = new Rack("Rack " + height + "HE", height);
            _nextPlacedId = 1;
            RackLoop();
        }

        private static void RackLoop()
        {
            bool inRack = true;
            while (inRack)
            {
                Console.Clear();
                ShowHeader();
                _currentRack.Render();
                Console.WriteLine();
                Console.WriteLine("[1] Einschub hinzufügen");
                Console.WriteLine("[2] Einschub löschen (per Nummer)");
                Console.WriteLine("[3] Konfiguration speichern");
                Console.WriteLine("[4] Kosten anzeigen (Rack & Einzel)");
                Console.WriteLine("[5] Ausfall simulieren (Ersatzteilkosten)");
                Console.WriteLine("[0] Rack verlassen");
                Console.Write("Auswahl: ");
                string inp = Console.ReadLine();

                if (inp == "1") Action_AddServerToRack();
                else if (inp == "2") Action_RemoveFromRack();
                else if (inp == "3") Action_SaveCurrentRackConfig();
                else if (inp == "4") Action_ShowCosts();
                else if (inp == "5") Action_SimulateFailure();
                else if (inp == "0") inRack = false;
                else Message("Ungültige Auswahl.");
            }
        }

        private static void Action_AddServerToRack()
        {
            if (_app.ServerTemplates.Count == 0) { Message("Keine Servervorlagen vorhanden."); return; }

            Console.Clear();
            Console.WriteLine("EINSCHUB HINZUFÜGEN");
            Console.WriteLine("--------------------");
            for (int i = 0; i < _app.ServerTemplates.Count; i++)
            {
                var s = _app.ServerTemplates[i];
                Console.WriteLine("[{0}] {1} | {2}HE, fan{3}mm:{4}, NT:{5}, SSD:{6}, HDD:{7}, GPU:{8} ({9})",
                    i + 1, s.Typ, s.HeightU, s.FanMm, s.FanCount, s.Netzteile, s.SSD, s.HDD, s.GPU, s.Origin);
            }
            Console.Write("Auswahl (Zahl): ");
            string a = Console.ReadLine();
            int idx; if (!int.TryParse(a, out idx)) { Message("Abbruch."); return; }
            idx = idx - 1; if (idx < 0 || idx >= _app.ServerTemplates.Count) { Message("Ungültig."); return; }

            var template = _app.ServerTemplates[idx];
            BaseServer server = template.Clone();

            int startPos = _currentRack.FindFirstFit(server.HeightU);
            if (startPos == -1) { Message("Kein Platz verfügbar für " + server.HeightU + "HE."); return; }

            var placed = new PlacedItem(_nextPlacedId, startPos, server);
            _nextPlacedId++;
            bool ok = _currentRack.Place(placed);
            if (ok) Message("Einschub hinzugefügt (#" + placed.PlacedId + ") an Position " + startPos + ".");
            else Message("Einfügen fehlgeschlagen.");
        }

        private static void Action_RemoveFromRack()
        {
            Console.Write("Lösch-ID eingeben: ");
            string s = Console.ReadLine();
            int id; if (!int.TryParse(s, out id)) { Message("Abbruch."); return; }
            bool ok = _currentRack.RemoveByPlacedId(id);
            if (ok) Message("Einschub #" + id + " entfernt."); else Message("Nicht gefunden.");
        }

        private static void Action_SaveCurrentRackConfig()
        {
            if (_currentRack == null) { Message("Kein aktives Rack."); return; }
            Console.Write("Konfigurationsname: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Message("Abbruch."); return; }

            RackConfig cfg = new RackConfig();
            cfg.Name = name.Trim();
            cfg.TotalU = _currentRack.TotalU;
            cfg.Items = new List<PlacedItemDTO>();

            for (int i = 0; i < _currentRack.Items.Count; i++)
            {
                var it = _currentRack.Items[i];
                var dto = new PlacedItemDTO();
                dto.PlacedId = it.PlacedId;
                dto.StartU = it.StartU;
                dto.Server = BaseServerDTO.FromServer(it.Server);
                cfg.Items.Add(dto);
            }

            int idx = FindConfigIndexByName(cfg.Name);
            if (idx != -1) _app.Configurations[idx] = cfg; else _app.Configurations.Add(cfg);
            Save();
            Message("Konfiguration gespeichert: " + cfg.Name);
        }

        private static int FindConfigIndexByName(string name)
        {
            for (int i = 0; i < _app.Configurations.Count; i++)
            {
                if (string.Equals(_app.Configurations[i].Name, name, StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        private static void Action_ConfigRacksMenu()
        {
            Console.Clear();
            Console.WriteLine("KONFIGURIERTE RACKS");
            Console.WriteLine("-------------------");
            if (_app.Configurations.Count == 0) { Message("Keine gespeicherten Konfigurationen."); return; }

            for (int i = 0; i < _app.Configurations.Count; i++)
            {
                var c = _app.Configurations[i];
                Console.WriteLine("[{0}] {1} ({2}HE, {3} Elemente)", i + 1, c.Name, c.TotalU, c.Items != null ? c.Items.Count : 0);
            }
            Console.WriteLine("[L] Laden  [D] Löschen  [Enter] Zurück");
            Console.Write("Auswahl: ");
            string a = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(a)) return;

            if (a.Equals("L", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Index zum Laden: ");
                string s = Console.ReadLine();
                int idx; if (!int.TryParse(s, out idx)) { Message("Abbruch."); return; }
                idx = idx - 1; if (idx < 0 || idx >= _app.Configurations.Count) { Message("Ungültig."); return; }
                LoadConfigIntoRack(_app.Configurations[idx]);
                RackLoop();
            }
            else if (a.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Index zum Löschen: ");
                string s = Console.ReadLine();
                int idx; if (!int.TryParse(s, out idx)) { Message("Abbruch."); return; }
                idx = idx - 1; if (idx < 0 || idx >= _app.Configurations.Count) { Message("Ungültig."); return; }
                var name = _app.Configurations[idx].Name;
                _app.Configurations.RemoveAt(idx);
                Save();
                Message("Konfiguration gelöscht: " + name);
            }
        }

        private static void LoadConfigIntoRack(RackConfig cfg)
        {
            _currentRack = new Rack(cfg.Name + " (geladen)", cfg.TotalU);
            _nextPlacedId = 1;
            if (cfg.Items != null)
            {
                for (int i = 0; i < cfg.Items.Count; i++)
                {
                    var dto = cfg.Items[i];
                    BaseServer server = dto.Server.ToServer();
                    var placed = new PlacedItem(_nextPlacedId, dto.StartU, server);
                    bool ok = _currentRack.Place(placed);
                    if (ok) _nextPlacedId++;
                }
            }
        }

        private static void Action_CreateCustomServer()
        {
            Console.Clear();
            Console.WriteLine("CUSTOM SERVER ERSTELLEN");
            Console.WriteLine("-----------------------");
            if (_currentUser == null) { Message("Bitte zuerst einloggen."); return; }

            Console.Write("Höhe (1-4 HE): ");
            string hs = Console.ReadLine();
            int h; if (!int.TryParse(hs, out h)) { Message("Abbruch."); return; }
            if (h < 1 || h > 4) { Message("Nur 1-4 HE erlaubt."); return; }

            Console.Write("Typ/Bezeichnung: ");
            string typ = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(typ)) typ = "Custom";

            int fanMm = (h == 1) ? 40 : (h == 2) ? 60 : (h == 3) ? 80 : 120;
            int fanCount = AskInt("Lüfteranzahl (4-8): ", 4, 8);
            int nt = AskInt("Netzteile (1-3): ", 1, 3);
            int ssd = AskInt("SSD (0-4): ", 0, 4);
            int hdd = AskInt("HDD (0-24): ", 0, 24);
            int gpu = AskInt("GPU (0-8): ", 0, 8);

            BaseServer s;
            if (h == 1) s = new He1(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "c");
            else if (h == 2) s = new He2(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "c");
            else if (h == 3) s = new He3(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "c");
            else s = new He4(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "c");

            _app.ServerTemplates.Add(s);
            Save();
            Message("Custom-Server gespeichert (" + s.Typ + ", " + s.HeightU + "HE).");
        }

        private static int AskInt(string label, int min, int max)
        {
            int val;
            while (true)
            {
                Console.Write(label);
                string s = Console.ReadLine();
                if (int.TryParse(s, out val))
                {
                    if (val >= min && val <= max) return val;
                }
                Console.WriteLine("Bitte Zahl zwischen {0} und {1}.", min, max);
            }
        }

        private static void Action_Settings()
        {
            Console.Clear();
            Console.WriteLine("EINSTELLUNGEN");
            Console.WriteLine("-------------");
            Console.WriteLine("[1] Admin: Servervorlagen verwalten (erstellen/ändern/löschen)");
            Console.WriteLine("[2] User-Slots umbenennen");
            Console.WriteLine("[3] Kostenmodell anpassen");
            Console.WriteLine("[0] Zurück");
            Console.Write("Auswahl: ");
            string a = Console.ReadLine();

            if (a == "1")
            {
                if (_currentUser == null || _currentUser.Role != Role.Admin) { Message("Nur Admin."); return; }
                AdminManageTemplates();
            }
            else if (a == "2")
            {
                RenameUserSlots();
            }
            else if (a == "3")
            {
                EditCosts();
            }
        }

        private static void AdminManageTemplates()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("ADMIN: SERVERVORLAGEN");
                Console.WriteLine("---------------------");
                if (_app.ServerTemplates.Count == 0) Console.WriteLine("(leer)");
                for (int i = 0; i < _app.ServerTemplates.Count; i++)
                {
                    var s = _app.ServerTemplates[i];
                    Console.WriteLine("[{0}] {1} | {2}HE, fan{3}mm:{4}, NT:{5}, SSD:{6}, HDD:{7}, GPU:{8} ({9})",
                        i + 1, s.Typ, s.HeightU, s.FanMm, s.FanCount, s.Netzteile, s.SSD, s.HDD, s.GPU, s.Origin);
                }
                Console.WriteLine();
                Console.WriteLine("[N] Neu  [E] Editieren  [D] Löschen  [Z] Zurück");
                Console.Write("Auswahl: ");
                string a = Console.ReadLine();

                if (a.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Höhe (1-4 HE): ");
                    string hs = Console.ReadLine();
                    int h; if (!int.TryParse(hs, out h)) { continue; }
                    if (h < 1 || h > 4) { continue; }
                    Console.Write("Typ/Bezeichnung: ");
                    string typ = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(typ)) typ = "Base";
                    int fanMm = (h == 1) ? 40 : (h == 2) ? 60 : (h == 3) ? 80 : 120;
                    int fanCount = AskInt("Lüfteranzahl (4-8): ", 4, 8);
                    int nt = AskInt("Netzteile (1-3): ", 1, 3);
                    int ssd = AskInt("SSD (0-4): ", 0, 4);
                    int hdd = AskInt("HDD (0-24): ", 0, 24);
                    int gpu = AskInt("GPU (0-8): ", 0, 8);
                    BaseServer s;
                    if (h == 1) s = new He1(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "b");
                    else if (h == 2) s = new He2(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "b");
                    else if (h == 3) s = new He3(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "b");
                    else s = new He4(typ, fanMm, fanCount, nt, ssd, hdd, gpu, "b");
                    _app.ServerTemplates.Add(s);
                    Save();
                }
                else if (a.Equals("E", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Index zum Editieren: ");
                    string s = Console.ReadLine();
                    int idx; if (!int.TryParse(s, out idx)) { continue; }
                    idx = idx - 1; if (idx < 0 || idx >= _app.ServerTemplates.Count) { continue; }
                    var e = _app.ServerTemplates[idx];

                    Console.WriteLine("Bearbeite: {0}", e.Typ);
                    Console.Write("Neuer Typ (Enter=unverändert): ");
                    string neuTyp = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(neuTyp)) e.Typ = neuTyp.Trim();

                    Console.Write("Neue Lüfteranzahl (4-8, Enter=unverändert): ");
                    string fcS = Console.ReadLine();
                    int v; if (int.TryParse(fcS, out v)) if (v >= 4 && v <= 8) e.FanCount = v;

                    Console.Write("Neue Netzteile (1-3, Enter=unverändert): ");
                    string ntS = Console.ReadLine();
                    if (int.TryParse(ntS, out v)) if (v >= 1 && v <= 3) e.Netzteile = v;

                    Console.Write("Neue SSD (0-4, Enter=unverändert): ");
                    string ssdS = Console.ReadLine();
                    if (int.TryParse(ssdS, out v)) if (v >= 0 && v <= 4) e.SSD = v;

                    Console.Write("Neue HDD (0-24, Enter=unverändert): ");
                    string hddS = Console.ReadLine();
                    if (int.TryParse(hddS, out v)) if (v >= 0 && v <= 24) e.HDD = v;

                    Console.Write("Neue GPU (0-8, Enter=unverändert): ");
                    string gpuS = Console.ReadLine();
                    if (int.TryParse(gpuS, out v)) if (v >= 0 && v <= 8) e.GPU = v;

                    Save();
                }
                else if (a.Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Index zum Löschen: ");
                    string s = Console.ReadLine();
                    int idx; if (!int.TryParse(s, out idx)) { continue; }
                    idx = idx - 1; if (idx < 0 || idx >= _app.ServerTemplates.Count) { continue; }
                    var name = _app.ServerTemplates[idx].Typ;
                    _app.ServerTemplates.RemoveAt(idx);
                    Save();
                    Console.WriteLine("Gelöscht: " + name);
                    Console.WriteLine("(Enter)"); Console.ReadLine();
                }
                else if (a.Equals("Z", StringComparison.OrdinalIgnoreCase))
                {
                    loop = false;
                }
            }
        }

        private static void EditCosts()
        {
            var c = _app.Costs;
            Console.Clear();
            Console.WriteLine("KOSTENMODELL ANPASSEN (€)");
            Console.WriteLine("--------------------------");
            Console.WriteLine("Aktuell:");
            PrintCosts(c);
            Console.WriteLine();

            Console.Write("Basispreis pro HE (Enter=unverändert): ");
            string s = Console.ReadLine(); decimal d;
            if (decimal.TryParse(s, out d)) c.BasePerU = d;

            Console.Write("Lüfter 40mm (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.Fan40 = d;
            Console.Write("Lüfter 60mm (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.Fan60 = d;
            Console.Write("Lüfter 80mm (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.Fan80 = d;
            Console.Write("Lüfter 120mm (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.Fan120 = d;

            Console.Write("Netzteil (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.PSU = d;
            Console.Write("SSD (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.SSD = d;
            Console.Write("HDD (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.HDD = d;
            Console.Write("GPU (€/Stk): "); s = Console.ReadLine(); if (decimal.TryParse(s, out d)) c.GPU = d;

            Save();
            Message("Kostenmodell gespeichert.");
        }

        private static void PrintCosts(CostSettings c)
        {
            Console.WriteLine("Basis pro HE: {0}€", c.BasePerU);
            Console.WriteLine("Lüfter: 40mm={0}€, 60mm={1}€, 80mm={2}€, 120mm={3}€", c.Fan40, c.Fan60, c.Fan80, c.Fan120);
            Console.WriteLine("PSU={0}€, SSD={1}€, HDD={2}€, GPU={3}€", c.PSU, c.SSD, c.HDD, c.GPU);
        }

        private static void Action_ShowCosts()
        {
            if (_currentRack == null) { Message("Kein aktives Rack."); return; }
            Console.Clear();
            Console.WriteLine("KOSTENÜBERSICHT");
            Console.WriteLine("----------------");
            decimal sum = 0m;
            for (int i = 0; i < _currentRack.Items.Count; i++)
            {
                var it = _currentRack.Items[i];
                decimal c = CostModel.CalcServerCost(it.Server, _app.Costs);
                sum += c;
                Console.WriteLine("#{0} @U{1}: {2}HE {3}  → {4}€", it.PlacedId, it.StartU, it.Server.HeightU, it.Server.Typ, c);
            }
            Console.WriteLine("----------------");
            Console.WriteLine("Rack-Gesamt: {0}€", sum);
            Console.WriteLine("(Enter)"); Console.ReadLine();
        }

        private static void Action_SimulateFailure()
        {
            if (_currentRack == null) { Message("Kein aktives Rack."); return; }
            Console.Clear();
            Console.WriteLine("AUSFALL SIMULIEREN (Ersatzteilkosten)");
            Console.WriteLine("-------------------------------------");
            for (int i = 0; i < _currentRack.Items.Count; i++)
            {
                var it = _currentRack.Items[i];
                Console.WriteLine("#{0} @U{1}: {2} ({3}HE)", it.PlacedId, it.StartU, it.Server.Typ, it.Server.HeightU);
            }
            Console.Write("Wähle #ID: ");
            string s = Console.ReadLine(); int id; if (!int.TryParse(s, out id)) { Message("Abbruch."); return; }
            var item = _currentRack.FindByPlacedId(id);
            if (item == null) { Message("Nicht gefunden."); return; }

            Console.WriteLine("Komponente: [1] Lüfter  [2] Netzteil  [3] SSD  [4] HDD  [5] GPU");
            Console.Write("Auswahl: "); string a = Console.ReadLine(); int comp;
            if (!int.TryParse(a, out comp) || comp < 1 || comp > 5) { Message("Abbruch."); return; }

            Console.Write("Anzahl defekt: "); string k = Console.ReadLine(); int qty; if (!int.TryParse(k, out qty) || qty < 1) { Message("Abbruch."); return; }

            decimal cost = CostModel.CalcFailureCost(item.Server, comp, qty, _app.Costs);
            Console.WriteLine("Ersatzteilkosten: {0}€", cost);
            Console.WriteLine("(Enter)"); Console.ReadLine();
        }
    }
}