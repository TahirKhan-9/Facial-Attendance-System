using FaceAttendance.Classes;
using System.Windows;

namespace FaceAttendance
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (SingleInstance.AlreadyRunning())
                App.Current.Shutdown(); // Just shutdown the current application,if any instance found.  

            base.OnStartup(e);
        }
    }
}
