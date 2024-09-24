using System;

namespace Smartfarm1
{
    public class FarmStatus
    {
        //public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public int SFID { get; set; }
        public int CO2 { get; set; }
        public int SoilMoisture { get; set; }
        public int Light_0x5C { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }
}
