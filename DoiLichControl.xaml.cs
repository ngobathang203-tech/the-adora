using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net.UI
{
    // ViewModel hien thi trong DataGrid
    public class DoiLichViewModel
    {
        public int MaDat { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string TenSanh { get; set; }
        public string TenSuKien { get; set; }
        public DateTime? NgayToChuc { get; set; }
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public string TenMenu { get; set; }
        public string TrangThai { get; set; }

        public string NgayHienThi => NgayToChuc.HasValue
            ? NgayToChuc.Value.ToString("dd/MM/yyyy") : "";

        public string GioHienThi
        {
            get
            {
                string bd = GioBatDau.HasValue
                    ? $"{GioBatDau.Value.Hours:D2}:{GioBatDau.Value.Minutes:D2}" : "--";
                string kt = GioKetThuc.HasValue
                    ? $"{GioKetThuc.Value.Hours:D2}:{GioKetThuc.Value.Minutes:D2}" : "--";
                return $"{bd} - {kt}";
            }
        }
    }

    public partial class DoiLichControl : UserControl
    {
        private readonly DatSanhBLL _bll = new DatSanhBLL();
        private readonly user_SanhBLL _sanhBll = new user_SanhBLL();
        private List<DoiLichViewModel> _allRows = new List<DoiLichViewModel>();

        public DoiLichControl()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        // ── Load ─────────────────────────────────────────────────────
        private void LoadData()
        {
            try
            {
                var db = dataDataContext.Create();

                var rows = (from ds in db.DatSanhs
                            join kh in db.KhachHangs on ds.MaKhachHang equals kh.MaKhachHang
                            join s in db.Sanhs on ds.MaSanh equals s.MaSanh
                            join m in db.Menus on ds.MaMenu equals m.MaMenu into mj
                            from m in mj.DefaultIfEmpty()
                            orderby ds.NgayToChuc descending
                            select new DoiLichViewModel
                            {
                                MaDat = ds.MaDat,
                                TenKhachHang = kh.TenKhachHang,
                                SoDienThoai = kh.SoDienThoai,
                                TenSanh = s.TenSanh,
                                TenSuKien = ds.TenSuKien,
                                NgayToChuc = ds.NgayToChuc,
                                GioBatDau = ds.GioBatDau,
                                GioKetThuc = ds.GioKetThuc,
                                TenMenu = m != null ? m.TenMenu : "",
                                TrangThai = ds.TrangThai
                            }).ToList();

                _allRows = rows;
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi tai du lieu: " + ex.Message, "Loi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            if (dgDoiLich == null) return;
            string keyword = txtSearch?.Text?.Trim() ?? "";
            string trangThai = (cboFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tat ca";

            var filtered = _allRows.AsEnumerable();

            if (!string.IsNullOrEmpty(keyword))
                filtered = filtered.Where(r =>
                    r.TenKhachHang.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.SoDienThoai.Contains(keyword) ||
                    r.TenSanh.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.MaDat.ToString().Contains(keyword));

            if (trangThai != "Tat ca" && trangThai != "Tất cả")
                filtered = filtered.Where(r => r.TrangThai == trangThai);

            var list = filtered.ToList();
            dgDoiLich.ItemsSource = list;
            txtTong.Text = list.Count.ToString();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();
        private void CboFilter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cboFilter.SelectedIndex = 0;
            LoadData();
        }

        private void BtnDoiLich_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Tag is DoiLichViewModel vm)) return;

            if (vm.TrangThai == "Da huy" || vm.TrangThai == "Đã hủy" ||
                vm.TrangThai == "Hoan thanh" || vm.TrangThai == "Hoàn thành")
            {
                MessageBox.Show(
                    "Khong the doi lich don co trang thai \"" + vm.TrangThai + "\".",
                    "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new DoiLichDialog(vm) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() != true) return;

            LoadData();
            MessageBox.Show("Doi lich don #" + vm.MaDat + " thanh cong!",
                "Thanh cong", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}