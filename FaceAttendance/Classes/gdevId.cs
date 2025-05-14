using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FaceAttendance.Classes
{
    public class gdevId
    {

        
        public int MId { get; set; }

        public string Ip { get; set; }        
        public string Location { get; set; }

        private bool borderVisible = true;

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
