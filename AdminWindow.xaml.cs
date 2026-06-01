    using DoAnCN_Net.UI;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    namespace DoAnCN_Net
    {
        public partial class AdminWindow : Window
        {
            private bool isMenuCollapsed = false;
            private double originalMenuWidth = 280;
            private double collapsedMenuWidth = 65;

            public AdminWindow()
            {
                InitializeComponent();
                LoadDashboard();

                // Đảm bảo ListView có thể focus và nhận sự kiện
                lvMenu.Focusable = true;
                lvMenu.IsEnabled = true;
            }

            private void LoadDashboard()
            {
                // Mặc định chọn item đầu tiên
                if (lvMenu.Items.Count > 0)
                {
                    var firstItem = lvMenu.Items[0] as ListViewItem;
                    if (firstItem != null)
                    {
                        firstItem.IsSelected = true;
                        txtBreadcrumb.Text = firstItem.Content.ToString();
                    }
                }

                // Tạm thời hiển thị trang mặc định
                MainFrame.Content = new DashboardView();
            }

            private void ToggleMenu(object sender, RoutedEventArgs e)
            {
                isMenuCollapsed = !isMenuCollapsed;

                if (isMenuCollapsed)
                {
                    MenuColumn.Width = new GridLength(collapsedMenuWidth);
                    btnToggleMenu.Content = "☰";
                }
                else
                {
                    MenuColumn.Width = new GridLength(originalMenuWidth);
                    btnToggleMenu.Content = "☰";
                }
            }

            private void lvMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (lvMenu.SelectedItem == null) return;

                ListViewItem selectedItem = lvMenu.SelectedItem as ListViewItem;
                if (selectedItem == null) return;

                string menuText = selectedItem.Content.ToString();

                // Cập nhật breadcrumb
                txtBreadcrumb.Text = menuText;

                // Chuyển trang dựa trên menu được chọn
                switch (menuText)
                {
                    case "Tổng quan":
                        MainFrame.Content = new DashboardView();
                        break;
                    case "Chi nhánh":
                        MainFrame.Content = new ChiNhanhView();
                        break;
                    case "Sảnh":
                        MainFrame.Content = new SanhControl();
                        break;
                    case "Khách hàng":
                        MainFrame.Content = new KhachHangControl();
                        break;
                    case "Đặt sảnh":
                        MainFrame.Content = new DatSanhControl(); 
                        break;
                    
                    case "Menu":
                        MainFrame.Content = new MenuAdmin();
                        break;
                    case "Món ăn":
                        MainFrame.Content = new MonAnControl();

                        break;
                    case "Dịch vụ":
                        MainFrame.Content = new DichVuControl();

                        break;
                    case "Khuyến mãi":
                    MainFrame.Content = new KhuyenMaiControl();

                    break;
                    case "Hóa đơn":
                    MainFrame.Content = new HoaDonControl();
                    break;
                    case "Đổi lịch":
                    MainFrame.Content = new DoiLichControl();
                    break;
                    default:
                        break;
                }
            }

            // Custom Window Controls
            private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (e.ClickCount == 2)
                {
                    MaximizeWindow_Click(sender, e);
                }
                else
                {
                    this.DragMove();
                }
            }

            private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
            {
                this.WindowState = WindowState.Minimized;
            }

            private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    btnMaximize.Content = "□";
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    btnMaximize.Content = "❐";
                }
            }

            private void CloseWindow_Click(object sender, RoutedEventArgs e)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?", "Xác nhận",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }

            private void Logout_Click(object sender, MouseButtonEventArgs e)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Mở lại login window
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
        }
    }