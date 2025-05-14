//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using UsmanCodeBlocks.Data.Sql.Local;


//namespace FaceAttendance.Classes
//{
//    public class MonthlyRepo
//    {
//        int temDay;
//        private string ConnString;
//        bool SaturdayChecked;
//        bool SundayChecked;
//        private DataTable dtE = new DataTable();
//        private DataTable dtMr = new DataTable();
//        private DataTable dtDr = new DataTable();
//        object[] obj;

//        public MonthlyRepo(string constring)
//        {
//            ConnString = constring;
//        }
//        public MonthlyRepo(string constring, bool SaturdayFlag, bool SundayFlag)
//        {
//            ConnString = constring;
//            SaturdayChecked = SaturdayFlag;
//            SundayChecked = SundayFlag;
//        }


//        ////EnrollNumber EName
//        ////    1	
//        ////    1	    kashan

//        ////    2	
//        ////    2	    kashan

//        ////    3	

//        ////    4	

//        private void getdtE()
//        {
//            //string query1 = "select EnrollNumber,EName from tbl_enroll";
//            string query = "select   EnrollNumber , EName from tbl_enroll group by EnrollNumber, EName";
//            DataTable dt_tmp = new DataTable();
//            dt_tmp = DBFactory.GetAllByQuery(ConnString, query);
//            int id1, id2;
//            int count = dt_tmp.Rows.Count;
//            dtE.Columns.Add("Enroll_Id", typeof(System.String));
//            dtE.Columns.Add("User_Name", typeof(System.String));
//            int c = 1;
//            for (int i = 0; i < count; i++)
//            {
//                if (i + 1 < count)
//                {
//                    id1 = int.Parse(dt_tmp.Rows[i].ItemArray[0].ToString());
//                    id2 = int.Parse(dt_tmp.Rows[i + 1].ItemArray[0].ToString());
//                    if (id1 == id2)
//                    {
//                        c += 1;
//                        if (dt_tmp.Rows[i].ItemArray[1].ToString() != "")
//                        {

//                            //DataRow workRow;
//                            //workRow = dtE.NewRow();
//                            object[] workRow = new object[2];
//                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
//                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

//                            dtE.Rows.Add(workRow);
//                        }
//                        else if (dt_tmp.Rows[i + 1].ItemArray[1].ToString() != "")
//                        {

//                            //DataRow workRow;
//                            //workRow = dtE.NewRow();
//                            object[] workRow = new object[2];
//                            string dump = dt_tmp.Rows[i].ItemArray[0].ToString();
//                            workRow[0] = dump;
//                            workRow[1] = dt_tmp.Rows[i + 1].ItemArray[1].ToString();

//                            dtE.Rows.Add(workRow);
//                        }
//                        else
//                        {
//                            //DataRow workRow;
//                            //workRow = dtE.NewRow();
//                            object[] workRow = new object[2];
//                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
//                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

//                            dtE.Rows.Add(workRow);
//                        }
//                        i = i + 1;

//                    }
//                    else
//                    {
//                        c += 1;
//                        //DataRow workRow;
//                        //workRow = dtE.NewRow();
//                        object[] workRow = new object[2];
//                        workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
//                        workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

//                        dtE.Rows.Add(workRow);

//                    }
//                }
//                else
//                {
//                    c += 1;
//                    //DataRow workRow;
//                    //workRow = dtE.NewRow();
//                    object[] workRow = new object[2];
//                    workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
//                    workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

//                    dtE.Rows.Add(workRow);
//                }

//            }

//            int a = c;
//        }

//        //private void initialSetdtMr()
//        //{
//        //    dtMr.Columns.Add("Id", typeof(System.String));
//        //    dtMr.Columns.Add("Enroll_Id", typeof(System.String));
//        //    dtMr.Columns.Add("User_Name", typeof(System.String));
//        //    for (int i = 0; i < 31; i++)
//        //    {
//        //        dtMr.Columns.Add("Day" + (i + 1).ToString(), typeof(System.String));
//        //    }


//        //}


//        private void baseLoop(int days, int month , int year,string inTime,string outTime)
//        {
//            dtMr.Columns.Add("No", typeof(System.String));
//            dtMr.Columns.Add("User_ID", typeof(System.String));
//            dtMr.Columns.Add("User_Name", typeof(System.String));
//            for (int i = 1; i <= days; i++)
//            {
//                dtMr.Columns.Add("Day" + (i).ToString(), typeof(System.String));
//                //dtMr.Columns.Add("Day"+i.ToString() + "/"+month.ToString() + "/"+year.ToString(), typeof(System.String));
//            }
//            dtMr.Columns.Add("Remark", typeof(System.String));
//            //dtMr.Columns.Add("OverTime", typeof(System.String));
//            //initialSetdtMr();
//            getdtE();
//            getdtDr( year, month , days,inTime,outTime);

//            obj = new object[days + 4]; 
//            for (int i = 0; i < dtE.Rows.Count; i++)
//            {
//                obj[0] = i + 1;
//                obj[1] = dtE.Rows[i].ItemArray[0].ToString();
//                obj[2] = dtE.Rows[i].ItemArray[1].ToString();
//                for (int j = 3; j < days + 4; j++)

//                {
//                    //dtMr.Rows[j].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
//                    //dtMr.Rows[j].ItemArray[0] = "<br>";
//                    //dtMr.Rows[j].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
//                    //if (dtMr.Rows[i].ItemArray[0].ToString() == dtDr.Rows[j].ItemArray[4].ToString())
//                    //{
//                    //obj[j] = dtDr.Rows[i].ItemArray[6].ToString();
//                    //obj[j] += " <br>";
//                    //obj[j] += dtDr.Rows[i].ItemArray[7].ToString();  
//                    //}
//                    //obj[j] = "a";
//                    //obj[j] += " <br>";
//                    //obj[j] += "a";
//                    //obj[j] = dtDr.Rows[i].ItemArray[0].ToString();
//                    obj[j] = "".ToString();


