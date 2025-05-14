using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;


namespace UsmanCodeBlocks.Data.Sql.Local
{
    public class DBFactory
    {
        static SqlConnection objConnection = new SqlConnection();
        public static bool ConnectServer(string connString)
        {


            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    if (objConnection.State.ToString() == "Closed")
                    {
                        objConnection = Connect(connString);
                        return true;
                    }
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }

        }
        public static async Task<bool> ConnectServerAsync(string connString)
        {


            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    if (objConnection.State.ToString() == "Closed")
                    {
                        objConnection = await ConnectAsync(connString);
                        return true;
                    }
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }

        }

        public static SqlConnection Connect(string connString)
        {
            try
            {
                objConnection = new SqlConnection(connString);
                objConnection.Open();
                return objConnection;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<SqlConnection> ConnectAsync(string connString)
        {
            try
            {
                objConnection = new SqlConnection(connString);
                await objConnection.OpenAsync();
                return objConnection;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool DisconnectServer()
        {
            if (objConnection.State != ConnectionState.Closed)
                objConnection.Close();

            if (objConnection.State == ConnectionState.Closed)
                return true;
            else
                return false;
        }

        public static bool Disconnect(SqlConnection con)
        {
            if (con.State != ConnectionState.Closed)
                con.Close();

            if (con.State == ConnectionState.Closed)
                return true;
            else
                return false;
        }

        public static bool BulkInsert(string connString, string query)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            SqlCommand cmd = new SqlCommand(query, objConnection);
            return Convert.ToBoolean(cmd.ExecuteNonQuery());
        }

        public static DataTable GetTableSchema(string connString, string tableName)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from [" + tableName + "]", objConnection);
            SqlDataReader reader = cmd.ExecuteReader();
            dt = reader.GetSchemaTable();
            reader.Close();

            return dt;
        }


        public static DataTable GetTableSingleColumn(string connString, string colName, string tableName)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select [" + colName + "] from [" + tableName + "]", objConnection);
            SqlDataReader reader = cmd.ExecuteReader();
            dt = reader.GetSchemaTable();
            reader.Close();

            return dt;
        }



        public static int AddPhoto(string connString, string tableName,string fieldName, Byte[] imgData, string idField, int idValue)
        {
            if(objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            string query = "UPDATE [{0}] SET {1}=@img WHERE {2}={3}";
            query = string.Format(query,tableName, fieldName, idField,idValue);
            SqlParameter parameter = new SqlParameter("@img", imgData);
            parameter.SqlDbType = SqlDbType.VarBinary;

            SqlCommand cmd = new SqlCommand(query, objConnection);
            cmd.Parameters.Add(parameter);
            return cmd.ExecuteNonQuery();



        }
        public static Byte[] GetPhoto(string connString, string tableName, string fieldName, string idField, int idValue)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            try
            {
                string query = "SELECT {0} FROM [{1}] WHERE {2} = {3}";
                query = string.Format(query, fieldName, tableName,idField,idValue);
                SqlCommand cmd = new SqlCommand(query, objConnection);
                object o = cmd.ExecuteScalar();
                if (o != null)
                {
                    return (Byte[])cmd.ExecuteScalar();
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static int ClearPhoto(string connString,string tableName, string fieldName, string idField, int idValue)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            string query = "UPDATE [{0}] SET{1}=NULL WHERE {2}={3}";
            query = string.Format(query, tableName, fieldName,idField,idValue);
            SqlCommand cmd = new SqlCommand(query, objConnection);
            return cmd.ExecuteNonQuery();
        }

        public static object Insert(string connString, Object obj, bool returnID)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            Type t = obj.GetType();
            
            string query = "INSERT INTO [<TABLENAME>](<FIELDS>) VALUES(<VALUES>)";
            DataTable dt = GetTableSchema(connString, t.Name);
            query = query.Replace("<TABLENAME>", t.Name);

            string fields = "";
            string values = "";
            string idfield = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row["datatypename"].ToString().ToLower() != "varbinary")
                {

                    if (Convert.ToBoolean(row["isidentity"]))
                        idfield = row["columnname"].ToString();

                    if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                    {
                        fields += row["columnname"].ToString() + ",";

                        PropertyInfo property = t.GetProperty(row["columnname"].ToString());

                        if (row["datatypename"].ToString().ToLower() == "datetime")
                        {
                            if (property.GetValue(obj, null) != null)
                                values += "'" + Convert.ToDateTime(property.GetValue(obj, null)).ToString("dd-MMM-yyyy HH:mm:ss") + "',";
                            else
                                values += "NULL,";
                        }
                        else if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "nvarchar")
                            values += "'" + property.GetValue(obj, null) + "',";
                        else
                        {
                            if (row["datatypename"].ToString().ToLower() == "bit")
                                values += Convert.ToInt32(property.GetValue(obj, null)) + ",";
                            else
                                values += property.GetValue(obj, null) + ",";
                        }
                    }
                }
            }

            fields = fields.Remove(fields.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            query = query.Replace("<FIELDS>", fields);
            query = query.Replace("<VALUES>", values);

            if (returnID)
            {
                query += ";SELECT @@IDENTITY";
                SqlCommand cmd = new SqlCommand(query, objConnection);
                return cmd.ExecuteScalar();
            }
            else
            {
                SqlCommand cmd = new SqlCommand(query, objConnection);
                return cmd.ExecuteScalar();
            }
        }

        public static string GetInsertString(string connString, Object obj, bool returnID)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            Type t = obj.GetType();

            string query = "INSERT INTO [<TABLENAME>](<FIELDS>) VALUES(<VALUES>)";
            DataTable dt = GetTableSchema(connString, t.Name);
            query = query.Replace("<TABLENAME>", t.Name);

            string fields = "";
            string values = "";
            string idfield = "";
            foreach (DataRow row in dt.Rows)
            {
                if (row["datatypename"].ToString().ToLower() != "varbinary")
                {

                    if (Convert.ToBoolean(row["isidentity"]))
                        idfield = row["columnname"].ToString();

                    if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                    {
                        fields += row["columnname"].ToString() + ",";

                        PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                        if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "datetime" || row["datatypename"].ToString().ToLower() == "nvarchar")
                            values += "'" + property.GetValue(obj, null) + "',";
                        else
                        {
                            if (row["datatypename"].ToString().ToLower() == "bit")
                                values += Convert.ToInt32(property.GetValue(obj, null)) + ",";
                            else
                                values += property.GetValue(obj, null) + ",";
                        }
                    }
                }
            }

            fields = fields.Remove(fields.Length - 1, 1);
            values = values.Remove(values.Length - 1, 1);

            query = query.Replace("<FIELDS>", fields);
            query = query.Replace("<VALUES>", values);

            return query;
        }

        public static int Insert(string connString, string insertQuery)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            SqlCommand cmd = new SqlCommand(insertQuery, objConnection);
            int ret =  cmd.ExecuteNonQuery();

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return ret;
        }
        public static async Task<int> InsertAsync(string connString, string insertQuery)
        {
            if (objConnection.State != ConnectionState.Open)
                await ConnectServerAsync(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            SqlCommand cmd = new SqlCommand(insertQuery, objConnection);
            int ret = await cmd.ExecuteNonQueryAsync();

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return ret;
        }
        public static int Insert(string insertQuery, SqlConnection con, SqlTransaction trans) //overloaded for the purpose of transaction in ucTimeSlots
        {

            int ret = -1;
            if (con.State == ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand(insertQuery, con, trans);
                ret = cmd.ExecuteNonQuery();
            }
            return ret;
        }

        public static bool Update(string connString, Object obj)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            Type t = obj.GetType();
            string query = "UPDATE <TABLENAME> SET <PARAMS> WHERE <IDFIELD> = <VALUE>";
            DataTable dt = GetTableSchema(connString, t.Name);

            query = query.Replace("<TABLENAME>", t.Name);
            string parameters = "";
            string idfield = "";
            string idvalue = "";

            foreach (DataRow row in dt.Rows)
            {
                if (row["columnname"].ToString() != "Photo")
                {
                    if (Convert.ToBoolean(row["isidentity"]))
                        idfield = row["columnname"].ToString();

                    if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                    {
                        PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                        if (property.GetValue(obj, null) != null)
                        {
                            if (row["datatypename"].ToString().ToLower() == "datetime")
                            {
                                string strVal = null;
                                if (property.GetValue(obj, null) != null)
                                {
                                    DateTime val = Convert.ToDateTime(property.GetValue(obj, null).ToString());
                                    strVal = val.ToString("dd-MMM-yyyy HH:mm:ss");
                                }
                                
                                parameters += row["columnname"].ToString() + " = '" + strVal + "', ";
                            }
                            else if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "nvarchar")
                                parameters += row["columnname"].ToString() + " = '" + property.GetValue(obj, null).ToString() + "', ";
                            else if (row["datatypename"].ToString().ToLower() == "bit")
                                parameters += row["columnname"].ToString() + " = " + Convert.ToInt32(property.GetValue(obj, null)) + ", ";
                            else
                                parameters += row["columnname"].ToString() + " = " + property.GetValue(obj, null) + ", ";
                        }
                    }
                }
            }

            idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            parameters = parameters.Trim();
            parameters = parameters.Remove(parameters.Length - 1, 1);
            query = query.Replace("<PARAMS>", parameters);
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", idvalue);

            SqlCommand cmd = new SqlCommand(query, objConnection);
            return Convert.ToBoolean(cmd.ExecuteNonQuery());
        }

        public static string GetUpdateString(string connString, Object obj)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return "";

            Type t = obj.GetType();
            string query = "UPDATE <TABLENAME> SET <PARAMS> WHERE <IDFIELD> = <VALUE>";
            DataTable dt = GetTableSchema(connString, t.Name);

            query = query.Replace("<TABLENAME>", t.Name);
            string parameters = "";
            string idfield = "";
            string idvalue = "";

            foreach (DataRow row in dt.Rows)
            {
                if (row["columnname"].ToString() != "Photo")
                {
                    if (Convert.ToBoolean(row["isidentity"]))
                        idfield = row["columnname"].ToString();

                    if (!Convert.ToBoolean(row["IsAutoIncrement"]))
                    {
                        PropertyInfo property = t.GetProperty(row["columnname"].ToString());
                        if (property.GetValue(obj, null) != null)
                        {
                            if (row["datatypename"].ToString().ToLower() == "varchar" || row["datatypename"].ToString().ToLower() == "datetime")
                                parameters += row["columnname"].ToString() + " = '" + property.GetValue(obj, null).ToString() + "', ";
                            else
                            {
                                if (row["datatypename"].ToString().ToLower() == "bit")
                                    parameters += row["columnname"].ToString() + " = " + Convert.ToInt32(property.GetValue(obj, null)) + ", ";
                                else
                                    parameters += row["columnname"].ToString() + " = " + property.GetValue(obj, null) + ", ";
                            }
                        }
                    }
                }
            }

            idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            parameters = parameters.Trim();
            parameters = parameters.Remove(parameters.Length - 1, 1);
            query = query.Replace("<PARAMS>", parameters);
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", idvalue);

            return query;
        }

        public static bool Update(string connString, string updateQuery)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            SqlCommand cmd = new SqlCommand(updateQuery, objConnection);

            bool rec = Convert.ToBoolean(cmd.ExecuteNonQuery());

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return rec;
        }
        public static async Task<bool> UpdateAsync(string connString, string updateQuery)
        {
            if (objConnection.State != ConnectionState.Open)
                await ConnectServerAsync(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            SqlCommand cmd = new SqlCommand(updateQuery, objConnection);

            bool rec = Convert.ToBoolean(await cmd.ExecuteNonQueryAsync());

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return rec;
        }

        public static bool Update(string updateQuery, SqlConnection con, SqlTransaction trans)//overloaded to use with the transaction happening in the ucTimeSlots
        {
            bool rec = false;
            if (con.State == ConnectionState.Open) {
                SqlCommand cmd = new SqlCommand(updateQuery, con, trans);
                rec = Convert.ToBoolean(cmd.ExecuteNonQuery());
                
            }
            return rec;
        }

        public static bool Delete(string connString, Object obj)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            Type t = obj.GetType();
            string query = "DELETE FROM <TABLENAME> WHERE <IDFIELD> = <VALUE>";
            DataTable dt = GetTableSchema(connString, t.Name);
            query = query.Replace("<TABLENAME>", t.Name);
            string idfield = "";
            string idvalue = "";
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["isidentity"]))
                {
                    idfield = row["columnname"].ToString();
                    break;
                }
            }

            idvalue = t.GetProperty(idfield).GetValue(obj, null).ToString();
            query = query.Replace("<IDFIELD>", idfield);
            query = query.Replace("<VALUE>", idvalue);

            SqlCommand cmd = new SqlCommand(query, objConnection);

            bool ret = Convert.ToBoolean(cmd.ExecuteNonQuery());

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return ret;
        }

        public static bool Delete(string connString, string query)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return false;

            SqlCommand cmd = new SqlCommand(query, objConnection);
            bool ret = Convert.ToBoolean(cmd.ExecuteNonQuery());

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return ret;
        }

        public static DataTable GetAll(string connString, string tableName)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            DataTable dt = GetTableSchema(connString, tableName);
            string query = "SELECT * FROM [" + tableName + "] ORDER BY <idfield>";
            string idfield = "";
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["isidentity"]))
                {
                    idfield = row["columnname"].ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(idfield))
                idfield = dt.Rows[0]["columnname"].ToString();

            query = query.Replace("<idfield>", idfield);

            SqlDataAdapter adapter = new SqlDataAdapter(query, objConnection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
        public static DataTable GetAll(string connString, string tableName, List<string> excludedColumns)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            string cols = "";
            string idfield = "";
            DataTable dt = GetTableSchema(connString, tableName);
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["isidentity"]))
                {
                    idfield = row["columnname"].ToString();
                }
                if (!excludedColumns.Contains(row["columnname"].ToString()))
                    cols += row["columnname"].ToString() + ",";
            }
            cols = cols.Remove(cols.Length - 1, 1);

            string query = "SELECT {0} FROM [" + tableName + "] ORDER BY <idfield>";
            query = string.Format(query, cols);

            
            if (string.IsNullOrEmpty(idfield))
                idfield = dt.Rows[0]["columnname"].ToString();

            query = query.Replace("<idfield>", idfield);

            SqlDataAdapter adapter = new SqlDataAdapter(query, objConnection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return table;
        }

        public static DataTable GetAllByQuery(string connString, string query)
        {
            objConnection = new SqlConnection();

            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            SqlDataAdapter adapter = new SqlDataAdapter(query, objConnection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            //connection Close
            if (objConnection.State == ConnectionState.Open)
                objConnection.Close();

            return table;
        }
        public static async Task<DataTable> GetAllByQueryAsync(string connString, string query)
        {
            objConnection = new SqlConnection();
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);
            if (objConnection.State != ConnectionState.Open)
                return null;
            SqlDataAdapter adapter = new SqlDataAdapter(query, objConnection);
            DataTable table = new DataTable();
            await Task.Run(() => adapter.Fill(table));
            return table;
        }

        public static DataTable GetByID(string connString, string tableName, int id)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return null;

            DataTable dt = GetTableSchema(connString, tableName);
            string query = "SELECT * FROM [" + tableName + "] WHERE <idfield> = <idvalue>";
            string idfield = "";
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["isidentity"]))
                {
                    idfield = row["columnname"].ToString();
                    break;
                }
            }

            query = query.Replace("<idfield>", idfield);
            query = query.Replace("<idvalue>", id.ToString());

            SqlDataAdapter adapter = new SqlDataAdapter(query, objConnection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public static int GetRecCount(string connString, string tableName)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            string query = "SELECT COUNT(*) FROM [" + tableName + "]";
            SqlCommand cmd = new SqlCommand(query,objConnection);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count;
        }

        public static int GetRecCountByQuery(string connString, string query)
        {
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            SqlCommand cmd = new SqlCommand(query, objConnection);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count;
        }

        public static int GetMaxID(string connString, string tableName)
        {
            string query = " SELECT max(id) FROM [" + tableName + "]";
            if (objConnection.State != ConnectionState.Open)
                ConnectServer(connString);

            if (objConnection.State != ConnectionState.Open)
                return 0;

            int count = GetRecCount(connString, tableName);
            if (count >= 1)
            {
                SqlCommand cmd = new SqlCommand(query, objConnection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
                return count;

        }

    }
}
