using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DoAnCN_Net.DAL;
using DalMenu = DoAnCN_Net.DAL.Menu; // fix CS0104: ambiguous 'Menu'

namespace DoAnCN_Net.UI
{
    public partial class DoiLichDialog : Window
    {
        private readonly int _maDat;

        public DoiLichDialog(DoiLichViewModel vm)
        {
            InitializeComponent();
            _maDat = vm.MaDat;
            LoadComboBoxes(vm);
        }

        private void LoadComboBoxes(DoiLichViewModel vm)
        {
            // Gio
            var gios = new List<string>();
            for (int h = 6; h <= 23; h++) gios.Add($"{h:D2}:00");
            cboGioBatDau.ItemsSource = new List<string>(gios);
            cboGioKetThuc.ItemsSource = new List<string>(gios);

            // Sanh & Menu
            var db = dataDataContext.Create();
            cboSanh.ItemsSource = db.Sanhs.ToList();
            cboMenu.ItemsSource = db.Menus.ToList();

            // Hien thi thong tin cu
            string gioBD = vm.GioBatDau.HasValue
                ? $"{vm.GioBatDau.Value.Hours:D2}:{vm.GioBatDau.Value.Minutes:D2}" : "--";
            string gioKT = vm.GioKetThuc.HasValue
                ? $"{vm.GioKetThuc.Value.Hours:D2}:{vm.GioKetThuc.Value.Minutes:D2}" : "--";

            lblThongTinCu.Text =
                $"Ma dat: #{vm.MaDat}   |   Khach: {vm.TenKhachHang}   |   DT: {vm.SoDienThoai}\n" +
                $"Sanh: {vm.TenSanh}   |   Menu: {vm.TenMenu}\n" +
                $"Ngay: {vm.NgayHienThi}   |   Gio: {gioBD} - {gioKT}";

            // Dien san gia tri cu
            dpNgayMoi.SelectedDate = vm.NgayToChuc;

            if (vm.GioBatDau.HasValue)
                cboGioBatDau.SelectedItem = $"{vm.GioBatDau.Value.Hours:D2}:00";
            if (vm.GioKetThuc.HasValue)
                cboGioKetThuc.SelectedItem = $"{vm.GioKetThuc.Value.Hours:D2}:00";

            // Pre-select sanh & menu
            var ds = db.DatSanhs.FirstOrDefault(d => d.MaDat == _maDat);
            if (ds != null)
            {
                foreach (Sanh s in cboSanh.Items)
                    if (s.MaSanh == ds.MaSanh) { cboSanh.SelectedItem = s; break; }

                foreach (DalMenu m in cboMenu.Items) // dùng DalMenu thay vì Menu
                    if (m.MaMenu == ds.MaMenu) { cboMenu.SelectedItem = m; break; }
            }
        }

        // Handler cho ComboBox (SelectionChanged)
        private void Input_Changed(object sender, SelectionChangedEventArgs e)
            => KiemTraTrungLich();

        // Handler riêng cho DatePicker (SelectedDateChanged)
        private void DpNgayMoi_Changed(object sender, SelectionChangedEventArgs e)
            => KiemTraTrungLich();

        // Handler cho ComboBox Sanh
        private void CboSanh_Changed(object sender, SelectionChangedEventArgs e)
            => KiemTraTrungLich();

        private void KiemTraTrungLich()
        {
            pnlTrungLich.Visibility = Visibility.Collapsed;

            if (dpNgayMoi.SelectedDate == null ||
                cboGioBatDau.SelectedItem == null ||
                cboGioKetThuc.SelectedItem == null ||
                cboSanh.SelectedValue == null) return;

            if (!TimeSpan.TryParse(cboGioBatDau.SelectedItem.ToString(), out var gioBD) ||
                !TimeSpan.TryParse(cboGioKetThuc.SelectedItem.ToString(), out var gioKT)) return;

            try
            {
                var db = dataDataContext.Create();
                int maSanh = (int)cboSanh.SelectedValue;

                bool trung = db.DatSanhs.Any(d =>
                    d.MaSanh == maSanh &&
                    d.MaDat != _maDat &&
                    d.NgayToChuc == dpNgayMoi.SelectedDate.Value &&
                    d.TrangThai != "Da huy" &&
                    d.TrangThai != "Đã hủy" &&
                    d.GioBatDau < gioKT &&
                    d.GioKetThuc > gioBD);

                pnlTrungLich.Visibility = trung ? Visibility.Visible : Visibility.Collapsed;
            }
            catch { }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            if (dpNgayMoi.SelectedDate == null)
            { ShowError("Vui long chon ngay to chuc moi."); return; }

            if (cboGioBatDau.SelectedItem == null || cboGioKetThuc.SelectedItem == null)
            { ShowError("Vui long chon gio."); return; }

            if (!TimeSpan.TryParse(cboGioBatDau.SelectedItem.ToString(), out var gioBD) ||
                !TimeSpan.TryParse(cboGioKetThuc.SelectedItem.ToString(), out var gioKT))
            { ShowError("Gio khong hop le."); return; }

            if (gioBD >= gioKT)
            { ShowError("Gio ket thuc phai sau gio bat dau."); return; }

            if (dpNgayMoi.SelectedDate.Value.Date < DateTime.Today)
            { ShowError("Ngay to chuc khong duoc trong qua khu."); return; }

            if (cboSanh.SelectedValue == null)
            { ShowError("Vui long chon sanh."); return; }

            if (cboMenu.SelectedValue == null)
            { ShowError("Vui long chon menu."); return; }

            if (pnlTrungLich.Visibility == Visibility.Visible)
            { ShowError("Sanh da co lich trong khung gio nay!"); return; }

            try
            {
                var db = dataDataContext.Create();
                var ds = db.DatSanhs.FirstOrDefault(d => d.MaDat == _maDat);
                if (ds == null) { ShowError("Khong tim thay don dat sanh."); return; }

                ds.MaSanh = (int)cboSanh.SelectedValue;
                ds.MaMenu = (int)cboMenu.SelectedValue;
                ds.NgayToChuc = dpNgayMoi.SelectedDate.Value;
                ds.GioBatDau = gioBD;
                ds.GioKetThuc = gioKT;
                db.SubmitChanges();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            pnlError.Visibility = Visibility.Visible;
        }
    }
}