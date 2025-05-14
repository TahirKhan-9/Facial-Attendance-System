using AxFP_CLOCKLib;
using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using UsmanCodeBlocks.Data.Sql.Local;
using FaceAttendance.UserControls;

namespace FaceAttendance.Forms
{

    /// <summary>
    /// Interaction logic for ucStaffMan.xaml
    /// </summary>
    public partial class ucStaffMan : UserControl
    {
        public string sql;
        public string connstring;
        string fingData;
        UdownDisp user;
        int duplEnr = 0;
        DataTable dptTable;
        List<Departments> dptNames;
        string dptID;
        string facNotUpload = "";
        bool upc, doc = false;
        int upFaceCount = 0;
        int upFingCount = 0;
        int cardUpCount = 0;
        ObservableCollection<gdevId> gId;
        ObservableCollection<UdownDisp> EnD;
        ObservableCollection<tbl_DevGroups> DevGroupList;
        List<UdownDisp> enrLst;
        int count;
        List<UdownDisp> notFace = new List<UdownDisp>();
        List<UdownDisp> notFingger = new List<UdownDisp>();
        private byte[] gbytEnrollDataFace;
        private byte[] gbytEnrollData;

        public ucStaffMan(string constring)
        {
            try
            {
                connstring = constring;
                InitializeComponent();

                //for searching in Download data list
                UD_List();
                //gId = new List<gdevId>();
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(downuserlist.ItemsSource);
                view.Filter = UserFilter;

                //for searching on Upload user List
                DbU_List();
                CollectionView viewforUploadTabLeft = (CollectionView)CollectionViewSource.GetDefaultView(dbuserlist.Items);
                viewforUploadTabLeft.Filter = UserFilterForUpload;

                //for searching on Delete user List
                DelU_List();
                CollectionView viewfordel = (CollectionView)CollectionViewSource.GetDefaultView(deleteuserlist.ItemsSource);
                viewfordel.Filter = UserFilterForDel;
                string quer = "select * from tbl_enroll";
                enrLst = Globals.ConvertDataTable<UdownDisp>(DBFactory.GetAllByQuery(connstring, quer));
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( ucStaffMan  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }

        }

        private async void Download_Data_Click(object sender, RoutedEventArgs e)
        {
            if (Devlist.SelectedItems.Count == 0 && GroupList.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "No Device or Group is selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }

            StatusWindow.Text = "Wait...\tDownloading Files";
            D_Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            //IEnumerable items = this.Devlist.SelectedItems;
            List<gdevId> items = new List<gdevId>();
            if (Devlist.SelectedItems.Count >= 1)
            {
                foreach (gdevId item in Devlist.SelectedItems)
                {
                    items.Add(item);
                }
            }
            if (GroupList.SelectedItems.Count >= 1)
            {
                items = await FetchMachiesFromGroup(GroupList.SelectedItems);
                if (items.Count == 0)
                {
                    string txt = GroupList.SelectedItems.Count == 1 ? "group is" : "groups are";
                    StatusWindow.Text = $"Selected {txt} empty!";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
            }
            LoadingWindow loader = new LoadingWindow();
            foreach (gdevId gid in items)
            {
                try
                {
                    if (condev(gid.Ip.ToString(), gid.MId))
                    {
                        //string query = "delete from tbl_enroll"; 

                        string query = "delete from tbl_enroll where EMachineNumber=" + gid.MId;
                        DBFactory.Delete(connstring, query);

                        //await Task.Run(() => { get_enroll_data(gid.Ip.ToString(), gid.MId); });
                        //await GetEnrollDataAsync(gid.Ip.ToString(), gid.MId);
                        await get_enroll_data(gid.Ip.ToString(), gid.MId, loader);
                        //string sqlforBU = "INSERT INTO tbl_unblockedUsers SELECT [EMachineNumber], [EnrollNumber] ,[FingerNumber] ,[Privilige] ,[enPassword] ,[EName] FROM tbl_enroll WHERE NOT EXISTS (SELECT * FROM tbl_blockedUsers WHERE tbl_blockedUsers.EMachineNumber = tbl_enroll.EMachineNumber  and tbl_blockedUsers.EnrollNumber = tbl_enroll.EnrollNumber)";
                        //DBFactory.Insert(connstring, sqlforBU);
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();

                    }
                    if (doc == false)
                    {
                        StatusWindow.Text = "Download Failed!";
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    loader.Hide();
                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                    string msg = "( " + DateTime.Now + " ) ( DownloadData  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                    File.AppendAllText(path, msg);
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }

            }

            downuserlist.ItemsSource = null;
            UD_List();
            D_Wait.Visibility = Visibility.Collapsed;

        }
        private async Task<List<gdevId>> FetchMachiesFromGroup(IEnumerable groupList)
        {
            List<gdevId> Items = new List<gdevId>();
            foreach (tbl_DevGroups item in groupList)
            {
                DataTable dt = await DBFactory.GetAllByQueryAsync(connstring, $"Select MId, Ip from tbl_dev where GroupID = {item.ID}");
                foreach (DataRow row in dt.Rows)
                {
                    gdevId obj = new gdevId();
                    obj.MId = int.Parse(row["MId"].ToString());
                    obj.Ip = row["Ip"].ToString();
                    Items.Add(obj);
                }
            }
            return Items;
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
        private async Task<bool> DisableDeviceAsync(string ip, int id)
        {
            await condevAsync(ip, id);
            bool result = await Task.Run(() => {
                bool bRet = axFP_CLOCK.EnableDevice(id, 0);
                return bRet;
            });
            return result;
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
        public async Task<bool> condevAsync(string ip, int id)
        {
            bool result = await Task.Run(() =>
            {
                bool bRet = axFP_CLOCK.SetIPAddress(ref ip, 5005, 0);
                if (bRet)
                {
                    bRet = axFP_CLOCK.OpenCommPort(id);
                }
                return bRet;
            });
            return result;
        }

        private void EnableDevice(int id)
        {

            axFP_CLOCK.EnableDevice(id, 1);
        }
        private async Task EnableDeviceAsync(int id)
        {
            await Task.Run(() => {
                axFP_CLOCK.EnableDevice(id, 1);
            });
        }
        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            axFP_CLOCK.GetLastError(ref nErrorValue);

        }
        private async Task ShowErrorInfoAsync()
        {
            int nErrorValue = 0;
            await Task.Run(() =>
            {
                axFP_CLOCK.GetLastError(ref nErrorValue);
            });
        }

        //Uploads data from System and write it in the machine
        private async Task set_enroll_data(string ip, int id)
        {

            bool bRet = false;

            int dwEMachineNumber;
            string dwName;
            int dwEnrollNumber;
            int dwFingerNumber;
            int dwPrivilege;
            int dwPassword;
            int[] dwFPData = new int[1420 / 4];
            int[] FacedwFPData = new int[1888 / 4];
            object obj = 0;

            if (selecteduserlist.Items.Count == 0)
            {

                return;
            }
            //DataRow dRow = dra[1];


            //Thread[] array = new Thread[selecteduserlist.Items.Count];
            int count = 0;
            IList<UdownDisp> selectedItems = selecteduserlist.Items.Cast<UdownDisp>().ToList();
            bool faceUpChecked = (bool)faceUp.IsChecked;
            bool fingUpChecked = (bool)fingUp.IsChecked;
            bool cardUpChecked = (bool)cardUp.IsChecked;
            await Task.Run(() =>
            {
                //foreach (UdownDisp item in selecteduserlist.Items)
                foreach(UdownDisp item in selectedItems)
                {
                    dwEMachineNumber = Int32.Parse(item.EMachineNumber.ToString());
                    dwEnrollNumber = Int32.Parse(item.EnrollNumber.ToString());
                    dwFingerNumber = Int32.Parse(item.FingerNumber.ToString());
                    dwPrivilege = Int32.Parse(item.Privilige.ToString());
                    dwPassword = Int32.Parse(item.enPassword.ToString());
                    dwName = item.EName.ToString();
                    if (item.FPData != "Empty")
                    {
                        obj = Convert.FromBase64String(item.FPData);
                    }
                    //// Start the thread with a ParameterizedThreadStart.
                    //ParameterizedThreadStart start = new ParameterizedThreadStart(UploadContentinloop);
                    //array[count] = new Thread(start);
                    //array[count].Start(count);
                    //count++;

                    //ucStaffMan ucstf = new ucStaffMan(connstring);
                    //Thread newThread = new Thread(ucstf.UploadContentinloop);
                    //newThread.Start(42);
                    //enrLst = DBFactory.GetAllByQuery()
                    //if (faceUp.IsChecked == true && fingUp.IsChecked == true && cardUp.IsChecked == true)
                    if (faceUpChecked && fingUpChecked && cardUpChecked)
                    {
                        Thread t = new Thread(() => UploadContentinloop(ref count, ref axFP_CLOCK, ref upc, id, bRet, dwEMachineNumber, dwName, dwEnrollNumber, dwFingerNumber, dwPrivilege, dwPassword, dwFPData, FacedwFPData, obj));
                        t.Start();
                        t.Join();
                    }
                    else
                    {
                        //if (faceUp.IsChecked == true)
                        if(faceUpChecked)
                        {
                            UdownDisp user = enrLst.Where(x => x.EnrollNumber == item.EnrollNumber && x.FingerNumber == item.FingerNumber).FirstOrDefault();
                            if (user != null)
                            {
                                if (user.FingerNumber == 50)
                                {
                                    //Thread t = new Thread(() => UploadContentinloop(ref count, ref axFP_CLOCK, ref upc, id, bRet, dwEMachineNumber, dwName, dwEnrollNumber, dwFingerNumber, dwPrivilege, dwPassword, dwFPData, FacedwFPData, obj));
                                    Thread t = new Thread(() => UploadContentinloop(ref count, ref axFP_CLOCK, ref upc, id, bRet, user.EMachineNumber, user.EName, user.EnrollNumber, user.FingerNumber, user.Privilige, user.enPassword, dwFPData, FacedwFPData, obj));
                                    t.Start();
                                    t.Join();

                                }
                            }
                            else
                            {
                                notFace.Add(item);

                            }
                        }
                        //if (fingUp.IsChecked == true)
                        if(fingUpChecked)
                        {
                            UdownDisp user = enrLst.Where(x => x.EnrollNumber == item.EnrollNumber && x.FingerNumber == item.FingerNumber).FirstOrDefault();
                            if (user != null)
                            {
                                if (user.FingerNumber == 0)
                                {
                                    Thread t = new Thread(() => UploadContentinloop(ref count, ref axFP_CLOCK, ref upc, id, bRet, user.EMachineNumber, user.EName, user.EnrollNumber, user.FingerNumber, user.Privilige, user.enPassword, dwFPData, FacedwFPData, obj));
                                    t.Start();
                                    t.Join();
                                    //upFingCount++;
                                }
                            }
                            else
                            {
                                notFingger.Add(item);
                            }
                            user = null;
                        }
                        //if (cardUp.IsChecked == true)
                        if(cardUpChecked)
                        {
                            UdownDisp user = enrLst.Where(x => x.EnrollNumber == item.EnrollNumber && x.FingerNumber == item.FingerNumber).FirstOrDefault();
                            if (user != null)
                            {
                                if (user.FingerNumber == 11)
                                {
                                    Thread t = new Thread(() => UploadContentinloop(ref count, ref axFP_CLOCK, ref upc, id, bRet, user.EMachineNumber, user.EName, user.EnrollNumber, user.FingerNumber, user.Privilige, user.enPassword, dwFPData, FacedwFPData, obj));
                                    t.Start();
                                    t.Join();
                                    //cardUpCount++;
                                }
                            }
                            else
                            {
                                notFingger.Add(item);
                            }
                            user = null;
                        }
                    }




                    //ThreadStart ts = new ThreadStart(UploadContentinloop(ref axFP_CLOCK, ref upc, id, bRet, dwEMachineNumber, dwName, dwEnrollNumber, dwFingerNumber, dwPrivilege, dwPassword, dwFPData, FacedwFPData, obj));
                    // create new thread
                    //Thread thrd = new Thread(ts);
                    //// start thread
                    //thrd.Start();



                    //        initServiceWorker.WorkerReportsProgress = true;
                    //        initServiceWorker.DoWork += initServiceWorker_DoWork(id, bRet, dwEMachineNumber,
                    // dwName,
                    // dwEnrollNumber,
                    // dwFingerNumber,
                    // dwPrivilege,
                    // dwPassword,
                    //dwFPData,
                    // FacedwFPData,
                    // obj);
                    //        initServiceWorker.RunWorkerCompleted += initServiceWorker_RunWorkerCompleted;
                    //        initServiceWorker.RunWorkerAsync(1000);





                    //dwEMachineNumber = Int32.Parse(item.EMachineNumber.ToString());
                    //dwEnrollNumber = Int32.Parse(item.EnrollNumber.ToString());
                    //dwFingerNumber = Int32.Parse(item.FingerNumber.ToString());
                    //dwPrivilege = Int32.Parse(item.Privilige.ToString());
                    //dwPassword = Int32.Parse(item.enPassword.ToString());
                    //dwName = item.EName.ToString();

                    //List<int> ToUpload = new List<int>() { dwEMachineNumber, dwEnrollNumber, dwFingerNumber, dwPrivilege, dwPassword };


                    //if (dwFingerNumber < 10)
                    //{
                    //    //obj = new System.Runtime.InteropServices.VariantWrapper(item. FPData);
                    //}
                    //else if (dwFingerNumber == 50)
                    //{
                    //    obj = new System.Runtime.InteropServices.VariantWrapper(dwFPData);
                    //}
                    //else if (dwFingerNumber > 19)
                    //{
                    //    //obj = new System.Runtime.InteropServices.VariantWrapper(dr["FPData"]);
                    //}
                    //else
                    //{
                    //    obj = new System.Runtime.InteropServices.VariantWrapper(dwFPData);
                    //}

                    //if (dwFingerNumber == 50)
                    //{
                    //    upc = true;
                    //    //int vPhotoSize = 0;
                    //    int[] indexDataFacePhoto = new int[400800];
                    //    //分配内存
                    //    IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                    //    //string path = @"c:\test.jpg";                     
                    //    string path = @"C:\\PHOTO\" + "D" + item.EMachineNumber.ToString() + "U" + dwEnrollNumber.ToString() + ".jpg"; //\" + dwEnrollNumber.ToString() + ".jpg";
                    //    bRet = System.IO.File.Exists(path);
                    //    if (bRet)
                    //    {
                    //        byte[] mbytCurEnrollData = System.IO.File.ReadAllBytes(path);
                    //        Marshal.Copy(mbytCurEnrollData, 0, ptrIndexFacePhoto, mbytCurEnrollData.Length);
                    //        bRet = axFP_CLOCK.SetEnrollPhotoCS(id, dwEnrollNumber, mbytCurEnrollData.Length, ptrIndexFacePhoto);

                    //        object objN = new System.Runtime.InteropServices.VariantWrapper(dwName);

                    //        bool nRet = axFP_CLOCK.SetUserName(0,
                    //          id,
                    //          dwEnrollNumber,
                    //          dwEMachineNumber,
                    //          ref objN
                    //          );
                    //    }
                    //    else
                    //    {
                    //        bRet = true;
                    //    }

                    //}
                    //else
                    //{
                    //    upc = true;
                    //    bRet = axFP_CLOCK.SetEnrollData(
                    //        id,
                    //        dwEnrollNumber,
                    //        dwEMachineNumber,
                    //        dwFingerNumber,
                    //        dwPrivilege,
                    //        ref obj,
                    //        dwPassword);

                    //    object objN = new System.Runtime.InteropServices.VariantWrapper(dwName);

                    //    bool nRet = axFP_CLOCK.SetUserName(0,
                    //      id,
                    //      dwEnrollNumber,
                    //      dwEMachineNumber,
                    //      ref objN
                    //      );

                    //}

                    //if (!bRet)
                    //{
                    //    ShowErrorInfo();
                    //    //DialogResult dlgr;
                    //    //dlgr = MessageBox.Show("Continue?", "SetEnrollData", MessageBoxButtons.YesNo);

                    //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Continue?\n", false);
                    //    message.btnCancel.Visibility = Visibility.Collapsed;
                    //    message.btnOk.Content = "Ok";
                    //    message.ShowDialog();

                    //    if (message.yes)
                    //    {
                    //        bRet = true;
                    //    }
                    //    else
                    //    {
                    //        EnableDevice(id);
                    //        //labelInfo.Text = "fail on SetEnrollData";

                    //        //myAccessConn.Close();
                    //        return;
                    //    }
                    //}

                }
            });
            
            if (!String.IsNullOrEmpty(facNotUpload))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "These Faces Not Uploaded\nBad Template\n" + "EnrollNumbers " + facNotUpload, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                facNotUpload = "";
                duplEnr = 0;
            }
            await EnableDeviceAsync(id);
        }

        public bool UploadContentinloop(ref int count, ref AxFP_CLOCK axFP_CLOCK, ref bool upc, int id, bool bRet, int dwEMachineNumber,
        string dwName,
        int dwEnrollNumber,
        int dwFingerNumber,
        int dwPrivilege,
        int dwPassword,
        int[] dwFPData,
        int[] FacedwFPData,
        object obj)
        {
            int machinename = dwEMachineNumber;

            if (dwFingerNumber == 0)
            {
                obj = new System.Runtime.InteropServices.VariantWrapper(obj);
            }
            else if (dwFingerNumber == 50)
            {
                obj = new System.Runtime.InteropServices.VariantWrapper(dwFPData);
            }
            else if (dwFingerNumber > 19)
            {
                //obj = new System.Runtime.InteropServices.VariantWrapper(dr["FPData"]);
            }
            else
            {
                obj = new System.Runtime.InteropServices.VariantWrapper(dwFPData);
            }

            if (dwFingerNumber == 50)
            {
                upc = true;
                //int vPhotoSize = 0;
                //int[] indexDataFacePhoto = new int[400800];
                //分配内存
                //IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                IntPtr ptrIndexFacePhoto;
                //string path = @"c:\test.jpg";                     
                //string path = @"C:\\PHOTO\" + "D" + machinename.ToString() + "U" + dwEnrollNumber.ToString() + ".jpg"; //\" + dwEnrollNumber.ToString() + ".jpg";
                //bRet = System.IO.File.Exists(path);
                byte[] Photo = ImageOperations.GetImageDataFromDatabase(connstring, dwEnrollNumber);
                if(Photo == null)
                {
                    facNotUpload += dwEnrollNumber;
                    facNotUpload += ",";
                    return false;
                }
                int QualityThreshold = 100;
                if (Photo.Length > 400800)
                {
                    while (!(Photo.Length < 400800))
                    {
                        Photo = CompressImage(Photo, QualityThreshold);
                        QualityThreshold -= 10;
                    }
                }
                //Photo = CompressImage(Photo, 100);
                //if (bRet)
                if (Photo != null)
                {
                    //byte[] mbytCurEnrollData = System.IO.File.ReadAllBytes(path);
                    byte[] mbytCurEnrollData = Photo;
                    ptrIndexFacePhoto = Marshal.AllocHGlobal(mbytCurEnrollData.Length);
                    Marshal.Copy(mbytCurEnrollData, 0, ptrIndexFacePhoto, mbytCurEnrollData.Length);
                    bRet = axFP_CLOCK.SetEnrollPhotoCS(id, dwEnrollNumber, mbytCurEnrollData.Length, ptrIndexFacePhoto);

                    if (!bRet)
                    {
                        //if(duplEnr != dwEnrollNumber)
                        //{
                        //    duplEnr = dwEnrollNumber;
                        facNotUpload += dwEnrollNumber;
                        facNotUpload += ",";
                        //}
                    }
                    else
                    {
                        upFaceCount++;
                    }
                    object objN = new System.Runtime.InteropServices.VariantWrapper(dwName);

                    bool nRet = axFP_CLOCK.SetUserName(0,
                      id,
                      dwEnrollNumber,
                      dwEMachineNumber,
                      ref objN
                      );
                    Marshal.FreeHGlobal(ptrIndexFacePhoto);

                }
                else
                {
                    bRet = true;
                }

            }
            else
            {
                if (dwFingerNumber == 11)
                {
                    cardUpCount++;
                }
                else
                {
                    upFingCount++;
                }
                upc = true;
                bRet = axFP_CLOCK.SetEnrollData(
                    id,
                    dwEnrollNumber,
                    dwEMachineNumber,
                    dwFingerNumber,
                    dwPrivilege,
                    ref obj,
                    dwPassword);

                object objN = new System.Runtime.InteropServices.VariantWrapper(dwName);

                bool nRet = axFP_CLOCK.SetUserName(0,
                  id,
                  dwEnrollNumber,
                  dwEMachineNumber,
                  ref objN
                  );

            }

            if (!bRet)
            {
                //ShowErrorInfo();
                //DialogResult dlgr;
                //dlgr = MessageBox.Show("Continue?", "SetEnrollData", MessageBoxButtons.YesNo);

                //MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Continue?\n", false);
                //message.btnCancel.Visibility = Visibility.Collapsed;
                //message.btnOk.Content = "Ok";
                //message.ShowDialog();

                if (!bRet)//message.yes)
                {
                    bRet = true;
                }
                else
                {
                    //EnableDevice(id);
                    axFP_CLOCK.EnableDevice(id, 1);
                    //labelInfo.Text = "fail on SetEnrollData";

                    //myAccessConn.Close();
                    return false;
                }
            }


            return false;
        }

        public static byte[] CompressImage(byte[] imgToCompress, long quality)
        {
            using (var ms = new MemoryStream(imgToCompress))
            using (var srcImage = System.Drawing.Image.FromStream(ms))
            using (var compressedStream = new MemoryStream())
            {
                var jpegEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                srcImage.Save(compressedStream, jpegEncoder, encoderParams);
                return compressedStream.ToArray();
            }
        }

        // downloads data from machine and write it in the database
        //public async Task get_enroll_data(string ip, int id)
        //{
        //    LoadingWindow loader = null;
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        loader = new LoadingWindow();
        //        loader.Show();
        //    });
        //    DisableDevice(ip, id);
        //    bool bBreakFail = false;
        //    bool bRet;
        //    bRet = axFP_CLOCK.ReadAllUserID(id);
        //    if (!bRet)
        //    {
        //        ShowErrorInfo();
        //        EnableDevice(id);

        //        return;
        //    }

        //    int dwEnrollNumber = 0;
        //    int dwEnMachineID = 0;
        //    int dwBackupNum = 0;
        //    int dwPrivilegeNum = 0;
        //    int dwEnable = 0;
        //    int dwPassWord = 0;
        //    int vPhotoSize = 0;
        //    byte[] Photo = null;

        //    string dwName = "";

        //    int count = -1;
        //    string quer = "select * from tbl_enroll";
        //    //enrLst = Globals.ConvertDataTable<UdownDisp>(DBFactory.GetAllByQuery(connstring, quer));
        //    enrLst = Globals.ConvertDataTable<UdownDisp>(await DBFactory.GetAllByQueryAsync(connstring, quer));
        //    await Task.Run(async () => {
        //        do
        //        {
        //            count++;

        //            int[] dwData = new int[1420 / 4];
        //            int[] FacedwData = new int[1888 / 4];
        //            int[] indexDataFacePhoto = new int[400800];
        //            object obj = new System.Runtime.InteropServices.VariantWrapper(FacedwData);

        //            bRet = axFP_CLOCK.GetAllUserID(
        //                id,
        //                ref dwEnrollNumber,
        //                ref dwEnMachineID,
        //                ref dwBackupNum,
        //                ref dwPrivilegeNum,
        //                ref dwEnable
        //                );

        //            //read finished
        //            if (bRet == false)
        //            {
        //                //EnableDevice();
        //                bBreakFail = true;
        //                break;
        //            }

        //            if (dwBackupNum == 50)
        //            {
        //                doc = true;
        //                IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);

        //                bRet = axFP_CLOCK.GetEnrollPhotoCS(id, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
        //                if (bRet)
        //                {
        //                    byte[] mbytCurEnrollData = new byte[vPhotoSize];
        //                    Marshal.Copy(ptrIndexFacePhoto, mbytCurEnrollData, 0, vPhotoSize);
        //                    Photo = mbytCurEnrollData;
        //                    //code added by kashan
        //                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
        //                }


        //                string strName = "";
        //                object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
        //                object ob = new object();
        //                ob = strName;

        //                bool nRet = axFP_CLOCK.GetUserName(0,
        //                    dwEnMachineID,
        //                    dwEnrollNumber,
        //                    dwEnMachineID,
        //                    ref objN
        //                    );
        //                if (bRet)
        //                {
        //                    dwName = (string)objN;
        //                }
        //            }
        //            else
        //            {
        //                doc = true;
        //                bRet = axFP_CLOCK.GetEnrollData(
        //                    id,
        //                    dwEnrollNumber,
        //                    dwEnMachineID,
        //                    dwBackupNum,
        //                    ref dwPrivilegeNum,
        //                    ref obj,
        //                    ref dwPassWord);
        //            }
        //            if (!bRet)
        //            {
        //                ShowErrorInfo();


        //            }

        //            if (dwBackupNum == 50)
        //            {
        //                vPhotoSize = 0;
        //            }
        //            else
        //            {

        //            }
        //            if (dwBackupNum == 50 || dwBackupNum == 10 || dwBackupNum == 11)
        //            {
        //                string strName = "";
        //                object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
        //                object ob = new object();
        //                ob = strName;

        //                bool nRet = axFP_CLOCK.GetUserName(0,
        //                    dwEnMachineID,
        //                    dwEnrollNumber,
        //                    dwEnMachineID,
        //                    ref objN
        //                    );
        //                if (bRet)
        //                {
        //                    dwName = (string)objN;
        //                }
        //                string queri = "SELECT * FROM[AASFF].[dbo].[tbl_enroll] where EnrollNumber = '" + dwEnrollNumber + "'";
        //                sql = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData)VALUES(" + id + ",'" + dwEnrollNumber + "','" + dwBackupNum + "','" + dwPrivilegeNum + "','" + dwPassWord + "','" + dwName + "','Empty')";
        //                dwName = "";
        //            }

        //            else
        //            if (dwBackupNum == 0)
        //            {
        //                #region getname
        //                string strName = "";
        //                object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
        //                object ob = new object();
        //                ob = strName;

        //                bool nRet = axFP_CLOCK.GetUserName(0,
        //                    dwEnMachineID,
        //                    dwEnrollNumber,
        //                    dwEnMachineID,
        //                    ref objN
        //                    );
        //                if (bRet)
        //                {
        //                    dwName = (string)objN;
        //                }
        //                #endregion
        //                dwData = (int[])obj;
        //                byte[] _indexData = new byte[1420];
        //                IntPtr _ptrIndex = Marshal.AllocHGlobal(_indexData.Length);
        //                Marshal.Copy(dwData, 0, _ptrIndex, 1420 / 4);  //be careful
        //                Marshal.Copy(_ptrIndex, _indexData, 0, 1420);
        //                Marshal.FreeHGlobal(_ptrIndex);
        //                FacedwData = (int[])obj;
        //                byte[] _indexDataFace = new byte[1888];
        //                IntPtr _ptrIndexFace = Marshal.AllocHGlobal(_indexDataFace.Length);
        //                Marshal.Copy(FacedwData, 0, _ptrIndexFace, 1888 / 4);  //be careful
        //                Marshal.Copy(_ptrIndexFace, _indexDataFace, 0, 1888);
        //                Marshal.FreeHGlobal(_ptrIndexFace);
        //                string FPData = Convert.ToBase64String(_indexData);
        //                fingData = FPData;
        //                sql = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData, Photo)VALUES(" + id + ",'" + dwEnrollNumber + "','" + dwBackupNum + "','" + dwPrivilegeNum + "','" + dwPassWord + "','" + dwName + "','" + FPData + "', " + Photo + ")";

        //            }
        //            user = enrLst.Where(x => x.EnrollNumber == dwEnrollNumber).Where(x => x.FingerNumber == dwBackupNum).FirstOrDefault();

        //            if (enrLst.Contains(user))
        //            {
        //                if (user.FingerNumber == 11)
        //                {
        //                    if (!String.IsNullOrEmpty(user.DptID))
        //                    {
        //                        string updateObj = "update tbl_enroll set EMachineNumber = '" + id + "', FingerNumber = '" + dwBackupNum + "',enPassword = '" + dwPassWord + "',DptID='" + user.DptID + "' where EnrollNumber = '" + dwEnrollNumber + "' and  FingerNumber ='11'";
        //                        await DBFactory.UpdateAsync(connstring, updateObj);
        //                    }
        //                    else
        //                    {
        //                        string updateObj = "update tbl_enroll set EMachineNumber = '" + id + "', FingerNumber = '" + dwBackupNum + "',enPassword = '" + dwPassWord + "' where EnrollNumber = '" + dwEnrollNumber + "' and  FingerNumber ='11'";
        //                        await DBFactory.UpdateAsync(connstring, updateObj);
        //                    }

        //                }
        //                if (user.FingerNumber == 50)
        //                {
        //                    string updateObj = "update tbl_enroll set EMachineNumber = '" + id + "', FingerNumber = '" + dwBackupNum + "',enPassword = '" + dwPassWord + "' where EnrollNumber = '" + dwEnrollNumber + "' and  FingerNumber ='50' ";
        //                    await ImageOperations.UpdateImageAsync(connstring, dwEnrollNumber, Photo);
        //                    await DBFactory.UpdateAsync(connstring, updateObj);
        //                }
        //                if (user.FingerNumber == 0)
        //                {
        //                    string updateObj = "update tbl_enroll set EMachineNumber = '" + id + "', FingerNumber = '" + dwBackupNum + "',enPassword = '" + dwPassWord + "',FPData = '" + fingData + "' where EnrollNumber = '" + dwEnrollNumber + "' and  FingerNumber ='0' ";
        //                    await DBFactory.UpdateAsync(connstring, updateObj);
        //                }


        //                //dwName = "";
        //            }
        //            else
        //            {
        //                _ = DBFactory.InsertAsync(connstring, sql);
        //                if (Photo != null)
        //                {
        //                    _ = ImageOperations.UpdateImageAsync(connstring, dwEnrollNumber, Photo);
        //                }
        //                Photo = null;
        //                // dwName = "";
        //            }
        //            //if(enrLst.Contains())
        //            //}


        //            //garbage collection!
        //            //GC.Collect();


        //            //reset
        //            dwPassWord = 0;

        //        } while (bRet);
        //    });
        //    EnableDevice(id);
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        loader.Hide();
        //        StatusWindow.Text = count + " files Downloaded!";
        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
        //        message.btnCancel.Visibility = Visibility.Collapsed;
        //        message.btnOk.Content = "Ok";
        //        message.ShowDialog();
        //    });


        //    if (bBreakFail)
        //    {
        //        //labelInfo.Text = "Saved all Enroll Data to database...";
        //    }

        //    //conn.Close();

        //}
        public async Task get_enroll_data(string ip, int id, LoadingWindow loader)
        {
            //LoadingWindow loader = null;
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //loader = new LoadingWindow();
                loader.Show();
            });
            bool bBreakFail = false;
            int count = -1;
            await Task.Run(async () =>
            {
                //DisableDevice(ip,id); ;
                await DisableDeviceAsync(ip, id);
                bBreakFail = false;
                //bool bRet = axFP_CLOCK.ReadAllUserID(id);
                bool bRet = await ReadAllUserIDAsync(id);

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
                byte[] Photo = null;
                string dwName = "";
                

                string quer = "select * from tbl_enroll";
                var enrLst = Globals.ConvertDataTable<UdownDisp>(await DBFactory.GetAllByQueryAsync(connstring, quer));

                do
                {
                    count++;
                    int[] dwData = new int[1420 / 4];
                    int[] FacedwData = new int[1888 / 4];
                    int[] indexDataFacePhoto = new int[400800];
                    object obj = new System.Runtime.InteropServices.VariantWrapper(FacedwData);

                    //bRet = axFP_CLOCK.GetAllUserID(
                    //    id,
                    //    ref dwEnrollNumber,
                    //    ref dwEnMachineID,
                    //    ref dwBackupNum,
                    //    ref dwPrivilegeNum,
                    //    ref dwEnable
                    //);
                    UserIDResult res = await GetAllUserIDAsync(id);
                    bRet = res.Success;
                    dwEnrollNumber = res.DwEnrollNumber;
                    dwEnMachineID = res.DwEnMachineID;
                    dwBackupNum = res.DwBackupNum;
                    dwPrivilegeNum = res.DwPrivilegeNum;
                    dwEnable = res.DwEnable;

                    if (!bRet)
                    {
                        bBreakFail = true;
                        break;
                    }

                    if (dwBackupNum == 50)
                    {
                        doc = true;
                        //IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);

                        //bRet = axFP_CLOCK.GetEnrollPhotoCS(id, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
                        //if (bRet)
                        //{
                        //    Photo = new byte[vPhotoSize];
                        //    Marshal.Copy(ptrIndexFacePhoto, Photo, 0, vPhotoSize);
                        //    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                        //}
                        PhotoResult photo = await GetEnrollPhotoCSAsync(id, res.DwEnrollNumber, indexDataFacePhoto.Length);
                        if (photo.Success)
                        {
                            Photo = photo.Photo;
                        }

                        //string strName = "";
                        //object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
                        //bool nRet = axFP_CLOCK.GetUserName(0, dwEnMachineID, dwEnrollNumber, dwEnMachineID, ref objN);
                        //if (nRet)
                        //{
                        //    dwName = (string)objN;
                        //}

                        //redundant call here
                        //NameResult Name = await GetUserNameAsync(dwEnMachineID, dwEnrollNumber);
                        //if (Name.Success)
                        //{
                        //    dwName = Name.Name;
                        //}
                    }
                    else
                    {
                        doc = true;
                        //bRet = axFP_CLOCK.GetEnrollData(id, dwEnrollNumber, dwEnMachineID, dwBackupNum, ref dwPrivilegeNum, ref obj, ref dwPassWord);
                        UserIDResult data = await GetEnrollDataAsync(id, dwEnrollNumber, dwEnMachineID, dwBackupNum);
                        dwPrivilegeNum = data.DwPrivilegeNum;
                        obj = data.obj;
                        dwPassWord = data.dwPassword;
                        
                    }

                    if (!bRet)
                    {
                        ShowErrorInfo();
                    }

                    if (dwBackupNum == 50 || dwBackupNum == 10 || dwBackupNum == 11)
                    {
                        //string strName = "";
                        //object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
                        //bool nRet = axFP_CLOCK.GetUserName(0, dwEnMachineID, dwEnrollNumber, dwEnMachineID, ref objN);
                        //if (nRet)
                        //{
                        //    dwName = (string)objN;
                        //}
                        NameResult name = await GetUserNameAsync(dwEnMachineID, dwEnrollNumber);
                        if (name.Success)
                        {
                            dwName = name.Name;
                        }

                        sql = $"INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData) VALUES({id},'{dwEnrollNumber}','{dwBackupNum}','{dwPrivilegeNum}','{dwPassWord}','{dwName}','Empty')";
                        dwName = "";
                    }
                    else if (dwBackupNum == 0)
                    {
                        //string strName = "";
                        //object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
                        //bool nRet = axFP_CLOCK.GetUserName(0, dwEnMachineID, dwEnrollNumber, dwEnMachineID, ref objN);
                        //if (nRet)
                        //{
                        //    dwName = (string)objN;
                        //}
                        NameResult name = await GetUserNameAsync(dwEnMachineID, dwEnrollNumber);
                        if (name.Success)
                        {
                            dwName = name.Name;
                        }

                        dwData = (int[])obj;
                        byte[] _indexData = new byte[1420];
                        IntPtr _ptrIndex = Marshal.AllocHGlobal(_indexData.Length);
                        Marshal.Copy(dwData, 0, _ptrIndex, 1420 / 4);
                        Marshal.Copy(_ptrIndex, _indexData, 0, 1420);
                        Marshal.FreeHGlobal(_ptrIndex);

                        FacedwData = (int[])obj;
                        byte[] _indexDataFace = new byte[1888];
                        IntPtr _ptrIndexFace = Marshal.AllocHGlobal(_indexDataFace.Length);
                        Marshal.Copy(FacedwData, 0, _ptrIndexFace, 1888 / 4);
                        Marshal.Copy(_ptrIndexFace, _indexDataFace, 0, 1888);
                        Marshal.FreeHGlobal(_ptrIndexFace);

                        string FPData = Convert.ToBase64String(_indexData);
                        sql = $"INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData, Photo) VALUES({id},'{dwEnrollNumber}','{dwBackupNum}','{dwPrivilegeNum}','{dwPassWord}','{dwName}','{FPData}', {Photo})";
                    }

                    var user = enrLst.FirstOrDefault(x => x.EnrollNumber == dwEnrollNumber && x.FingerNumber == dwBackupNum);

                    if (enrLst.Contains(user))
                    {
                        string updateObj = $"update tbl_enroll set EMachineNumber = '{id}', FingerNumber = '{dwBackupNum}', enPassword = '{dwPassWord}' where EnrollNumber = '{dwEnrollNumber}' and FingerNumber ='{user.FingerNumber}'";
                        await DBFactory.UpdateAsync(connstring, updateObj);

                        if (user.FingerNumber == 50 && Photo != null)
                        {
                            await ImageOperations.UpdateImageAsync(connstring, dwEnrollNumber, Photo);
                        }
                    }
                    else
                    {
                        await DBFactory.InsertAsync(connstring, sql);
                        if (Photo != null)
                        {
                            await ImageOperations.UpdateImageAsync(connstring, dwEnrollNumber, Photo);
                        }
                        Photo = null;
                    }

                    dwPassWord = 0;
                } while (bRet);
            });

            //EnableDevice(id);
            await EnableDeviceAsync(id);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                loader.Hide();
                StatusWindow.Text = $"{count} files Downloaded!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            });

            if (bBreakFail)
            {
                //labelInfo.Text = "Saved all Enroll Data to database...";
            }
        }
        public async Task<bool> ReadAllUserIDAsync(int id)
        {
            bool result = await Task.Run(() =>
            {
                bool bRet = axFP_CLOCK.ReadAllUserID(id);
                return bRet;
            });
            return result;
        }
        //int dwEnrollNumber = 0;
        //int dwEnMachineID = 0;
        //int dwBackupNum = 0;
        //int dwPrivilegeNum = 0;
        //int dwEnable = 0;
        //int dwPassWord = 0;
        //int vPhotoSize = 0;
        //byte[] Photo = null;
        //string dwName = "";
        //int count = -1;
        public async Task<UserIDResult> GetAllUserIDAsync(int id)
        {
            return await Task.Run(() =>
            {
                var result = new UserIDResult();
                int dwEnrollNumber = 0;
                int dwEnMachineID = 0;
                int dwBackupNum = 0;
                int dwPrivilegeNum = 0;
                int dwEnable = 0;
                result.Success = axFP_CLOCK.GetAllUserID(id,
                                                         ref dwEnrollNumber,
                                                         ref dwEnMachineID,
                                                         ref dwBackupNum,
                                                         ref dwPrivilegeNum,
                                                         ref dwEnable);
                result.DwEnrollNumber = dwEnrollNumber;
                result.DwEnMachineID = dwEnMachineID;
                result.DwBackupNum = dwBackupNum;
                result.DwPrivilegeNum = dwPrivilegeNum;
                result.DwEnable = dwEnable;

                return result;
            });
        }
        public async Task<PhotoResult> GetEnrollPhotoCSAsync(int id, int dwEnrollNumber, int FacePhotoLength)
        {
            IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(FacePhotoLength);
            int vPhotoSize = 0;
            PhotoResult result = await Task.Run(() => {
                bool bRet = axFP_CLOCK.GetEnrollPhotoCS(id, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
                byte[] ImageData = null;
                PhotoResult res = new PhotoResult();
                if (bRet)
                {
                    ImageData = new byte[vPhotoSize];
                    Marshal.Copy(ptrIndexFacePhoto, ImageData, 0, vPhotoSize);
                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                    res.Photo = ImageData;
                }
                res.Success = bRet;
                return res;
            });
            return result;
        }
        public async Task<NameResult> GetUserNameAsync(int dwEnMachineID, int dwEnrollNumber)
        {
            string strName = "";
            object objN = new System.Runtime.InteropServices.VariantWrapper(strName);
            NameResult result = await Task.Run(() =>
            {
                NameResult res = new NameResult();
                bool nRet = axFP_CLOCK.GetUserName(0, dwEnMachineID, dwEnrollNumber, dwEnMachineID, ref objN);
                if (nRet)
                {
                    res.Name = (string)objN;
                }
                res.Success = nRet;
                return res;
            });
            return result;
        }
        public async Task<UserIDResult> GetEnrollDataAsync(int id, int dwEnrollNumber, int dwEnMachineID, int dwBackupNum)
        {
            int[] FacedwData = new int[1888 / 4];
            object obj = new System.Runtime.InteropServices.VariantWrapper(FacedwData);
            int dwPrivilegeNum = 0;
            int dwPassWord = 0;
            UserIDResult result = await Task.Run(() =>
            {
                UserIDResult res = new UserIDResult();
                bool bRet = axFP_CLOCK.GetEnrollData(id, dwEnrollNumber, dwEnMachineID, dwBackupNum, ref dwPrivilegeNum, ref obj, ref dwPassWord);
                if (bRet)
                {
                    res.obj = obj;
                    res.DwPrivilegeNum = dwPrivilegeNum;
                    res.dwPassword = dwPassWord;
                }
                res.Success = bRet;
                return res;
            });
            return result;
        }

        public void DeleteImageIfAlreadyExists(string ImagePath)
        {
            if (File.Exists(ImagePath))
            {
                File.Delete(ImagePath);
            }
        }
        public void get_single_data(string ip, int id, string backupnum, string enrollnum)
        {
            //listBox1.Items.Clear();

            DisableDevice(ip, id);
            bool bRet;

            int dwBackupNum = Convert.ToInt32(backupnum);
            int dwEnMachineID = Convert.ToInt32(id);
            //int dwPrivilegeNum = cmbPrivilege.SelectedIndex;
            int dwPrivilegeNum = 0;
            //need check
            int dwEnrollNumber = Convert.ToInt32(enrollnum);

            //int[] dwData = new int[1420 / 4];
            //object obj = new System.Runtime.InteropServices.VariantWrapper(dwData);

            int[] dwData = new int[1420 / 4];
            //object obj = new System.Runtime.InteropServices.VariantWrapper(dwData);
            int[] FacedwData = new int[1888 / 4];
            object obj = new System.Runtime.InteropServices.VariantWrapper(FacedwData);
            int dwPassword = 0;
            int vPhotoSize = 0;
            int[] indexDataFacePhoto = new int[400800];
            int dwCardNum = 0;
            long dwCardNum1 = 0;

            if (dwBackupNum == 50)
            {
                //分配内存
                IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                bRet = axFP_CLOCK.GetEnrollPhotoCS(dwEnMachineID, dwEnrollNumber, ref vPhotoSize, ptrIndexFacePhoto);
                if (bRet)
                {
                    byte[] mbytCurEnrollData = new byte[vPhotoSize];
                    Marshal.Copy(ptrIndexFacePhoto, mbytCurEnrollData, 0, vPhotoSize);
                    ImageOperations.UpdateImage(connstring, dwEnrollNumber, mbytCurEnrollData);
                    //System.IO.File.WriteAllBytes(@"C:\\PHOTO\" + "D" + dwEnMachineID + "U" + dwEnrollNumber.ToString() + ".jpg", mbytCurEnrollData);
                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                    doc = true;
                }
            }

            else
            {
                bRet = axFP_CLOCK.GetEnrollData(
                    dwEnMachineID,
                    dwEnrollNumber,
                    dwEnMachineID,
                    dwBackupNum,
                    ref dwPrivilegeNum,
                    ref obj,
                    ref dwPassword
                    );

                if (bRet)
                {

                    if (dwBackupNum == 11)
                    {

                        dwCardNum = dwPassword;
                        //tbCardNum.Text = dwCardNum.ToString();
                        if (dwCardNum < 0)
                        {
                            dwCardNum1 = dwCardNum + 4294967296;
                            //tbCardNum.Text = dwCardNum1.ToString();
                        }
                        string updateQuer = "update tbl_enroll set enPassword ='" + dwCardNum + "' where EnrollNumber = '" + enrollnum + "' and FingerNumber = 11 ";
                        DBFactory.Update(connstring, updateQuer);
                        doc = true;
                    }
                    else if (dwBackupNum >= 20)
                    {
                        int[] intArrar = (int[])obj;

                        int arrayLength = 1888 / 4;
                        if (arrayLength > intArrar.Length)
                        {
                            arrayLength = intArrar.Length;
                        }

                        //for (int i = 0; i < intArrar.Length; i++ )
                        for (int i = 0; i < arrayLength; i++)
                        {
                            //listBox1.Items.Add(intArrar[i].ToString("X8"));
                        }

                        FacedwData = (int[])obj;
                        byte[] _indexDataFace = new byte[1888];
                        //分配内存
                        IntPtr _ptrIndexFace = Marshal.AllocHGlobal(_indexDataFace.Length);
                        //int[]  转成 byte[]
                        Marshal.Copy(FacedwData, 0, _ptrIndexFace, 1888 / 4);  //be careful
                        Marshal.Copy(_ptrIndexFace, _indexDataFace, 0, 1888);
                        Marshal.FreeHGlobal(_ptrIndexFace);
                        gbytEnrollDataFace = _indexDataFace;   //accept byte[]
                        doc = true;
                    }
                    else
                    {
                        int[] intArrar = (int[])obj;

                        int arrayLength = 1420 / 4;
                        if (arrayLength > intArrar.Length)
                        {
                            arrayLength = intArrar.Length;
                        }

                        //for (int i = 0; i < intArrar.Length; i++ )
                        for (int i = 0; i < arrayLength; i++)
                        {
                            //listBox1.Items.Add(intArrar[i].ToString("X8"));
                        }

                        dwData = (int[])obj;
                        byte[] _indexData = new byte[1420];
                        //分配内存
                        IntPtr _ptrIndex = Marshal.AllocHGlobal(_indexData.Length);
                        //int[]  转成 byte[]
                        Marshal.Copy(dwData, 0, _ptrIndex, 1420 / 4);  //be careful
                        Marshal.Copy(_ptrIndex, _indexData, 0, 1420);
                        Marshal.FreeHGlobal(_ptrIndex);
                        gbytEnrollData = _indexData;
                        //accept byte[]
                        //
                        string fingData = Convert.ToBase64String(gbytEnrollData);
                        string updateQuer = "update tbl_enroll set FPData ='" + fingData + "' where EnrollNumber = '" + enrollnum + "'  and FingerNumber = 0";
                        DBFactory.Update(connstring, updateQuer);
                        doc = true;
                    }

                    StatusWindow.Text = " files Downloaded!";
                }
                else
                {
                    doc = false;
                    ShowErrorInfo();
                }
            }

            //conn.Close();

            EnableDevice(id);
        }

        //to set the device list
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
                ObservableCollection<DevInfo> devList = ConvertDataTableObs<DevInfo>(dt);
                foreach(DevInfo dev in devList)
                {
                    string serial = dev.Serial;
                    string hwpin = dev.HWpin;
                    if(Dev_Validation.match_serial(serial, hwpin))
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


        //to set the userdatalist 
        public void UD_List()
        {
            try
            {
                if (downuserlist.ItemsSource == null || downuserlist.Items.Count == 0)
                {
                    EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                    DataTable dt = new DataTable();
                    bool con;
                    con = DBFactory.ConnectServer(connstring);
                    if (con == true)
                    {
                        //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                        //new query
                        //
                        dt = DBFactory.GetAllByQuery(connstring, "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll");
                        //dt = DBFactory.GetAllByQuery(connstring, "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,EMachineNumber as DevID,EnrollNumber as UserID,EName as UserName,CASE when FingerNumber = 50 THEN 'Face OK' WHEN FingerNumber = 0 THEN 'FingerPrint OK' ELSE 'NO DATA' END AS Data FROM tbl_enroll");

                        EnD = ConvertDataTableObs<UdownDisp>(dt);
                        string quer = "select * from tbl_enroll where FingerNumber = 11";
                        //usersLst = Globals.ConvertDataTable<UdownDisp>(DBFactory.GetAllByQuery(connstring, quer));
                        DataTable count = DBFactory.GetAllByQuery(connstring, "select count(distinct EnrollNumber) as Count from tbl_enroll");
                        string hehe = count.Rows[0]["count"].ToString();
                        foreach (UdownDisp item in EnD)
                        {
                            if (item.FingerNumber == 11)
                                item.type = "Card";
                            else if (item.FingerNumber == 0)
                                item.type = "Finger";
                            else if (item.FingerNumber == 50)
                                item.type = "Face";
                            //dbuserlist.Items.Add(item);
                        }
                        //usCount.Content = usersLst.Count;
                        usCount.Content = hehe;

                        downuserlist.ItemsSource = EnD;
                    }
                    else
                    {
                        downuserlist.ItemsSource = null;
                    }
                }
            }
            catch (Exception ex)
            {

                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( UDList  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                MessageBox.Show(message);
                File.AppendAllText(path, message);
            }

        }


        //to set the userdata list from the database 
        public async Task DbU_List()
        {
            dbuserlist.Items.Clear();

            if (dbuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    dt = await DBFactory.GetAllByQueryAsync(connstring, "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll");
                    EnD = ConvertDataTableObs<UdownDisp>(dt);

                    //dbuserlist.ItemsSource = EnD;
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        dbuserlist.Items.Add(item);
                    }
                    //just testing this line ahead
                }
                else
                {
                    dbuserlist.ItemsSource = null;
                }
            }
        }

        public void DbU_List(string quer)
        {
            dbuserlist.Items.Clear();

            if (dbuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    // "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll"
                    dt = DBFactory.GetAllByQuery(connstring, quer);
                    EnD = ConvertDataTableObs<UdownDisp>(dt);

                    //dbuserlist.ItemsSource = EnD;
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        dbuserlist.Items.Add(item);
                    }
                    //just testing this line ahead
                }
                else
                {
                    dbuserlist.ItemsSource = null;
                }
            }
        }
        public void DelU_List()
        {

            if (deleteuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    dt = DBFactory.GetAllByQuery(connstring, "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll");
                    EnD = ConvertDataTableObs<UdownDisp>(dt);
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        //dbuserlist.Items.Add(item);
                    }
                    //just testing this line ahead
                    deleteuserlist.ItemsSource = EnD;

                }
                else
                {
                    deleteuserlist.ItemsSource = null;
                }
            }
        }

