    using DoAnCN_Net.BLL;
    using DoAnCN_Net.DAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    namespace DoAnCN_Net
    {
        public partial class ChiTietSanhWindow : Window
        {
            private readonly Sanh _sanh;
            private readonly user_SanhBLL _bll;
            private List<BLL.DichVuChon> _dsDichVu = new List<BLL.DichVuChon>();

            public event EventHandler DatTiecThanhCong;

            public ChiTietSanhWindow(Sanh sanh, user_SanhBLL bll)
            {
                InitializeComponent();
                _sanh = sanh;
                _bll = bll;

                HienThiThongTinSanh();
                LoadComboBoxes();
                LoadDichVu();
                LoadKhuyenMai();
                TinhTamTinh();

                var user = SessionManager.CurrentUser;
                if (user != null)
                {
                    txtTenKhach.Text = user.TenKhachHang;
                    txtSoDienThoai.Text = user.SoDienThoai;
                    txtEmail.Text = user.Email;
                }
            }

            // ── Load ──────────────────────────────────────────────────────

            private void HienThiThongTinSanh()
            {
                lblTenSanh.Text = _sanh.TenSanh;
                lblChiNhanh.Text = _sanh.TenChiNhanh;
                lblSucChua.Text = $"{_sanh.SucChua} người";
                lblGiaThue.Text = $"{_sanh.GiaThue:N0} VND";
                lblGiaSanh2.Text = $"{_sanh.GiaThue:N0} VND";
                lblTamTinh.Text = $"{_sanh.GiaThue:N0} VND";

                try
                {
                    string path = (_sanh.HinhAnh ?? "").TrimStart('/').Replace('/', '\\');
                    string full = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, path);
                    if (System.IO.File.Exists(full))
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(full, UriKind.Absolute);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        imgSanh.Source = bmp;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh: {ex.Message}");
                }
            }

            private void LoadComboBoxes()
            {
                var gios = new List<string>();
                for (int h = 6; h <= 23; h++) gios.Add($"{h:D2}:00");
                cboGioBatDau.ItemsSource = gios;
                cboGioKetThuc.ItemsSource = new List<string>(gios);
                cboGioBatDau.SelectedIndex = 0;
                cboGioKetThuc.SelectedIndex = 4;

                cboLoaiSuKien.ItemsSource = _bll.GetAllLoaiSuKien();
                cboMenu.ItemsSource = _bll.GetAllMenu();
            }

            private void LoadDichVu()
            {
                _dsDichVu = _bll.GetAllDichVu()
        .Select(d => new BLL.DichVuChon
        {
            MaDichVu = d.MaDichVu,
            TenDichVu = d.TenDichVu,
            Gia = d.Gia ?? 0,
            MoTa = d.MoTa
        }).ToList();
                icDichVu.ItemsSource = _dsDichVu;
            }

            private void LoadKhuyenMai()
            {
                var dsKM = _bll.GetKhuyenMaiHoatDong();
                dsKM.Insert(0, new KhuyenMai
                {
                    MaKhuyenMai = 0,
                    TenKhuyenMai = "-- Không áp dụng --"
                });
                cboKhuyenMai.ItemsSource = dsKM;
                cboKhuyenMai.SelectedIndex = 0;
            }

            // ── Tính tạm tính ─────────────────────────────────────────────

            private void TinhTamTinh()
            {
                decimal tongTien = _sanh.GiaThue ?? 0;

                // Cộng dịch vụ đã chọn
                decimal tongDV = _dsDichVu
                    .Where(d => d.DaChon)
                    .Sum(d => d.Gia);

                if (tongDV > 0)
                {
                    rowDichVu.Visibility = Visibility.Visible;
                    lblTongDichVu.Text = $"{tongDV:N0} VND";
                    tongTien += tongDV;
                }
                else
                {
                    rowDichVu.Visibility = Visibility.Collapsed;
                }

                // Trừ khuyến mãi
                decimal giamGia = 0;
                if (cboKhuyenMai.SelectedItem is KhuyenMai km && km.MaKhuyenMai > 0)
                {
                    if (km.PhanTramGiam.HasValue)
                        giamGia = tongTien * km.PhanTramGiam.Value / 100;
                    else if (km.SoTienGiam.HasValue)
                        giamGia = km.SoTienGiam.Value;

                    rowKM.Visibility = Visibility.Visible;
                    lblGiamGia.Text = $"{giamGia:N0} VND";
                    tongTien -= giamGia;
                }
                else
                {
                    rowKM.Visibility = Visibility.Collapsed;
                }

                lblTamTinh.Text = $"{tongTien:N0} VND";
            }

            // ── Event handlers ────────────────────────────────────────────

            private void TxtSoBan_Changed(object sender, TextChangedEventArgs e)
            {
                if (txtSoBan == null || txtKhachMoiBan == null) return;
                if (lblTongKhach == null || lblCanhBaoBan == null) return; // ← thêm dòng này

                int.TryParse(txtSoBan.Text, out int soBan);
                int.TryParse(txtKhachMoiBan.Text, out int khachMoiBan);
                int tongKhach = soBan * khachMoiBan;

                lblTongKhach.Text = $"→ Tổng: {tongKhach} khách";

                bool vuot = tongKhach > (_sanh.SucChua ?? 0);
                lblCanhBaoBan.Visibility = vuot
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            private void DichVu_Changed(object sender, RoutedEventArgs e)
                => TinhTamTinh();

            private void CboKhuyenMai_Changed(object sender, SelectionChangedEventArgs e)
            {
                if (cboKhuyenMai?.SelectedItem is KhuyenMai km && km.MaKhuyenMai > 0)
                {
                    lblMoTaKM.Text = km.PhanTramGiam.HasValue
                        ? $"Giảm {km.PhanTramGiam}% tổng hoá đơn"
                        : $"Giảm {km.SoTienGiam:N0} VND";
                    lblMoTaKM.Visibility = Visibility.Visible;
                }
                else
                {
                    lblMoTaKM.Visibility = Visibility.Collapsed;
                }
                TinhTamTinh();
            }

            private void Dp_Changed(object sender, SelectionChangedEventArgs e)
                => KiemTraTrungLich();

            private void KiemTraTrungLich()
            {
                if (dpNgayToChuc.SelectedDate == null ||
                    cboGioBatDau.SelectedItem == null ||
                    cboGioKetThuc.SelectedItem == null) return;

                if (!TimeSpan.TryParse(cboGioBatDau.SelectedItem.ToString(), out var gioBD) ||
                    !TimeSpan.TryParse(cboGioKetThuc.SelectedItem.ToString(), out var gioKT)) return;

                try
                {
                    bool trung = _bll.KiemTraTrungLich(
                        _sanh.MaSanh,
                        dpNgayToChuc.SelectedDate.Value,
                        gioBD, gioKT);
                    lblCanhBao.Visibility = trung
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi kiểm tra: {ex.Message}");
                }
            }

            // ── Đặt tiệc ──────────────────────────────────────────────────

            private void BtnDatTiec_Click(object sender, RoutedEventArgs e)
            {
                var user = SessionManager.CurrentUser;
                if (user == null)
                {
                    MessageBox.Show("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validation
                if (string.IsNullOrWhiteSpace(txtTenKhach.Text))
                { MessageBox.Show("Vui lòng nhập tên khách!"); return; }
                if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
                { MessageBox.Show("Vui lòng nhập số điện thoại!"); return; }
                if (dpNgayToChuc.SelectedDate == null)
                { MessageBox.Show("Vui lòng chọn ngày tổ chức!"); return; }
                if (dpNgayToChuc.SelectedDate.Value.Date < DateTime.Today)
                { MessageBox.Show("Ngày tổ chức không được trong quá khứ!"); return; }
                if (cboLoaiSuKien.SelectedValue == null)
                { MessageBox.Show("Vui lòng chọn loại sự kiện!"); return; }
                if (cboMenu.SelectedValue == null)
                { MessageBox.Show("Vui lòng chọn menu!"); return; }
                if (!TimeSpan.TryParse(cboGioBatDau.SelectedItem?.ToString(), out var gioBD) ||
                    !TimeSpan.TryParse(cboGioKetThuc.SelectedItem?.ToString(), out var gioKT))
                { MessageBox.Show("Vui lòng chọn giờ!"); return; }
                if (gioBD >= gioKT)
                { MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu!"); return; }
                if (lblCanhBao.Visibility == Visibility.Visible)
                { MessageBox.Show("Sảnh đã có lịch trong khung giờ này!"); return; }
                if (!int.TryParse(txtSoBan.Text, out int soBan) || soBan <= 0)
                { MessageBox.Show("Số bàn không hợp lệ!"); return; }
                if (!int.TryParse(txtKhachMoiBan.Text, out int khachMoiBan) || khachMoiBan <= 0)
                { MessageBox.Show("Số khách/bàn không hợp lệ!"); return; }
                if (lblCanhBaoBan.Visibility == Visibility.Visible)
                { MessageBox.Show("Tổng khách vượt sức chứa sảnh!"); return; }

                // Khuyến mãi
                int? maKM = null;
                if (cboKhuyenMai.SelectedItem is KhuyenMai km && km.MaKhuyenMai > 0)
                    maKM = km.MaKhuyenMai;

                try
                {
                    _bll.DatSanhDayDu(
                        sanh: _sanh,
                        tenKhach: txtTenKhach.Text.Trim(),
                        sdt: txtSoDienThoai.Text.Trim(),
                        email: txtEmail.Text.Trim(),
                        tenSuKien: txtTenSuKien.Text.Trim(),
                        maLoai: (int)cboLoaiSuKien.SelectedValue,
                        maMenu: (int)cboMenu.SelectedValue,
                        ngay: dpNgayToChuc.SelectedDate.Value,
                        gioBD: gioBD,
                        gioKT: gioKT,
                        maKhachHang: user.MaKhachHang,
                        soBan: soBan,
                        khachMoiBan: khachMoiBan,
                        dsDichVuChon: _dsDichVu,
                        maKhuyenMai: maKM
                    );

                    MessageBox.Show(
                        "🎉 Đặt tiệc thành công!\nChúng tôi sẽ liên hệ xác nhận sớm nhất.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    DatTiecThanhCong?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            private void BtnXemMenu_Click(object sender, RoutedEventArgs e)
            {
                var win = new user_XemChiTietMenuTrongDatSanh();
                win.Owner = this;
                win.ShowDialog();
            }

            private void BtnHuy_Click(object sender, RoutedEventArgs e) => this.Close();
        }
    }