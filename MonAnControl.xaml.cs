using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net.UI
{
    public partial class MonAnControl : UserControl
    {
        private readonly monAnBLL _bll = new monAnBLL();
        private List<MonAnRow> _allRows = new List<MonAnRow>();

        // Wrapper để hiển thị thêm cột SoMenu
        private class MonAnRow
        {
            public int MaMon { get; set; }
            public string TenMon { get; set; }
            public decimal Gia { get; set; }
            public int SoMenu { get; set; }
        }

        public MonAnControl()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        // ── Load ─────────────────────────────────────────────────────────────
        private void LoadData()
        {
            try
            {
                var dsMonAn = _bll.GetAll();
                var soMenuMap = _bll.GetSoMenuChuaMon(); // số menu dùng mỗi món

                _allRows = dsMonAn.Select(m => new MonAnRow
                {
                    MaMon = m.MaMon,
                    TenMon = m.TenMon,
                    Gia = m.Gia ?? 0m,
                    SoMenu = soMenuMap.TryGetValue(m.MaMon, out int n) ? n : 0
                }).ToList();

                UpdateStats();
                ApplyFilter();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string kw = txtSearch?.Text?.Trim() ?? "";
            var filtered = string.IsNullOrEmpty(kw)
                ? _allRows
                : _allRows.Where(r =>
                    r.TenMon.IndexOf(kw, System.StringComparison.OrdinalIgnoreCase) >= 0
                  ).ToList();

            dgMonAn.ItemsSource = filtered;
        }

        private void UpdateStats()
        {
            txtTongMon.Text = _allRows.Count.ToString();

            if (_allRows.Count == 0)
            {
                txtGiaMin.Text = "—";
                txtGiaMax.Text = "—";
                return;
            }

            txtGiaMin.Text = _allRows.Min(r => r.Gia).ToString("N0");
            txtGiaMax.Text = _allRows.Max(r => r.Gia).ToString("N0");
        }

        // ── Events ───────────────────────────────────────────────────────────
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new MonAnDialog();
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            MessageBox.Show("Thêm món ăn thành công!", "OK",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgMonAn.SelectedItem is MonAnRow sel))
            {
                MessageBox.Show("Vui lòng chọn món ăn cần sửa.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mon = new MonAn { MaMon = sel.MaMon, TenMon = sel.TenMon, Gia = sel.Gia };
            var dlg = new MonAnDialog(mon);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            MessageBox.Show("Cập nhật thành công!", "OK",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgMonAn.SelectedItem is MonAnRow sel))
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sel.SoMenu > 0)
            {
                MessageBox.Show(
                    $"Không thể xóa! Món \"{sel.TenMon}\" đang được dùng trong {sel.SoMenu} menu.",
                    "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa món \"{sel.TenMon}\"?", "Xác nhận",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            try
            {
                _bll.DeleteMonAn(sel.MaMon);
                LoadData();
                MessageBox.Show("Xóa thành công!", "OK",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}