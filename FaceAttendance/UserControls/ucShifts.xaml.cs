using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsmanCodeBlocks.Data.Sql.Local;
using System.Threading;
using System.Data.SqlClient;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucShifts.xaml
    /// </summary>
    public partial class ucShifts : UserControl
    {
        string connString;
        ObservableCollection<shift> EnD;
        ObservableCollection<UdownDisp> UdP;
        bool BtnModifiedClicked = false;
        int ShiftID = 0;
        public ucShifts(string constring)
        {
            connString = constring;
            
            InitializeComponent();
            DisableControls();
            UD_List();
            US_List();
            UAS_List();
        }

        private void MoveSelectedRight(object sender, MouseButtonEventArgs e)
        {
            if (Shiftlist.SelectedItems.Count.Equals(1) && dbuserlist.SelectedItems.Count > 0)
            {
                StatusWindow.Foreground = Brushes.Green;
                StatusWindow.Text = "Wait...\tAssigning Shifts";
                U_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                messaget.ShowDialog();
                //MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                //messaget.ShowDialog();
                string query;
                foreach (shift st in Shiftlist.SelectedItems)
                {
                    foreach (UdownDisp item in dbuserlist.SelectedItems)
                    {
                        try
                        {
                        query = "update tbl_enroll set ShiftId=" + st.ShiftId + " where EnrollNumber=" + item.EnrollNumber;
                        _ = DBFactory.Insert(connString, query);
                        }
                        catch (Exception)
                        {
                            TaskIncomplete();
                        }
                    }
                }
                dbuserlist.ItemsSource = null;
                ShiftAssignlist.ItemsSource = null;
                US_List();
                UAS_List();
                U_Wait.Visibility = Visibility.Collapsed;
                TaskCompleted();
                //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                //message.btnCancel.Visibility = Visibility.Collapsed;
                //message.btnOk.Content = "Ok";
                //message.ShowDialog();
            }
            else
            {
                TaskIncomplete();
            }



        }

        private void MoveAllRight(object sender, MouseButtonEventArgs e)
        {
            if (Shiftlist.SelectedItems.Count.Equals(1))
            {
                StatusWindow.Foreground = Brushes.Green;
                StatusWindow.Text = @"Wait...	Assigning Shifts to All";
                U_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                _ = messaget.ShowDialog();

                string query;
                foreach (shift shift in Shiftlist.SelectedItems)
                {
                    bool allUpdated = true;
                    foreach (UdownDisp user in dbuserlist.Items)
                    {
                        query = "update tbl_enroll set ShiftId=" + shift.ShiftId + " where EnrollNumber=" + user.EnrollNumber;
                        bool updated = DBFactory.Update(connString, query);
                        allUpdated = allUpdated && updated;
                    }
                    if (allUpdated)
                    {
                        dbuserlist.ItemsSource = null;
                        ShiftAssignlist.ItemsSource = null;
                        US_List();
                        U_Wait.Visibility = Visibility.Collapsed;
                        UAS_List();
                        U_Wait.Visibility = Visibility.Collapsed;
                        TaskCompleted();

                    }
                }


            }
            else
            {
                TaskIncomplete();
            }
        }

        private void MoveSelectedBack(object sender, MouseButtonEventArgs e)
        {
            if(ShiftAssignlist.SelectedItems.Count > 0)
            {
            StatusWindow.Text = "Wait...\tRemoving Shifts";
            U_Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            string query;
           
                foreach (UdownDisp item in ShiftAssignlist.SelectedItems)
                {
                    query = "update tbl_enroll set ShiftId= NULL where EnrollNumber=" + item.EnrollNumber;
                    DBFactory.Insert(connString, query);
                }



            dbuserlist.ItemsSource = null;
            ShiftAssignlist.ItemsSource = null;
            US_List();
            U_Wait.Visibility = Visibility.Collapsed;
            UAS_List();
            U_Wait.Visibility = Visibility.Collapsed;
                TaskCompleted();
            //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
            //message.btnCancel.Visibility = Visibility.Collapsed;
            //message.btnOk.Content = "Ok";
            //message.ShowDialog();

            }
            else
            {
                TaskIncomplete();
            }
        }

        private void MoveAllBack(object sender, MouseButtonEventArgs e)
        {
            if(ShiftAssignlist.Items.Count > 0)
            {
                StatusWindow.Foreground = Brushes.Green;
            StatusWindow.Text = "Wait...\tRemoving All shifts";
            U_Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();

            string query;
            
                foreach (UdownDisp item in ShiftAssignlist.ItemsSource)
                {
                    query = "update tbl_enroll set ShiftId= NULL where EnrollNumber=" + item.EnrollNumber;
                    DBFactory.Insert(connString, query);
                }



            dbuserlist.ItemsSource = null;
            ShiftAssignlist.ItemsSource = null;
            US_List();
            U_Wait.Visibility = Visibility.Collapsed;
            UAS_List();
            U_Wait.Visibility = Visibility.Collapsed;
                //StatusWindow.Text = "Completed !!!";
                TaskCompleted();
            //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
            //message.btnCancel.Visibility = Visibility.Collapsed;
            //message.btnOk.Content = "Ok";
            //message.ShowDialog();
            }
            else
            {
                TaskIncomplete();
            }
        }

        public static bool IsDateTime(string dt)
        {
            DateTime tmpDt;
            return DateTime.TryParse(dt, out tmpDt);
        }
        public static bool IsTimeSpan(string time)
        {
            TimeSpan tmp_time;
            return TimeSpan.TryParse(time, out tmp_time);
        }

        private void btnCreateShift_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtST.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Shift Start Time", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                _ = Keyboard.Focus(txtST);
                return;
            }
            if (string.IsNullOrEmpty(txtET.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Shift End Time", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                _ = Keyboard.Focus(txtET);
                return;
            }
            if (string.IsNullOrEmpty(txt_shifName.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Shift Name", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                _ = Keyboard.Focus(txt_shifName);
                return;
            }
            //if(!IsTimeSpan(MinStartTime.Text) && MinStartTime.Text.Length > 0)
            //{
            //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please enter the correct minimum start time, or you may choose to leave this field empty.", false);
            //    message.btnCancel.Visibility = Visibility.Collapsed;
            //    message.btnOk.Content = "Ok";
            //    _ = message.ShowDialog();
            //    _ = Keyboard.Focus(MinStartTime);
            //    return;
            //}
            //if (!IsTimeSpan(MaxEndTime.Text) && MaxEndTime.Text.Length > 0)
            //{
            //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please enter the correct maximum end time, or you may choose to leave this field empty.", false);
            //    message.btnCancel.Visibility = Visibility.Collapsed;
            //    message.btnOk.Content = "Ok";
            //    _ = message.ShowDialog();
            //    _ = Keyboard.Focus(MaxEndTime);
            //    return;
            //}
            //if (IsDateTime(txtET.Text) && IsDateTime(txtST.Text) && (MinStartTime.Text.Length == 0 || (MinStartTime.Text.Length > 0 && IsDateTime(MinStartTime.Text))) && (MaxEndTime.Text.Length == 0 || (MaxEndTime.Text.Length > 0 && IsDateTime(MaxEndTime.Text))))
            if(IsDateTime(txtET.Text) && IsDateTime(txtST.Text))
            {
                TimeSpan shiftStart = TimeSpan.Parse(txtST.Text);
                TimeSpan shiftEnd = TimeSpan.Parse(txtET.Text);
                //TimeSpan minStartTime = IsTimeSpan(MinStartTime.Text) ? TimeSpan.Parse(MinStartTime.Text) : TimeSpan.Parse("00:00");
                //TimeSpan maxEndTime = IsTimeSpan(MaxEndTime.Text) ? TimeSpan.Parse(MaxEndTime.Text) : TimeSpan.Parse("00:00");

                if (shiftEnd <= shiftStart)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Unable to create shift due to overlapping or ambiguity.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    _ = message.ShowDialog();
                    _ = Keyboard.Focus(txt_shifName);
                    return;
                }

 

                if (BtnModifiedClicked && ShiftID > 0)
                {
                    try
                    {
                        //if (MinStartTime.Text.Length.Equals(0) && MaxEndTime.Text.Length > 0)
                        //{
                        //    string query = $"Update tbl_shifts SET ShiftStart = '{txtST.Text}', ShiftEnd = '{txtET.Text}', ShiftName = '{txt_shifName.Text}', MaxEndTime = '{MaxEndTime.Text}' Where Shiftid = {ShiftID}";
                        //    bool updated = DBFactory.Update(connString, query);
                        //    if (updated)
                        //    {
                        //        TaskCompleted();
                        //    }
                        //    else
                        //    {
                        //        TaskIncomplete();
                        //    }
                        //}
                        //else if (MaxEndTime.Text.Length.Equals(0) && MinStartTime.Text.Length > 0)
                        //{
                        //    string query = $"Update tbl_shifts SET ShiftStart = '{txtST.Text}', ShiftEnd = '{txtET.Text}', ShiftName = '{txt_shifName.Text}', MinStartTime = '{MinStartTime.Text}' Where Shiftid = {ShiftID}";
                        //    bool updated = DBFactory.Update(connString, query);
                        //    if (updated)
                        //    {
                        //        TaskCompleted();
                        //    }
                        //    else
                        //    {
                        //        TaskIncomplete();
                        //    }
                        //}
                        //else if(MinStartTime.Text.Length > 0 && MaxEndTime.Text.Length > 0)
                        //{
                        //    string query = $"Update tbl_shifts SET ShiftStart = '{txtST.Text}', ShiftEnd = '{txtET.Text}', ShiftName = '{txt_shifName.Text}', MinStartTime = '{MinStartTime.Text}', MaxEndTime = '{MaxEndTime.Text}' Where Shiftid = {ShiftID}";
                        //    bool updated = DBFactory.Update(connString, query);
                        //    if (updated)
                        //    {
                        //        TaskCompleted();
                        //    }
                        //    else
                        //    {
                        //        TaskIncomplete();
                        //    }
                        //}
                        //else
                        //{
                        //string query = "Update tbl_shifts SET ShiftStart = '" + txtST.Text + "', ShiftEnd = '" + txtET.Text + "', ShiftName = '" + txt_shifName.Text + "' Where Shiftid = " + ShiftID + "";
                        string query = "Update tbl_shifts SET ShiftStart = '" + txtST.Text + "', ShiftEnd = '" + txtET.Text + "', ShiftName = '" + txt_shifName.Text + "' Where Shiftid = " + ShiftID + "";
                        bool updated = DBFactory.Update(connString, query);
                            if (updated)
                            {
                                TaskCompleted();
                            }
                            else
                            {
                                TaskIncomplete();
                            }

                        //}
                    }
                    catch(Exception)
                    {
                        TaskIncomplete();
                    }
                    
                }
                else
                {
                    //TimeSpan shiftStart = TimeSpan.Parse(txtST.Text);
                    //TimeSpan shiftEnd = TimeSpan.Parse(txtET.Text);
                    //TimeSpan minStartTime = IsTimeSpan(MinStartTime.Text) ? TimeSpan.Parse(MinStartTime.Text) : TimeSpan.Parse("00:00");
                    //TimeSpan maxEndTime = IsTimeSpan(MaxEndTime.Text) ? TimeSpan.Parse(MaxEndTime.Text) : TimeSpan.Parse("00:00");
                    try
                    {
                        //if (shiftStart > minStartTime && shiftEnd < maxEndTime) {
                        //    string query = "insert into tbl_shifts (ShiftStart,ShiftEnd,ShiftName, MinStartTime, MaxEndTime ) values ('" + txtST.Text + "','" + txtET.Text + "','" + txt_shifName.Text + "', '" + minStartTime + "', '" + maxEndTime + "')";
                        //    _ = DBFactory.Insert(connString, query);
                        //    TaskCompleted();
                        //}
                        //else
                        //{
                        //    minStartTime = shiftStart - TimeSpan.FromMinutes(15);
                        //    maxEndTime = shiftEnd + TimeSpan.FromMinutes(15);
                        //    string query = "insert into tbl_shifts (ShiftStart,ShiftEnd,ShiftName, MinStartTime, MaxEndTime ) values ('" + txtST.Text + "','" + txtET.Text + "','" + txt_shifName.Text + "', '" + minStartTime + "', '" + maxEndTime + "')";
                        //    _ = DBFactory.Insert(connString, query);
                        //    TaskCompleted();
                        //}
                        string query = $"insert into tbl_shifts (ShiftStart, ShiftEnd, ShiftName) values('{txtST.Text}', '{txtET.Text}', '{txt_shifName.Text}')";
                        if(DBFactory.Insert(connString, query) == 1)
                        {
                            TaskCompleted();
                        }
                    }
                    catch (Exception)
                    {
                        TaskIncomplete();
                    }
                }
                
                
                //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                //message.btnCancel.Visibility = Visibility.Collapsed;
                //message.btnOk.Content = "Ok";
                //_ = message.ShowDialog();
            }
            UD_List();
            ClearTextBoxes();
            TextBoxDefaults();
            DisableControls();
            BtnModifiedClicked = false;
            //txtShif.Visibility = Visibility.Visible;
            //txtST1.Visibility = Visibility.Visible;
            //txtET1.Visibility = Visibility.Visible;
        }

        

        


        private void ClearTextBoxes()
        {
            txt_shifName.Clear();
            txtST.Clear();
            txtET.Clear();
            //MinStartTime.Clear();
            //MaxEndTime.Clear();
        }
        public void UAS_List()
        {
            if (ShiftAssignlist.ItemsSource == null || ShiftAssignlist.Items.Count > 0)
            {
                UdP = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt;
                if (DBFactory.ConnectServer(connString))
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    //new query
                    //
                    dt = DBFactory.GetAllByQuery(connString, "SELECT * FROM tbl_enroll where ShiftId is not NULL");

                    UdP = ConvertDataTableObs<UdownDisp>(dt);
                    ShiftAssignlist.ItemsSource = UdP;

                }
                else
                {
                    ShiftAssignlist.ItemsSource = null;
                }
            }
        }



        public void US_List()
        {
            if (dbuserlist.ItemsSource == null || dbuserlist.Items.Count > 0)
            {
                UdP = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connString);
                if (con)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    //new query
                    //
                    dt = DBFactory.GetAllByQuery(connString, "SELECT * FROM tbl_enroll where ShiftId is NULL order by EnrollNumber asc");

                    UdP = ConvertDataTableObs<UdownDisp>(dt);
                    dbuserlist.ItemsSource = UdP;

                }
                else
                {
                    dbuserlist.ItemsSource = null;
                    
                }
                
            }
        }


        public void UD_List()
        {
            Shiftlist.ItemsSource = null;
            if (Shiftlist.ItemsSource == null || Shiftlist.Items.Count > 0)
            {
                EnD = new ObservableCollection<shift>();//List<gdevId>();
                DataTable dt;
                if (DBFactory.ConnectServer(connString))
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    //new query
                    //
                    dt = DBFactory.GetAllByQuery(connString, "SELECT* FROM tbl_shifts");

                    EnD = ConvertDataTableObs<shift>(dt);
                    Shiftlist.ItemsSource = EnD;

                }
                else
                {
                    Shiftlist.ItemsSource = null;
                }
            }
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
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    
                    if (pro.Name == column.ColumnName)
                    {
                        if (dr[column.ColumnName]== DBNull.Value)
                        {
                            if(pro.Name =="DptID" && dr[column.ColumnName] == DBNull.Value)
                            {
                                continue;
                            }
                            else if(pro.Name == "FPData" && dr[column.ColumnName] == DBNull.Value){
                                pro.SetValue(obj, "0", null);
                            }
                            else
                            {
                                pro.SetValue(obj, 0, null);

                            }
                        }
                        
                        else
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                    }
                        
                    else
                        continue;
                }
            }
            return obj;
        }

        private void search_text_changed(object sender, TextChangedEventArgs e)
        {
            //txt_search1.Visibility = Visibility.Collapsed;
            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dbuserlist.ItemsSource);
            //view.Filter = UserFilter;
            if (txt_search.Text.Length.Equals(0) && dbuserlist != null)
            {
                CollectionViewSource.GetDefaultView(dbuserlist.ItemsSource).Refresh();
            }

            //if (dbuserlist != null)
            //{
            //CollectionViewSource.GetDefaultView(dbuserlist.ItemsSource).Refresh();
            //}
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txt_search.Text))
                return true;
            else
                return ((item as UdownDisp).EnrollNumber.ToString().IndexOf(txt_search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void clrst_textchnged(object sender, TextChangedEventArgs e)
        {
            txtST1.Visibility = Visibility.Collapsed;
        }

        private void clret_textchnaged(object sender, TextChangedEventArgs e)
        {
            txtET1.Visibility = Visibility.Collapsed;
        }

        private void refreshClick(object sender, MouseButtonEventArgs e)
        {
            UD_List();
            UAS_List();
            US_List();
        }

       

        private void txt_shifName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtShif.Visibility = Visibility.Collapsed;
        }

        //private void btnDeleteShift_Click(object sender, RoutedEventArgs e)
        //{
        //    string query;
        //    foreach (shift st in Shiftlist.SelectedItems)
        //    {
        //        foreach (UdownDisp item in dbuserlist.ItemsSource)
        //        {
        //            query = "delete from tbl_shifts where ShiftId=" + st.ShiftId;
        //            DBFactory.Delete(connString, query);
        //        }
        //    }
        //    StatusWindow.Text = "Shift Deleted !!!";
        //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
        //    message.btnCancel.Visibility = Visibility.Collapsed;
        //    message.btnOk.Content = "Ok";
        //    message.ShowDialog();
        //    UD_List();
        //}

        

        private void txtSTart_GotFocus(object sender, RoutedEventArgs e)
        {
            txtST1.Visibility = Visibility.Collapsed;
        }

        private void txtEnd_GotFocus(object sender, RoutedEventArgs e)
        {
            txtET1.Visibility = Visibility.Collapsed;
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtShif.Visibility = Visibility.Collapsed;
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_shifName.Text.Length == 0)
            {
            txtShif.Visibility = Visibility.Visible;
            }
        }

        private void txtEnd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtET.Text.Length.Equals(0))
            {
                txtET1.Visibility = Visibility.Visible;
            }
        }

        private void txtSTart_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtST.Text.Length.Equals(0))
            {
                txtST1.Visibility = Visibility.Visible;
            }
        }
        private void DisableControls() 
        {
            txtST.IsEnabled = false;
            txtET.IsEnabled = false;
            txt_shifName.IsEnabled = false;
            btnCreateShift.IsEnabled = false;
            //MaxEndTime.IsEnabled = false;
            //MinStartTime.IsEnabled = false;
        }
        private void EnableControls()
        {
            txtST.IsEnabled = true;
            txtET.IsEnabled = true;
            txt_shifName.IsEnabled = true;
            btnCreateShift.IsEnabled = true;
            //MinStartTime.IsEnabled = true;
            //MaxEndTime.IsEnabled = true;
        }
        private void TextBoxDefaults() 
        {
            txtET1.Visibility = Visibility.Visible;
            txtST1.Visibility = Visibility.Visible;
            txtShif.Visibility = Visibility.Visible;
            //MinOverlay.Visibility = Visibility.Visible;
            //MaxOverlay.Visibility = Visibility.Visible;
        }
        private void CreateShift_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EnableControls();
            _ = txtST.Focus();
        }
        private void ModifyShift_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Shiftlist.SelectedItems.Count.Equals(1))
            {
                PopulateToFields();
                
            }
            else
            {
                TaskIncomplete();
            }
        }
        private void PopulateToFields()
        {
            foreach (shift item in Shiftlist.SelectedItems)
            {
                ShiftID = item.ShiftId;
                txtST.Text = item.ShiftStart.ToString().Substring(0, 5);
                txtET.Text = item.ShiftEnd.ToString().Substring(0, 5);
                //MinStartTime.Text = item.MinStartTime.ToString().Substring(0, 5);
                //MaxEndTime.Text = item.MaxEndTime.ToString().Substring(0, 5);
                txt_shifName.Text = item.ShiftName.ToString();
                EnableControls();
                BtnModifiedClicked = true;
                TaskCompleted();
            }
        }
        private void DeleteShift_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                shift SelectedShift = Shiftlist.SelectedItem as shift;
                if(SelectedShift != null)
                {
                    bool deleted = DBFactory.Delete(connString, "DELETE from tbl_shifts where Shiftid = " + SelectedShift.ShiftId + "");
                    if (deleted)
                    {
                        _ = EnD.Remove(EnD.FirstOrDefault(shift => shift.ShiftId == SelectedShift.ShiftId));
                        ClearTextBoxes();
                        TextBoxDefaults();
                        DisableControls();
                        BtnModifiedClicked = false;
                        List<int> shiftIDList = new List<int>();

                        using (SqlConnection connection = new SqlConnection(connString))
                        {
                            connection.Open();

                            string query = "SELECT ShiftId FROM tbl_enroll";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        // Read the ShiftID value from the reader and add it to the list
                                        if (reader["ShiftID"] != DBNull.Value)
                                        {
                                            int shiftID = (int)reader["ShiftID"];
                                            shiftIDList.Add(shiftID);
                                        }
                                    }
                                }
                            }
                        }
                        if (!shiftIDList.Contains(SelectedShift.ShiftId))
                        {
                            TaskCompleted();
                            return;
                        }
                        if (ShiftAssignlist.Items.Count > 0)
                        {
                            string query = "update tbl_enroll set ShiftId = NULL where ShiftId=" + SelectedShift.ShiftId;
                            bool ShiftRemoved = DBFactory.Update(connString, query);
                            if (ShiftRemoved)
                            {
                                dbuserlist.ItemsSource = null;
                                ShiftAssignlist.ItemsSource = null;
                                US_List();
                                UAS_List();
                                TaskCompleted();
                            }
                            else
                            {
                                TaskIncomplete();
                            }
                        }
                        else
                        {
                            TaskIncomplete();
                        }
                    }
                    else
                    {
                        TaskIncomplete();
                    }
                    
                }
                else
                {
                    TaskIncomplete();
                }

            }
            catch(Exception)
            {
                TaskIncomplete();
            }
        }
        
        private void TaskIncomplete()
        {
            StatusWindow.Text = "Task Incomplete";
            StatusWindow.Foreground = Brushes.Red;
            Thread newThread = new Thread(ClearStatusWindows);
            newThread.Start();
        }
        private void TaskCompleted() 
        {
            StatusWindow.Text = "Task Completed";
            StatusWindow.Foreground = Brushes.Green;
            Thread newThread = new Thread(ClearStatusWindows);
            newThread.Start();
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

        private void ShiftList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BtnModifiedClicked)
            {
                PopulateToFields();
                
            }
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search1.Visibility = Visibility.Collapsed;
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_search.Text.Length.Equals(0))
            {
                txt_search1.Visibility = Visibility.Collapsed;
            }
        }

        //private void Search_Click(object sender, MouseButtonEventArgs e)
        //{
           
        //}

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dbuserlist.ItemsSource);
            view.Filter = UserFilter;


            if (dbuserlist != null)
            {
                CollectionViewSource.GetDefaultView(dbuserlist.ItemsSource).Refresh();
            }
        }

        private void MinStartTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //MinOverlay.Visibility = Visibility.Collapsed;
        }

        private void MinStartTime_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (MinStartTime.Text.Length.Equals(0))
            //{
            //    MinOverlay.Visibility = Visibility.Visible;
            //}
        }

        private void MaxEndTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //MaxOverlay.Visibility = Visibility.Collapsed;
        }

        private void MaxEndTime_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (MaxEndTime.Text.Length.Equals(0))
            //{
            //    MaxOverlay.Visibility = Visibility.Visible;
            //}
        }

        private void MinStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MinOverlay.Visibility = Visibility.Collapsed;
        }

        private void MaxEndTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MaxOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
