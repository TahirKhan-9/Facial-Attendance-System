using FaceAttendance.Classes;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;
using UsmanCodeBlocks.Data.Sql.Local;

namespace FaceAttendance.UserControls
{
    /// <summary>
    /// Interaction logic for ucLogsMigration.xaml
    /// </summary>
    public partial class ucLogsMigration : UserControl
    {
        string ConString;
        string TargetConString;
        int DBRowID = 0;
        bool AlreadySaved = false;
        List<TableSchema> TableSchemas = new List<TableSchema>();
        private List<ComboBox> _comboBoxes; // Stores all the ComboBoxes
        List<string> selectedItems = new List<string>();
        List<LockedItems> LockedCmbItems = new List<LockedItems>();

        public ucLogsMigration(string ConnectionString)
        {
            this.ConString = ConnectionString;
            InitializeComponent();
        }

        private void txtUserID_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOverlayUserID.Visibility = Visibility.Collapsed;
        }

        private void txtUserID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserID.Text.Length == 0)
            {
                txtOverlayUserID.Visibility = Visibility.Visible;
            }
            else
            {
                txtOverlayUserID.Visibility = Visibility.Collapsed;
            }
        }

        private void txtServer_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOverlayServer.Visibility = Visibility.Collapsed;
        }

        private void txtServer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtServer.Text.Length == 0)
            {
                txtOverlayServer.Visibility = Visibility.Visible;
            }
            else
            {
                txtOverlayServer.Visibility = Visibility.Collapsed;
            }
        }

        private void txtPwd_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOverlayPwd.Visibility = Visibility.Collapsed;
        }

        private void txtPwd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPwd.Text.Length == 0)
            {
                txtOverlayPwd.Visibility = Visibility.Visible;
            }
            else
            {
                txtOverlayPwd.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSaveConnection_Click(object sender, RoutedEventArgs e)
        {
            bool FieldsValidated = ValidateSqlServerFields();
            if (!FieldsValidated)
            {
                return;
            }
            MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Info, "Please Wait!!!", true);
            _ = message.ShowDialog();

            string serverIp = txtServer.Text;
            string userId = txtUserID.Text;
            string password = txtPwd.Text;
            string DBname = txtDBName.Text;
            string ConnectionString = $"Server={serverIp};User Id={userId};Password={password};";
            try
            {
                bool serverConnected = ConnectServer(ConnectionString);
                if (serverConnected)
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Success, "Server Connection Succeed", true);
                    msg.ShowDialog();
                    MessageBoxWindow msg2 = new MessageBoxWindow(MessageBoxType.Info, "Connecting to Database...", true);
                    msg2.ShowDialog();
                    string res = FindDbInstance(serverIp, userId, password, DBname);
                    if (res.Length == 3)
                    {
                        switch (res)
                        {
                            case "404":
                                MessageBoxWindow msg3 = new MessageBoxWindow(MessageBoxType.Alert, "Specified Database Not Found!", false);
                                msg3.ShowDialog();
                                break;
                            case "402":
                                MessageBoxWindow msg4 = new MessageBoxWindow(MessageBoxType.Error, "Database Found But Connection Failed!", false);
                                msg4.ShowDialog();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        MessageBoxWindow msg5 = new MessageBoxWindow(MessageBoxType.Success, "Database Connected Successfully.", true);
                        msg5.ShowDialog();
                        MessageBoxWindow msg6 = new MessageBoxWindow(MessageBoxType.Info, "Saving Database Connection!", true);
                        msg6.ShowDialog();
                        TargetConString = res;
                        string result = SaveDatabaseCredentials(serverIp, userId, password, DBname, out DBRowID);
                        switch (result)
                        {
                            case "Updated":
                                MessageBoxWindow msg10 = new MessageBoxWindow(MessageBoxType.Success, "Updated Successfully.", false);
                                msg10.ShowDialog();
                                EnableControls(false);
                                PopulateTableComboBox(TargetConString);
                                break;
                            case "AlreadySaved":
                                MessageBoxWindow msg7 = new MessageBoxWindow(MessageBoxType.Alert, "Already Saved! You can update the setting.", false);
                                msg7.ShowDialog();
                                break;
                            case "1":
                                MessageBoxWindow msg8 = new MessageBoxWindow(MessageBoxType.Success, "Saved Successfully.", false);
                                msg8.ShowDialog();
                                EnableControls(false);
                                AlreadySaved = true;
                                PopulateTableComboBox(TargetConString);
                                break;
                            case "0":
                                MessageBoxWindow msg9 = new MessageBoxWindow(MessageBoxType.Info, "Settings not saved!", false);
                                msg9.ShowDialog();
                                break;
                            default:
                                break;
                        }

                    }

                }
                else
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Server Connection Failed!", false);
                    msg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
                return;
            }
        }
        private void PopulateTableComboBox(string ConnectionString)
        {
            if (ConnectionString != null)
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                cmbTables.Items.Add(row["TABLE_NAME"]);
                            }
                        }
                    }
                }
            }
        }
        private string SaveDatabaseCredentials(string serverIp, string userID, string password, string DbName, out int AffectedID)
        {
            int tempDBRowID = DBRowID;
            AffectedID = 0;
            try
            {
                string query = string.Empty;
                if (AlreadySaved)
                {
                    query = $"update logsmigration set Server = '{serverIp}', UserId = '{userID}', Password = '{password}', DBName = '{DbName}' where Id = {tempDBRowID}";
                    //DBFactory.Insert(ConString, "insert into tbl_enroll (EMachineNumber, EnrollNumber, FingerNumber, Privilige, enPassword, EName, DptID) values (0, 1122, 11, 0, 1122, 'Testing', 0)");
                    bool updated = DBFactory.Update(ConString, query);
                    if (updated)
                    {
                        return "Updated";
                    }
                }
                else
                {
                    query = $"Insert into logsmigration (Server, UserId, Password, DBName) values ('{serverIp}', '{userID}', '{password}', '{DbName}')";
                    int AffectedRows = DBFactory.Insert(ConString, query);
                    if (AffectedRows == 1)
                    {
                        AffectedID = Convert.ToInt32(DBFactory.GetAllByQuery(ConString, "Select Id from LogsMigration").Rows[0]["Id"]);
                        return "1";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
                return "err";
            }
            return "0";
        }

        private string FindDbInstance(string serverIp, string userId, string password, string DbName)
        {
            string ConnectionString = $"Server={serverIp};User Id={userId};Password={password};";
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlDataAdapter sda = new SqlDataAdapter("Select name from sys.databases", con);
                    sda.Fill(dt);
                }
                //DataTable dt = DBFactory.GetAllByQuery(ConnectionString, "SELECT name FROM sys.databases;");
                List<string> dbNames = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    dbNames.Add(row["name"].ToString());
                }
                if (dbNames.Contains(DbName))
                {
                    string connectionString = $"Server={serverIp};Initial Catalog={DbName};User Id={userId};Password={password}";
                    bool dbConnected = ConnectServer(connectionString);
                    if (dbConnected)
                    {
                        return connectionString;
                    }
                    else
                    {
                        return "402";
                    }
                }
                else
                {
                    return "404";
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
                return "err";
            }
        }
        private bool ValidateSqlServerFields()
        {
            if (string.IsNullOrEmpty(txtServer.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Sql Server", false);
                msg.ShowDialog();
                txtServer.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtUserID.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter User ID", false);
                msg.ShowDialog();
                txtUserID.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPwd.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Password", false);
                msg.ShowDialog();
                txtPwd.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtDBName.Text))
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Please Enter Database Name", false);
                msg.ShowDialog();
                txtPwd.Focus();
                return false;
            }
            return true;
        }
        private bool ConnectServer(string ConnectionString, bool ShowMsg)
        {
            using (SqlConnection con = DBFactory.Connect(ConnectionString))
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    if (ShowMsg)
                    {
                        MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Success, "Server Connection Succeed", false);
                        msg.ShowDialog();
                    }
                    return true;
                }
                else
                {
                    if (ShowMsg)
                    {
                        MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Server Connection Failed!", false);
                        msg.ShowDialog();
                        return false;
                    }
                }
            }
            return false;
        }
        private bool ConnectServer(string ConnectionString)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    con.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public void EnableControls(bool flag)
        {
            txtServer.IsEnabled = flag;
            txtPwd.IsEnabled = flag;
            txtUserID.IsEnabled = flag;
            txtDBName.IsEnabled = flag;
            btnSaveConnection.IsEnabled = flag;

        }
        public void RefreshFields()
        {
            txtServer.Text = string.Empty;
            txtPwd.Text = string.Empty;
            txtUserID.Text = string.Empty;
            txtDBName.Text = string.Empty;
        }

        private void CreateConnection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AlreadySaved)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "Already Saved! You can update the setting.", false);
                msg.ShowDialog();
                return;
            }
            EnableControls(true);
            txtServer.Focus();
        }

        private void ModifyConnection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!AlreadySaved)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "Save Settings First.", false);
                msg.ShowDialog();
                return;
            }
            EnableControls(true);
            btnSaveConnection.Visibility = Visibility.Collapsed;
            btnUpdateConnection.Visibility = Visibility.Visible;
        }

        private void DeleteConnection_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!AlreadySaved)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Info, "Nothing to delete!", false);
                msg.ShowDialog();
                return;
            }
            MessageBoxWindow message1 = new MessageBoxWindow(MessageBoxType.Confirm, "Are you sure you want to delete the settings?", false);
            message1.ShowDialog();
            if (!message1.yes)
            {
                return;
            }
            string query = "Delete from LogsMigration";
            try
            {
                bool deleted = DBFactory.Delete(ConString, query);
                if (deleted)
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Success, "Deleted Successfully.", false);
                    msg.ShowDialog();
                    AlreadySaved = false;
                    DBRowID = 0;
                    EnableControls(false);
                    RefreshFields();
                    HideOverlays(true);
                }
                else
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Success, "Failed to Delete!", false);
                    msg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
            }

        }

        private void txtDBName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOverlayDBName.Visibility = Visibility.Collapsed;
        }

        private void txtDBName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDBName.Text))
            {
                txtOverlayDBName.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FetchSettingsIfSaved();
            if (AlreadySaved)
            {
                TargetConString = $"Server={txtServer.Text};Initial Catalog={txtDBName.Text};User Id={txtUserID.Text};Password={txtPwd.Text}";
                PopulateTableComboBox(TargetConString);
            }
            _comboBoxes = new List<ComboBox> { cmbId, cmbEMchNo, cmbEnrollNo, cmbTMchNo, cmbInOut, cmbVeriMode, cmbDateTime };
        }
        private void HideOverlays(bool flag)
        {
            Visibility vis = flag ? Visibility.Hidden : Visibility.Visible;
            txtOverlayServer.Visibility = vis;
            txtOverlayUserID.Visibility = vis;
            txtOverlayPwd.Visibility = vis;
            txtOverlayDBName.Visibility = vis;
        }
        private void FetchSettingsIfSaved()
        {
            try
            {
                string query = "select * from LogsMigration";
                DataTable dt = DBFactory.GetAllByQuery(ConString, query);
                if (dt.Rows.Count == 1)
                {
                    HideOverlays(true);
                    txtServer.Text = dt.Rows[0]["Server"].ToString();
                    txtUserID.Text = dt.Rows[0]["UserId"].ToString();
                    txtPwd.Text = dt.Rows[0]["Password"].ToString();
                    txtDBName.Text = dt.Rows[0]["DBName"].ToString();
                    AlreadySaved = true;
                    DBRowID = Convert.ToInt32(dt.Rows[0]["Id"]);

                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
            }
        }

        private void resetSettingFields_Click(object sender, MouseButtonEventArgs e)
        {
            if (AlreadySaved)
            {
                FetchSettingsIfSaved();
                HideOverlays(true);
            }
            else
            {
                RefreshFields();
                HideOverlays(false);
            }
            btnSaveConnection.Visibility = Visibility.Visible;
            btnUpdateConnection.Visibility = Visibility.Collapsed;
            EnableControls(false);
        }

        private void cmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTables.SelectedIndex != 0)
            {
                string tableName = cmbTables.SelectedItem.ToString();
                List<string> tblColumns = getTblColumns(TargetConString, tableName);
                if (tblColumns != null && tblColumns.Count > 0)
                {
                    RefreshColumnComboBoxes();
                    foreach (string item in tblColumns)
                    {
                        ComboBoxItem cmbItem1 = new ComboBoxItem();
                        cmbItem1.Content = item;
                        ComboBoxItem cmbItem2 = new ComboBoxItem();
                        cmbItem2.Content = item;
                        ComboBoxItem cmbItem3 = new ComboBoxItem();
                        cmbItem3.Content = item;
                        ComboBoxItem cmbItem4 = new ComboBoxItem();
                        cmbItem4.Content = item;
                        ComboBoxItem cmbItem5 = new ComboBoxItem();
                        cmbItem5.Content = item;
                        ComboBoxItem cmbItem6 = new ComboBoxItem();
                        cmbItem6.Content = item;
                        ComboBoxItem cmbItem7 = new ComboBoxItem();
                        cmbItem7.Content = item;
                        cmbId.Items.Add(cmbItem1);
                        cmbEMchNo.Items.Add(cmbItem2);
                        cmbEnrollNo.Items.Add(cmbItem3);
                        cmbTMchNo.Items.Add(cmbItem4);
                        cmbInOut.Items.Add(cmbItem5);
                        cmbVeriMode.Items.Add(cmbItem6);
                        cmbDateTime.Items.Add(cmbItem7);
                    }
                }
            }
        }
        private void RefreshColumnComboBoxes()
        {
            foreach (ComboBox cmb in _comboBoxes)
            {
                cmb.Items.Clear();
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "--- Select Column ---";
                item.IsEnabled = true;
                cmb.Items.Add(item);
                cmb.SelectedIndex = 0;
            }
        }
        private List<string> getTblColumns(string connectionString, string tableName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    //string query = $"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE,IS_IDENTITY, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                    string query = $"SELECT  c.name AS COLUMN_NAME, t.name AS DATA_TYPE, c.is_nullable AS IS_NULLABLE, c.max_length AS CHARACTER_MAXIMUM_LENGTH, c.is_identity AS IS_IDENTITY FROM  sys.columns c INNER JOIN  sys.types t ON c.user_type_id = t.user_type_id INNER JOIN sys.tables tbl ON c.object_id = tbl.object_id WHERE  tbl.name = '{tableName}'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            List<string> columns = new List<string>();
                            List<TableSchema> tblSchemas = new List<TableSchema>();
                            foreach (DataRow row in dt.Rows)
                            {
                                columns.Add($"{ row["COLUMN_NAME"]} ({row["DATA_TYPE"]})");
                                TableSchema obj = new TableSchema
                                {
                                    ColumnName = $"{ row["COLUMN_NAME"]} ({row["DATA_TYPE"]})",
                                    DataType = row["DATA_TYPE"].ToString(),
                                    IsIdentity = (bool)row["IS_IDENTITY"]

                                };
                                tblSchemas.Add(obj);
                            }
                            TableSchemas = new List<TableSchema>(tblSchemas);
                            return columns;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void Column_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentCmb = sender as ComboBox;
            if (_comboBoxes != null && currentCmb.Items != null && currentCmb.Items.Count > 1)
            {
                if (currentCmb.SelectedIndex != 0)
                {
                    // Get the selected ComboBoxItem
                    ComboBoxItem selectedItem = currentCmb.SelectedItem as ComboBoxItem;

                    // Ensure selectedItem is not null
                    if (selectedItem != null)
                    {
                        string selectedValue = selectedItem.Content.ToString();
                        LockedItems alreadyLocked = LockedCmbItems.FirstOrDefault(x => x.LockedBy == currentCmb.Name);
                        if (alreadyLocked != null)
                        {
                            LockedCmbItems.Remove(LockedCmbItems.FirstOrDefault(x => x.LockedBy == alreadyLocked.LockedBy));
                            UnlockPreviouslyLockedItem(currentCmb, alreadyLocked.ItemName, alreadyLocked.LockedBy);
                        }
                        LockedItems obj = new LockedItems
                        {
                            LockedBy = currentCmb.Name,
                            ItemName = selectedValue
                        };
                        LockedCmbItems.Add(obj);
                        foreach (ComboBox cmb in _comboBoxes)
                        {
                            if (cmb.Name == currentCmb.Name)
                            {
                                continue;
                            }

                            // Iterate over ComboBox items
                            foreach (var item in cmb.Items)
                            {
                                // Check if item is a ComboBoxItem
                                if (item is ComboBoxItem currentItem)
                                {
                                    string itemContent = currentItem.Content.ToString();

                                    // Check and handle special case for default item
                                    if (itemContent == "--- Select Column ---")
                                    {
                                        continue;
                                    }

                                    // Disable the item if it matches the selected value
                                    if (itemContent == selectedValue)
                                    {
                                        currentItem.IsEnabled = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Optionally, handle the case where no valid selection is made
                    if (currentCmb.SelectedIndex == 0)
                    {
                        LockedItems LockedItemByCurrentCmb = LockedCmbItems.FirstOrDefault(x => x.LockedBy == currentCmb.Name);
                        foreach (ComboBox cmb in _comboBoxes)
                        {
                            if (cmb.Name == currentCmb.Name)
                            {
                                //LockedItemByCurrentCmb = LockedCmbItems.FirstOrDefault(x => x.LockedBy == cmb.Name);
                                continue;
                            }
                            foreach (var item in cmb.Items)
                            {
                                if (item is ComboBoxItem currentItem)
                                {
                                    string itemContent = currentItem.Content.ToString();
                                    if (itemContent == LockedItemByCurrentCmb.ItemName)
                                    {
                                        currentItem.IsEnabled = true;
                                    }
                                }
                            }
                        }
                        LockedCmbItems.Remove(LockedCmbItems.FirstOrDefault(x => x.LockedBy == LockedItemByCurrentCmb.LockedBy && x.ItemName == LockedItemByCurrentCmb.ItemName));
                    }
                }
            }
        }

        private void UnlockPreviouslyLockedItem(ComboBox currentCmb, string LockedItem, string LockedBy)
        {
            foreach (ComboBox cmb in _comboBoxes)
            {
                if (currentCmb.Name == cmb.Name)
                {
                    continue;
                }
                foreach (var item in cmb.Items)
                {
                    if (item is ComboBoxItem currentItem)
                    {
                        string currentVal = currentItem.Content.ToString();
                        if (currentVal == LockedItem)
                        {
                            currentItem.IsEnabled = true;
                        }
                    }
                }
            }
        }

        private void btnSaveBinding_Click(object sender, RoutedEventArgs e)
        {
            string tblName = cmbTables.SelectedItem.ToString();
            string IdBinding;
            string EnrollNoBinding;
            string EMchNoBinding;
            string TMchNoBinding;
            string VeriModeBinding;
            string DateTimeBinding;
            string InOutBinding;
            bool ColumnsValidated = ValidateColumnBindings(out IdBinding, out EMchNoBinding, out TMchNoBinding, out VeriModeBinding, out InOutBinding, out DateTimeBinding, out EnrollNoBinding);
            if (!ColumnsValidated)
            {
                return;
            }
            try
            {
                string query = $"Update LogsMigration set TableName = '{tblName}', ID_Binding = '{IdBinding}', TMchNo_Binding = '{TMchNoBinding}', EnrollNo_Binding = '{EnrollNoBinding}', EMchNo_Binding = '{EMchNoBinding}', InOut_Binding = '{InOutBinding}', VeriMode_Binding = '{VeriModeBinding}', DateTime_Binding = '{DateTimeBinding}' where Id = {DBRowID} ";
                bool updated = DBFactory.Update(ConString, query);
                if (updated)
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Success, "Bindings Saved Successfully.", false);
                    msg.ShowDialog();
                }
                else
                {
                    MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "Bindings not Saved.", false);
                    msg.ShowDialog();
                }
            }
            catch(Exception ex)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Error, $"Error Occured\n{ex.Message}", false);
                msg.ShowDialog();
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {

        }
        private bool ValidateColumnBindings(out string id, out string EMchNo, out string TMchNo, out string VeriMode, out string InOut, out string DateTime, out string EnrollNo)
        {
            id = string.Empty;
            EMchNo = string.Empty;
            TMchNo = string.Empty;
            VeriMode = string.Empty;
            InOut = string.Empty;
            DateTime = string.Empty;
            EnrollNo = string.Empty;

            if (cmbId.SelectedIndex == 0 || cmbEMchNo.SelectedIndex == 0 || cmbEnrollNo.SelectedIndex == 0 || cmbTMchNo.SelectedIndex == 0 || cmbInOut.SelectedIndex == 0 || cmbVeriMode.SelectedIndex == 0 || cmbDateTime.SelectedIndex == 0)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "Please Select All the Bindings", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbIdSelectedItem = cmbId.SelectedItem as ComboBoxItem;
            TableSchema cmb_IdSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbIdSelectedItem.Content.ToString());
            id = cmbIdSelectedItem.Content.ToString().Split(' ')[0];
            if (cmb_IdSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "ID Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmb_IdSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "ID Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbTMchNoSelectedItem = cmbTMchNo.SelectedItem as ComboBoxItem;
            TableSchema cmbTMchNoSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbTMchNoSelectedItem.Content.ToString());
            TMchNo = cmbTMchNoSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbTMchNoSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "TMchNo Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbTMchNoSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "TMchNo Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbEnrollNoSelectedItem = cmbEnrollNo.SelectedItem as ComboBoxItem;
            TableSchema cmbEnrollNoSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbEnrollNoSelectedItem.Content.ToString());
            EnrollNo = cmbEnrollNoSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbEnrollNoSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "EnrollNo Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbEnrollNoSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "EnrollNo Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbEMchNoSelectedItem = cmbEMchNo.SelectedItem as ComboBoxItem;
            TableSchema cmbEMchNoSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbEMchNoSelectedItem.Content.ToString());
            EMchNo = cmbEMchNoSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbEMchNoSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "EMchNo Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbEMchNoSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "EMchNo Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbInOutSelectedItem = cmbInOut.SelectedItem as ComboBoxItem;
            TableSchema cmbInOutSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbInOutSelectedItem.Content.ToString());
            InOut = cmbInOutSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbInOutSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "InOut Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbInOutSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "InOut Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbVeriModeSelectedItem = cmbVeriMode.SelectedItem as ComboBoxItem;
            TableSchema cmbVeriModeSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbVeriModeSelectedItem.Content.ToString());
            VeriMode = cmbVeriModeSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbVeriModeSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "VeriMode Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbVeriModeSchema.DataType != "int")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "VeriMode Binding Must Be Integer Type.", false);
                msg.ShowDialog();
                return false;
            }
            ComboBoxItem cmbDateTimeSelectedItem = cmbDateTime.SelectedItem as ComboBoxItem;
            TableSchema cmbDateTimeSchema = TableSchemas.FirstOrDefault(x => x.ColumnName == cmbDateTimeSelectedItem.Content.ToString());
            DateTime = cmbDateTimeSelectedItem.Content.ToString().Split(' ')[0];
            if (cmbDateTimeSchema.IsIdentity)
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "DateTime Binding Must Not Be Identity.", false);
                msg.ShowDialog();
                return false;
            }
            if (cmbDateTimeSchema.DataType != "datetime")
            {
                MessageBoxWindow msg = new MessageBoxWindow(MessageBoxType.Alert, "DateTime Binding Must Be DateTime Type.", false);
                msg.ShowDialog();
                return false;
            }

            return true;
        }
    }
}
public class TableSchema
{
    public string ColumnName { get; set; }
    public string DataType { get; set; }
    public bool IsIdentity { get; set; }
}
public class LockedItems
{
    public string LockedBy { get; set; }
    public string ItemName { get; set; }
}
