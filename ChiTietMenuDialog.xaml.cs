using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DoAnCN_Net.UI
{
    public partial class ChiTietMenuDialog : Window
    {
        // ── fields ───────────────────────────────────────────────────────────
        private readonly menuBLL _bll = new menuBLL();
        private readonly monAnBLL _monBll = new monAnBLL();
        private readonly int _maMenu;

        // Wrapper để hiển thị GiaText đã format (không sửa DAL entity)
        private class MonAnRow
        {
            public int MaMon { get; set; }
            public string TenMon { get; set; }
            public decimal Gia { get; set; }
            public string GiaText => Gia.ToString("N0") + "đ";
        }

        // ── ctor ─────────────────────────────────────────────────────────────
        public ChiTietMenuDialog(int maMenu, string tenMenu)
        {
            InitializeComponent();
            _maMenu = maMenu;
            txtTitle.Text = "Chi tiết: " + tenMenu;
            LoadData();
        }

        // ── load ─────────────────────────────────────────────────────────────
        private void LoadData()
        {
            pnlError.Visibility = Visibility.Collapsed;
            try
            {
                // Món đang có trong menu
                var trongMenu = _bll.GetMonAnByMenu(_maMenu)
                    .Select(ToRow).ToList();
                dgTrongMenu.ItemsSource = trongMenu;

                // Tất cả món — lọc bỏ những món đã có
                var daTrongMenu = trongMenu.Select(r => r.MaMon).ToHashSet();
                var tatCa = _monBll.GetAll()
                    .Where(m => !daTrongMenu.Contains(m.MaMon))
                    .Select(ToRow).ToList();
                dgTatCaMon.ItemsSource = tatCa;
            }
            catch (System.Exception ex) { ShowError(ex.Message); }
        }

        // ── events ───────────────────────────────────────────────────────────

        // Thêm món từ cột phải vào menu
        private void btnThemVaoMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgTatCaMon.SelectedItem is MonAnRow sel))
            { ShowError("Vui lòng chọn món cần thêm."); return; }

            try
            {
                _bll.AddMonVaoMenu(_maMenu, sel.MaMon);
                LoadData();
            }
            catch (System.Exception ex) { ShowError(ex.Message); }
        }

        // Xóa món đang chọn ở cột trái khỏi menu
        private void btnXoaKhoiMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgTrongMenu.SelectedItem is MonAnRow sel))
            { ShowError("Vui lòng chọn món cần xóa."); return; }

            if (MessageBox.Show(
                    "Xóa \"" + sel.TenMon + "\" khỏi menu?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            try
            {
                _bll.RemoveMonKhoiMenu(_maMenu, sel.MaMon);
                LoadData();
            }
            catch (System.Exception ex) { ShowError(ex.Message); }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        // ── helpers ──────────────────────────────────────────────────────────
        private static MonAnRow ToRow(MonAn m) => new MonAnRow
        {
            MaMon = m.MaMon,
            TenMon = m.TenMon,
            Gia = m.Gia ?? 0m
        };

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            pnlError.Visibility = Visibility.Visible;
        }
    }
}