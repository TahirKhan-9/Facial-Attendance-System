using System;
using System.Windows;
using System.Windows.Media;

using System.ComponentModel;
using System.Data;
using UsmanCodeBlocks.Data.Sql.Local;
using System.Data.SqlClient;
using System.IO;
using FaceAttendance.Classes;
//using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.SqlServer.Management.Smo;
using System.Data.Sql;

namespace FaceAttendance
{
    /// <summary>
    /// Interaction logic for master_splash.xaml
    /// </summary>
    public partial class master_splash : Window
    {


        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker initServiceWorker = new BackgroundWorker();

        string inputfile = StringCipher.GetPath();
        string key = "123";
        string connstringG = "";
        string connstring = "";
        bool status;
        bool flag=false;
        private delegate void OnShowMessageHandler(String value);

        public bool userObtained = false;
        public bool isServiceRunning = false;
        public string sql;
        public int Opt;



        public master_splash()
        {
            InitializeComponent();
            this.Topmost = true;
        }
        public master_splash(string ip , int id, int Case)
        {
            InitializeComponent();
            this.Topmost = true;
            condev( ip,  id);
            flag = true;
            Opt = Case;
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

        private void EnableDevice(int id)
        {
            //axFP_CLOCK.EnableDevice(m_nMachineNum, 1);
            axFP_CLOCK.EnableDevice(id, 1);
        }
        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            axFP_CLOCK.GetLastError(ref nErrorValue);
            // labelInfo.Text = common.FormErrorStr(nErrorValue);
        }


        public void get_enroll_data(string ip, int id)
        {
            //listBox1.Items.Clear();

            DisableDevice(ip, id);



            bool bBreakFail = false;
            bool bRet;
            //bRet = axFP_CLOCK.ReadAllUserID(m_nMachineNum);
            bRet = axFP_CLOCK.ReadAllUserID(id);
            if (!bRet)
            {
                ShowErrorInfo();
                EnableDevice(id);

                return;
            }

            int dwEnrollNumber = 0;
            int dwEnMachineID = 0;
            int dwBackupNum = 0;
            int dwPrivilegeNum = 0;
            int dwEnable = 0;
            int dwPassWord = 0;
            int vPhotoSize = 0;




            do
            {

                int[] dwData = new int[1420 / 4];
                int[] FacedwData = new int[1888 / 4];
                int[] indexDataFacePhoto = new int[400800];
                object obj = new System.Runtime.InteropServices.VariantWrapper(FacedwData);

               
                bRet = axFP_CLOCK.GetAllUserID(
                    id,
                    ref dwEnrollNumber,
                    ref dwEnMachineID,
                    ref dwBackupNum,
                    ref dwPrivilegeNum,
                    ref dwEnable
                    );

                //read finished
                if (bRet == false)
                {
                    //EnableDevice();
                    bBreakFail = true;
                    //labelInfo.Text = "fail on GetAllUserID";
                    break;
                }

                if (dwBackupNum == 50)
                {
                    IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                    //bRet = axFP_CLOCK.GetEnrollPhotoCS(m_nMachineNum, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
                    bRet = axFP_CLOCK.GetEnrollPhotoCS(id, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
                    if (bRet)
                    {
                        byte[] mbytCurEnrollData = new byte[vPhotoSize];
                        Marshal.Copy(ptrIndexFacePhoto, mbytCurEnrollData, 0, vPhotoSize);
                        // System.IO.File.WriteAllBytes(@"c:\test.jpg", mbytCurEnrollData);
                        System.IO.File.WriteAllBytes(@"C:\\PHOTO\" + "D" + dwEnMachineID + "U" + dwEnrollNumber.ToString() + ".jpg", mbytCurEnrollData);
                        // labelInfo.Text = "GetEnrollData OK";
                    }
                }
                else
                {

                    bRet = axFP_CLOCK.GetEnrollData(
                        id,
                        dwEnrollNumber,
                        dwEnMachineID,
                        dwBackupNum,
                        ref dwPrivilegeNum,
                        ref obj,
                        ref dwPassWord);
                }
                if (!bRet)
                {
                    ShowErrorInfo();


                }

                if (dwBackupNum == 50)
                {
                    vPhotoSize = 0;
                }
                else
                {

                }


                if (dwBackupNum == 50)
                {
                    sql = "INSERT INTO tblEnroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword)VALUES(" + id + ",'" + dwEnrollNumber + "','" + dwBackupNum + "','" + dwPrivilegeNum + "','" + dwPassWord + "')";
                }

                else
                {

                }
                DBFactory.Insert(connstring, sql);


                //reset
                dwPassWord = 0;

            } while (bRet);


            if (bBreakFail)
            {
                //labelInfo.Text = "Saved all Enroll Data to database...";
            }


            EnableDevice(id);
        }



        private bool Decrypt()
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

                return true;

            }
            else
            {

            }

            return false;
        }




        



