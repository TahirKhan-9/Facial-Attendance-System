namespace FaceAttendance.Classes
{
    public class userS
    {
        public long SrNo { get; set; }
        public int EMachineNumber { get; set; }
        public int EnrollNumber { get; set; }
        public int FingerNumber { get; set; }
        public int Privilige { get; set; }
        public int enPassword { get; set; }
        public string EName { get; set; }

        public string ShiftId { get; set; }

        public string Ip { get; set; }
        public string DptID { get; set; }
        public string FingerName { get; set; }
        public string PriviligeName { get; set; }
        public string DepartmentName { get; set; }
        public byte[] Photo { get; set; }
        public userS()
        {

        }
        public userS(userS Original)
        {
            SrNo = Original.SrNo;
            EMachineNumber = Original.EMachineNumber;
            EnrollNumber = Original.EnrollNumber;
            FingerNumber = Original.FingerNumber;
            Privilige = Original.Privilige;
            enPassword = Original.enPassword;
            EName = Original.EName;
            ShiftId = Original.ShiftId;
            Ip = Original.Ip;
            DptID = Original.DptID;
            FingerName = Original.FingerName;
            PriviligeName = Original.PriviligeName;
            DepartmentName = Original.DepartmentName;
            Photo = Original.Photo;
            
        }
    }
}
