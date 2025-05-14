using FaceAttendance.Classes;
using FaceAttendance.UserControls;
using HIKVISION;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FaceAttendance
{

    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window
    {
        int x = 1;
        string capImgPath;
        private ucAddStaff ocrWin;
        public test(ucAddStaff oCR)
        {
            InitializeComponent();
            ocrWin = oCR;
        }


        public static ImageSource ToImageSource(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        private void Exit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        public byte[] ImageConv(BitmapSource bitmapSource)
        {
            var stream = new MemoryStream();
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
            byte[] buffer = stream.GetBuffer();
            return buffer;
        }
        public void closeaftCap()
        {
            if (image.Source != null)
            {
                bool isCaptured = false;
                //while (!isCaptured)
                //{
                //capImgPath = @"C:\\PHOTO\" + Guid.NewGuid() + "CamCap.jpg";

                //capImgPath = "./" + x + "CamCap.jpg";
                ocrWin.capImg.Source = image.Source;
                ImageSource imageSource = ocrWin.capImg.Source;
                BitmapSource bmpSrc = (BitmapSource)imageSource;
                //var stream = new MemoryStream();
                //var encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(bmpSrc));
                //encoder.Save(stream);
                //byte[] buffer = stream.GetBuffer();
                byte[] buffer = ImageConv(bmpSrc);
                Bitmap bmp = new System.Drawing.Bitmap(new MemoryStream(buffer));
                //Bitmap bmp = BitmapSourceToBitmap2(bmpSrc);
                if (ocrWin.capImg.Source != null)
                {
                    //bmp.Save(capImgPath);
                    ocrWin.UserImage = (Bitmap)bmp.Clone();
                    isCaptured = true;
                    if (isCaptured)
                    {
                        //ocrWin.CapImgPath = capImgPath;
                        Close();
                        ocrWin.ocrCapBtn.Content = "Retake";
                        ocrWin.RetakeClicked = true;
                        ocrWin.clearImage.IsEnabled = true;
                        ocrWin.ClearImageClicked = false;
                        ocrWin.capImg.Width = 200;
                        ocrWin.capImg.Height = 150;
                    }

                }

            }
            //ocrWin.capImg.
            //Bitmap bmp = new Bitmap(ocrWin.capImg.ToString());
            //bmp.Save(capImgPath);
            //string capImg = ocrWin.capImg.Source.ToString();
            //Bitmap bmp = new Bitmap(capImg);
            //bmp.Save(System.Windows.Forms.Application.StartupPath + "/Faces/face" + x + ".bmp");
            // Cache.imgsource = image.Source;

            //ocrWin.Show();


        }
        public static Bitmap BitmapSourceToBitmap2(BitmapSource srs)
        {
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
                {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new System.Drawing.Bitmap(btm);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }
        private void Mini_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClearImg(object sender, RoutedEventArgs e)
        {
            image.Source = null;
        }

        private void btnSavePiv(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get a gray frame from capture device
                //while (mAutoStarted)
                //{
                //string capImgPath = "./" + "CamCap.jpg";
                //grabber = new Emgu.CV.Capture();
                //grabber.QueryFrame();
                //currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                ////Convert it to Grayscale
                //gray = currentFrame.Convert<Gray, Byte>();
                //gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //Bitmap bmp = gray.ToBitmap();
                //var srcImg = ToImageSource(bmp);
                //gray.Save(capImgPath);
                //gray.Save(capImgPath);
                //var camPath = Directory.GetCurrentDirectory() + "\\CamCap.jpg";
                //imageBox1.Source = new ImageSourceConverter().ConvertFromString(capImgPath) as ImageSource;
                //imageBox1.Source = srcImg;
                // b++;
                //break;
                // }


            }
            catch
            {

            }
        }
        List<string> cameras = new List<string>
        {
            "USB2.0 Camera",
            "A4tech PC Camera"
        };
        string selectedCamera = string.Empty;
        private void testWinLoad(object sender, RoutedEventArgs e)
        {
            try
            {

                var devices = UsbCamera.FindDevices();
                if (devices.Length == 0)
                {
                    MessageBoxWindow message = new MessageBoxWindow(MessageBoxType.Error, "Camera Device not Detected", false);
                    message.ShowDialog();
                    return;
                }// no device.
                int cameraIndex = 0;
                // get video format.
                for (int i = 0; i < devices.Length; i++)
                {
                    foreach (string camera1 in cameras)
                    {
                        if (devices[i] == camera1)
                        {
                            selectedCamera = camera1;
                            cameraIndex = i;
                        }
                    }
                    //if (devices[i] == "USB2.0 Camera")
                    //    cameraIndex = i;
                }

                var formats = UsbCamera.GetVideoFormat(cameraIndex);

                // select the format you want.
                foreach (var item in formats) Console.WriteLine(item);
                dynamic format = string.Empty;
                if (selectedCamera == "A4tech PC Camera")
                {
                    format = formats[4];
                }
                else
                {
                    format = formats[0];
                }

                // create instance.
                var camera = new UsbCamera(cameraIndex, format);
                // this closing event hander make sure that the instance is not subject to garbage collection.
                this.Closing += (s, ev) => camera.Release(); // release when close.

                // show preview on control. (works light.)
                // SetPreviewControl requires window handle but WPF control does not have handle.
                // it is recommended to use PictureBox with WindowsFormsHost.
                // or use handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                var handle = pictureBox.Handle;
                camera.SetPreviewControl(handle, new System.Drawing.Size(395, 240));

                // or use this conventional way. (works little heavy)
                //var timer = new System.Timers.Timer(1000 / 30);
                //timer.Elapsed += (s, ev) => Dispatcher.Invoke(() => image.Source = camera.GetBitmap());
                //timer.Start();
                //this.Closing += (s, ev) => timer.Stop();
                // start.
                camera.Start();
                //MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                //     face,
                //     1.2,
                //     10,
                //     Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                //     new Size(20, 20));
                //OCR ocrWin = new OCR();
                button.Click += (s, ev) => image.Source = camera.GetBitmap();
                button.Click += (s, ev) => closeaftCap();
            }
            catch (Exception ex)
            {

            }
        }


    }
}
