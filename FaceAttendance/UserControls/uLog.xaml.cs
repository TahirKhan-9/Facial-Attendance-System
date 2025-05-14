using FaceAttendance.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using UsmanCodeBlocks.Data.Sql.Local;
using HIKVISION;
//using MaterialDesignThemes.wpf;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections;
using System.Threading;
using FPClient;
using System.IO;
using Microsoft.Win32;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for uLog.xaml
    /// </summary>
    public partial class uLog : UserControl
    {
        int control = 0;
        public string sql;
        public string connstring;
        public string mDate;
        //public AxFP_CLOCKLib.AxFP_CLOCK pOcxObject;
        //int m_nMachineNum = 1;
        //int id = 2;
        //string ip = "192.168.0.111";
        //bool Flag;
        //private AxFP_CLOCKLib.AxFP_CLOCK pOcxObject;
        List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();
        

        //ObservableCollection<GLog> lstloginfo;
        ObservableCollection<GLog>  lstloginfo = new ObservableCollection<GLog>();



        ObservableCollection<gdevId> gId;
        //ObservableCollection<gdevId> Ll;

        public uLog(string constring)
        {
            connstring = constring;



            //Loglist.ItemsSource= Loglist.Items.Add( new GLog
            //{
            //    TMchNo = "1",
            //    EnrollNo = "1",
            //    EMchNo = "1",
            //    InOut = "1",
            //    VeriMode = "1",
            //    DateTime = "1"
            //});


            //Loglist.Items.Add(new GLog
            //{
            //    TMchNo = "1",
            //    EnrollNo = "1",
            //    EMchNo = "1",
            //    InOut = "1",
            //    VeriMode = "1",
            //    DateTime = "1"
            //});
            ////gId = new List<gdevId>();
            ///


            
            if (Loglist != null)
            {
                //setSearch();
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Loglist.ItemsSource);
                view.Filter = UserFilter;
            }

            InitializeComponent();
            //this.pOcxObject = ref axFP_CLOCK;
        }

        public void setSearch()
        {

            if (Loglist != null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Loglist.ItemsSource);
                view.Filter = UserFilter;
            }
            //gId = new List<gdevId>();
            
        }
        

        public bool condev(string ip, int id)
        {
            bool bRet;
            bRet = axFP_CLOCK.SetIPAddress(ref ip, 5005, 0);
            bRet = axFP_CLOCK.OpenCommPort(id);
            if (bRet)
            {
                //statusbar_green();
                return true;
            }
            else
            {
                //statusbar_red();
                return false;
            }
        }



        private bool EnableDevice(int id)
        {

            return axFP_CLOCK.EnableDevice(id, 1);
        }
        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            axFP_CLOCK.GetLastError(ref nErrorValue);

        }


        public void dev_list()
        {
            //Devlist.ItemsSource = null;
            gId = new ObservableCollection<gdevId>();//List<gdevId>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connstring);
            if (con)
            {
                //dt = DBFactory.GetAllByQuery(connstring, "SELECT  [MId],[Ip] from tbl_dev ");
                dt = DBFactory.GetAllByQuery(connstring, "SELECT * from tbl_dev");
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
            dev_list();
            InitGLogListView();
        }

        private void btnReadLog_Click(object sender, RoutedEventArgs e)
        {
            //StatusWindow.Text = "Wait...\tReading Log Data!";
            Thread newThread = new Thread(ClearStatusWindow);
            try
            {

                if (Devlist.SelectedItems.Count.Equals(0))
                {
                    MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Info, "Device not selected.", true);
                    _ = message1.ShowDialog();
                    return;

                }
                //Thread StatusReadin = new Thread(StatusReading);
                //StatusReadin.Start();
                /*lstloginfo = new ObservableCollection<GLog>()*/;
                LD_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Reading Log Data!", true);
                messaget.ShowDialog();
                myArray.Clear();
                IEnumerable items = this.Devlist.SelectedItems;
                List<gdevId> inActiveMachines = new List<gdevId>();

                //Loglist.Items.Clear();
                foreach (gdevId gid in items)
                {
                    try
                    {

                        if (condev(gid.Ip.ToString(), gid.MId))
                        {
                            ReadGLogData(gid.Ip.ToString(), gid.MId);
                            _ = EmptyGLogData(gid.Ip.ToString(), gid.MId);
                            //LogCount.Content = myArray.Count;
                        }
                        else
                        {
                            MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                            msg.btnCancel.Visibility = Visibility.Collapsed;
                            msg.btnOk.Content = "Ok";
                            msg.ShowDialog();
                            //Devlist.SelectedItems.Remove(gid);
                            inActiveMachines.Add(gid);
                            if(Devlist.SelectedItems.Count == 1)
                            {
                                LD_Wait.Visibility = Visibility.Collapsed;
                                return;
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxWindow ms = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                        ms.btnCancel.Visibility = Visibility.Collapsed;
                        ms.btnOk.Content = "Ok";
                        ms.ShowDialog();
                        newThread.Start();
                    }
                }
                if (inActiveMachines.Count > 0)
                {
                    foreach (gdevId Machine in inActiveMachines)
                    {
                        Devlist.SelectedItems.Remove(Machine);
                    }
                }
                LogCount.Content = Loglist.Items.Count;
                //setSearch();
                LD_Wait.Visibility = Visibility.Collapsed;
                StatusWindow.Foreground = Brushes.Green;
                StatusWindow.Text = "Process Completed!";
                string text = "";
                if (Devlist.SelectedItems.Count > 1)
                {

                    text = int.Parse(LogCount.Content.ToString()) == 0 ? "Selected Machines doesn't contain log data" : StatusWindow.Text;
                }
                else
                {
                    text = int.Parse(LogCount.Content.ToString()) == 0 ? "Selected Machine doesn't contain log data" : StatusWindow.Text;
                }
                //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                newThread.Start();
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( btnReadLog_Click  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
                newThread.Start();
            }
           

        }
        private void StatusReading()
        {
            Action StatusReading = () =>
            {
                StatusWindow.Text = "Wait...\tReading Log Data!";
            };
            Dispatcher.Invoke(StatusReading);
        }
        private bool onlyDisable(int id)
        {
            return axFP_CLOCK.EnableDevice(id, 0);
        }
        private bool DisableDevice(string ip, int id)
        {
            //condev("192.168.0.111", 2);
            condev(ip, id);

            bool bRet = axFP_CLOCK.EnableDevice(id, 0);
            if (bRet)
            {
                //    labelInfo.Text = "Disable Device Success!";
                return true;
            }
            else
            {
                //  labelInfo.Text = "No Device...";
                return false;
            }
        }
        
        private void InitGLogListView()
        {
            /*Loglist.Items.Clear();
            Loglist.Columns.Add("", 40, HorizontalAlignment.Left);
            Loglist.Columns.Add("TMchNo", 90, HorizontalAlignment.Left);
            Loglist.Columns.Add("EnrollNo", 100, HorizontalAlignment.Left);
            Loglist.Columns.Add("EMchNo", 90, HorizontalAlignment.Left);     //
            Loglist.Columns.Add("InOut", 60, HorizontalAlignment.Left);
            Loglist.Columns.Add("VeriMode", 130, HorizontalAlignment.Left);
            Loglist.Columns.Add("DateTime", 130, HorizontalAlignment.Left);
            */
            GridView myGridView = new GridView();
            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "Employee Log Information";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("TMchNo");
            gvc1.Header = "Device ID";
            gvc1.Width = 200;
            //gvc1.Width = c1.ActualWidth;
            myGridView.Columns.Add(gvc1);

            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("EnrollNo");
            gvc2.Header = "User ID";
            gvc2.Width = 200;
            //gvc2.Width = c2.ActualWidth;
            myGridView.Columns.Add(gvc2);

            //GridViewColumn gvc3 = new GridViewColumn();
            //gvc3.DisplayMemberBinding = new Binding("EMchNo");
            //gvc3.Header = "EMchNo";
            //gvc3.Width = c3.ActualWidth;
            //myGridView.Columns.Add(gvc3);

            //GridViewColumn gvc4 = new GridViewColumn();
            //gvc4.DisplayMemberBinding = new Binding("InOut");
            //gvc4.Header = "InOut";
            //gvc4.Width = c4.ActualWidth;
            //myGridView.Columns.Add(gvc4);

            //GridViewColumn gvc5 = new GridViewColumn();
            //gvc5.DisplayMemberBinding = new Binding("VeriMode");
            //gvc5.Header = "VeriMode";
            //gvc5.Width = c5.ActualWidth;
            //myGridView.Columns.Add(gvc5);

            GridViewColumn gvc6 = new GridViewColumn();
            gvc6.DisplayMemberBinding = new Binding("DateTime");
            gvc6.Header = "DateTime";
            gvc6.Width = 200;
            //gvc6.Width = c6.ActualWidth;
            myGridView.Columns.Add(gvc6);

            Loglist.View = myGridView;

        }

        private  void ReadGLogData(string ip, int id)
        {
            Loglist.ItemsSource = null;
            //InitGLogListView();
            //lstloginfo = new ObservableCollection<GLog>();

            Loglist.Items.Clear();
            bool bRet;
            GeneralLogInfo gLogInfo = new GeneralLogInfo();

            //List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();

            // if true, only read new log
            axFP_CLOCK.ReadMark = true;//checkBox1.Checked;
            //bool status = EnableDevice(id);
            bRet = DisableDevice(ip, id);
            //condev(ip, id);
            //bRet = onlyDisable(id);
            //bool connected = condev(ip, id);
            //bRet = axFP_CLOCK.ReadGeneralLogData(id);
            bRet = axFP_CLOCK.ReadAllGLogData(id);
                if (!bRet)
                {
                    ShowErrorInfo();

                    EnableDevice(id);
                    return;
                }

                do
                {
                    try
                    {

                    //Task<bool> task = new Task<bool>(() => log_data(gLogInfo,  id));
                    ////Task<int> task = new Task<int>(() => log_data1());
                    //task.Start();
                    //Flag = await task;

                    //bRet = log_data(gLogInfo,id);
                    bRet = axFP_CLOCK.GetGeneralLogDataWithSecond(id,
                    ref gLogInfo.dwTMachineNumber,
                    ref gLogInfo.dwEnrollNumber,
                    ref gLogInfo.dwEMachineNumber,
                    ref gLogInfo.dwVerifyMode,
                    ref gLogInfo.dwInout,
                    ref gLogInfo.dwEvent,
                    ref gLogInfo.dwYear,
                    ref gLogInfo.dwMonth,
                    ref gLogInfo.dwDay,
                    ref gLogInfo.dwHour,
                    ref gLogInfo.dwMinute,
                    ref gLogInfo.dwSecond
                );
                    //_ = test1(gLogInfo, bRet, id);
                }
                catch (Exception ex)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Error!\n"+ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    }
                    
                if (bRet)
                    {
                        myArray.Add(gLogInfo);
                    }
                    
                    //else
                    //{
                    //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Error!\n", false);
                    //    message.ShowDialog();
                    //}
                
            } while (bRet);

                string sql;
                int i = 1;
            if (myArray.Count !=0)
            {
                foreach (GeneralLogInfo gInfo in myArray)
                {
                    DateTime dt = new DateTime(gInfo.dwYear,
                        gInfo.dwMonth,
                        gInfo.dwDay,
                        gInfo.dwHour,
                        gInfo.dwMinute,
                        gInfo.dwSecond);

                    lstloginfo.Add(new GLog
                    {
                        TMchNo = gInfo.dwTMachineNumber.ToString(),
                        EnrollNo = gInfo.dwEnrollNumber.ToString("D8"),
                        //EMchNo = gInfo.dwEMachineNumber.ToString(),
                        //InOut = gInfo.dwEvent.ToString(),
                       // VeriMode = gInfo.dwVerifyMode.ToString(),
                        DateTime = dt.ToString("yyyy/MM/dd HH:mm:ss")
                    }); //= ConvertDataTableObs<DevInfo>(dt);

                    //Loglist.Items.Add(new GLog
                    //{
                    //    TMchNo = gInfo.dwTMachineNumber.ToString(),
                    //    EnrollNo = gInfo.dwEnrollNumber.ToString("D8"),
                    //    EMchNo = gInfo.dwEMachineNumber.ToString(),
                    //    InOut = gInfo.dwEvent.ToString(),
                    //    VeriMode = gInfo.dwVerifyMode.ToString(),
                    //    DateTime = dt.ToString("yyyy/MM/dd HH:mm:ss")
                    //});

                    //Query for Inserting LogData
                    //insert into tblLog (TMchNo,EnrollNo,EMchNo,InOut,VeriMode,[DateTime]) values (2,00000011,2,0,8,'2021/10/18 15:49:54')
                    sql = "insert into tblLog (TMchNo,EnrollNo,EMchNo,InOut,VeriMode,[DateTime]) values (" + gInfo.dwTMachineNumber.ToString() + ",'" + gInfo.dwEnrollNumber.ToString("D8") + "','" + gInfo.dwEMachineNumber.ToString() + "','" + gInfo.dwEvent.ToString() + "','" + gInfo.dwVerifyMode.ToString() + "','" + dt.ToString("yyyy/MM/dd HH:mm:ss") + "')";
                    DBFactory.Insert(connstring, sql);

                }
               // Loglist.ItemsSource = myArray;
                Loglist.ItemsSource = lstloginfo;
                
            }


            i -= 1;

            //if (Loglist.Items.Count != 0)
            //{
            //    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Loglist.ItemsSource);
            //    view.Filter = UserFilter;
            //} 

            bool status = EnableDevice(id);
            //axFP_CLOCK.CloseCommPort();
            //LogCount.Content = myArray.Count;
            myArray.Clear();
            
        }

        private void EmptyLog_Click(object sender, RoutedEventArgs e)
        {
            lstloginfo.Clear();
            Loglist.ItemsSource = null;
            LogCount.Content = "0";
            //Loglist.Items.Clear();
        }

        private void ExportLog_Click(object sender, RoutedEventArgs e)
        {
            Thread ClearStatus = new Thread(ClearStatusWindow);
            try
            {
                StatusWindow.Text = "Wait...\tExporting Log Data!";
                LD_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                messaget.ShowDialog();

                DataTable table = new DataTable();

                table.Columns.Add("TMchNo", typeof(string));
                table.Columns.Add("EnrollNo", typeof(string));
                //table.Columns.Add("EMchNo", typeof(string));
                table.Columns.Add("DateTime", typeof(string));

                foreach (GLog item in Loglist.Items)
                {
                    table.Rows.Add(item.TMchNo, item.EnrollNo, item.DateTime);
                    //table.Rows.Add(item.TMchNo, item.EnrollNo, item.EMchNo, item.DateTime);
                }
                table.Rows.Add("------------", "------------", "------------------------");
                //table.Rows.Add("end", "end", "end", "end", "end", "end");

                try
                {
                    bool res = ExcelFunc.Write_Excel(table);
                    if (res)
                    {
                        MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Confirm, "Exported Successfully!\n", false);
                        msg.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxWindow ms = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    ms.btnCancel.Visibility = Visibility.Collapsed;
                    ms.btnOk.Content = "Ok";
                    ms.ShowDialog();
                    ClearStatus.Start();
                }

                LD_Wait.Visibility = Visibility.Collapsed;
                StatusWindow.Text = "Excel File Creation Succesfull!!";
                
                ClearStatus.Start();
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( ExportLog_Click  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
                ClearStatus.Start();
            }
            
        }



        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txt_search.Text))
                return true;
            else
                return ((item as GLog).EnrollNo.ToString().IndexOf(txt_search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //if (datePicker.SelectedDate.HasValue)
            //{
            //    mDate = datePicker.SelectedDate.Value.ToString("dd/MM/yyyy");
            //}

            if (!(Loglist == null) && Loglist.Items.Count != 0 && control == 0)
            {
                control++;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Loglist.ItemsSource);
                
                view.Filter = UserFilter;
            }
            //string st = txt_search.Text;
            //string chr = st[st.Length-1].ToString;
            if (!(Loglist == null) && (int.TryParse(txt_search.Text, out int n) || (txt_search.Text == "" && control != 0)))
            {
                CollectionViewSource.GetDefaultView(Loglist.ItemsSource).Refresh();
                LogCount.Content = Loglist.Items.Count;
                if(Loglist.Items.Count == 0)
                {
                    string infoText = $"No user found with {txt_search.Text} ID";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, infoText, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    _ = message.ShowDialog();

                }

            }
            if (txt_search.Text == "")
            {
                txt_search1.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search1.Visibility = Visibility.Collapsed;
            }


        }

        private void txt_search_on_chnge(object sender, TextChangedEventArgs e)
        {
            
            if (txt_search.Text == "")
            {
                txt_search1.Visibility = Visibility.Visible;
            if (!(Loglist == null) && Loglist.Items.Count != 0 && control == 0)
            {
                control++;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Loglist.ItemsSource);
                view.Filter = UserFilter;
            }
            //string st = txt_search.Text;
            //string chr = st[st.Length-1].ToString;
            if (!(Loglist == null) && (int.TryParse(txt_search.Text, out int n) || (txt_search.Text == "" && control != 0)))
            {
                CollectionViewSource.GetDefaultView(Loglist.ItemsSource).Refresh();
                    LogCount.Content = Loglist.Items.Count;
            }
            }
            else
            {
                txt_search1.Visibility = Visibility.Collapsed;
            }

        }

        private void txt_search_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search1.Visibility = Visibility.Collapsed;
        }

        private void btnselectAllMch(object sender, RoutedEventArgs e)
        {
            int count = Devlist.SelectedItems.Count;
            if (count == 0)
            {
                Devlist.SelectAll();
                allMch.Content = "UNCHECK ALL";
                // check = true;
            }
            if (count > 0)
            {
                Devlist.UnselectAll();
                allMch.Content = "CHECK ALL";
                //check = true;
            }
           
        }
        private bool EmptyGLogData(string IP, int MachineID)
        {
            bool bRet;

            DisableDevice(IP, MachineID);

            bRet = axFP_CLOCK.EmptyGeneralLogData(MachineID);


            axFP_CLOCK.EnableDevice(MachineID, 1);
            return bRet;

        }
        private void ClearStatusWindow()
        {
            Thread.Sleep(2000);
            Action UpdateUI = () =>
            {
                StatusWindow.Text = "";
            };
            Dispatcher.Invoke(UpdateUI);
        }

        private void ReadLogFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog SelectFileDialog = new OpenFileDialog();
            SelectFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if(SelectFileDialog.ShowDialog() == true)
            {
                string FilePath = SelectFileDialog.FileName;
                string FileContent = "";
                try
                {
                    FileContent = File.ReadAllText(FilePath);
                    if (FileContent.Split('\r')[0] == "")
                    {
                        MessageBoxWindow InfoMsg = new MessageBoxWindow(MessageBoxType.Info, "Text file is empty or in a wrong format.", false);
                        
                        _ = InfoMsg.ShowDialog();
                        return;
                    }
                    else
                    {
                        string[] ReadColumnNames = FileContent.Split('\r')[0].Split('\t');
                        string[] CorrectColumnNames = new string[] {"No", "TMNo", "EnNo", "Name", "INOUT", "Mode", "DateTime" };
                        if(ReadColumnNames.Length == CorrectColumnNames.Length)
                        {
                            for (int i = 0; i < CorrectColumnNames.Length; i++)
                            {
                                if(ReadColumnNames[i] == CorrectColumnNames[i])
                                {
                                    continue;
                                }
                                else
                                {
                                    MessageBoxWindow err_msg = new MessageBoxWindow(MessageBoxType.Error, "Column Names are Altered", false);
                                    _ = err_msg.ShowDialog();
                                    return;
                                }
                            }
                            string[] Rows = FileContent.Split('\n');
                            if(Rows.Length > 1)
                            {
                                int count = 0;
                                for (int i = 1; i < Rows.Length; i++)
                                {
                                    if(Rows[i] != "")
                                    {
                                        string[] RowElements = Rows[i].Split('\t');
                                        if(RowElements.Length == 7)
                                        {
                                            int TMNo = int.Parse(RowElements[1]);
                                            int EnNo = int.Parse(RowElements[2]);
                                            //string Name = RowElements[3].ToString();
                                            int InOut = int.Parse(RowElements[4]);
                                            int VeriMode = int.Parse(RowElements[5]);
                                            string dateTime = DateTime.Parse(RowElements[6]).ToString("yyyy/MM/dd HH:mm:ss");
                                            string query = "insert into tblLog (TMchNo,EnrollNo,EMchNo,InOut,VeriMode,[DateTime]) values (" + TMNo.ToString() + ",'" + EnNo.ToString("D8") + "','" + TMNo.ToString() + "','" + InOut.ToString() + "','" + VeriMode.ToString() + "','" + dateTime + "')";
                                            DBFactory.Insert(connstring, query);
                                            count++;
                                            if (Loglist.ItemsSource == null)
                                            {
                                                Loglist.ItemsSource = lstloginfo;
                                            }
                                            lstloginfo.Add(new GLog
                                            {
                                                TMchNo = TMNo.ToString(),
                                                EnrollNo = EnNo.ToString("D8"),
                                                //EMchNo = gInfo.dwEMachineNumber.ToString(),
                                                //InOut = gInfo.dwEvent.ToString(),
                                                // VeriMode = gInfo.dwVerifyMode.ToString(),
                                                DateTime = dateTime
                                            });

                                        }

                                    }
                                }
                                int previousCount = int.Parse(LogCount.Content.ToString());
                                LogCount.Content = previousCount + count;
                                MessageBoxWindow err_msg = new MessageBoxWindow(MessageBoxType.Info, "Logs Imported Successfully.", false);
                                _ = err_msg.ShowDialog();
                                return;
                            }
                            else
                            {
                                MessageBoxWindow err_msg = new MessageBoxWindow(MessageBoxType.Error, "There are no records available in the text file.", false);
                                _ = err_msg.ShowDialog();
                                return;
                            }

                        }
                        else
                        {
                            MessageBoxWindow err_ms = new MessageBoxWindow(MessageBoxType.Error, "Text file is not in the right format.", false);
                            _ = err_ms.ShowDialog();
                            return;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBoxWindow err_msg = new MessageBoxWindow(MessageBoxType.Error, "Error Occurred While Reading File.", false);
                    _ = err_msg.ShowDialog();
                    return;
                }

            }
        }


        //private bool log_data(GeneralLogInfo gLogInfo, int id)
        //    {
        //        bool flag;
        //            flag = axFP_CLOCK.GetGeneralLogDataWithSecond(id,
        //               ref gLogInfo.dwTMachineNumber,
        //               ref gLogInfo.dwEnrollNumber,
        //               ref gLogInfo.dwEMachineNumber,
        //               ref gLogInfo.dwVerifyMode,
        //               ref gLogInfo.dwInout,
        //               ref gLogInfo.dwEvent,
        //               ref gLogInfo.dwYear,
        //               ref gLogInfo.dwMonth,
        //               ref gLogInfo.dwDay,
        //               ref gLogInfo.dwHour,
        //               ref gLogInfo.dwMinute,
        //               ref gLogInfo.dwSecond
        //           );
        //        return flag;
        //    }
        //    private int log_data1()
        //    {
        //        int c = 1;

        //        return c;
        //    }
    }
}
