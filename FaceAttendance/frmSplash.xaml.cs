using FaceAttendance.Classes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Data.SqlClient;
//using System.Data;
using System.IO;
using System.Data;
//using FaceAttendance.Classes;
//using System.Drawing;

namespace FaceAttendance
{
    /// <summary>
    /// Interaction logic for frmSplash.xaml
    /// </summary>
    public partial class frmSplash : Window
    {
        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker initServiceWorker = new BackgroundWorker();
        SqlConnection conn;
        SqlCommand cmd;
        string inputfile = StringCipher.GetPath();
        string key = "123";
        string connstringG = "";
        bool status;

        private delegate void OnShowMessageHandler(String value);

        public bool userObtained = false;
        public bool isServiceRunning = false;
        public frmSplash()
        {
            InitializeComponent();

            this.Topmost = true;
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




        //void initServiceWorker_DoWork(object sender, DoWorkEventArgs e)
        //{

        //    //e.Result = initializeFAS();
        //    ShowMessage("Decrypting databse file...");
        //    System.Threading.Thread.Sleep(100);
        //    if (Decrypt() == true)
        //    {
        //        //SqlConnection con = DBFactory.Connect(connstringG);
        //        //"Data Source = DESKTOP-7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True"
        //        ShowMessage("Getting Database Connection...");
        //        status = DBFactory.ConnectServer(connstringG);

        //    }
        //}

        //void initServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{

        //    if (status /*== true*/)
        //    {
        //        ShowMessageS("Database Connection Successful");

        //        System.Threading.Thread.Sleep(2000);
        //        ShowMessage("Task Completed");
        //        System.Threading.Thread.Sleep(500);
        //        MainWindow MN = new MainWindow(connstringG, status);
        //        this.Hide();
        //        MN.Show();


        //    }
        //    else
        //    {
        //        ShowMessageF("Database Connection failed");

        //        //lblProcessing.Foreground = new SolidColorBrush(Colors.Red);
        //        System.Threading.Thread.Sleep(5000);
        //        ShowMessageF("Go to settings");
        //        System.Threading.Thread.Sleep(2000);
        //        ShowMessageF("To create new conn");
        //        System.Threading.Thread.Sleep(2000);
        //        MainWindow MN = new MainWindow(connstringG, status);
        //        MN.Show();



        //    }


        //}



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
        private void ExecuteSQLStmt(string sql)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.ConnectionString = connstringG;
            conn.Open();
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
        void initServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            MainWindow MN = new MainWindow(connstringG, status);
            MN.Show();
            this.Hide();
            MN.Show();


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
            initServiceWorker.RunWorkerCompleted += initServiceWorker_RunWorkerCompleted;
            initServiceWorker.RunWorkerAsync(1000);
        }
    }
}











//using FaceAttendance.Classes;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Linq;
//using System.Net;
//using System.Net.NetworkInformation;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using UsmanCodeBlocks.Data.Sql.Local;
//using System.Data.SqlClient;
//using System.Data;
//using System.Security.Cryptography;
//using System.IO;
//using FaceAttendance.Classes;
//using System.Drawing;
//using HIKVISION;

//namespace FaceAttendance
//{
//    /// <summary>
//    /// Interaction logic for frmSplash.xaml
//    /// </summary>
//    public partial class frmSplash : Window
//    {
//        BackgroundWorker worker = new BackgroundWorker();
//        BackgroundWorker initServiceWorker = new BackgroundWorker();

//        string inputfile = StringCipher.GetPath();
//        string key = "123";
//        string connstringG = "";

//        private delegate void OnShowMessageHandler(String value);

//        public bool userObtained = false;
//        public bool isServiceRunning = false;
//        public frmSplash()
//        {
//            InitializeComponent();

//            this.Topmost = true;
//        }
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            worker.WorkerReportsProgress = true;
//            worker.DoWork += worker_DoWork;
//            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
//            worker.RunWorkerAsync(1000);
//        }



//        private bool Decrypt()
//        {
//            if (File.Exists(inputfile))
//            {
//                int dot = inputfile.LastIndexOf(".");
//                string keyfile = System.IO.Path.ChangeExtension(inputfile, ".enc");
//                string keyfiledup = System.IO.Path.ChangeExtension(inputfile, ".txt");


//                File.Move(keyfile, System.IO.Path.ChangeExtension(keyfile, ".txt"));

//                StreamReader sr = new StreamReader(keyfiledup);

//                // This is use to specify from where  
//                // to start reading input stream 
//                sr.BaseStream.Seek(0, SeekOrigin.Begin);

//                // To read line from input stream 
//                string str = sr.ReadLine();
//                sr.Close();
//                connstringG = StringCipher.Decrypt(str, key);

//                File.Move(keyfiledup, System.IO.Path.ChangeExtension(keyfiledup, ".enc"));

//                return true;

//            }
//            else
//            {

//            }

//            return false;

//        }

