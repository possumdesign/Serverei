using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serverei
{
    internal class server19
    {
        public int HEunit { get; set; }
        public string Hardware { get; set; }
        public int Fan12 { get; set; }
        public int Fan6 { get; set; }  
        public int Fan4 { get; set; }   

        public void Anzeigen()
        {
            Console.WriteLine($"Typ: {HEunit} mit {Hardware}");
        }
    }
    internal class HE4 : server19 
    {
        [JsonConstructor]
        public HE4(int HEunit, string Hardware)
        {
            this.HEunit = HEunit;
            this.Hardware = Hardware;
        }

        public HE4(int HEunit)
        {
            HEunit = 4;
        }
        public int Fan12 = 6;
    }
    internal class HE3 : server19
    {
        [JsonConstructor]
        public HE3(int HEunit, string Hardware)
        {
            this.HEunit = HEunit;
            this.Hardware = Hardware;
        }
        public HE3(int HEunit)
        {
            HEunit = 3;
        }
        public int Fan6 = 5;
    }

    internal class HE2 : server19
    {
        [JsonConstructor]
        public HE2(int HEunit, string Hardware)
        {
            this.HEunit = HEunit;
            this.Hardware = Hardware;
        }
        public HE2(int HEunit)
        {
            HEunit = 2;
        }
        public int Fan6 = 4;
    }
    internal class HE1 : server19
    {
        [JsonConstructor]
        public HE1(int HEunit, string Hardware)
        {
            this.HEunit = HEunit;
            this.Hardware = Hardware;
        }
        public HE1(int HEunit)
        {
            HEunit = 1;
        }
        public int Fan4 = 5;
    }
}
