using FaceAttendance.Classes;
using HIKVISION;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UsmanCodeBlocks.Data.Sql.Local;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucAddStaff.xaml
    /// </summary>
    public partial class ucAddStaff : UserControl
    {
        public int FingerNumber;
        public int Privilige;
        public Bitmap UserImage;
        string dptID = "0";
        public int enPassword;
        List<tbl_cadets> cadets;
        List<Departments> dptNames;
        DataTable dptTable;
        userS UserToUpdate;
        public string txtEditor;
        public bool RetakeClicked;
        public bool ClearImageClicked;
        public string importPath;
        public string UserImagePath;
        public bool BtnModifiedClicked;
        string ConnString;
        int ED = 0;
        public int MchNo;
        
        public List<userS> UserIDFingerList = new List<userS>();
        
        DataTable staffTab;
        ObservableCollection<userS> EnD;
        ObservableCollection<userS> EnD_Backup;
        ObservableCollection<userS> impList;
        //List<UdownDisp> impList;
        List<userS> usersList;
        userS checkUser;
        //private StorageFilestoreFile;  
        //private IRandomAccessStream stream;

        public ucAddStaff(string connstring)
        {
            ConnString = connstring;

            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor = File.ReadAllText(openFileDialog.FileName);
                //txtPath.Text = openFileDialog.FileName;
                // txtPath1.Visibility = Visibility.Collapsed;

                System.Drawing.Image i = System.Drawing.Image.FromFile(openFileDialog.FileName);

                System.Drawing.Image img1 = i.GetThumbnailImage(100, 100, null, new IntPtr());
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                try
                {
                    if (img1 != null)
                    {

                        bitmap.BeginInit();
                        System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                        img1.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        bitmap.StreamSource = memoryStream;
                        bitmap.EndInit();
                    }
                }
                catch
                {

                }

                //uImg.Source = bitmap;
            }
        }

        private void EnableUserControlsAddButton(object sender, MouseButtonEventArgs e)
        {
            if (newenrolllist.SelectedItems.Count > 0)
            {
                newenrolllist.SelectedItems.Clear();
            }
            if (BtnModifiedClicked)
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you don't want to save the changes.", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                reset_controls();
                EnableControlsforAdd();
                BtnModifiedClicked = false;
                btnEnroll.Content = "Add User";
                txtUID.Focus();
            }
            else
            {
                btnEnroll.Content = "Add User";
                EnableControlsforAdd();
                txtUID.Focus();

            }


        }

        private void EnableControlsforAdd()
        {

            txtCID.IsReadOnly = false;
            //txtDevID.IsReadOnly = false;
            //txtPassword.IsReadOnly = false;
            txtUID.IsReadOnly = false;
            txtUname.IsReadOnly = false;
            btnOpenFile.IsEnabled = true;
            btnEnroll.IsEnabled = true;
            Privillige.IsEnabled = true;
            Dept.IsEnabled = true;
            ocrCapBtn.IsEnabled = true;


            txtCID1.Visibility = Visibility.Collapsed;
            //txtDevID1.Visibility = Visibility.Collapsed;
            //txtPassword1.Visibility = Visibility.Collapsed;
            txtUID1.Visibility = Visibility.Collapsed;
            txtUname1.Visibility = Visibility.Collapsed;
            //txtPath1.Visibility = Visibility.Collapsed;
            BtnModifiedClicked = false;
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
                    if (pro.Name == "ShiftId" && dr[column.ColumnName] == DBNull.Value)
                    {
                        continue;
                    }
                    if (pro.Name == "DptID" && dr[column.ColumnName] == DBNull.Value)
                    {
                        continue;
                    }
                    if (pro.Name == column.ColumnName)
                    {
                        bool isInt32 = pro.PropertyType.FullName == "System.Int32" || pro.PropertyType.FullName == "System.Int64";
                        switch (isInt32)
                        {
                            case true:
                                pro.SetValue(obj, dr[column.ColumnName], null);
                                break;
                            case false:
                                pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                                break;

                        }
                        //if (pro.PropertyType.FullName == "System.Int32")
                        //{

                        //}
                        //pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        private void btnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region btnImportExcel
                //newenrolllist.Items.Clear();
                string filename;
                string ext;
                string directoryname;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;";
                Co_Wait.Visibility = Visibility.Visible;
                if (openFileDialog.ShowDialog() == true)
                {
                    txtEditor = File.ReadAllText(openFileDialog.FileName);
                    importPath = openFileDialog.FileName;
                    filename = System.IO.Path.GetFileName(importPath);
                    ext = System.IO.Path.GetExtension(importPath);
                    directoryname = System.IO.Path.GetDirectoryName(importPath);
                    DataTable dt = ExcelFunc.getExcelFile(importPath);
                    if (dt.Rows.Count > 0)
                    {
                        //File.Delete(openFileDialog.FileName);
                        string query;
                        //impList = Globals.ConvertDataTable<UdownDisp>(dt);
                        //impList = ConvertDataTableObs<UdownDisp>(dt);
                        impList = ConvertDataTableObs<userS>(dt);
                        string users = "select * from tbl_enroll where FingerNumber = '11' order by EnrollNumber asc";
                        // DataTable dataTable = DBFactory.GetAllByQuery(ConnString, users);
                        usersList = Globals.ConvertDataTable<userS>(DBFactory.GetAllByQuery(ConnString, users));
                        //usersList = ConvertDataTableObs<userS>(DBFactory.GetAllByQuery(ConnString, users));

                        ////////////////foreach (userS item in impList)
                        ////////////////{
                        ////////////////    {
                        ////////////////        //item.SrNo = 0;
                        ////////////////        //newenrolllist.Items.Add(item);
                        ////////////////        EnD.Add(item);

                        ////////////////    }
                        ////////////////}
                        foreach (DataRow dr in dt.Rows)
                        {
                            checkUser = usersList.Where(x => x.EnrollNumber == Convert.ToInt32(dr.ItemArray[1])).Where(x => x.FingerNumber == Convert.ToInt32(dr.ItemArray[2])).FirstOrDefault();
                            if (!usersList.Contains(checkUser))
                            {
                                if (false) { 
                                if (String.IsNullOrEmpty(dr.ItemArray[5].ToString()))
                                {
                                    //query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "'," + dr.ItemArray[6].ToString() + ")";
                                    //DBFactory.Insert(ConnString, query);
                                    string photoname = "D" + dr.ItemArray[0].ToString() + "U" + dr.ItemArray[1].ToString();
                                    copy(directoryname + "\\PHOTO", photoname, photoname);

                                    //newenrolllist.Items.Add(dr);
                                }
                                }
                                else
                                {
                                    string DepartmentID = dr.ItemArray[7].ToString() == "Null" ? "" : dr.ItemArray[7].ToString();

                                    query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData,DptID)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + dr.ItemArray[5].ToString() + "','Empty','" + DepartmentID + "')";
                                    DBFactory.Insert(ConnString, query);
                                    string photoname = "D" + dr.ItemArray[0].ToString() + "U" + dr.ItemArray[1].ToString();
                                    if (!String.IsNullOrEmpty(photoname))
                                    {
                                        if (Directory.Exists(photoname))
                                        {
                                            copy(directoryname + "\\PHOTO", photoname, photoname);
                                        }
                                    }




                                    // newenrolllist.Items.Add(dr);
                                }
                            }
                            else
                            {

                            }






                            // string al1 = dr.ItemArray[5].ToString();
                        }




                        //if (dr.ItemArray[5].ToString() == "" && dr.ItemArray[6].ToString() == "NULL")
                        //    {
                        //        query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "',NULL)";
                        //        DBFactory.Insert(ConnString, query);
                        //    }
                        //if (dr.ItemArray[5].ToString() == "" && dr.ItemArray[6].ToString() != "NULL")
                        //{
                        //    query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "',"+ dr.ItemArray[6].ToString() + ")";
                        //    DBFactory.Insert(ConnString, query);
                        //}
                        //if (dr.ItemArray[5].ToString() != "" && dr.ItemArray[6].ToString() != "NULL")
                        //{
                        //    query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "'," + dr.ItemArray[6].ToString() + ")";
                        //    DBFactory.Insert(ConnString, query);
                        //}










                        //for(int i = 0; i < dt.Rows.Count; i++)
                        //{

                        //}

                        string value = dt.Rows[0].ItemArray[0].ToString();
                        Co_Wait.Visibility = Visibility.Collapsed;
                        StatusWindow.Foreground = System.Windows.Media.Brushes.Green;
                        StatusWindow.Text = "Users Imported Succesfully";
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        getEnrollList();
                        Thread clearStatus = new Thread(ClearStatusWindows);
                        clearStatus.Start();



                    }
                    else
                    {
                        statusbar_red();
                        Co_Wait.Visibility = Visibility.Collapsed;
                    }
                    #endregion
                    //directoryname = System.IO.Path.GetDirectoryName(txtPath.Text);
                    //txtPath.Text = openFileDialog.FileName;
                    //txtPath1.Visibility = Visibility.Collapsed;
                    System.Drawing.Image i = System.Drawing.Image.FromFile(openFileDialog.FileName);
                    System.Drawing.Image img1 = i.GetThumbnailImage(100, 100, null, new IntPtr());
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    try
                    {
                        if (img1 != null)
                        {

                            bitmap.BeginInit();
                            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                            img1.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                            bitmap.StreamSource = memoryStream;
                            bitmap.EndInit();
                        }
                    }
                    catch
                    {

                    }

                    //uImg.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                if(ex.GetType().ToString() == "System.IO.IOException")
                {
                    MessageBoxWindow exception = new MessageBoxWindow(MessageBoxType.Info, ex.Message, false);
                    exception.btnCancel.Visibility = Visibility.Collapsed;
                    exception.btnOk.Content = "Ok";
                    exception.ShowDialog();
                }
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( ImportUsers  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }
        }
        public void statusbar_green()
        {
            StatusWindow.Text = "Task Completed";
            StatusWindow.FontSize = 12;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Green);
        }
        public void statusbar_red()
        {
            StatusWindow.Text = "Task Not Completed";
            StatusWindow.FontSize = 12;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Red);
        }

        public void statusbar_yellow()
        {
            StatusWindow.Text = "Wait! In Progress... ";
            StatusWindow.FontSize = 12;
            StatusWindow.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        private void copy(string sourcePath, string fileName, string newfileName)
        {
            //string fileName = "test.txt";
            //string sourcePath = @"C:\Users\Public\TestFolder";
            string targetPath = @"C:\PHOTO";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, newfileName);

            // To copy a folder's contents to a new location
            // Create a new target folder.
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            // To copy a file to another location and
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            //if (System.IO.Directory.Exists(sourcePath))
            //{
            //    string[] files = System.IO.Directory.GetFiles(sourcePath);

            //    // Copy the files and overwrite destination files if they already exist.
            //    foreach (string s in files)
            //    {
            //        // Use static Path methods to extract only the file name from the path.
            //        fileName = System.IO.Path.GetFileName(s);
            //        destFile = System.IO.Path.Combine(targetPath, fileName);
            //        System.IO.File.Copy(s, destFile, true);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Source path does not exist!");
            //}

            //// Keep console window open in debug mode.
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }

        private void btnEnroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnrollFunc())
                {
                    string StatusWindowInfo = BtnModifiedClicked ? "User Updated" : "User Enrolled";
                    string MessageInfo = BtnModifiedClicked ? "User Updated Successfully" : "User Enrolled Successfully";
                    StatusWindow.Text = StatusWindowInfo;
                    StatusWindow.Foreground = System.Windows.Media.Brushes.Green;
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, MessageInfo, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    btnEnroll.Content = "Add User";
                    Thread thread = new Thread(ClearStatusWindows);
                    thread.Start();
                    getEnrollList();
                    RetakeClicked = false;
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string message = "( " + DateTime.Now + " ) ( Enroll User  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, message);
            }

        }

        private bool EnrollFunc()
        {
            try
            {
                string query;
                int admin = 0;
                int User_ID;
                int deviceID = 0;
                string UserName;
                int cardNo = 0;
                
                if (string.IsNullOrEmpty(txtUID.Text) || !int.TryParse(txtUID.Text, out User_ID))
                {
                    if (string.IsNullOrEmpty(txtUID.Text))
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please enter User ID.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;

                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "You can only enter numerics in User ID.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(txtUname.Text))
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please enter user name.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    _ = message.ShowDialog();
                    return false;
                }
                else
                {
                    UserName = txtUname.Text;
                }
                if ((string.IsNullOrEmpty(txtCID.Text) || !int.TryParse(txtCID.Text, out cardNo)) && txtCID.IsReadOnly == false)
                {
                    if (string.IsNullOrEmpty(txtCID.Text))
                    {
                        cardNo = 0;
                    }
                    else
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "You can only enter numerics in RFID.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;
                    }
                }
                if (txtCID.IsReadOnly && BtnModifiedClicked)
                {
                    cardNo = 0;
                }
                
                if (Privillige.IsChecked == true)
                {
                    admin = 1;
                }
                else
                {
                    admin = 0;
                }
                
                if (txtUID.Text != "" && txtUname.Text != "")
                {
                    
                    int card = 0;
                    int any = 0;
                    int face = 0;
                    int duplicate = 0;
                    userS AlreadyEnrolledCard = null;
                    userS AlreadyEnrolledAny = null;
                    userS AlreadyEnrolledFace = null;
                    userS Duplicate = null;
                    foreach (userS user in UserIDFingerList)
                    {
                        if(user.EnrollNumber == User_ID && user.FingerNumber == 11 && user.EName == UserName && card == 0)
                        {
                            AlreadyEnrolledCard = new userS(user);
                            card = 1;
                        }
                        if(user.EnrollNumber == User_ID && (user.FingerNumber == 50 || user.FingerNumber == 0 || user.FingerNumber == 10) && user.EName == UserName && any == 0)
                        {
                            AlreadyEnrolledAny = new userS(user);
                            any = 1;
                        }
                        if(user.EnrollNumber == User_ID && user.FingerNumber == 50 && user.EName == UserName && face == 0)
                        {
                            AlreadyEnrolledFace = new userS(user);
                            face = 1;
                        }
                        if(user.EnrollNumber == User_ID && (user.FingerNumber == 50 || user.FingerNumber == 0 || user.FingerNumber == 10 || user.FingerNumber == 11) && user.EName != UserName && duplicate == 0)
                        {
                            Duplicate = new userS(user);
                            duplicate = 1;
                        }
                    }
                    //userS AlreadyEnrolledCard = UserIDFingerList.FirstOrDefault(user => user.EnrollNumber == User_ID && user.FingerNumber == 11 && user.EName == UserName);
                    //userS AlreadyEnrolledAny = UserIDFingerList.FirstOrDefault(user => user.EnrollNumber == User_ID && (user.FingerNumber == 50 || user.FingerNumber == 0 || user.FingerNumber == 10) && user.EName == UserName);
                    //userS Duplicate = UserIDFingerList.FirstOrDefault(user => user.EnrollNumber == User_ID && (user.FingerNumber == 11 || user.FingerNumber == 50) && user.EName != UserName);
                    if ((AlreadyEnrolledAny != null) && (AlreadyEnrolledAny.DptID != null || AlreadyEnrolledAny.DptID != "0") && !BtnModifiedClicked)
                    {
                        dptID = AlreadyEnrolledAny.DptID;
                    }
                    if(AlreadyEnrolledCard != null && AlreadyEnrolledFace != null && !BtnModifiedClicked)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "User's Card and Face is already enrolled.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;

                    }
                    if (AlreadyEnrolledCard != null && !BtnModifiedClicked && clearImage.IsEnabled == false)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "User's Card is already enrolled.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;

                    }
                    if(AlreadyEnrolledFace != null && !BtnModifiedClicked && clearImage.IsEnabled)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "User's Face is already enrolled.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;

                    }
                    if (AlreadyEnrolledCard == null && Duplicate != null && !BtnModifiedClicked)
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Another user with the same ID is already enrolled.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        _ = message.ShowDialog();
                        return false;
                    }
                    if (AlreadyEnrolledAny != null && !BtnModifiedClicked)
                    {
                        admin = AlreadyEnrolledAny.Privilige;
                        deviceID = AlreadyEnrolledAny.EMachineNumber;
                        dptID = AlreadyEnrolledAny.DptID;
                    }
                    bool updated = false;
                    if (BtnModifiedClicked)
                    {
                        if(txtCID.IsReadOnly == true)//mtlb kay sirf card nahi baqi sab hai 
                        
                        {
                            if (UserToUpdate.FingerNumber == 50)
                            {
                                if (clearImage.IsEnabled && RetakeClicked && !ClearImageClicked)
                                {

                                    //UserImagePath = @"C:\\PHOTO\" + "D" + deviceID + "U" + User_ID + ".jpg";
                                    //resetImage();
                                    //Bitmap copy = (Bitmap)UserImage.Clone();
                                    Bitmap copy = new Bitmap(UserImage);
                                    
                                    //System.GC.Collect();
                                    //System.GC.WaitForPendingFinalizers();

                                    //var directoryInfo = new DirectoryInfo(@"C:\\PHOTO");
                                    //var fileInfo = directoryInfo.GetFiles().Where(f => f.Name == $"D{deviceID}U{User_ID}.jpg").FirstOrDefault();

                                    //if (fileInfo != null)
                                    //{
                                    //    fileInfo.Delete();
                                    //    copy.Save(UserImagePath);
                                    //    copy = null;
                                    //    UserImagePath = null;
                                    //    System.GC.Collect();
                                    //    System.GC.WaitForPendingFinalizers();

                                    //}
                                    _ = ImageOperations.UpdateImage(ConnString, UserToUpdate.EnrollNumber, ImageOperations.BitmapToByteArray(copy));
                                    UserImage = null;

                                }
                                //else if (clearImage.IsEnabled && !RetakeClicked && !ClearImageClicked && UserToUpdate.EMachineNumber != deviceID)
                                //{
                                //    UserImage.Dispose();
                                //    UserImagePath = @"C:\PHOTO\" + "D" + UserToUpdate.EMachineNumber + "U" + User_ID + ".jpg";
                                //    resetImage();
                                //    //Bitmap copy = (Bitmap)UserImage.Clone();
                                //    //UserImage = null;
                                //    GC.Collect();
                                //    GC.WaitForPendingFinalizers();
                                //    string NewName = @"C:\PHOTO\" + "D" + deviceID + "U" + User_ID + ".jpg";
                                //    File.Move(UserImagePath, NewName);
                                //}
                            }
                            if (string.IsNullOrEmpty(dptID))
                            {
                                query = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', Privilige = {admin}, dptID = 0 where EnrollNumber = {User_ID} and FingerNumber in (50, 0, 1, 2, 10, 11)";
                                updated = DBFactory.Update(ConnString, query);
                                dptID = "";
                                reset_controls();
                                disable_controls();
                            }
                            else
                            {
                                query = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', Privilige = {admin}, dptID = '{dptID}' where EnrollNumber = {User_ID} and FingerNumber in (50, 0, 1, 2, 10, 11)";
                                updated = DBFactory.Update(ConnString, query);
                                dptID = "";
                                reset_controls();
                                disable_controls();
                            }
                        }
                        else // mtlb kay card hai sirf 
                        {
                            if (string.IsNullOrEmpty(dptID))
                            {
                                string UpdateCard = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', enPassword = {cardNo}, Privilige = {admin}, dptID = 0 where EnrollNumber = {User_ID} and FingerNumber = 11";
                                updated = DBFactory.Update(ConnString, UpdateCard);
                                if (AlreadyEnrolledAny != null)
                                {
                                    string UpdateOthers = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', Privilige = {admin}, dptID = 0 where EnrollNumber = {User_ID} and FingerNumber in (50, 0, 1, 2, 10)";
                                    updated = updated && DBFactory.Update(ConnString, UpdateOthers);

                                }
                                dptID = "";
                                reset_controls();
                                disable_controls();
                            }
                            else
                            {
                                string UpdateCard = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', enPassword = {cardNo}, Privilige = {admin}, dptID = '{dptID}' where EnrollNumber = {User_ID} and FingerNumber = 11";
                                updated = DBFactory.Update(ConnString, UpdateCard);
                                if(AlreadyEnrolledAny != null)
                                {
                                    string UpdateOthers = $"Update tbl_enroll set EMachineNumber = {deviceID},  EName = '{UserName}', Privilige = {admin}, dptID = '{dptID}' where EnrollNumber = {User_ID} and FingerNumber in (50, 0, 1, 2, 10)";
                                    updated = updated && DBFactory.Update(ConnString, UpdateOthers);

                                }
                                dptID = "";
                                reset_controls();
                                disable_controls();
                            }
                        }
                       

                    }
                    bool isFaceEnrolled = false;
                    int RowsAffected = 0;
                    if (String.IsNullOrEmpty(dptID) && !BtnModifiedClicked)
                    {
                        //System.IO.File.WriteAllBytes(@"C:\\PHOTO\" + "D" + dwEnMachineID + "U" + dwEnrollNumber.ToString() + ".jpg", mbytCurEnrollData);

                        if (clearImage.IsEnabled)
                        {
                            //UserImagePath = @"C:\\PHOTO\" + "D" + deviceID + "U" + User_ID + ".jpg";
                            //UserImage.Save(UserImagePath);
                            byte[] imgToSave = ImageOperations.BitmapToByteArray(UserImage);
                            //string facequery = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData, Photo)VALUES(" + deviceID + ",'" + User_ID + "','" + 50 + "','" + admin + "','" + 0 + "','" + UserName + "','Empty', '" + hexString + "')";
                            RowsAffected += ImageOperations.InsertImage(ConnString, deviceID, User_ID, admin, UserName, imgToSave, "0");
                            //InsertImage(string connectionString, int deviceID, string User_ID, string admin, string UserName, byte[] imgToSave)
                            isFaceEnrolled = true;
                        }
                        if(AlreadyEnrolledCard == null && cardNo != 0)
                        {
                            query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData,dptID)VALUES(" + deviceID + ",'" + User_ID + "','" + 11 + "','" + admin + "','" + cardNo + "','" + UserName + "','Empty', '0' )";
                            RowsAffected += DBFactory.Insert(ConnString, query);

                        }
                        if(!clearImage.IsEnabled && cardNo == 0)
                        {
                            dptID = "";
                            reset_controls();
                            disable_controls();
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please provide at least one of the following (Card/Face)", false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                            return false;
                        }
                        dptID = "";
                        reset_controls();
                        disable_controls();
                    }
                    else if(!string.IsNullOrEmpty(dptID) && !BtnModifiedClicked)
                    {
                        if (clearImage.IsEnabled)
                        {
                            //UserImagePath = @"C:\\PHOTO\" + "D" + deviceID + "U" + User_ID + ".jpg";
                            //UserImage.Save(UserImagePath);
                            //string facequery = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData,dptID)VALUES(" + deviceID + ",'" + User_ID + "','" + 50 + "','" + admin + "','" + 0 + "','" + UserName + "','Empty','" + dptID + "')";
                            //RowsAffected += DBFactory.Insert(ConnString, facequery);
                            byte[] imgToSave = ImageOperations.BitmapToByteArray(UserImage);
                            RowsAffected += ImageOperations.InsertImage(ConnString, deviceID, User_ID, admin, UserName, imgToSave, dptID); 
                            isFaceEnrolled = true;
                        }
                        if(AlreadyEnrolledCard == null && cardNo != 0)
                        {
                            query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,FPData,dptID)VALUES(" + deviceID + ",'" + User_ID + "','" + 11 + "','" + admin + "','" + cardNo + "','" + UserName + "','Empty','" + dptID + "')";
                            RowsAffected += DBFactory.Insert(ConnString, query);
                        }
                        if(!clearImage.IsEnabled && cardNo == 0)
                        {
                            dptID = "";
                            reset_controls();
                            disable_controls();
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please provide at least one of the following (Card/Face)", false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                            return false;
                        }
                        
                        dptID = "";
                        reset_controls();
                        disable_controls();
                    }

                    if (RowsAffected == 1 || (RowsAffected == 2 && isFaceEnrolled) || updated)
                    {
                        isFaceEnrolled = false;
                        //Rows Affedted == 1 means that the data row is inserted into the table because in insert query above there is only one insert query will be executed
                        return true;

                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public void disable_controls()
        {
            txtCID.IsReadOnly = true;
            //txtDevID.IsReadOnly = true;
            //txtPassword.IsReadOnly = false;
            txtUID.IsReadOnly = true;
            txtUname.IsReadOnly = true;
            btnOpenFile.IsEnabled = false;
            btnEnroll.IsEnabled = false;
            Privillige.IsEnabled = false;
            Dept.IsEnabled = false;


            txtCID1.Visibility = Visibility.Visible;
            //txtDevID1.Visibility = Visibility.Visible;
            //txtPassword1.Visibility = Visibility.Collapsed;
            txtUID1.Visibility = Visibility.Visible;
            txtUname1.Visibility = Visibility.Visible;

        }
        public void reset_controls()
        {
            //txtDevID.Text = "";
            txtUID.Text = "";
            txtUname.Text = "";
            txtCID.Text = "";
            dptCombo.Text = "---Select Dpt---";
            resetImage();
            ocrCapBtn.IsEnabled = false;
            Dept.IsChecked = false;
            Privillige.IsChecked = false;
        }
        private void del_click(object sender, MouseButtonEventArgs e)
        {
            if (newenrolllist.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please select atleast one record to delete.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return;
            }
            else
            {
                string DeletePrompt = newenrolllist.SelectedItems.Count > 1 ? "Are you sure you want to delete the selected records" : "Are you sure you want to delete the selected record";
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, DeletePrompt, false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                bool Deleted = false;
                foreach(userS user in newenrolllist.SelectedItems)
                {
                    string DeleteQuery = $"delete from tbl_enroll where EnrollNumber = {user.EnrollNumber} and FingerNumber = {user.FingerNumber}";
                    Deleted = DBFactory.Delete(ConnString, DeleteQuery);
                }
                if (Deleted)
                {
                    string StatusWindowInfo = newenrolllist.SelectedItems.Count > 1 ? "Users Deleted" : "User Deleted";
                    StatusWindow.Text = StatusWindowInfo;
                    StatusWindow.Foreground = System.Windows.Media.Brushes.Green;
                    string MessageInfo = newenrolllist.SelectedItems.Count > 1 ? "Users successfully deleted" : "User Successfully deleted";
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, MessageInfo, false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    reset_controls();
                    disable_controls();
                    BtnModifiedClicked = false;
                    Thread thread = new Thread(ClearStatusWindows);
                    thread.Start();
                    getEnrollList();
                }
            }
        }

        private void ED_Done_click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            if (ED == 1)
            {
                string query = "delete from tbl_enroll where EnrollNumber = " + txtEID.Text;
                bool deleted = DBFactory.Delete(ConnString, query);
                if (deleted)
                {
                    int EnrollNumberToDelete = Convert.ToInt32(txtEID.Text);
                    UserIDFingerList.RemoveAll(user => user.EnrollNumber == EnrollNumberToDelete);
                }
                StatusWindow.Text = "User Deleted !!";
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                getEnrollList();
                ED = 0;

            }
            else if (ED == 2)
            {
                try
                {
                    if (string.IsNullOrEmpty(txtEID.Text))
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Enter User ID", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                        Keyboard.Focus(txtEID);
                        return;
                    }
                    string query = "select * from tbl_enroll where EnrollNumber = " + txtEID.Text;
                    dt = DBFactory.GetAllByQuery(ConnString, query);
                    if (dt.Rows.Count > 0)
                    {
                        EnableControlsforAdd();
                        txtUID.Text = dt.Rows[0].ItemArray[1].ToString();
                        //txtDevID.Text = dt.Rows[0].ItemArray[0].ToString();
                        //txtPath.Text = "C:\\Photo\\D" + txtDevID.Text + "U" + txtUID.Text + ".jpg";
                        if (dt.Rows[0].ItemArray[3].ToString() == "1")
                        {
                            Privillige.IsChecked = true;
                        }
                        else
                        {
                            Privillige.IsChecked = false;
                        }
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.ItemArray[2].ToString() == "11")
                            {
                                txtCID.Text = dr.ItemArray[4].ToString();
                                if (!String.IsNullOrEmpty(dr.ItemArray[8].ToString()))
                                {
                                    dptCombo.IsEnabled = true;
                                    dptCombo.SelectedValue = dr.ItemArray[8];
                                }
                            }
                            else if (dr.ItemArray[2].ToString() == "50")
                            {
                                txtUname.Text = dr.ItemArray[5].ToString();
                            }

                            //EnableUserControlsAddButton();
                            //EnrollFunc();
                        }


                        //System.Drawing.Image i = System.Drawing.Image.FromFile(txtPath.Text);

                        // System.Drawing.Image img1 = i.GetThumbnailImage(100, 100, null, new IntPtr());
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                        //try
                        //{
                        //    if (img1 != null)
                        //    {

                        //        bitmap.BeginInit();
                        //        System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                        //        img1.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        //        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                        //        bitmap.StreamSource = memoryStream;
                        //        bitmap.EndInit();
                        //    }
                        //}
                        //catch
                        //{

                        //}

                        //uImg.Source = bitmap;
                        StatusWindow.Text = "User Fetched !!";
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, StatusWindow.Text, false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    }

                }




                //}
                catch (Exception ex)
                {

                }

            }
            TB_ED.Visibility = Visibility.Collapsed;
            // }


        }

        private void Ed_click(object sender, MouseButtonEventArgs e)
        {
            //ED = 2;
            //TB_ED.Visibility = Visibility.Visible;
            //txtEID.Focus();
            PopulateFields();
            


        }
        public void PopulateFields()
        {
            if (newenrolllist.SelectedItems.Count == 1)
            {
                MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to edit the select record.", false);
                message1.ShowDialog();
                if (!message1.yes)
                {
                    return;
                }
                foreach (userS user in newenrolllist.SelectedItems)
                {
                    UserToUpdate = new userS(user);
                    
                    if (user.FingerNumber != 11)
                    {
                        txtUID.Text = user.EnrollNumber.ToString();
                        txtUID.IsReadOnly = true;
                        txtCID.Text = "";
                        txtUname.Text = user.EName;
                        txtUname.IsReadOnly = false;
                        txtCID.IsReadOnly = true;
                    }
                    else
                    {
                        txtUID.Text = user.EnrollNumber.ToString();
                        txtUID.IsReadOnly = true;
                        txtUname.Text = user.EName;
                        txtUname.IsReadOnly = false;
                        txtCID.Text = user.enPassword.ToString();
                        txtCID.IsReadOnly = false;
                    }
                    Privillige.IsEnabled = true;
                    if (user.Privilige == 1)
                    {
                        Privillige.IsChecked = true;
                    }


                    if (int.TryParse(user.DptID, out _))
                    {

                        if (Convert.ToInt32(user.DptID) >= 1)
                        {
                            Dept.IsEnabled = true;
                            Dept.IsChecked = true;
                            foreach (DataRow row in dptTable.Rows)
                            {
                                if ((int)row.ItemArray[0] == Convert.ToInt32(user.DptID))
                                {
                                    dptCombo.SelectedItem = row.ItemArray[1].ToString();
                                    dptCombo.IsEnabled = true;
                                }
                            }
                        }
                        else
                        {
                            dptCombo.SelectedIndex = -1;
                            dptCombo.Text = "----Select Dept----";
                            dptCombo.IsEnabled = false;
                            Dept.IsEnabled = true;
                            Dept.IsChecked = false;
                        }
                    }
                    else
                    {
                        dptCombo.SelectedIndex = -1;
                        dptCombo.Text = "----Select Dept----";
                        dptCombo.IsEnabled = false;
                        Dept.IsEnabled = true;
                        Dept.IsChecked = false;
                    }
                    if (user.FingerNumber == 50)
                    {
                        try
                        {
                            if (capImg.Source != null)
                            {
                                byte[] imgByteArray = ImageOperations.GetImageDataFromDatabase(ConnString, user.EnrollNumber);
                                Bitmap bmp = ImageOperations.ByteArrayToBitmap(imgByteArray);
                                UserImage = null;
                                capImg.Source = null;
                                int width = bmp.Width;
                                int height = bmp.Height;
                                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                     bmp.GetHbitmap(),
                                     IntPtr.Zero,
                                     Int32Rect.Empty,
                                     BitmapSizeOptions.FromEmptyOptions());
                                capImg.Source = bitmapSource;
                                UserImage = (Bitmap)bmp.Clone();
                                ocrCapBtn.Content = "Retake";
                                ocrCapBtn.IsEnabled = true;
                                clearImage.IsEnabled = true;

                                if (width == 480 && height == 640)
                                {
                                    capImg.Width = 110;
                                    capImg.Height = 150;
                                }
                                else
                                {
                                    capImg.Width = 200;
                                    capImg.Height = 150;
                                }
                            }

                        }
                        catch(Exception ex)
                        {
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, ex.Message, false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                        }
                    
                    }
                    else
                    {
                        ocrCapBtn.IsEnabled = false;
                        clearImage.IsEnabled = false;
                        resetImage();
                    }
                    txtCID1.Visibility = Visibility.Collapsed;
                    txtUID1.Visibility = Visibility.Collapsed;
                    txtUname1.Visibility = Visibility.Collapsed;
                    btnEnroll.IsEnabled = true;
                    BtnModifiedClicked = true;
                }
                btnEnroll.Content = "Update User";

            }
            else if (newenrolllist.SelectedItems.Count == 0)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please select atleast one row from the list.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
            else
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "You can only select one row to modify at a time.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }

        public void getEnrollList()
        {
            //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,EMachineNumber as DevID,EnrollNumber as UserID,EName as UserName,CASE when FingerNumber = 50 THEN 'Face OK' WHEN FingerNumber = 0 THEN 'FingerPrint OK' ELSE 'NO DATA' END AS Data FROM tbl_enroll";
            //newenrolllist.Items.Clear();
            newenrolllist.ItemsSource = null;
            //string query = "SELECT distinct EnrollNumber,ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll";
            string query = "SELECT distinct EnrollNumber,ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo, EMachineNumber, EnrollNumber, FingerNumber, Privilige, enPassword, EName, ShiftId, FPData, DptID FROM tbl_enroll";
            staffTab = DBFactory.GetAllByQuery(ConnString, query);
            EnD = new ObservableCollection<userS>();
            EnD = ConvertDataTableObs<userS>(staffTab);
            EnD_Backup = new ObservableCollection<userS>(EnD);
            UserIDFingerList = EnD.ToList();
            newenrolllist.ItemsSource = EnD;
            int distinctCount = EnD.Select(user => user.EnrollNumber).Distinct().Count();
            usCount.Content = distinctCount;

            foreach (userS user in EnD)
            {
                foreach(Departments department in dptNames)
                {

                    if(user.DptID != null)
                    {
                        if(int.Parse(user.DptID) == department.dept_id)
                        {
                            user.DepartmentName = department.dept_name;
                        }
                    }
                }

                user.PriviligeName = user.Privilige == 0 ? "User" : "Admin";
                if (user.FingerNumber == 11)
                {
                    user.FingerName = "Card";
                }
                else if (user.FingerNumber == 50)
                {
                    user.FingerName = "Face";
                }
                else if (user.FingerNumber == 10)
                {
                    user.FingerName = "Password";
                }
                else
                {
                    user.FingerName = "Finger";
                }
            }
            //usCount.Content = EnD.Count;
            //foreach (userS item in EnD)
            //{
            //    //////////////UdownDisp item = new UdownDisp();
            //    newenrolllist.Items.Add(item);
            //}
            //foreach (DataRow dr in staffTab.Rows)
            //{
            //    string al1 = dr.ItemArray[5].ToString();
            //    if (String.IsNullOrEmpty(dr.ItemArray[5].ToString()))
            //    {
            //        //query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "'," + dr.ItemArray[6].ToString() + ")";
            //        //DBFactory.Insert(ConnString, query);
            //        //string photoname = "D" + dr.ItemArray[0].ToString() + "U" + dr.ItemArray[1].ToString();
            //        //copy(directoryname + "\\PHOTO", photoname, photoname);

            //        //newenrolllist.Items.Add(dr);
            //    }
            //    else
            //    {

            //        newenrolllist.Items.Add(dr);
            //    }
            //    //newenrolllist.ItemsSource = staffTab.AsDataView();

            //}
        }
        public void getEnrollList(string quer)
        {
            //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,EMachineNumber as DevID,EnrollNumber as UserID,EName as UserName,CASE when FingerNumber = 50 THEN 'Face OK' WHEN FingerNumber = 0 THEN 'FingerPrint OK' ELSE 'NO DATA' END AS Data FROM tbl_enroll";
            //string query = "SELECT * FROM tbl_enroll";
            newenrolllist.Items.Clear();
            DataTable staffTab = DBFactory.GetAllByQuery(ConnString, quer);
            EnD = new ObservableCollection<userS>();
            EnD = ConvertDataTableObs<userS>(staffTab);
            foreach (userS item in EnD)
            {
                //UdownDisp item = new UdownDisp();
                newenrolllist.Items.Add(item);
            }
            usCount.Content = EnD.Count;
            //foreach (DataRow dr in staffTab.Rows)
            //{
            //    string al1 = dr.ItemArray[5].ToString();
            //    if (String.IsNullOrEmpty(dr.ItemArray[5].ToString()))
            //    {
            //        //query = "INSERT INTO tbl_enroll(EMachineNumber,EnrollNumber,FingerNumber,Privilige,enPassword,EName,ShiftId)VALUES(" + dr.ItemArray[0].ToString() + ",'" + dr.ItemArray[1].ToString() + "','" + dr.ItemArray[2].ToString() + "','" + dr.ItemArray[3].ToString() + "','" + dr.ItemArray[4].ToString() + "','" + "" + "'," + dr.ItemArray[6].ToString() + ")";
            //        //DBFactory.Insert(ConnString, query);
            //        //string photoname = "D" + dr.ItemArray[0].ToString() + "U" + dr.ItemArray[1].ToString();
            //        //copy(directoryname + "\\PHOTO", photoname, photoname);

            //        //newenrolllist.Items.Add(dr);
            //    }
            //    else
            //    {

            //        newenrolllist.Items.Add(dr);
            //    }
            //    //newenrolllist.ItemsSource = staffTab.AsDataView();


            //}
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dptCombo.SelectedIndex = -1;
            dptCombo.Text = "----Select Dept----";
            setDptCombo();
            getEnrollList();
            //foreach(DataRow row in staffTab.Rows)
            //{
            //    userS obj = new userS
            //    {
            //        EnrollNumber = Convert.ToInt32(row.ItemArray[0]),
            //        FingerNumber = Convert.ToInt32(row.ItemArray[4]),
            //        EName = row.ItemArray[7].ToString(),
            //        DptID = row.ItemArray[10].ToString()
            //    };
            //    UserIDFingerList.Add(obj);
            //}

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            BtnModifiedClicked = false;
            resetImage();
            UserImage = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public void setDptCombo()
        {
            dptTable = DBFactory.GetAllByQuery(ConnString, "SELECT * FROM tbl_department");
            dptNames = Globals.ConvertDataTable<Departments>(dptTable).ToList();
            dptCombo.ItemsSource = dptNames.Select(x => x.dept_name);
            //dptCombo.SelectedItem = "SQN 1";
        }
        private void txtEID_KeyDown(object sender, KeyEventArgs e)
        {
            txtEID1.Visibility = Visibility.Collapsed;
        }

        private void btnClear(object sender, RoutedEventArgs e)
        {
            txtEID.Clear();
            txtUID.Clear();
            //txtPath.Clear();
            //txtDevID.Clear();
            //txtPassword.Clear();
            txtCID.Clear();
            txtUname.Clear();
            //uImg.Source = null;
            txtCID1.Visibility = Visibility.Visible;
            //txtDevID1.Visibility = Visibility.Visible;
            //txtPassword1.Visibility = Visibility.Visible;
            txtUID1.Visibility = Visibility.Visible;
            txtUname1.Visibility = Visibility.Visible;
            //txtPath1.Visibility = Visibility.Visible;
            txtEID1.Visibility = Visibility.Visible;


        }


        private void refreshEnroList(object sender, MouseButtonEventArgs e)
        {
            //if (ReferenceEquals(EnD_Backup, null))
            //{
            //    EnD_Backup = EnD;

            //}
            int count = 0;
            if (face.IsChecked == true && finger.IsChecked == false && notEnr.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 50";
                //getEnrollList(query);

                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => Convert.ToInt32(user.FingerNumber) == 50));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;

            }
            if (finger.IsChecked == true && notEnr.IsChecked == false && face.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 0";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => Convert.ToInt32(user.FingerNumber) == 0));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if (notEnr.IsChecked == true && finger.IsChecked == false && face.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 11";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => Convert.ToInt32(user.FingerNumber) == 11));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if (face.IsChecked == true && finger.IsChecked == true && notEnr.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 50 or FingerNumber = 0";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => (Convert.ToInt32(user.FingerNumber) == 00) || Convert.ToInt32(user.FingerNumber) == 50));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if (face.IsChecked == true && notEnr.IsChecked == true && finger.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 50 or FingerNumber = 11";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => (Convert.ToInt32(user.FingerNumber) == 50) || Convert.ToInt32(user.FingerNumber) == 11));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if (finger.IsChecked == true && notEnr.IsChecked == true && face.IsChecked == false)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 0 or FingerNumber = 11";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => (Convert.ToInt32(user.FingerNumber) == 0) || Convert.ToInt32(user.FingerNumber) == 11));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if (face.IsChecked == true && finger.IsChecked == true && notEnr.IsChecked == true)
            {
                //string query = "SELECT ROW_NUMBER() OVER (ORDER BY EnrollNumber ASC) AS SrNo,* FROM tbl_enroll where FingerNumber = 50 or FingerNumber = 0 or FingerNumber = 11";
                //getEnrollList(query);
                ObservableCollection<userS> temp_Users = new ObservableCollection<userS>(EnD_Backup.Where(user => (Convert.ToInt32(user.FingerNumber) == 50) || Convert.ToInt32(user.FingerNumber) == 11 || Convert.ToInt32(user.FingerNumber) == 0));
                count = temp_Users.Count();
                EnD = temp_Users;

                newenrolllist.ItemsSource = null;
                newenrolllist.ItemsSource = EnD;
            }
            if(face.IsChecked == false && finger.IsChecked == false && notEnr.IsChecked == false)
            {
                newenrolllist.ItemsSource = null;
                count = 0;
            }
            usCount.Content = count.ToString();

        }
        //private void ExportUsersWithFacesOnly()
        //{
        //    string filePath = ShowSaveFileDialog();
        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        if (EnD.Count > 0)
        //        {
        //            SaveFileDialog sfd = new SaveFileDialog();
        //            Application.Current.Dispatcher.Invoke((Action)delegate
        //            {
        //                try
        //                {
        //                    MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
        //                    messaget.ShowDialog();
        //                    DataTable table = new DataTable();
        //                    table.Columns.Add("SrNo", typeof(long));
        //                    table.Columns.Add("EMachineNumber", typeof(int));
        //                    table.Columns.Add("EnrollNumber", typeof(int));
        //                    table.Columns.Add("FingerNumber", typeof(int));
        //                    table.Columns.Add("Privilege", typeof(int));
        //                    table.Columns.Add("enPassword", typeof(int));
        //                    table.Columns.Add("EName", typeof(string));
        //                    table.Columns.Add("ShiftID", typeof(string));
        //                    table.Columns.Add("DptID", typeof(string));
        //                    int count = 0;
        //                    foreach (userS item in EnD)
        //                    {
        //                        if (item.FingerNumber == 50)
        //                        {
        //                            count++;

        //                            table.Rows.Add(item.SrNo, item.EMachineNumber, item.EnrollNumber, item.FingerNumber, item.Privilige, item.enPassword, item.EName, item.ShiftId, item.DptID);

        //                        }
        //                    }


        //                    try
        //                    {
        //                        bool res = ExcelFunc.Write_Excel(table, filePath);
        //                        if (res)
        //                        {
        //                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Exported Successfully!\n", false);
        //                            message.ShowDialog();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occured!\n" + ex.Message, false);
        //                        message.btnCancel.Visibility = Visibility.Collapsed;
        //                        message.btnOk.Content = "Ok";
        //                        message.ShowDialog();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //                    string message = "( " + DateTime.Now + " ) ( ExportUsers_Click  ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //                    File.AppendAllText(path, message);
        //                }
        //            });

        //        }
        //        else
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "There is no data to be exported.", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //        }
        //    }
        //}
        private void ExportUsersWithFacesOnly(bool ExportImages)
        {
            LoadingWindow loader = null;
            string filePath = ShowSaveFileDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                string Directory = Path.GetDirectoryName(filePath);
                if (EnD.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() => { loader = new LoadingWindow(); loader.Show(); });
                    try
                    {
                        //DataTable table = new DataTable();
                        DataTable table = GenerateTableWithFaces(ExportImages);
                        //table.Columns.Add("SrNo", typeof(long));
                        ////table.Columns.Add("EMachineNumber", typeof(int));
                        //table.Columns.Add("EnrollNumber", typeof(int));
                        ////table.Columns.Add("FingerNumber", typeof(int));
                        ////table.Columns.Add("Privilege", typeof(int));
                        ////table.Columns.Add("enPassword", typeof(int));
                        //table.Columns.Add("EName", typeof(string));
                        ////table.Columns.Add("ShiftID", typeof(string));
                        ////table.Columns.Add("DptID", typeof(string));
                        //table.Columns.Add("Photo", typeof(byte[]));
                        //DataTable dt = DBFactory.GetAllByQuery(ConnString, "SELECT EnrollNumber, Photo FROM tbl_enroll WHERE FingerNumber = 50");
                        //// Create a dictionary from dt for faster lookups
                        //Dictionary<int, byte[]> photoMap = dt.AsEnumerable().ToDictionary(row => row.Field<int>("EnrollNumber"), row => row.Field<byte[]>("Photo"));
                        //int count = 1;
                        //foreach (userS item in EnD)
                        //{
                        //    if (item.FingerNumber == 50)
                        //    {
                        //        byte[] photo = photoMap.ContainsKey(item.EnrollNumber) ? photoMap[item.EnrollNumber] : null;
                        //        //table.Rows.Add(item.SrNo, item.EMachineNumber, item.EnrollNumber, item.FingerNumber, item.Privilige, item.enPassword, item.EName, item.ShiftId, item.DptID, photo);
                        //        table.Rows.Add(count, item.EnrollNumber, item.EName, photo);
                        //        count++;
                        //    }
                        //}
                        //ExportImages(dt,filePath);
                        string msg = string.Empty;
                        bool res = false;
                        //bool res = ExcelFunc.Write_Excel(table, filePath, out msg);
                        switch (ExportImages)
                        {
                            case true:
                                res = ExcelFunc.Write_ExcelWithImages(table, filePath, out msg);
                                break;
                            case false:
                                res = ExcelFunc.Write_Excel_Without_Images(table, filePath, out msg);
                                break;
                        }
                        //bool res = ExcelFunc.Write_ExcelWithImages(table, filePath, out msg);
                        //if (ExportImages) { ExportPhotos(dt, Directory); }
                        // Display success message on UI thread
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            loader.Hide();
                            if (res)
                            {
                                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Exported Successfully!\n", false);
                                message.ShowDialog();
                            }
                            else if(!string.IsNullOrEmpty(msg))
                            {
                                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occurred!\n" + msg, false);
                                message.ShowDialog();
                            }
                        });
                        //if (res)
                        //{
                        //    //Application.Current.Dispatcher.Invoke(() => { loader.Hide(); });
                        //    Application.Current.Dispatcher.Invoke(() =>
                        //    {
                        //        loader.Hide();
                        //        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Exported Successfully!\n", false);
                        //        message.ShowDialog();
                        //    });
                        //}
                        //Application.Current.Dispatcher.Invoke(() => { loader.Hide(); });
                    }
                    catch (Exception ex)
                    {
                        //Application.Current.Dispatcher.Invoke(() => { loader.Hide(); });
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            loader.Hide();
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occurred!\n" + ex.Message, false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                        });
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "There is no data to be exported.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    });
                }
            }
        }
        private DataTable GenerateTableWithFaces(bool ExportImages)
        {
            DataTable table = new DataTable();
            switch (ExportImages)
            {
                case true:
                    table.Columns.Add("SrNo", typeof(long));
                    table.Columns.Add("EnrollNumber", typeof(int));
                    table.Columns.Add("Employee Name", typeof(string));
                    table.Columns.Add("Photo", typeof(byte[]));
                    DataTable dt = DBFactory.GetAllByQuery(ConnString, "SELECT EnrollNumber, Photo FROM tbl_enroll WHERE FingerNumber = 50");
                    // Create a dictionary from dt for faster lookups
                    Dictionary<int, byte[]> photoMap = dt.AsEnumerable().ToDictionary(row => row.Field<int>("EnrollNumber"), row => row.Field<byte[]>("Photo"));
                    int count = 1;
                    foreach (userS item in EnD)
                    {
                        if (item.FingerNumber == 50)
                        {
                            byte[] photo = photoMap.ContainsKey(item.EnrollNumber) ? photoMap[item.EnrollNumber] : null;
                            table.Rows.Add(count, item.EnrollNumber, item.EName, photo);
                            count++;
                        }
                    }
                    break;
                case false:
                    table.Columns.Add("SrNo", typeof(long));
                    table.Columns.Add("EMachineNumber", typeof(int));
                    table.Columns.Add("EnrollNumber", typeof(int));
                    table.Columns.Add("FingerNumber", typeof(int));
                    table.Columns.Add("Privilege", typeof(int));
                    table.Columns.Add("enPassword", typeof(int));
                    table.Columns.Add("EName", typeof(string));
                    table.Columns.Add("ShiftID", typeof(string));
                    table.Columns.Add("DptID", typeof(string));
                    table.Columns.Add("Photo", typeof(byte[]));
                    foreach (userS item in EnD)
                    {
                        if (item.FingerNumber == 50)
                        {
                            table.Rows.Add(item.SrNo, item.EMachineNumber, item.EnrollNumber, item.FingerNumber, item.Privilige, item.enPassword, item.EName, item.ShiftId, item.DptID);
                        }
                    }
                    break;
            }
            return table;

        }
        private void ExportPhotos(DataTable dt, string path)
        {
            foreach(DataRow row in dt.Rows)
            {
                try
                {
                    string imagesFolder = Path.Combine(path, "Images");
                    // Get EnrollNumber and Photo (byte array) from the current row
                    int enrollNumber = Convert.ToInt32(row["EnrollNumber"]);
                    byte[] photoBytes = row["Photo"] as byte[];
                    Directory.CreateDirectory(imagesFolder);
                    if (photoBytes != null && photoBytes.Length > 0)
                    {
                        // Convert the byte array to an image
                        using (MemoryStream ms = new MemoryStream(photoBytes))
                        {
                            // Create an image from the byte array
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                            // Define the path for the image file (EnrollNumber as filename)
                            string imagePath = Path.Combine(imagesFolder, $"{enrollNumber}.jpg");

                            // Save the image to the file system
                            image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions (e.g., if a row has invalid data)
                    Console.WriteLine($"Error processing EnrollNumber {row["EnrollNumber"]}: {ex.Message}");
                }
            }
        }

        //private void ExportUsers()
        //{
        //    string filePath = ShowSaveFileDialog();
        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        if (EnD.Count > 0)
        //        {

        //            Application.Current.Dispatcher.Invoke((Action)delegate
        //            {
        //                try
        //                {
        //                    //StatusWindow.Text = "Wait...\tExporting Log Data!";
        //                    //Co_Wait.Visibility = Visibility.Visible;
        //                    MessageBoxWindow messaget = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
        //                    messaget.ShowDialog();
        //                    DataTable table = new DataTable();
        //                    table.Columns.Add("SrNo", typeof(long));
        //                    table.Columns.Add("EMachineNumber", typeof(int));
        //                    table.Columns.Add("EnrollNumber", typeof(int));
        //                    table.Columns.Add("FingerNumber", typeof(int));
        //                    table.Columns.Add("Privilege", typeof(int));
        //                    table.Columns.Add("enPassword", typeof(int));
        //                    table.Columns.Add("EName", typeof(string));
        //                    table.Columns.Add("ShiftID", typeof(string));
        //                    table.Columns.Add("DptID", typeof(string));
        //                    //List<userS> list = newenrolllist.Items;
        //                    foreach (userS item in EnD)
        //                    {
        //                        if (item.FingerNumber == 11)
        //                        {
        //                            table.Rows.Add(item.SrNo, item.EMachineNumber, item.EnrollNumber, item.FingerNumber, item.Privilige, item.enPassword, item.EName, item.ShiftId, item.DptID);

        //                        }
        //                        //foreach (var it in item) //For adding row values
        //                        //    table.Rows.Add(it.ToString());
        //                    }
        //                    //foreach (ListViewItem item in newenrolllist.Items)
        //                    //{
        //                    //    table.Rows.Add(item.,item.EMachineNumber, item.EnrollNumber, item.EName, item.FingerNumber);
        //                    //    //table.Rows.Add(item.TMchNo, item.EnrollNo, item.EMchNo, item.DateTime);
        //                    //}
        //                    //table.Rows.Add("end", "end", "end", "end", "end", "end");
        //                    //table.Rows.Add("end", "end", "end", "end", "end", "end");

        //                    try
        //                    {
        //                        bool res = ExcelFunc.Write_Excel(table, filePath);
        //                        if (res)
        //                        {
        //                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Exported Successfully!\n", false);
        //                            message.ShowDialog();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occured!\n" + ex.Message, false);
        //                        message.btnCancel.Visibility = Visibility.Collapsed;
        //                        message.btnOk.Content = "Ok";
        //                        message.ShowDialog();
        //                    }

        //                    //Co_Wait.Visibility = Visibility.Collapsed;
        //                    //tatusWindow.Text = "Excel File Creation Succesfull!!";
        //                }
        //                catch (Exception ex)
        //                {
        //                    string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
        //                    string message = "( " + DateTime.Now + " ) ( ExportUsers_Click  ) ( " + ex.ToString() + " )" + Environment.NewLine;
        //                    File.AppendAllText(path, message);
        //                }
        //            });

        //        }
        //        else
        //        {
        //            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "There is no data to be exported.", false);
        //            message.btnCancel.Visibility = Visibility.Collapsed;
        //            message.btnOk.Content = "Ok";
        //            message.ShowDialog();
        //        }
        //    }

        //}
        private void ExportUsers()
        {
            LoadingWindow loader = null;
            string filePath = ShowSaveFileDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                if (EnD.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() => { loader = new LoadingWindow(); loader.Show(); });
                    try
                    {
                        DataTable table = new DataTable();
                        table.Columns.Add("SrNo", typeof(long));
                        table.Columns.Add("EMachineNumber", typeof(int));
                        table.Columns.Add("EnrollNumber", typeof(int));
                        table.Columns.Add("FingerNumber", typeof(int));
                        table.Columns.Add("Privilege", typeof(int));
                        table.Columns.Add("enPassword", typeof(int));
                        table.Columns.Add("EName", typeof(string));
                        table.Columns.Add("ShiftID", typeof(string));
                        table.Columns.Add("DptID", typeof(string));

                        foreach (userS item in EnD)
                        {
                            if (item.FingerNumber == 11)
                            {
                                table.Rows.Add(item.SrNo, item.EMachineNumber, item.EnrollNumber, item.FingerNumber, item.Privilige, item.enPassword, item.EName, item.ShiftId, item.DptID);
                            }
                        }
                        string msg;
                        bool res = ExcelFunc.Write_Excel(table, filePath, out msg);

                        // Display success message on UI thread
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            loader.Hide();
                            if (res)
                            {
                                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Exported Successfully!\n", false);
                                message.ShowDialog();
                            }
                            else if (!string.IsNullOrEmpty(msg))
                            {
                                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occurred!\n" + msg, false);
                                message.ShowDialog();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log error and show exception message on UI thread
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            loader.Hide();
                            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Exception Occurred!\n" + ex.Message, false);
                            message.btnCancel.Visibility = Visibility.Collapsed;
                            message.btnOk.Content = "Ok";
                            message.ShowDialog();
                        });

                        string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                        string errorMessage = "( " + DateTime.Now + " ) ( ExportUsers ) ( " + ex.ToString() + " )" + Environment.NewLine;
                        File.AppendAllText(path, errorMessage);
                    }
                }
                else
                {
                    // Show "no data" message on UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "There is no data to be exported.", false);
                        message.btnCancel.Visibility = Visibility.Collapsed;
                        message.btnOk.Content = "Ok";
                        message.ShowDialog();
                    });
                }
            }
        }

        public string ShowSaveFileDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            // Set the filter for .xls files only
            sfd.Filter = "Excel Files (*.xls)|*.xls";

            // Set the default file extension to .xls
            sfd.DefaultExt = "xls";

            // Set the title for the dialog box
            sfd.Title = "Save Excel File";

            // Display the SaveFileDialog and check if the user clicked 'Save'
            if (sfd.ShowDialog() == true)
            {
                // Get the file path chosen by the user
                return sfd.FileName;                

                // You can now use filePath to save the file
                // (implement saving logic here)
            }
            return "";
        }
        private async void btnExExcel(object sender, RoutedEventArgs e)
        {
            // Show the loading window
            //LoadingWindow loader = new LoadingWindow();
            //loader.Show();
            try
            {
                if((bool)face.IsChecked && (bool)notEnr.IsChecked)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "You can only export one at a time!", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
                if ((bool)face.IsChecked)
                {
                    bool ExportFaces = (bool)chk_ExportFaces.IsChecked;
                    await Task.Run(() => ExportUsersWithFacesOnly(ExportFaces));
                    //Thread backgroundThread1 = new Thread(new ThreadStart(ExportUsersWithFacesOnly));
                    //backgroundThread1.Start();
                }
                else
                {
                    await Task.Run(() => ExportUsers());
                    //Thread backgroundThread = new Thread(new ThreadStart(ExportUsers));
                    //// Start thread  
                    //backgroundThread.Start();
                }
            }
            finally
            {
                //loader.Hide();
            }
            //if ((bool)face.IsChecked)
            //{
            //    Thread backgroundThread1 = new Thread(new ThreadStart(ExportUsersWithFacesOnly));
            //    backgroundThread1.Start();
            //}
            //else
            //{
            //    Thread backgroundThread = new Thread(new ThreadStart(ExportUsers));
            //    // Start thread  
            //    backgroundThread.Start();
            //}
        }

        private void dptCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dptCombo.SelectedIndex != -1)
            {
                dptID = dptTable.Rows[dptCombo.SelectedIndex].ItemArray[0].ToString();

            }
            else
            {
                dptID = "";
                dptCombo.Text = "---Select Dpt---";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            dptCombo.Text = "---Select Dpt---";
            dptCombo.IsEnabled = true;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            dptCombo.SelectedIndex = -1;
            dptCombo.IsEnabled = false;
        }

        private void txtEID_GotFocus(object sender, RoutedEventArgs e)
        {
            txtEID1.Visibility = Visibility.Collapsed;
        }

        private void txtUID1_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtUID.IsReadOnly)
            {
                txtUID1.Visibility = Visibility.Collapsed;

            }
        }

        //private void txtDevID_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (!txtDevID.IsReadOnly)
        //    {
        //        txtDevID1.Visibility = Visibility.Collapsed;

        //    }
        //}

        private void txtUname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtUname.IsReadOnly)
            {
                txtUname1.Visibility = Visibility.Collapsed;

            }
        }

        private void txtCID_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtCID.IsReadOnly)
            {
                txtCID1.Visibility = Visibility.Collapsed;

            }
        }

        private void newenrolllist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (userS item in newenrolllist.SelectedItems)
            {
                EnableControlsforAdd();
                txtUID.Text = item.EnrollNumber.ToString();
                //txtDevID.Text = item.EMachineNumber.ToString();
                //txtPath.Text = "C:\\Photo\\D" + txtDevID.Text + "U" + txtUID.Text + ".jpg";
                if (item.Privilige == 1)
                {
                    Privillige.IsChecked = true;
                }
                else
                {
                    Privillige.IsChecked = false;
                }

                if (item.FingerNumber == 11)
                {
                    txtCID.Text = item.enPassword.ToString();
                    if (!String.IsNullOrEmpty(item.DptID.ToString()))
                    {
                        dptCombo.IsEnabled = true;
                        dptCombo.SelectedValue = item.DptID;
                    }
                }
                else if (item.FingerNumber == 50)
                {
                    txtUname.Text = item.EName;
                }


            }
        }

        private void txtUID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUID.Text.Length.Equals(0))
            {
                txtUID1.Visibility = Visibility.Visible;
            }
        }

        //private void txtDevID_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtDevID.Text.Length.Equals(0))
        //    {
        //        txtDevID1.Visibility = Visibility.Visible;
        //    }
        //}

        private void txtUname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUname.Text.Length.Equals(0))
            {
                txtUname1.Visibility = Visibility.Visible;
            }
        }

        private void txtCID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCID.Text.Length.Equals(0))
            {
                txtCID1.Visibility = Visibility.Visible;
            }
        }
        public void ClearStatusWindows()
        {
            Thread.Sleep(2000);
            Action updateUI = () =>
            {
                StatusWindow.Text = "";
            };
            Dispatcher.Invoke(updateUI);
        }

        private void btnCapImg(object sender, RoutedEventArgs e)
        {
            test capImgScr = new test(this);
            capImgScr.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            resetImage();
            ClearImageClicked = true;
            RetakeClicked = false;
        }
        private void resetImage()
        {
            
                capImg.Source = null;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("../Images/userImage.png", UriKind.Relative);
                bitmapImage.EndInit();

                capImg.Source = bitmapImage;
                ocrCapBtn.Content = "Image";
                clearImage.IsEnabled = false;
                capImg.Width = 130;
                capImg.Height = 150;

            
        }
        private Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            Bitmap bitmap;

            using (MemoryStream stream = new MemoryStream())
            {
                // Save BitmapImage to stream
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);

                // Create Bitmap from stream
                bitmap = new Bitmap(stream);
            }

            return bitmap;
        }

        private void chkAll_Checked(object sender, RoutedEventArgs e)
        {
            if(newenrolllist.Items.Count > 0)
            {
                newenrolllist.SelectAll();
                chkAll.Content = "Clear Selection";
            }
        }

        private void chkAll_UnChecked(object sender, RoutedEventArgs e)
        {
            if(newenrolllist.Items.Count > 0 && newenrolllist.SelectedItems.Count > 0)
            {
                newenrolllist.SelectedItems.Clear();
                chkAll.Content = "Select All";
            }
        }

        private void face_Checked(object sender, RoutedEventArgs e)
        {
            chk_ExportFaces.IsEnabled = true;
            //chk_ExportFaces.IsChecked = true;
        }

        private void face_Unchecked(object sender, RoutedEventArgs e)
        {
            chk_ExportFaces.IsEnabled = false;
            chk_ExportFaces.IsChecked = false;
        }

        private void txt_search_1_GotFocus(object sender, RoutedEventArgs e)
        {
            txt_search_1.Visibility = Visibility.Collapsed;
        }

        private void txt_search_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txt_search.Text.Length == 0)
            {
                txt_search_1.Visibility = Visibility.Visible;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string input = txt_search.Text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            int id;
            if(int.TryParse(input, out id))
            {
                find(input);
                if(newenrolllist.SelectedItems.Count == 0)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Record not found.", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                    return;
                }
            }
            else
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Please enter a valid integer value.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
            }
        }
        private void find(string str)
        {
            foreach (userS item in newenrolllist.Items)
            {
                int val = -1;
                bool res = int.TryParse(str, out int n);
                if (res)
                {
                    val = int.Parse(str);
                }
                if (item.EnrollNumber == val)
                {
                    newenrolllist.SelectedItems.Add(item);
                    newenrolllist.ScrollIntoView(item);  // Scroll to the item
                }
                else
                {
                    newenrolllist.SelectedItems.Remove(item);
                }
            }

        }
    }
}
