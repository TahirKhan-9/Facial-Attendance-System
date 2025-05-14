//using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.CrystalReports.Engine;
using FaceAttendance.Classes;
using FaceAttendance.DataSets;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UsmanCodeBlocks.Data.Sql.Local;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using UserControl = System.Windows.Controls.UserControl;
using System.Globalization;
//using CrystalReportsReportDefModelLib;
//using SAPBusinessObjects.WPF.Viewer;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucReports.xaml
    /// </summary>
    public partial class ucReports : UserControl
    {
        private int CheckBoxIterator = 0;
        public ObservableCollection<char> Letters { get; set; }
        SqlDataAdapter cmd;
        public string ConnString;
        private SqlConnection sqlcon;
        DataTable dataTable;
        private SqlCommand sqlcmd;
        List<shift> shiftNames;
        private SqlDataAdapter sqlda;
        string shiftID;
        string shiftName;
        string SelectedMonth;
        string SelectedMonthUser;
        string shiftIDUser;
        int UserID;
        string shiftNameUser;
        List<userS> usrList;
        DateTime ShiftStart;
        DateTime ShiftEnd;
        string RadioName = "";
        private DataTable dt;
        ReportDocument rd = new ReportDocument();
        DataTable dt_tmpMR = new DataTable();
        int month;
        int year;
        //ReportDocument report;
        public ucReports(string connstring)
        {
            ConnString = connstring;
            //dpdreport.IsTodayHighlighted = true;
            InitializeComponent();

            Letters = new ObservableCollection<char>
            {
                'A', 'B', 'C', 'D'
            };

            DataContext = this;
        }

        private void startGDReport_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (shiftsCombo.SelectedIndex == -1)
                {
                    CrystalReport.crDR rpt = new CrystalReport.crDR();
                    rpt.Load("CrystalReport/crDR.rpt");
                    if (dpdreport.SelectedDate.HasValue)
                    {
                        rpt.SetDataSource(sqlDs().Tables[0]);
                        DailyRep.ViewerCore.ReportSource = rpt;
                        DailyRep.Visibility = Visibility.Visible;
                        //dbtnExpCvs.Visibility = Visibility.Visible;
                        //dbtnExpEp.Visibility = Visibility.Visible;
                        //dlabelExport.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Date Selected!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(shiftsCombo.SelectedIndex.ToString()))
                    {
                        DataTable selectedShiftData = DBFactory.GetByID(ConnString, "tbl_shifts", Convert.ToInt32(shiftID));
                        DataRow row = selectedShiftData.Rows[0];
                        string shiftStart = row.ItemArray[1].ToString().Substring(0, 5);
                        string shiftEnd = row.ItemArray[2].ToString().Substring(0, 5);
                        CrystalReport.DailyReport rpt = new CrystalReport.DailyReport();
                        CrystalReport.Consolidated ConsolidatedReport = new CrystalReport.Consolidated();

                        //rpt.Load("CrystalReport/crDR.rpt");
                        if (dpdreport.SelectedDate.HasValue)
                        {
                            string reportDate = "As On " + dpdreport.SelectedDate.Value.ToString("dd-MM-yyyy");
                            if (All_CheckBox.IsChecked.Equals(true))
                            {
                                ConsolidatedReport.SetDataSource(sqlDsShift().Tables[0]);
                                ConsolidatedReport.SetParameterValue("headerDate", reportDate);
                                ConsolidatedReport.SetParameterValue("shiftStart", shiftStart);
                                ConsolidatedReport.SetParameterValue("shiftEnd", shiftEnd);
                            }
                            else
                            {
                                rpt.SetDataSource(sqlDsShift().Tables[0]);
                                rpt.SetParameterValue("headerDate", reportDate);
                                rpt.SetParameterValue("shiftStart", shiftStart);
                                rpt.SetParameterValue("shiftEnd", shiftEnd);
                            }
                            
                            //rpt.SetParameterValue("headerContent", "Header Content");
                            if (LateIn.IsChecked.Equals(true))
                            {
                                rpt.SetParameterValue("headerContent", "Late Arrival Report");
                                rpt.SetParameterValue("Remark", "Late Arrival");
                            }
                            else if (EarlyOut.IsChecked.Equals(true))
                            {
                                rpt.SetParameterValue("headerContent", "Early Departure Report");
                                rpt.SetParameterValue("Remark", "Early Departure");
                            }
                            else if (All_CheckBox.IsChecked.Equals(true))
                            {
                                ConsolidatedReport.SetParameterValue("headerContent", "Attendance Report");
                                ConsolidatedReport.SetParameterValue("Remark", "Remarks");
                            }
                            else
                            {
                                rpt.SetParameterValue("headerContent", "Absent Report");
                                rpt.SetParameterValue("Remark", "Remark");
                            }
                            if (All_CheckBox.IsChecked.Equals(true))
                            {
                                DailyRep.ViewerCore.ReportSource = ConsolidatedReport;
                            }
                            else
                            {

                            DailyRep.ViewerCore.ReportSource = rpt;
                            }
                            DailyRep.Visibility = Visibility.Visible;
                            dbtnExpCvs.Visibility = Visibility.Visible;
                            dbtnExpEp.Visibility = Visibility.Visible;
                            dlabelExport.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Date Selected!", false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Get Report ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
            //U:\FaceAttendance Versions\FaceAttendance ver 1.25 Setup - Full -\FaceAttendance\CrystalReport\
            //rd.Load(@"U:\FaceAttendance Versions\FaceAttendance ver 1.25 Setup - Full -\FaceAttendance\CrystalReport\crDR.rpt");
            //rd.SetDataSource(sqlDs());
            //DailyRep.ViewerCore.ReportSource = rd;
            //;

        }


        //        if (dateTimePicker1.Text.Contains("Saturday"))
        //           {
        //               dateTimePicker1.Text = dateTimePicker1.Value.AddDays(2).ToString();
        //    }
        //           else if(dateTimePicker1.Text.Contains("Sunday"))
        //           {
        //               dateTimePicker1.Text = dateTimePicker1.Value.AddDays(1).ToString();
        //}

        private DataSet sqlDs()
        {
            var drDate = dpdreport.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
            SqlConnection con = new SqlConnection(ConnString);
            DataSet1 ds = new DataSet1();
            if (Absent.IsChecked.Equals(true))
            {
                string query = "SELECT * FROM( SELECT e.[EMachineNumber], e.[EName],"
        + "CONVERT(VARCHAR(5), CASE WHEN s.[ShiftStart] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftStart], 108) END) AS 'ShiftStart',"
        + "CONVERT(VARCHAR(5), CASE WHEN s.[ShiftEnd] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftEnd], 108) END) AS 'ShiftEnd',"
        + "e.[EnrollNumber] AS 'Userid',"
        + "'" + drDate + "' AS 'Date',"
        + "'-------' AS 'TimeIN',"
        + "'-------' AS 'TimeOut',"
        + "'Absent' AS 'Remark'"
        + " FROM tbl_enroll e"
        + " LEFT JOIN tbl_shifts s ON e.ShiftId = s.ShiftId"
        + " WHERE e.FingerNumber in (0,1,2,3,4,5,11,50)" 
        + " AND e.EnrollNumber NOT IN( SELECT DISTINCT EnrollNo FROM tblLog WHERE CAST([DateTime] AS DATE) = '" + drDate + "')) AS Subquery"
        + " ORDER BY Subquery.[Date] ASC;";
                //(e.FingerNumber = 50 OR e.FingerNumber = 11)



                cmd = new SqlDataAdapter(query, con);
                con.Open();
                cmd.Fill(ds, "DataTable1");
                con.Close();
                return ds;
            }
            else
            {
                //            string query = " SELECT y.[EMachineNumber],y.[EName],"
                //    + "EnrollNo 'Userid',"
                //   + "'" + drDate + "' 'Date',"
                //    + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(5))) 'TimeIN',"
                //    + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
                //    + "    '' "
                //    + "ELSE"
                //    + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(5)))"
                //    + " END 'TimeOut',"
                //    + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
                //    + "    'Didn''t Clock Out'"
                //    + "ELSE"
                //     + "   '-------' "
                //    + "END 'Remark'"
                //+ " FROM tblLog inner join tbl_enroll y on EnrollNo = y.EnrollNumber  where y.FingerNumber = 50 and[DateTime] >= '" + drDate + "' and[DateTime] <= '" + drDate + " 23:59:59'"
                //+ " GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]) order by[Date] asc";
                string query = "SELECT y.[EMachineNumber], y.[EName], EnrollNo AS 'Userid', '" + drDate + "' AS 'Date',"
                   + "CONVERT(VARCHAR(5), CASE WHEN s.[ShiftStart] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftStart], 108) END) AS 'ShiftStart',"
                   + "CONVERT(VARCHAR(5), CASE WHEN s.[ShiftEnd] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftEnd], 108) END) AS 'ShiftEnd',"
                   + "MIN(CONVERT(VARCHAR(5), CAST([DateTime] AS TIME))) AS 'TimeIN',"
                   + "CASE WHEN MIN(CONVERT(VARCHAR(8), CAST([DateTime] AS TIME))) = MAX(CONVERT(VARCHAR(8), CAST([DateTime] AS TIME))) THEN ''"
                   + "ELSE MAX(CONVERT(VARCHAR(5), CAST([DateTime] AS TIME)))"
                   + "END AS 'TimeOut', CASE "
                   + "WHEN MIN(CONVERT(VARCHAR(8), CAST([DateTime] AS TIME))) = MAX(CONVERT(VARCHAR(8), CAST([DateTime] AS TIME))) THEN 'Didn''t Clock Out'"
                   + "ELSE '-------' END AS 'Remark' FROM tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber "
                   + "LEFT JOIN tbl_shifts s ON y.ShiftId = s.ShiftId WHERE y.FingerNumber in (0,1,2,3,4,5,11,50) AND[DateTime] >= '" + drDate + "'"
                   + "AND[DateTime] <= '" + drDate + " 23:59:59'"
                   + "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo,"
                   + "CASE WHEN s.[ShiftStart] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftStart], 108) END,"
                   + "CASE WHEN s.[ShiftEnd] IS NULL THEN '-------' ELSE CONVERT(VARCHAR(5), s.[ShiftEnd], 108) END"
                   + " ORDER BY 'Date' ASC";
                cmd = new SqlDataAdapter(query, con);
            con.Open();
            cmd.Fill(ds, "DataTable1");
            con.Close();

            return ds;

            }
        
        }
        private DataSet sqlDsShift()
        {
 
            var drDate = dpdreport.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
            SqlConnection con = new SqlConnection(ConnString);
            dsDR dataSet = new dsDR();
            //        string LateInQuery = "SELECT y.[EMachineNumber], y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS  'Date',"
            //+ "MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) 'TimeIN',"
            //+ "CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) THEN '-------'"
            //+ "ELSE MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) END 'TimeOut',"
            //+ "CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) THEN 'Late ' + CAST(DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) AS VARCHAR) + ' min'"
            //+ "ELSE '' END 'Remark' FROM"
            //+ " tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' WHERE "
            //+ "y.FingerNumber = 50 AND[DateTime] >= '" + drDate + "' AND[DateTime] <= '" + drDate + " 23:59:59'"
            //+ "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd]"
            //+ "HAVING MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME))"
            //+ "ORDER BY [Date] ASC";
            string LateInQuery = "SELECT y.[EMachineNumber],y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart',"
                    + "CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date',"
                    + "MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) 'TimeIN', CASE"
                    + " WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8)))"
                    + " THEN '-------' ELSE MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) END 'TimeOut', "
                    + " CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) THEN "
                    + " case when"
                    + " LEN(CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60)) = 1 THEN"
                    + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) / 60) + ' : ' + "
                    + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60) "
                    + " ELSE "
                    + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) / 60) + ' : ' + "
                    + " CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60) "
                    + " end ELSE '' END 'Remark' FROM tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber "
                    + "INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' WHERE y.FingerNumber in (0,1,2,3,4,5,11,50) "
                    + " AND[DateTime] >= '" + drDate + "' AND [DateTime] <= '" + drDate + " 23:59:59' GROUP BY y.[EMachineNumber], y.[EName],"
                    + "EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd] "
                    + "HAVING MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) "
                    + " ORDER BY[Date] ASC;";

            //        string EarlyOutQuery = "SELECT y.[EMachineNumber], y.[EName],CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date',"
            //+ "CONVERT(VARCHAR(5), MIN(CONVERT(TIME, [DateTime])), 108) 'TimeIN',"
            //+ "CASE WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' ELSE CONVERT(VARCHAR(5), MAX(CONVERT(TIME, [DateTime])), 108)"
            //+ "END 'TimeOut',CASE WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN 'Didn''t Clock Out'"
            //+ "WHEN MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME)"
            //+ "THEN 'Early Out ' + CAST(DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) AS VARCHAR) + ' min'"
            //+ "ELSE '-------' END 'Remark' FROM"
            //+ " tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "'"
            //+ "WHERE y.FingerNumber = 50 AND[DateTime] >= '" + drDate + "' AND[DateTime] <= '" + drDate + " 23:59:59'"
            //+ "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd]"
            //+ "HAVING MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) AND MAX(CAST(CONVERT(TIME,[DateTime]) AS DATETIME)) <> MIN(CAST(CONVERT(TIME,[DateTime]) AS DATETIME))"
            //+ "ORDER BY [Date] ASC";
            string EarlyOutQuery = "SELECT y.[EMachineNumber], y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', "
        + "CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date', "
        + "CONVERT(VARCHAR(5), MIN(CONVERT(TIME, [DateTime])), 108) 'TimeIN', CASE "
        + "WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' "
        + "ELSE CONVERT(VARCHAR(5), MAX(CONVERT(TIME, [DateTime])), 108) END 'TimeOut', CASE "
        + "WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' "
        + "WHEN MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) THEN "
        + "case when "
        + "len(CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60)) = 1 then "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) / 60) + ' : ' + "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60) "
        + "else "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) / 60) + ' : ' + "
        + "CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60) end ELSE '-------' "
        + "END 'Remark' FROM tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' "
        + "WHERE y.FingerNumber in (0,1,2,3,4,5,11,50) AND [DateTime] >= '" + drDate + "' AND [DateTime] <= '" + drDate + " 23:59:59' GROUP BY y.[EMachineNumber], "
        + "y.[EName], EnrollNo,DATEPART(MONTH, [DateTime]),CONVERT(DATE, [DateTime]),b.[ShiftStart], b.[ShiftEnd] "
        + "HAVING MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) "
        + "AND MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) <> MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) ORDER BY[Date] ASC;";
            string AbsentQuery = "SELECT * FROM( SELECT e.[EMachineNumber], e.[EName],CONVERT(VARCHAR(5), s.[ShiftStart], 108) AS 'ShiftStart',CONVERT(VARCHAR(5), s.[ShiftEnd], 108) AS 'ShiftEnd', e.[EnrollNumber] AS 'Userid','" + drDate + "' AS 'Date',"
        + "'-------' AS 'TimeIN',"
        + "'-------' AS 'TimeOut',"
        + "'Absent' AS 'Remark' FROM tbl_enroll e LEFT JOIN tbl_shifts s ON e.ShiftId = s.ShiftId WHERE e.FingerNumber in (0,1,2,3,4,5,11,50)"
        + "AND e.EnrollNumber NOT IN(SELECT DISTINCT EnrollNo FROM tblLog WHERE CAST([DateTime] AS DATE) = '" + drDate + "') AND e.ShiftId = " + shiftID + " "
        + ") AS Subquery ORDER BY Subquery.[Date] ASC; ";
    //        if (Overlap_CheckBox.IsChecked.Equals(true))
    //        {
    //            DateTime date = Convert.ToDateTime(drDate);
    //            date = date.AddDays(1);
    //            string UpdatedDate = date.ToString("yyyy-MM-dd");
    //            LateInQuery = "SELECT y.[EMachineNumber], y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS  'Date',"
    //+ "MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) 'TimeIN',"
    //+ "CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) THEN '-------'"
    //+ "ELSE MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) END 'TimeOut',"
    //+ "CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) THEN 'Late ' + CAST(DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) AS VARCHAR) + ' min'"
    //+ "ELSE '' END 'Remark' FROM"
    //+ " tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' WHERE "
    //+ "y.FingerNumber = 50 AND[DateTime] >= '" + drDate + "' AND[DateTime] <= '" + UpdatedDate + " 23:59:59'"
    //+ "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd]"
    //+ "HAVING MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME))"
    //+ "ORDER BY [Date] ASC";
    //            EarlyOutQuery = "SELECT y.[EMachineNumber], y.[EName],CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date',"
    //+ "CONVERT(VARCHAR(5), MIN(CONVERT(TIME, [DateTime])), 108) 'TimeIN',"
    //+ "CASE WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' ELSE CONVERT(VARCHAR(5), MAX(CONVERT(TIME, [DateTime])), 108)"
    //+ "END 'TimeOut',CASE WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN 'Didn''t Clock Out'"
    //+ "WHEN MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME)"
    //+ "THEN 'Early Out ' + CAST(DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) AS VARCHAR) + ' min'"
    //+ "ELSE '-------' END 'Remark' FROM"
    //+ " tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "'"
    //+ "WHERE y.FingerNumber = 50 AND[DateTime] >= '" + drDate + "' AND[DateTime] <= '" + UpdatedDate + " 23:59:59'"
    //+ "GROUP BY y.[EMachineNumber], y.[EName], EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd]"
    //+ "HAVING MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) AND MAX(CAST(CONVERT(TIME,[DateTime]) AS DATETIME)) <> MIN(CAST(CONVERT(TIME,[DateTime]) AS DATETIME))"
    //+ "ORDER BY [Date] ASC";
    //        }


            if (LateIn.IsChecked == true || EarlyOut.IsChecked == true || Absent.IsChecked == true || All_CheckBox.IsChecked == true)
            {

  
                if (LateIn.IsChecked == true)
                {
                    
                    cmd = new SqlDataAdapter(LateInQuery, con);
                    con.Open();
                    _ = cmd.Fill(dataSet, "Daily_Attendance");
                    con.Close();

                    return dataSet;
                }
                else if (EarlyOut.IsChecked == true)
                {
                    cmd = new SqlDataAdapter(EarlyOutQuery, con);
                    con.Open();
                    cmd.Fill(dataSet, "Daily_Attendance");
                    con.Close();
                    return dataSet;
                }
                else if(Absent.IsChecked == true)
                {
                    cmd = new SqlDataAdapter(AbsentQuery, con);
                    con.Open();
                    cmd.Fill(dataSet, "Daily_Attendance");
                    con.Close();
                    return dataSet;
                }
                else
                {
                    Consolidated ConsolidatedDataSet = new Consolidated();
                    DataTable Daily_Attendance = new DataTable();

                    Daily_Attendance.Columns.Add("EMachineNumber", typeof(string));
                    Daily_Attendance.Columns.Add("EName", typeof(string));
                    Daily_Attendance.Columns.Add("ShiftStart", typeof(string));
                    Daily_Attendance.Columns.Add("ShiftEnd", typeof(string));
                    Daily_Attendance.Columns.Add("Userid", typeof(string));
                    Daily_Attendance.Columns.Add("Date", typeof(string));
                    Daily_Attendance.Columns.Add("TimeIN", typeof(string));
                    Daily_Attendance.Columns.Add("TimeOut", typeof(string));
                    Daily_Attendance.Columns.Add("Remark", typeof(string));
                    Daily_Attendance.Columns.Add("GroupBy", typeof(string));
                    Daily_Attendance.TableName = "Daily_Attendance";
                    string LateQuery = "SELECT y.[EMachineNumber],y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', ' Late Arrivals' AS GroupBy,"
                   + "CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date',"
                   + "MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) 'TimeIN', CASE"
                   + " WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(8)))"
                   + " THEN '-------' ELSE MAX(CAST(CONVERT(TIME, [DateTime]) AS VARCHAR(5))) END 'TimeOut', "
                   + " CASE WHEN MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) THEN "
                   + " case when"
                   + " LEN(CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60)) = 1 THEN"
                   + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) / 60) + ' : ' + "
                   + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60) "
                   + " ELSE "
                   + " '0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) / 60) + ' : ' + "
                   + " CONVERT(VARCHAR(2), DATEDIFF(MINUTE, DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)), MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME))) % 60) "
                   + " end ELSE '' END 'Remark' FROM tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber "
                   + "INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' WHERE y.FingerNumber in (0,1,2,3,4,5,11,50)"
                   + " AND[DateTime] >= '" + drDate + "' AND [DateTime] <= '" + drDate + " 23:59:59' GROUP BY y.[EMachineNumber], y.[EName],"
                   + "EnrollNo, DATEPART(MONTH, [DateTime]), CONVERT(DATE, [DateTime]), b.[ShiftStart], b.[ShiftEnd] "
                   + "HAVING MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) >= DATEADD(MINUTE, 15, CAST(b.[ShiftStart] AS DATETIME)) "
                   + " ORDER BY[Date] ASC;";
                    string EarlyQuery = "SELECT y.[EMachineNumber], y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart', ' Early Departures' AS GroupBy,"
        + "CONVERT(VARCHAR(5), b.[ShiftEnd], 108) AS 'ShiftEnd', EnrollNo 'Userid', '" + drDate + "' AS 'Date', "
        + "CONVERT(VARCHAR(5), MIN(CONVERT(TIME, [DateTime])), 108) 'TimeIN', CASE "
        + "WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' "
        + "ELSE CONVERT(VARCHAR(5), MAX(CONVERT(TIME, [DateTime])), 108) END 'TimeOut', CASE "
        + "WHEN MIN(CONVERT(TIME, [DateTime])) = MAX(CONVERT(TIME, [DateTime])) THEN '-------' "
        + "WHEN MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) THEN "
        + "case when "
        + "len(CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60)) = 1 then "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) / 60) + ' : ' + "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60) "
        + "else "
        + "'0' + CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) / 60) + ' : ' + "
        + "CONVERT(VARCHAR(2), DATEDIFF(MINUTE, MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)), CAST(b.[ShiftEnd] AS DATETIME)) % 60) end ELSE '-------' "
        + "END 'Remark' FROM tblLog INNER JOIN tbl_enroll y ON EnrollNo = y.EnrollNumber INNER JOIN tbl_shifts b ON y.ShiftId = '" + shiftID + "' AND b.ShiftName = '" + shiftName + "' "
        + "WHERE y.FingerNumber in (0,1,2,3,4,5,11,50) AND [DateTime] >= '" + drDate + "' AND [DateTime] <= '" + drDate + " 23:59:59' GROUP BY y.[EMachineNumber], "
        + "y.[EName], EnrollNo,DATEPART(MONTH, [DateTime]),CONVERT(DATE, [DateTime]),b.[ShiftStart], b.[ShiftEnd] "
        + "HAVING MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) < CAST(b.[ShiftEnd] AS DATETIME) "
        + "AND MAX(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) <> MIN(CAST(CONVERT(TIME, [DateTime]) AS DATETIME)) ORDER BY[Date] ASC;";
                    string Absentquery = "SELECT * FROM( SELECT e.[EMachineNumber],' Absent' AS GroupBy, e.[EName],CONVERT(VARCHAR(5), s.[ShiftStart], 108) AS 'ShiftStart',CONVERT(VARCHAR(5), s.[ShiftEnd], 108) AS 'ShiftEnd', e.[EnrollNumber] AS 'Userid','" + drDate + "' AS 'Date',"
        + "'-------' AS 'TimeIN',"
        + "'-------' AS 'TimeOut',"
        + "'Absent' AS 'Remark' FROM tbl_enroll e LEFT JOIN tbl_shifts s ON e.ShiftId = s.ShiftId WHERE e.FingerNumber in (0,1,2,3,4,5,11,50)"
        + "AND e.EnrollNumber NOT IN(SELECT DISTINCT EnrollNo FROM tblLog WHERE CAST([DateTime] AS DATE) = '" + drDate + "') AND e.ShiftId = " + shiftID + " "
        + ") AS Subquery ORDER BY Subquery.[Date] ASC; ";

                    DataTable LateIn = DBFactory.GetAllByQuery(ConnString, LateQuery);
                    DataTable EarlyOut = DBFactory.GetAllByQuery(ConnString, EarlyQuery);
                    DataTable Absent = DBFactory.GetAllByQuery(ConnString, Absentquery);
                    //Daily_Attendance.Merge(LateIn);
                    //Daily_Attendance.Merge(EarlyOut);
                    //Daily_Attendance.Merge(Absent);
                    //foreach(DataRow row in LateIn.Rows)
                    //{
                    //    Daily_Attendance.Rows.Add(row.ItemArray);
                    //}
                    foreach (DataRow sourceRow in LateIn.Rows)
                    {
                        DataRow destRow = Daily_Attendance.NewRow();

                        // Map the values from sourceRow to destRow based on column names
                        destRow["EMachineNumber"] = sourceRow["EMachineNumber"];
                        destRow["EName"] = sourceRow["EName"];
                        destRow["ShiftStart"] = sourceRow["ShiftStart"];
                        destRow["ShiftEnd"] = sourceRow["ShiftEnd"];
                        destRow["Userid"] = sourceRow["Userid"];
                        destRow["Date"] = sourceRow["Date"];
                        destRow["TimeIN"] = sourceRow["TimeIN"];
                        destRow["TimeOut"] = sourceRow["TimeOut"];
                        destRow["Remark"] = sourceRow["Remark"];
                        destRow["GroupBy"] = sourceRow["GroupBy"];

                        Daily_Attendance.Rows.Add(destRow);
                    }

                    // Now, Daily_Attendance DataTable contains the mapped data.

                    //foreach (DataRow row in EarlyOut.Rows)
                    //{
                    //    Daily_Attendance.Rows.Add(row.ItemArray);
                    //}
                    foreach(DataRow sourceRow in EarlyOut.Rows)
                    {
                        DataRow destRow = Daily_Attendance.NewRow();
                        destRow["EMachineNumber"] = sourceRow["EMachineNumber"];
                        destRow["EName"] = sourceRow["EName"];
                        destRow["ShiftStart"] = sourceRow["ShiftStart"];
                        destRow["ShiftEnd"] = sourceRow["ShiftEnd"];
                        destRow["Userid"] = sourceRow["Userid"];
                        destRow["Date"] = sourceRow["Date"];
                        destRow["TimeIN"] = sourceRow["TimeIN"];
                        destRow["TimeOut"] = sourceRow["TimeOut"];
                        destRow["Remark"] = sourceRow["Remark"];
                        destRow["GroupBy"] = sourceRow["GroupBy"];
                        Daily_Attendance.Rows.Add(destRow);
                    }
                    //foreach(DataRow row in Absent.Rows)
                    //{
                    //    Daily_Attendance.Rows.Add(row.ItemArray);
                    //}
                    foreach(DataRow sourceRow in Absent.Rows)
                    {
                        DataRow destRow = Daily_Attendance.NewRow();
                        destRow["EMachineNumber"] = sourceRow["EMachineNumber"];
                        destRow["EName"] = sourceRow["EName"];
                        destRow["ShiftStart"] = sourceRow["ShiftStart"];
                        destRow["ShiftEnd"] = sourceRow["ShiftEnd"];
                        destRow["Userid"] = sourceRow["Userid"];
                        destRow["Date"] = sourceRow["Date"];
                        destRow["TimeIN"] = sourceRow["TimeIN"];
                        destRow["TimeOut"] = sourceRow["TimeOut"];
                        destRow["Remark"] = sourceRow["Remark"];
                        destRow["GroupBy"] = sourceRow["GroupBy"];
                        Daily_Attendance.Rows.Add(destRow);
                    }
                    foreach(DataRow row in Daily_Attendance.Rows)
                    {
                        ConsolidatedDataSet.Daily_Attendance.Rows.Add(row.ItemArray);
                    }
                    //dataSet.Tables.Add(Daily_Attendance);
                    return ConsolidatedDataSet;
                    
                    //cmd = new SqlDataAdapter(AllInOneQuery, con);
                    //con.Open();
                    //cmd.Fill(dataSet, "Daily_Attendance");
                    //con.Close();
                    //return dataSet;
                }
            }
            else
            {
                string query = " SELECT y.[EMachineNumber],y.[EName], CONVERT(VARCHAR(5), b.[ShiftStart], 108) AS 'ShiftStart',CONVERT(VARCHAR(5),b.[ShiftEnd], 108) AS 'ShiftEnd',  EnrollNo 'Userid',"
            + "'" + drDate + "' AS 'Date',"
            + "MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(5))) 'TimeIN',"
            + "CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            + "    '' "
            + "ELSE"
            + "    MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8)))"
            + " END 'TimeOut',"
            + " CASE WHEN MIN(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) = MAX(CAST(CONVERT(TIME,[DateTime]) AS VARCHAR(8))) THEN"
            + "    'Didn''t Clock Out'"
            + "ELSE"
            + "   '-------' "
            + "END 'Remark'"
            + " FROM tblLog,tbl_enroll y,tbl_shifts b where EnrollNo = y.EnrollNumber and y.FingerNumber in (0,1,2,3,4,5,11,50) and[DateTime] >= '" + drDate + "' and[DateTime] <= '" + drDate + " 23:59:59' and y.ShiftId = '" + shiftID + "' " +
             "and b.ShiftName = '" + shiftName +
            "'GROUP BY y.[EMachineNumber],y.[EName], EnrollNo,  DATEPART(MONTH,[DateTime]),CONVERT(DATE,[DateTime]), b.[ShiftStart],b.[ShiftEnd] order by[Date] asc";


                
                cmd = new SqlDataAdapter(query, con);
                con.Open();                
                cmd.Fill(dataSet, "Daily_Attendance");
                con.Close();

                return dataSet;

            }


            
            

        }
        private DataSet sqlDaily()
        {

            var drDate = dailyLogDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
            string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";

            SqlConnection con = new SqlConnection(ConnString);
            cmd = new SqlDataAdapter(query, con);
            con.Open();
            dsDR ds = new dsDR();

            cmd.Fill(ds, "Daily_Attendance");
            con.Close();
            return ds;

        }
        private void dexportCVS()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = sqlDs().Tables[0].Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in sqlDs().Tables[0].Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                    ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(Directory.GetCurrentDirectory() + "\\output.csv", sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }
        private void dexportexcel()
        {
            try
            {
                //D_Wait.Visibility = Visibility.Visible;
                object[] obj = new object[sqlDs().Tables[0].Columns.Count];
                for (int j = 0; j < sqlDs().Tables[0].Columns.Count; j++)
                {
                    obj[j] = "";
                }

                sqlDs().Tables[0].Rows.Add(obj);
                sqlDs().Tables[0].Rows.Add(obj);
                try
                {
                    bool res = ExcelFunc.Write_Excel(sqlDs().Tables[0]);

                    if (res)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exported Successfully!\n", false);
                        message.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( GetExcelReport  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
        }
        private void Logxportexcel()
        {
            try
            {
                //D_Wait.Visibility = Visibility.Visible;
                //object[] obj = new object[dt.Columns.Count];
                //for (int j = 0; j < dt.Columns.Count; j++)
                //{
                //    obj[j] = "";
                //}

                //dt.Rows.Add(obj);
                //dt.Rows.Add(obj);
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        bool res = ExcelFunc.Write_Excel(dt);
                        if (res)
                        {
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exported Successfully!\n", false);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "First Select Any Report!\n", false);
                        message.ShowDialog();
                    }

                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( GetExcelReport  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
        }
        private void exportexcel()
        {


            object[] obj = new object[dt_tmpMR.Columns.Count];
            for (int j = 0; j < dt_tmpMR.Columns.Count; j++)
            {
                obj[j] = "";
            }

            dt_tmpMR.Rows.Add(obj);
            dt_tmpMR.Rows.Add(obj);


            try
            {
                bool res = ExcelFunc.Write_Excel(dt_tmpMR);
                if (res)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exported Successfully!\n", false);
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }
        private void exportCVS()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string[] columnNames = dt_tmpMR.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt_tmpMR.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                    ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(Directory.GetCurrentDirectory() + "\\output.csv", sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }
        private void btn_MRep(object sender, RoutedEventArgs e)
        {
            LoadingWindow loader = new LoadingWindow();
            try
            {
                MonthlyRepo mr = new MonthlyRepo(ConnString);
                DateTime dpm = dpmreport.DisplayDate;//SelectedDate.ToString();

                int month, year;

                SqlDataAdapter sda = new SqlDataAdapter();

                if (string.IsNullOrEmpty(inTime.Text))
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter IN TIME", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Keyboard.Focus(inTime);
                    return;
                }
                if (string.IsNullOrEmpty(outTime.Text))
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter OUT TIME", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Keyboard.Focus(outTime);
                    return;
                }
                //if (string.IsNullOrEmpty(maxoutTime.Text))
                //{
                //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter MAX OUT TIME", false);
                //    message.btnCancel.Visibility = Visibility.Collapsed;
                //    message.btnOk.Content = "Ok";
                //    message.ShowDialog();
                //    Keyboard.Focus(maxoutTime);
                //    return;
                //}&& IsDateTime(maxoutTime.Text)

                if (dpmreport.SelectedDate.HasValue && IsDateTime(inTime.Text) && IsDateTime(outTime.Text))
                {
                    loader.Show();
                    //year = int.Parse(dpm.Substring(0, 4));
                    //month = int.Parse(dpm.Substring(5));
                    year = dpm.Year;
                    month = dpm.Month;
                    dt_tmpMR = mr.fillBody(year, month, inTime.Text, outTime.Text);
                    dsMR dm = new dsMR();
                    dm.Tables.Add(dt_tmpMR);
                    CrystalReport.crMR rpt = new CrystalReport.crMR();
                    rpt.SetDataSource(dm.Tables[1]);
                    string timings = inTime.Text + "-" + outTime.Text;
                    rpt.SetParameterValue("remark", cusShiftCombo.Text);
                    rpt.SetParameterValue("shiftTimings", timings);

                    MonthlyRep.ViewerCore.ReportSource = rpt;
                    MonthlyRep.Visibility = Visibility.Visible;
                    //btnExpCvs.Visibility = Visibility.Visible;
                    //btnExpEp.Visibility = Visibility.Visible;
                    //labelExport.Visibility = Visibility.Visible;
                    inTime.Clear();
                    outTime.Clear();
                    maxoutTime.Clear();
                    txtdemoIn.Visibility = Visibility.Visible;
                    txtdemoOut.Visibility = Visibility.Visible;
                    txtdemoMaxTimeOut.Visibility = Visibility.Visible;
                    dpmreport.SelectedDate = null;
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Month Selected!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref loader);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Generate  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);

            }
            HideLoaderWithCondition(ref loader);


        }
        private void HideLoaderWithCondition(ref LoadingWindow ld)
        {
            if (ld.ShowActivated)
            {
                ld.Hide();
            }
        }
        //private void btn_MRep(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        MonthlyRepo mr = new MonthlyRepo(ConnString);
        //        DateTime dpm = dpmreport.DisplayDate;//SelectedDate.ToString();

        //        int month, year;

        //        SqlDataAdapter sda = new SqlDataAdapter();

        //        if (string.IsNullOrEmpty(inTime.Text))
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter IN TIME", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //            Keyboard.Focus(inTime);
        //            return;
        //        }
        //        if (string.IsNullOrEmpty(outTime.Text))
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter OUT TIME", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //            Keyboard.Focus(outTime);
        //            return; 
        //        }
        //        //if (string.IsNullOrEmpty(maxoutTime.Text))
        //        //{
        //        //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter MAX OUT TIME", false);
        //        //    message.btnCancel.Visibility = Visibility.Collapsed;
        //        //    message.btnOk.Content = "Ok";
        //        //    message.ShowDialog();
        //        //    Keyboard.Focus(maxoutTime);
        //        //    return;
        //        //}&& IsDateTime(maxoutTime.Text)

        //        if (dpmreport.SelectedDate.HasValue && IsDateTime(inTime.Text) && IsDateTime(outTime.Text) )
        //        {
        //            //year = int.Parse(dpm.Substring(0, 4));
        //            //month = int.Parse(dpm.Substring(5));
        //            year = dpm.Year;
        //            month = dpm.Month;
        //            dt_tmpMR = mr.fillBody(year, month,inTime.Text,outTime.Text);
        //            dsMR dm = new dsMR();
        //            dm.Tables.Add(dt_tmpMR);
        //            CrystalReport.crMR rpt = new CrystalReport.crMR();
        //            rpt.SetDataSource(dm.Tables[1]);
        //            string timings = inTime.Text + "-" + outTime.Text;
        //            rpt.SetParameterValue("remark", cusShiftCombo.Text);
        //            rpt.SetParameterValue("shiftTimings", timings);

        //            MonthlyRep.ViewerCore.ReportSource = rpt;
        //            MonthlyRep.Visibility = Visibility.Visible;
        //            //btnExpCvs.Visibility = Visibility.Visible;
        //            //btnExpEp.Visibility = Visibility.Visible;
        //            //labelExport.Visibility = Visibility.Visible;
        //            inTime.Clear();
        //            outTime.Clear();
        //            maxoutTime.Clear();
        //            txtdemoIn.Visibility = Visibility.Visible;
        //            txtdemoOut.Visibility = Visibility.Visible;
        //            txtdemoMaxTimeOut.Visibility = Visibility.Visible;
        //            dpmreport.SelectedDate = null;
        //        }
        //        else
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Month Selected!", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //        string message = "( " + DateTime.Now + " ) ( Generate  ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //        File.AppendAllText(path, message);

        //    }


        //}
        private void clearst_click(object sender, MouseButtonEventArgs e)
        {
            txtdemoIn.Visibility = Visibility.Collapsed;
        }
        public static bool IsDateTime(string dt)
        {
            DateTime tmpDt;
            return DateTime.TryParse(dt, out tmpDt);
        }
        private void export_Excel(object sender, RoutedEventArgs e)
        {
            Thread backgroundThread = new Thread(new ThreadStart(exportexcel));
            // Start thread  
            backgroundThread.Start();

            //exportexcel();
        }



        private void export_Cvs(object sender, RoutedEventArgs e)
        {
            exportCVS();

            //Thread backgroundThread = new Thread(new ThreadStart(exportCVS));
            //// Start thread  
            //backgroundThread.Start();
        }

        private void dexport_Excel(object sender, RoutedEventArgs e)
        {
            try
            {
                dexportexcel();
              
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( btnExport  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
            // }));
        }

        private void dexport_Cvs(object sender, RoutedEventArgs e)
        {
            //Thread backfroundThread = new Thread(new ThreadStart(dexportCVS));
            //backfroundThread.Start();
            dexportCVS();
        }
        private void fillDatagrid()
        {
            try
            {
                //string spQuer = "select VisitorName,CNIC,CheckInTime,CheckOutTime,RFID from dbo.Visitors where (CheckOutTime is null or CheckOutTime = '') and (CheckInTime= DATEADD(day, 0, CAST(GETDATE() AS date)))order by CheckInTime desc";
                var drDate = dailyLogDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");

                //string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                //string demoQuery = "select * from [dbo].[v_DailyLogTesting] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                string demoQuery = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                using (sqlcon = new SqlConnection(ConnString))
                {
                    //sqlcmd = new SqlCommand(query, sqlcon);
                    sqlcmd = new SqlCommand(demoQuery, sqlcon);
                    sqlda = new SqlDataAdapter(sqlcmd);
                    dt = new DataTable("[dbo].[v_dailyLog]");
                    int rows = sqlda.Fill(dt);
                    if (rows > 0)
                    {
                        //CrystalReport Dail = new CrystalReport.crDR();
                        //dailLogGrid.ItemsSource = dt.DefaultView;
                        sqlcon.Close();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) ( DailyLog FillDataGrid  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }
            //string query = "select VisitorName,CNIC,CheckInTime,CheckOutTime from dbo.Visitors where CheckInTime= CAST(CURRENT_TIMESTAMP AS DATE)";
        }
        private void fillDatagrid(string query)
        {
            try
            {
                //string spQuer = "select VisitorName,CNIC,CheckInTime,CheckOutTime,RFID from dbo.Visitors where (CheckOutTime is null or CheckOutTime = '') and (CheckInTime= DATEADD(day, 0, CAST(GETDATE() AS date)))order by CheckInTime desc";
                //var monstDate = startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
                //string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                using (sqlcon = new SqlConnection(ConnString))
                {
                    sqlcmd = new SqlCommand(query, sqlcon);
                    sqlda = new SqlDataAdapter(sqlcmd);
                    dt = new DataTable("[dbo].[v_dailyLog]");
                    int rows = sqlda.Fill(dt);
                    if (rows > 0)
                    {
                        //monthLogGrid.ItemsSource = dt.DefaultView;
                        sqlcon.Close();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) ( MonthlyLog FillDataGrid  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }
            //string query = "select VisitorName,CNIC,CheckInTime,CheckOutTime from dbo.Visitors where CheckInTime= CAST(CURRENT_TIMESTAMP AS DATE)";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadingWindow loader = new LoadingWindow();
            if (string.IsNullOrEmpty(dailyLogDate.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Starting Date", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Keyboard.Focus(dailyLogDate);
                return;
            }
            fillDatagrid(ref loader);
            try
            {
                if (dt.Rows.Count != 0)
                {
                    //DailyRep.Visibility = Visibility.Visible;
                    //dailbtnExpEp.Visibility = Visibility.Visible;
                    //dbtnExpEp.Visibility = Visibility.Visible;
                    //dlabelExport.Visibility = Visibility.Visible;
                    //CrystalReport.DailyLog rpt = new CrystalReport.DailyLog();
                    //ReportDocument cryRpt = new ReportDocument();
                    //cryRpt.Load(@"DailyLog.rep");

                    DailyLog crystalReport1 = new DailyLog();
                    crystalReport1.Load(@"DailyLog.rep");
                    //dt.TableName = "[dbo].[v_dailyLog]";
                    crystalReport1.SetDataSource(dt);
                    dailRep.ViewerCore.ReportSource = crystalReport1;
                    // crystalReport1.Refresh();
                    //string path = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "DailyLogRep.xls";//UPDATE Visitors SET CheckOutTime = '" + dt + "' WHERE id =" + a.ID + "","./Reports/" + "'+time+'" + "VisitorReport.pdf"
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "No Data Exists", false, true);
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref loader);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( btnVisReport ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
            //try
            //{

            //    DailyLog rpt = new DailyLog();
            //    if (dailyLogDate.SelectedDate.HasValue)
            //    {
            //        rpt.SetDataSource(sqlDaily().Tables[0]);
            //        DailyRep.ViewerCore.ReportSource = rpt;
            //        DailyRep.Visibility = Visibility.Visible;
            //        dbtnExpCvs.Visibility = Visibility.Visible;
            //        dbtnExpEp.Visibility = Visibility.Visible;
            //        dlabelExport.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Date Selected!", false);
            //        message.btnCancel.Visibility = Visibility.Collapsed;
            //        message.btnOk.Content = "Ok";
            //        message.ShowDialog();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
            //    string message = "( " + DateTime.Now + " ) ( Get Report ) ( " + ex.ToString() + " )" + Environment.NewLine;
            //    File.AppendAllText(path, message);
            //}
            HideLoaderWithCondition(ref loader);
        }
        private void fillDatagrid(ref LoadingWindow ld)
        {
            try
            {
                ld.Show();
                //string spQuer = "select VisitorName,CNIC,CheckInTime,CheckOutTime,RFID from dbo.Visitors where (CheckOutTime is null or CheckOutTime = '') and (CheckInTime= DATEADD(day, 0, CAST(GETDATE() AS date)))order by CheckInTime desc";
                var drDate = dailyLogDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");

                string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                using (sqlcon = new SqlConnection(ConnString))
                {
                    sqlcmd = new SqlCommand(query, sqlcon);
                    sqlda = new SqlDataAdapter(sqlcmd);
                    dt = new DataTable("[dbo].[v_dailyLog]");
                    int rows = sqlda.Fill(dt);
                    if (rows > 0)
                    {
                        //CrystalReport Dail = new CrystalReport.crDR();
                        //dailLogGrid.ItemsSource = dt.DefaultView;
                        sqlcon.Close();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref ld);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) ( DailyLog FillDataGrid  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }
            //string query = "select VisitorName,CNIC,CheckInTime,CheckOutTime from dbo.Visitors where CheckInTime= CAST(CURRENT_TIMESTAMP AS DATE)";
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(dailyLogDate.Text))
        //    {
        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Starting Date", false);
        //        message.btnCancel.Visibility = Visibility.Collapsed;
        //        message.btnOk.Content = "Ok";
        //        message.ShowDialog();
        //        Keyboard.Focus(dailyLogDate);
        //        return;
        //    }
        //    fillDatagrid();
        //    try
        //    {
        //        if (dt.Rows.Count != 0)
        //        {
        //            //DailyRep.Visibility = Visibility.Visible;
        //            //dailbtnExpEp.Visibility = Visibility.Visible;
        //            //dbtnExpEp.Visibility = Visibility.Visible;
        //            //dlabelExport.Visibility = Visibility.Visible;
        //            //CrystalReport.DailyLog rpt = new CrystalReport.DailyLog();
        //            //ReportDocument cryRpt = new ReportDocument();
        //            //cryRpt.Load(@"DailyLog.rep");

        //            DailyLog crystalReport1 = new DailyLog();
        //            crystalReport1.Load(@"DailyLog.rep");
        //            dt.TableName = "[dbo].[v_dailyLog]";
        //            crystalReport1.SetDataSource(dt);
        //            dailRep.ViewerCore.ReportSource = crystalReport1;
        //            // crystalReport1.Refresh();
        //            //string path = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "DailyLogRep.xls";//UPDATE Visitors SET CheckOutTime = '" + dt + "' WHERE id =" + a.ID + "","./Reports/" + "'+time+'" + "VisitorReport.pdf"
        //        }
        //        else
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "No Data Exists", false, true);
        //            message.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //        string message = "( " + DateTime.Now + " ) ( btnVisReport ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //        File.AppendAllText(path, message);
        //    }
        //    //try
        //    //{

        //    //    DailyLog rpt = new DailyLog();
        //    //    if (dailyLogDate.SelectedDate.HasValue)
        //    //    {
        //    //        rpt.SetDataSource(sqlDaily().Tables[0]);
        //    //        DailyRep.ViewerCore.ReportSource = rpt;
        //    //        DailyRep.Visibility = Visibility.Visible;
        //    //        dbtnExpCvs.Visibility = Visibility.Visible;
        //    //        dbtnExpEp.Visibility = Visibility.Visible;
        //    //        dlabelExport.Visibility = Visibility.Visible;
        //    //    }
        //    //    else
        //    //    {
        //    //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "No Date Selected!", false);
        //    //        message.btnCancel.Visibility = Visibility.Collapsed;
        //    //        message.btnOk.Content = "Ok";
        //    //        message.ShowDialog();
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //    //    string message = "( " + DateTime.Now + " ) ( Get Report ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //    //    File.AppendAllText(path, message);
        //    //}
        //}

        private void dailbtnExpEp_Click(object sender, RoutedEventArgs e)
        {
            Thread backgroundThread = new Thread(new ThreadStart(Logxportexcel));
            // Start thread  
            backgroundThread.Start();
            //dailyLogxportexcel();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadingWindow loader = new LoadingWindow();
            if (string.IsNullOrEmpty(startDate.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Starting Date", false, true);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Keyboard.Focus(startDate);
                return;
            }
            if (string.IsNullOrEmpty(endDate.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Ending Date", false, true);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Keyboard.Focus(endDate);
                return;
            }
            var monstDate = startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
            var monsendDate = endDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
            string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + monstDate + "' and [DateTime] <= '" + monsendDate + " 23:59:59'";
            fillDatagrid(query, ref loader);
            try
            {
                if (dt.Rows.Count != 0)
                {
                    //DailyRep.Visibility = Visibility.Visible;
                    //monthbtnExpEp.Visibility = Visibility.Visible;
                    //dbtnExpEp.Visibility = Visibility.Visible;
                    //dlabelExport.Visibility = Visibility.Visible;
                    MonthlyLog crystalReport1 = new MonthlyLog();
                    crystalReport1.Load(@"DailyLog.rep");
                    dt.TableName = "[dbo].[v_dailyLog]";
                    crystalReport1.SetDataSource(dt);
                    monthLogGrid.ViewerCore.ReportSource = crystalReport1;
                    //crystalReport1.Load(@"DailyLog.rep");
                    //crystalReport1.SetDataSource(dt);
                    //viewer.ViewerCore.ReportSource = crystalReport1;
                    //crystalReport1.Refresh();
                    //string path = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "DailyLogRep.xls";//UPDATE Visitors SET CheckOutTime = '" + dt + "' WHERE id =" + a.ID + "","./Reports/" + "'+time+'" + "VisitorReport.pdf"
                }
                else
                {
                    HideLoaderWithCondition(ref loader);
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "No Data Exists", false, true);
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref loader);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( btnVisReport ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
            HideLoaderWithCondition(ref loader);

        }
        private void fillDatagrid(string query, ref LoadingWindow ld)
        {
            ld.Show();
            try
            {
                //string spQuer = "select VisitorName,CNIC,CheckInTime,CheckOutTime,RFID from dbo.Visitors where (CheckOutTime is null or CheckOutTime = '') and (CheckInTime= DATEADD(day, 0, CAST(GETDATE() AS date)))order by CheckInTime desc";
                //var monstDate = startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
                //string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + drDate + "' and [DateTime] <= '" + drDate + " 23:59:59'" + " order by DateTime desc ";
                using (sqlcon = new SqlConnection(ConnString))
                {
                    sqlcmd = new SqlCommand(query, sqlcon);
                    sqlda = new SqlDataAdapter(sqlcmd);
                    dt = new DataTable("[dbo].[v_dailyLog]");
                    int rows = sqlda.Fill(dt);
                    if (rows > 0)
                    {
                        //monthLogGrid.ItemsSource = dt.DefaultView;
                        sqlcon.Close();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref ld);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) ( MonthlyLog FillDataGrid  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }
            //string query = "select VisitorName,CNIC,CheckInTime,CheckOutTime from dbo.Visitors where CheckInTime= CAST(CURRENT_TIMESTAMP AS DATE)";
        }
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(startDate.Text))
        //    {
        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Starting Date", false, true);
        //        message.btnCancel.Visibility = Visibility.Collapsed;
        //        message.btnOk.Content = "Ok";
        //        message.ShowDialog();
        //        Keyboard.Focus(startDate);
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(endDate.Text))
        //    {
        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select Ending Date", false, true);
        //        message.btnCancel.Visibility = Visibility.Collapsed;
        //        message.btnOk.Content = "Ok";
        //        message.ShowDialog();
        //        Keyboard.Focus(endDate);
        //        return;
        //    }
        //    var monstDate = startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
        //    var monsendDate = endDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
        //    //string query = "select * from [dbo].[v_dailyLog] where [DateTime] >= '" + monstDate + "' and [DateTime] <= '" + monsendDate + " 23:59:59'";
        //    string query = "select * from [dbo].[v_DailyLogTesting] where [DateTime] >= '" + monstDate + "' and [DateTime] <= '" + monsendDate + " 23:59:59'";
        //    fillDatagrid(query);
        //    try
        //    {
        //        if (dt.Rows.Count != 0)
        //        {
        //            //DailyRep.Visibility = Visibility.Visible;
        //            //monthbtnExpEp.Visibility = Visibility.Visible;
        //            //dbtnExpEp.Visibility = Visibility.Visible;
        //            //dlabelExport.Visibility = Visibility.Visible;
        //            MonthlyLog crystalReport1 = new MonthlyLog();
        //            crystalReport1.Load(@"DailyLog.rep");
        //            dt.TableName = "[dbo].[v_dailyLog]";
        //             crystalReport1.SetDataSource(dt);
        //            monthLogGrid.ViewerCore.ReportSource = crystalReport1;
        //            //crystalReport1.Load(@"DailyLog.rep");
        //            //crystalReport1.SetDataSource(dt);
        //            //viewer.ViewerCore.ReportSource = crystalReport1;
        //            //crystalReport1.Refresh();
        //            //string path = Directory.GetCurrentDirectory() + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "DailyLogRep.xls";//UPDATE Visitors SET CheckOutTime = '" + dt + "' WHERE id =" + a.ID + "","./Reports/" + "'+time+'" + "VisitorReport.pdf"
        //        }
        //        else
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "No Data Exists", false, true);
        //            message.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //        string message = "( " + DateTime.Now + " ) ( btnVisReport ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //        File.AppendAllText(path, message);
        //    }

        //}

        private void monthbtnExpEp_Click(object sender, RoutedEventArgs e)
        {
            Thread backgroundThread = new Thread(new ThreadStart(Logxportexcel));
            // Start thread  
            backgroundThread.Start();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dataTable = DBFactory.GetAllByQuery(ConnString, "SELECT * FROM tbl_shifts");

            //UdP =
            //Obs<UdownDisp>(dt);
            shiftNames = Globals.ConvertDataTable<shift>(dataTable).ToList();
            shiftsCombo.ItemsSource = shiftNames.Select(x => x.ShiftName);
            
            Monthly_ShiftCombo.ItemsSource = shiftNames.Select(x => x.ShiftName);
            Monthly_ShiftCombo1.ItemsSource = shiftNames.Select(x => x.ShiftName);
            dpdreport.SelectedDate = DateTime.Now;
            LateIn.IsEnabled = false;
            EarlyOut.IsEnabled = false;
            All_CheckBox.IsEnabled = false;
            Overlap_CheckBox.IsEnabled = false;
            CheckBoxLabel.PreviewMouseDown -= CheckBoxLabel_PreviewMouseDown;
            OverlapCheckBoxLabel.PreviewMouseDown -= OverlapLabel_PreviewMouseDown;
            //shiftsCombo.ItemsSource = shiftNames.Select(x => x.ShiftName);

        }
        bool EventsAttached = false;
        private void shiftsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(shiftsCombo.SelectedIndex != -1)
            {
            shiftID = dataTable.Rows[shiftsCombo.SelectedIndex].ItemArray[0].ToString();
            shiftName = shiftsCombo.SelectedItem.ToString();
            refresh.Visibility = Visibility.Visible;
                LateIn.IsEnabled = true;
                EarlyOut.IsEnabled = true;
                All_CheckBox.IsEnabled = true;
                Overlap_CheckBox.IsEnabled = true;
                if (!EventsAttached)
                {
                    CheckBoxLabel.PreviewMouseDown += CheckBoxLabel_PreviewMouseDown;
                    OverlapCheckBoxLabel.PreviewMouseDown += OverlapLabel_PreviewMouseDown;
                    EventsAttached = true;
                }
            }
            else
            {
                LateIn.IsEnabled = false;
                EarlyOut.IsEnabled = false;
                All_CheckBox.IsEnabled = false;
                Overlap_CheckBox.IsEnabled = false;
                CheckBoxLabel.PreviewMouseDown -= CheckBoxLabel_PreviewMouseDown;
                OverlapCheckBoxLabel.PreviewMouseDown -= OverlapLabel_PreviewMouseDown;
                EventsAttached = false;
               
            }
        }

        private void dailShifRep_Loaded(object sender, RoutedEventArgs e)
        {
            shiftsCombo.Text = "---Select Shift---";
        }

        private void outTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtdemoOut.Visibility = Visibility.Collapsed;
        }

        private void maxoutTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtdemoMaxTimeOut.Visibility = Visibility.Collapsed;
        }

        private void inTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtdemoIn.Visibility = Visibility.Collapsed;
        }

        private void outTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtdemoOut.Visibility = Visibility.Collapsed;
        }

        private void maxoutTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtdemoMaxTimeOut.Visibility = Visibility.Collapsed;
        }

        private void inTime_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdemoIn.Visibility = Visibility.Collapsed;
        }

        private void outTime_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdemoOut.Visibility = Visibility.Collapsed;
        }

        private void maxoutTime_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdemoMaxTimeOut.Visibility = Visibility.Collapsed;
        }
        int count = 0;
        RadioButton lastSelected;
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(lastSelected==null?"":lastSelected.Name);
            count++;
            RadioButton selected = sender as RadioButton;
            if(selected != null && selected.IsChecked == true)
            {
                RadioName = selected.Name.ToString();
                //MessageBox.Show(RadioName);
            }
            //if(lastSelected != null && count > 0)
            //{
            //if(selected.IsChecked == true && count%2==0 && lastSelected==selected)
            //{
            //    selected.IsChecked = false;
            //}

            //}
            CheckBoxIterator = 2;
            
        }

        
        private void Radio_Clicked(object sender, RoutedEventArgs e)
        {
            if (All_CheckBox.IsChecked.Equals(true))
            {
            CheckBoxIterator++;
            All_CheckBox.IsChecked = false;
            }
            RadioButton origin = sender as RadioButton;
            if(origin.IsChecked == true && count % 2 == 1 && lastSelected == origin)
            {
                origin.IsChecked = false;
                count++;
            }
            if(count % 2 == 0)
            {
                count++;
            }
            
            lastSelected = origin;
        }

        private void All_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LateIn.IsChecked = false;
            EarlyOut.IsChecked = false;
            Absent.IsChecked = false;
        }
        private void CheckBoxLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            All_CheckBox.IsChecked = CheckBoxIterator % 2 == 0;
            CheckBoxIterator++;
            return;
        }

        private void refresh_MouseEnter(object sender, MouseEventArgs e)
        {
            refresh.Opacity = 0.7;
        }

        private void refresh_MouseLeave(object sender, MouseEventArgs e)
        {
            refresh.Opacity = 1;
        }

        private void refresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            shiftsCombo.SelectedIndex = -1;
            shiftsCombo.Text = "---Select Shift---";
            refresh.Visibility = Visibility.Collapsed;
            All_CheckBox.IsChecked = false;
            CheckBoxIterator = 0;
            LateIn.IsChecked = false;
            EarlyOut.IsChecked = false;
            Absent.IsChecked = false;
            Overlap_CheckBox.IsChecked = false;

        }
        private void OverlapLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Overlap_CheckBox.IsChecked.Equals(true))
            {
                Overlap_CheckBox.IsChecked = false;
            }
            else
            {
                Overlap_CheckBox.IsChecked = true;
            }
        }
        private void RotateImage()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 360,    // Rotate to 360 degrees (full circle)
                Duration = TimeSpan.FromSeconds(2),  // Set the duration of the animation
                RepeatBehavior = RepeatBehavior.Forever // Repeat the animation indefinitely
            };

            RotateTransform rotateTransform = D_1Wait1.RenderTransform as RotateTransform;
            if (rotateTransform != null)
            {
                // Begin the animation
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
            }
        }
        private void StopAndResetImageRotation()
        {
            RotateTransform rotateTransform = D_1Wait1.RenderTransform as RotateTransform;

            if (rotateTransform != null)
            {
                // Stop the animation
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);

                // Reset the image to its default position
                rotateTransform.Angle = 0;
            }
        }


        private void SaturdayLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaturdayOff_CheckBox.IsChecked = !SaturdayOff_CheckBox.IsChecked.Equals(true);
        }

        private void SundayLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SundayOff_CheckBox.IsChecked = !SundayOff_CheckBox.IsChecked.Equals(true);
        }
        private void GenerateMonthly_Click(object sender, RoutedEventArgs e)
        {
            LoadingWindow loader = new LoadingWindow();
            try

            {

                MonthlyRepo MonthlyReport = new MonthlyRepo(ConnString, (bool)SaturdayOff_CheckBox.IsChecked, (bool)SundayOff_CheckBox.IsChecked);

                int month, year;

                SqlDataAdapter sda = new SqlDataAdapter();

                if (MonthCombo.SelectedIndex == -1)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Month", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else
                {
                    month = MonthCombo.SelectedIndex + 1;
                }
                if (Monthly_ShiftCombo.SelectedIndex == -1)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Shift", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                string YearText = YearInput.Text;
                if (YearText[0] != '2' || YearText[1] != '0' || YearText.Length < 4)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Correct Date", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else
                {
                    year = Convert.ToInt32(YearText);
                }




                loader.Show();
                if ((bool)rad_WithTime.IsChecked)
                {
                    int ShiftID = Convert.ToInt32(shiftID);
                    int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
                    dt_tmpMR = MonthlyReport.fillBody(year, month, ShiftID, shiftName, days);
                    dsMR dm = new dsMR();
                    dm.Tables.Add(dt_tmpMR);
                    CrystalReport.MonthlyReport rpt = new CrystalReport.MonthlyReport();
                    rpt.SetDataSource(dm.Tables[1]);
                    int daysDifference = 31 - days;
                    for (int i = 1; i <= days; i++)
                    {
                        DateTime specificDate = new DateTime(year, month, i);
                        string paramValue = specificDate.DayOfWeek.ToString().Substring(0, 3) + '-' + i.ToString();
                        string paramName = "Day" + i.ToString();
                        rpt.SetParameterValue(paramName, paramValue);
                    }
                    if (daysDifference > 0)
                    {
                        for (int i = 1; i <= daysDifference; i++)
                        {
                            rpt.SetParameterValue("Day" + (i + days).ToString(), "");
                        }
                    }
                    DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", Convert.ToInt32(shiftID));
                    string shiftTiming;

                    if (ShiftInfo.Rows.Count > 0)
                    {
                        DataRow row = ShiftInfo.Rows[0];
                        shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
                    }
                    else
                    {
                        shiftTiming = "";
                    }
                    rpt.SetParameterValue("remark", shiftName);
                    rpt.SetParameterValue("shiftTimings", shiftTiming);
                    rpt.SetParameterValue("ReportingMonth", SelectedMonth);
                    MonthlyReport2.ViewerCore.ReportSource = rpt;
                    MonthlyReport2.Visibility = Visibility.Visible;
                }
                else
                {
                    int ShiftID = Convert.ToInt32(shiftID);
                    int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
                    dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftName, days, 0);
                    dsSummaryReportLSF dm = new dsSummaryReportLSF();
                    dm.Tables.Add(dt_tmpMR);
                    CrystalReport.MonthlySummaryReportLSF rpt = new CrystalReport.MonthlySummaryReportLSF();
                    rpt.SetDataSource(dm.Tables[1]);
                    DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", Convert.ToInt32(shiftID));
                    string shiftTiming;

                    if (ShiftInfo.Rows.Count > 0)
                    {
                        DataRow row = ShiftInfo.Rows[0];
                        shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
                    }
                    else
                    {
                        shiftTiming = "";
                    }
                    rpt.SetParameterValue("ShiftName", shiftName);
                    rpt.SetParameterValue("Shift Timing", shiftTiming);
                    rpt.SetParameterValue("ReportingMonth", $"{SelectedMonth} - {YearInput.Text}");
                    MonthlyReport2.ViewerCore.ReportSource = rpt;
                    MonthlyReport2.Visibility = Visibility.Visible;
                }

                //btnExpCvs.Visibility = Visibility.Visible;
                //btnExpEp.Visibility = Visibility.Visible;
                //labelExport.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref loader);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Generate  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);

            }
            HideLoaderWithCondition(ref loader);


        }

        //private void GenerateMonthly_Click(object sender, RoutedEventArgs e)
        //{
        //    try

        //    {

        //        MonthlyRepo MonthlyReport = new MonthlyRepo(ConnString, (bool)SaturdayOff_CheckBox.IsChecked, (bool)SundayOff_CheckBox.IsChecked);

        //        int month, year;

        //        SqlDataAdapter sda = new SqlDataAdapter();

        //        if (MonthCombo.SelectedIndex == -1)
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Month", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //            return;
        //        }
        //        else
        //        {
        //            month = MonthCombo.SelectedIndex + 1;
        //        }
        //        if (Monthly_ShiftCombo.SelectedIndex == -1)
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Shift", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //            return;
        //        }
        //        string YearText = YearInput.Text;
        //        if(YearText[0] != '2' || YearText[1] != '0' || YearText.Length < 4)
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Correct Date", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //            return;
        //        }
        //        else
        //        {
        //            year = Convert.ToInt32(YearText);
        //        }




        //        int ShiftID = Convert.ToInt32(shiftID);
        //        int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
        //        dt_tmpMR = MonthlyReport.fillBody(year, month, ShiftID, shiftName, days);
        //        dsMR dm = new dsMR();
        //        dm.Tables.Add(dt_tmpMR);
        //        CrystalReport.MonthlyReport rpt = new CrystalReport.MonthlyReport();
        //        rpt.SetDataSource(dm.Tables[1]);
        //        int daysDifference = 31 - days;
        //        for (int i = 1; i <= days; i++)
        //        {
        //            DateTime specificDate = new DateTime(year, month, i);
        //            string paramValue = specificDate.DayOfWeek.ToString().Substring(0, 3) + '-' + i.ToString();
        //            string paramName = "Day" + i.ToString();
        //            rpt.SetParameterValue(paramName, paramValue);
        //        }
        //        if(daysDifference > 0)
        //        {
        //            for(int i = 1; i <= daysDifference; i++)
        //            {
        //                rpt.SetParameterValue("Day" + (i + days).ToString(), "");
        //            }
        //        }
        //        DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", Convert.ToInt32(shiftID));
        //        string shiftTiming;

        //        if(ShiftInfo.Rows.Count > 0)
        //        {
        //            DataRow row = ShiftInfo.Rows[0];
        //            shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
        //        }
        //        else
        //        {
        //            shiftTiming = "";
        //        }
        //        rpt.SetParameterValue("remark", shiftName);
        //        rpt.SetParameterValue("shiftTimings", shiftTiming);
        //        rpt.SetParameterValue("ReportingMonth", SelectedMonth);
        //        MonthlyReport2.ViewerCore.ReportSource = rpt;
        //        MonthlyReport2.Visibility = Visibility.Visible;
        //        //btnExpCvs.Visibility = Visibility.Visible;
        //        //btnExpEp.Visibility = Visibility.Visible;
        //        //labelExport.Visibility = Visibility.Visible;
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //        string message = "( " + DateTime.Now + " ) ( Generate  ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //        File.AppendAllText(path, message);

        //    }


        //}


        private void MonthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            month = MonthCombo.SelectedIndex + 1;
            string Year = DateTime.Now.Year.ToString();
            //YearInput.Text = "2023";
            YearInput.Text = Year;
            YearInput.Visibility = Visibility.Visible;
            year = Convert.ToInt32(YearInput.Text);
            SelectedMonth = ((ComboBoxItem)MonthCombo.SelectedItem).Content.ToString();
        }
        private void MonthCombo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            month = MonthCombo1.SelectedIndex + 1;
            string Year = DateTime.Now.Year.ToString();
            //YearInput.Text = "2023";
            YearInput1.Text = Year;
            YearInput1.Visibility = Visibility.Visible;
            year = Convert.ToInt32(YearInput1.Text);
            SelectedMonthUser = ((ComboBoxItem)MonthCombo1.SelectedItem).Content.ToString();
        }
        private void YearInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "^[0-9]+$"))
            {
                e.Handled = true;
            }
            if(e.Text.Contains(" "))
            {
                e.Handled = true;
            }

            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length >= 4 && e.Text.Length > 0)
            {
                e.Handled = true; 
            }
        }
        private void YearInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void Monthly_ShiftCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            shiftID = dataTable.Rows[Monthly_ShiftCombo.SelectedIndex].ItemArray[0].ToString();
            shiftName = Monthly_ShiftCombo.SelectedItem.ToString();
        }

        private void Monthly_ShiftCombo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            shiftIDUser = dataTable.Rows[Monthly_ShiftCombo1.SelectedIndex].ItemArray[0].ToString();
            shiftNameUser = Monthly_ShiftCombo1.SelectedItem.ToString();
            if (Monthly_ShiftCombo1.SelectedIndex != -1)
            {
                //string deptID = deptTab.Rows[dept2.SelectedIndex].ItemArray[0].ToString();
                //ReportingDept2 = deptTab.Rows[dept2.SelectedIndex].ItemArray[2].ToString();
                string query = $"select EnrollNumber ,EName  from tbl_enroll where ShiftId = '{int.Parse(shiftIDUser)}' group by EnrollNumber, EName";
                DataTable dt = DBFactory.GetAllByQuery(ConnString, query);
                usrList = Globals.ConvertDataTable<userS>(dt);
                cmbUsers.ItemsSource = usrList.Select(x => $"{x.EName.Trim()} - {x.EnrollNumber}");
                cmbUsers.SelectedIndex = -1;
                UserID = 0;
                cmbUsers.Text = "Select User";
                // Attach the KeyUp event to the TextBox within the ComboBox
                var textBox = (TextBox)cmbUsers.Template.FindName("PART_EditableTextBox", cmbUsers);
                if (textBox != null)
                {
                    textBox.KeyUp += TextBox_KeyUp;
                }
            }
        }
        //private async void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    var textBox = sender as TextBox;
        //    List<userS> filteredList = new List<userS>();
        //    if (textBox != null)
        //    {
        //        int id = 0;
        //        switch(int.TryParse(textBox.Text, out id))
        //        {
        //            case true:
        //                await Task.Run(() =>{
        //                    filteredList = usrList.Where(user => user.EnrollNumber == id).ToList();
        //                });
        //                break;
        //            case false:
        //                await Task.Run(() =>{
        //                    filteredList = usrList.Where(user => user.EName.ToLower().Contains(textBox.Text.ToLower())).ToList();
        //                });
        //                break;
        //        }
        //        cmbUsers.ItemsSource = filteredList.Select(x => $"{x.EName.Trim()} - {x.EnrollNumber}");
        //        cmbUsers.IsDropDownOpen = true;
        //        textBox.SelectionStart = textBox.Text.Length;
        //    }
        //}
        private async void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            List<userS> filteredList = new List<userS>();
            string searchText = textBox.Text.ToLower();

            if (textBox != null)
            {
                int id = 0;
                switch (int.TryParse(textBox.Text, out id))
                {
                    case true:
                        // Perform the filtering on a background thread
                        filteredList = await Task.Run(() =>
                        {
                            return usrList.Where(user => user.EnrollNumber == id).ToList();
                        });
                        break;
                    case false:
                        // Perform the filtering on a background thread
                        filteredList = await Task.Run(() =>
                        {
                            return usrList.Where(user => user.EName.ToLower().Contains(searchText)).ToList();
                        });
                        break;
                }

                // Update the UI on the UI thread
                cmbUsers.Dispatcher.Invoke(() =>
                {
                    cmbUsers.ItemsSource = filteredList.Select(x => $"{x.EName.Trim()} - {x.EnrollNumber}");
                    cmbUsers.IsDropDownOpen = true;
                });

                // Set the caret position on the UI thread
                textBox.Dispatcher.Invoke(() =>
                {
                    textBox.SelectionStart = textBox.Text.Length;
                });
            }
        }

        private void cmbUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            if (cmbUsers.Text == "Select User")
            {
                cmbUsers.Text = string.Empty;
                if (!cmbUsers.IsDropDownOpen)
                {
                    cmbUsers.IsDropDownOpen = true;
                }

            }
        }

        private void cmbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUsers.SelectedIndex != -1)
            {
                string text = cmbUsers.SelectedItem.ToString();
                UserID = int.Parse(text.Split('-')[1].Trim());
            }
            else
            {
                UserID = 0;
            }
        }

        private void GenerateMonthly1_Click(object sender, RoutedEventArgs e)
        {
            LoadingWindow loader = new LoadingWindow();
            try

            {

                MonthlyRepo MonthlyReport = new MonthlyRepo(ConnString, (bool)SaturdayOff_CheckBox1.IsChecked, (bool)SundayOff_CheckBox1.IsChecked);

                int month, year;

                SqlDataAdapter sda = new SqlDataAdapter();

                if (MonthCombo1.SelectedIndex == -1)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Month", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else
                {
                    month = MonthCombo1.SelectedIndex + 1;
                }
                if (Monthly_ShiftCombo1.SelectedIndex == -1)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Shift", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                string YearText = YearInput1.Text;
                if (YearText[0] != '2' || YearText[1] != '0' || YearText.Length < 4)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Correct Date", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else
                {
                    year = Convert.ToInt32(YearText);
                }

                loader.Show();
                int ShiftID = Convert.ToInt32(shiftIDUser);
                int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
                if(cmbUsers.SelectedIndex == -1)
                {
                    dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftNameUser, days, 0);

                }
                else
                {
                    dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftNameUser, days, UserID);
                }
                dsSummaryReportLSF dm = new dsSummaryReportLSF();
                dm.Tables.Add(dt_tmpMR);
                CrystalReport.UserPerformance rpt = new CrystalReport.UserPerformance();
                rpt.SetDataSource(dm.Tables[1]);
                DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", ShiftID);
                string shiftTiming;
                if (ShiftInfo.Rows.Count > 0)
                {
                    DataRow row = ShiftInfo.Rows[0];
                    shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
                }
                else
                {
                    shiftTiming = "";
                }
                rpt.SetParameterValue("ShiftName", shiftNameUser);
                rpt.SetParameterValue("Shift Timing", shiftTiming);
                rpt.SetParameterValue("ReportingMonth", $"{SelectedMonthUser} - {YearInput1.Text}");
                MonthlyReport3.ViewerCore.ReportSource = rpt;
                MonthlyReport3.Visibility = Visibility.Visible;
                //if (cmbUsers.SelectedIndex != -1)
                //{
                //    int ShiftID = Convert.ToInt32(shiftIDUser);
                //    int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
                //    dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftNameUser, days, 0);
                //    //dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftName, days);
                //    dsSummaryReportLSF dm = new dsSummaryReportLSF();
                //    dm.Tables.Add(dt_tmpMR);
                //    CrystalReport.UserPerformance rpt = new CrystalReport.UserPerformance();
                //    rpt.SetDataSource(dm.Tables[1]);
                //    int daysDifference = 31 - days;
                //    for (int i = 1; i <= days; i++)
                //    {
                //        DateTime specificDate = new DateTime(year, month, i);
                //        string paramValue = specificDate.DayOfWeek.ToString().Substring(0, 3) + '-' + i.ToString();
                //        string paramName = "Day" + i.ToString();
                //        rpt.SetParameterValue(paramName, paramValue);
                //    }
                //    if (daysDifference > 0)
                //    {
                //        for (int i = 1; i <= daysDifference; i++)
                //        {
                //            rpt.SetParameterValue("Day" + (i + days).ToString(), "");
                //        }
                //    }
                //    DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", Convert.ToInt32(shiftID));
                //    string shiftTiming;

                //    if (ShiftInfo.Rows.Count > 0)
                //    {
                //        DataRow row = ShiftInfo.Rows[0];
                //        shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
                //    }
                //    else
                //    {
                //        shiftTiming = "";
                //    }
                //    rpt.SetParameterValue("remark", shiftName);
                //    rpt.SetParameterValue("shiftTimings", shiftTiming);
                //    rpt.SetParameterValue("ReportingMonth", SelectedMonth);
                //    MonthlyReport2.ViewerCore.ReportSource = rpt;
                //    MonthlyReport2.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    int ShiftID = Convert.ToInt32(shiftIDUser);
                //    int days = CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);
                //    dt_tmpMR = MonthlyReport.fillAttendanceRegister(year, month, ShiftID, shiftNameUser, days, 0);
                //    dsSummaryReportLSF dm = new dsSummaryReportLSF();
                //    dm.Tables.Add(dt_tmpMR);
                //    CrystalReport.UserPerformance rpt = new CrystalReport.UserPerformance();
                //    rpt.SetDataSource(dm.Tables[1]);
                //    DataTable ShiftInfo = DBFactory.GetByID(ConnString, "tbl_shifts", ShiftID);
                //    string shiftTiming;
                //    if (ShiftInfo.Rows.Count > 0)
                //    {
                //        DataRow row = ShiftInfo.Rows[0];
                //        shiftTiming = row["ShiftStart"].ToString().Substring(0, 5) + "-" + row["ShiftEnd"].ToString().Substring(0, 5);
                //    }
                //    else
                //    {
                //        shiftTiming = "";
                //    }
                //    rpt.SetParameterValue("ShiftName", shiftNameUser);
                //    rpt.SetParameterValue("Shift Timing", shiftTiming);
                //    rpt.SetParameterValue("ReportingMonth", $"{SelectedMonthUser} - {YearInput1.Text}");                    
                //    MonthlyReport3.ViewerCore.ReportSource = rpt;
                //    MonthlyReport3.Visibility = Visibility.Visible;
                //}
            }
            catch (Exception ex)
            {
                HideLoaderWithCondition(ref loader);
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Generate  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);

            }
            HideLoaderWithCondition(ref loader);
        }

        private void SundayLabel1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SundayOff_CheckBox1.IsChecked = !SundayOff_CheckBox1.IsChecked.Equals(true);
        }

        private void SaturdayLabel1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaturdayOff_CheckBox1.IsChecked = !SaturdayOff_CheckBox1.IsChecked.Equals(true);
        }
    }

}