        void initServiceWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            //e.Result = initializeFAS();
            ShowMessage("Decrypting databse file...");
            System.Threading.Thread.Sleep(300);
            if (Decrypt() == true)
            {
                //SqlConnection con = DBFactory.Connect(connstringG);
                //"Data Source = DESKTOP-7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True"
                ShowMessage("Getting Database Connection...");
                //status = DBFactory.ConnectServer(connstringG);
                using (SqlConnection connection = new SqlConnection(connstringG))
                {
                    try
                    {
                        connection.Open();
                        status = true;
                    }
                    catch (SqlException)
                    {
                        status = false;
                    }
                }
            }


            if (status /*== true*/)
            {

                System.Threading.Thread.Sleep(500);
                ShowMessageS("Database Connection Successful");

                System.Threading.Thread.Sleep(500);
                ShowMessage("Task Completed");



            }
            else
            {

                ShowMessageF("Database Connection failed");

                //lblProcessing.Foreground = new SolidColorBrush(Colors.Red);
                System.Threading.Thread.Sleep(2000);
                ShowMessageF("Go to settings");
                System.Threading.Thread.Sleep(2000);
                ShowMessageF("To create new conn");
                System.Threading.Thread.Sleep(2000);


            }
        }

        string initServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            MainWindow MN = new MainWindow(connstringG, status);
            MN.Show();
            this.Hide();
            MN.Show();
            return "bakanee";

        }


        private void ShowMessage(string v)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new OnShowMessageHandler(this.ShowMessage), new Object[] { v });
                return;
            }

            lblProcessing.Content = v;
        }

        private void ShowMessageS(string v)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new OnShowMessageHandler(this.ShowMessage), new Object[] { v });
                return;
            }

            lblProcessing.Content = v;
            lblProcessing.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void ShowMessageF(string v)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new OnShowMessageHandler(this.ShowMessage), new Object[] { v });
                return;
            }

            lblProcessing.Content = v;
            lblProcessing.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void splashL(object sender, RoutedEventArgs e)
        {
            initServiceWorker.WorkerReportsProgress = true;
            initServiceWorker.DoWork += initServiceWorker_DoWork;
            //initServiceWorker.RunWorkerCompleted += initServiceWorker_RunWorkerCompleted;
            initServiceWorker.RunWorkerAsync(1000);
        }


        private string GetDataSources1()
        {
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            DataTable table = instance.GetDataSources();
            string ServerName = Environment.MachineName;
            foreach (DataRow row in table.Rows)
            {
                //Console.WriteLine(ServerName + "\\" + row["InstanceName"].ToString());
                //lbldevStatus.Content += ServerName + "\\" + row["InstanceName"].ToString();
                return ServerName + "\\" + row["InstanceName"].ToString();
            }
            return "";
        }


        private void GetDataSources()
        {
            DataTable table = SmoApplication.EnumAvailableSqlServers(true);
            string ServerName = Environment.MachineName;
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine(ServerName + "\\" + row["InstanceName"].ToString());
            }
        }

        private Tuple<string, string> GetDataSources2()
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
            return new Tuple<string, string>("", "");
        }
    }
}
