using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Current_GPS_Teller
{
    public class MainPageModel
    {
        public string Type { get; set; }
        // Server Update
        public string Temperature { get; set; } = "N/A";
        public string BatteryPercentage { get; set; } = "N/A";
        public string Alert { get; set; }

        // Location
        public string FileName { get; set; } = "N/A";
        public string LocationName { get; set; } = "N/A";
        public double Latitude { get; set; } = -1;
        public double Longitude { get; set; } = -1;
        public string Time { get; set; } = "N/A";
    }
}