        public void DelU_List(string quer)
        {
            deleteuserlist.ItemsSource = null;
            if (deleteuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    dt = DBFactory.GetAllByQuery(connstring, quer);
                    EnD = ConvertDataTableObs<UdownDisp>(dt);
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        //dbuserlist.Items.Add(item);
                    }
                    //just testing this line ahead
                    deleteuserlist.ItemsSource = EnD;

                }
                else
                {
                    deleteuserlist.ItemsSource = null;
                }
            }
        }



        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
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
                    if (pro.Name == "ShiftId")
                    {
                        continue;
                    }
                    if (pro.Name == "DptID")
                    {
                        continue;
                    }
                    if (pro.Name == column.ColumnName)
                    {
                        if (!Convert.IsDBNull(dr[column.ColumnName]))
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                        else
                        {
                            // Assign an empty string in the DBNull case
                            pro.SetValue(obj, string.Empty, null);
                        }
                    }
                    //pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        private void devIdOnload(object sender, RoutedEventArgs e)
        {
            //load the devId here on list.
            dev_list();
            DbU_List();
            UD_List();
            DelU_List();
            UnB_List();
            BU_List();

        }

        private void checkb_Checked(object sender, RoutedEventArgs e)
        {
            int index = 0;
            index = Devlist.SelectedIndex;
        }

        private void Udownloaded(object sender, RoutedEventArgs e)
        {
            setDptCombo();
            //UD_List();
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            //FindListViewItem(downuserlist);
            if (downuserlist != null)
                CollectionViewSource.GetDefaultView(downuserlist.ItemsSource).Refresh();
        }



        private void Txt_chnged_Item_resetdown_List(object sender, TextChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(downuserlist.ItemsSource);
            view.Filter = UserFilter;


            if (downuserlist != null)
            {
                CollectionViewSource.GetDefaultView(downuserlist.ItemsSource).Refresh();
            }



            if (txt_search.Text == "")
            {
                txt_search_1.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search_1.Visibility = Visibility.Collapsed;
            }

        }

        private void Txt_chnged_ItemSo_reset(object sender, TextChangedEventArgs e)
        {
            if (downuserlist != null)
            {
                CollectionViewSource.GetDefaultView(downuserlist.ItemsSource).Refresh();
            }
            if (dbuserlist != null)
            {
                find(txt_search1.Text);

            }
            if (unbuserlist != null)
            {
                findunb(txt_searchb1.Text);
            }
            if (deleteuserlist != null)
            {
                findDelete(txt_search_forDel.Text);
                CollectionViewSource.GetDefaultView(deleteuserlist.ItemsSource).Refresh();
            }
            if (downuserlist != null)
            {
                CollectionViewSource.GetDefaultView(downuserlist.ItemsSource).Refresh();
            }
            if (txt_search_forDel.Text == "")
            {
                txt_search_forDel1.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search_forDel1.Visibility = Visibility.Collapsed;
            }

            if (txt_search1.Text == "")
            {
                txt_search11.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search11.Visibility = Visibility.Collapsed;
            }

            if (txt_search.Text == "")
            {
                txt_search_1.Visibility = Visibility.Visible;
            }
            else
            {
                txt_search_1.Visibility = Visibility.Collapsed;
            }

        }


        private void find(string str)
        {
            foreach (UdownDisp item in dbuserlist.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    dbuserlist.SelectedItems.Add(item);
                }
                else
                {
                    dbuserlist.SelectedItems.Remove(item);
                }
            }

        }


        private void findunb(string str)
        {
            foreach (UdownDisp item in unbuserlist.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    if (item.FingerNumber == 11)
                        item.type = "Card";
                    else if (item.FingerNumber == 0)
                        item.type = "Finger";
                    else if (item.FingerNumber == 50)
                        item.type = "Face";

                    unbuserlist.SelectedItems.Add(item);
                }
                else
                {
                    unbuserlist.SelectedItems.Remove(item);
                }
            }


        }
        private void findDelete(string str)
        {
            foreach (UdownDisp item in deleteuserlist.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    if (item.FingerNumber == 11)
                        item.type = "Card";
                    else if (item.FingerNumber == 0)
                        item.type = "Finger";
                    else if (item.FingerNumber == 50)
                        item.type = "Face";

                    deleteuserlist.SelectedItems.Add(item);
                }
                else
                {
                    deleteuserlist.SelectedItems.Remove(item);
                }
            }


        }

        private void select(string str)
        {
            foreach (UdownDisp item in dbuserlist.Items)
            {
                if (item.EnrollNumber.Equals(str))
                {
                    dbuserlist.SelectedItems.Add(item);
                    //item=dbuserlist.ItemsSource.OfType<UdownDisp>;
                    //item.PropertyChanged = true;
                }
                else
                {
                    //item.IsSelected = false;
                    dbuserlist.SelectedItems.Remove(item);
                }
            }
        }





        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txt_search.Text))
                return true;
            else
                return ((item as UdownDisp).EnrollNumber.ToString().IndexOf(txt_search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool UserFilterForUpload(object item)
        {
            if (String.IsNullOrEmpty(txt_search1.Text))
                return true;
            else
                return ((item as UdownDisp).EnrollNumber.ToString().IndexOf(txt_search1.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool UserFilterForDel(object item)
        {
            if (String.IsNullOrEmpty(txt_search_forDel.Text))
                return true;
            else
                return ((item as UdownDisp).EnrollNumber.ToString().IndexOf(txt_search_forDel.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void MoveAllRight(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in dbuserlist.Items)
            {

                if (item.FingerNumber == 11)
                    item.type = "Card";
                else if (item.FingerNumber == 0)
                    item.type = "Finger";
                else if (item.FingerNumber == 50)
                    item.type = "Face";
                //dbuserlist.Items.Add(item);

                selecteduserlist.Items.Add(item);
                //dbuserlist.Items.Remove(item);
            }

            dbuserlist.Items.Clear();


        }

        private void MoveSelectedRight(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in dbuserlist.SelectedItems)
            {
                //dbuserlist.Items.Remove(item);
                selecteduserlist.Items.Add(item);

            }
            for (int x = 0; x < dbuserlist.Items.Count; x++)
            {
                if (dbuserlist.SelectedItem == dbuserlist.Items[x])
                {
                    dbuserlist.Items.RemoveAt(x);
                }
                if (dbuserlist.SelectedItems.Count != 0 && x + 1 == dbuserlist.Items.Count)
                {
                    x = -1;
                }
            }

        }

        private void MoveSelectedBack(object sender, MouseButtonEventArgs e)
        {
            int index = selecteduserlist.SelectedItems.Count;
            while (index > 0)
            {
                dbuserlist.Items.Add(selecteduserlist.SelectedItems[0]);
                selecteduserlist.Items.Remove(selecteduserlist.SelectedItems[0]);

                index--;
            }
        }

        private void MoveAllBack(object sender, MouseButtonEventArgs e)
        {

            foreach (UdownDisp item in selecteduserlist.Items)
            {
                dbuserlist.Items.Add(item);

            }

            selecteduserlist.Items.Clear();
        }

        private async void Upload_to_device_Click(object sender, RoutedEventArgs e)
        {
            upc = false;
           
            U_Wait.Visibility = Visibility.Visible;


            //if (Devlist.SelectedItems.Count == 0)
            //{
            //    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Machine not selected.", false);
            //    message.btnCancel.Visibility = Visibility.Collapsed;
            //    message.btnOk.Content = "Ok";
            //    message.ShowDialog();
            //    U_Wait.Visibility = Visibility.Collapsed;
            //    return;
            //}
            if (Devlist.SelectedItems.Count == 0 && GroupList.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "No Device or Group is selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            if (selecteduserlist.Items.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "User not selected.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                U_Wait.Visibility = Visibility.Collapsed;
                return;
            }
            StatusWindow.Text = "Wait...\tUploading Files";
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            //IEnumerable items = this.Devlist.SelectedItems;
            List<gdevId> items = new List<gdevId>();
            if (Devlist.SelectedItems.Count >= 1)
            {
                foreach (gdevId item in Devlist.SelectedItems)
                {
                    items.Add(item);
                }
            }
            if (GroupList.SelectedItems.Count >= 1)
            {
                items = await FetchMachiesFromGroup(GroupList.SelectedItems);
                if (items.Count == 0)
                {
                    string txt = GroupList.SelectedItems.Count == 1 ? "group" : "groups";
                    StatusWindow.Text = $"Selected {txt} are empty!";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
            }
            LoadingWindow loader = new LoadingWindow();
            foreach (gdevId gid in items)
            {
                try
                {
                    loader.Show();
                    if (await condevAsync(gid.Ip.ToString(), gid.MId))
                    {
                        await set_enroll_data(gid.Ip.ToString(), gid.MId);

                    }
                    else
                    {
                        loader.Hide();
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();

                    }
                }
                catch (Exception ex)
                {
                    loader.Hide();
                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                    string msg = "( " + DateTime.Now + " ) ( UploadtoDevice  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                    File.AppendAllText(path, msg);
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }


            }

            U_Wait.Visibility = Visibility.Collapsed;
            loader.Hide();
            if (upc == true && faceUp.IsChecked == true)
            {
                string text = upFaceCount == 1 ? "Face Uploaded " : "Faces Uploaded ";
                StatusWindow.Text = text + upFaceCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();

            }
            if (upc == true && fingUp.IsChecked == true)
            {
                string text = upFingCount == 1 ? "Finger Uploaded " : "Fingers Uploaded ";
                StatusWindow.Text = text + upFingCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();

            }
            if (upc == true && cardUp.IsChecked == true)
            {
                string text = cardUpCount == 1 ? "Card Uploaded " : "Cards Uploaded ";
                StatusWindow.Text = text + cardUpCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            if (upc == true && faceUp.IsChecked == true && fingUp.IsChecked == true && cardUp.IsChecked == false)
            {
                int faceFingerCount = upFaceCount + upFingCount;
                //StatusWindow.Text = "Uploaded files " + selecteduserlist.Items.Count;
                StatusWindow.Text = "Face + Finger Uploaded " + faceFingerCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            if (upc == true && cardUp.IsChecked == true && fingUp.IsChecked == true && faceUp.IsChecked == false)
            {
                int cardFingerCount = cardUpCount + upFingCount;
                //StatusWindow.Text = "Uploaded files " + selecteduserlist.Items.Count;
                StatusWindow.Text = "Card + Finger Uploaded " + cardFingerCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            if (upc == true && faceUp.IsChecked == true && cardUp.IsChecked == true && fingUp.IsChecked == false)
            {
                int faceCardCount = upFaceCount + cardUpCount;
                //StatusWindow.Text = "Uploaded files " + selecteduserlist.Items.Count;
                StatusWindow.Text = "Face + Card Uploaded " + faceCardCount;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            if (upc == true && faceUp.IsChecked == true && cardUp.IsChecked == true && fingUp.IsChecked == true)
            {
                StatusWindow.Text = "Total Files Uploaded " + selecteduserlist.Items.Count;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            if (upc == false)
            {
                StatusWindow.Text = "upload failed!! " + selecteduserlist.Items.Count;
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }

            await DbU_List();
            selecteduserlist.Items.Clear();
            upFaceCount = 0;
            upFingCount = 0;
            cardUpCount = 0;


        }

        private async void Delete_from_DB_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (Devlist.SelectedItems.Count == 0 && GroupList.SelectedItems.Count == 0)
                {
                    StatusWindow.Text = "No Device or Group is selected!";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                else if (deleteuserlist.SelectedItems.Count == 0)
                {
                    StatusWindow.Text = "No record Selected!";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                StatusWindow.Text = "Wait...\tDeleting Files";
                De_Wait.Visibility = Visibility.Visible;
                MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
                messaget.ShowDialog();
                List<gdevId> items = new List<gdevId>();
                if (Devlist.SelectedItems.Count >= 1)
                {
                    foreach (gdevId item in Devlist.SelectedItems)
                    {
                        items.Add(item);
                    }
                }
                if (GroupList.SelectedItems.Count >= 1)
                {
                    items = await FetchMachiesFromGroup(GroupList.SelectedItems);
                    if (items.Count == 0)
                    {
                        string txt = GroupList.SelectedItems.Count == 1 ? "group" : "groups";
                        StatusWindow.Text = $"Selected {txt} are empty!";
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        return;
                    }
                }
                LoadingWindow loader = new LoadingWindow();
                foreach (gdevId gid in items)
                {
                    loader.Show();
                    try
                    {
                        if (await condevAsync(gid.Ip.ToString(), gid.MId))
                        {

                            //string sql;
                            int count = 0;
                            int total = deleteuserlist.SelectedItems.Count;
                            await DisableDeviceAsync(gid.Ip.ToString(), gid.MId);
                            IList<UdownDisp> selectedItems = deleteuserlist.SelectedItems.Cast<UdownDisp>().ToList();
                            count = await DeleteSelectedData(gid.MId, selectedItems);
                            //foreach (UdownDisp item in deleteuserlist.SelectedItems)
                            //{
                            //    //Deletion from Device
                            //    //int dwEnrollNum = Int32.Parse(item.EnrollNumber);
                            //    int dwEnrollNum = item.EnrollNumber;
                            //    int dwEnMachineID = gid.MId;
                            //    int dwBackupNum = Convert.ToInt32(12);


                            //    bool bRet = axFP_CLOCK.DeleteEnrollData(
                            //        gid.MId,
                            //        dwEnrollNum,
                            //        dwEnMachineID,
                            //        dwBackupNum);
                            //    if (bRet)
                            //    {
                            //        count++;
                            //        Thread.Sleep(1);
                            //        //Thread.Sleep(1);

                            //    }
                            //    else
                            //    {
                            //        ShowErrorInfo();
                            //        MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong dwEnMachineID= " + item.EMachineNumber + " & gid.MId = " + gid.MId, false);
                            //        msg.btnCancel.Visibility = Visibility.Collapsed;
                            //        msg.btnOk.Content = "Ok";
                            //        msg.ShowDialog();
                            //    }

                            //    //Deletion from DB
                            //    //sql = "delete from tbl_enroll where EnrollNumber=" + item.EnrollNumber + "  and EMachineNumber=" + gid.MId;
                            //    //DBFactory.Delete(connstring, sql);

                            //}
                            await EnableDeviceAsync(gid.MId);
                            loader.Hide();
                            StatusWindow.Text = "Record Deleted " + count + "/" + total;
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                            //deleteuserlist.ItemsSource = null;
                            //DelU_List();

                        }
                        else
                        {
                            loader.Hide();
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();

                        }
                    }
                    catch (Exception ex)
                    {
                        loader.Hide();
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }


                }


                //CollectionViewSource.GetDefaultView(deleteuserlist.ItemsSource).Refresh();
                //deleteuserlist.ItemsSource = null;
                //DelU_List();
                De_Wait.Visibility = Visibility.Collapsed;
                //CollectionViewSource.GetDefaultView(deleteuserlist.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Delete_from_DB_Click  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }

        }
        private async Task<int> DeleteSelectedData(int MId, IList<UdownDisp> selectedItems)
        {
            int deletedCount = 0;
            await Task.Run(async () => {
                foreach (UdownDisp item in selectedItems)
                {
                    //Deletion from Device
                    //int dwEnrollNum = Int32.Parse(item.EnrollNumber);
                    int dwEnrollNum = item.EnrollNumber;
                    int dwEnMachineID = MId;
                    int dwBackupNum = Convert.ToInt32(12);


                    bool bRet = axFP_CLOCK.DeleteEnrollData(
                        MId,
                        dwEnrollNum,
                        dwEnMachineID,
                        dwBackupNum);
                    if (bRet)
                    {
                        deletedCount++;
                        Thread.Sleep(1);
                        //Thread.Sleep(1);

                    }
                    else
                    {
                        await ShowErrorInfoAsync();
                        await Application.Current.Dispatcher.InvokeAsync(() => {
                            MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong dwEnMachineID= " + item.EMachineNumber + " & gid.MId = " + MId, false);
                            msg.btnCancel.Visibility = Visibility.Collapsed;
                            msg.btnOk.Content = "Ok";
                            msg.ShowDialog();
                        });
                        
                    }

                    //Deletion from DB
                    //sql = "delete from tbl_enroll where EnrollNumber=" + item.EnrollNumber + "  and EMachineNumber=" + gid.MId;
                    //DBFactory.Delete(connstring, sql);

                }
            });
            return deletedCount;
        }
        private void txt_search_forDel1_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search_forDel1.Visibility = Visibility.Collapsed;
        }

        private void txt_search11_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search11.Visibility = Visibility.Collapsed;
        }

        private void txt_search_1_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search_1.Visibility = Visibility.Collapsed;

        }

        private void reload_del_list(object sender, MouseButtonEventArgs e)
        {
            deleteuserlist.ItemsSource = null;
            DelU_List();
            usersCount.Visibility = Visibility.Collapsed;
        }

        private void reload_upload_list(object sender, MouseButtonEventArgs e)
        {
            // unbuserlist.Items.Clear();//selecteduserlist.Items.Clear();
            //DbU_List();
            usersCount.Visibility = Visibility.Collapsed;
        }

        private void rel_up_list(object sender, MouseButtonEventArgs e)
        {
            DbU_List();
            //selecteduserlist.Items.Clear();
        }

        private void UnBlockuserMaster(object sender, MouseButtonEventArgs e)
        {
            StatusWindow.Text = "Wait...Un-Blocking Users";
            U_Waitb.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            IEnumerable items = this.Devlist.SelectedItems;
            if (Devlist.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "Device not selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            else if (deleteuserlist.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "No record Selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }

            foreach (gdevId gid in items)
            {
                try
                {
                    if (condev(gid.Ip.ToString(), gid.MId))
                    {

                        string sql;
                        int count = 0;
                        int total = buserlist.SelectedItems.Count;
                        foreach (UdownDisp item in buserlist.SelectedItems)
                        {
                            List<UdownDisp> selectedList = Globals.ConvertDataTable<UdownDisp>(DBFactory.GetAllByQuery(connstring, "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_blockedUsers where EnrollNumber = '" + item.EnrollNumber + "'"));
                            foreach (var blocUser in selectedList)
                            {
                                int dwEnrollNum = blocUser.EnrollNumber;
                                int dwEnMachineID = blocUser.EMachineNumber;//gid.MId;
                                int dwBackupNum = Convert.ToInt32(12);

                                DisableDevice(gid.Ip.ToString(), gid.MId);

                                //bool bRet = axFP_CLOCK.DeleteEnrollData(
                                //    gid.MId,
                                //    dwEnrollNum,
                                //    dwEnMachineID,
                                //    dwBackupNum);
                                bool bRet = axFP_CLOCK.EnableUser(
                                    gid.MId,
                                    dwEnrollNum,
                                    dwEnMachineID,
                                    dwBackupNum,
                                    1);

                                if (bRet)
                                {
                                    count++;
                                    StatusWindow.Text = "User Unblocked " + total;
                                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                                    message.btnCancel.Visibility = Visibility.Collapsed;
                                    message.btnOk.Content = "Ok";
                                    message.ShowDialog();
                                }
                                else
                                {
                                    ShowErrorInfo();
                                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong dwEnMachineID= " + item.EMachineNumber + " & gid.MId = " + gid.MId, false);
                                    message.btnCancel.Visibility = Visibility.Collapsed;
                                    message.btnOk.Content = "Ok";
                                    message.ShowDialog();
                                }
                                EnableDevice(gid.MId);

                                //Deletion from table tbl_blockedUser in AASFF DB
                                sql = "delete from tbl_blockedUsers where EnrollNumber=" + item.EnrollNumber + "  and EMachineNumber=" + dwEnMachineID;//gid.MId;
                                DBFactory.Delete(connstring, sql);

                                sql = "INSERT INTO [dbo].[tbl_unblockedUsers] (EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName)VALUES(" + dwEnMachineID + ",'" + item.EnrollNumber + "','" + dwBackupNum + "','" + item.Privilige + "','" + item.enPassword + "','" + item.EName + "')";
                                DBFactory.Insert(connstring, sql);
                            }

                        }
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();

                    }
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            buserlist.SelectedItems.Clear();
            UnB_List();
            BU_List();
            U_Waitb.Visibility = Visibility.Collapsed;
        }

        private void BlockUserMaster(object sender, MouseButtonEventArgs e)
        {
            StatusWindow.Text = "Wait...Blocking Users";
            U_Waitb.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            IEnumerable items = this.Devlist.SelectedItems;
            if (Devlist.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "Device not selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            else if (deleteuserlist.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "No record Selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }

            foreach (gdevId gid in items)
            {
                try
                {
                    if (condev(gid.Ip.ToString(), gid.MId))
                    {
                        string sql;
                        int count = 0;
                        int total = unbuserlist.SelectedItems.Count;
                        foreach (UdownDisp item in unbuserlist.SelectedItems)
                        {
                            List<UdownDisp> selectedList = Globals.ConvertDataTable<UdownDisp>(DBFactory.GetAllByQuery(connstring, "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_unblockedUsers where EnrollNumber = '" + item.EnrollNumber + "'"));
                            foreach (var unbUser in selectedList)
                            {
                                int dwEnrollNum = unbUser.EnrollNumber;
                                int dwEnMachineID = unbUser.EMachineNumber;//gid.MId;
                                int dwBackupNum = Convert.ToInt32(12);

                                DisableDevice(gid.Ip.ToString(), gid.MId);

                                //bool bRet = axFP_CLOCK.DeleteEnrollData(
                                //    gid.MId,
                                //    dwEnrollNum,
                                //    dwEnMachineID,
                                //    dwBackupNum);
                                bool bRet = axFP_CLOCK.EnableUser(
                                    gid.MId,
                                    dwEnrollNum,
                                    dwEnMachineID,
                                    dwBackupNum,
                                    0);

                                if (bRet)
                                {
                                    count++;
                                    StatusWindow.Text = "User Blocked " + total;
                                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                                    message.btnCancel.Visibility = Visibility.Collapsed;
                                    message.btnOk.Content = "Ok";
                                    message.ShowDialog();
                                }
                                else
                                {
                                    ShowErrorInfo();
                                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Wrong dwEnMachineID= " + item.EMachineNumber + " & gid.MId = " + gid.MId, false);
                                    message.btnCancel.Visibility = Visibility.Collapsed;
                                    message.btnOk.Content = "Ok";
                                    message.ShowDialog();
                                }
                                EnableDevice(gid.MId);

                                //Deletion from table tbl_blockedUser in AASFF DB
                                sql = "delete from tbl_unblockedUsers where EnrollNumber=" + item.EnrollNumber + "  and EMachineNumber=" + dwEnMachineID;
                                DBFactory.Delete(connstring, sql);

                                sql = "INSERT INTO [dbo].[tbl_blockedUsers] (EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName)VALUES(" + dwEnMachineID + ",'" + item.EnrollNumber + "','" + dwBackupNum + "','" + item.Privilige + "','" + item.enPassword + "','" + item.EName + "')";
                                DBFactory.Insert(connstring, sql);
                            }
                        }
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();

                    }
                }
                catch (Exception ex)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
            }
            unbuserlist.SelectedItems.Clear();

            UnB_List();
            BU_List();
            U_Waitb.Visibility = Visibility.Collapsed;
        }

        public void UnB_List()
        {
            unbuserlist.Items.Clear();

            if (unbuserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll");
                    dt = DBFactory.GetAllByQuery(connstring, "SELECT distinct ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,EMachineNumber,EnrollNumber,EName FROM tbl_unblockedUsers group by EnrollNumber,EName,EMachineNumber");
                    EnD = ConvertDataTableObs<UdownDisp>(dt);
                    //dbuserlist.ItemsSource = EnD;
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        //dbuserlist.Items.Add(item);
                        unbuserlist.Items.Add(item);

                    }
                    //just testing this line ahead
                }
                else
                {
                    unbuserlist.ItemsSource = null;
                }
            }
        }

        public void BU_List()
        {
            buserlist.Items.Clear();

            if (buserlist.ItemsSource == null)
            {
                EnD = new ObservableCollection<UdownDisp>();//List<gdevId>();
                DataTable dt = new DataTable();
                bool con;
                con = DBFactory.ConnectServer(connstring);
                if (con == true)
                {
                    //dt = DBFactory.GetAllByQuery(connstring, "SELECT  * from tblEnroll");
                    dt = DBFactory.GetAllByQuery(connstring, "SELECT distinct ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,EMachineNumber,EnrollNumber,EName FROM tbl_blockedUsers group by EnrollNumber,EName,EMachineNumber");
                    EnD = ConvertDataTableObs<UdownDisp>(dt);
                    //dbuserlist.ItemsSource = EnD;
                    foreach (UdownDisp item in EnD)
                    {
                        if (item.FingerNumber == 11)
                            item.type = "Card";
                        else if (item.FingerNumber == 0)
                            item.type = "Finger";
                        else if (item.FingerNumber == 50)
                            item.type = "Face";
                        buserlist.Items.Add(item);
                    }
                    //just testing this line ahead
                }
                else
                {
                    buserlist.ItemsSource = null;
                }
            }
        }

        private void txt_searchublist_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_searchub.Visibility = Visibility.Collapsed;
        }

        private void btnDownUser(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_search.Text))
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Enroll No", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                Keyboard.Focus(txt_search);
                return;
            }
            if (Devlist.SelectedItems.Count == 0)
            {
                StatusWindow.Text = "Device not selected!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            StatusWindow.Text = "Wait...\tDownloading Files";
            D_Wait.Visibility = Visibility.Visible;
            MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            messaget.ShowDialog();
            IEnumerable items = this.Devlist.SelectedItems;
            foreach (gdevId gid in items)
            {
                try
                {
                    if (condev(gid.Ip.ToString(), gid.MId))
                    {
                        if (checkFace.IsChecked == true)
                        {
                            get_single_data(gid.Ip.ToString(), gid.MId, "50", txt_search.Text);
                            StatusWindow.Text = "Downloaded";

                        }
                        if (checkFing.IsChecked == true)
                        {
                            get_single_data(gid.Ip.ToString(), gid.MId, "0", txt_search.Text);
                            StatusWindow.Text = "Downloaded";

                        }
                        if (checkCard.IsChecked == true)
                        {
                            get_single_data(gid.Ip.ToString(), gid.MId, "11", txt_search.Text);
                            StatusWindow.Text = "Downloaded";
                        }
                        //string sqlforBU = "INSERT INTO tbl_unblockedUsers SELECT [EMachineNumber], [EnrollNumber] ,[FingerNumber] ,[Privilige] ,[enPassword] ,[EName] FROM tbl_enroll WHERE NOT EXISTS (SELECT * FROM tbl_blockedUsers WHERE tbl_blockedUsers.EMachineNumber = tbl_enroll.EMachineNumber  and tbl_blockedUsers.EnrollNumber = tbl_enroll.EnrollNumber)";
                        //DBFactory.Insert(connstring, sqlforBU);
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "IP = " + gid.Ip.ToString() + " is not active!", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();

                    }
                    if (doc == false)
                    {
                        StatusWindow.Text = "Download Failed!";
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }
                    doc = false;
                }
                catch (Exception ex)
                {

                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                    string msg = "( " + DateTime.Now + " ) ( DownloadData  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                    File.AppendAllText(path, msg);
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Confirm, "Exception Occured!\n" + ex.Message, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }


            }


            downuserlist.ItemsSource = null;
            UD_List();
            D_Wait.Visibility = Visibility.Collapsed;
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            //usersCount.Visibility = Visibility.Visible;
        }

        private void tabItemUpload(object sender, RoutedEventArgs e)
        {
            //usersCount.Visibility = Visibility.Visible;
        }

        private void delTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            //usersCount.Visibility = Visibility.Collapsed;
        }

        private void blocTabItem_loaded(object sender, RoutedEventArgs e)
        {
            //usersCount.Visibility = Visibility.Collapsed;
        }

        private void btn_checkAll(object sender, RoutedEventArgs e)
        {
            count = Devlist.SelectedItems.Count;
            if (count == 0)
            {
                Devlist.SelectAll();
                mchCh.Content = "UNCHECK ALL";
            }
            if (count > 0)
            {
                mchCh.Content = "Check All";
                Devlist.UnselectAll();
            }

        }

        private void buserlist_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void del_rel_list(object sender, MouseButtonEventArgs e)
        {
            deleteuserlist.ItemsSource = null;
            DelU_List();
        }

        private void down_rel_list(object sender, MouseButtonEventArgs e)
        {
            UD_List();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            dptCombo.Text = "---Select Dpt---";
            dptCombo.IsEnabled = true;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            dptCombo.IsEnabled = false;
            dptCombo.Text = "---Select Dpt---";
            dptID = "";
        }

        private void dptComb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dptComb.SelectedIndex != -1)
            {
                dptID = dptTable.Rows[dptComb.SelectedIndex].ItemArray[1].ToString();
                if (!String.IsNullOrEmpty(dptID))
                {
                    string dptStaffQuer = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where DptID = '" + dptID + "'";
                    DelU_List(dptStaffQuer);
                }

            }
            //"SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll"
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            dptComb.Text = "---Select Dpt---";
            dptComb.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            dptComb.IsEnabled = false;
            dptID = "";
        }

        private void selDelUsers_Checked(object sender, RoutedEventArgs e)
        {
            deleteuserlist.SelectAll();
        }

        private void selDelUsers_Unchecked(object sender, RoutedEventArgs e)
        {
            deleteuserlist.UnselectAll();
        }

        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GroupList.SelectedItems.Count >= 1)
            {
                Devlist.SelectedItems.Clear();
                mchCh.Content = "Check All";
            }
        }

        private void Devlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Devlist.SelectedItems.Count >= 1)
            {
                GroupList.SelectedItems.Clear();
                grpCh.Content = "Check All";
            }
        }

        private void GroupList_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGroupList(); ;
        }
        public void LoadGroupList()
        {
            DataTable dt = new DataTable();
            bool con;
            con = DBFactory.ConnectServer(connstring);
            if (con)
            {
                dt = DBFactory.GetAllByQuery(connstring, "Select * from tbl_DevGroups Order by ID ASC");
                DevGroupList = ConvertDataTableObs<tbl_DevGroups>(dt);
                GroupList.ItemsSource = DevGroupList;
            }
        }

        private void grpCh_Click(object sender, RoutedEventArgs e)
        {
            count = GroupList.SelectedItems.Count;
            if (count == 0)
            {
                GroupList.SelectAll();
                grpCh.Content = "UNCHECK ALL";
            }
            if (count > 0)
            {
                grpCh.Content = "CHECK ALL";
                GroupList.UnselectAll();
            }
        }

        private void dptCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dptCombo.SelectedIndex != -1)
            {
                dptID = dptTable.Rows[dptCombo.SelectedIndex].ItemArray[0].ToString();
                if(!String.IsNullOrEmpty(dptID))
                {
                    string dptStaffQuer = "SELECT distinct EnrollNumber, ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where DptID = '"+dptID+"'"; 
                    DbU_List(dptStaffQuer);
                }

            }
        }
        public void setDptCombo()
        {
            dptTable = DBFactory.GetAllByQuery(connstring, "SELECT * FROM tbl_department");
            dptNames = Globals.ConvertDataTable<Departments>(dptTable).ToList();
            dptCombo.ItemsSource = dptNames.Select(x => x.dept_name);
            dptComb.ItemsSource = dptNames.Select(x => x.dept_name);
        }
    }
}

public class UserIDResult
{
    public int DwEnrollNumber { get; set; }
    public int DwEnMachineID { get; set; }
    public int DwBackupNum { get; set; }
    public int DwPrivilegeNum { get; set; }
    public int DwEnable { get; set; }
    public object obj { get; set; }
    public int dwPassword { get; set; }
    public bool Success { get; set; }
}
public class PhotoResult
{
    public byte[] Photo { get; set; }
    public bool Success { get; set; }
}
public class NameResult
{
    public string Name { get; set; }
    public bool Success { get; set; }
}
