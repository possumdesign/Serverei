using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

internal abstract class BaseServer
{
    public Guid Id = Guid.NewGuid();
    public string Typ;
    public int HeightU;      // 1..4
    public int FanMm;        // 40/60/80/120
    public int Netzteile;    // 1 bis 3
    public int SSD;          // 0 bis 4
    public int HDD;          // 0bis 24
    public int GPU;          // 0 bis 8
    public string Origin;    // "b" = Base (admin), "c" = Custom (User)

    public BaseServer() { }
    public BaseServer(string typ, int heightU, int fanMm, int netzteile, int ssd, int hdd, int gpu, string origin)
    {
        Typ = typ; HeightU = heightU; FanMm = fanMm; Netzteile = netzteile; SSD = ssd; HDD = hdd; GPU = gpu; Origin = origin;
    }

    public string FanLabel()
    {
        return "fan" + FanMm + "mm";
    }

    public abstract BaseServer Clone();
}

internal class He1 : BaseServer
{
    public He1() { }
    public He1(string typ, int fanMm, int netzteile, int ssd, int hdd, int gpu, string origin)
        : base(typ, 1, fanMm, netzteile, ssd, hdd, gpu, origin) { }
    public override BaseServer Clone()
    {
        return new He1(Typ, FanMm, Netzteile, SSD, HDD, GPU, Origin);
    }
}

internal class He2 : BaseServer
{
    public He2() { }
    public He2(string typ, int fanMm, int netzteile, int ssd, int hdd, int gpu, string origin)
        : base(typ, 2, fanMm, netzteile, ssd, hdd, gpu, origin) { }
    public override BaseServer Clone()
    {
        return new He2(Typ, FanMm, Netzteile, SSD, HDD, GPU, Origin);
    }
}

internal class He3 : BaseServer
{
    public He3() { }
    public He3(string typ, int fanMm, int netzteile, int ssd, int hdd, int gpu, string origin)
        : base(typ, 3, fanMm, netzteile, ssd, hdd, gpu, origin) { }
    public override BaseServer Clone()
    {
        return new He3(Typ, FanMm, Netzteile, SSD, HDD, GPU, Origin);
    }
}

internal class He4 : BaseServer
{
    public He4() { }
    public He4(string typ, int fanMm, int netzteile, int ssd, int hdd, int gpu, string origin)
        : base(typ, 4, fanMm, netzteile, ssd, hdd, gpu, origin) { }
    public override BaseServer Clone()
    {
        return new He4(Typ, FanMm, Netzteile, SSD, HDD, GPU, Origin);
    }
}
//wurde öfter autovervollständigt,zweiter versuch, Proof of concept