//                }
//                dtMr.Rows.Add(obj);
//                //initialSetdtMr();
//                //getdtE();
//                //int row = dtE.Rows.Count;
//                //for (int i=0; i<row;i++)
//                //{
//                //    dtMr.Rows[i].ItemArray[0] = (i+1).ToString();
//                //    dtMr.Rows[i].ItemArray[0] = dtE.Rows[i].ItemArray[0];
//                //    dtMr.Rows[i].ItemArray[0] = dtE.Rows[i].ItemArray[1];
//                //}
//            }
//        }
//        private void baseLoop(int days, int month, int year)
//        {
//            dtMr.Columns.Add("No", typeof(System.String));
//            dtMr.Columns.Add("User_ID", typeof(System.String));
//            dtMr.Columns.Add("User_Name", typeof(System.String));
//            for (int i = 1; i <= days; i++)
//            {
//                _ = dtMr.Columns.Add("Day" + i.ToString(), typeof(System.String));
//            }
//            dtMr.Columns.Add("Remark", typeof(System.String));
//            getdtE();
//            //My Modification starts from here
//            List<int> SaturdayDates = new List<int>();
//            List<int> SundayDates = new List<int>();
//            for (int day = 1; day <= days; day++)
//            {
//                DateTime currentDate = new DateTime(year, month, day);
//                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
//                {
//                    SaturdayDates.Add(day);
//                }
//                else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
//                {
//                    SundayDates.Add(day);
//                }
//                else
//                {
//                    continue;
//                }
//            }
//            obj = new object[days + 4];
//            for (int i = 0; i < dtE.Rows.Count; i++)
//            {
//                obj[0] = i + 1;
//                obj[1] = dtE.Rows[i].ItemArray[0].ToString();
//                obj[2] = dtE.Rows[i].ItemArray[1].ToString();
//                for (int j = 3; j < days + 4; j++)

//                {

//                    int day = j - 2;
//                    if (SundayDates.Contains(day) && SundayChecked)
//                    {
//                        obj[j] = "-----".ToString();
//                    }
//                    else if (SaturdayDates.Contains(day) && SaturdayChecked)
//                    {
//                        obj[j] = "-----".ToString();
//                    }
//                    else
//                    {
//                        obj[j] = "".ToString();

//                    }
//                }
//                dtMr.Rows.Add(obj);

//            }
//        }

//        private void getdtDr(int year, int month, int days,string inTime,string outTime)
//        {
//                //            string query = " SELECT y.[EMachineNumber],y.[EName], b.[ShiftStart],b.[ShiftEnd],  EnrollNo 'Userid',"
//                //    + "CONVERT(DATE,[DateTime]) 'Date',"
//                //    + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
//                //    + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
//                //    + "    '' "
//                //    + "ELSE"
//                //    + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8)))"
//                //    + " END 'TimeOut',"
//                //    + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
//                //    + "    'Didn''t Clock Out'"
//                //    + "ELSE"
//                //     + "   '' "
//                //    + "END 'Remark'"
//                //+ " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber  inner join tbl_shifts b on b.ShiftId = y.ShiftId where y.FingerNumber = 50 and[DateTime] >= '" + "drDate" + "' and[DateTime] <= '" + "drDate" + " 23:59:59'"
//                //+ " GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]), b.[ShiftStart],b.[ShiftEnd] order by[Date] asc";


//            string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date', MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    '' ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut', CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'ELSE   '' END 'Remark' FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber where y.FingerNumber in (0,1,2,3,4,5,11,50) and[DateTime] >= '" + year + "-" + month + "-01' and[DateTime] <= '" + year + "-" + month + "-" + days + " 23:59:59' and (CONVERT(TIME,[DateTime])) >= '"+ inTime + "' and (CONVERT(TIME,[DateTime])) <= '"+outTime+"' GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) order by[EnrollNo] asc";

//            dtDr = DBFactory.GetAllByQuery(ConnString, query);
//        }
//        private void getdtDr(int year, int month, int days, int shiftID, string shiftName)
//        {
//            //            string query = " SELECT y.[EMachineNumber],y.[EName], b.[ShiftStart],b.[ShiftEnd],  EnrollNo 'Userid',"
//            //    + "CONVERT(DATE,[DateTime]) 'Date',"
//            //    + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
//            //    + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
//            //    + "    '' "
//            //    + "ELSE"
//            //    + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8)))"
//            //    + " END 'TimeOut',"
//            //    + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
//            //    + "    'Didn''t Clock Out'"
//            //    + "ELSE"
//            //     + "   '' "
//            //    + "END 'Remark'"
//            //+ " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber  inner join tbl_shifts b on b.ShiftId = y.ShiftId where y.FingerNumber = 50 and[DateTime] >= '" + "drDate" + "' and[DateTime] <= '" + "drDate" + " 23:59:59'"
//            //+ " GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]), b.[ShiftStart],b.[ShiftEnd] order by[Date] asc";


//            //string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date', MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    '' ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut', CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'ELSE   '' END 'Remark' FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber where y.FingerNumber = 50 and[DateTime] >= '" + year + "-" + month + "-01' and[DateTime] <= '" + year + "-" + month + "-" + days + " 23:59:59' and (CONVERT(TIME,[DateTime])) >= '" + inTime + "' and (CONVERT(TIME,[DateTime])) <= '" + outTime + "' GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) order by[EnrollNo] asc";
//            string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date',"
//                            + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
//                            + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    ''"
//                            + "ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut',"
//                            + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'"
//                            + "ELSE   '' END 'Remark'"
//                            + " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber inner join tbl_shifts s on y.ShiftId = s.ShiftId "
//                            + "where y.FingerNumber in (0,1,2,3,4,5,11,50) and[DateTime] >= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-01' "
//                            + "and s.ShiftId = '" + shiftID + "' and s.ShiftName = '" + shiftName + "' "
//                            + "and[DateTime] <= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-' + CAST('" + days + "' AS NVARCHAR(2)) + ' 23:59:59' "
//                            + "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) "
//                            + "order by [EnrollNo] asc";
//            dtDr = DBFactory.GetAllByQuery(ConnString, query);
//        }


//        public DataTable fillBody(int year, int month, string inTime, string outTime)
//         {
//            //int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
//            int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);

//            baseLoop(days,month , year,inTime,outTime);
//            getdtDr(year, month, days,inTime,outTime);
//            //dynamic[] workRow = new dynamic[days + 3];
//            for (int d = 1; d <= days; d++)
//            {
//                for (int i = 0; i < dtMr.Rows.Count; i++)
//                {

