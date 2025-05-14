using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace FaceAttendance.Classes
{
    public class ImageOperations
    {
        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray));

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return new Bitmap(ms);
            }
        }
        public static byte[] GetImageDataFromDatabase(string connectionString, int userEnrollNumber)
        {
            string query = "SELECT Photo FROM tbl_enroll WHERE EnrollNumber = @EnrollNumber and FingerNumber = 50";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EnrollNumber", userEnrollNumber);

                try
                {
                    connection.Open();
                    return (byte[])command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error retrieving image data: " + ex.Message);
                    return null;
                }
            }
        }
        public static int UpdateImage(string connectionString, int User_ID, byte[] imgToSave)
        {
            string updateQuery = "UPDATE tbl_enroll SET Photo = @Photo Where FingerNumber = 50 AND EnrollNumber = @EnrollNumber ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);

                command.Parameters.AddWithValue("@EnrollNumber", User_ID);
                command.Parameters.Add("@Photo", SqlDbType.VarBinary, -1).Value = imgToSave;

                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting data: " + ex.Message);
                    return 0;
                }
            }
        }
        public static async Task<int> UpdateImageAsync(string connectionString, int User_ID, byte[] imgToSave)
        {
            string updateQuery = "UPDATE tbl_enroll SET Photo = @Photo Where FingerNumber = 50 AND EnrollNumber = @EnrollNumber ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);

                command.Parameters.AddWithValue("@EnrollNumber", User_ID);
                command.Parameters.Add("@Photo", SqlDbType.VarBinary, -1).Value = imgToSave;

                try
                {
                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting data: " + ex.Message);
                    return 0;
                }
            }
        }
        public static int InsertImage(string connectionString, int deviceID, int User_ID, int admin, string UserName, byte[] imgToSave, string dptID)
        {
            string facequery = "INSERT INTO tbl_enroll (EMachineNumber, EnrollNumber, FingerNumber, Privilige, enPassword, EName, FPData, Photo, DptID) " +
                               "VALUES (@EMachineNumber, @EnrollNumber, @FingerNumber, @Privilige, @enPassword, @EName, @FPData, @Photo, @DptID)";
            if(dptID == null)
            {
                dptID = "0";
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(facequery, connection);
                command.Parameters.AddWithValue("@EMachineNumber", deviceID);
                command.Parameters.AddWithValue("@EnrollNumber", User_ID);
                command.Parameters.AddWithValue("@FingerNumber", 50);
                command.Parameters.AddWithValue("@Privilige", admin);
                command.Parameters.AddWithValue("@enPassword", 0);
                command.Parameters.AddWithValue("@EName", UserName);
                command.Parameters.AddWithValue("@FPData", "Empty");
                command.Parameters.Add("@Photo", SqlDbType.VarBinary, -1).Value = imgToSave;
                command.Parameters.AddWithValue("@DptID", dptID);

                try
                {
                    connection.Open();
                    return command.ExecuteNonQuery(); // Return the number of rows affected
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting data: " + ex.Message);
                    return 0;
                }
            }
        }
        public static byte[] BitmapToByteArray(Bitmap img)
        {
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                imageData = ms.ToArray();
            }
            return imageData;
        }
    }
}
