using DoAnCN_Net.BLL;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoAnCN_Net
{
    public partial class TrangChu_User : Window
    {
        private user_KhuyenMaiBLL khuyenMaiBLL;
        private user_KhachHangBLL khachHangBLL;

        public TrangChu_User()
        {
            InitializeComponent();
            khuyenMaiBLL = new user_KhuyenMaiBLL();
            khachHangBLL = new user_KhachHangBLL();
            LoadKhuyenMai();
            LoadUserInfo();
        }

        // ── Load dữ liệu ──────────────────────────────────

        private void LoadKhuyenMai()
        {
            try
            {
                var khuyenMais = khuyenMaiBLL.GetKhuyenMaiDangHoatDong();
                if (khuyenMais.Count > 0)
                    ItemsKhuyenMai.ItemsSource = khuyenMais;
                else
                    ShowSamplePromotions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải khuyến mãi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ShowSamplePromotions();
            }
        }

        private void ShowSamplePromotions()
        {
            var list = new List<KhuyenMaiViewModel>
            {
                new KhuyenMaiViewModel
                {
                    MaKhuyenMai   = 1,
                    TenKhuyenMai  = "GIẢM 10% TIỆC CƯỚI THÁNG 5",
                    BadgeText     = "🔥 GIẢM 10%",
                    GiaTriHienThi = "-10%",
                    MoTa          = "Giảm ngay 10% trên tổng hóa đơn tiệc cưới",
                    NgayBatDau    = DateTime.Now,
                    NgayKetThuc   = DateTime.Now.AddMonths(1),
                    TrangThai     = "Hoạt động",
                    SoNgayConLai  = 30
                },
                new KhuyenMaiViewModel
                {
                    MaKhuyenMai   = 2,
                    TenKhuyenMai  = "GIẢM 5.000.000đ CHO TIỆC TRÊN 50 TRIỆU",
                    BadgeText     = "💰 GIẢM 5,000,000 VNĐ",
                    GiaTriHienThi = "-5.000.000đ",
                    MoTa          = "Giảm ngay 5.000.000 VNĐ cho hóa đơn từ 30 triệu",
                    NgayBatDau    = DateTime.Now,
                    NgayKetThuc   = DateTime.Now.AddMonths(2),
                    TrangThai     = "Hoạt động",
                    SoNgayConLai  = 60
                },
                new KhuyenMaiViewModel
                {
                    MaKhuyenMai   = 3,
                    TenKhuyenMai  = "TẶNG MENU TRÁNG MIỆNG CAO CẤP",
                    BadgeText     = "🎁 QUÀ TẶNG",
                    GiaTriHienThi = "🎁",
                    MoTa          = "Tặng kèm menu tráng miệng cao cấp trị giá 2 triệu",
                    NgayBatDau    = DateTime.Now,
                    NgayKetThuc   = DateTime.Now.AddMonths(1),
                    TrangThai     = "Hoạt động",
                    SoNgayConLai  = 30
                }
            };
            ItemsKhuyenMai.ItemsSource = list;
        }

        private void LoadUserInfo()
        {
            try
            {
                var user = SessionManager.CurrentUser;
                if (user != null)
                    txtUserName.Text = user.TenKhachHang;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải thông tin user: {ex.Message}");
            }
        }

        // ── Navigation core ───────────────────────────────

        private void Navigate(string menuText)
        {
            switch (menuText)
            {
                case "TrangChu":
                    MainContent.Visibility = Visibility.Collapsed;
                    MainContent.Content = null;
                    gridTrangChu.Visibility = Visibility.Visible;
                    break;

                case "SanhTiec":
                    gridTrangChu.Visibility = Visibility.Collapsed;
                    MainContent.Visibility = Visibility.Visible;
                    MainContent.Content = new SanhUser_Control();
                    break;

                case "ThucDon":
                    user_XemChiTietMenuTrongDatSanh menu = new user_XemChiTietMenuTrongDatSanh();
                    menu.Show();
                    break;

                case "DatTiec":
                    gridTrangChu.Visibility = Visibility.Collapsed;
                    MainContent.Visibility = Visibility.Visible;
                    MainContent.Content = new SanhUser_Control();
                    break;

                case "LichSu":
                    gridTrangChu.Visibility = Visibility.Collapsed;
                    MainContent.Visibility = Visibility.Visible;
                    MainContent.Content = new LichSuDatTiec_Control();
                    break;
            

                case "KhuyenMai":
                    MessageBox.Show("Danh sách tất cả khuyến mãi đang cập nhật", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                default:
                    break;
            }
        }

        // ── Window Controls ───────────────────────────────

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
            => this.WindowState = WindowState.Minimized;

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                this.Close();
        }

        // ── Navigation Click Events ───────────────────────

        private void OpenTrangChu_Click(object sender, RoutedEventArgs e)
            => Navigate("TrangChu");

        private void OpenSanhList_Click(object sender, RoutedEventArgs e)
            => Navigate("SanhTiec");

        private void OpenMenuList_Click(object sender, RoutedEventArgs e)
            => Navigate("ThucDon");

        private void OpenBooking_Click(object sender, RoutedEventArgs e)
            => Navigate("DatTiec");

        private void OpenBookingHistory_Click(object sender, RoutedEventArgs e)
            => Navigate("LichSu");

        private void ViewAllPromotions_Click(object sender, RoutedEventArgs e)
            => Navigate("KhuyenMai");

        // ── Khuyến mãi ────────────────────────────────────

        private void ApplyPromotion_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int maKM = Convert.ToInt32(button.Tag);
                if (khuyenMaiBLL.KiemTraKhuyenMaiHopLe(maKM))
                {
                    MessageBox.Show($"Đã áp dụng mã khuyến mãi #{maKM}.\nLiên hệ nhân viên để được tư vấn chi tiết!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Khuyến mãi này đã hết hạn hoặc không còn hiệu lực!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadKhuyenMai();
                }
            }
        }

        //reload trang lich su
        public void RefreshLichSu()
        {
            // Kiểm tra nếu MainContent đang hiển thị LichSuDatTiec_Control
            if (MainContent.Visibility == Visibility.Visible &&
                MainContent.Content is LichSuDatTiec_Control lichSuControl)
            {
                lichSuControl.RefreshData();
            }
        }

        // ── Đăng xuất ─────────────────────────────────────

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SessionManager.CurrentUser = null;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}