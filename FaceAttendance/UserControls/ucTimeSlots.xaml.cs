using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UsmanCodeBlocks.Data.Sql.Local;

public struct PasstimeInfo
{
    public byte bSHour;
    public byte bSMinute;
    public byte bEHour;
    public byte bEMinute;

}

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucTimeSlots.xaml
    /// </summary>
    public partial class ucTimeSlots : UserControl

    {
        private string ConString;
        public ObservableCollection<gdevId> gId;
        private ObservableCollection<tbl_TimeSlots> TimeSlotsList;
        private ObservableCollection<tbl_TimeSlotsDay> DaySlotsList;
        private bool BtnModifiedDayClicked;
        private bool BtnModifiedWeekClicked;
        //private List<UdownDisp> usersLst;
        private ObservableCollection<UdownDisp> EnD;

 



        public ucTimeSlots(string conString)
        {
            InitializeComponent();
            ConString = conString;
        }

        private void btnGetUserCtrl_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnClearAllUserCtrl_Click(object sender, RoutedEventArgs e)
        {
            if (Devlist.SelectedItems.Count > 0 && UsersList.Items.Count > 0)
            {
                bool Deleted = true;
                foreach (gdevId Device in Devlist.SelectedItems)
                {
                    int DeviceId = Device.MId;
                    string DeviceIp = Device.Ip;
                    List<int> Users_List = new List<int>();
                    foreach (UdownDisp user in UsersList.Items)
                    {
                        Users_List.Add(user.EnrollNumber);
                    }
                    List<int> MachineUsers_List = GetEnrolledUserIDs(DeviceId, DeviceIp);
                    foreach (int MachineUserID in MachineUsers_List)
                    {
                        if (Users_List.Contains(MachineUserID))
                        {
                            Deleted = DelUserControl(DeviceId, DeviceIp, MachineUserID) ? true && Deleted : false;
                        }
                    }
                }
                if (Deleted)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Delete All User Control Success!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    TaskCompleted();
                }
                else
                {
                    TaskNotCompleted();
                }
            }
            else
            {
                TaskNotCompleted();
            }

        }
        private void btnDelUserCtrl_Click(object sender, RoutedEventArgs e)
        {
            if (Devlist.SelectedItems.Count > 0)
            {
                bool Deleted = true;
                foreach (gdevId Device in Devlist.SelectedItems)
                {
                    string Ip = Device.Ip;
                    int DeviceId = Device.MId;
                    List<int> UserID_List = new List<int>();
                    foreach (UdownDisp user in selecteduserlist.Items)
                    {
                        UserID_List.Add(user.EnrollNumber);
                    }
                    List<int> MachineUserID_List = GetEnrolledUserIDs(DeviceId, Ip);
                    foreach (int UserID in UserID_List)
                    {
                        if (MachineUserID_List.Contains(UserID))
                        {
                            Deleted = DelUserControl(DeviceId, Ip, UserID) ? true && Deleted : false;
                        }
                    }
                }
                if (Deleted)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Delete User Control Success!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    TaskCompleted();
                }
                else 
                {
                    TaskNotCompleted();
                }
            }
            else
            {
                TaskNotCompleted();
            }
            
            //DelUserControl(int DeviceId, string ip, int EnrollNumber)





        }

        private void btnSetUserCtrl_Click(object sender, RoutedEventArgs e)
        {

            DateTime TransactionDate = DateTime.Now;
            DateTime? StartDate = startDate.SelectedDate;
            DateTime? EndDate = endDate.SelectedDate;
            int TimeSlotID = 0;
            

            if (StartDate.HasValue && EndDate.HasValue && selecteduserlist.Items.Count > 0 && TimeSlots.SelectedItems.Count == 1 && Devlist.SelectedItems.Count > 0)
            {

                foreach (gdevId item in Devlist.SelectedItems)
                {
                    int DeviceID = item.MId;
                    string IP = item.Ip;
                    List<int> UserID_List = new List<int>();
                    foreach (UdownDisp user in selecteduserlist.Items)
                    {
                        UserID_List.Add(user.EnrollNumber);
                    }    
                    bool bret = DisableDevice(IP, DeviceID);
                    if (EnableDevice(DeviceID))
                    {
                        bool DayWriteSuccess = true;
                        bool WeekWriteSuccess = false;
                        bool SetUserControlSuccess = true;
                        foreach (tbl_TimeSlots timeslot in TimeSlots.SelectedItems)
                        {
                            TimeSlotID = timeslot.ID;

                            DataTable dt = DBFactory.GetAllByQuery(ConString, "SELECT * FROM tbl_TimeSlotsWeek Where ID = '" + timeslot.ID + "'");
                            List<tbl_TimeSlotsWeek> weekData = ConvertDataTable<tbl_TimeSlotsWeek>(dt);
                            List<int> DaysList = new List<int>();
                            int Week_ID = 0;
                            foreach (tbl_TimeSlotsWeek x in weekData)
                            {
                                DaysList.Add(Convert.ToInt32(x.Monday));
                                DaysList.Add(Convert.ToInt32(x.Tuesday));
                                DaysList.Add(Convert.ToInt32(x.Wednesday));
                                DaysList.Add(Convert.ToInt32(x.Thursday));
                                DaysList.Add(Convert.ToInt32(x.Friday));
                                DaysList.Add(Convert.ToInt32(x.Saturday));
                                DaysList.Add(Convert.ToInt32(x.Sunday));
                                Week_ID = x.ID;

                            }
                           
                            List<int> DistintDaysList = DaysList.Distinct().ToList();

                            
                            foreach (int DayID in DistintDaysList)
                            {
                                if (DayID > 0)
                                {
                                    if (!Machine_dayWriteSet(DayID, DeviceID, IP))
                                    {
                                        DayWriteSuccess = false;
                                        break; // Exit the loop as soon as a failure is encountered
                                    }
                                }
                                
                            }

                            
                            if (Week_ID > 0)
                            {
                                WeekWriteSuccess = WeekWriteSet(Week_ID, DeviceID, IP);
                            }
                            List<int> MachineUserID_List = GetEnrolledUserIDs(DeviceID, IP);
                            
                            if (DayWriteSuccess && WeekWriteSuccess)
                            {
                                foreach (UdownDisp selected in selecteduserlist.Items)
                                {
                                    if (MachineUserID_List.Contains(selected.EnrollNumber))
                                    {
                                        SetUserControlSuccess = SetUserControlSuccess && SetUserCtrl(selected.EnrollNumber, Week_ID, DeviceID, StartDate.Value, EndDate.Value);
                                    }
                                }
                            }
                            
                            
                           
                            

                        }
                        if (SetUserControlSuccess)
                        {
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Set User Control Success!", false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                            bool SavedToDB = true;
                            foreach (UdownDisp user in selecteduserlist.Items)
                            {
                                
                                try
                                {
                                    if (DBFactory.ConnectServer(ConString))
                                    {
                                       
                                        bool saved = DBFactory.BulkInsert(ConString, "Insert into tbl_UsersTimezone(Device_ID,Name,Start_Date,End_Date,Trans_Date,Timezone_ID)VALUES(" + DeviceID + ",'" + user.EName + "','" + StartDate + "','" + EndDate + "','" + TransactionDate + "','" + TimeSlotID + "')");
                                        SavedToDB = SavedToDB && saved;

                                    }
                                }
                                catch (Exception)
                                {
                                    TaskNotCompleted();

                                }
                            }
                            if (SavedToDB)
                            {
                                TaskCompleted();
                            }
                        }
                        
                    }
                    else
                    {
                        TaskNotCompleted();
                        return;
                    }
                    

                }
                
            }
            else
            {

                TaskNotCompleted();
            }

        }



        private void btnWeekReadSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReadSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPasstimeReadSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPasstimeWriteSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnWriteSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnWeekWriteSet_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnSaveDay_Click(object sender, RoutedEventArgs e)
        {

            if (DBFactory.ConnectServer(ConString))
            {
                int dayID = 0;
                foreach (tbl_TimeSlotsDay record in TimeSlotsDay.SelectedItems)
                {
                    dayID = record.ID;
                }
                string ST_1 = $"{textStartTime_Hour1.Text}:{textStartTime_Minute1.Text}";
                string ST_2 = $"{textStartTime_Hour2.Text}:{textStartTime_Minute2.Text}";
                string ST_3 = $"{textStartTime_Hour3.Text}:{textStartTime_Minute3.Text}";
                string ST_4 = $"{textStartTime_Hour4.Text}:{textStartTime_Minute4.Text}";
                string ST_5 = $"{textStartTime_Hour5.Text}:{textStartTime_Minute5.Text}";
                string ET_1 = $"{textEndTime_Hour1.Text}:{textEndTime_Minute1.Text}";
                string ET_2 = $"{textEndTime_Hour2.Text}:{textEndTime_Minute2.Text}";
                string ET_3 = $"{textEndTime_Hour3.Text}:{textEndTime_Minute3.Text}";
                string ET_4 = $"{textEndTime_Hour4.Text}:{textEndTime_Minute4.Text}";
                string ET_5 = $"{textEndTime_Hour5.Text}:{textEndTime_Minute5.Text}";
                if (dayID > 0)
                {
                    bool SaveToDB = DBFactory.Update(ConString, "UPDATE tbl_TimeSlotsDay SET ST_1='" + ST_1 + "',ST_2='" + ST_2 + "',ST_3='" + ST_3 + "',ST_4='" + ST_4 + "',ST_5='" + ST_5 + "',ET_1= '" + ET_1 + "',ET_2='" + ET_2 + "',ET_3='" + ET_3 + "',ET_4='" + ET_4 + "',ET_5='" + ET_5 + "' where ID = '" + dayID + "'");
                    if (SaveToDB)
                    {
                        TaskCompleted();
                    }
                    else
                    {
                        TaskNotCompleted();
                    }
                }
                else
                {
                    TaskNotCompleted();
                }


            }
            else
            {
                TaskNotCompleted();
            }
            TimeSlotsDay.SelectedItem = null;
            ClearDayFields();
            DisableDayCategory();
            DaySlots_list();
            BtnModifiedDayClicked = false;
        }

        private void ucTimeSlots_Loaded(object sender, RoutedEventArgs e)
        {

            DisableDayCategory();
            DisableWeekCategory();

        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == "ShiftId")
                    {
                        continue;
                    }
                    if (pro.Name == "DptID")
                    {
                        continue;
                    }
                    if (pro.Name == column.ColumnName)
                    {
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    }

                    else {
                        continue;
                    }

                }
            }
            return obj;
        }
        private static ObservableCollection<T> ConvertDataTableObs<T>(DataTable dt)
        {
            ObservableCollection<T> data = new ObservableCollection<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);

                data.Add(item);
            }
            return data;
        }
        //public void dev_list()
        //{
        //    //Devlist.ItemsSource = null;
        //    gId = new ObservableCollection<gdevId>();//List<gdevId>();
        //    DataTable dt = new DataTable();
        //    bool con;
        //    con = DBFactory.ConnectServer(ConString);
        //    if (con == true)
        //    {
        //        dt = DBFactory.GetAllByQuery(ConString, "SELECT  [MId],[Ip] from tbl_dev ");
        //        gId = ConvertDataTableObs<gdevId>(dt);
        //        Devlist.ItemsSource = gId;

        //    }
        //    else
        //    {
        //        Devlist.ItemsSource = null;
        //    }
        //}
        public void dev_list()
        {
            //Devlist.ItemsSource = null;
            gId = new ObservableCollection<gdevId>();//List<gdevId>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(ConString);
            if (con)
            {
                //dt = DBFactory.GetAllByQuery(connstring, "SELECT  [MId],[Ip] from tbl_dev ");
                dt = DBFactory.GetAllByQuery(ConString, "SELECT * from tbl_dev");
                //gId = ConvertDataTableObs<gdevId>(dt);
                gId = new ObservableCollection<gdevId>();
                // this is temporary
                //Loglist.ItemsSource = gId;
                ObservableCollection<DevInfo> devList = ConvertDataTableObs<DevInfo>(dt);
                foreach (DevInfo dev in devList)
                {
                    string serial = dev.Serial;
                    string hwpin = dev.HWpin;
                    if (Dev_Validation.match_serial(serial, hwpin))
                    {
                        gdevId obj = new gdevId()
                        {
                            MId = dev.MId,
                            Ip = dev.Ip
                        };
                        gId.Add(obj);
                    }
                }
                Devlist.ItemsSource = gId;

            }
            else
            {
                Devlist.ItemsSource = null;
            }
        }
        public void TimeSlots_list() {
            TimeSlotsList = new ObservableCollection<tbl_TimeSlots>();
            DataTable dt = new DataTable();
            if (DBFactory.ConnectServer(ConString))
            {
                dt = DBFactory.GetAllByQuery(ConString, "SELECT * from tbl_TimeSlots");
                TimeSlotsList = ConvertDataTableObs<tbl_TimeSlots>(dt);
                TimeSlots.ItemsSource = TimeSlotsList;
                TimeSlotsFirstTab.ItemsSource = TimeSlotsList;


            }
            else {
                TimeSlots.ItemsSource = null;

            }


        }
        public void DaySlots_list()
        {
            DaySlotsList = new ObservableCollection<tbl_TimeSlotsDay>();
            DataTable dt = new DataTable();
            if (DBFactory.ConnectServer(ConString))
            {
                dt = DBFactory.GetAllByQuery(ConString, "SELECT * from tbl_TimeSlotsDay");
                DaySlotsList = ConvertDataTableObs<tbl_TimeSlotsDay>(dt);
                TimeSlotsDay.ItemsSource = DaySlotsList;
            }
            else
            {
                TimeSlotsDay.ItemsSource = null;
            }
        }

        private void devIdOnload(object sender, RoutedEventArgs e)
        {
            //load the devId here on list.
            dev_list();
            TimeSlots_list();
            DaySlots_list();
            //DbU_List();
            //UD_List();
            //DelU_List();
            //UnB_List();
            //BU_List();

        }
        private void MoveAllBack(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in selecteduserlist.Items)
            {
                EnD.Add(item);

            }
            selecteduserlist.Items.Clear();
            ObservableCollection<UdownDisp> Sorted = new ObservableCollection<UdownDisp>(EnD.OrderBy(x => x.SrNo));
            EnD.Clear();
            foreach (UdownDisp item in Sorted)
            {
                EnD.Add(item);
            }
        }
        private void MoveAllRight(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in UsersList.Items)
            {

                if (item.FingerNumber == 11)
                    item.type = "Card";
                else if (item.FingerNumber == 0)
                    item.type = "Finger";
                else if (item.FingerNumber == 50)
                    item.type = "Face";
                //dbuserlist.Items.Add(item);

                selecteduserlist.Items.Add(item);
                //dbuserlist.Items.Remove(item);
            }

            EnD.Clear();


        }
        private void MoveSelectedBack(object sender, MouseButtonEventArgs e)
        {
            ObservableCollection<UdownDisp> IterateThrough = new ObservableCollection<UdownDisp>();
            foreach (UdownDisp item in selecteduserlist.SelectedItems)
            {
                IterateThrough.Add(item);
            }
            foreach (UdownDisp item in IterateThrough)
            {

                EnD.Add(item);
                selecteduserlist.Items.Remove(item);
            }
            ObservableCollection<UdownDisp> Sorted = new ObservableCollection<UdownDisp>(EnD.OrderBy(x => x.SrNo));
            EnD.Clear();
            foreach (UdownDisp item in Sorted)
            {
                EnD.Add(item);
            }
        }
        private void MoveSelectedRight(object sender, MouseButtonEventArgs e)
        {
            var selectedItems = UsersList.SelectedItems;
            ObservableCollection<UdownDisp> iterateThrough = new ObservableCollection<UdownDisp>();

            foreach (UdownDisp item in selectedItems)
            {
                selecteduserlist.Items.Add(item);
                iterateThrough.Add(item);
            }
            foreach (UdownDisp item in iterateThrough)
            {
                EnD.Remove(item);

            }

        }
        private void Week_Defaults()
        {
            //cmbWeekList.SelectedItem = 0;
            textMonday.Text = "0";
            textTuesday.Text = "0";
            textWednesday.Text = "0";
            textThursday.Text = "0";
            textFriday.Text = "0";
            textSaturday.Text = "0";
            textSunday.Text = "0";
        }
        //private void Day_Defaults()
        //{
        //    cmbPasstimeDayList.SelectedIndex = 0;
        //    textStartTime_Hour1.Text = "00";
        //    textStartTime_Hour2.Text = "00";
        //    textStartTime_Hour3.Text = "00";
        //    textStartTime_Hour4.Text = "00";
        //    textStartTime_Hour5.Text = "00";
        //    textStartTime_Minute1.Text = "00";
        //    textStartTime_Minute2.Text = "00";
        //    textStartTime_Minute3.Text = "00";
        //    textStartTime_Minute4.Text = "00";
        //    textStartTime_Minute5.Text = "00";
        //    textEndTime_Hour1.Text = "00";
        //    textEndTime_Hour2.Text = "00";
        //    textEndTime_Hour3.Text = "00";
        //    textEndTime_Hour4.Text = "00";
        //    textEndTime_Hour5.Text = "00";
        //    textEndTime_Minute1.Text = "00";
        //    textEndTime_Minute2.Text = "00";
        //    textEndTime_Minute3.Text = "00";
        //    textEndTime_Minute4.Text = "00";
        //    textEndTime_Minute5.Text = "00";



        //}
        private void DisableWeekCategory()
        {
            textSunday.IsReadOnly = true;
            textMonday.IsReadOnly = true;
            textTuesday.IsReadOnly = true;
            textWednesday.IsReadOnly = true;
            textThursday.IsReadOnly = true;
            textFriday.IsReadOnly = true;
            textSaturday.IsReadOnly = true;
            timezoneName.IsReadOnly = true;
            //cmbWeekList.IsEnabled = false;
            btnSaveWeek.IsEnabled = false;
        }
        private void DisableDayCategory()
        {
            //cmbPasstimeDayList.IsEnabled = false;
            textStartTime_Hour1.IsReadOnly = true;
            textStartTime_Hour2.IsReadOnly = true;
            textStartTime_Hour3.IsReadOnly = true;
            textStartTime_Hour4.IsReadOnly = true;
            textStartTime_Hour5.IsReadOnly = true;
            textStartTime_Minute1.IsReadOnly = true;
            textStartTime_Minute2.IsReadOnly = true;
            textStartTime_Minute3.IsReadOnly = true;
            textStartTime_Minute4.IsReadOnly = true;
            textStartTime_Minute5.IsReadOnly = true;
            textEndTime_Hour1.IsReadOnly = true;
            textEndTime_Hour2.IsReadOnly = true;
            textEndTime_Hour3.IsReadOnly = true;
            textEndTime_Hour4.IsReadOnly = true;
            textEndTime_Hour5.IsReadOnly = true;
            textEndTime_Minute1.IsReadOnly = true;
            textEndTime_Minute2.IsReadOnly = true;
            textEndTime_Minute3.IsReadOnly = true;
            textEndTime_Minute4.IsReadOnly = true;
            textEndTime_Minute5.IsReadOnly = true;



            btnSaveDay.IsEnabled = false;


        }
        private void EnableWeekCategory()
        {
            textSunday.IsReadOnly = false;
            textMonday.IsReadOnly = false;
            textTuesday.IsReadOnly = false;
            textWednesday.IsReadOnly = false;
            textThursday.IsReadOnly = false;
            textFriday.IsReadOnly = false;
            textSaturday.IsReadOnly = false;
            timezoneName.IsReadOnly = false;
            //cmbWeekList.IsEnabled = true;

            btnSaveWeek.IsEnabled = true;
        }
        private void EnableDayCategory()
        {
            //cmbPasstimeDayList.IsEnabled = true;
            textStartTime_Hour1.IsReadOnly = false;
            textStartTime_Hour2.IsReadOnly = false;
            textStartTime_Hour3.IsReadOnly = false;
            textStartTime_Hour4.IsReadOnly = false;
            textStartTime_Hour5.IsReadOnly = false;
            textStartTime_Minute1.IsReadOnly = false;
            textStartTime_Minute2.IsReadOnly = false;
            textStartTime_Minute3.IsReadOnly = false;
            textStartTime_Minute4.IsReadOnly = false;
            textStartTime_Minute5.IsReadOnly = false;
            textEndTime_Hour1.IsReadOnly = false;
            textEndTime_Hour2.IsReadOnly = false;
            textEndTime_Hour3.IsReadOnly = false;
            textEndTime_Hour4.IsReadOnly = false;
            textEndTime_Hour5.IsReadOnly = false;
            textEndTime_Minute1.IsReadOnly = false;
            textEndTime_Minute2.IsReadOnly = false;
            textEndTime_Minute3.IsReadOnly = false;
            textEndTime_Minute4.IsReadOnly = false;
            textEndTime_Minute5.IsReadOnly = false;



            btnSaveDay.IsEnabled = true;

        }
        private void ClearWeekFields()
        {
            timezoneName.Text = "";
            textMonday.Text = "";
            textTuesday.Text = "";
            textWednesday.Text = "";
            textThursday.Text = "";
            textFriday.Text = "";
            textSaturday.Text = "";
            textSunday.Text = "";
            //cmbWeekList.SelectedIndex = 0;
        }
        private void ClearDayFields()
        {
            textStartTime_Hour1.Text = "";
            textStartTime_Hour2.Text = "";
            textStartTime_Hour3.Text = "";
            textStartTime_Hour4.Text = "";
            textStartTime_Hour5.Text = "";
            textStartTime_Minute1.Text = "";
            textStartTime_Minute2.Text = "";
            textStartTime_Minute3.Text = "";
            textStartTime_Minute4.Text = "";
            textStartTime_Minute5.Text = "";
            textEndTime_Hour1.Text = "";
            textEndTime_Hour2.Text = "";
            textEndTime_Hour3.Text = "";
            textEndTime_Hour4.Text = "";
            textEndTime_Hour5.Text = "";
            textEndTime_Minute1.Text = "";
            textEndTime_Minute2.Text = "";
            textEndTime_Minute3.Text = "";
            textEndTime_Minute4.Text = "";
            textEndTime_Minute5.Text = "";
            //cmbPasstimeDayList.SelectedIndex = 0;

        }
        private void BtnSaveWeek_Click(object sender, RoutedEventArgs e)
        {
            string Name = timezoneName.Text;
            SqlTransaction transaction = null;
            int SelectedItemID;
            string SelectedItem_Name = "";
            int Week_ID = 0;
            foreach (tbl_TimeSlots record in TimeSlotsFirstTab.SelectedItems)
            {
                SelectedItemID = record.ID;
                SelectedItem_Name = record.Name;
                Week_ID = record.Week_ID;
            }


            if (Name.Length == 0 || Name == SelectedItem_Name)
            {
                if (Week_ID > 0)
                {
                    using (SqlConnection sqlCon = new SqlConnection(ConString))
                    {
                        sqlCon.Open();
                        if (sqlCon.State == ConnectionState.Open)
                        {
                            try
                            {
                                string Sunday = textSunday.Text;
                                string Monday = textMonday.Text;
                                string Tuesday = textTuesday.Text;
                                string Wednesday = textWednesday.Text;
                                string Thursday = textThursday.Text;
                                string Friday = textFriday.Text;
                                string Saturday = textSaturday.Text;
                                bool Week_Updated = DBFactory.Update(ConString, "UPDATE tbl_TimeSlotsWeek SET Sunday = '" + Sunday + "',Monday = '" + Monday + "',Tuesday = '" + Tuesday + "',Wednesday = '" + Wednesday + "',Thursday = '" + Thursday + "',Friday = '" + Friday + "',Saturday = '" + Saturday + "' where ID = '" + Week_ID + "'");
                                if (Week_Updated)
                                {
                                    TaskCompleted();
                                }
                                else
                                {
                                    TaskNotCompleted();
                                }
                            }
                            catch (Exception ex)
                            {
                                TaskNotCompleted();
                                _ = MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            TaskNotCompleted();
                        }
                    }

                }

            }
            else
            {

                using (SqlConnection sqlCon = new SqlConnection(ConString))
                {
                    sqlCon.Open();

                    try
                    {
                        if (sqlCon.State == ConnectionState.Open)
                        {
                            transaction = sqlCon.BeginTransaction();
                            string Sunday = textSunday.Text;
                            string Monday = textMonday.Text;
                            string Tuesday = textTuesday.Text;
                            string Wednesday = textWednesday.Text;
                            string Thursday = textThursday.Text;
                            string Friday = textFriday.Text;
                            string Saturday = textSaturday.Text;
                            _ = DBFactory.Update("UPDATE tbl_TimeSlotsWeek SET Sunday = '" + Sunday + "',Monday = '" + Monday + "',Tuesday = '" + Tuesday + "',Wednesday = '" + Wednesday + "',Thursday = '" + Thursday + "',Friday = '" + Friday + "',Saturday = '" + Saturday + "' where ID = '" + Week_ID + "'", sqlCon, transaction);
                            _ = DBFactory.Insert("UPDATE tbl_TimeSlots SET Name = '" + Name + "' Where Week_ID = '" + Week_ID + "'", sqlCon, transaction);
                            transaction.Commit();
                            TaskCompleted();
                            sqlCon.Close();
                        }
                        else
                        {
                            TaskNotCompleted();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TaskNotCompleted();
                        _ = MessageBox.Show(ex.Message);
                    }
                }

            }
            TimeSlotsFirstTab.SelectedItem = null;
            ClearWeekFields();
            DisableWeekCategory();
            TimeSlots_list();
            BtnModifiedWeekClicked = false;
        }
        //private void AddDay_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    BtnModifiedDayClicked = true;
        //    EnableDayCategory();
        //    Day_Defaults();
        //}
        private void ModifyDay_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (TimeSlotsDay.SelectedItems.Count == 0)
            {
                TaskNotCompleted();
                return;
            }
            else
            {
                PopulateDayFields();
                EnableDayCategory();
                BtnModifiedDayClicked = true;
            }

        }
        private void ModifyWeek_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TimeSlotsFirstTab.SelectedItems.Count == 0)
            {
                TaskNotCompleted();
                return;
            }
            else
            {
                PopulateWeekFields();
                EnableWeekCategory();
                BtnModifiedWeekClicked = true;
            }
        }
        private void ClearWeek_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TimeSlotsFirstTab.SelectedItems.Count == 0)
            {
                TaskNotCompleted();
                return;
            }
            else
            {
                int Week_ID = 0;
                foreach (tbl_TimeSlots record in TimeSlotsFirstTab.SelectedItems)
                {
                    Week_ID = record.ID;
                }
                if (BtnModifiedWeekClicked)
                {
                    Clear_WeekSlot(Week_ID);
                    Week_Defaults();
                }
                else
                {
                    Clear_WeekSlot(Week_ID);
                }


            }
        }
        private void Clear_WeekSlot(int Week_ID)
        {
            if (Week_ID > 0)
            {
                bool cleared = DBFactory.Update(ConString, "Update tbl_TimeSlotsWeek SET Sunday = '0', Monday = '0', Tuesday = '0', Wednesday = '0', Thursday = '0', Friday = '0', Saturday = '0' Where ID = " + Week_ID + "");
                if (cleared)
                {
                    TaskCompleted();
                }
                else
                {
                    TaskNotCompleted();
                    return;
                }
            }
            else
            {
                TaskNotCompleted();
                return;
            }
        }
        private void ClrDaySlot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TimeSlotsDay.SelectedItems.Count == 0)
            {
                TaskNotCompleted();
                return;
            }
            else
            {
                bool cleared = false;
                foreach (tbl_TimeSlotsDay record in TimeSlotsDay.SelectedItems)
                {
                    int ID = record.ID;
                    cleared = DBFactory.Update(ConString, "UPDATE tbl_TimeSlotsDay SET ST_1 = '00:00', ST_2 = '00:00', ST_3 = '00:00', ST_4 = '00:00', ST_5 = '00:00', ET_1 = '00:00', ET_2 = '00:00', ET_3 = '00:00', ET_4 = '00:00', ET_5 = '00:00' WHERE ID = '" + ID + "'");


                }
                if (cleared)
                {
                    DaySlots_list();
                    TaskCompleted();
                    DisableDayCategory();
                    ClearDayFields();
                    BtnModifiedDayClicked = false;
                }

            }
        }
        //private void AddWeek_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    EnableWeekCategory();
        //    Week_Defaults();
        //}
        private void TaskCompleted()
        {
            StatusWindow.Text = "Task Completed";
            StatusWindow.Foreground = Brushes.Green;
            Thread newThread = new Thread(ClearStatusWindows);
            newThread.Start();
            
        }
        private void TaskNotCompleted()
        {
            StatusWindow.Text = "Task Incomplete";
            StatusWindow.Foreground = Brushes.Red;
            Thread newThread = new Thread(ClearStatusWindows);
            newThread.Start();
            
        }
        private string ConvertToSubstring(string txt, int index)
        {
            string[] substrings = txt.Split(':');
            if (substrings.Length == 2)
            {
                return substrings[index];
            }
            return null;
        }
        private void PopulateDayFields()
        {
            foreach (tbl_TimeSlotsDay record in TimeSlotsDay.SelectedItems)
            {
                textStartTime_Hour1.Text = ConvertToSubstring(record.ST_1.ToString(), 0);
                textStartTime_Minute1.Text = ConvertToSubstring(record.ST_1.ToString(), 1);
                textStartTime_Hour2.Text = ConvertToSubstring(record.ST_2.ToString(), 0);
                textStartTime_Minute2.Text = ConvertToSubstring(record.ST_2.ToString(), 1);
                textStartTime_Hour3.Text = ConvertToSubstring(record.ST_3.ToString(), 0);
                textStartTime_Minute3.Text = ConvertToSubstring(record.ST_3.ToString(), 1);
                textStartTime_Hour4.Text = ConvertToSubstring(record.ST_4.ToString(), 0);
                textStartTime_Minute4.Text = ConvertToSubstring(record.ST_4.ToString(), 1);
                textStartTime_Hour5.Text = ConvertToSubstring(record.ST_5.ToString(), 0);
                textStartTime_Minute5.Text = ConvertToSubstring(record.ST_5.ToString(), 1);

                textEndTime_Hour1.Text = ConvertToSubstring(record.ET_1.ToString(), 0);
                textEndTime_Minute1.Text = ConvertToSubstring(record.ET_1.ToString(), 1);
                textEndTime_Hour2.Text = ConvertToSubstring(record.ET_2.ToString(), 0);
                textEndTime_Minute2.Text = ConvertToSubstring(record.ET_2.ToString(), 1);
                textEndTime_Hour3.Text = ConvertToSubstring(record.ET_3.ToString(), 0);
                textEndTime_Minute3.Text = ConvertToSubstring(record.ET_3.ToString(), 1);
                textEndTime_Hour4.Text = ConvertToSubstring(record.ET_4.ToString(), 0);
                textEndTime_Minute4.Text = ConvertToSubstring(record.ET_4.ToString(), 1);
                textEndTime_Hour5.Text = ConvertToSubstring(record.ET_5.ToString(), 0);
                textEndTime_Minute5.Text = ConvertToSubstring(record.ET_5.ToString(), 1);
                //cmbPasstimeDayList.SelectedIndex = record.ID - 1;

            }
        }
        private void PopulateWeekFields()
        {
            foreach (tbl_TimeSlots timeslot in TimeSlotsFirstTab.SelectedItems)
            {
                timezoneName.Text = timeslot.Name;
                int Week_ID = timeslot.Week_ID;
                DataTable dt = DBFactory.GetAllByQuery(ConString, "SELECT * FROM tbl_TimeSlotsWeek Where ID = '" + Week_ID + "'");
                List<tbl_TimeSlotsWeek> WeekData = new List<tbl_TimeSlotsWeek>();
                WeekData = ConvertDataTable<tbl_TimeSlotsWeek>(dt);
                foreach (tbl_TimeSlotsWeek item in WeekData)
                {
                    //cmbWeekList.SelectedIndex = Week_ID - 1;
                    textFriday.Text = item.Friday;
                    textSaturday.Text = item.Saturday;
                    textSunday.Text = item.Sunday;
                    textMonday.Text = item.Monday;
                    textTuesday.Text = item.Tuesday;
                    textWednesday.Text = item.Wednesday;
                    textThursday.Text = item.Thursday;
                }
            }
        }
        private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BtnModifiedDayClicked && TimeSlotsDay.SelectedItems.Count > 0)
            {
                PopulateDayFields();
            }
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private void TimeSlotsHome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BtnModifiedWeekClicked && TimeSlotsFirstTab.SelectedItems.Count == 1)
            {
                PopulateWeekFields();
            }
        }

        private void UsersList_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                if (UsersList.ItemsSource == null || UsersList.Items.Count > 0)
                {
                    EnD = new ObservableCollection<UdownDisp>();
                    DataTable dt = new DataTable();
                    if (DBFactory.ConnectServer(ConString))
                    {
                        dt = DBFactory.GetAllByQuery(ConString, "select distinct EnrollNumber, EName from tbl_enroll");
                        DataColumn SrNo = new DataColumn("SrNo", typeof(int));
                        dt.Columns.Add(SrNo);
                        int index = 1;
                        foreach (DataRow row in dt.Rows)
                        {
                            row["SrNo"] = index;
                            index++;
                        }
                        EnD = ConvertDataTableObs<UdownDisp>(dt);
                        UsersList.ItemsSource = EnD;
                    }
                    else
                    {
                        UsersList.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {

                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( UDList  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }

        }

        private void txt_search_1_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search_1.Visibility = Visibility.Collapsed;
        }

        //private void Txt_chnged_Item_resetdown_List(object sender, TextChangedEventArgs e)
        //{
        //    if (UsersList != null)
        //    {
        //        CollectionViewSource.GetDefaultView(UsersList.ItemsSource).Refresh();
        //    }
        //    if (UsersList != null)
        //    {
        //        find(txt_search.Text);

        //    }


        //    if (UsersList != null)
        //    {
        //        CollectionViewSource.GetDefaultView(UsersList.ItemsSource).Refresh();
        //    }



        //    if (txt_search.Text == "")
        //    {
        //        txt_search_1.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        txt_search_1.Visibility = Visibility.Collapsed;
        //    }


        //}
        private void find(string str)
        {
            foreach (UdownDisp item in UsersList.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    UsersList.SelectedItems.Add(item);
                }
                else
                {
                    UsersList.SelectedItems.Remove(item);
                }
            }

        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            ////FindListViewItem(downuserlist);
            //if (UsersList != null)
            //    CollectionViewSource.GetDefaultView(UsersList.ItemsSource).Refresh();
            if (UsersList != null)
            {
                CollectionViewSource.GetDefaultView(UsersList.ItemsSource).Refresh();
            }
            if (UsersList != null)
            {
                find(txt_search.Text);

            }


            if (UsersList != null)
            {
                CollectionViewSource.GetDefaultView(UsersList.ItemsSource).Refresh();
            }



            if (txt_search.Text == "")
            {
                txt_search_1.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search_1.Visibility = Visibility.Collapsed;
            }
        }

        private bool EnableDevice(int id)
        {

            return axFP_CLOCK.EnableDevice(id, 1);
        }
        private bool DisableDevice(string ip, int DeviceId)
        {
            if (ConnectDevice(ip, DeviceId))
            {
                return axFP_CLOCK.EnableDevice(DeviceId, 0);
            }
            return false;
        }
        public unsafe bool Machine_dayWriteSet(int DayID,int MachineID, string ip)
        {

            bool Device_Enabled = EnableDevice(MachineID);
            bool IPset = axFP_CLOCK.SetIPAddress(ref ip, 5005, 0);

            if (axFP_CLOCK.OpenCommPort(MachineID) && Device_Enabled && IPset)
            {
                PasstimeInfo* dwTimeInfo = stackalloc PasstimeInfo[5];

                IntPtr ptr = new IntPtr(dwTimeInfo);
                try
                {
                    if (DBFactory.ConnectServer(ConString))
                    {
                        DataTable dt = DBFactory.GetAllByQuery(ConString, "SELECT * from tbl_TimeSlotsDay Where ID = '" + DayID + "'");
                        List<tbl_TimeSlotsDay> dayData = ConvertDataTable<tbl_TimeSlotsDay>(dt);
                        foreach (tbl_TimeSlotsDay day in dayData)
                        {
                            dwTimeInfo->bSHour = Convert.ToByte(ConvertToSubstring(day.ST_1.ToString(), 0));
                            dwTimeInfo->bSMinute = Convert.ToByte(ConvertToSubstring(day.ST_1.ToString(), 1));
                            dwTimeInfo->bEHour = Convert.ToByte(ConvertToSubstring(day.ET_1.ToString(), 0));
                            dwTimeInfo->bEMinute = Convert.ToByte(ConvertToSubstring(day.ET_1.ToString(), 1));
                            dwTimeInfo++;

                            dwTimeInfo->bSHour = Convert.ToByte(ConvertToSubstring(day.ST_2.ToString(), 0));
                            dwTimeInfo->bSMinute = Convert.ToByte(ConvertToSubstring(day.ST_2.ToString(), 1));
                            dwTimeInfo->bEHour = Convert.ToByte(ConvertToSubstring(day.ET_2.ToString(), 0));
                            dwTimeInfo->bEMinute = Convert.ToByte(ConvertToSubstring(day.ET_2.ToString(), 1));
                            dwTimeInfo++;

                            dwTimeInfo->bSHour = Convert.ToByte(ConvertToSubstring(day.ST_3.ToString(), 0));
                            dwTimeInfo->bSMinute = Convert.ToByte(ConvertToSubstring(day.ST_3.ToString(), 1));
                            dwTimeInfo->bEHour = Convert.ToByte(ConvertToSubstring(day.ET_3.ToString(), 0));
                            dwTimeInfo->bEMinute = Convert.ToByte(ConvertToSubstring(day.ET_3.ToString(), 1));
                            dwTimeInfo++;

                            dwTimeInfo->bSHour = Convert.ToByte(ConvertToSubstring(day.ST_4.ToString(), 0));
                            dwTimeInfo->bSMinute = Convert.ToByte(ConvertToSubstring(day.ST_4.ToString(), 1));
                            dwTimeInfo->bEHour = Convert.ToByte(ConvertToSubstring(day.ET_4.ToString(), 0));
                            dwTimeInfo->bEMinute = Convert.ToByte(ConvertToSubstring(day.ET_4.ToString(), 1));
                            dwTimeInfo++;

                            dwTimeInfo->bSHour = Convert.ToByte(ConvertToSubstring(day.ST_5.ToString(), 0));
                            dwTimeInfo->bSMinute = Convert.ToByte(ConvertToSubstring(day.ST_5.ToString(), 1));
                            dwTimeInfo->bEHour = Convert.ToByte(ConvertToSubstring(day.ET_5.ToString(), 0));
                            dwTimeInfo->bEMinute = Convert.ToByte(ConvertToSubstring(day.ET_5.ToString(), 1));

                            bool bRet = axFP_CLOCK.SetDayPassTime(MachineID, day.ID, ptr);
                            _ = axFP_CLOCK.EnableDevice(MachineID, 1);
                            return bRet;

                        }

                    }
                }
                catch (Exception)
                {
                    return false;
                }


            }
            else
            {
                return false;
            }


            return false; //to fix the error message not all code paths returns a value
        }
        public unsafe bool WeekWriteSet(int Week_ID, int MachineID, string IP)
        {
            

            bool Enabled = EnableDevice(MachineID);
            bool IpSet = axFP_CLOCK.SetIPAddress(ref IP, 5005, 0);
            if (axFP_CLOCK.OpenCommPort(MachineID) && Enabled && IpSet)
            {
                DataTable dt = DBFactory.GetAllByQuery(ConString, "SELECT * from tbl_TimeSlotsWeek Where ID = '" + Week_ID + "'");
                List<tbl_TimeSlotsWeek> WeekData = ConvertDataTable<tbl_TimeSlotsWeek>(dt);
                bool bRet = false;
                foreach (tbl_TimeSlotsWeek item in WeekData) {
                    byte* byteInfo = stackalloc byte[7];

                    IntPtr ptrAddr = new IntPtr(byteInfo);

                    *byteInfo++ = Convert.ToByte(item.Sunday);
                    *byteInfo++ = Convert.ToByte(item.Monday);
                    *byteInfo++ = Convert.ToByte(item.Tuesday);
                    *byteInfo++ = Convert.ToByte(item.Wednesday);
                    *byteInfo++ = Convert.ToByte(item.Thursday);
                    *byteInfo++ = Convert.ToByte(item.Friday);
                    *byteInfo = Convert.ToByte(item.Saturday);
                     bRet = axFP_CLOCK.SetWeekPassTime(MachineID, Week_ID, ptrAddr);
                    _ = EnableDevice(MachineID);
                    return bRet;
                }
            }
            else 
            {
                return false;
            }
            return false;
        }
        public bool SetUserCtrl(int UserID, int Week_ID, int Machine_ID, DateTime DateStart, DateTime DateEnd)
        {

            bool bRet = axFP_CLOCK.SetUserCtrl(
                Machine_ID,
                UserID,
                Week_ID,
                0,
                DateStart.Year,
                DateStart.Month,
                DateStart.Day,
                DateEnd.Year,
                DateEnd.Month,
                DateEnd.Day);

            if (bRet)
            {
                _ = EnableDevice(Machine_ID);
            }
            return bRet;

        }
        public void ClearStatusWindows() 
        {
            Thread.Sleep(2000);
            Action updateUI = () =>
            {
                StatusWindow.Text = "";
            };
            Dispatcher.Invoke(updateUI);
        }

        public bool ConnectDevice(string ip, int id)
        {
            if (ip.Length > 0 && id > 0)
            {
                bool SetIpSuccess = axFP_CLOCK.SetIPAddress(ref ip, 5005, id);
                bool OpenComPortSuccess = axFP_CLOCK.OpenCommPort(id);
                return SetIpSuccess && OpenComPortSuccess;
            }
            return false;
        }
        private List<int> GetEnrolledUserIDs(int Machine_ID, string ip)
        {
            _ = DisableDevice(ip, Machine_ID);
            bool IPset = axFP_CLOCK.SetIPAddress(ref ip, 5005, Machine_ID);
            bool Device_Enabled = EnableDevice(Machine_ID);
            List<int> userIds = new List<int>();
            if (Device_Enabled && IPset)
            {
                bool bBreakFail = false;
                bool bRet = axFP_CLOCK.ReadAllUserID(Machine_ID);
                if (!bRet)
                {

                    _ = EnableDevice(Machine_ID);
                    return userIds;
                }
                int dwEnrollNumber = 0;
                int dwEnMachineID = 0;
                int dwBackupNum = 0;
                int dwPrivilegeNum = 0;
                int dwEnable = 0;
                //int dwPassWord = 0;
                //int vPhotoSize = 0;

                do
                {
                    bRet = axFP_CLOCK.GetAllUserID(
                        Machine_ID,
                        ref dwEnrollNumber,
                        ref dwEnMachineID,
                        ref dwBackupNum,
                        ref dwPrivilegeNum,
                        ref dwEnable
                    );

                    if (bRet == false)
                    {
                        bBreakFail = true;
                        break;
                    }

                    // Store the user ID in the list
                    userIds.Add(dwEnrollNumber);

                    // Reset other variables if needed

                } while (bRet);

                if (bBreakFail)
                {
                    //success
                }


                EnableDevice(Machine_ID);

                // Return the list of user IDs
                return userIds;
            }
            return userIds;

        }

        private bool DelUserControl(int DeviceId, string ip, int EnrollNumber) 
        {
            if (!DisableDevice(ip, DeviceId))
            {
                return false;
            }
            bool bRet = axFP_CLOCK.DeleteUserCtrl(DeviceId, EnrollNumber);
            if (bRet) {
                bool enabled = axFP_CLOCK.EnableDevice(DeviceId, 1);
                return enabled && bRet;
            }
            return false;

        }
        private void TextBoxDay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            
            if (!IsValidInput(newText) || e.Text.Contains(" "))
            {
                e.Handled = true; 
            }
        }

        private bool IsValidInput(string text)
        {
            // Check if the text length exceeds 2 characters
            if (text.Length > 2)
            {
                return false;
            }

            // Check if the text contains non-numeric characters
            if (!Regex.IsMatch(text, "^[0-9]*$"))
            {
                return false;
            }

            // Check the specific conditions for the first and second digits
            if (text.Length >= 1)
            {
                int firstDigit = int.Parse(text[0].ToString());

                // First digit can only be 0, 1, or 2
                if (firstDigit < 0 || firstDigit > 2)
                {
                    return false;
                }

                // If the first digit is 2, the second digit can only be 0, 1, 2, 3, or 4
                if (firstDigit == 2 && text.Length == 2)
                {
                    int secondDigit = int.Parse(text[1].ToString());
                    if (secondDigit < 0 || secondDigit > 4)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private void TextBoxWeek_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            string newText = textBox.Text + e.Text;

            if (!Regex.IsMatch(newText, @"^[0-8]$"))
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevent the spacebar key press
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }










    }




}