//                    for (int j = 0; j < dtDr.Rows.Count; j++)
//                    {
//                        if(!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[3].ToString()))
//                        {
//                             temDay = int.Parse( dtDr.Rows[j].ItemArray[3].ToString().Substring(3,2));
//                        }
//                        if (dtMr.Rows[i].ItemArray[1].ToString() == dtDr.Rows[j].ItemArray[2].ToString() && d == temDay)
//                        {
//                            object[] workRow = new object[days + 4];
//                            if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && !String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
//                            {
//                                workRow[d + 2] = dtDr.Rows[j].ItemArray[4].ToString().Substring(0, 5);
//                                workRow[d + 2] += "-";
//                                workRow[d + 2] += dtDr.Rows[j].ItemArray[5].ToString().Substring(0, 5);
//                            }
//                            //int index = workRow.Length - 1;
//                            //int overTimeIndex = workRow.Length - 1;
//                            //workRow[index] += ;
//                            //workRow[overTimeIndex] += dtDr.Rows[j].ItemArray[7].ToString();
//                            DataRow tempRow = dtMr.Rows[i];
//                            tempRow.ItemArray = workRow;
//                            dtMr.Rows[i].ItemArray[d] = workRow;
//                            //obj[j+3] = dtDr.Rows[j].ItemArray[6].ToString();
//                            //obj[j+3] += " <br>";
//                            //obj[j+3] += dtDr.Rows[j].ItemArray[7].ToString();
//                            //dtMr.Rows[i].ItemArray[d] = obj;
//                            //DataRow temRow = dtMr.Rows[i];
//                            //temRow.ItemArray = obj;
//                            //dtMr.Rows.Add(obj);
//                            //DataRow row;
//                            //dynamic[] workRow = new dynamic[days + 3];
//                            //dtMr.Rows.Add(workRow);
//                            // dtMr.Rows[i].ItemArray[d] = workRow;
//                            //dtMr.AcceptChanges();
//                            //DataRow workRow;
//                            //workRow = dtMr.NewRow();
//                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
//                            //dtMr.Rows[i].ItemArray[0] = "<br>";
//                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
//                            //dtMr.Rows[i].ItemArray[0] = "i";
//                            //dtMr.Rows[i].ItemArray[0] = "<br>";
//                            //dtMr.Rows[i].ItemArray[0] = "usama";
//                            //object[] workRow = new Object[days+3];
//                            //workRow[count] = dtDr.Rows[j].ItemArray[6].ToString();
//                            //workRow[count] += " <br>";
//                            //workRow[count] += dtDr.Rows[j].ItemArray[7].ToString();
//                            ////dtMr.Rows[i].ItemArray[d]= workRow;
//                            //dtMr.Rows[count].ItemArray[d] = workRow;
//                            //row = dtMr.NewRow();
//                            //row.ItemArray = workRow;
//                        }

//                    }
//                }
//            }

//            return dtMr;

//        }
//        public DataTable fillBody(int year, int month, int shiftID, string shiftName, int days)
//        {
//            //int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);


//            baseLoop(days, month, year);
//            //getdtDr(year, month, days, inTime, outTime);
//            getdtDr(year, month, days, shiftID, shiftName);
//            //dynamic[] workRow = new dynamic[days + 3];
//            for (int d = 1; d <= days; d++)
//            {
//                for (int i = 0; i < dtMr.Rows.Count; i++)
//                {

//                    for (int j = 0; j < dtDr.Rows.Count; j++)
//                    {
//                        if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[3].ToString()))
//                        {
//                            temDay = int.Parse(dtDr.Rows[j].ItemArray[3].ToString().Substring(3, 2));
//                        }
//                        if (dtMr.Rows[i].ItemArray[1].ToString() == dtDr.Rows[j].ItemArray[2].ToString() && d == temDay)
//                        {
//                            object[] workRow = new object[days + 4];
//                            if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && !String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
//                            {
//                                workRow[d + 2] = dtDr.Rows[j].ItemArray[4].ToString().Substring(0, 5);
//                                workRow[d + 2] += "-";
//                                workRow[d + 2] += dtDr.Rows[j].ItemArray[5].ToString().Substring(0, 5);
//                            }
//                            //int index = workRow.Length - 1;
//                            //int overTimeIndex = workRow.Length - 1;
//                            //workRow[index] += ;
//                            //workRow[overTimeIndex] += dtDr.Rows[j].ItemArray[7].ToString();
//                            DataRow tempRow = dtMr.Rows[i];
//                            tempRow.ItemArray = workRow;
//                            dtMr.Rows[i].ItemArray[d] = workRow;
//                            //obj[j+3] = dtDr.Rows[j].ItemArray[6].ToString();
//                            //obj[j+3] += " <br>";
//                            //obj[j+3] += dtDr.Rows[j].ItemArray[7].ToString();
//                            //dtMr.Rows[i].ItemArray[d] = obj;
//                            //DataRow temRow = dtMr.Rows[i];
//                            //temRow.ItemArray = obj;
//                            //dtMr.Rows.Add(obj);
//                            //DataRow row;
//                            //dynamic[] workRow = new dynamic[days + 3];
//                            //dtMr.Rows.Add(workRow);
//                            // dtMr.Rows[i].ItemArray[d] = workRow;
//                            //dtMr.AcceptChanges();
//                            //DataRow workRow;
//                            //workRow = dtMr.NewRow();
//                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
//                            //dtMr.Rows[i].ItemArray[0] = "<br>";
//                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
//                            //dtMr.Rows[i].ItemArray[0] = "i";
//                            //dtMr.Rows[i].ItemArray[0] = "<br>";
//                            //dtMr.Rows[i].ItemArray[0] = "usama";
//                            //object[] workRow = new Object[days+3];
//                            //workRow[count] = dtDr.Rows[j].ItemArray[6].ToString();
//                            //workRow[count] += " <br>";
//                            //workRow[count] += dtDr.Rows[j].ItemArray[7].ToString();
//                            ////dtMr.Rows[i].ItemArray[d]= workRow;
//                            //dtMr.Rows[count].ItemArray[d] = workRow;
//                            //row = dtMr.NewRow();
//                            //row.ItemArray = workRow;
//                        }

//                    }
//                }
//            }

//            return dtMr;

//        }



//    }
//}






using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UsmanCodeBlocks.Data.Sql.Local;


namespace FaceAttendance.Classes
{
    public class MonthlyRepo
    {
        int temDay;
        private string ConnString;
        bool SaturdayChecked;
        bool SundayChecked;
        private DataTable dtE = new DataTable();
        private DataTable dtMr = new DataTable();
        private DataTable dtDr = new DataTable();
        private DataTable dtAttendanceRegister = new DataTable();
        private DataTable dtEmployeeAttendanceRegister = new DataTable();
        object[] obj;
        private Dictionary<string, string> usersDepartments = new Dictionary<string, string>();
        public MonthlyRepo(string constring)
        {
            ConnString = constring;
        }
        public MonthlyRepo(string constring, bool SaturdayFlag, bool SundayFlag)
        {
            ConnString = constring;
            SaturdayChecked = SaturdayFlag;
            SundayChecked = SundayFlag;
        }


        ////EnrollNumber EName
        ////    1	
        ////    1	    kashan

        ////    2	
        ////    2	    kashan

        ////    3	

        ////    4	

