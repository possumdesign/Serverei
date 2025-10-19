using System;

namespace ServerRackSimulator
{
    internal abstract class BaseServer
    {
        public Guid Id = Guid.NewGuid();
        public string Typ;
        public int HeightU;      // 1..4
        public int FanMm;        // 40/60/80/120 (abhängig von HE)
        public int FanCount;     // 4..8
        public int Netzteile;    // 1..3
        public int SSD;          // 0..4
        public int HDD;          // 0..24
        public int GPU;          // 0..8
        public string Origin;    // "b"=Base(Admin), "c"=Custom(User)

        public BaseServer() { }
        public BaseServer(string typ, int heightU, int fanMm, int fanCount, int netzteile, int ssd, int hdd, int gpu, string origin)
        {
            Typ = typ; HeightU = heightU; FanMm = fanMm; FanCount = fanCount; Netzteile = netzteile; SSD = ssd; HDD = hdd; GPU = gpu; Origin = origin;
        }
        public abstract BaseServer Clone();
    }

    internal class He1 : BaseServer
    {
        public He1() { }
        public He1(string typ, int fanMm, int fanCount, int netzteile, int ssd, int hdd, int gpu, string origin)
            : base(typ, 1, fanMm, fanCount, netzteile, ssd, hdd, gpu, origin) { }
        public override BaseServer Clone() { return new He1(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin); }
    }
    internal class He2 : BaseServer
    {
        public He2() { }
        public He2(string typ, int fanMm, int fanCount, int netzteile, int ssd, int hdd, int gpu, string origin)
            : base(typ, 2, fanMm, fanCount, netzteile, ssd, hdd, gpu, origin) { }
        public override BaseServer Clone() { return new He2(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin); }
    }
    internal class He3 : BaseServer
    {
        public He3() { }
        public He3(string typ, int fanMm, int fanCount, int netzteile, int ssd, int hdd, int gpu, string origin)
            : base(typ, 3, fanMm, fanCount, netzteile, ssd, hdd, gpu, origin) { }
        public override BaseServer Clone() { return new He3(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin); }
    }
    internal class He4 : BaseServer
    {
        public He4() { }
        public He4(string typ, int fanMm, int fanCount, int netzteile, int ssd, int hdd, int gpu, string origin)
            : base(typ, 4, fanMm, fanCount, netzteile, ssd, hdd, gpu, origin) { }
        public override BaseServer Clone() { return new He4(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin); }
    }

    internal class BaseServerDTO
    {
        public string Typ;
        public int HeightU;
        public int FanMm;
        public int FanCount;
        public int Netzteile;
        public int SSD;
        public int HDD;
        public int GPU;
        public string Origin;

        public static BaseServerDTO FromServer(BaseServer s)
        {
            BaseServerDTO d = new BaseServerDTO();
            d.Typ = s.Typ; d.HeightU = s.HeightU; d.FanMm = s.FanMm; d.FanCount = s.FanCount; d.Netzteile = s.Netzteile; d.SSD = s.SSD; d.HDD = s.HDD; d.GPU = s.GPU; d.Origin = s.Origin;
            return d;
        }

        public BaseServer ToServer()
        {
            if (HeightU == 1) return new He1(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin);
            if (HeightU == 2) return new He2(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin);
            if (HeightU == 3) return new He3(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin);
            return new He4(Typ, FanMm, FanCount, Netzteile, SSD, HDD, GPU, Origin);
        }
    }
}

//wurde öfter autovervollständigt,zweiter versuch, Proof of concept
