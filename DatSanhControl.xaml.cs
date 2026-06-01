using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DoAnCN_Net
{
    public partial class DatSanhControl : UserControl
    {
        // ── Dependencies ──────────────────────────────────
        private readonly DatSanhBLL _bll = new DatSanhBLL();
        private readonly user_SanhBLL _sanhBll = new user_SanhBLL();

        // ── State ─────────────────────────────────────────
        private List<DatSanh> _allData = new List<DatSanh>();
        private List<DatSanh> _filtered = new List<DatSanh>();
        private int _editingId = 0;
        private bool _isLoading = true;

        // ── Search debounce ───────────────────────────────
        private DispatcherTimer _searchTimer;

        // ── Paging ────────────────────────────────────────
        private int _pageSize = 12;
        private int _currentPage = 1;
        private int _totalPages = 1;

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        public DatSanhControl()
        {
            InitializeComponent();
            SetupSearchTimer();
            _isLoading = true;
            LoadFormData();
            _isLoading = false;
            LoadData();
        }

        // ═══════════════════════════════════════════════════
        // SEARCH TIMER (debounce 200ms)
        // ═══════════════════════════════════════════════════

        private void SetupSearchTimer()
        {
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _searchTimer.Tick += (s, e) =>
            {
                _searchTimer.Stop();
                UpdateSuggestPopup();
                ApplyFilter();
            };
        }

        // ═══════════════════════════════════════════════════
        // DATA
        // ═══════════════════════════════════════════════════

        private void LoadData()
        {
            try
            {
                _allData = _bll.GetAll();
                ApplyFilter();
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFormData()
        {
            // Giờ
            var gios = new List<string>();
            for (int h = 6; h <= 23; h++) gios.Add($"{h:D2}:00");
            cboGioBD.ItemsSource = gios;
            cboGioKT.ItemsSource = new List<string>(gios);
            cboGioBD.SelectedIndex = 0;
            cboGioKT.SelectedIndex = 4;

            // Khách hàng
            try
            {
                var khDal = new KhachHangDAL();
                cboKhachHang.ItemsSource = khDal.GetAll();
            }
            catch { }

            // Sảnh
            var sanhs = _sanhBll.GetAll();
            cboSanh.ItemsSource = sanhs;

            // Filter sảnh
            var sanhFilter = new List<Sanh> { new Sanh { MaSanh = 0, TenSanh = "-- Tất cả sảnh --" } };
            sanhFilter.AddRange(sanhs);
            cboFilterSanh.ItemsSource = sanhFilter;
            cboFilterSanh.DisplayMemberPath = "TenSanh";
            cboFilterSanh.SelectedValuePath = "MaSanh";
            cboFilterSanh.SelectedIndex = 0;

            // Loại sự kiện
            cboLoaiSuKien.ItemsSource = _sanhBll.GetAllLoaiSuKien();

            // Menu
            cboMenu.ItemsSource = _sanhBll.GetAllMenu();
        }

        // ═══════════════════════════════════════════════════
        // FILTER + PAGING
        // ═══════════════════════════════════════════════════

        private void ApplyFilter()
        {
            if (_isLoading) return;
            if (cboFilterSanh == null || dpFilter == null) return;

            var query = _allData.AsEnumerable();

            // Search text — dùng trực tiếp txtSearch.Text (không dùng placeholder flag)
            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d =>
                    (d.KhachHang?.TenKhachHang?.ToLower().Contains(keyword) == true) ||
                    (d.TenSuKien?.ToLower().Contains(keyword) == true));
            }

            // Filter trạng thái
            string trangThai = (cboFilterTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(trangThai) && !trangThai.StartsWith("--"))
                query = query.Where(d => d.TrangThai == trangThai);

            // Filter sảnh
            if (cboFilterSanh?.SelectedValue is int maSanh && maSanh > 0)
                query = query.Where(d => d.MaSanh == maSanh);

            // Filter ngày
            if (dpFilter?.SelectedDate.HasValue == true)
                query = query.Where(d => d.NgayToChuc == dpFilter.SelectedDate.Value);

            _filtered = query.OrderByDescending(d => d.MaDat).ToList();
            _currentPage = 1;
            RenderPage();
        }

        private void RenderPage()
        {
            _totalPages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)_pageSize));
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

            var pageData = _filtered
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            int startIndex = (_currentPage - 1) * _pageSize + 1;
            var viewModels = pageData.Select((d, i) => new DatSanhViewModel
            {
                STT = startIndex + i,
                MaDat = d.MaDat,
                TenSuKien = d.TenSuKien,
                NgayToChuc = d.NgayToChuc,
                GioBatDau = d.GioBatDau,
                GioKetThuc = d.GioKetThuc,
                TrangThai = d.TrangThai,
                KhachHang = d.KhachHang,
                Sanh = d.Sanh
            }).ToList();

            dgDatSanh.ItemsSource = viewModels;

            lblTongDon.Text = $"Tổng: {_filtered.Count} đơn";
            lblPaging.Text = $"Hiển thị {pageData.Count}/{_filtered.Count} đơn";
            lblPage.Text = $"{_currentPage} / {_totalPages}";
            btnPrev.IsEnabled = _currentPage > 1;
            btnNext.IsEnabled = _currentPage < _totalPages;
        }

        private void UpdateStats()
        {
            lblDangDat.Text = _allData.Count(d => d.TrangThai == "Đang đặt").ToString();
            lblDaCoc.Text = _allData.Count(d => d.TrangThai == "Đã cọc").ToString();
            lblHoanThanh.Text = _allData.Count(d => d.TrangThai == "Hoàn thành").ToString();
            lblDaHuy.Text = _allData.Count(d => d.TrangThai == "Đã hủy").ToString();
        }

        // ═══════════════════════════════════════════════════
        // SEARCH — LIVE + SUGGEST POPUP
        // ═══════════════════════════════════════════════════

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ẩn/hiện placeholder TextBlock
            txtPlaceholder.Visibility = string.IsNullOrEmpty(txtSearch.Text)
                ? Visibility.Visible : Visibility.Collapsed;

            // Debounce: reset timer mỗi lần gõ
            _searchTimer?.Stop();
            _searchTimer?.Start();
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPlaceholder.Visibility = Visibility.Collapsed;

            // Nếu đang có text thì mở lại popup gợi ý
            if (!string.IsNullOrEmpty(txtSearch.Text))
                UpdateSuggestPopup();
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Delay 150ms để kịp xử lý click vào lstSuggest trước khi đóng popup
            System.Threading.Tasks.Task.Delay(150).ContinueWith(_ =>
                Dispatcher.Invoke(() =>
                {
                    popupSuggest.IsOpen = false;
                    if (string.IsNullOrEmpty(txtSearch.Text))
                        txtPlaceholder.Visibility = Visibility.Visible;
                }));
        }

        // Cập nhật danh sách gợi ý trong popup
        private void UpdateSuggestPopup()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                popupSuggest.IsOpen = false;
                return;
            }

            // Gợi ý: tên khách hàng + tên sự kiện match keyword, tối đa 8 mục
            var suggestions = _allData
                .SelectMany(d => new[]
                {
                    d.KhachHang?.TenKhachHang?.Trim(),
                    d.TenSuKien?.Trim()
                })
                .Where(s => !string.IsNullOrEmpty(s) &&
                            s.ToLower().Contains(keyword))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .Take(8)
                .ToList();

            if (suggestions.Count == 0)
            {
                popupSuggest.IsOpen = false;
                return;
            }

            lstSuggest.ItemsSource = suggestions;
            lstSuggest.SelectedIndex = -1;
            popupSuggest.IsOpen = true;
        }

        // Người dùng click chọn gợi ý
        private void LstSuggest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSuggest.SelectedItem is string selected)
            {
                txtSearch.Text = selected;
                txtSearch.CaretIndex = selected.Length;
                popupSuggest.IsOpen = false;
                txtPlaceholder.Visibility = Visibility.Collapsed;
                ApplyFilter();
            }
        }

        // ═══════════════════════════════════════════════════
        // EVENT HANDLERS — TOOLBAR
        // ═══════════════════════════════════════════════════

        // Nút "Làm mới" — reset toàn bộ filter + tải lại data từ DB
        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            _isLoading = true;

            // Xóa search
            txtSearch.Text = "";
            txtPlaceholder.Visibility = Visibility.Visible;
            popupSuggest.IsOpen = false;
            _searchTimer?.Stop();

            // Xóa tất cả filter
            cboFilterTrangThai.SelectedIndex = 0;
            cboFilterSanh.SelectedIndex = 0;
            dpFilter.SelectedDate = null;

            _isLoading = false;

            // Tải lại toàn bộ data từ DB
            LoadData();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        // ═══════════════════════════════════════════════════
        // EVENT HANDLERS — DATAGRID
        // ═══════════════════════════════════════════════════

        private void DgDatSanh_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int maDat)
            {
                var ds = _bll.FindById(maDat);
                if (ds != null) OpenFormSua(ds);
            }
        }

        private void BtnDoiTrangThai_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Tag is int maDat)) return;

            var ds = _bll.FindById(maDat);
            if (ds == null) return;

            var win = new Window
            {
                Title = $"Đổi trạng thái - Đơn #{maDat}",
                Width = 320,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };

            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock
            {
                Text = $"Trạng thái hiện tại: {ds.TrangThai}",
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 12)
            });

            var cbo = new ComboBox { Height = 32, FontSize = 13, Margin = new Thickness(0, 0, 0, 20) };
            foreach (var tt in new[] { "Đang đặt", "Đã cọc", "Hoàn thành", "Đã hủy" })
                cbo.Items.Add(tt);
            cbo.SelectedItem = ds.TrangThai;
            panel.Children.Add(cbo);

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var btnOk = new Button
            {
                Content = "Xác nhận",
                Width = 90,
                Height = 32,
                Background = System.Windows.Media.Brushes.DarkRed,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 8, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            var btnCancel = new Button { Content = "Hủy", Width = 70, Height = 32 };
            btnOk.Click += (s, ev) => win.DialogResult = true;
            btnCancel.Click += (s, ev) => win.DialogResult = false;
            btnPanel.Children.Add(btnOk);
            btnPanel.Children.Add(btnCancel);
            panel.Children.Add(btnPanel);
            win.Content = panel;

            if (win.ShowDialog() == true && cbo.SelectedItem != null)
            {
                ds.TrangThai = cbo.SelectedItem.ToString();
                try
                {
                    _bll.Update(ds);
                    LoadData();
                    MessageBox.Show("Cập nhật trạng thái thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Tag is int maDat)) return;

            var confirm = MessageBox.Show(
                $"Xóa đơn đặt sảnh #{maDat}?\nHành động này không thể hoàn tác!",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                _bll.Delete(maDat);
                LoadData();
                if (_editingId == maDat) pnlForm.Visibility = Visibility.Collapsed;
                MessageBox.Show("Đã xóa thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════
        // FORM
        // ═══════════════════════════════════════════════════

        private void OpenFormThem()
        {
            _editingId = 0;
            lblFormTitle.Text = "➕ Thêm đặt sảnh mới";
            ClearForm();
            pnlForm.Visibility = Visibility.Visible;
        }

        private void OpenFormSua(DatSanh ds)
        {
            _editingId = ds.MaDat;
            lblFormTitle.Text = $"✏️ Sửa đơn #{ds.MaDat}";

            cboKhachHang.SelectedValue = ds.MaKhachHang;
            cboSanh.SelectedValue = ds.MaSanh;
            txtTenSuKien.Text = ds.TenSuKien;
            cboLoaiSuKien.SelectedValue = ds.MaLoai;
            cboMenu.SelectedValue = ds.MaMenu;
            dpNgay.SelectedDate = ds.NgayToChuc;

            string giobd = ds.GioBatDau?.ToString(@"hh\:mm");
            string giokt = ds.GioKetThuc?.ToString(@"hh\:mm");
            cboGioBD.SelectedItem = giobd;
            cboGioKT.SelectedItem = giokt;

            foreach (ComboBoxItem item in cboTrangThai.Items)
                if (item.Content.ToString() == ds.TrangThai)
                { cboTrangThai.SelectedItem = item; break; }

            pnlCanhBao.Visibility = Visibility.Collapsed;
            pnlForm.Visibility = Visibility.Visible;
        }

        private void ClearForm()
        {
            cboKhachHang.SelectedIndex = -1;
            cboSanh.SelectedIndex = -1;
            txtTenSuKien.Text = "";
            cboLoaiSuKien.SelectedIndex = -1;
            cboMenu.SelectedIndex = -1;
            dpNgay.SelectedDate = null;
            cboGioBD.SelectedIndex = 0;
            cboGioKT.SelectedIndex = 4;
            cboTrangThai.SelectedIndex = 0;
            pnlCanhBao.Visibility = Visibility.Collapsed;
        }

        private bool ValidateForm()
        {
            if (cboKhachHang.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn khách hàng!"); return false; }
            if (cboSanh.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn sảnh!"); return false; }
            if (string.IsNullOrWhiteSpace(txtTenSuKien.Text))
            { MessageBox.Show("Vui lòng nhập tên sự kiện!"); return false; }
            if (cboLoaiSuKien.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn loại sự kiện!"); return false; }
            if (cboMenu.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn menu!"); return false; }
            if (!dpNgay.SelectedDate.HasValue)
            { MessageBox.Show("Vui lòng chọn ngày tổ chức!"); return false; }
            if (!TimeSpan.TryParse(cboGioBD.SelectedItem?.ToString(), out var gioBD) ||
                !TimeSpan.TryParse(cboGioKT.SelectedItem?.ToString(), out var gioKT))
            { MessageBox.Show("Vui lòng chọn giờ hợp lệ!"); return false; }
            if (gioBD >= gioKT)
            { MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu!"); return false; }
            if (pnlCanhBao.Visibility == Visibility.Visible)
            { MessageBox.Show("Sảnh đã có lịch trong khung giờ này!"); return false; }
            return true;
        }

        // ═══════════════════════════════════════════════════
        // KIỂM TRA TRÙNG LỊCH
        // ═══════════════════════════════════════════════════

        private void KiemTraTrungLich()
        {
            if (cboSanh.SelectedValue == null || !dpNgay.SelectedDate.HasValue ||
                cboGioBD.SelectedItem == null || cboGioKT.SelectedItem == null) return;

            if (!TimeSpan.TryParse(cboGioBD.SelectedItem.ToString(), out var gioBD) ||
                !TimeSpan.TryParse(cboGioKT.SelectedItem.ToString(), out var gioKT)) return;

            try
            {
                int maSanh = (int)cboSanh.SelectedValue;
                bool trung = _sanhBll.KiemTraTrungLich(maSanh, dpNgay.SelectedDate.Value, gioBD, gioKT);

                if (trung && _editingId > 0)
                {
                    var chinh = _allData.FirstOrDefault(d => d.MaDat == _editingId);
                    if (chinh != null &&
                        chinh.MaSanh == maSanh &&
                        chinh.NgayToChuc == dpNgay.SelectedDate.Value &&
                        chinh.GioBatDau == gioBD &&
                        chinh.GioKetThuc == gioKT)
                        trung = false;
                }

                pnlCanhBao.Visibility = trung ? Visibility.Visible : Visibility.Collapsed;
            }
            catch { }
        }

        // ═══════════════════════════════════════════════════
        // EVENT HANDLERS — FORM
        // ═══════════════════════════════════════════════════

        private void BtnDongForm_Click(object sender, RoutedEventArgs e)
        {
            pnlForm.Visibility = Visibility.Collapsed;
            _editingId = 0;
        }

        private void CboSanh_Changed(object sender, SelectionChangedEventArgs e) => KiemTraTrungLich();
        private void DpNgay_Changed(object sender, SelectionChangedEventArgs e) => KiemTraTrungLich();
        private void Gio_Changed(object sender, SelectionChangedEventArgs e) => KiemTraTrungLich();

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            TimeSpan.TryParse(cboGioBD.SelectedItem.ToString(), out var gioBD);
            TimeSpan.TryParse(cboGioKT.SelectedItem.ToString(), out var gioKT);

            try
            {
                if (_editingId == 0)
                {
                    var ds = new DatSanh
                    {
                        MaKhachHang = (int)cboKhachHang.SelectedValue,
                        MaSanh = (int)cboSanh.SelectedValue,
                        TenSuKien = txtTenSuKien.Text.Trim(),
                        MaLoai = (int)cboLoaiSuKien.SelectedValue,
                        MaMenu = (int)cboMenu.SelectedValue,
                        NgayToChuc = dpNgay.SelectedDate.Value,
                        GioBatDau = gioBD,
                        GioKetThuc = gioKT,
                        TrangThai = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Đang đặt"
                    };
                    _bll.Insert(ds);
                    MessageBox.Show("Thêm đơn đặt sảnh thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var ds = _bll.FindById(_editingId);
                    ds.MaKhachHang = (int)cboKhachHang.SelectedValue;
                    ds.MaSanh = (int)cboSanh.SelectedValue;
                    ds.TenSuKien = txtTenSuKien.Text.Trim();
                    ds.MaLoai = (int)cboLoaiSuKien.SelectedValue;
                    ds.MaMenu = (int)cboMenu.SelectedValue;
                    ds.NgayToChuc = dpNgay.SelectedDate.Value;
                    ds.GioBatDau = gioBD;
                    ds.GioKetThuc = gioKT;
                    ds.TrangThai = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    _bll.Update(ds);
                    MessageBox.Show("Cập nhật thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadData();
                pnlForm.Visibility = Visibility.Collapsed;
                _editingId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════
        // PAGING
        // ═══════════════════════════════════════════════════

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1) { _currentPage--; RenderPage(); }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages) { _currentPage++; RenderPage(); }
        }
    }
}

// ═══════════════════════════════════════════════════
// VIEW MODEL
// ═══════════════════════════════════════════════════

public class DatSanhViewModel
{
    public int STT { get; set; }
    public int MaDat { get; set; }
    public string TenKhachHang => KhachHang?.TenKhachHang;
    public string SoDienThoai => KhachHang?.SoDienThoai;
    public string TenSanh => Sanh?.TenSanh;
    public string TenSuKien { get; set; }
    public DateTime? NgayToChuc { get; set; }
    public TimeSpan? GioBatDau { get; set; }
    public TimeSpan? GioKetThuc { get; set; }
    public string TrangThai { get; set; }
    public KhachHang KhachHang { get; set; }
    public Sanh Sanh { get; set; }
}