        private void getdtE()
        {
            //string query1 = "select EnrollNumber,EName from tbl_enroll";
            string query = "select   EnrollNumber , EName from tbl_enroll group by EnrollNumber, EName";
            DataTable dt_tmp = new DataTable();
            dt_tmp = DBFactory.GetAllByQuery(ConnString, query);
            int id1, id2;
            int count = dt_tmp.Rows.Count;
            dtE.Columns.Add("Enroll_Id", typeof(System.String));
            dtE.Columns.Add("User_Name", typeof(System.String));
            int c = 1;
            for (int i = 0; i < count; i++)
            {
                if (i + 1 < count)
                {
                    id1 = int.Parse(dt_tmp.Rows[i].ItemArray[0].ToString());
                    id2 = int.Parse(dt_tmp.Rows[i + 1].ItemArray[0].ToString());
                    if (id1 == id2)
                    {
                        c += 1;
                        if (dt_tmp.Rows[i].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        else if (dt_tmp.Rows[i + 1].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            string dump = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[0] = dump;
                            workRow[1] = dt_tmp.Rows[i + 1].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        else
                        {
                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        i = i + 1;

                    }
                    else
                    {
                        c += 1;
                        //DataRow workRow;
                        //workRow = dtE.NewRow();
                        object[] workRow = new object[2];
                        workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                        workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                        dtE.Rows.Add(workRow);

                    }
                }
                else
                {
                    c += 1;
                    //DataRow workRow;
                    //workRow = dtE.NewRow();
                    object[] workRow = new object[2];
                    workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                    workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                    dtE.Rows.Add(workRow);
                }

            }

            int a = c;
        }
        private void getdtEByShiftID(int shiftID)
        {
            //string query1 = "select EnrollNumber,EName from tbl_enroll";
            string query = $"select   EnrollNumber , EName from tbl_enroll where ShiftId = {shiftID} group by EnrollNumber, EName ";
            DataTable dt_tmp = new DataTable();
            dt_tmp = DBFactory.GetAllByQuery(ConnString, query);
            int id1, id2;
            int count = dt_tmp.Rows.Count;
            dtE.Columns.Add("Enroll_Id", typeof(System.String));
            dtE.Columns.Add("User_Name", typeof(System.String));
            int c = 1;
            for (int i = 0; i < count; i++)
            {
                if (i + 1 < count)
                {
                    id1 = int.Parse(dt_tmp.Rows[i].ItemArray[0].ToString());
                    id2 = int.Parse(dt_tmp.Rows[i + 1].ItemArray[0].ToString());
                    if (id1 == id2)
                    {
                        c += 1;
                        if (dt_tmp.Rows[i].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        else if (dt_tmp.Rows[i + 1].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            string dump = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[0] = dump;
                            workRow[1] = dt_tmp.Rows[i + 1].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        else
                        {
                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtE.Rows.Add(workRow);
                        }
                        i = i + 1;

                    }
                    else
                    {
                        c += 1;
                        //DataRow workRow;
                        //workRow = dtE.NewRow();
                        object[] workRow = new object[2];
                        workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                        workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                        dtE.Rows.Add(workRow);

                    }
                }
                else
                {
                    c += 1;
                    //DataRow workRow;
                    //workRow = dtE.NewRow();
                    object[] workRow = new object[2];
                    workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                    workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                    dtE.Rows.Add(workRow);
                }

            }

            int a = c;
        }
        //private void initialSetdtMr()
        //{
        //    dtMr.Columns.Add("Id", typeof(System.String));
        //    dtMr.Columns.Add("Enroll_Id", typeof(System.String));
        //    dtMr.Columns.Add("User_Name", typeof(System.String));
        //    for (int i = 0; i < 31; i++)
        //    {
        //        dtMr.Columns.Add("Day" + (i + 1).ToString(), typeof(System.String));
        //    }


        //}


        private void baseLoop(int days, int month, int year, string inTime, string outTime)
        {
            dtMr.Columns.Add("No", typeof(System.String));
            dtMr.Columns.Add("User_ID", typeof(System.String));
            dtMr.Columns.Add("User_Name", typeof(System.String));
            for (int i = 1; i <= days; i++)
            {
                dtMr.Columns.Add("Day" + (i).ToString(), typeof(System.String));
                //dtMr.Columns.Add("Day"+i.ToString() + "/"+month.ToString() + "/"+year.ToString(), typeof(System.String));
            }
            dtMr.Columns.Add("Remark", typeof(System.String));
            //dtMr.Columns.Add("OverTime", typeof(System.String));
            //initialSetdtMr();
            getdtE();
            getdtDr(year, month, days, inTime, outTime);

            obj = new object[days + 4];
            for (int i = 0; i < dtE.Rows.Count; i++)
            {
                obj[0] = i + 1;
                obj[1] = dtE.Rows[i].ItemArray[0].ToString();
                obj[2] = dtE.Rows[i].ItemArray[1].ToString();
                for (int j = 3; j < days + 4; j++)

                {
                    //dtMr.Rows[j].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
                    //dtMr.Rows[j].ItemArray[0] = "<br>";
                    //dtMr.Rows[j].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
                    //if (dtMr.Rows[i].ItemArray[0].ToString() == dtDr.Rows[j].ItemArray[4].ToString())
                    //{
                    //obj[j] = dtDr.Rows[i].ItemArray[6].ToString();
                    //obj[j] += " <br>";
                    //obj[j] += dtDr.Rows[i].ItemArray[7].ToString();  
                    //}
                    //obj[j] = "a";
                    //obj[j] += " <br>";
                    //obj[j] += "a";
                    //obj[j] = dtDr.Rows[i].ItemArray[0].ToString();
                    obj[j] = "".ToString();


                }
                dtMr.Rows.Add(obj);
                //initialSetdtMr();
                //getdtE();
                //int row = dtE.Rows.Count;
                //for (int i=0; i<row;i++)
                //{
                //    dtMr.Rows[i].ItemArray[0] = (i+1).ToString();
                //    dtMr.Rows[i].ItemArray[0] = dtE.Rows[i].ItemArray[0];
                //    dtMr.Rows[i].ItemArray[0] = dtE.Rows[i].ItemArray[1];
                //}
            }
        }
        private void baseLoop(int days, int month, int year, int shiftID)
        {
            dtMr.Columns.Add("No", typeof(System.String));
            dtMr.Columns.Add("User_ID", typeof(System.String));
            dtMr.Columns.Add("User_Name", typeof(System.String));
            for (int i = 1; i <= days; i++)
            {
                _ = dtMr.Columns.Add("Day" + i.ToString(), typeof(System.String));
            }
            dtMr.Columns.Add("Remark", typeof(System.String));
            //getdtE();
            getdtEByShiftID(shiftID);
            //My Modification starts from here
            List<int> SaturdayDates = new List<int>();
            List<int> SundayDates = new List<int>();
            for (int day = 1; day <= days; day++)
            {
                DateTime currentDate = new DateTime(year, month, day);
                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    SaturdayDates.Add(day);
                }
                else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    SundayDates.Add(day);
                }
                else
                {
                    continue;
                }
            }
            obj = new object[days + 4];
            for (int i = 0; i < dtE.Rows.Count; i++)
            {
                obj[0] = i + 1;
                obj[1] = dtE.Rows[i].ItemArray[0].ToString();
                obj[2] = dtE.Rows[i].ItemArray[1].ToString();
                for (int j = 3; j < days + 4; j++)

                {

                    int day = j - 2;
                    if (SundayDates.Contains(day) && SundayChecked)
                    {
                        obj[j] = "-----".ToString();
                    }
                    else if (SaturdayDates.Contains(day) && SaturdayChecked)
                    {
                        obj[j] = "-----".ToString();
                    }
                    else
                    {
                        obj[j] = "".ToString();

                    }
                }
                dtMr.Rows.Add(obj);

            }
        }

        private void getdtDr(int year, int month, int days, string inTime, string outTime)
        {
            //            string query = " SELECT y.[EMachineNumber],y.[EName], b.[ShiftStart],b.[ShiftEnd],  EnrollNo 'Userid',"
            //    + "CONVERT(DATE,[DateTime]) 'Date',"
            //    + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
            //    + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            //    + "    '' "
            //    + "ELSE"
            //    + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8)))"
            //    + " END 'TimeOut',"
            //    + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            //    + "    'Didn''t Clock Out'"
            //    + "ELSE"
            //     + "   '' "
            //    + "END 'Remark'"
            //+ " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber  inner join tbl_shifts b on b.ShiftId = y.ShiftId where y.FingerNumber = 50 and[DateTime] >= '" + "drDate" + "' and[DateTime] <= '" + "drDate" + " 23:59:59'"
            //+ " GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]), b.[ShiftStart],b.[ShiftEnd] order by[Date] asc";


            string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date', MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    '' ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut', CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'ELSE   '' END 'Remark' FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber where y.FingerNumber in (0,1,2,3,4,5,11,50) and[DateTime] >= '" + year + "-" + month + "-01' and[DateTime] <= '" + year + "-" + month + "-" + days + " 23:59:59' and (CONVERT(TIME,[DateTime])) >= '" + inTime + "' and (CONVERT(TIME,[DateTime])) <= '" + outTime + "' GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) order by[EnrollNo] asc";

            dtDr = DBFactory.GetAllByQuery(ConnString, query);
        }
        private void getdtDr(int year, int month, int days, int shiftID, string shiftName)
        {
            //            string query = " SELECT y.[EMachineNumber],y.[EName], b.[ShiftStart],b.[ShiftEnd],  EnrollNo 'Userid',"
            //    + "CONVERT(DATE,[DateTime]) 'Date',"
            //    + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
            //    + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            //    + "    '' "
            //    + "ELSE"
            //    + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8)))"
            //    + " END 'TimeOut',"
            //    + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            //    + "    'Didn''t Clock Out'"
            //    + "ELSE"
            //     + "   '' "
            //    + "END 'Remark'"
            //+ " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber  inner join tbl_shifts b on b.ShiftId = y.ShiftId where y.FingerNumber = 50 and[DateTime] >= '" + "drDate" + "' and[DateTime] <= '" + "drDate" + " 23:59:59'"
            //+ " GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]), b.[ShiftStart],b.[ShiftEnd] order by[Date] asc";


            //string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date', MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    '' ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut', CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'ELSE   '' END 'Remark' FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber where y.FingerNumber = 50 and[DateTime] >= '" + year + "-" + month + "-01' and[DateTime] <= '" + year + "-" + month + "-" + days + " 23:59:59' and (CONVERT(TIME,[DateTime])) >= '" + inTime + "' and (CONVERT(TIME,[DateTime])) <= '" + outTime + "' GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) order by[EnrollNo] asc";
            string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo 'Userid', CONVERT(DATE,[DateTime]) 'Date',"
                            + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) 'TimeIN',"
                            + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    ''"
                            + "ELSE MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) END 'TimeOut',"
                            + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN    'Didn''t Clock Out'"
                            + "ELSE   '' END 'Remark'"
                            + " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber inner join tbl_shifts s on y.ShiftId = s.ShiftId "
                            + "where y.FingerNumber in (0,1,2,3,4,5,11,50) and[DateTime] >= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-01' "
                            + "and s.ShiftId = '" + shiftID + "' and s.ShiftName = '" + shiftName + "' "
                            + "and[DateTime] <= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-' + CAST('" + days + "' AS NVARCHAR(2)) + ' 23:59:59' "
                            + "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH,[DateTime]), CONVERT(DATE,[DateTime]) "
                            + "order by [EnrollNo] asc";
            dtDr = DBFactory.GetAllByQuery(ConnString, query);
        }
        private void getdtDrForMonthlyRegister(int year, int month, int days, int shiftID, string shiftName, int UserID)
        {
            string query = string.Empty;
            switch (UserID)
            {
                case 0:
                    query = "SELECT y.[EMachineNumber], y.[EName], y.EnrollNumber 'Userid', ISNULL(CONVERT(VARCHAR(10), CONVERT(DATE, l.[DateTime])), '---') 'Date',"
                            + "ISNULL(MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))), '---') 'TimeIN', ISNULL( CASE "
                            + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN '---'"
                            + "ELSE MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) END, '---') 'TimeOut', CASE "
                            + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN 'Didn''t Clock Out' ELSE '' END 'Remark'"
                            + " FROM tbl_enroll y LEFT JOIN tblLog l ON y.EnrollNumber = l.EnrollNo AND l.[DateTime] >= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-01'"
                            + " AND l.[DateTime] <= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-' + CAST('" + days + "' AS NVARCHAR(2)) + ' 23:59:59'"
                            + " INNER JOIN tbl_shifts s ON y.ShiftId = s.ShiftId WHERE y.FingerNumber IN(0,1,2,3,4,5,11,50) AND s.ShiftId = ' " + shiftID + " ' AND s.ShiftName = '" + shiftName + "'"
                            + " GROUP BY y.[EMachineNumber], y.[EName], y.EnrollNumber, DATEPART(MONTH, l.[DateTime]), CONVERT(DATE, l.[DateTime])"
                            + " ORDER BY y.EnrollNumber ASC;";
                    break;
                default:
                    query = "SELECT y.[EMachineNumber], y.[EName], y.EnrollNumber 'Userid', ISNULL(CONVERT(VARCHAR(10), CONVERT(DATE, l.[DateTime])), '---') 'Date',"
                            + "ISNULL(MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))), '---') 'TimeIN', ISNULL( CASE "
                            + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN '---'"
                            + "ELSE MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) END, '---') 'TimeOut', CASE "
                            + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN 'Didn''t Clock Out' ELSE '' END 'Remark'"
                            + " FROM tbl_enroll y LEFT JOIN tblLog l ON y.EnrollNumber = l.EnrollNo AND l.[DateTime] >= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-01'"
                            + " AND l.[DateTime] <= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-' + CAST('" + days + "' AS NVARCHAR(2)) + ' 23:59:59'"
                            + " INNER JOIN tbl_shifts s ON y.ShiftId = s.ShiftId WHERE y.FingerNumber IN(0,1,2,3,4,5,11,50) AND s.ShiftId = ' " + shiftID + " ' AND s.ShiftName = '" + shiftName + "' AND y.EnrollNumber = " + UserID + " "
                            + " GROUP BY y.[EMachineNumber], y.[EName], y.EnrollNumber, DATEPART(MONTH, l.[DateTime]), CONVERT(DATE, l.[DateTime])"
                            + " ORDER BY y.EnrollNumber ASC;";
                    break;
            }
            //string query = "SELECT y.[EMachineNumber], y.[EName], y.EnrollNumber 'Userid', ISNULL(CONVERT(VARCHAR(10), CONVERT(DATE, l.[DateTime])), '---') 'Date',"
            //                + "ISNULL(MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))), '---') 'TimeIN', ISNULL( CASE "
            //                + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN '---'"
            //                + "ELSE MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) END, '---') 'TimeOut', CASE "
            //                + "WHEN MIN(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, l.[DateTime]) AS VARCHAR(8))) THEN 'Didn''t Clock Out' ELSE '' END 'Remark'"
            //                + " FROM tbl_enroll y LEFT JOIN tblLog l ON y.EnrollNumber = l.EnrollNo AND l.[DateTime] >= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-01'"
            //                + " AND l.[DateTime] <= CAST('" + year + "' AS NVARCHAR(4)) + '-' + RIGHT('0' + CAST('" + month + "' AS NVARCHAR(2)), 2) + '-' + CAST('" + days + "' AS NVARCHAR(2)) + ' 23:59:59'"
            //                + " INNER JOIN tbl_shifts s ON y.ShiftId = s.ShiftId WHERE y.FingerNumber IN(0,1,2,3,4,5,11,50) AND s.ShiftId = ' " + shiftID + " ' AND s.ShiftName = '" + shiftName + "'"
            //                + " GROUP BY y.[EMachineNumber], y.[EName], y.EnrollNumber, DATEPART(MONTH, l.[DateTime]), CONVERT(DATE, l.[DateTime])"
            //                + " ORDER BY y.EnrollNumber ASC;";
            dtDr = DBFactory.GetAllByQuery(ConnString, query);
        }


        public DataTable fillBody(int year, int month, string inTime, string outTime)
        {
            //int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
            int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);

            baseLoop(days, month, year, inTime, outTime);
            getdtDr(year, month, days, inTime, outTime);
            //dynamic[] workRow = new dynamic[days + 3];
            for (int d = 1; d <= days; d++)
            {
                for (int i = 0; i < dtMr.Rows.Count; i++)
                {

                    for (int j = 0; j < dtDr.Rows.Count; j++)
                    {
                        if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[3].ToString()))
                        {
                            //temDay = int.Parse( dtDr.Rows[j].ItemArray[3].ToString().Substring(3,2));
                            DateTime tempDate = Convert.ToDateTime(dtDr.Rows[j].ItemArray[3].ToString());
                            temDay = tempDate.Day;
                        }
                        if (dtMr.Rows[i].ItemArray[1].ToString() == dtDr.Rows[j].ItemArray[2].ToString() && d == temDay)
                        {
                            object[] workRow = new object[days + 4];
                            if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && !String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
                            {
                                workRow[d + 2] = dtDr.Rows[j].ItemArray[4].ToString().Substring(0, 5);
                                workRow[d + 2] += "-";
                                workRow[d + 2] += dtDr.Rows[j].ItemArray[5].ToString().Substring(0, 5);
                            }
                            //int index = workRow.Length - 1;
                            //int overTimeIndex = workRow.Length - 1;
                            //workRow[index] += ;
                            //workRow[overTimeIndex] += dtDr.Rows[j].ItemArray[7].ToString();
                            DataRow tempRow = dtMr.Rows[i];
                            tempRow.ItemArray = workRow;
                            dtMr.Rows[i].ItemArray[d] = workRow;
                            //obj[j+3] = dtDr.Rows[j].ItemArray[6].ToString();
                            //obj[j+3] += " <br>";
                            //obj[j+3] += dtDr.Rows[j].ItemArray[7].ToString();
                            //dtMr.Rows[i].ItemArray[d] = obj;
                            //DataRow temRow = dtMr.Rows[i];
                            //temRow.ItemArray = obj;
                            //dtMr.Rows.Add(obj);
                            //DataRow row;
                            //dynamic[] workRow = new dynamic[days + 3];
                            //dtMr.Rows.Add(workRow);
                            // dtMr.Rows[i].ItemArray[d] = workRow;
                            //dtMr.AcceptChanges();
                            //DataRow workRow;
                            //workRow = dtMr.NewRow();
                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
                            //dtMr.Rows[i].ItemArray[0] = "<br>";
                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
                            //dtMr.Rows[i].ItemArray[0] = "i";
                            //dtMr.Rows[i].ItemArray[0] = "<br>";
                            //dtMr.Rows[i].ItemArray[0] = "usama";
                            //object[] workRow = new Object[days+3];
                            //workRow[count] = dtDr.Rows[j].ItemArray[6].ToString();
                            //workRow[count] += " <br>";
                            //workRow[count] += dtDr.Rows[j].ItemArray[7].ToString();
                            ////dtMr.Rows[i].ItemArray[d]= workRow;
                            //dtMr.Rows[count].ItemArray[d] = workRow;
                            //row = dtMr.NewRow();
                            //row.ItemArray = workRow;
                        }

                    }
                }
            }

