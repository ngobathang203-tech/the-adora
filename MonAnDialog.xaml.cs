using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Input;

namespace DoAnCN_Net.UI
{
    public partial class MonAnDialog : Window
    {
        private readonly monAnBLL _bll = new monAnBLL();
        private readonly int? _editingMa;

        // Thêm mới
        public MonAnDialog()
        {
            InitializeComponent();
            txtTitle.Text = "Thêm món ăn";
            btnSave.Content = "+ Thêm";
        }

        // Chỉnh sửa
        public MonAnDialog(MonAn mon)
        {
            InitializeComponent();
            _editingMa = mon.MaMon;
            txtTitle.Text = "Chỉnh sửa món ăn";
            btnSave.Content = "Lưu";
            txtTenMon.Text = mon.TenMon;
            txtGia.Text = (mon.Gia ?? 0m).ToString("N0").Replace(",", "");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            string ten = txtTenMon.Text.Trim();

            if (!decimal.TryParse(txtGia.Text.Trim().Replace(",", "").Replace(".", ""),
                    out decimal gia))
            {
                txtError.Text = "Giá phải là số hợp lệ.";
                pnlError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                if (_editingMa == null)
                    _bll.AddMonAn(ten, gia);
                else
                    _bll.UpdateMonAn(_editingMa.Value, ten, gia);

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