using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace FaceAttendance.Classes
{
    public class Globals
    {
        private static string _connectionString = null;
        private static string serPath = null;

        private static string portNo;
        public static string FirebaseUsername = string.Empty;
        public static string FirebasePassword = string.Empty;
        public static string UserID { get; set; }
        public static string UserPassword { get; set; }
        public static string Domain { get; set; }

        public static string ServicePath
        {
            get
            {
                if (serPath == null)
                    serPath = GetServPath();
                return serPath;
            }
        }
        private static string GetServPath()
        {
            string path = "";
            string file = Directory.GetCurrentDirectory() + "\\Path.txt";
            if (File.Exists(file))
            {
                path = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Path.txt");
            }

            return path;
        }
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                    _connectionString = GetConnectionString();
                return _connectionString;
            }
        }
        public static int PortNo
        {
            get
            {
                if (portNo == null)
                {
                    portNo = GetPortNo();

                }
                return Int16.Parse(portNo);
            }
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        //public static T GetItem<T>(DataRow dr)
        //{
        //    Type temp = typeof(T);
        //    T obj = Activator.CreateInstance<T>();

        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        foreach (PropertyInfo pro in temp.GetProperties())
        //        {
        //            if (pro.Name.ToLower() == column.ColumnName.ToLower())
        //            {
        //                if (!dr.IsNull(column.ColumnName))


        //                    pro.SetValue(obj, dr[column.ColumnName], null);
        //            }
        //            else
        //                continue;
        //        }
        //    }
        //    return obj;
        //}
        public static T GetItem<T>(DataRow dr)
        {
            Type targetType = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                string columnName = column.ColumnName.ToLower();
                PropertyInfo property = targetType.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    object columnValue = dr[columnName];

                    if (!Convert.IsDBNull(columnValue))
                    {
                        // Check if the types are compatible before setting the value
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(obj, Convert.ToString(columnValue), null);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(obj, Convert.ToInt32(columnValue), null);
                        }
                        else
                        {
                            // Handle unsupported types or conversion issues
                            // You may want to log or throw an exception here
                        }
                    }
                }
            }

            return obj;
        }
        public static string PSDomain { get; internal set; }
        private static string GetPortNo()
        {
            string portNo = "";
            string file = Directory.GetCurrentDirectory() + "\\DispenserPort.txt";
            if (File.Exists(file))
            {
                portNo = File.ReadAllText(Directory.GetCurrentDirectory() + "\\DispenserPort.txt");
            }
            return portNo;
         }
        private static string GetConnectionString()
        {
            string connectionString = "";
            string file = Directory.GetCurrentDirectory() + "\\dbconfig.txt";
            if (File.Exists(file))
            {
                connectionString = File.ReadAllText(Directory.GetCurrentDirectory() + "\\dbconfig.txt");
            }

            return connectionString;
        }

       

       

       

        
    }
}
