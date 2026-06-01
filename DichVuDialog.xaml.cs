using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Input;

namespace DoAnCN_Net.UI
{
    public partial class DichVuDialog : Window
    {
        private readonly dichVuBLL _bll = new dichVuBLL();
        private readonly int? _editingMa;

        public DichVuDialog()
        {
            InitializeComponent();
            txtTitle.Text = "Thêm dịch vụ";
            btnSave.Content = "+ Thêm";
        }

        public DichVuDialog(DichVu dv)
        {
            InitializeComponent();
            _editingMa = dv.MaDichVu;
            txtTitle.Text = "Chỉnh sửa dịch vụ";
            btnSave.Content = "Lưu";
            txtTenDichVu.Text = dv.TenDichVu;
            txtGia.Text = (dv.Gia ?? 0m).ToString("N0").Replace(",", "");
            txtMoTa.Text = dv.MoTa ?? "";   // ← thêm
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            string ten = txtTenDichVu.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            if (!decimal.TryParse(
                    txtGia.Text.Trim().Replace(",", "").Replace(".", ""),
                    out decimal gia))
            {
                txtError.Text = "Giá phải là số hợp lệ.";
                pnlError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                if (_editingMa == null)
                    _bll.AddDichVu(ten, gia, moTa);
                else
                    _bll.UpdateDichVu(_editingMa.Value, ten, gia, moTa);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message;
                pnlError.Visibility = Visibility.Visible;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();
    }
}