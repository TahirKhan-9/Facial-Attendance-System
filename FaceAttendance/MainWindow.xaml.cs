using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UsmanCodeBlocks.Data.Sql.Local;
using System.Data.SqlClient;
using System.Data;
using FaceAttendance.Classes;
//using System.Drawing;
using HIKVISION;
using System.Diagnostics;
using System.IO;
using System;

namespace FaceAttendance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string inputfile = StringCipher.GetPath();
       // string key = "123";
        string connstringG = "";

        bool Status;

        public MainWindow(string ConnstringG, bool status)
        {
            InitializeComponent();
            connstringG = ConnstringG;
            Status = status;

            
        }



        //private bool Decrypt()
        //{
        //    if (File.Exists(inputfile))
        //    {
        //        int dot = inputfile.LastIndexOf(".");
        //        string keyfile = System.IO.Path.ChangeExtension(inputfile, ".enc");
        //        string keyfiledup = System.IO.Path.ChangeExtension(inputfile, ".txt");


        //        File.Move(keyfile, System.IO.Path.ChangeExtension(keyfile, ".txt"));

        //        StreamReader sr = new StreamReader(keyfiledup);

        //        // This is use to specify from where  
        //        // to start reading input stream 
        //        sr.BaseStream.Seek(0, SeekOrigin.Begin);

        //        // To read line from input stream 
        //        string str = sr.ReadLine();
        //        sr.Close();
        //        connstringG = StringCipher.Decrypt(str, key);

        //        File.Move(keyfiledup, System.IO.Path.ChangeExtension(keyfiledup, ".enc"));

        //        return true;

        //    }
        //    else
        //    {

        //    }

        //    return false;

        //}

        private void mainWindow_PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            if(e.Key == Key.Enter) 
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            /*//3O9BTg56Ex3ak3qV6W5JiRgkJ/JGMbrlhFgZMa8EcUAYozXRBhtRiK5qf/b/uYkW7MOMIqBLaaJGuXD4Ow9cjUA1UThKZruZv9GtDwQkqL7gKP1xmZSmnABrWvb5cIA6XcHIfW/DleRtubKE6GAJQdnPr3kskvI5gC/kbiGZM9ghxvgtLiU45GgjZHQCBTdO50RIwYoBK+jk3hYQo13HyQ==
            //string connstring = "Data Source = DESKTOP-7V9OI2S\\SQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True";*/

            SqlConnection con = DBFactory.Connect(connstringG);

            //SqlConnection con = new SqlConnection(connstringG);
            //MessageBox.Show(connstringG);

            if (txtLoginID.Text != "" && txtPassword.Password != "")
            {
                if ((txtLoginID.Text.ToString() == "Admin" && txtPassword.Password.ToString() == "admin"))
                {
                    //MessageBox.Show("login succesful");
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Login Successful", false);
                    message.ShowDialog();
                    if (message.yes)
                    {
                        DeviceManagement deviceManagement = new DeviceManagement(connstringG,Status);
                        deviceManagement.Show();
                        this.Hide();
                    }
                }
                else
                {
                    if (Status)
                    {
                        SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM [tbl_users] WHERE [UId]='" + txtLoginID.Text + "' AND [Pass]='" + txtPassword.Password + "'", con);
                        /* in above line the program is selecting the whole data from table and the matching it with the user name and password provided by user. */
                        DataTable dt = new DataTable(); //this is creating a virtual table  
                        sda.Fill(dt);
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            try
                            {
                                /* I have made a new page called home page. If the user is successfully authenticated then the form will be moved to the next form */
                                //this.Hide();
                                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Success, "Login Successful", false);
                                message.ShowDialog();
                                if (message.yes)
                                {
                                   
                                    DeviceManagement deviceManagement = new DeviceManagement(connstringG, Status);
                                    deviceManagement.Show();
                                    this.Hide();
                                    //StartServ();
                                }
                            }
                            catch (Exception ex)
                            {
                                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                                string msg = "( " + DateTime.Now + " ) (btn_Login  ) ( " + ex.Message.ToString() + " )" + Environment.NewLine;
                                File.AppendAllText(path, msg);
                            }
                          
                        }
                        else
                        {
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Wrong User Id or Password", false);
                            message.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Database not Connected!", false);
                        message.ShowDialog();
                    }
                    
                }
            }
            else
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Fields Emplty.", false);
                message.ShowDialog();
            }

            //DataTable dt = new DataTable();
            //dt= DBFactory.GetAllByQuery(con,dt);
            //con;



            // DBFactory.Disconnect(con);
        }
        private void StartServ()
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
        private void main_loaded(object sender, RoutedEventArgs e)
        {
            if (Status /*== true*/)
            {
                statusshow.Content = "Database Connection Successful";
                statusshow.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                statusshow.Content = "Database Connection failed";
                statusshow.Foreground = new SolidColorBrush(Colors.Red);

            }
            


            //if (Decrypt() == true)
            //{
            //    //SqlConnection con = DBFactory.Connect(connstringG);
            //    //"Data Source = DESKTOP-7V9OI2SSQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True"
            //    bool status = DBFactory.ConnectServer(connstringG);
            //    if (status /*== true*/)
            //    {
            //        statusshow.Content = "Database Connection Successful";
            //        statusshow.Foreground = new SolidColorBrush(Colors.Green);
            //    }
            //    else
            //    {
            //        statusshow.Content = "Database Connection failed";
            //        statusshow.Foreground = new SolidColorBrush(Colors.Red);

            //    }
            //}
            //string connstring = "Data Source = DESKTOP - 7V9OI2S\\SQLEXPRESS; Initial Catalog = FMDB; Integrated Security = True";

        }

        private void exitfunc(object sender, RoutedEventArgs e)
        {
            //this.Close();
            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Are You Sure To Exit Application...???", false);
            message.ShowDialog();
            if (message.yes)
                Application.Current.Shutdown();

        }

        private void txtLoginID_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLoginID1.Visibility = Visibility.Collapsed;
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword1.Visibility = Visibility.Collapsed;
        }

        //private void set_tb_enter(object sender, TouchEventArgs e)
        //{
        //    if (txtLoginID.Text=="")
        //    {
        //        txtLoginID1.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        txtLoginID1.Visibility = Visibility.Collapsed;
        //    }
        //    if (txtPassword.Password == "")
        //    {
        //        txtPassword1.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        txtPassword1.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void set_tb_enter(object sender, MouseEventArgs e)
        {
            if (txtLoginID.Text == "")
            {
                txtLoginID1.Visibility = Visibility.Visible;
            }
            else
            {
                txtLoginID1.Visibility = Visibility.Collapsed;
            }
            if (txtPassword.Password == "")
            {
                txtPassword1.Visibility = Visibility.Visible;
            }
            else
            {
                txtPassword1.Visibility = Visibility.Collapsed;
            }
        }

        private void set_tb_leave(object sender, MouseEventArgs e)
        {
            if (txtLoginID.Text == "")
            {
                txtLoginID1.Visibility = Visibility.Visible;
            }
            else
            {
                txtLoginID1.Visibility = Visibility.Collapsed;
            }
            if (txtPassword.Password == "")
            {
                txtPassword1.Visibility = Visibility.Visible;
            }
            else
            {
                txtPassword1.Visibility = Visibility.Collapsed;
            }
        }
    }
}
