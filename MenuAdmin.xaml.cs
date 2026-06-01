using DoAnCN_Net.BLL;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DALMenu = DoAnCN_Net.DAL.Menu;

namespace DoAnCN_Net.UI
{
    public partial class MenuAdmin : UserControl
    {
        private readonly menuBLL _bll = new menuBLL();
        private List<DALMenu> _allMenus = new List<DALMenu>();
        private Dictionary<int, int> _soMonMap = new Dictionary<int, int>();
        private bool _isLoaded = false; // guard: tranh goi truoc khi control ready

        public MenuAdmin()
        {
            InitializeComponent();
            Loaded += (s, e) => { _isLoaded = true; LoadData(); };
        }

        // -- Load --------------------------------------------------------------
        private void LoadData()
        {
            try
            {
                _allMenus = _bll.GetAllMenus();
                _soMonMap = _bll.GetSoMonByMenu();
                UpdateStats();
                ApplyFilter();
                ClearDetail();
            }
            catch (System.Exception ex) { ShowError("Loi tai du lieu: " + ex.Message); }
        }

        private void ApplyFilter()
        {
            // Guard: neu control chua load xong thi bo qua
            if (!_isLoaded || dgMenu == null) return;

            string keyword = txtSearch.Text;
            string loai = (cboFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tat ca";
            dgMenu.ItemsSource = _bll.Filter(_allMenus, keyword, loai);

            // RefreshSoMon sau khi DataGrid render xong
            dgMenu.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Loaded,
                new System.Action(RefreshSoMonCells));
        }

        private void UpdateStats()
        {
            txtTotalMenu.Text = _bll.TongSoMenu(_allMenus).ToString();
            txtMenuChay.Text = _bll.SoMenuChay(_allMenus).ToString();
            txtMenuMan.Text = _bll.SoMenuMan(_allMenus).ToString();
        }

        // -- Detail ------------------------------------------------------------
        private void ShowDetail(DALMenu menu)
        {
            try
            {
                var monAns = _bll.GetMonAnByMenu(menu.MaMenu);
                txtDetailName.Text = menu.TenMenu;
                txtDetailLoai.Text = menu.LoaiMenu;
                txtDetailSoMon.Text = monAns.Count.ToString();
                dgMonAn.ItemsSource = monAns;

                decimal tong = _bll.TongGiaMenu(monAns);
                txtTongGia.Text = tong > 0 ? string.Format("{0:N0}d", tong) : "-";

                bool chay = menu.LoaiMenu == "Chay";
                bdgLoai.Background = MakeBrush(chay ? "#E8F5E9" : "#FFF3E0");
                txtDetailLoai.Foreground = MakeBrush(chay ? "#27AE60" : "#E67E22");
            }
            catch (System.Exception ex) { ShowError("Loi chi tiet: " + ex.Message); }
        }

        private void ClearDetail()
        {
            txtDetailName.Text = "Chon mot menu de xem chi tiet";
            txtDetailLoai.Text = "-";
            txtDetailSoMon.Text = "0";
            txtTongGia.Text = "-";
            dgMonAn.ItemsSource = null;
            bdgLoai.Background = MakeBrush("#F8F9FA");
            txtDetailLoai.Foreground = MakeBrush("#999999");
        }

        // -- Events ------------------------------------------------------------
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        private void cboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilter(); // an toan vi co guard _isLoaded

        private void dgMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMenu.SelectedItem is DALMenu sel) ShowDetail(sel);
            else ClearDetail();
        }

        private void btnAddMenu_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new MenuDialog();
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            ShowInfo("Them menu thanh cong!");
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgMenu.SelectedItem is DALMenu sel))
            { ShowWarning("Vui long chon menu can sua."); return; }

            var dlg = new MenuDialog(sel);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() != true) return;
            LoadData();
            ShowInfo("Cap nhat thanh cong!");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgMenu.SelectedItem is DALMenu sel))
            { ShowWarning("Vui long chon menu can xoa."); return; }

            if (MessageBox.Show(
                    "Xoa menu \"" + sel.TenMenu + "\"?",
                    "Xac nhan", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            try { _bll.DeleteMenu(sel.MaMenu); LoadData(); ShowInfo("Xoa thanh cong!"); }
            catch (System.Exception ex) { ShowError(ex.Message); }
        }

        private void btnChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgMenu.SelectedItem is DALMenu sel))
            { ShowWarning("Vui long chon menu."); return; }

            var dlg = new ChiTietMenuDialog(sel.MaMenu, sel.TenMenu);
            dlg.Owner = Window.GetWindow(this);
            dlg.ShowDialog();
            LoadData();
            // Re-select de detail panel cap nhat lai
            foreach (var item in dgMenu.Items)
                if (item is DALMenu m && m.MaMenu == sel.MaMenu)
                { dgMenu.SelectedItem = item; break; }
        }

        // -- Refresh so mon ----------------------------------------------------
        private void RefreshSoMonCells()
        {
            if (dgMenu == null) return;
            foreach (var item in dgMenu.Items)
            {
                if (!(item is DALMenu menu)) continue;
                var row = dgMenu.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row == null) continue;
                var cell = GetCell(row, 3);
                if (cell == null) continue;
                var tb = FindChild<TextBlock>(cell, "txtSoMon");
                if (tb != null)
                    tb.Text = _soMonMap.TryGetValue(menu.MaMenu, out int n) ? n.ToString() : "0";
            }
        }

        private static DataGridCell GetCell(DataGridRow row, int col)
        {
            var p = FindChild<DataGridCellsPresenter>(row);
            if (p == null) return null;
            var c = p.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;
            if (c == null)
            {
                row.BringIntoView();
                c = p.ItemContainerGenerator.ContainerFromIndex(col) as DataGridCell;
            }
            return c;
        }

        private static T FindChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            if (parent == null) return null;
            T found = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && (name == null || (child is FrameworkElement fe && fe.Name == name)))
                { found = t; break; }
                found = FindChild<T>(child, name);
                if (found != null) break;
            }
            return found;
        }

        private static SolidColorBrush MakeBrush(string hex) =>
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));

        private static void ShowInfo(string m) => MessageBox.Show(m, "OK", MessageBoxButton.OK, MessageBoxImage.Information);
        private static void ShowError(string m) => MessageBox.Show(m, "Loi", MessageBoxButton.OK, MessageBoxImage.Error);
        private static void ShowWarning(string m) => MessageBox.Show(m, "Canh bao", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}