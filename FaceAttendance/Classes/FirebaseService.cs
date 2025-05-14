using Firebase.Database;
using Firebase.Database.Query;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using HIKVISION;
using System.Windows;

namespace FaceAttendance.Classes
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebaseClient;
        public bool ConnectivityIssue = false;

        public FirebaseService()
        {
            try
            {
                            _firebaseClient = new FirebaseClient(
                "https://photo-enroll-73d3d-default-rtdb.firebaseio.com/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult("8iVlTv1dcx9VRBn5WhG7OVYo3gBYJJrE1gCKbfLd")
                });
            }
            catch(Exception ex) {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Firebase Error, Check Internet or Contact DDS.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                string path = Directory.GetCurrentDirectory() + "\\ErrorLogs.txt";
                string errmessage = "( " + DateTime.Now + " ) ( FirebaseService()  ) ( " + ex.ToString() + " )" + Environment.NewLine;
                File.AppendAllText(path, errmessage);                
            }

        }
        public async Task<bool> IsConnected()
        {
            try
            {
                var loginDate = await _firebaseClient
                    .Child("logins")
                    .OnceSingleAsync<Login>();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            try
            {
                var loginData = await _firebaseClient
                .Child("logins")
                .Child(username)
                .OnceSingleAsync<Login>();

                if (loginData == null)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Firebase User Doesn't Exist", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                if (loginData != null && loginData.password != password)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Firebase Password doesn't match", false);
                    message.btnCancel.Visibility = Visibility.Collapsed;
                    message.btnOk.Content = "Ok";
                    message.ShowDialog();
                }
                if (loginData != null && loginData.password == password)
                {
                    //MessageBox.Show("Firebase Connection Successfull");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Alert, "Firebase Connectivity Error, Check Internet or Contact DDS.", false);
                message.btnCancel.Visibility = Visibility.Collapsed;
                message.btnOk.Content = "Ok";
                message.ShowDialog();
                return false;
            }            
        }

        public async Task<List<ImageUpload>> GetImageUrlsByUsername(string username)
        {
            var images = await _firebaseClient
                .Child("uploads")
                .Child(username)
                .OnceAsync<ImageUpload>();

            List<string> imageUrls = new List<string>();
            List<ImageUpload> imagesUpload = new List<ImageUpload>();
            foreach (var image in images)
            {
                ImageUpload newImage = new ImageUpload();
                newImage.Id = image.Object.Id;
                newImage.Url = image.Object.Url;
                //imageUrls.Add(image.Object.Url);
                imagesUpload.Add(newImage);
            }

            return imagesUpload;
        }

        public async Task<byte[]> DownloadImage(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(imageUrl);

                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                    //File.WriteAllBytes($"I:\\Fahad\\{savePath}.jpg", imageBytes);
                    return imageBytes;
                    //await File.WriteAllBytesAsync(savePath, imageBytes);
                }
                else
                {
                    throw new Exception("Failed to download image");
                }
            }
        }
    }
}
public class ImageUpload
{
    public string Id { get; set; }
    public string Url { get; set; }
}
public class Login
{

    public string password { get; set; }
    public string username { get; set; }

}