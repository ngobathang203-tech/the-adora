using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class LichSuDatTiec_Control : UserControl
    {
        // Danh sách gốc để tìm kiếm / lọc không mất data
        private List<DatSanh> _danhSachGoc = new List<DatSanh>();

        public LichSuDatTiec_Control()
        {
            InitializeComponent();
            LoadData();
        }

        // ─────────────────────────────────────────
        // LOAD DATA
        // ─────────────────────────────────────────
        private void LoadData()
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return;

            var bll = new DatSanhBLL();
            var list = bll.GetByKhachHang(user.MaKhachHang);

            _danhSachGoc = list ?? new List<DatSanh>();
            ApplyFilter();
        }

        // ─────────────────────────────────────────
        // ÁP DỤNG LỌC + TÌM KIẾM
        // ─────────────────────────────────────────
        private void ApplyFilter()
        {
            var result = _danhSachGoc.AsEnumerable();

            // Lọc theo trạng thái
            string trangThai = GetSelectedTrangThai();
            if (trangThai != "Tất cả")
                result = result.Where(x => x.TrangThai == trangThai);

            // Lọc theo từ khóa tìm kiếm
            string keyword = txtSearch?.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(keyword))
                result = result.Where(x =>
                    (x.TenSuKien?.ToLower().Contains(keyword) == true) ||
                    (x.Sanh?.TenSanh?.ToLower().Contains(keyword) == true));

            var filtered = result.ToList();

            if (filtered.Count == 0)
            {
                dgLichSu.Visibility = Visibility.Collapsed;
                pnlEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                dgLichSu.Visibility = Visibility.Visible;
                pnlEmpty.Visibility = Visibility.Collapsed;
                dgLichSu.ItemsSource = filtered;
            }

            txtTongDon.Text = filtered.Count.ToString();
        }

        // Lấy trạng thái đang chọn từ RadioButton
        private string GetSelectedTrangThai()
        {
            // Tìm RadioButton đang được check trong pnlFilterChip
            foreach (var child in pnlFilterChip.Children)
            {
                if (child is RadioButton rb && rb.IsChecked == true)
                    return rb.Content?.ToString() ?? "Tất cả";
            }
            return "Tất cả";
        }

        // ─────────────────────────────────────────
        // TÌM KIẾM
        // ─────────────────────────────────────────
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        // ─────────────────────────────────────────
        // LỌC TRẠNG THÁI
        // ─────────────────────────────────────────
        private void FilterTrangThai_Changed(object sender, RoutedEventArgs e)
        {
            // ApplyFilter cần dgLichSu đã init (tránh lỗi khi XAML load)
            if (dgLichSu == null) return;
            ApplyFilter();
        }

        // ─────────────────────────────────────────
        // XEM CHI TIẾT
        // ─────────────────────────────────────────
        private void BtnChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is int maDat)) return;

            var bll = new DatSanhBLL();
            var don = bll.FindById(maDat);
            if (don == null) return;

            string chiTiet =
                $"Mã đặt      : {don.MaDat}\n" +
                $"Tên sự kiện : {don.TenSuKien}\n" +
                $"Sảnh        : {don.Sanh?.TenSanh}\n" +
                $"Ngày tổ chức: {don.NgayToChuc:dd/MM/yyyy}\n" +
                $"Giờ bắt đầu : {don.GioBatDau}\n" +
                $"Giờ kết thúc: {don.GioKetThuc}\n" +
                $"Trạng thái  : {don.TrangThai}";

            MessageBox.Show(chiTiet, "Chi Tiết Đơn Đặt Tiệc",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ─────────────────────────────────────────
        // HỦY ĐẶT TIỆC — dùng Update đổi TrangThai
        // ─────────────────────────────────────────
        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is int maDat)) return;

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn hủy đơn đặt tiệc này?\nHành động này không thể hoàn tác.",
                "Xác nhận hủy",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            var bll = new DatSanhBLL();
            var don = bll.FindById(maDat);
            if (don == null) return;

            don.TrangThai = "Đã hủy";
            bool ok = bll.Update(don);

            if (ok)
            {
                MessageBox.Show("Đã hủy đơn đặt tiệc thành công.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Hủy không thành công. Vui lòng thử lại.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─────────────────────────────────────────
        // SELECTION CHANGED (highlight row)
        // ─────────────────────────────────────────
        private void DgLichSu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể dùng để preview chi tiết nếu cần
        }

        // ─────────────────────────────────────────
        // REFRESH TỪ BÊN NGOÀI
        // ─────────────────────────────────────────
        public void RefreshData() => LoadData();
    }
}