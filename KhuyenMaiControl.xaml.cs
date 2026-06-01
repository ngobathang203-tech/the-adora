using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DoAnCN_Net.UI
{
    public partial class KhuyenMaiControl : UserControl
    {
        // ── BLL ──────────────────────────────────────────────────────────
        private readonly khuyenMaiBLL _bll = new khuyenMaiBLL();

        // ── State ────────────────────────────────────────────────────────
        private List<KhuyenMai> _allData = new List<KhuyenMai>();
        private List<KmViewModel> _vmList = new List<KmViewModel>();
        private int? _editingMa = null;
        private bool _formOpen = false;

        // ── ViewModel ────────────────────────────────────────────────────
        public class KmViewModel
        {
            public int STT { get; set; }
            public int MaKhuyenMai { get; set; }
            public string TenKhuyenMai { get; set; }
            public string MoTa { get; set; }
            public string GiamGiaHienThi { get; set; }
            public string NgayBatDauHienThi { get; set; }
            public string NgayKetThucHienThi { get; set; }
            public string TrangThai { get; set; }
            public Brush TrangThaiBg { get; set; }
            public Brush TrangThaiColor { get; set; }

            // raw
            public decimal? PhanTramGiam { get; set; }
            public decimal? SoTienGiam { get; set; }
            public DateTime? NgayBatDau { get; set; }
            public DateTime? NgayKetThuc { get; set; }
        }

        // ── Constructor ──────────────────────────────────────────────────
        public KhuyenMaiControl()
        {
            InitializeComponent();
            LoadData();
        }

        // ── Load / Bind ──────────────────────────────────────────────────
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

        private void ApplyFilter()
        {
            // Guard: chưa init xong thì bỏ qua
            if (dgKhuyenMai == null) return;

            string keyword = txtSearch?.Text?.Trim() ?? "";
            string trangThai = (cboFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";

            List<KhuyenMai> filtered = _bll.Filter(_allData, keyword, trangThai);
            _vmList = filtered.Select((k, i) => ToViewModel(k, i + 1)).ToList();

            dgKhuyenMai.ItemsSource = _vmList;
            txtRecordCount.Text = $"Hiển thị {_vmList.Count} / {_allData.Count} bản ghi";
        }

        private void UpdateStats()
        {
            txtTongSo.Text = _bll.TongSo(_allData).ToString();
            txtHoatDong.Text = _bll.SoHoatDong(_allData).ToString();
            txtHetHan.Text = _bll.SoHetHan(_allData).ToString();
            txtTamDung.Text = _allData.Count(k => k.TrangThai == "Tạm dừng").ToString();
        }

        private KmViewModel ToViewModel(KhuyenMai k, int stt)
        {
            // Discount display
            string giam = k.PhanTramGiam.HasValue
                ? $"{k.PhanTramGiam:0.##}%"
                : k.SoTienGiam.HasValue
                    ? $"{k.SoTienGiam.Value:N0} ₫"
                    : "—";

            // Status badge colors
            Brush bg, fg;
            string trangThai = k.TrangThai ?? "Hoạt động";

            // Auto-detect expired
            if (k.NgayKetThuc.HasValue && k.NgayKetThuc.Value.Date < DateTime.Today)
                trangThai = "Hết hạn";

            switch (trangThai)
            {
                case "Hoạt động":
                    bg = new SolidColorBrush(Color.FromRgb(240, 255, 244));
                    fg = new SolidColorBrush(Color.FromRgb(39, 103, 73));
                    break;
                case "Tạm dừng":
                    bg = new SolidColorBrush(Color.FromRgb(255, 250, 240));
                    fg = new SolidColorBrush(Color.FromRgb(116, 66, 16));
                    break;
                case "Hết hạn":
                    bg = new SolidColorBrush(Color.FromRgb(255, 245, 245));
                    fg = new SolidColorBrush(Color.FromRgb(155, 44, 44));
                    break;
                default:
                    bg = new SolidColorBrush(Color.FromRgb(247, 250, 252));
                    fg = new SolidColorBrush(Color.FromRgb(113, 128, 150));
                    break;
            }

            return new KmViewModel
            {
                STT = stt,
                MaKhuyenMai = k.MaKhuyenMai,
                TenKhuyenMai = k.TenKhuyenMai,
                MoTa = k.MoTa,
                GiamGiaHienThi = giam,
                NgayBatDauHienThi = k.NgayBatDau?.ToString("dd/MM/yyyy") ?? "—",
                NgayKetThucHienThi = k.NgayKetThuc?.ToString("dd/MM/yyyy") ?? "—",
                TrangThai = trangThai,
                TrangThaiBg = bg,
                TrangThaiColor = fg,
                PhanTramGiam = k.PhanTramGiam,
                SoTienGiam = k.SoTienGiam,
                NgayBatDau = k.NgayBatDau,
                NgayKetThuc = k.NgayKetThuc
            };
        }

        // ── Toolbar events ───────────────────────────────────────────────
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        private void cboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilter();

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
            => LoadData();

        // ── DataGrid events ──────────────────────────────────────────────
        private void dgKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: preview selection
        }

        // ── Form open/close ──────────────────────────────────────────────
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _editingMa = null;
            txtFormTitle.Text = "Thêm khuyến mãi mới";
            ClearForm();
            OpenForm();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int ma)
                OpenEditForm(ma);
        }

        private void OpenEditForm(int ma)
        {
            var km = _bll.GetById(ma);
            if (km == null) return;

            _editingMa = ma;
            txtFormTitle.Text = "Chỉnh sửa khuyến mãi";
            ClearForm();

            txtTen.Text = km.TenKhuyenMai;
            txtMoTa.Text = km.MoTa ?? "";
            dpNgayBD.SelectedDate = km.NgayBatDau;
            dpNgayKT.SelectedDate = km.NgayKetThuc;

            // Trạng thái
            foreach (ComboBoxItem item in cboTrangThai.Items)
                if (item.Content.ToString() == km.TrangThai)
                { cboTrangThai.SelectedItem = item; break; }

            // Loại giảm
            if (km.PhanTramGiam.HasValue)
            {
                cboLoaiGiam.SelectedIndex = 0;
                txtGiaTri.Text = km.PhanTramGiam.Value.ToString("0.##");
            }
            else
            {
                cboLoaiGiam.SelectedIndex = 1;
                txtGiaTri.Text = km.SoTienGiam.HasValue
                                 ? km.SoTienGiam.Value.ToString("N0")
                                 : "";
            }

            OpenForm();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Tag is int ma)) return;

            var vm = _vmList.FirstOrDefault(k => k.MaKhuyenMai == ma);
            string ten = vm?.TenKhuyenMai ?? "";

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa khuyến mãi \"{ten}\"?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _bll.DeleteKhuyenMai(ma);
                LoadData();

                // Close form if editing the deleted item
                if (_editingMa == ma) CloseForm();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            HideError();

            // ── 1. Đọc input ─────────────────────────────────────────────
            string ten = txtTen.Text?.Trim() ?? "";
            string moTa = txtMoTa.Text?.Trim();
            DateTime? ngayBD = dpNgayBD.SelectedDate;
            DateTime? ngayKT = dpNgayKT.SelectedDate;
            string trangThai = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Hoạt động";
            bool isPhanTram = (cboLoaiGiam.SelectedIndex == 0);
            string giaTriStr = txtGiaTri.Text?.Trim() ?? "";

            // ── 2. Validate UI trước khi gọi BLL ─────────────────────────
            if (string.IsNullOrWhiteSpace(ten))
            {
                ShowError("Vui lòng nhập tên khuyến mãi.");
                txtTen.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(giaTriStr))
            {
                ShowError("Vui lòng nhập giá trị giảm.");
                txtGiaTri.Focus();
                return;
            }

            // ── 3. Parse giá trị giảm ────────────────────────────────────
            // Xóa dấu phẩy/chấm phân cách hàng nghìn, giữ lại dấu thập phân
            string normalized = giaTriStr.Replace(",", "");
            if (!decimal.TryParse(normalized,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal giaTriDecimal))
            {
                ShowError("Giá trị giảm không hợp lệ. Vui lòng nhập số.");
                txtGiaTri.Focus();
                return;
            }

            decimal? phanTram = isPhanTram ? giaTriDecimal : (decimal?)null;
            decimal? soTien = isPhanTram ? (decimal?)null : giaTriDecimal;

            // ── 4. Gọi BLL ───────────────────────────────────────────────
            try
            {
                if (_editingMa == null)
                    _bll.AddKhuyenMai(ten, phanTram, soTien, ngayBD, ngayKT, trangThai, moTa);
                else
                    _bll.UpdateKhuyenMai(_editingMa.Value, ten, phanTram, soTien,
                                          ngayBD, ngayKT, trangThai, moTa);

                LoadData();
                CloseForm();
            }
            catch (Exception ex)
            {
                // Lấy inner exception nếu có để rõ nguyên nhân hơn
                string msg = ex.InnerException?.Message ?? ex.Message;
                ShowError(msg);
            }
        }

        private void btnCloseForm_Click(object sender, RoutedEventArgs e)
            => CloseForm();

        // ── Form helpers ──────────────────────────────────────────────────
        private void OpenForm()
        {
            if (_formOpen) return;
            _formOpen = true;

            var anim = new GridLengthAnimation
            {
                From = new GridLength(0),
                To = new GridLength(360),
                Duration = new Duration(TimeSpan.FromMilliseconds(250))
            };
            FormColumn.BeginAnimation(ColumnDefinition.WidthProperty, anim);
        }

        private void CloseForm()
        {
            if (!_formOpen) return;
            _formOpen = false;

            var anim = new GridLengthAnimation
            {
                From = new GridLength(360),
                To = new GridLength(0),
                Duration = new Duration(TimeSpan.FromMilliseconds(200))
            };
            FormColumn.BeginAnimation(ColumnDefinition.WidthProperty, anim);

            _editingMa = null;
            ClearForm();
        }

        private void ClearForm()
        {
            txtTen.Text = "";
            txtGiaTri.Text = "";
            txtMoTa.Text = "";
            dpNgayBD.SelectedDate = null;
            dpNgayKT.SelectedDate = null;
            cboLoaiGiam.SelectedIndex = 0;
            cboTrangThai.SelectedIndex = 0;
            HideError();
        }

        private void ShowError(string msg)
        {
            txtError.Text = msg;
            errBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            errBorder.Visibility = Visibility.Collapsed;
            txtError.Text = "";
        }

        // ── Combo: Loại giảm ─────────────────────────────────────────────
        private void cboLoaiGiam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblGiaTri == null) return;
            lblGiaTri.Text = (cboLoaiGiam.SelectedIndex == 0)
                             ? "Phần trăm giảm (%) *"
                             : "Số tiền giảm (VNĐ) *";
        }
    }

    // ── GridLength animation helper ──────────────────────────────────────
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }
        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public override Type TargetPropertyType => typeof(GridLength);

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue,
                                               AnimationClock animationClock)
        {
            double progress = animationClock.CurrentProgress ?? 0;

            // Ease out cubic
            progress = 1 - Math.Pow(1 - progress, 3);

            double fromVal = From.Value;
            double toVal = To.Value;
            double current = fromVal + (toVal - fromVal) * progress;

            return new GridLength(current);
        }
    }
}