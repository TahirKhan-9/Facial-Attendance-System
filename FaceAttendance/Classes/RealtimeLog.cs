using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceAttendance.Classes
{
    internal class RealtimeLog
    {
        public string MId { get; set; }
        public string EnrollNo { get; set; }

        public string Ip { get; set; }        
        public string InOut { get; set; }
        public string VeriMode { get; set; }
        public string DateTime { get; set; }
        public string Location { get; set; }
        public bool IsEnrolled { get; set; }
    }
}