            return dtMr;

        }
        public DataTable fillBody(int year, int month, int shiftID, string shiftName, int days)
        {
            //int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);


            baseLoop(days, month, year, shiftID);
            //getdtDr(year, month, days, inTime, outTime);
            getdtDr(year, month, days, shiftID, shiftName);
            //dynamic[] workRow = new dynamic[days + 3];
            for (int d = 1; d <= days; d++)
            {
                for (int i = 0; i < dtMr.Rows.Count; i++)
                {

                    for (int j = 0; j < dtDr.Rows.Count; j++)
                    {
                        if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[3].ToString()))
                        {
                            DateTime tempDate = Convert.ToDateTime(dtDr.Rows[j].ItemArray[3].ToString());
                            temDay = tempDate.Day;
                        }
                        if (dtMr.Rows[i].ItemArray[1].ToString() == dtDr.Rows[j].ItemArray[2].ToString() && d == temDay)
                        {
                            object[] workRow = new object[days + 4];
                            if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && !String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
                            {
                                workRow[d + 2] = dtDr.Rows[j].ItemArray[4].ToString().Substring(0, 5);
                                workRow[d + 2] += "-";
                                workRow[d + 2] += dtDr.Rows[j].ItemArray[5].ToString().Substring(0, 5);
                            }
                            else if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
                            {
                                workRow[d + 2] = dtDr.Rows[j].ItemArray[4].ToString().Substring(0, 5);
                            }
                                //int index = workRow.Length - 1;
                                //int overTimeIndex = workRow.Length - 1;
                                //workRow[index] += ;
                                //workRow[overTimeIndex] += dtDr.Rows[j].ItemArray[7].ToString();
                                DataRow tempRow = dtMr.Rows[i];
                            tempRow.ItemArray = workRow;
                            dtMr.Rows[i].ItemArray[d] = workRow;
                            //obj[j+3] = dtDr.Rows[j].ItemArray[6].ToString();
                            //obj[j+3] += " <br>";
                            //obj[j+3] += dtDr.Rows[j].ItemArray[7].ToString();
                            //dtMr.Rows[i].ItemArray[d] = obj;
                            //DataRow temRow = dtMr.Rows[i];
                            //temRow.ItemArray = obj;
                            //dtMr.Rows.Add(obj);
                            //DataRow row;
                            //dynamic[] workRow = new dynamic[days + 3];
                            //dtMr.Rows.Add(workRow);
                            // dtMr.Rows[i].ItemArray[d] = workRow;
                            //dtMr.AcceptChanges();
                            //DataRow workRow;
                            //workRow = dtMr.NewRow();
                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[6];
                            //dtMr.Rows[i].ItemArray[0] = "<br>";
                            //dtMr.Rows[i].ItemArray[0] = dtDr.Rows[j].ItemArray[7];
                            //dtMr.Rows[i].ItemArray[0] = "i";
                            //dtMr.Rows[i].ItemArray[0] = "<br>";
                            //dtMr.Rows[i].ItemArray[0] = "usama";
                            //object[] workRow = new Object[days+3];
                            //workRow[count] = dtDr.Rows[j].ItemArray[6].ToString();
                            //workRow[count] += " <br>";
                            //workRow[count] += dtDr.Rows[j].ItemArray[7].ToString();
                            ////dtMr.Rows[i].ItemArray[d]= workRow;
                            //dtMr.Rows[count].ItemArray[d] = workRow;
                            //row = dtMr.NewRow();
                            //row.ItemArray = workRow;
                        }

                    }
                }
            }

            return dtMr;

        }
        public DataTable fillAttendanceRegister(int year, int month, int shiftID, string shiftName, int days, int userid)
        {
            DataTable dt = DBFactory.GetAllByQuery(ConnString, $"Select * from tbl_shifts where ShiftId = {shiftID}");
            string shiftStart = dt.Rows[0]["ShiftStart"].ToString();
            string shiftEnd = dt.Rows[0]["ShiftEnd"].ToString();
            Register_baseLoop(days, month, year, shiftID, userid);
            getdtDrForMonthlyRegister(year, month, days, shiftID, shiftName, userid);
            for (int d = 1; d <= days; d++)
            {
                for (int i = 0; i < dtAttendanceRegister.Rows.Count; i++)
                {

                    for (int j = 0; j < dtDr.Rows.Count; j++)
                    {
                        if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[3].ToString()) && dtDr.Rows[j].ItemArray[3].ToString() != "---")
                        {
                            DateTime tempDate = Convert.ToDateTime(dtDr.Rows[j].ItemArray[3].ToString());
                            temDay = tempDate.Day;
                        }
                        else if (dtDr.Rows[j].ItemArray[3].ToString() == "---")
                        {
                            temDay = 1;
                        }
                        if (dtAttendanceRegister.Rows[i].ItemArray[0].ToString() == dtDr.Rows[j].ItemArray[2].ToString() && d == temDay)
                        {
                            object[] workRow = new object[days + 8];
                            if (!String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[4].ToString()) && !String.IsNullOrEmpty(dtDr.Rows[j].ItemArray[5].ToString()))
                            {

                                string inTime = dtDr.Rows[j].ItemArray[4].ToString();
                                string outTime = dtDr.Rows[j].ItemArray[5].ToString();
                                workRow[d + 1] = CalculateRemarks(inTime, outTime, shiftStart, shiftEnd, 30);
                            }
                            DataRow tempRow = dtAttendanceRegister.Rows[i];
                            tempRow.ItemArray = workRow;
                            dtAttendanceRegister.Rows[i].ItemArray[d] = workRow;
                        }

                    }
                }
            }
            foreach (DataRow row in dtAttendanceRegister.Rows)
            {
                int I = 0;
                int P = 0;
                int A = 0;
                int L = 0;
                int E = 0;
                string userID = row[0].ToString();
                for (int i = 1; i <= days; i++)
                {
                    string columnValue = row[i + 1].ToString();
                    if (columnValue == "P")
                    {
                        P += 1;
                    }
                    else if (columnValue == "I")
                    {
                        I += 1;
                    }
                    else if (columnValue == "L")
                    {
                        L += 1;
                    }
                    else if (columnValue == "E")
                    {
                        E += 1;
                    }
                    else if(columnValue == "")
                    {
                        A += 1;
                        row[i+1] = "A";
                    }
                    else
                    {
                        continue;
                    }
                }
                string department = "";
                if (usersDepartments.TryGetValue(userID, out department))
                {
                    row[38] = department;
                }
                row[33] = P.ToString();
                row[34] = A.ToString();
                row[35] = L.ToString();
                row[36] = I.ToString();
                row[37] = E.ToString();
            }

            return dtAttendanceRegister;

        }
        public string CalculateRemarks(string inTime, string outTime, string shiftStart, string shiftEnd, int graceMinutes)
        {
            //if (inTime == "--------" && outTime == "--------")
            //{
            //    return "Absent";
            //}
            if(inTime == "---" && outTime == "---")
            {
                return "A";
            }
            if (outTime == "---")
            {
                return "I";
            }

            if (!string.IsNullOrEmpty(inTime) && !string.IsNullOrEmpty(outTime))
            {
                DateTime inTimeDT = DateTime.Parse(inTime);
                DateTime outTimeDT = DateTime.Parse(outTime);
                DateTime shiftStartDT = DateTime.Parse(shiftStart);
                DateTime shiftEndDT = DateTime.Parse(shiftEnd);

                // Calculate the grace period
                DateTime graceEndTime = shiftStartDT.AddMinutes(graceMinutes);

                // Calculate required working hours based on shift start and shift end
                TimeSpan requiredDuration = shiftEndDT - shiftStartDT;
                TimeSpan workedDuration = outTimeDT - inTimeDT;
                bool isEarlyOut = outTimeDT < shiftEndDT;
                bool isLate = inTimeDT > graceEndTime;
                bool completedWorkingHours = workedDuration >= requiredDuration;

                //if (isLate && completedWorkingHours)
                //{
                //    return "L";
                //}
                //else if (!isLate && !completedWorkingHours)
                ////else
                //{
                //    return "I";
                //}
                //else if (isLate && !completedWorkingHours)
                //{
                //    return "I";
                //}
                //else
                //{
                //    return "P";
                //}
                if (isLate && !isEarlyOut)
                {
                    return "L";
                }
                else if (!isLate && isEarlyOut)
                {
                    return "E";
                }
                else if (isLate && isEarlyOut)
                {
                    return "L";
                }
                else if (!isLate && !isEarlyOut)
                {
                    return "P";
                }

            }

            return "---";
        }
        private void Register_baseLoop(int days, int month, int year, int shiftID, int UserID)
        {
            _ = dtAttendanceRegister.Columns.Add("UserID", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Name", typeof(string));
            for (int i = 1; i <= 31; i++)
            {
                _ = dtAttendanceRegister.Columns.Add("Day" + i.ToString(), typeof(string));
            }
            _ = dtAttendanceRegister.Columns.Add("Present", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Absent", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Late", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Irregular", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Early", typeof(string));
            _ = dtAttendanceRegister.Columns.Add("Department", typeof(string));
            getdtEmployeeByShiftID(shiftID, UserID);
            //My Modification starts from here
            List<int> SaturdayDates = new List<int>();
            List<int> SundayDates = new List<int>();
            for (int day = 1; day <= days; day++)
            {
                DateTime currentDate = new DateTime(year, month, day);
                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    SaturdayDates.Add(day);
                }
                else if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    SundayDates.Add(day);
                }
                else
                {
                    continue;
                }
            }
            //obj = new object[days + 8];
            obj = new object[39];
            for (int i = 0; i < dtEmployeeAttendanceRegister.Rows.Count; i++)
            {
                //obj[0] = i + 1;
                obj[0] = dtEmployeeAttendanceRegister.Rows[i].ItemArray[0].ToString();
                obj[1] = dtEmployeeAttendanceRegister.Rows[i].ItemArray[1].ToString();
                //for (int j = 2; j < days + 8; j++)
                for (int j = 2; j < 31 + 8; j++)

                {

                    int day = j - 1;
                    if (SundayDates.Contains(day) && SundayChecked)
                    {
                        obj[j] = "S".ToString();
                    }
                    else if (SaturdayDates.Contains(day) && SaturdayChecked)
                    {
                        obj[j] = "S".ToString();
                    }
                    else
                    {
                        obj[j] = "".ToString();

                    }
                }
                dtAttendanceRegister.Rows.Add(obj);

            }
        }
        private void getdtEmployeeByShiftID(int shiftID, int UserID)
        {
            //string query1 = "select EnrollNumber,EName from tbl_enroll";
            //string query = $"select   EnrollNumber , EName from tbl_enroll where ShiftId = {shiftID} group by EnrollNumber, EName ";
            string query;
            switch (UserID)
            {
                case 0:
                    query = $"select   EnrollNumber , EName, dept_name from tbl_enroll left join tbl_department on tbl_enroll.DptID = tbl_department.dept_id where ShiftId = {shiftID} group by EnrollNumber, EName, dept_name";
                    break;
                default:
                    query = $"select   EnrollNumber , EName, dept_name from tbl_enroll left join tbl_department on tbl_enroll.DptID = tbl_department.dept_id where ShiftId = {shiftID} and EnrollNumber = {UserID} group by EnrollNumber, EName, dept_name";
                    break;
            }
            //string query = $"select   EnrollNumber , EName, dept_name from tbl_enroll left join tbl_department on tbl_enroll.DptID = tbl_department.dept_id where ShiftId = {shiftID} group by EnrollNumber, EName, dept_name";

            DataTable dt_tmp = DBFactory.GetAllByQuery(ConnString, query);
            int id1, id2;
            int count = dt_tmp.Rows.Count;
            dtEmployeeAttendanceRegister.Columns.Add("UserID", typeof(System.String));
            dtEmployeeAttendanceRegister.Columns.Add("Name", typeof(System.String));
            int c = 1;
            for (int i = 0; i < count; i++)
            {
                string user;
                if (usersDepartments.TryGetValue(dt_tmp.Rows[i].ItemArray[0].ToString(), out user))
                {
                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(dt_tmp.Rows[i].ItemArray[2].ToString()))
                    {
                        //usersDepartments.Add(dt_tmp.Rows[i].ItemArray[0]); dictionary.Add("apple", 1);
                        usersDepartments.Add(dt_tmp.Rows[i].ItemArray[0].ToString(), dt_tmp.Rows[i].ItemArray[2].ToString());
                    }
                }
                if (i + 1 < count)
                {
                    id1 = int.Parse(dt_tmp.Rows[i].ItemArray[0].ToString());
                    id2 = int.Parse(dt_tmp.Rows[i + 1].ItemArray[0].ToString());
                    if (id1 == id2)
                    {
                        c += 1;
                        if (dt_tmp.Rows[i].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtEmployeeAttendanceRegister.Rows.Add(workRow);
                        }
                        else if (dt_tmp.Rows[i + 1].ItemArray[1].ToString() != "")
                        {

                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            string dump = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[0] = dump;
                            workRow[1] = dt_tmp.Rows[i + 1].ItemArray[1].ToString();

                            dtEmployeeAttendanceRegister.Rows.Add(workRow);
                        }
                        else
                        {
                            //DataRow workRow;
                            //workRow = dtE.NewRow();
                            object[] workRow = new object[2];
                            workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                            workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                            dtEmployeeAttendanceRegister.Rows.Add(workRow);
                        }
                        i = i + 1;

                    }
                    else
                    {
                        c += 1;
                        //DataRow workRow;
                        //workRow = dtE.NewRow();
                        object[] workRow = new object[2];
                        workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                        workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                        dtEmployeeAttendanceRegister.Rows.Add(workRow);

                    }
                }
                else
                {
                    c += 1;
                    //DataRow workRow;
                    //workRow = dtE.NewRow();
                    object[] workRow = new object[2];
                    workRow[0] = dt_tmp.Rows[i].ItemArray[0].ToString();
                    workRow[1] = dt_tmp.Rows[i].ItemArray[1].ToString();

                    dtEmployeeAttendanceRegister.Rows.Add(workRow);
                }

            }

            int a = c;
        }



    }
}
