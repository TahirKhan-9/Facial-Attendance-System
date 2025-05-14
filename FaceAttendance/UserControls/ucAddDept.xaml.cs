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
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UsmanCodeBlocks.Data.Sql.Local;

namespace FaceAttendance.UserControls
{
   
    /// <summary>
    /// Interaction logic for ucAddDept.xaml
    /// </summary>
    public partial class ucAddDept : UserControl
    {
        int del_id;
        ObservableCollection<Departments> lstDept;
        public string connnString;
        static bool flag = false;
        public ucAddDept(string connString)
        {
            InitializeComponent();
            connnString = connString;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dept_list();
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int result = 0;
                if (String.IsNullOrEmpty(txtdeptName.Text))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Department Name", false);
                    msg.ShowDialog();
                    txtdeptName.Focus();
                    return;
                }
                if(string.IsNullOrEmpty(txtdeptDesc.Text))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Department ID", false);
                    msg.ShowDialog();
                    txtdeptDesc.Focus();
                    return;
                }
                int departmentID;
                if(!int.TryParse(txtdeptDesc.Text, out departmentID))
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please enter only numeric values in the Department ID field.", false);
                    msg.ShowDialog();
                    txtdeptDesc.Focus();
                    return;
                }
                if(departmentID == 0)
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please enter Department ID > 0.", false);
                    msg.ShowDialog();
                    txtdeptDesc.Focus();
                    return;
                }
               
                if (flag == true)
                {
                    try
                    {
                        //string query = "Delete from tbl_dev where MId =" + txtid;
                        string query = $"update tbl_department set dept_name = '{txtdeptName.Text}'  where dept_id =" + del_id;
                        bool updated = DBFactory.Update(connnString, query);
                        result = updated ? 1 : 0;
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
                //string saveDeptQuer = "insert into tbl_department(dept_name, dept_id) values('" + txtdeptName.Text + "')";
                string saveDeptQuer = $"insert into tbl_department(dept_name, dept_id) values('{txtdeptName.Text}', {txtdeptDesc.Text})";
                result = DBFactory.Insert(connnString, saveDeptQuer);

                }
                if (result == 1)
                {
                    dept_list();
                    statusbar_green();
                }
                else
                {
                    statusbar_red();
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Changes are not saved.", false);
                    msg.ShowDialog();

                }
                disable_controls();
                txtName.Visibility = Visibility.Visible;
                txtDes.Visibility = Visibility.Visible;
                reset_controls();
                
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( AddDept  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
           
        }
        public void dept_list()
        {
            lstDept = new ObservableCollection<Departments>();
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connnString);
            if (con == true)
            {
                dt = DBFactory.GetAllByQuery(connnString, "select * from tbl_department");
                lstDept = ConvertDataTableObs<Departments>(dt);
                lvdevinfo.ItemsSource = lstDept;

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
            lvdevinfo.SelectedItems.Clear();
            if (flag)
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you don't want to save the change.", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                flag = false;
                enable_controls();
                reset_controls();
                _ = txtdeptName.Focus();
            }
            else
            {
                enable_controls();
                _ = txtdeptDesc.Focus();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //txtName.Visibility = Visibility.Collapsed;
            //txtDes.Visibility = Visibility.Collapsed;
           
            if (lvdevinfo.SelectedItems.Count == 0)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Select Department to modify.", false);
                msg.ShowDialog();
                //statusbar_red();
                return;
            }
            if (lvdevinfo.SelectedItems.Count > 1)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please select only one department to update at a time.", false);
                msg.ShowDialog();
                //statusbar_red();
                return;
            }
            else
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to update the selected department?", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                flag = true;
                enable_controls();

                foreach (Departments dpt in lvdevinfo.SelectedItems)
                {
                    del_id = dpt.dept_id;
                    txtdeptDesc.Text = del_id.ToString();
                    txtdeptDesc.IsReadOnly = true;
                    txtDes.Visibility = Visibility.Collapsed;
                    txtName.Visibility = Visibility.Collapsed;
                    txtdeptName.Text = dpt.dept_name.ToString();                }
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
            txtdeptName.IsReadOnly = false;
            txtdeptDesc.IsReadOnly = false;

        }
        public void disable_controls()
        {
            btnsave.IsEnabled = false;
            txtdeptName.IsReadOnly = true;
            txtdeptDesc.IsReadOnly = true;
        }
        public void reset_controls()
        {
            txtdeptName.Text = "";
            txtdeptDesc.Text = "";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            if (lvdevinfo.SelectedItems.Count == 0)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Select Department to Delete.", false);
                msg.ShowDialog();
                statusbar_red();
                return;
            }
            else
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to delete the selected department?", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                if (flag)
                {
                    flag = false;
                    disable_controls();
                    reset_controls();
                }
                foreach (Departments dpt in lvdevinfo.SelectedItems)
                {
                    del_id = dpt.dept_id;
                }

                try
                {
                    string query = "Delete from tbl_department where dept_id ='" + del_id + "'";
                    bool deleted = DBFactory.Delete(connnString, query);
                    if (deleted)
                    {
                        statusbar_green();
                        dept_list();

                    }

                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Error\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
        }
        public void statusbar_green()
        {
            StatusWindow.Content = "Task Completed";
            StatusWindow.FontSize = 24;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Green);
            Thread newThread = new Thread(ClearStatusWindow);
            newThread.Start();
        }

        public void statusbar_yellow()
        {
            StatusWindow.Foreground = new SolidColorBrush(Colors.Yellow);
            StatusWindow.Content = "Wait! In Progress... ";
            StatusWindow.FontSize = 24;
            Thread newThread = new Thread(ClearStatusWindow);
            newThread.Start();

        }

        public void statusbar_red()
        {
            StatusWindow.Foreground = new SolidColorBrush(Colors.Red);
            StatusWindow.Content = "Task Not Completed";
            StatusWindow.FontSize = 24;
            Thread newThread = new Thread(ClearStatusWindow);
            newThread.Start();
        }

        private void DoubleArrowb_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void ClearStatusWindow()
        {
            Thread.Sleep(2000);
            Action UpdateUI = () =>
            {
                StatusWindow.Content = "";
            };
            Dispatcher.Invoke(UpdateUI);
        }
    }
}
