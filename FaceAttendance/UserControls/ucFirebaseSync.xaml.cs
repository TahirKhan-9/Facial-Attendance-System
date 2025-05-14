using FaceAttendance.Classes;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsmanCodeBlocks.Data.Sql.Local;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using HIKVISION;
using OfficeOpenXml.ConditionalFormatting;
using System.IO;
using CrystalDecisions.ReportSource;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucFirebaseSync.xaml
    /// </summary>
    public partial class ucFirebaseSync : UserControl
    {
        ObservableCollection<UdownDisp> EnD;
        string ConString;
        System.Data.DataTable dptTable;
        List<Departments> dptNames;
        string FirebaseUsername;
        List<UdownDisp> UsersList;
        string FirebasePassword;
        DeviceManagement deviceManagement;
        string dptID;
        int UsersCount = 0;
        public ucFirebaseSync(string ConString, DeviceManagement deviceManagement)
        {
            this.ConString = ConString;
            InitializeComponent();
            this.deviceManagement = deviceManagement;
        }

        //private void Upload_to_device_Click(object sender, RoutedEventArgs e)
        //{
        //    if(selecteduserlist.Items.Count == 0)
        //    {
        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Selected User List is Empty.", false);
        //        message.btnCancel.Visibility = Visibility.Collapsed;
        //        message.btnOk.Content = "Ok";
        //        _ = message.ShowDialog();
        //        return;
        //    }
        //    U_Wait.Visibility = Visibility.Visible;
        //    foreach (UdownDisp item in selecteduserlist.Items)
        //    {
        //        item.IsSynchronized = true;
        //    }
        //}
        private bool FirebaseCredentialsSaved()
        {
            try
            {
                System.Data.DataTable dt = DBFactory.GetAllByQuery(ConString, "select * from FirebaseCredentials");
                if (dt != null)
                {
                    if(dt.Rows.Count > 0)
                    {
                        FirebaseUsername = dt.Rows[0]["Username"].ToString();
                        FirebasePassword = dt.Rows[0]["Password"].ToString();
                        if(FirebaseUsername.Length == 0 && FirebasePassword.Length == 0) {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex) {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( FirebaseCredentialsSaved  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
                return false;
            }
        }
        private void disableControls()
        {
            SelectionGrid.IsEnabled = false;
            btnSync.IsEnabled = false;
            SearchPanel.IsEnabled = false;
            DisableDeviceManagementButtons();
        }
        private void enableControls()
        {
            SelectionGrid.IsEnabled = true;
            btnSync.IsEnabled = true;
            SearchPanel.IsEnabled = true;
            EnableDeviceManagementButtons();
        }
        private void DisableDeviceManagementButtons()
        {
            deviceManagement.btnDevManagement.IsEnabled = false;
            deviceManagement.btnStaffManage.IsEnabled = false;
            deviceManagement.btnAddStaff.IsEnabled = false;
            deviceManagement.btnAddDept.IsEnabled = false;
            deviceManagement.btnAddDept.IsEnabled = false;
            deviceManagement.btnLogM.IsEnabled = false;
            deviceManagement.btnShiftDef.IsEnabled = false;
            deviceManagement.btnReportmanage.IsEnabled = false;
            deviceManagement.btnTimeSlots.IsEnabled = false;
            deviceManagement.btnSettings.IsEnabled = false;
            deviceManagement.btnReltime.IsEnabled = false;
            deviceManagement.gridCloseApp.IsEnabled = false;
            deviceManagement.btnFirebaseSync.IsEnabled = false;
            deviceManagement.btnLogMigration.IsEnabled = false;
        }
        private void EnableDeviceManagementButtons()
        {
            deviceManagement.btnDevManagement.IsEnabled = true;
            deviceManagement.btnStaffManage.IsEnabled = true;
            deviceManagement.btnAddStaff.IsEnabled = true;
            deviceManagement.btnAddDept.IsEnabled = true;
            deviceManagement.btnAddDept.IsEnabled = true;
            deviceManagement.btnLogM.IsEnabled = true;
            deviceManagement.btnShiftDef.IsEnabled = true;
            deviceManagement.btnReportmanage.IsEnabled = true;
            deviceManagement.btnTimeSlots.IsEnabled = true;
            deviceManagement.btnSettings.IsEnabled = true;
            deviceManagement.btnReltime.IsEnabled = true;
            deviceManagement.gridCloseApp.IsEnabled = true;
            deviceManagement.btnFirebaseSync.IsEnabled = true;
            deviceManagement.btnLogMigration.IsEnabled = true;
        }
        private async void Upload_to_device_Click(object sender, RoutedEventArgs e)
        {
            ChangeBackgroundAll();
            if (selecteduserlist.Items.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Selected User List is Empty.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                return;
            }
            disableControls();
            if (!FirebaseCredentialsSaved())
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Save Firebase Credentials From Settings First.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                enableControls();
                return;
            }
            U_Wait.Visibility = Visibility.Visible;
            FirebaseService fs = new FirebaseService();
            bool Connected = await fs.IsConnected();
            if(!Connected)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Firebase Error, Check Internet or Contact DDS.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                U_Wait.Visibility = Visibility.Collapsed;
                enableControls();
                return;
            }
            bool authenticated = await fs.AuthenticateUser(FirebaseUsername, FirebasePassword);
            if (!authenticated)
            {
                U_Wait.Visibility = Visibility.Collapsed;
                enableControls();
                return;
            }
            List<ImageUpload> Uploads = await fs.GetImageUrlsByUsername(FirebaseUsername);
            if(Uploads.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Database is Empty, Please Upload Images First.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                U_Wait.Visibility = Visibility.Collapsed;
                enableControls();
                return;
            }
            foreach (var item in selecteduserlist.Items)
            {
                if (item is UdownDisp udownDispItem)
                {
                    ImageUpload Search = Uploads.FirstOrDefault(x=> x.Id == udownDispItem.EnrollNumber.ToString());
                    if(Search == null)
                    {
                        ChangeBackgroundColor(ref udownDispItem, "red");
                        continue;
                    }
                    byte[] downloadedImage;
                    try
                    {
                        downloadedImage = await fs.DownloadImage(Search.Url);
                    }
                    catch (Exception ex)
                    {
                        ChangeBackgroundColor(ref udownDispItem, "red");
                        continue;
                    }
                    //List<UdownDisp> users = UsersList.Where(x => x.EnrollNumber == udownDispItem.EnrollNumber && x.FingerNumber == 50);
                    List<UdownDisp> users = UsersList.Where(x => x.EnrollNumber == udownDispItem.EnrollNumber).ToList();
                    int rowsAffected = 0;
                    if(users.Count > 0)
                    {
                        if (IsFingerEnrolled(users))
                        {
                            rowsAffected = ImageOperations.UpdateImage(ConString, users[0].EnrollNumber, downloadedImage);
                        }
                        else
                        {
                            UdownDisp userInfo = users[0];
                            rowsAffected = ImageOperations.InsertImage(ConString, userInfo.EMachineNumber, userInfo.EnrollNumber, userInfo.Privilige, userInfo.EName, downloadedImage, userInfo.DptID);
                        }
                    }

                    if(rowsAffected == 1)
                    {
                        ChangeBackgroundColor(ref udownDispItem, "green");
                    }
                    else
                    {
                        ChangeBackgroundColor(ref udownDispItem, "red");
                    }





                    //if (udownDispItem.IsSynchronized)
                    //{
                    //    udownDispItem.IsSynchronized = false;
                    //}
                    //else
                    //{
                    //    udownDispItem.IsSynchronized = true;
                    //}

                }
            }
            LoadUserList();
            enableControls();
            U_Wait.Visibility = Visibility.Collapsed;
        }
        private void ChangeBackgroundAll()
        {
            if(selecteduserlist.Items.Count == 0)
            {
                return;
            }
            foreach (var item in selecteduserlist.Items)
            {
                if (item is UdownDisp udownDispItem)
                {
                    udownDispItem.IsSynchronized = null;
                }
            }
        }
        //green/red
        private void ChangeBackgroundColor(ref UdownDisp SelectedListView, string color)
        {
            bool flag = color != "red";            
            SelectedListView.IsSynchronized = flag;
        }
        private bool IsFingerEnrolled(List<UdownDisp> users)
        {
            UdownDisp user = users.FirstOrDefault(x => x.FingerNumber == 50);
            if(user != null)
            {
                return true;
            }
            return false;
        }

        private void txt_search11_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search11.Visibility = Visibility.Collapsed;
        }

        private void Txt_chnged_ItemSo_reset(object sender, TextChangedEventArgs e)
        {
            if (txt_search1.Text == "")
            {
                txt_search11.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search11.Visibility = Visibility.Collapsed;
            }

        }
        private void find(string str)
        {
            foreach (UdownDisp item in dbuserlist.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    dbuserlist.SelectedItems.Add(item);
                }
                else
                {
                    dbuserlist.SelectedItems.Remove(item);
                }
            }

        }

        private void rel_up_list(object sender, MouseButtonEventArgs e)
        {
            bool flag = selecteduserlist.Items.Count > 0;
            DbU_List(flag);
            //selecteduserlist.Items.Clear();
            dptCheckbox.IsChecked = false;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            dptCombo.Text = "---Select Dpt---";
            dptCombo.IsEnabled = true;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            dptCombo.IsEnabled = false;
            dptCombo.Text = "---Select Dpt---";
            dptID = "";
        }

        private void MoveSelectedRight(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in dbuserlist.SelectedItems)
            {
                //dbuserlist.Items.Remove(item);
                item.IsSynchronized = null;
                selecteduserlist.Items.Add(item);
                //dbuserlist.Items.Remove(item);

            }
            for (int x = 0; x < dbuserlist.Items.Count; x++)
            {
                if (dbuserlist.SelectedItem == dbuserlist.Items[x])
                {
                    dbuserlist.Items.RemoveAt(x);
                }
                if (dbuserlist.SelectedItems.Count != 0 && x + 1 == dbuserlist.Items.Count)
                {
                    x = -1;
                }
            }

        }
        public void setDptCombo()
        {
            dptTable = DBFactory.GetAllByQuery(ConString, "SELECT * FROM tbl_department");
            dptNames = Globals.ConvertDataTable<Departments>(dptTable).ToList();
            dptCombo.ItemsSource = dptNames.Select(x => x.dept_name);            
        }

        private void MoveSelectedBack(object sender, MouseButtonEventArgs e)
        {
            int index = selecteduserlist.SelectedItems.Count;
            while (index > 0)
            {
                dbuserlist.Items.Add(selecteduserlist.SelectedItems[0]);
                selecteduserlist.Items.Remove(selecteduserlist.SelectedItems[0]);

                index--;
            }
            SortDbuserlist(dbuserlist);
        }

        private void MoveAllBack(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in selecteduserlist.Items)
            {
                dbuserlist.Items.Add(item);

            }
            SortDbuserlist(dbuserlist);

            selecteduserlist.Items.Clear();
        }
        private void SortDbuserlist(ItemsControl dbuserlist)
        {
            List<UdownDisp> items = dbuserlist.Items.Cast<UdownDisp>().ToList();
            dbuserlist.Items.Clear();
            items = items.OrderBy(x => x.EnrollNumber).ToList();
            foreach(UdownDisp item in items)
            {
                dbuserlist.Items.Add(item);
            }
        }

        private void MoveAllRight(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in dbuserlist.Items)
            {

                //if (item.FingerNumber == 11)
                //    item.type = "Card";
                //else if (item.FingerNumber == 0)
                //    item.type = "Finger";
                //else if (item.FingerNumber == 50)
                //    item.type = "Face";
                //dbuserlist.Items.Add(item);
                //item.IsSynchronized = false;
                item.IsSynchronized = null;
                selecteduserlist.Items.Add(item);
                //dbuserlist.Items.Remove(item);
            }

            dbuserlist.Items.Clear();


        }
        
        private void dptCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (dptCombo.SelectedIndex != -1)
            {
                dptID = dptTable.Rows[dptCombo.SelectedIndex].ItemArray[0].ToString();
                if (!String.IsNullOrEmpty(dptID))
                {
                    //string dptStaffQuer = $"select Distinct EnrollNumber, EName, EMachineNumber, DptID from tbl_enroll where DptID = '{dptID}'";
                    //DbU_List(dptStaffQuer);
                    //DbU_List(dbuserlist.Items.Cast<UdownDisp>().ToList(), dptID);
                    DbU_List(EnD.Where(x => x.DptID == dptID).ToList(), dptID);
                }

            }
        }

        private void UsersListView_Loaded(object sender, RoutedEventArgs e)
        {
            DbU_List(false);
        }
        public void DbU_List(List<UdownDisp> usersList, string departID)
        {            
            dbuserlist.Items.Clear();
            if(usersList.Count > 0)            
            {
                if(selecteduserlist.Items.Count == 0)
                {
                    foreach(UdownDisp item in usersList)
                    {
                        dbuserlist.Items.Add(item);
                    }
                }
                else
                {
                    List<UdownDisp> selectedUsersList = selecteduserlist.Items.Cast<UdownDisp>().ToList();
                    List<int> selectedEnrollNumbers = selectedUsersList.Select(x=>x.EnrollNumber).ToList();
                    foreach(UdownDisp item in usersList)
                    {
                        if (!selectedEnrollNumbers.Contains(item.EnrollNumber))
                        {
                            dbuserlist.Items.Add(item);
                        }
                    }
                }
            }
        }
        public void DbU_List(string quer)
        {
            dbuserlist.Items.Clear();
            //dbuserlist.ItemsSource = null;

            if (dbuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                System.Data.DataTable dt = new System.Data.DataTable();
                bool con;
                con = DBFactory.ConnectServer(ConString);
                if (con == true)
                {
                    // "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll"
                    dt = DBFactory.GetAllByQuery(ConString, quer);
                    EnD = ConvertDataTableObs<UdownDisp>(dt);

                    int index = 1;
                    foreach (UdownDisp d in EnD)
                    {
                        d.SrNo = index;
                        dbuserlist.Items.Add(d);
                        index++;
                    }
                    //dbuserlist.ItemsSource = EnD;
                    ////////foreach (UdownDisp item in EnD)
                    ////////{
                    ////////    if (item.FingerNumber == 11)
                    ////////        item.type = "Card";
                    ////////    else if (item.FingerNumber == 0)
                    ////////        item.type = "Finger";
                    ////////    else if (item.FingerNumber == 50)
                    ////////        item.type = "Face";
                    ////////    dbuserlist.Items.Add(item);
                    ////////}
                    //just testing this line ahead
                    //dbuserlist.ItemsSource = EnD;
                }
                else
                {
                    dbuserlist.ItemsSource = null;
                }
            }
        }
        public void DbU_List(bool flag)
        {
            dbuserlist.Items.Clear();
            //dbuserlist.ItemsSource = null;

            if (dbuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                System.Data.DataTable dt = new System.Data.DataTable();
                bool con;
                con = DBFactory.ConnectServer(ConString);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    //dt = DBFactory.GetAllByQuery(ConString, "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll");
                    dt = DBFactory.GetAllByQuery(ConString, "select Distinct EnrollNumber, EName, EMachineNumber, DptID from tbl_enroll");
                    EnD = ConvertDataTableObs<UdownDisp>(dt);
                    int index = 1;
                    List<UdownDisp> selectedUsersList = selecteduserlist.Items.Cast<UdownDisp>().ToList();
                    foreach (UdownDisp item in EnD)
                    {
                        if (flag)
                        {
                            if (AlreadyPresent(selectedUsersList, item))
                            {
                                continue;
                            }
                        }
                        item.SrNo = index;
                        dbuserlist.Items.Add(item);
                        index++;
                    }
                    UsersCount = dbuserlist.Items.Count + selecteduserlist.Items.Count;
                    //dbuserlist.ItemsSource = EnD;
                    ////////foreach (UdownDisp item in EnD)
                    ////////{
                    ////////    if (item.FingerNumber == 11)
                    ////////        item.type = "Card";
                    ////////    else if (item.FingerNumber == 0)
                    ////////        item.type = "Finger";
                    ////////    else if (item.FingerNumber == 50)
                    ////////        item.type = "Face";
                    ////////    dbuserlist.Items.Add(item);
                    ////////}
                    //just testing this line ahead
                    //dbuserlist.ItemsSource  = EnD;  
                }
                else
                {
                    dbuserlist.ItemsSource = null;
                }
            }
        }
        private bool AlreadyPresent( List<UdownDisp> selectedUsersList,UdownDisp item)
        {

            //UdownDisp searchedItem = selecteduserlist.Items.Cast<UdownDisp>().ToList().FirstOrDefault(x => x.EnrollNumber == item.EnrollNumber);
            UdownDisp searchedItem = selectedUsersList.FirstOrDefault(x => x.EnrollNumber == item.EnrollNumber);
            if(searchedItem != null)
            {
                return true;
            }
            return false;

        }
        private static ObservableCollection<T> ConvertDataTableObs<T>(System.Data.DataTable dt)
        {
            ObservableCollection<T> data = new ObservableCollection<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);

                data.Add(item);
            }
            return data;
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
                    //if (pro.Name == "DptID")
                    //{
                    //    continue;
                    //}
                    if (pro.Name == column.ColumnName)
                    {
                        if (!Convert.IsDBNull(dr[column.ColumnName]))
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                        else
                        {
                            // Assign an empty string in the DBNull case
                            pro.SetValue(obj, string.Empty, null);
                        }
                    }
                    //pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (dbuserlist != null && txt_search1.Text.Count() != 0)
            {
                find(txt_search1.Text);

            }
        }

        private void ucFirebaseSync_Loaded(object sender, RoutedEventArgs e)
        {
            setDptCombo();
            LoadUserList();
        }
        private void LoadUserList()
        {
            try
            {
                System.Data.DataTable dt = DBFactory.GetAllByQuery(ConString, "select * from tbl_enroll");
                UsersList = Globals.ConvertDataTable<UdownDisp>(dt);

            }
            catch(Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( LoadUsersList() ucFirebaseSync  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);                
            }
        }

        private void txt_search1_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txt_search1.Text.Count() == 0)
            {
                txt_search11.Visibility = Visibility.Visible;
            }
        }

        private void selecteduserlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
