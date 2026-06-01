using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoAnCN_Net.UI
{
    public partial class KhuyenMaiDialog : Window
    {
        private readonly khuyenMaiBLL _bll = new khuyenMaiBLL();
        private readonly int?         _editingMa;

        public KhuyenMaiDialog()
        {
            InitializeComponent();
            txtTitle.Text   = "Thêm khuyến mãi";
            btnSave.Content = "+ Thêm";
        }

        public KhuyenMaiDialog(KhuyenMai km)
        {
            InitializeComponent();
            _editingMa      = km.MaKhuyenMai;
            txtTitle.Text   = "Chỉnh sửa khuyến mãi";
            btnSave.Content = "Lưu";

            txtTen.Text  = km.TenKhuyenMai;
            txtMoTa.Text = km.MoTa ?? "";

            if (km.PhanTramGiam.HasValue)
            {
                rdoPhanTram.IsChecked = true;
                txtGiam.Text = km.PhanTramGiam.Value.ToString("N0");
            }
            else if (km.SoTienGiam.HasValue)
            {
                rdoSoTien.IsChecked = true;
                txtGiam.Text = km.SoTienGiam.Value.ToString("N0");
            }

            dpBatDau.SelectedDate  = km.NgayBatDau;
            dpKetThuc.SelectedDate = km.NgayKetThuc;

            foreach (ComboBoxItem item in cboTrangThai.Items)
                if (item.Content.ToString() == km.TrangThai)
                { item.IsSelected = true; break; }
        }

        private void rdoLoai_Checked(object sender, RoutedEventArgs e)
        {
            if (lblGiam == null) return;
            lblGiam.Text = rdoPhanTram.IsChecked == true
                ? "% giảm (0-100)"
                : "Số tiền giảm (đồng)";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            string ten    = txtTen.Text.Trim();
            string moTa   = txtMoTa.Text.Trim();
            string trangThai = (cboTrangThai.SelectedItem as ComboBoxItem)
                               ?.Content?.ToString() ?? "Hoạt động";

            // Parse giá trị giảm
            decimal? phanTram = null;
            decimal? soTien   = null;

            string giamRaw = txtGiam.Text.Trim().Replace(",", "").Replace(".", "");
            if (!decimal.TryParse(giamRaw, out decimal giamVal))
            {
                ShowError("Giá trị giảm phải là số hợp lệ.");
                return;
            }

            if (rdoPhanTram.IsChecked == true) phanTram = giamVal;
            else                               soTien   = giamVal;

            DateTime? ngayBD = dpBatDau.SelectedDate;
            DateTime? ngayKT = dpKetThuc.SelectedDate;

            try
            {
                if (_editingMa == null)
                    _bll.AddKhuyenMai(ten, phanTram, soTien, ngayBD, ngayKT, trangThai, moTa);
                else
                    _bll.UpdateKhuyenMai(_editingMa.Value, ten, phanTram, soTien,
                                         ngayBD, ngayKT, trangThai, moTa);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void ShowError(string msg)
        {
            txtError.Text       = msg;
            pnlError.Visibility = Visibility.Visible;
        }
    }
}