using System.ComponentModel;

namespace DoAnCN_Net.BLL
{
    public class DichVuChon : INotifyPropertyChanged
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }

        private bool _daChon;
        public bool DaChon
        {
            get => _daChon;
            set { _daChon = value; OnPropertyChanged(nameof(DaChon)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}