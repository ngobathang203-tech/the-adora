using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Input;

namespace DoAnCN_Net.UI
{
    public partial class HoaDonDialog : Window
    {
        private readonly HoaDonBLL _bll = new HoaDonBLL();
        private readonly int? _editingMa;

        // Tạo mới
        public HoaDonDialog()
        {
            InitializeComponent();
            txtTitle.Text = "Tạo hóa đơn";
            btnSave.Content = "+ Tạo";
        }

        // Sửa
        public HoaDonDialog(HoaDon hd)
        {
            InitializeComponent();
            _editingMa = hd.MaHoaDon;
            txtTitle.Text = "Chỉnh sửa hóa đơn";
            btnSave.Content = "Lưu";

            txtMaDat.Text = hd.MaDat?.ToString() ?? "";
            txtMaDat.IsReadOnly = true; // không cho sửa mã đặt
            txtTongTien.Text = (hd.TongTien ?? 0m).ToString("N0").Replace(",", "");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            if (!decimal.TryParse(
                    txtTongTien.Text.Trim().Replace(",", "").Replace(".", ""),
                    out decimal tongTien))
            {
                ShowError("Tổng tiền phải là số hợp lệ.");
                return;
            }

            try
            {
                if (_editingMa == null)
                {
                    if (!int.TryParse(txtMaDat.Text.Trim(), out int maDat))
                    {
                        ShowError("Mã đặt sảnh phải là số nguyên.");
                        return;
                    }
                    _bll.AddHoaDon(maDat, tongTien);
                }
                else
                {
                    _bll.UpdateHoaDon(_editingMa.Value, tongTien);
                }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            pnlError.Visibility = Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();
    }
}