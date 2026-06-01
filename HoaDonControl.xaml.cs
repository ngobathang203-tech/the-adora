using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net.UI
{
    public partial class HoaDonControl : UserControl
    {
        private readonly HoaDonBLL _bll = new HoaDonBLL();
        private readonly DatSanhDAL _dsDal = new DatSanhDAL();
        private readonly KhachHangDAL _khDal = new KhachHangDAL();
        private List<HoaDonRow> _allRows = new List<HoaDonRow>();

        public class HoaDonRow
        {
            public int MaHoaDon { get; set; }
            public int? MaDat { get; set; }
            public string TenKhachHang { get; set; }
            public string TenSuKien { get; set; }
            public decimal? TongTien { get; set; }
            public DateTime? NgayLap { get; set; }
            public string NgayLapHienThi => NgayLap?.ToString("dd/MM/yyyy HH:mm") ?? "";
        }

        public HoaDonControl()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                var hoaDons = _bll.GetAll();
                var datSanhs = _dsDal.GetAll();
                var khachHangs = _khDal.GetAll();

                _allRows = hoaDons.Select(h =>
                {
                    var ds = datSanhs.FirstOrDefault(d => d.MaDat == h.MaDat);
                    var kh = ds != null ? khachHangs.FirstOrDefault(k => k.MaKhachHang == ds.MaKhachHang) : null;
                    return new HoaDonRow
                    {
                        MaHoaDon = h.MaHoaDon,
                        MaDat = h.MaDat,
                        TenKhachHang = kh?.TenKhachHang ?? $"Đặt #{h.MaDat}",
                        TenSuKien = ds?.TenSuKien ?? "",
                        TongTien = h.TongTien,
                        NgayLap = h.NgayLap
                    };
                }).ToList();

                UpdateStats(hoaDons);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            var keyword = txtSearch?.Text?.Trim() ?? "";
            DateTime? tuNgay = dpTuNgay?.SelectedDate;
            DateTime? denNgay = dpDenNgay?.SelectedDate;

            var filtered = _allRows.AsEnumerable();

            if (!string.IsNullOrEmpty(keyword))
                filtered = filtered.Where(r =>
                    r.MaHoaDon.ToString().Contains(keyword) ||
                    r.MaDat.ToString().Contains(keyword) ||
                    (r.TenKhachHang?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r.TenSuKien?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));

            if (tuNgay.HasValue)
                filtered = filtered.Where(r => r.NgayLap.HasValue && r.NgayLap.Value.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                filtered = filtered.Where(r => r.NgayLap.HasValue && r.NgayLap.Value.Date <= denNgay.Value.Date);

            dgHoaDon.ItemsSource = filtered.ToList();
        }

        private void UpdateStats(List<HoaDon> list)
        {
            txtTong.Text = _bll.TongSo(list).ToString();
            txtDoanhThu.Text = _bll.TongDoanhThu(list).ToString("N0");
            txtTrungBinh.Text = _bll.TrungBinh(list).ToString("N0");
            txtCaoNhat.Text = _bll.CaoNhat(list).ToString("N0");
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();
        private void dpTuNgay_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();
        private void dpDenNgay_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            dpTuNgay.SelectedDate = null;
            dpDenNgay.SelectedDate = null;
            ApplyFilter();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new HoaDonDialog();
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            MessageBox.Show("Tạo hóa đơn thành công!", "OK",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgHoaDon.SelectedItem is HoaDonRow sel))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần sửa.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var hd = _bll.GetById(sel.MaHoaDon);
            var dlg = new HoaDonDialog(hd);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            MessageBox.Show("Cập nhật hóa đơn thành công!", "OK",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgHoaDon.SelectedItem is HoaDonRow sel))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Xóa hóa đơn #{sel.MaHoaDon}?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                _bll.DeleteHoaDon(sel.MaHoaDon);
                LoadData();
                MessageBox.Show("Xóa thành công!", "OK",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXuatPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rows = dgHoaDon.ItemsSource as IEnumerable<HoaDonRow>;
                if (rows == null || !rows.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text files|*.txt",
                    FileName = $"HoaDon_{DateTime.Now:yyyyMMdd_HHmm}.txt"
                };
                if (dlg.ShowDialog() != true) return;

                var sb = new StringBuilder();
                sb.AppendLine("============================================================");
                sb.AppendLine("                   ADORA - DANH SÁCH HÓA ĐƠN");
                sb.AppendLine($"                   Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sb.AppendLine("============================================================");
                sb.AppendLine($"{"Mã HD",-8}{"Mã đặt",-10}{"Khách hàng",-25}{"Ngày lập",-20}{"Tổng tiền",15}");
                sb.AppendLine(new string('-', 80));

                foreach (var r in rows)
                    sb.AppendLine($"{r.MaHoaDon,-8}{r.MaDat,-10}{r.TenKhachHang,-25}{r.NgayLapHienThi,-20}{r.TongTien:N0,15}");

                sb.AppendLine(new string('-', 80));
                sb.AppendLine($"{"Tổng doanh thu:",-43}{rows.Sum(r => r.TongTien ?? 0):N0,15} đ");
                sb.AppendLine("============================================================");

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Đã xuất file thành công!\n{dlg.FileName}", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}