//        void worker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            ShowMessage("Decrypting databse file...");
//            if (Decrypt() == true)
//            {
//                //SqlConnection con = DBFactory.Connect(connstringG);
//                //"Data Source = DESKTOP-7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True"
//                ShowMessage("Getting Database Connection...");
//                bool status = DBFactory.ConnectServer(connstringG);
//                if (status /*== true*/)
//                {
//                    lblProcessing.Content = "Database Connection Successful";
//                    lblProcessing.Foreground = new SolidColorBrush(Colors.Green);
//                }
//                else
//                {
//                    lblProcessing.Content = "Database Connection failed";
//                    lblProcessing.Foreground = new SolidColorBrush(Colors.Red);

//                }
//            }


//            //Globals.UpdateConnectionInfo();

//            //Globals.thisPcMac = Globals.GetMacAddress().ToString();
//            //Globals.mapperMac = tblVISMac.GetRecord(1).MAC;

//            //ShowMessage("Getting Users from FAS...");

//            //DataTable empl = new Employee().GetAll();
//            //if (empl != null)
//            //{
//            //    ShowMessage("Users Obtained...");
//            //    Cache.employeeList = Globals.ConvertDataTable<Employee>(empl);

//            //    Cache.Faculty = (from c in Cache.employeeList
//            //                     where c.Suspend == false && !string.IsNullOrEmpty(c.Name) && c.Section.Contains("Faculty") || c.Section.EndsWith("Staff")
//            //                     select c).ToList();

//            //    userObtained = true;
//            //}
//            //else
//            //{
//            //    Cache.employeeList = null;
//            //    Cache.Faculty = null;

//            //    ShowMessage("FAS Db not Connected...");
//            //    System.Threading.Thread.Sleep(2000);
//            //    userObtained = false;
//            //}
//        }
//        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (userObtained)
//            {
//                initServiceWorker.WorkerReportsProgress = true;
//                initServiceWorker.DoWork += initServiceWorker_DoWork;
//                initServiceWorker.RunWorkerCompleted += initServiceWorker_RunWorkerCompleted;
//                initServiceWorker.RunWorkerAsync(1000);
//            }
//            else
//            {
//                this.Close();
//            }
//        }

//        void initServiceWorker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            ShowMessage("Starting FAS Service...");
//            //e.Result = initializeFAS();
//        }

//        void initServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            ShowMessage("Task Completed");
//        }


//        private void ShowMessage(string v)
//        {
//            if (!Dispatcher.CheckAccess())
//            {
//                Dispatcher.Invoke(new OnShowMessageHandler(this.ShowMessage), new Object[] { v });
//                return;
//            }

//            lblProcessing.Content = v;
//        }



//    }
//}





//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Linq;
//using System.Net;
//using System.Net.NetworkInformation;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace FaceAttendance
//{
//    /// <summary>
//    /// Interaction logic for frmSplash.xaml
//    /// </summary>
//    public partial class frmSplash : Window
//    {
//        BackgroundWorker worker = new BackgroundWorker();
//        BackgroundWorker initServiceWorker = new BackgroundWorker();

//        private delegate void OnShowMessageHandler(String value);

//        public bool userObtained = false;
//        public bool isServiceRunning = false;
//        public frmSplash()
//        {
//            InitializeComponent();

//            this.Topmost = true;
//        }
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            worker.WorkerReportsProgress = true;
//            worker.DoWork += worker_DoWork;
//            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
//            worker.RunWorkerAsync(10000);
//        }
//        void worker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            //Globals.UpdateConnectionInfo();

//            //Globals.thisPcMac = Globals.GetMacAddress().ToString();
//            //Globals.mapperMac = tblVISMac.GetRecord(1).MAC;

//            //ShowMessage("Getting Users from FAS...");

//            //DataTable empl = new Employee().GetAll();
//            //if (empl != null)
//            //{
//            //    ShowMessage("Users Obtained...");
//            //    Cache.employeeList = Globals.ConvertDataTable<Employee>(empl);

//            //    Cache.Faculty = (from c in Cache.employeeList
//            //                     where c.Suspend == false && !string.IsNullOrEmpty(c.Name) && c.Section.Contains("Faculty") || c.Section.EndsWith("Staff")
//            //                     select c).ToList();

//            //    userObtained = true;
//            //}
//            //else
//            //{
//            //    Cache.employeeList = null;
//            //    Cache.Faculty = null;

//            //    ShowMessage("FAS Db not Connected...");
//            //    System.Threading.Thread.Sleep(2000);
//            //    userObtained = false;
//            //}
//        }
//        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (userObtained)
//            {
//                initServiceWorker.WorkerReportsProgress = true;
//                initServiceWorker.DoWork += initServiceWorker_DoWork;
//                initServiceWorker.RunWorkerCompleted += initServiceWorker_RunWorkerCompleted;
//                initServiceWorker.RunWorkerAsync(10000);
//            }
//            else
//            {
//                this.Close();
//            }
//        }

//        void initServiceWorker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            ShowMessage("Starting FAS Service...");
//            //e.Result = initializeFAS();
//        }

//        void initServiceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {

//        }


//        private void ShowMessage(string v)
//        {
//            if (!Dispatcher.CheckAccess())
//            {
//                Dispatcher.Invoke(new OnShowMessageHandler(this.ShowMessage), new Object[] { v });
//                return;
//            }

//            lblProcessing.Content = v;
//        }



//    }
//}
