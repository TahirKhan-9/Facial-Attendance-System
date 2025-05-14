using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
//using MaterialDesignThemes.Wpf;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UsmanCodeBlocks.Data.Sql.Local;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for uRealTimeLog.xaml
    /// </summary>
    public partial class uRealTimeLog : UserControl
    {

        public string sql;
        public string connstring;
        public BitmapImage defaultImg;
        List<int> EnrollNumbers;
        private int nIndex = 0;
        List<int> selectedDevicesID = new List<int>();
        //public AxFP_CLOCKLib.AxFP_CLOCK pOcxObject;
        //int m_nMachineNum = 1;
        //int id = 2;
        //string ip = "192.168.0.111";

        ObservableCollection<gdevId> gId;
        DeviceManagement DeviceManagement { get; set; }
        public uRealTimeLog(string constring, DeviceManagement devManagement )
        {
            connstring = constring;
            DeviceManagement = devManagement;
            InitializeComponent();  
            this.axFPCLOCK_Svr1.OnReceiveGLogData += new AxFPCLOCK_SVRLib._DFPCLOCK_SvrEvents_OnReceiveGLogDataEventHandler(this.axFPCLOCK_Svr1_OnReceiveGLogData);
        }

        
        //public bool condev(string ip, int id)
        //{
        //    bool bRet;
        //    bRet = axFP_CLOCK.SetIPAddress(ref ip, 5005, 0);
        //    bRet = axFP_CLOCK.OpenCommPort(id);
        //    if (bRet)
        //    {
        //        //statusbar_green();
        //        return true;
        //    }
        //    else
        //    {
        //        //statusbar_red();
        //        return false;
        //    }
        //}



        //private void EnableDevice(int id)
        //{

        //    axFP_CLOCK.EnableDevice(id, 1);
        //}
        //private void ShowErrorInfo()
        //{
        //    int nErrorValue = 0;
        //    axFP_CLOCK.GetLastError(ref nErrorValue);

        //}


        public void dev_list()
        {
            //Devlist.ItemsSource = null;
            gId = new ObservableCollection<gdevId>();//List<gdevId>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connstring);
            if (con == true)
            {
                dt = DBFactory.GetAllByQuery(connstring, "SELECT  [MId],[Ip],[Location] from tbl_dev ");
                gId = ConvertDataTableObs<gdevId>(dt);
                Devlist.ItemsSource = gId;

            }
            else
            {
                Devlist.ItemsSource = null;
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

        private void OnLoadDevlist(object sender, RoutedEventArgs e)
        {
            //dev_list();
            Loaddev_list();
        }
        private void btnStartMonitoringClick(object sender, RoutedEventArgs e)
        {
            selectedDevicesID.Clear();
            if(!(txtPort.Text.ToString().Trim() == "7005"))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please Enter Correct Port No", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                return;
            }
            if(Devlist.SelectedItems.Count == 0 && btnStartMonitoring.Content.ToString() == "Start Monitoring")
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please Select a Machine To Start Monitoring", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                _ = message.ShowDialog();
                return;
            }
            else if(btnStartMonitoring.Content.ToString() == "Start Monitoring")
            {
                foreach(gdevId item in Devlist.SelectedItems)
                {
                    selectedDevicesID.Add(item.MId);
                }
            }
            if (defaultImg == null)
            {
                string path = Directory.GetCurrentDirectory() + "/Resources/userImage.png";
                defaultImg = ByteArrayToBitmapImage(File.ReadAllBytes(path));
            }
            if(btnStartMonitoring.Content.ToString() == "Start Monitoring")
            {
                Connect();
                txtPort.IsReadOnly = true;
                btnStartMonitoring.Content = "Stop Monitoring";
                Devlist.IsEnabled = false;
                DisableButtons();                
                
            }
            else if(btnStartMonitoring.Content.ToString() == "Stop Monitoring")
            {
                Disconnect();
                txtPort.IsReadOnly= false;
                btnStartMonitoring.Content = "Start Monitoring";
                Devlist.IsEnabled = true;
                realtimeImg.Source = defaultImg;
                EnableButtons();
            }

        }
        private void DisableButtons()
        {
            DeviceManagement.btnDevManagement.IsEnabled = false;
            DeviceManagement.btnStaffManage.IsEnabled = false;
            DeviceManagement.btnAddStaff.IsEnabled = false;
            DeviceManagement.btnAddDept.IsEnabled = false;
            DeviceManagement.btnAddDept.IsEnabled = false;
            DeviceManagement.btnLogM.IsEnabled = false;
            DeviceManagement.btnShiftDef.IsEnabled = false;
            DeviceManagement.btnReportmanage.IsEnabled = false;
            DeviceManagement.btnTimeSlots.IsEnabled = false;
            DeviceManagement.btnSettings.IsEnabled = false;
            DeviceManagement.btnReltime.IsEnabled = false;
            DeviceManagement.gridCloseApp.IsEnabled = false;
            DeviceManagement.btnFirebaseSync.IsEnabled = false;
        }
        private void EnableButtons()
        {
            DeviceManagement.btnDevManagement.IsEnabled = true;
            DeviceManagement.btnStaffManage.IsEnabled = true;
            DeviceManagement.btnAddStaff.IsEnabled = true;
            DeviceManagement.btnAddDept.IsEnabled = true;
            DeviceManagement.btnAddDept.IsEnabled = true;
            DeviceManagement.btnLogM.IsEnabled = true;
            DeviceManagement.btnShiftDef.IsEnabled = true;
            DeviceManagement.btnReportmanage.IsEnabled = true;
            DeviceManagement.btnTimeSlots.IsEnabled = true;
            DeviceManagement.btnSettings.IsEnabled = true;
            DeviceManagement.btnReltime.IsEnabled = true;
            DeviceManagement.gridCloseApp.IsEnabled = true;
            DeviceManagement.btnFirebaseSync.IsEnabled=true;
        }
        private void Connect()
        {
            String str = this.txtPort.Text.Trim();
            int nPort = Convert.ToInt32(str);
            this.axFPCLOCK_Svr1.OpenNetwork(nPort);
        }
        private void Disconnect()
        {
            String str = this.txtPort.Text.Trim();
            int nPort = Convert.ToInt32(str);
            this.axFPCLOCK_Svr1.CloseNetwork(nPort);

        }
        private void axFPCLOCK_Svr1_OnReceiveGLogData(object sender, AxFPCLOCK_SVRLib._DFPCLOCK_SvrEvents_OnReceiveGLogDataEvent e)
        {
            if (selectedDevicesID.Contains(e.vnDeviceID))
            {
                if (e.anSEnrollNumber > 0)
                {
                    int imagelen = 0;
                    int[] imagebuff = new int[200 * 1024];
                    bool bRet;
                    IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(imagebuff.Length);
                    bRet = this.axFPCLOCK_Svr1.GetLogImageCS(e.linkindex, ref imagelen, ptrIndexFacePhoto);
                    if (bRet && imagelen > 0)
                    {
                        byte[] mbytCurEnrollData = new byte[imagelen];
                        Marshal.Copy(ptrIndexFacePhoto, mbytCurEnrollData, 0, imagelen);
                        realtimeImg.Source = ByteArrayToBitmapImage(mbytCurEnrollData);
                        //System.IO.File.WriteAllBytes(@"C:\\PHOTO\" + e.anSEnrollNumber.ToString() + "_" + e.anLogDate.ToString("yy_MM_dd_HH_mm_ss") + ".jpg", mbytCurEnrollData);
                    }
                    else
                    {
                        realtimeImg.Source = defaultImg;
                    }
                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                }

                //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
                if (e.anSEnrollNumber > 0)
                {
                    string enrollNumber;
                    if (e.anSEnrollNumber == 99999999)
                    {
                        enrollNumber = "Stranger";
                    }
                    else
                    {
                        enrollNumber = e.anSEnrollNumber.ToString();
                    }
                    RealtimeLog realtimeLog = new RealtimeLog
                    {
                        EnrollNo = enrollNumber,
                        VeriMode = e.anVerifyMode.ToString(),
                        InOut = e.anInOutMode == 1 ? "OUT" : e.anInOutMode == 0 ? "IN" : "--",
                        DateTime = e.anLogDate.ToString("yyyy/MM/dd HH:mm"),
                        Ip = e.astrDeviceIP.ToString(),
                        MId = e.vnDeviceID.ToString(),
                        IsEnrolled = true, // Example condition
                    };
                    try
                    {
                        //if(EnrollNumbers.Contains(e.anSEnrollNumber))
                        //{
                        //    realtimeLog.IsEnrolled = true;
                        //    string query = $"insert into tblLog (TMchNo,EnrollNo,EMchNo,InOut,VeriMode,[DateTime]) values ({e.vnDeviceID}, {e.anSEnrollNumber}, {e.vnDeviceID}, {e.anInOutMode}, {e.anVerifyMode}, '{e.anLogDate.ToString("yyyy/MM/dd HH:mm:ss")}')";

                        //    DBFactory.Insert(connstring, query);

                        //}
                        //else
                        //{
                        //    realtimeLog.IsEnrolled = false;
                        //}
                        if (e.anSEnrollNumber != 99999999)
                        {
                            realtimeLog.IsEnrolled = EnrollNumbers.Contains(e.anSEnrollNumber) ? true : false;
                            string query = $"insert into tblLog (TMchNo,EnrollNo,EMchNo,InOut,VeriMode,[DateTime]) values ({e.vnDeviceID}, {e.anSEnrollNumber}, {e.vnDeviceID}, {e.anInOutMode}, {e.anVerifyMode}, '{e.anLogDate.ToString("yyyy/MM/dd HH:mm:ss")}')";
                            DBFactory.Insert(connstring, query);
                        }
                        else
                        {
                            realtimeLog.IsEnrolled = false;
                        }
                        gdevId device = gId.FirstOrDefault(x => x.MId == e.vnDeviceID);
                        if (device != null)
                        {
                            realtimeLog.Location = device.Location;
                        }
                        
                        downuserlist.Items.Add(realtimeLog);



                        nIndex++;
                    }
                    catch (Exception ex)
                    {
                        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                        string message = "( " + DateTime.Now + " ) ( MonitoringError  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                        File.AppendAllText(path, message);
                    }
                }
                int nResult = 1;

                this.axFPCLOCK_Svr1.SendResultandTime(e.linkindex, e.vnDeviceID, e.anSEnrollNumber, nResult);

                if (nIndex > 1000)
                {
                    this.nIndex = 0;                    
                }
            }



        }
        public BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            bitmapImage.Freeze(); // To make it cross-thread accessible
            return bitmapImage;
        }

        private void txtPort_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPortOverlay.Visibility = Visibility.Collapsed;
        }

        private void txtPort_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtPort.Text.Length > 0)
            {
                txtPortOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPortOverlay.Visibility = Visibility.Visible;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if(downuserlist.Items.Count > 0)
            {
                downuserlist.Items.Clear();
                if(defaultImg != null)
                {
                    realtimeImg.Source = defaultImg;
                }
            }
        }

        private void ucRealtime_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = DBFactory.GetAllByQuery(connstring, "Select distinct EnrollNumber from tbl_enroll");
                EnrollNumbers = ConvertToList(dt);
            }
            catch(Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( ucRealtime_Loaded  ) ( " + ex.Message + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
        }
        private List<int> ConvertToList(DataTable dt)
        {
            List<int> ints = new List<int>();
            foreach (DataRow dr in dt.Rows)
            {
                ints.Add(Convert.ToInt32(dr["EnrollNumber"]));
            }
            return ints;
        }
        public void Loaddev_list()
        {
            Devlist.Items.Clear();
            gId = new ObservableCollection<gdevId>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connstring);
            if (con)
            {
                //dt = DBFactory.GetAllByQuery(connstring, "SELECT  [MId],[Ip], [Location] from tbl_dev ");
                dt = DBFactory.GetAllByQuery(connstring, "Select * from tbl_dev");
                //gId = ConvertDataTableObs<gdevId>(dt);
                ObservableCollection<DevInfo> devList = ConvertDataTableObs<DevInfo>(dt);
                foreach(DevInfo dev in devList)
                {
                    string serial = dev.Serial;
                    string hwpin = dev.HWpin;
                    if (Dev_Validation.match_serial(serial, hwpin))
                    {
                        gdevId obj = new gdevId()
                        {
                            MId = dev.MId,
                            Ip = dev.Ip,
                            Location = dev.Location
                        };
                        gId.Add(obj);
                    }
                }
                Devlist.ItemsSource = gId;
                //fetch remote devices and merge to show in the devices list
                DataTable remoteDevDT = DBFactory.GetAllByQuery(connstring, "Select * from tbl_RemoteDevices");
                gdevId remoteDev;
                foreach (DataRow dev in remoteDevDT.Rows)
                {
                    remoteDev = new gdevId();
                    remoteDev.Ip = dev["Ip"].ToString();
                    remoteDev.Location = dev["Location"].ToString();
                    remoteDev.MId = Convert.ToInt32(dev["MId"]);
                    gId.Add(remoteDev);
                }

            }
            else
            {
                Devlist.ItemsSource = null;
            }
        }
    }
}
