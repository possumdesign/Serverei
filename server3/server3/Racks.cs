using System;
using System.Collections.Generic;

namespace ServerRackSimulator
{
    internal class Rack
    {
        public string Name;
        public int TotalU;
        public List<PlacedItem> Items = new List<PlacedItem>();
        private bool[] _occupied;

        public Rack(string name, int totalU)
        {
            Name = name; TotalU = totalU; _occupied = new bool[TotalU];
        }

        public int FindFirstFit(int heightU)
        {
            int start = 1;
            while (start + heightU - 1 <= TotalU)
            {
                bool frei = true;
                for (int j = 0; j < heightU; j++)
                {
                    if (_occupied[(start - 1) + j]) { frei = false; break; }
                }
                if (frei) return start;
                start++;
            }
            return -1;
        }

        public bool Place(PlacedItem item)
        {
            if (item.StartU < 1 || item.StartU + item.Server.HeightU - 1 > TotalU) return false;
            for (int j = 0; j < item.Server.HeightU; j++) if (_occupied[(item.StartU - 1) + j]) return false;
            for (int j = 0; j < item.Server.HeightU; j++) _occupied[(item.StartU - 1) + j] = true;
            Items.Add(item);
            return true;
        }

        public bool RemoveByPlacedId(int id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].PlacedId == id)
                {
                    var it = Items[i];
                    for (int j = 0; j < it.Server.HeightU; j++) _occupied[(it.StartU - 1) + j] = false;
                    Items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public PlacedItem FindByPlacedId(int id)
        {
            for (int i = 0; i < Items.Count; i++) if (Items[i].PlacedId == id) return Items[i];
            return null;
        }

        public void Render()
        {
            Console.WriteLine("Darstellung (Oben=1HE):");
            Console.WriteLine("----------------------");
            for (int u = 1; u <= TotalU; u++)
            {
                PlacedItem startItem = null;
                for (int i = 0; i < Items.Count; i++) if (Items[i].StartU == u) { startItem = Items[i]; break; }

                if (startItem != null)
                {
                    var s = startItem.Server;
                    string line = u + "HE  " + s.Typ + ", fan" + s.FanMm + "mm:" + s.FanCount + ", Netzteil:" + s.Netzteile + ", SSD:" + s.SSD + ", HDD:" + s.HDD + ", GPU:" + s.GPU + ",  #" + startItem.PlacedId + "                 H";
                    Console.WriteLine(PadRightTo(line, 70));
                    for (int extra = 1; extra < s.HeightU; extra++)
                    {
                        string fill = (u + extra) + "HE  (belegt durch #" + startItem.PlacedId + ")" + RepeatSpace(46) + "H";
                        Console.WriteLine(PadRightTo(fill, 70));
                    }
                    u = u + s.HeightU - 1;
                }
                else
                {
                    string empty = u + "HE" + RepeatSpace(58) + "H";  //schlechte Lösung, muss überarbeitet werden
                    Console.WriteLine(PadRightTo(empty, 70));
                }
            }
        }

        private string PadRightTo(string text, int total)
        {
            if (text.Length >= total) return text; return text + new string(' ', total - text.Length);
        }
        private string RepeatSpace(int n) { if (n <= 0) return ""; return new string(' ', n); }
    }

    internal class PlacedItem
    {
        public int PlacedId;
        public int StartU;
        public BaseServer Server;
        public PlacedItem() { }
        public PlacedItem(int placedId, int startU, BaseServer server) { PlacedId = placedId; StartU = startU; Server = server; }
    }

    internal class PlacedItemDTO
    {
        public int PlacedId;
        public int StartU;
        public BaseServerDTO Server;
    }

    internal class RackConfig
    {
        public string Name;
        public int TotalU;
        public List<PlacedItemDTO> Items = new List<PlacedItemDTO>();
    }
}