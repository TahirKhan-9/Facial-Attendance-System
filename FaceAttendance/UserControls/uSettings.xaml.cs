using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using HIKVISION;
using UsmanCodeBlocks.Data.Sql.Local;
using System.IO;
using FaceAttendance.Classes;
using System.Data.SqlClient;
using System.Globalization;
using FPClient;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for uSettings.xaml
    /// </summary>
    public partial class uSettings : UserControl
    {

        string inputfile = StringCipher.GetPath();
        string key = "123";
        SqlCommand cmd;
        SqlConnection conn;
        string connstringG = "";
        object result;
        bool status,Status;
        string connstrig;

        public uSettings(string constring,bool status)
        {
            InitializeComponent();
            connstrig = constring;
            Status = status;
            if (Status == false)
            {
                txtUn.IsEnabled = false;
                txtcpass.IsEnabled = false;
                txtpass.IsEnabled = false;
                adduser.IsEnabled = false;
                txtUn1.IsEnabled = false;
                txtpass1.IsEnabled = false;
                cpass.IsEnabled = false;
                Pass.IsEnabled = false;
                Un.IsEnabled = false;
                txtcpass1.IsEnabled = false;
            }
            
        }
        private Tuple<string,string> GetDataSources2()
        {
            string ServerName = Environment.MachineName;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        Console.WriteLine(ServerName + "\\" + instanceName);
                        return new Tuple<string, string>(ServerName, instanceName);
                    }
                }
            }
            return new Tuple<string, string>(ServerName, "");
        }


        private void btnOnline_Click(object sender, RoutedEventArgs e)
        {
            GetDataSources2();
        }

        private void enable_textbox(object sender, MouseButtonEventArgs e)
        {
            enable_controls();
        }

        public void enable_controls()
        {

            txtUn.IsReadOnly = false;
            txtpass.IsReadOnly = false;
            txtcpass.IsReadOnly = false;
            btnsave.IsEnabled = true;

        }
        public void disable_controls()
        {
            txtUn.IsReadOnly = true;
            txtpass.IsReadOnly = true;
            txtcpass.IsReadOnly = true;
            btnsave.IsEnabled = false;
            txtMN.IsReadOnly = true;
            txtSN.IsReadOnly = true;
            btnCC.IsEnabled = false;
            txtUN.IsReadOnly = true;
            txtPwd.IsReadOnly = true;

        }
        public void reset_controls()
        {
            txtUn.Text = "";
            txtpass.Text = "";
            txtcpass.Text = "";
            txtMN.Text = "";
            txtSN.Text = "";
            txtUN.Text = "";
            txtPwd.Text = "";
        }

        

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            try
            {
                if (txtpass.Text == txtcpass.Text)
                {
                    string query = "Insert into tbl_users values ('" + txtUn.Text.ToString() + "','" + txtpass.Text.ToString() + "')";
                    DBFactory.Insert(connstrig, query);
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Password not Matched\n", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }

            }
            catch (Exception ex)
            {

                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }

            Wait.Visibility = Visibility.Collapsed;
            disable_controls();
            reset_controls();
        }

        private void enable_Server_controls(object sender, MouseButtonEventArgs e)
        {
            txtMN.IsReadOnly = false;
            txtSN.IsReadOnly = false;
            txtPwd.IsReadOnly = false;
            txtUN.IsReadOnly = false;
            btnCC.IsEnabled = true;
            var tuple = GetDataSources2();
            txtMN.Text = tuple.Item1;
            txtSN.Text = tuple.Item2;
            //txtUN.Text = tuple.Item3;
            //txtPwd.Text = tuple.item4;
            //txtUN1.Visibility = Visibility.Collapsed;
            txtSN1.Visibility = Visibility.Collapsed;
            txtMN1.Visibility = Visibility.Collapsed;

        }


        private bool conTester(string connstringG)
        {
            using (SqlConnection connection = new SqlConnection(connstringG))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }


        private bool Encrypt(string MachineName, string SQLInstance, string username, string password)
        {
            if (File.Exists(inputfile))
            {
                int dot = inputfile.LastIndexOf(".");
                string keyfile = System.IO.Path.ChangeExtension(inputfile, ".enc");
                string keyfiledup = System.IO.Path.ChangeExtension(inputfile, ".txt");


                File.Move(keyfile, System.IO.Path.ChangeExtension(keyfile, ".txt"));

                StreamReader sr = new StreamReader(keyfiledup);

                // This is use to specify from where  
                // to start reading input stream 
                sr.BaseStream.Seek(0, SeekOrigin.Begin);

                // To read line from input stream 
                string str = sr.ReadLine();
                sr.Close();
                connstringG = StringCipher.Decrypt(str, key);

                File.Move(keyfiledup, System.IO.Path.ChangeExtension(keyfiledup, ".enc"));
                CreateDataBase();

                status = conTester(connstringG);

                if (status)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Connection String Already Exist!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return true;
                }
                else
                {

                    string passPhrase;

                    passPhrase = "Data Source =" + MachineName + "; Initial Catalog = AASFF; User Id=" + username + ";Password=" + password;
                    status = conTester(passPhrase);
                    if (!status)
                    {

                        passPhrase = "Data Source =" + MachineName + "; Initial Catalog = AASFF; Integrated Security = True";
                        status = conTester(passPhrase);
                        if (!status)
                        {
                            passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + "; Initial Catalog = AASFF; Integrated Security = True";
                            status = conTester(passPhrase);
                            if (!status)
                            {
                                passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + ";  Integrated Security = True; User Id = " + username + "; Password = " + password;
                                status = conTester(passPhrase);
                                if (!status)
                                {
                                    passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + ";   User Id = " + username + "; Password = " + password;
                                    status = conTester(passPhrase);
                                }
                            }
                        }


                        //passPhrase = "Data Source =" + MachineName + "; Initial Catalog = AASFF; Integrated Security = True"; Initial Catalog = AASFF;
                        //status = conTester(passPhrase);
                        //if (!status)
                        //{
                        //    passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + "; Initial Catalog = AASFF; Integrated Security = True";
                        //    status = conTester(passPhrase);
                        //}
                    }


                    if (status)
                    {
                        string encstr = StringCipher.Encrypt(passPhrase, key);

                        // Create a new file     
                        using (StreamWriter sw = File.CreateText(inputfile))
                        {
                            //Data Source = DESKTOP - 7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True
                            //actual
                            //Data Source = DESKTOP-7V9OI2S\SQLEXPRESS; Initial Catalog = AASFF; Integrated Security = True

                            //false
                            //Data Source = DESKTOP-7V9OI2S\SQLEXPRESS; Initial Catalog = AASFF; Integrated Security = True
                            sw.Write(encstr);

                        }

                        return false;
                    }



                }



            }
            else
            {
                CreateDataBase();

                string passPhrase;

                passPhrase = "Data Source =" + MachineName + "; Initial Catalog = AASFF; User Id=" + username + ";Password=" + password;
                status = conTester(passPhrase);
                if (!status)
                {
                    passPhrase = "Data Source =" + MachineName + "; Initial Catalog = AASFF; Integrated Security = True";
                    status = conTester(passPhrase);
                    if (!status)
                    {
                        passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + "; Initial Catalog = AASFF; Integrated Security = True";
                        status = conTester(passPhrase);
                        if (!status)
                        {
                            passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + "; Initial Catalog = AASFF; Integrated Security = True; User Id = " + username + "; Password = " + password;
                            status = conTester(passPhrase);
                            if (!status)
                            {
                                passPhrase = "Data Source =" + MachineName + "\\" + SQLInstance + "; Initial Catalog = AASFF;  User Id = " + username + "; Password = " + password;
                                status = conTester(passPhrase);
                            }
                        }
                    }
                }


                if (status)
                {
                    string encstr = StringCipher.Encrypt(passPhrase, key);

                    // Create a new file     
                    using (StreamWriter sw = File.CreateText(inputfile))
                    {
                        //Data Source = DESKTOP - 7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True
                        //actual
                        //Data Source = DESKTOP-7V9OI2S\SQLEXPRESS; Initial Catalog = AASFF; Integrated Security = True

                        //false
                        //Data Source = DESKTOP-7V9OI2S\SQLEXPRESS; Initial Catalog = AASFF; Integrated Security = True
                        sw.Write(encstr);

                    }

                    return false;
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, " Invalid Connstring!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }

            }

            return false;

        }
        private void ExecuteSQLStmt(string sql)
        {
            //if (conn.State == ConnectionState.Open)
            //    conn.Close();
            //conn.ConnectionString = "Data Source = WALEED - PC\\SQLEXPRESS; Integrated Security = False;User Id=sa;Password=123 ";
            //conn.Open();
            cmd = new SqlCommand(sql, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ae)
            {
                MessageBox.Show(ae.Message.ToString());
            }
        }
        private void ExecuteSQLTabStmt(string sql,string tabConn)
        {
            SqlConnection sqlConnection = new SqlConnection(tabConn);
            sqlConnection.Open();
            cmd = new SqlCommand(sql, sqlConnection);
            try
            {

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ae)
            {
                MessageBox.Show(ae.Message.ToString());
            }
        }
        private void ExecuteSQLViewStmt(string sql, string tabConn)
        {
            SqlConnection sqlConnection = new SqlConnection(tabConn);
            sqlConnection.Open();
            cmd = new SqlCommand(sql, sqlConnection);
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ae)
            {
                MessageBox.Show(ae.Message.ToString());
            }
        }
        public void CreateDataBase()
        {
            //"Data Source =" + txtMN.Text + "; Initial Catalog = AASFF; User Id=" + txtUN.Text + ";Password=" + txtPwd.Text   "Data Source=WALEED-PC\\SQLEXPRESS;Integrated Security=True;"
            string dbConStr = "Data Source =" + txtMN.Text + "; User Id=" + txtUN.Text + ";Password=" + txtPwd.Text; 
            conn = new SqlConnection(dbConStr);
            {
                try
                {
                    conn.Open();
                    //string dbName = "AASFF";
                    SqlCommand command = new SqlCommand("SELECT database_id FROM sys.databases WHERE name = 'AASFF';", conn);
                    result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value && (int)result > 0)
                    {
                        
                    }
                    else
                    {
                        string createDataBase = "CREATE DATABASE [AASFF] ON  PRIMARY "
    + "( NAME = N'FMDB', FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL15.SQLEXPRESS\\MSSQL\\DATA\\FMDB.mdf' , SIZE = 21696KB ,"
    + "MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB)log on"
    + "(NAME = N'FMDB_log', FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL15.SQLEXPRESS\\MSSQL\\DATA\\FMDB_log.ldf' , SIZE = 27200KB ,"
    + "MAXSIZE = 2048GB , FILEGROWTH = 10%)";
                        ExecuteSQLStmt(createDataBase);
                        result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value && (int)result > 0)
                        {
                            string tabCreatConn = "Data Source =" + txtMN.Text + "; Initial Catalog = AASFF; User Id=" + txtUN.Text + ";Password=" + txtPwd.Text;
                            string devTab = "CREATE TABLE [dbo].[tbl_dev]([DId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,[MId] [int] NULL,[Ip] [varchar](20) NULL,[Status] [datetime] NULL,[Serial] [varchar](100) NULL,[HWpin] [varchar](500) NULL,[DevDescription] [varchar](max) NULL,[Location] [varchar](max) NULL)";
                            ExecuteSQLTabStmt(devTab, tabCreatConn);
                            string enrTab = "CREATE TABLE [dbo].[tbl_enroll]([EMachineNumber] [int] NULL,[EnrollNumber] [int] NOT NULL,[FingerNumber] [int] NULL,[Privilige] [int] NULL,[enPassword] [int] NULL,[EName] [varchar](80) NULL,[ShiftId] [int] NULL,[FPData] [nvarchar](max) NULL,[DptID] [varchar](50) NULL)";
                            ExecuteSQLTabStmt(enrTab, tabCreatConn);
                            string shiftsTab = "CREATE TABLE [dbo].[tbl_shifts]([ShiftId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,[ShiftStart] [time](7) NULL,[ShiftEnd] [time](7) NULL,[ShiftName] [varchar](50) NULL)";
                            ExecuteSQLTabStmt(shiftsTab, tabCreatConn);
                            string logTab = "CREATE TABLE [dbo].[tblLog]([Id] [int] IDENTITY(1,1) NOT NULL,[TMchNo] [int] NULL,[EnrollNo] [int] NULL,[EMchNo] [int] NULL,[InOut] [int] NULL,	[VeriMode] [int] NULL,[DateTime] [datetime] NULL)";
                            ExecuteSQLTabStmt(logTab, tabCreatConn);
                            string depTab = "CREATE TABLE [dbo].[tbl_department]([dept_id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,[dept_name] [nvarchar](max) NULL)";
                            ExecuteSQLTabStmt(depTab, tabCreatConn);
                            string blockUsTab = "CREATE TABLE [dbo].[tbl_blockedUsers]([EMachineNumber] [int] NULL,[EnrollNumber] [int] NULL,[FingerNumber] [int] NULL,[Privilige] [int] NULL,[enPassword] [int] NULL,[EName] [varchar](80) NULL)";
                            ExecuteSQLTabStmt(blockUsTab, tabCreatConn);
                            string unblockUsTab = "CREATE TABLE [dbo].[tbl_unblockedUsers]([EMachineNumber] [int] NULL,[EnrollNumber] [int] NULL,[FingerNumber] [int] NULL,[Privilige] [int] NULL,[enPassword] [int] NULL,[EName] [varchar](80) NULL)";
                            ExecuteSQLTabStmt(unblockUsTab, tabCreatConn);
                            string usersTab = "CREATE TABLE [dbo].[tbl_users]([PId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,[UId] [nvarchar](50) NOT NULL,[Pass] [nvarchar](50) NULL)";
                            ExecuteSQLTabStmt(usersTab,tabCreatConn);
                            string dailyLogView = "CREATE VIEW [dbo].[v_dailyLog] AS select distinct l.EnrollNo,enr.EName,l.DateTime,dev.Location,dev.DevDescription From [dbo].[tblLog] as l,[dbo].[tbl_dev] as dev,[dbo].[tbl_enroll] as enr where l.EnrollNo = enr.EnrollNumber and l.EMchNo = dev.MId";
                            ExecuteSQLViewStmt(dailyLogView, tabCreatConn);
                            //Console.WriteLine($"The database '{dbName}' does not exist.");
                        }
                       
                        //Console.WriteLine("The database exists with ID {0}.", databaseId);
                        // Further processing of the database
                    }
                    // Check if the database exists

                }
                catch (Exception ex)
                {
                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                    string message = "( " + DateTime.Now + " ) ( Create Database  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                    File.AppendAllText(path, message);
                }
            }
        }
        private void btnCC_Click(object sender, RoutedEventArgs e)
        {
            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();

            //file creation logic
            Encrypt(txtMN.Text, txtSN.Text, txtUN.Text,txtPwd.Text);

            disable_controls();
            reset_controls();

            //goto splash screen
            
            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Please Restart the App", false);
            message.btnCancel.Visibility = Visibility.Collapsed;
            message.btnOk.Content = "Ok";
            message.ShowDialog();

            Wait.Visibility = Visibility.Collapsed;
           
            System.Windows.Application.Current.Shutdown();
            //System.Windows.Forms.Application.Restart();

        }

        private void btnGetDevTime_Click(object sender, RoutedEventArgs e)
        {

            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();

            try
            {
                bool bRet;
                int mId = int.Parse(txtdevId.Text);
                string ip = txtdevIp.Text;

                IPAddress ipc;
                bool ValidateIP = IPAddress.TryParse(ip, out ipc);
                if (ValidateIP != true)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong Ip!\n", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Wait.Visibility = Visibility.Collapsed;
                    return;
                }

                TimeInfo tTimeInfo = new TimeInfo(2); //test value 

                bRet = DisableDevice(ip, mId);

                if (bRet)
                {
                    //labelInfo.Text = "Disable Device Success!";

                    axFP_CLOCK.GetDeviceTime(
                        mId,
                       ref tTimeInfo.nYear,
                       ref tTimeInfo.nMonth,
                       ref tTimeInfo.nDay,
                       ref tTimeInfo.nHour,
                       ref tTimeInfo.nMinute,
                       ref tTimeInfo.nDayofWeek
                        );
                    if (tTimeInfo.nDayofWeek == 0)
                    {
                        tTimeInfo.nDayofWeek = 7;
                    }

                    DateTime dateTime = new DateTime(
                        tTimeInfo.nYear,
                        tTimeInfo.nMonth,
                        tTimeInfo.nDay,
                        tTimeInfo.nHour,
                        tTimeInfo.nMinute,
                        0
                        );
                    CultureInfo enUS = new CultureInfo("en-US");

                    //labelInfo.Text = dateTime.ToString("R", enUS);
                    //txtdevId.Text = dateTime.ToString("f", enUS);
                    StatusWindow.Text = dateTime.ToString("f", enUS);


                }
                else
                {
                    StatusWindow.Text = "N0 Device ...";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();

                }

                EnableDevice(mId);
            }
            catch(Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Wait.Visibility = Visibility.Collapsed;
            }

            Wait.Visibility = Visibility.Collapsed;
            
        }

        private void btnSetDevTime_Click(object sender, RoutedEventArgs e)
        {

            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();

            try
            {
                bool bRet;
                int mId = int.Parse(txtdevId.Text);
                string ip = txtdevIp.Text;
                IPAddress ipc;
                bool ValidateIP = IPAddress.TryParse(ip, out ipc);
                if (ValidateIP != true)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong Ip!\n", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Wait.Visibility = Visibility.Collapsed;
                    return;
                }

                bRet = DisableDevice(ip,mId);
                if (bRet == false)
                {
                    return;
                }

                bRet = axFP_CLOCK.SetDeviceTime(int.Parse(txtdevId.Text));
                if (bRet)
                {
                    StatusWindow.Text = "Success...";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                else
                {
                    ShowErrorInfo();
                }

                EnableDevice(mId);
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Wait.Visibility = Visibility.Collapsed;
            }

            Wait.Visibility = Visibility.Collapsed;

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

        private void EnableDevice(int id)
        {

            axFP_CLOCK.EnableDevice(id, 1);
        }

        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            axFP_CLOCK.GetLastError(ref nErrorValue);

        }

        private void txtUn1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUn1.Visibility = Visibility.Collapsed;
        }

        private void txtpass1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtpass1.Visibility = Visibility.Collapsed;
        }

        private void txtcpass1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtcpass1.Visibility = Visibility.Collapsed;
        }

        private void txtMN1x_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMN1.Visibility = Visibility.Collapsed;
        }

        private void txtSN1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSN1.Visibility = Visibility.Collapsed;
        }

        private void txtPwd1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPwd1.Visibility = Visibility.Collapsed;
        }

        private void txtdevId1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdevId1.Visibility = Visibility.Collapsed;
        }

        private void txtdevIp1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtdevIp1.Visibility = Visibility.Collapsed;
        }

        private void btn_startServ(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] process = Process.GetProcessesByName("fp_server");
                if (process.Length == 0)
                {
                    Process.Start(Globals.ServicePath);
                }
                else if (process.Length > 0)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "CAMS Monitoring Service is already Running", false);
                    message.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) (btn_StartService  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }
          
           
        }

        private void Enable_Firebase_Controls(object sender, MouseButtonEventArgs e)
        {
            txtUsername.IsReadOnly = false;
            txtPassword.IsReadOnly = false;
            btnSaveFirebase.IsEnabled = true;
            txtUsername.Focus();
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtUsername.IsReadOnly == false)
            {
                UsernameOverlay.Visibility = Visibility.Collapsed;
            }
            
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtUsername.Text.Length == 0 && txtUsername.IsReadOnly == false)
            {
                UsernameOverlay.Visibility = Visibility.Visible;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txtPassword.IsReadOnly == false)
            {
                PasswordOverlay.Visibility = Visibility.Collapsed;
            }            
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Text.Length == 0 && txtPassword.IsReadOnly == false)
            {
                PasswordOverlay.Visibility = Visibility.Visible;
            }
        }
        private void ResetFirebaseControls()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.IsReadOnly = true;
            txtPassword.IsReadOnly = true;
            PasswordOverlay.Visibility = Visibility.Visible;
            UsernameOverlay.Visibility = Visibility.Visible;
            btnSaveFirebase.IsEnabled = false;
            Wait.Visibility = Visibility.Collapsed;
        }
        private async void btnSaveFirebase_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please Enter Username.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please Enter Password.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            FirebaseService firebaseService = new FirebaseService();
            bool Authenticated = await firebaseService.AuthenticateUser(txtUsername.Text, txtPassword.Text);
            if (!Authenticated)
            {
                Wait.Visibility = Visibility.Collapsed ;
                ResetFirebaseControls();
                return;
            }
            try
            {

                System.Data.DataTable dt = DBFactory.GetAllByQuery(connstrig, "select count(*) as RowsCount from FirebaseCredentials");
                int count = Convert.ToInt32(dt.Rows[0]["RowsCount"]);
                int affectedRows = 0;
                bool Updated = false;
                if (count == 0)
                {
                    string InsertQuery = $"Insert Into FirebaseCredentials (Username, Password) Values('{txtUsername.Text}', '{txtPassword.Text}')";
                    affectedRows = DBFactory.Insert(connstrig, InsertQuery);
                }
                if (count == 1)
                {
                    string UpdateQuery = $"Update FirebaseCredentials set Username = '{txtUsername.Text}', Password = '{txtPassword.Text}' where Id = 1";
                    Updated = DBFactory.Update(connstrig, UpdateQuery);
                }
                if(affectedRows > 0 || Updated)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Success, "Firebase Credentials Saved Successfully.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Globals.FirebaseUsername = txtUsername.Text;
                    Globals.FirebasePassword = txtPassword.Text;
                    ResetFirebaseControls();
                    return;
                }
                else
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Failed, Please Try Again.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    Wait.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            catch(Exception ex)
            {                
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, $"Exception Occured\n{ex.Message}", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                ResetFirebaseControls();
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string msg = "( " + DateTime.Now + " ) (btnSaveFirebase  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, msg);
            }


        }

        private void txtUin1_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUN1.Visibility = Visibility.Collapsed;
        }
    }
}
