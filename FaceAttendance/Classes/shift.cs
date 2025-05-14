using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceAttendance.Classes
{
    class shift
    {
        public int ShiftId { get; set; } = 0;
        //public int ShiftId
        //{
        //    get
        //    {
        //        return Parse (string.IsNullOrEmpty(ShiftId) ? 0 : ShiftId);
        //    }
        //    set
        //    {
        //        this.ShiftId = value;
        //    }
        //}
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public TimeSpan MinStartTime { get; set; }
        public TimeSpan MaxEndTime { get; set; }

        public string ShiftName { get; set; }

    }
}
