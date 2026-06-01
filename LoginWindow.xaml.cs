using DoAnCN_Net.BLL;
using System;
using System.Windows;

namespace DoAnCN_Net
{
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        // ── Toggle panels ──────────────────────────────────
        private void ShowRegister_Click(object sender, RoutedEventArgs e)
        {
            CardLogin.Visibility = Visibility.Collapsed;
            CardRegister.Visibility = Visibility.Visible;
        }

        private void ShowLogin_Click(object sender, RoutedEventArgs e)
        {
            CardRegister.Visibility = Visibility.Collapsed;
            CardLogin.Visibility = Visibility.Visible;
        }

        // ── Toggle hiện/ẩn mật khẩu ───────────────────────
        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                txtPassVisible.Text = txtPass.Password;
                txtPassVisible.Visibility = Visibility.Visible;
                txtPass.Visibility = Visibility.Collapsed;
                btnTogglePass.Content = "🙈";
            }
            else
            {
                txtPass.Password = txtPassVisible.Text;
                txtPass.Visibility = Visibility.Visible;
                txtPassVisible.Visibility = Visibility.Collapsed;
                btnTogglePass.Content = "👁";
            }
        }

        // ── Đăng nhập ──────────────────────────────────────
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = txtUser.Text.Trim();
                string pass = _isPasswordVisible ? txtPassVisible.Text : txtPass.Password;

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoginBLL bll = new LoginBLL();
                var a = bll.CheckLogin(user, pass);

                if (a != null)
                {
                    SessionManager.CurrentUser = a;

                    if (a.TenKhachHang == "a")
                    {
                        AdminWindow main = new AdminWindow();
                        main.Show();
                    }
                    else
                    {
                        TrangChu_User main = new TrangChu_User();
                        main.Show();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message + "\n\n" +
                                (ex.InnerException?.Message ?? ""),
                                "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Đăng ký ────────────────────────────────────────
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = txtRegName.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string phone = txtRegPhone.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng nhập đủ họ tên và số điện thoại!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RegisterBLL bll = new RegisterBLL();
            string result = bll.Register(name, phone, email);

            if (result == "OK")
            {
                MessageBox.Show("Đăng ký thành công!\nDùng Họ tên + Số điện thoại để đăng nhập.",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                txtUser.Text = name;
                txtPass.Password = phone;
                ShowLogin_Click(null, null);
            }
            else
            {
                MessageBox.Show(result, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}