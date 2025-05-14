using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucCustomShift.xaml
    /// </summary>
    public partial class ucCustomShift : UserControl
    {
        int del_id;
        ObservableCollection<tbl_CusShift> lstCusShift;
        public string connnString;
        static bool flag = false;
        public ucCustomShift(string connString)
        {
            InitializeComponent();
            connnString = connString;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CusShiftList();
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtTimeIn.Text))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter MIN TIME IN", false);
                    msg.ShowDialog();
                    txtTimeIn.Focus();
                    return;
                }
                if (String.IsNullOrEmpty(txtTimeOut.Text))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter MAX TIME OUT", false);
                    msg.ShowDialog();
                    txtTimeOut.Focus();
                    return;
                }

                if (flag == true)
                {
                    try
                    {
                        //string query = "Delete from tbl_dev where MId =" + txtid;
                        string query = "Delete from tbl_CusShift where id =" + del_id;
                        DBFactory.Delete(connnString, query);
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
                if (IsDateTime(txtTimeIn.Text) && IsDateTime(txtTimeOut.Text))
                {
                    string query = "insert into tbl_CusShift (Time_IN,Time_OUT) values ('" + txtTimeIn.Text + "','" + txtTimeOut.Text + "')";
                    int result = DBFactory.Insert(connnString, query);
                    StatusWindow.Content = "Completed";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Content.ToString(), false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    if (result == 1)
                    {
                        CusShiftList();
                        disable_controls();

                        reset_controls();
                        statusbar_green();
                        txtName.Visibility = Visibility.Visible;
                        txtDes.Visibility = Visibility.Visible;
                    }
                }
               
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( AddDept  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }

        }
        public static bool IsDateTime(string dt)
        {
            DateTime tmpDt;
            return DateTime.TryParse(dt, out tmpDt);
        }
        public void CusShiftList()
        {
            lstCusShift = new ObservableCollection<tbl_CusShift>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connnString);
            if (con == true)
            {
                dt = DBFactory.GetAllByQuery(connnString, "select * from tbl_CusShift");
                lstCusShift = ConvertDataTableObs<tbl_CusShift>(dt);
                lvdevinfo.ItemsSource = lstCusShift;

            }
            else
            {
                lvdevinfo.ItemsSource = null;
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

        private void AddDev_MouseDown(object sender, MouseButtonEventArgs e)
        {
            enable_controls();
            txtTimeIn.Focus();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Visibility = Visibility.Collapsed;
            txtDes.Visibility = Visibility.Collapsed;

            if (lvdevinfo.SelectedItems.Count == 0)
            {
                statusbar_red();
                return;
            }
            else
            {
                flag = true;
                enable_controls();

                foreach (tbl_CusShift dpt in lvdevinfo.SelectedItems)
                {
                    del_id = dpt.id;
                    txtTimeIn.Text = dpt.Time_IN.ToString();
                    txtTimeOut.Text = dpt.Time_OUT.ToString();
                }
                statusbar_green();
            }
        }

        private void txtDevName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDes.Visibility = Visibility.Collapsed;
        }

        private void txtDeptDesc_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.Visibility = Visibility.Collapsed;
        }
        public void enable_controls()
        {
            btnsave.IsEnabled = true;
            txtTimeIn.IsReadOnly = false;
            txtTimeOut.IsReadOnly = false;

        }
        public void disable_controls()
        {
            btnsave.IsEnabled = false;
            txtTimeIn.IsReadOnly = true;
            txtTimeOut.IsReadOnly = true;
        }
        public void reset_controls()
        {
            txtTimeIn.Text = "";
            txtTimeOut.Text = "";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (lvdevinfo.SelectedItems.Count == 0)
            {
                statusbar_red();
                return;
            }
            else
            {
                foreach (tbl_CusShift dpt in lvdevinfo.SelectedItems)
                {
                    del_id = dpt.id;
                }

                try
                {
                    string query = "Delete from tbl_CusShift where id ='" + del_id + "'";
                    DBFactory.Delete(connnString, query);

                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                statusbar_green();
                CusShiftList();
            }
        }
        public void statusbar_green()
        {
            StatusWindow.Content = "Task Completed";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Green);
        }

        public void statusbar_yellow()
        {
            StatusWindow.Content = "Wait! In Progress... ";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        public void statusbar_red()
        {
            StatusWindow.Content = "Task Not Completed";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void DoubleArrowb_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
