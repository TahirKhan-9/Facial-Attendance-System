using FaceAttendance.Classes;
using FaceAttendance.Forms;
using FaceAttendance.UserControls;
using HIKVISION;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FaceAttendance
{
    /// <summary>
    /// Interaction logic for DeviceManagement.xaml
    /// </summary>
    public partial class DeviceManagement : Window
    {
        string conString;
        bool Status;
        public DeviceManagement()
        {
            InitializeComponent();
        }
        public DeviceManagement(string ConString,bool status)
        {
            InitializeComponent();
            conString = ConString;
            Status = status;
            if (Status)
            {
                lblDeviceStatus.Content = "Server Connected!";
                lblDeviceStatus.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                btnDevManagement.IsEnabled = false;
                btnStaffManage.IsEnabled = false;
                btnLogM.IsEnabled = false;
                btnAddStaff.IsEnabled = false;
                btnShiftDef.IsEnabled = false;
                btnReportmanage.IsEnabled = false;
                btnAddDept.IsEnabled = false;
                btnCusShift.IsEnabled = false;
                btnTimeSlots.IsEnabled = false;
                btnFirebaseSync.IsEnabled = false;
                btnReltime.IsEnabled = false;
                btnLogMigration.IsEnabled = false;
            }

        }

        
        private void Exit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Are You Sure To Exit Application...???", false);
            message.ShowDialog();
            if (message.yes)
                Application.Current.Shutdown();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Mini_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Max_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            this.maxic.Visibility = this.maxic.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            this.minic.Visibility = this.minic.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            this.MaxTip.ToolTip = this.maxic.Visibility == Visibility.Collapsed ? "Exit Full Screen" : "Full Screen";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnDevManagement_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new UserControl1(conString);
        }

        private void btnStaffManage_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucStaffMan(conString);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new uSettings(conString,Status);
        }

        private void btnLogM_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new uLog(conString);
        }

        //private void btnRTLog_Click(object sender, RoutedEventArgs e)
        //{
        //    UserControlMain.Content = new uRealTimeLog(conString);
        //}

        private void ucCreateshift(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucShifts(conString);
        }

        private void ucCreatereport(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucReports(conString);
        }

        private void btnAddDept_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucAddDept(conString);
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucAddStaff(conString);
        }

        private void btnCusShift_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucCustomShift(conString);
        }
        private void btnTimeSlots_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucTimeSlots(conString);
        }

        private void btnMonitoringClick(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new uRealTimeLog(conString, this);
        }

        private void btnFirebaseSync_Click(object sender, RoutedEventArgs e)
        {

            UserControlMain.Content = new ucFirebaseSync(conString, this);

        }

        private void btnLogMigration_Click(object sender, RoutedEventArgs e)
        {
            UserControlMain.Content = new ucLogsMigration(conString);
        }
    }
}
