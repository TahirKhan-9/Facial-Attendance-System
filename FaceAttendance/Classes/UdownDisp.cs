using System.ComponentModel;
using System.Diagnostics;

namespace FaceAttendance.Classes
{
    public class UdownDisp : INotifyPropertyChanged
    {
        public long SrNo { get; set; }
        private bool? _isSynchronized;
        public int EMachineNumber { get; set; }
        public int EnrollNumber { get; set; }
        public int FingerNumber { get; set; }
        public int Privilige { get; set; }
        public string DptID { get; set; }
        public int enPassword { get; set; }
        public string EName { get; set; }
        public string FPData { get; set; }
        public int ShiftId { get; set; }
        public string type { get; set; }
        public string Ip { get; set; }

        private bool _borderVisible = true;

        public bool Enabled
        {
            get => _borderVisible;
            set
            {
                if (_borderVisible != value)
                {
                    _borderVisible = value;
                    NotifyPropertyChanged(nameof(Enabled));
                }
            }
        }

        public bool? IsSynchronized
        {
            get => _isSynchronized;
            set
            {
                if (_isSynchronized != value)
                {
                    _isSynchronized = value;
                    NotifyPropertyChanged(nameof(IsSynchronized));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
