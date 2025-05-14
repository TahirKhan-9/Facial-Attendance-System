using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAttendance.Classes
{
    class DevInfo: INotifyPropertyChanged
    {
        public int DId { get; set; }
        public int MId { get; set; }
        public string Ip { get; set; }
        public DateTime Status { get; set; }
        public string Serial { get; set; }
        public string HWpin { get; set; }
        public string Location { get; set; }
        public string DevDescription { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public bool isRemote { get; set; }


        

        private bool borderVisible = false;
        private bool borderVisible1 = false;

        public bool enabled
        {
            get
            {
                return borderVisible;
            }

            set
            {
                borderVisible = value;
                NotifyPropertyChanged("BorderVisible");
            }
        }
        public bool disabled
        {
            get
            {
                return borderVisible1;
            }

            set
            {
                borderVisible1 = value;
                NotifyPropertyChanged("BorderVisible1");
            }
        }
        //public bool notenabled
        //{
        //    get
        //    {
        //        return borderVisible;
        //    }

        //    set
        //    {
        //        borderVisible = value;
        //        NotifyPropertyChanged("BorderVisible");
        //    }
        //}

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

