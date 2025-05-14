using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceAttendance.Classes
{
    class tbl_CusShift
    {
        public int id { get; set; }

        public TimeSpan Time_IN { get; set; }
        public TimeSpan Time_OUT { get; set; }
    }
}
