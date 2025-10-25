namespace ServerRackSimulator
{
    internal class CostSettings
    {
        public decimal BasePerU;  // € pro HE (Gehäuse/Slots etc.)
        public decimal Fan40; public decimal Fan60; public decimal Fan80; public decimal Fan120; // € pro Lüfter
        public decimal PSU; public decimal SSD; public decimal HDD; public decimal GPU; // € pro Stück

        public static CostSettings CreateDefaults()
        {
            var c = new CostSettings();
            c.BasePerU = 100m;    // Beispielwerte – frei anpassbar
            c.Fan40 = 8m; c.Fan60 = 10m; c.Fan80 = 15m; c.Fan120 = 20m;
            c.PSU = 120m; c.SSD = 50m; c.HDD = 40m; c.GPU = 300m;
            return c;
        }
    }

    internal static class CostModel
    {
        public static decimal CalcServerCost(BaseServer s, CostSettings c)
        {
            decimal sum = 0m;
            sum += c.BasePerU * s.HeightU;
            sum += GetFanUnitPrice(s.FanMm, c) * s.FanCount;
            sum += c.PSU * s.Netzteile;
            sum += c.SSD * s.SSD;
            sum += c.HDD * s.HDD;
            sum += c.GPU * s.GPU;
            return sum;
        }

        public static decimal CalcFailureCost(BaseServer s, int componentCode, int qty, CostSettings c)
        {
            if (qty < 1) return 0m;
            if (componentCode == 1) return GetFanUnitPrice(s.FanMm, c) * qty;       // Lüfter
            if (componentCode == 2) return c.PSU * qty;                              // Netzteil
            if (componentCode == 3) return c.SSD * qty;                              // SSD
            if (componentCode == 4) return c.HDD * qty;                              // HDD
            if (componentCode == 5) return c.GPU * qty;                              // GPU
            return 0m;
        }

        private static decimal GetFanUnitPrice(int mm, CostSettings c)
        {
            if (mm == 40) return c.Fan40; if (mm == 60) return c.Fan60; if (mm == 80) return c.Fan80; return c.Fan120;
        }
    }
}