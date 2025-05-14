using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UsmanCodeBlocks.Data.Sql.Local;


namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        ObservableCollection<DevInfo> lstDevinfo;
        ObservableCollection<tbl_DevGroups> DevGroupList;
        public string Constring;
        public string txtip;
        public string txthwpin;
        public string txtDevDes;
        public string txtserial;
        string type;
        public int txtid;
        bool performingInsertion = false;
        bool performingDeletion = false;
        public int selectedDevID = 0;
        public string query;
        static bool flag = false;
        string del_id;
        DevInfo itemStat;
        public string masterId;


        public UserControl1()
        {
            InitializeComponent();
        }
        public UserControl1(string ConString)
        {
            InitializeComponent();
            Constring = ConString;
        }

        private bool GetSerialNum(string ip, int id)
        {
            bool bRet;
            bRet = DisableDevice(ip,id);
            if (!bRet)
            {
                txtSerial.Text = "Wrong Machine Id " + id + " or Device not Active!";
                return false;
            }

            string str = "";
            bRet = axFP_CLOCK.GetSerialNumber(id, ref str);
            if (bRet)
            {
                txtSerial.Text = str;
            }
            else
            {
                ShowErrorInfo();
                return false;
            }

            EnableDevice(id);
            return true;
        }

        private bool DisableDevice(string ip, int id)
        {
            //condev("192.168.0.111", 2);
            
            if (condev(ip, id))
            {
                bool bRet = axFP_CLOCK.EnableDevice(id, 0);
                if (bRet)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }

            
        }

        private void EnableDevice(int id)
        {

            axFP_CLOCK.EnableDevice(id, 1);
        }
        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            axFP_CLOCK.GetLastError(ref nErrorValue);

        }

        private void AddDev_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (flag)
            {
                reset_controls();
                enable_controls();
                flag = false;
            }
            else
            {
                enable_controls();
                inDes.IsChecked = true;
                isRemote.IsChecked = false;
            }
            _ = btngetserial.Focus();
            _ = txtIP.Focus();



        }
        public void enable_controls()
        { 
           
            txtDevID.IsReadOnly = false;
            txtHWpin.IsReadOnly = false;            
            txtIP.IsReadOnly = false;
            txtdevLoc.IsReadOnly = false;
            txtSerial.IsReadOnly = true;
            btnsave.IsEnabled = true;
            btngetserial.IsEnabled = true;
            inDes.IsEnabled = true;
            outDes.IsEnabled = true;
            isRemote.IsEnabled = true;

        }
        public void disable_controls()
        {
            btnsave.Focus();
            inDes.IsEnabled = false;
            outDes.IsEnabled = false;
            txtDevID.IsReadOnly = true;
            txtHWpin.IsReadOnly = true;
            txtIP.IsReadOnly = true;
            txtSerial.IsReadOnly = true;
            txtdevLoc.IsReadOnly = true ;
            btnsave.IsEnabled = false;
            isRemote.IsEnabled= false;
            btngetserial.IsEnabled = false;
        }
        public void reset_controls()
        {
            txtDevID.Text= "";
            txtHWpin.Text= "";
            txtIP.Text = "";
            txtSerial.Text= "";
            txtdevLoc.Text = "";
            inDes.IsChecked = false;
            outDes.IsChecked = false;
            isRemote.IsChecked = false;            
            txtDevID1.Visibility = Visibility.Visible;
            txtHWpin1.Visibility = Visibility.Visible;
            txtIP1.Visibility = Visibility.Visible;
            txtSerial1.Visibility = Visibility.Visible;
            devdes.Visibility = Visibility.Visible;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lvdevinfo.Items.Clear();
            dev_list();
            LoadGroupList();
        }
        private void saveRemoteDevice()
        {
            if (String.IsNullOrEmpty(txtIP.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device IP", false);
                msg.ShowDialog();
                txtIP.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtDevID.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device ID", false);
                msg.ShowDialog();
                txtDevID.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtdevLoc.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device Location", false);
                msg.ShowDialog();
                txtdevLoc.Focus();
                return;
            }
            int id1;
            if (!(int.TryParse(txtDevID.Text, out id1)))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Numeric Device ID", false);
                msg.ShowDialog();
                return;
            }
            IPAddress ip;
            bool ValidateIP = IPAddress.TryParse(txtIP.Text, out ip);
            if (!ValidateIP)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter a Valid IP Address", false);
                msg.ShowDialog();
                txtIP.Focus();
                return;
            }
            Co_Wait.Visibility = Visibility.Visible;

            string query;
            if (flag)
            {

                try
                {
                    query = $"update tbl_RemoteDevices set MId = {txtDevID.Text}, Ip = '{txtIP.Text}', Location = '{txtdevLoc.Text}' where MId = {selectedDevID} ";
                    bool Updated = DBFactory.Update(Constring, query);
                    if (!Updated)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error While Updating !!! Check Input Fields", false);
                        return;
                    }
                    flag = false;
                }catch(Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            else
            {
                foreach (DevInfo dev in lvdevinfo.Items)
                {
                    if (dev.MId == id1)
                    {
                        MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Device With Same ID Already Exist");
                        msg.ShowDialog();
                        txtDevID.Focus();
                        return;
                    }
                }
                try
                {
                    query = $"insert into tbl_RemoteDevices (MId, Ip, Location) values ({txtDevID.Text}, '{txtIP.Text}', '{txtdevLoc.Text}')";
                    int rowsAffected = DBFactory.Insert(Constring, query);
                    if (rowsAffected == 0)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error While Inserting !!! Check Input Fields", false);
                        return;
                    }                    
                }catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            OnSaveCompleted();

        }
        private void btnsave_Click(object sender, RoutedEventArgs e)
        {            
            if ((bool)isRemote.IsChecked)
            {
                performingInsertion = true;
                saveRemoteDevice();
                performingInsertion = false;
                return;
            }
            if (String.IsNullOrEmpty(txtIP.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device IP", false);
                msg.ShowDialog();
                txtIP.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtDevID.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device ID", false);
                msg.ShowDialog();
                txtDevID.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtHWpin.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Hardware Pin", false);
                msg.ShowDialog();
                txtHWpin.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtdevLoc.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device Location", false);
                msg.ShowDialog();
                txtdevLoc.Focus();
                return;
            }
            if (String.IsNullOrEmpty(type))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Select Device Type", false);
                msg.ShowDialog();
                return;
            }
            Co_Wait.Visibility = Visibility.Visible;



            //int id1 = int.Parse(txtDevID.Text);
            int id1;
            if(!(int.TryParse(txtDevID.Text, out id1)))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Numeric Device ID", false);
                msg.ShowDialog();
                return;
            }
            string ip1 = txtIP.Text;
            if (GetSerialNum(ip1,id1))
            {
                try
                {
                    txtip = txtIP.Text.ToString().Trim();
                    txthwpin = txtHWpin.Text.ToString().Trim();
                    txtserial = txtSerial.Text.ToString().Trim();
                    txtid = Convert.ToInt32(txtDevID.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Invalid Format\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            else
            {

                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Wrong Dev info\n" , false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            
            
            DateTime status = DateTime.Now;
            string strDate = status.ToString("yyyy - MM - dd HH: mm: ss");
            txtDevDes = txtdevLoc.Text;

            if (flag)
            {
                try 
                {
                    string updateQuery = $"update tbl_dev set MId = {txtid}, Ip = '{txtip}', Status = '{strDate}', Serial = '{txtserial}', HWpin = '{txthwpin}', DevDescription = '{type}', Location = '{txtDevDes}' where MId = {selectedDevID} ";
                    bool updated = DBFactory.Update(Constring, updateQuery);
                    if (!updated)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error While Updating !!! Check Input Fields", false);
                        return;
                    }
                    flag = false;
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    
                }
            }
            else
            {
                List<string> Serials = new List<string>();
                List<int> NodeIDs = new List<int>();
                if (lvdevinfo.Items.Count > 0)
                {
                    foreach(DevInfo item in lvdevinfo.Items)
                    {
                        Serials.Add(item.Serial.ToString());
                        NodeIDs.Add(item.MId);
                    }
                }
                if (NodeIDs.Contains(Convert.ToInt32(txtDevID.Text)))
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Device With Same ID Already Exist", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                if (Serials.Contains(txtserial)) 
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Device Info Already Exist", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }

            try
            {
                IPAddress ip;
                bool ValidateIP = IPAddress.TryParse(txtIP.Text, out ip);

                if (Dev_Validation.match_serial(txtserial, txthwpin) && ValidateIP==true)
                {
                    if (inDes.IsChecked == true)
                    {
                        DBFactory.Insert(Constring, "INSERT INTO tbl_dev(MId,Ip,Status,Serial,HWpin,DevDescription,Location, GroupID)VALUES(" + txtid + ",'" + txtip + "','" + strDate + "','" + txtserial + "','" + txthwpin + "','" + type + "','" + txtDevDes + "', " + 0 + ")");
                    }
                    if (outDes.IsChecked == true)
                    {
                        DBFactory.Insert(Constring, "INSERT INTO tbl_dev(MId,Ip,Status,Serial,HWpin,DevDescription,Location, GroupID)VALUES(" + txtid + ",'" + txtip + "','" + strDate + "','" + txtserial + "','" + txthwpin + "','" + type + "','" + txtDevDes + "', " + 0 + ")");
                    }
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Invalid HWpin OR Ip\n" , false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                
            }
            catch(Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Dev Id Already Exist\n" +ex.Message  , false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            }
            OnSaveCompleted();
        }
        private void OnSaveCompleted()
        {
            dev_list();
            disable_controls();
            reset_controls();
            statusbar_green();
            txtIP1.Visibility = Visibility.Visible;
            txtHWpin1.Visibility = Visibility.Visible;
            txtSerial1.Visibility = Visibility.Visible;
            txtDevID1.Visibility = Visibility.Visible;
            devdes.Visibility = Visibility.Visible;
            Co_Wait.Visibility = Visibility.Collapsed;
        }
        #region dev_data
        public void dev_list()
        {
            DevicesList.Items.Clear();
            lstDevinfo = new ObservableCollection<DevInfo>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(Constring);
            if (con)
            {
                dt = DBFactory.GetAllByQuery(Constring, "SELECT  * from tbl_dev ORDER BY MId ASC");
                DataTable GroupDT = DBFactory.GetAllByQuery(Constring, "Select * from tbl_DevGroups ");
                lstDevinfo = new ObservableCollection<DevInfo>();
                ObservableCollection<DevInfo> SavedDevices = ConvertDataTableObs<DevInfo>(dt);
                foreach(DevInfo dev in SavedDevices)
                {
                    string serial = dev.Serial;
                    string hwpin = dev.HWpin;
                    if (Dev_Validation.match_serial(serial, hwpin))
                    {
                        lstDevinfo.Add(dev);
                    }
                }

                foreach (DevInfo dev in lstDevinfo)
                {
                    dev.isRemote = false;
                    if (dev.GroupID == 0)
                    {
                        _ = DevicesList.Items.Add(dev);
                    }
                    else
                    {
                        foreach (DataRow group in GroupDT.Rows)
                        {
                            if (Convert.ToInt32(group["ID"]) == dev.GroupID)
                            {
                                dev.GroupName = group["Group_Name"].ToString();
                            }
                        }
                    }
                }
                //fetch remote devices and merge to show in the devices list
                int highestDid = lstDevinfo.Count == 0 ? 0 :lstDevinfo.Max(obj => obj.DId);
                DataTable remoteDevDT = DBFactory.GetAllByQuery(Constring, "Select * from tbl_RemoteDevices");
                DevInfo remoteDev;
                foreach (DataRow dev in remoteDevDT.Rows)
                {   
                    highestDid++;
                    remoteDev = new DevInfo();
                    remoteDev.isRemote = true;
                    remoteDev.HWpin = "--------";
                    remoteDev.Ip = dev["Ip"].ToString();
                    remoteDev.Serial = "--------";
                    remoteDev.DId = highestDid;
                    remoteDev.DevDescription = "--------";
                    remoteDev.GroupID = 0;
                    remoteDev.GroupName = "Remote";
                    remoteDev.Location = dev["Location"].ToString();
                    remoteDev.MId = Convert.ToInt32(dev["MId"]);
                    lstDevinfo.Add(remoteDev);
                }
                mchCount.Content = lstDevinfo.Count;
                lvdevinfo.ItemsSource = lstDevinfo;

            }
            else
            {
                lvdevinfo.ItemsSource = null;
            }
        }
        public void LoadGroupList()
        {
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(Constring);
            if (con)
            {
                dt = DBFactory.GetAllByQuery(Constring, "Select * from tbl_DevGroups Order by ID ASC");
                DevGroupList = ConvertDataTableObs<tbl_DevGroups>(dt);
                GroupList.ItemsSource = DevGroupList;
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
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        #endregion

        private void btnOnline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = lstDevinfo.Count();
                int remoteCount = 0;
                foreach (DevInfo dev in lstDevinfo)
                {
                    if (dev.isRemote)
                    {
                        remoteCount++;
                    }
                }
                if (count == remoteCount)
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "All Devices Are Remote, Unable to Check Online Status.", false);
                    msg.btnCancel.Visibility = Visibility.Collapsed;
                    msg.btnOk.Content = "Ok";
                    msg.ShowDialog(); return;
                }

                Co_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                message.ShowDialog();
                int i = 0;

                foreach (DevInfo items in lstDevinfo)
                {
                    if (items.isRemote)
                    {
                        continue;
                    }
                    DevInfo product = (DevInfo)lvdevinfo.Items.GetItemAt(i);
                    if (condev(items.Ip, items.MId))
                    {
                        product.enabled = true;
                        product.disabled = false;
                        i++;
                    }
                    else
                    {
                        product.enabled = false;
                        product.disabled = true;
                        i++;
                    }
                }

                ICollectionView view = CollectionViewSource.GetDefaultView(lvdevinfo.ItemsSource);
                view.Refresh();
                Co_Wait.Visibility = Visibility.Collapsed;
                statusbar_green();
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, ".net runtime error\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
        }
        public bool condev(string ip, int id)
        {
            bool bRet;
            bRet = axFP_CLOCK.SetIPAddress(ref ip, 5005, 0);
            bRet = axFP_CLOCK.OpenCommPort(id);
            if (bRet)
            {
                statusbar_green();
                return true;
            }
            else
            {
                statusbar_red();
                return false;
            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvdevinfo.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Device.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();                return;
            }
            else if (lvdevinfo.SelectedItems.Count.Equals(1))
            {
                
                DevInfo dev = (DevInfo)lvdevinfo.SelectedItem;
                if (!dev.isRemote)
                {
                    selectedDevID = dev.MId;
                    txtDevID1.Visibility = Visibility.Collapsed;
                    txtHWpin1.Visibility = Visibility.Collapsed;
                    txtIP1.Visibility = Visibility.Collapsed;
                    txtSerial1.Visibility = Visibility.Collapsed;
                    devdes.Visibility = Visibility.Collapsed;
                    PopulateFields();
                    enable_controls();
                }
                else
                {
                    selectedDevID = dev.MId;
                    PopulateRemoteFields();
                }
                flag = true;

            }

            else {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please select a single device to modify.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }
        private void PopulateFields()
        {
            foreach (DevInfo record in lvdevinfo.SelectedItems)
            {
                txtDevID.Text = record.MId.ToString();
                masterId = record.MId.ToString();
                txtSerial.Text = record.Serial.ToString();
                txtIP.Text = record.Ip.ToString();
                txtHWpin.Text = record.HWpin.ToString();
                txtdevLoc.Text = record.Location.ToString();
                if (record.DevDescription.ToString() == "IN")
                {
                    inDes.IsChecked = true;
                }
                else
                {
                    outDes.IsChecked = true;
                }
            }
            txtSerial.IsReadOnly = true;
            txtHWpin.IsReadOnly = true;
            txtDevID.IsReadOnly = true;
            statusbar_green();
        }
        private void PopulateRemoteFields()
        {
            DevInfo selectedDev = lvdevinfo.SelectedItem as DevInfo;
            txtDevID.Text = selectedDev.MId.ToString();txtDevID1.Visibility = Visibility.Collapsed;txtDevID.IsReadOnly = true;
            txtIP.Text = selectedDev.Ip.ToString();txtIP1.Visibility = Visibility.Collapsed;txtIP.IsReadOnly = false;
            txtdevLoc.Text = selectedDev.Location.ToString(); devdes.Visibility = Visibility.Collapsed;txtdevLoc.IsReadOnly = false;
            txtHWpin1.Visibility = Visibility.Visible;txtHWpin.Text = string.Empty;txtHWpin.IsReadOnly = true;
            inDes.IsChecked = false;outDes.IsChecked=false;
            inDes.IsEnabled= false;inDes.IsEnabled= false;
            txtSerial.Text= string.Empty;txtSerial1.Visibility=Visibility.Visible;
            isRemote.IsChecked = true;isRemote.IsEnabled= false;
            btngetserial.IsEnabled= false;
            btnsave.IsEnabled = true;

        }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (lvdevinfo.SelectedItems.Count == 0)
            {
                statusbar_red();
                return;
            }
            else
            {
                foreach (DevInfo record in lvdevinfo.SelectedItems)
                {
                    del_id = record.MId.ToString();
                }

                try
                {
                    string query = "Delete from tbl_dev where MId =" + del_id;
                    DBFactory.Delete(Constring, query);
                    
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                statusbar_green();
                dev_list();
            }

        }
        public void statusbar_green() 
        {
            StatusWindow.Content ="Task Completed";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Green);
            Thread thread = new Thread(ClearStatusWindows);
            thread.Start();

        }


        public void statusbar_red()
        {
            StatusWindow.Content = "Task Not Completed";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Red);
            Thread thread = new Thread(ClearStatusWindows);
            thread.Start();
        }
        public void ClearStatusWindows()
        {
            Thread.Sleep(2000);
            Action updateUI = () =>
            {
                StatusWindow.Content = "";
            };
            Dispatcher.Invoke(updateUI);
        }

        private void getSerialOnChange(object sender, TextChangedEventArgs e)
        {
            if(btnsave!=null && btnsave.IsEnabled==false && txtIP.Text.Length>=7 && txtDevID.Text.Length >= 1)
            {
                GetSerialNum(txtIP.Text, int.Parse(txtDevID.Text));
            }
        }

        private void serial_onclick(object sender, MouseButtonEventArgs e)
        {
            txtSerial.Text = "";
        }

        private void empty_dev_id(object sender, MouseButtonEventArgs e)
        {
            txtDevID.Text = "";
        }

        private void empty_dev_id(object sender, MouseEventArgs e)
        {
            txtDevID.Text = "";
        }


        private void empty_dev_id(object sender, TextCompositionEventArgs e)
        {
            txtDevID.Text = "";
        }

        private void empty_hw_pin(object sender, TextCompositionEventArgs e)
        {
            txtHWpin.Text = "";
        }



        private void txtSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtIP1.Visibility = Visibility.Collapsed;
        }

        private void txtIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtIP.IsReadOnly)
            {

                txtIP1.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSerial_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtSerial.IsReadOnly)
            {
                txtSerial1.Visibility = Visibility.Collapsed;

            }

        }

        private void txtDevID_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtDevID.IsReadOnly)
            {
                txtDevID1.Visibility = Visibility.Collapsed;

            }
        }

        private void txtHWpin_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtHWpin.IsReadOnly)
            {
            txtHWpin1.Visibility = Visibility.Collapsed;

            }
        }

        private void getserial_Click(object sender, RoutedEventArgs e)
        {

            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            _ = message.ShowDialog();

            IPAddress ip;
            bool ValidateIP = IPAddress.TryParse(txtIP.Text, out ip);
            if(txtDevID.Text=="" || txtIP.Text == "")
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Device Ip and Id !!! ", true);
                message1.ShowDialog();
            }
            else if (btnsave.IsEnabled == true && ValidateIP == true && txtDevID.Text.Length >= 1)
            {
                //int id1 = int.Parse(txtDevID.Text);
                int id1;
                if(!(int.TryParse(txtDevID.Text, out id1)))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Numeric Device ID", true);
                    msg.ShowDialog();
                    return;
                }
                string ip1 = txtIP.Text;


               bool SerialFetched = GetSerialNum(ip1, id1);

                if (SerialFetched) { 
                txtSerial1.Visibility = Visibility.Collapsed;
                btngetserial.IsEnabled = false;
                }
            }
            else
            {
                txtSerial1.Visibility = Visibility.Visible;
            }

            Wait.Visibility = Visibility.Collapsed;


        }

        private void Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvdevinfo.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Device.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                statusbar_red();
                return;
            }
            else
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to delete the device?", false); 
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                bool completelyDeleted = true;
                performingDeletion = true;
                foreach (DevInfo record in lvdevinfo.SelectedItems)
                {
                    del_id = record.MId.ToString();
                    try
                    {
                        string query;
                        if(record.isRemote)
                        {
                            query = "Delete from tbl_RemoteDevices where MId =" + del_id;
                        }
                        else
                        {
                            query = "Delete from tbl_dev where MId =" + del_id;
                        }                        
                        bool deleted = DBFactory.Delete(Constring, query);
                        completelyDeleted = completelyDeleted && deleted;

                    }
                    catch (Exception ex)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }
                }
                if (completelyDeleted)
                {
                    statusbar_green();
                    dev_list();
                    disable_controls();
                    reset_controls();
                }
                if (flag)
                {
                    flag = false;
                }
                performingDeletion = false;
            }
        }

        private void btnOnline_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvdevinfo.SelectedItems.Count == 0)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select a Device first.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                foreach (DevInfo dev in lvdevinfo.SelectedItems)
                {
                    if (dev.isRemote)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please Deselect Remote Device/Devices And Try Again.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        return;
                    }
                }
                Co_Wait.Visibility = Visibility.Visible;
                foreach (DevInfo record in lvdevinfo.SelectedItems)
                {
                    itemStat = record;
                    if (condev(itemStat.Ip, itemStat.MId))
                    {
                        itemStat.enabled = true;
                        itemStat.disabled = false;
                    }
                    else
                    {
                        itemStat.enabled = false;
                        itemStat.disabled = true;
                    }

                }
                ICollectionView view = CollectionViewSource.GetDefaultView(lvdevinfo.ItemsSource);
                view.Refresh();
                Co_Wait.Visibility = Visibility.Collapsed;
                statusbar_green();
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, ".net runtime error\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }

        }

        private void desc_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtdevLoc.IsReadOnly)
            {
                devdes.Visibility = Visibility.Collapsed;
            }
        }

        private void inDes_Checked(object sender, RoutedEventArgs e)
        {
            type = "IN";
        }

        private void outDes_Checked(object sender, RoutedEventArgs e)
        {
            type = "OUT";
        }

        private void txtIP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtIP.Text.Length == 0)
            {
                txtIP1.Visibility = Visibility.Visible;
            }
        }

        private void txtDevID_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtDevID.Text.Length == 0)
            {
                txtDevID1.Visibility = Visibility.Visible;
            }
        }

        private void txtSerial_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtSerial.Text.Length == 0)
            {
                txtSerial1.Visibility = Visibility.Visible;
            }
        }

        private void txtHWpin_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtHWpin.Text.Length == 0)
            {
                txtHWpin1.Visibility = Visibility.Visible;
            }
        }

        private void desc_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtdevLoc.Text.Length == 0)
            {
                devdes.Visibility = Visibility.Visible;
            }
        }

        private void lvdevinfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                flag = false;
                reset_controls();
                disable_controls();
            }
            if(lvdevinfo.SelectedItems.Count >= 1)
            {
                btnRemGrp.IsEnabled = true;
            }
            else
            {
                btnRemGrp.IsEnabled = false;
            }
            //if (flag && lvdevinfo.SelectedItems.Count.Equals(1))
            //{
            //    PopulateFields();
            //}
            //if(lvdevinfo.SelectedItems.Count >= 1)
            //{
            //    btnRemGrp.IsEnabled = true;
            //}
            //else
            //{
            //    btnRemGrp.IsEnabled = false;
            //}

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            flag = false;
        }

        private void AddGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GroupList.SelectedItem = null;
            Keyboard.Focus(txtGrpID);
            EnableGroupControls(true);

        }

        private void ModifyGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GroupList.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Select atleast one record to modify.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            else if (GroupList.SelectedItems.Count > 1)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "You can only modify one record at a time.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                GroupList.SelectedItems.Clear();
                return;
            }
            else
            {
                EnableGroupControls(true);
                txtGrpID.IsReadOnly = true;
                tbl_DevGroups group = (tbl_DevGroups)GroupList.SelectedItem;
                txtGrpID.Text = group.ID.ToString();
                txtGrpName.Text = group.Group_Name;
                btnSaveGroup.Content = "Update";
            }
        }

        private void DeleteGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(GroupList.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please select atleast one record to delete.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            else
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to delete the selected group?", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                tbl_DevGroups group = (tbl_DevGroups)GroupList.SelectedItem;
                string DelQuery = $"Delete from tbl_DevGroups Where ID = {group.ID}";
                bool deleted = DBFactory.Delete(Constring, DelQuery);
                if (deleted)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Group Deleted Successfully.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    bool updated = DBFactory.Update(Constring, $"update tbl_dev set GroupID = {0} where GroupID = {group.ID}");
                    if (updated)
                    {
                        dev_list();
                    }
                }
            }
            ResetGroupControls();
            EnableGroupControls(false);
            btnSaveGroup.Content = "Save";
            LoadGroupList();
        }
        private void EnableGroupControls(bool val)
        {
            txtGrpID1.Visibility = val ? Visibility.Collapsed : Visibility.Visible;
            txtGrpID.IsReadOnly = !val;
            txtGrpName1.Visibility = val ? Visibility.Collapsed : Visibility.Visible;
            txtGrpName.IsReadOnly = !val;
            btnSaveGroup.IsEnabled = val;
        }
        private void ResetGroupControls()
        {
            txtGrpID.Clear();
            txtGrpName.Clear();
        }
        private void btnSaveGrp_Click(object sender, RoutedEventArgs e)
        {
            if (txtGrpName.Text.Length == 0 || txtGrpID.Text.Length == 0)
            {
                if (txtGrpID.Text.Length == 0)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Group ID.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Group Name.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
            }
            int GroupID;
            if (!int.TryParse(txtGrpID.Text, out GroupID))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please enter numeric data in ID field.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            if (GroupList.Items.Count > 0)
            {
                foreach (tbl_DevGroups group in GroupList.Items)
                {
                    if (group.ID == GroupID && btnSaveGroup.Content.ToString() == "Save")
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Another group with the same ID is already created.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        return;
                    }
                    else if (group.ID != GroupID && group.Group_Name == txtGrpName.Text)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Same group with different ID is already created.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
            if (btnSaveGroup.Content.ToString() == "Update")
            {
                string UpdateQuery = $"Update tbl_DevGroups Set Group_Name = '{txtGrpName.Text}' Where ID = {GroupID}";
                bool UpdateSuccessfull = DBFactory.Update(Constring, UpdateQuery);
                if (UpdateSuccessfull)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Group Updated Successfully.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    btnSaveGroup.Content = "Save";
                }
            }
            else
            {
                string query = $"Insert Into tbl_DevGroups (ID, Group_Name) VALUES ({GroupID}, '{txtGrpName.Text}')";
                int rowsAffected = DBFactory.Insert(Constring, query);
                if (rowsAffected == 1)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Group Created Successfully.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }         
            EnableGroupControls(false);
            ResetGroupControls();
            LoadGroupList();
        }



        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 1)
            {
            }
        }

        private void DevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DevicesList.SelectedItems.Count >= 1 && GroupList.SelectedItems.Count == 1)
            {
                btnAssignGroup.IsEnabled = true;
            }
            else
            {
                btnAssignGroup.IsEnabled = false;
            }
        }

        private void btnAssignGrp_Click(object sender, RoutedEventArgs e)
        {
            int GroupID = ((tbl_DevGroups)GroupList.SelectedItem).ID;
            bool AssignedToAll = true;
            foreach (DevInfo dev in DevicesList.SelectedItems)
            {
                string UpdateQuery = $"update tbl_dev Set GroupID = {GroupID} Where Serial = '{dev.Serial}'";
                AssignedToAll = AssignedToAll && DBFactory.Update(Constring, UpdateQuery);
            }
            if (AssignedToAll)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Group Assigned Successfully.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                GroupList.SelectedItem = null;
                DevicesList.Items.Clear();
                dev_list();

            }
        }

        private void btnRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            bool GroupRemoved = true;
            foreach (DevInfo dev in lvdevinfo.SelectedItems)
            {
                if (!dev.isRemote)
                {
                    string updateQuery = $"update tbl_dev set GroupID = {0} where Serial = '{dev.Serial}'";
                    GroupRemoved = DBFactory.Update(Constring, updateQuery);
                }
                
            }
            if (GroupRemoved)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Group Removed Successfully.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                dev_list();
            }
        }

        private void isRemote_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!performingInsertion && !performingDeletion)
            {
                txtHWpin.IsReadOnly = false;
                inDes.IsEnabled = true;
                outDes.IsEnabled = true;
                inDes.IsChecked = true;
                btngetserial.IsEnabled = true;
            }
        }

        private void isRemote_Checked(object sender, RoutedEventArgs e)
        {
            txtSerial.Text = string.Empty;            
            txtSerial1.Visibility = Visibility.Visible;
            txtHWpin.Text = string.Empty;
            txtHWpin.IsReadOnly = true;
            txtHWpin1.Visibility = Visibility.Visible;
            inDes.IsEnabled = false;outDes.IsEnabled = false;
            inDes.IsChecked = false;outDes.IsChecked = false;
            btngetserial.IsEnabled = false;
        }
    }
}
