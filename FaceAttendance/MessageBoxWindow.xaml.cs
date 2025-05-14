using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HIKVISION
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public enum MessageBoxType { Info = 0, Alert = 1, Error = 2, Confirm = 3, Success = 4 };
    public partial class MessageBoxWindow : Window
    {
        public bool yes = false;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        MessageBoxType MsgType;
        private MessageBoxWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtMessage.HorizontalContentAlignment = HorizontalAlignment.Left;
            btnOk.Focus();
            this.Topmost = true;
        }
        //public MessageBoxWindow(MessageBoxType type, string message, bool AutoClose , bool showCancel = false, Window _owner = null)
        //{
        //    InitializeComponent();
        //    this.Owner = _owner;
        //    imgMsg.Source = null;
        //    MsgType = type;
        //    if (showCancel)
        //        btnCancel.Visibility = Visibility.Collapsed;
        //    switch (type)
        //    {
        //        case MessageBoxType.Alert:
        //            btnOk.Content = "OK";
        //            btnCancel.Content = "CANCEL";
        //            alertic.Visibility = Visibility.Visible;
        //            break;
        //        case MessageBoxType.Confirm:
        //            btnOk.Content = "YES";
        //            btnCancel.Content = "NO";
        //            quesic.Visibility = Visibility.Visible;
        //            break;
        //        case MessageBoxType.Error:
        //            btnOk.Content = "OK";
        //            btnCancel.Content = "CANCEL";
        //            erroric.Visibility = Visibility.Visible;
        //            break;
        //        case MessageBoxType.Info:
        //            btnOk.Content = "OK";
        //            btnCancel.Content = "CANCEL";
        //            infoic.Visibility = Visibility.Visible;
        //            break;
        //        case MessageBoxType.Success:
        //            btnOk.Content = "OK";
        //            btnCancel.Content = "CANCEL";
        //            successic.Visibility = Visibility.Visible;
        //            break;
        //        default:
        //            btnOk.Content = "OK";
        //            btnCancel.Content = "CANCEL";
        //            infoic.Visibility = Visibility.Visible;
        //            break;
        //    }

        //    if (AutoClose)
        //    {
        //        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        //        dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
        //        dispatcherTimer.Start();
        //    }
        //    txtMessage.Text = message;
        //}


        public MessageBoxWindow(MessageBoxType type, string message, bool AutoClose = false, bool showCancel = false,Window _owner = null)
        {
            InitializeComponent();
            this.Owner = _owner;
            imgMsg.Source = null;
            MsgType = type;
            if (showCancel)
                btnCancel.Visibility = Visibility.Collapsed;
            switch (type)
            {
                case MessageBoxType.Alert:
                    btnOk.Content = "OK";
                    btnCancel.Content = "CANCEL";
                    alertic.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Confirm:
                    btnOk.Content = "YES";
                    btnCancel.Content = "NO";
                    quesic.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Error:
                    btnOk.Content = "OK";
                    btnCancel.Content = "CANCEL";
                    erroric.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Info:
                    btnOk.Content = "OK";
                    btnCancel.Content = "CANCEL";
                    infoic.Visibility = Visibility.Visible;
                    break;
                case MessageBoxType.Success:
                    btnOk.Content = "OK";
                    btnCancel.Content = "CANCEL";
                    successic.Visibility = Visibility.Visible;
                    break;
                default:
                    btnOk.Content = "OK";
                    btnCancel.Content = "CANCEL";
                    infoic.Visibility = Visibility.Visible;
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                    dispatcherTimer.Start();
                    break;
            }

            if (AutoClose)
            {
                btnOk.Visibility = Visibility.Collapsed;
                btnCancel.Visibility = Visibility.Collapsed;
                if (type != MessageBoxType.Success)
                {
                    txtMessage.HorizontalAlignment = HorizontalAlignment.Center;
                    if (message.EndsWith("f1f2"))
                    {
                        string msgCopy = message.Clone().ToString();
                        message = "";
                        for (int i = 0; i < msgCopy.Length - 4; i++)
                        {
                            message += msgCopy[i];
                        }
                        txtMessage.FontSize = 18;
                    }
                    else
                    {
                        txtMessage.FontSize = 34;
                    }
                    //txtMessage.Margin = new Thickness(110, 0, 0, 0);//.Padding=PaddingProperty;
                }
                if (type == MessageBoxType.Success)
                {
                    if (message.EndsWith("f1f2"))
                    {
                        string msgCopy = message.Clone().ToString();
                        message = "";
                        for (int i = 0; i < msgCopy.Length - 4; i++)
                        {
                            message += msgCopy[i];
                        }
                        txtMessage.FontSize = 18;
                    }
                    else
                    {
                        txtMessage.FontSize = 34;
                    }
                }

                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                dispatcherTimer.Start();
            }
            txtMessage.Text = message;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.IsEnabled = false;
            this.Close();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            yes = true;
            if (dispatcherTimer != null)
                dispatcherTimer.IsEnabled = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            yes = false;
            this.Close();
        }

        private void OnKeyPress_Press(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnOk_Click(null, null);
            }
        }

    }
}
