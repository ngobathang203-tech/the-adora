using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class KhachHangControl : UserControl
    {
        khachHangBLL bll = new khachHangBLL();
        private int? currentSelectedId = null;

        public KhachHangControl()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            try
            {
                var list = bll.GetAll();
                dgKhachHang.ItemsSource = list;
                txtThongBaoTimKiem.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtTenKhachHang.Text = "";
            txtSoDienThoai.Text = "";
            txtEmail.Text = "";
            txtMaKhachHang.Text = "";
            currentSelectedId = null;
        }

        private void ClearSearch()
        {
            txtTimKiem.Text = "";
            cboKieuTimKiem.SelectedIndex = 0;
            txtThongBaoTimKiem.Visibility = Visibility.Collapsed;
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            ClearSearch();
            LoadData();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtTenKhachHang.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo đối tượng mới
                KhachHang khachHang = new KhachHang
                {
                    TenKhachHang = txtTenKhachHang.Text.Trim(),
                    SoDienThoai = txtSoDienThoai.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
                };

                bool result = bll.Add(khachHang);

                if (result)
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!currentSelectedId.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTenKhachHang.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khách hàng!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                KhachHang khachHang = new KhachHang
                {
                    MaKhachHang = currentSelectedId.Value,
                    TenKhachHang = txtTenKhachHang.Text.Trim(),
                    SoDienThoai = txtSoDienThoai.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
                };

                bool result = bll.Update(khachHang);

                if (result)
                {
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!currentSelectedId.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa khách hàng '{txtTenKhachHang.Text}'?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = bll.Delete(currentSelectedId.Value);

                    if (success)
                    {
                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKhachHang.SelectedItem != null)
            {
                var khachHang = dgKhachHang.SelectedItem as KhachHang;
                if (khachHang != null)
                {
                    txtTenKhachHang.Text = khachHang.TenKhachHang;
                    txtSoDienThoai.Text = khachHang.SoDienThoai;
                    txtEmail.Text = khachHang.Email;
                    txtMaKhachHang.Text = khachHang.MaKhachHang.ToString();
                    currentSelectedId = khachHang.MaKhachHang;
                }
            }
        }

        private void BtnXemLichSu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int maKhachHang = (int)btn.Tag;
                var khachHang = bll.GetById(maKhachHang);
                var lichSu = bll.GetBookingHistory(maKhachHang);

                if (lichSu.Count == 0)
                {
                    MessageBox.Show($"Khách hàng '{khachHang.TenKhachHang}' chưa có lịch sử đặt sảnh!",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Hiển thị lịch sử đặt sảnh
                string history = $"LỊCH SỬ ĐẶT SẢNH CỦA {khachHang.TenKhachHang.ToUpper()}\n\n";
                int stt = 1;
                foreach (var dat in lichSu)
                {
                    history += $"{stt}. 📅 Ngày: {dat.NgayToChuc:dd/MM/yyyy}\n";
                    history += $"   ⏰ Giờ: {dat.GioBatDau} - {dat.GioKetThuc}\n";
                    history += $"   🎉 Sự kiện: {dat.TenSuKien}\n";
                    history += $"   📌 Trạng thái: {dat.TrangThai}\n";
                    history += $"   ━━━━━━━━━━━━━━━━━━━━━\n";
                    stt++;
                }

                MessageBox.Show(history, "Lịch sử đặt sảnh",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cập nhật: Tìm kiếm theo tên hoặc họ
        private void BtnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyword = txtTimKiem.Text.Trim();
                string kieuTimKiem = (cboKieuTimKiem.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    LoadData();
                    return;
                }

                var allCustomers = bll.GetAll();
                var result = allCustomers.Where(k => IsMatch(k, keyword, kieuTimKiem)).ToList();

                dgKhachHang.ItemsSource = result;

                if (result.Count == 0)
                {
                    txtThongBaoTimKiem.Text = $"🔍 Không tìm thấy khách hàng nào với từ khóa '{keyword}' theo kiểu tìm '{kieuTimKiem}'";
                    txtThongBaoTimKiem.Visibility = Visibility.Visible;
                }
                else
                {
                    txtThongBaoTimKiem.Visibility = Visibility.Collapsed;
                    MessageBox.Show($"Tìm thấy {result.Count} kết quả!", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm kiểm tra khách hàng có khớp với từ khóa tìm kiếm không
        private bool IsMatch(KhachHang kh, string keyword, string kieuTimKiem)
        {
            keyword = keyword.ToLower().Trim();

            switch (kieuTimKiem)
            {
                case "Theo họ":
                    // Lấy họ (phần đầu tiên của tên)
                    string ho = kh.TenKhachHang.Trim().Split(' ')[0];
                    return ho.ToLower().Contains(keyword);

                case "Theo tên":
                    // Lấy tên (phần cuối cùng của tên)
                    string ten = kh.TenKhachHang.Trim().Split(' ').Last();
                    return ten.ToLower().Contains(keyword);

                case "Theo họ và tên":
                    return kh.TenKhachHang.ToLower().Contains(keyword);

                case "Theo số điện thoại":
                    return kh.SoDienThoai.Contains(keyword);

                case "Theo email":
                    return kh.Email != null && kh.Email.ToLower().Contains(keyword);

                default:
                    // Tìm kiếm tất cả
                    return kh.TenKhachHang.ToLower().Contains(keyword) ||
                           kh.SoDienThoai.Contains(keyword) ||
                           (kh.Email != null && kh.Email.ToLower().Contains(keyword));
            }
        }

        // Tìm kiếm theo tên (gõ nhanh)
        private void TxtTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                LoadData();
            }
            else
            {
                // Tự động tìm kiếm khi gõ (nếu muốn)
                // BtnTimKiem_Click(sender, null);
            }
        }

        // Tìm kiếm nâng cao: Tách họ và tên riêng
        private void BtnTimKiemNangCao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ho = txtHo.Text.Trim();
                string ten = txtTen.Text.Trim();
                string soDienThoai = txtSoDienThoaiTim.Text.Trim();
                string email = txtEmailTim.Text.Trim();

                if (string.IsNullOrWhiteSpace(ho) && string.IsNullOrWhiteSpace(ten) &&
                    string.IsNullOrWhiteSpace(soDienThoai) && string.IsNullOrWhiteSpace(email))
                {
                    LoadData();
                    return;
                }

                var allCustomers = bll.GetAll();
                var result = allCustomers.Where(k =>
                    (string.IsNullOrWhiteSpace(ho) || k.TenKhachHang.Split(' ')[0].ToLower().Contains(ho.ToLower())) &&
                    (string.IsNullOrWhiteSpace(ten) || k.TenKhachHang.Split(' ').Last().ToLower().Contains(ten.ToLower())) &&
                    (string.IsNullOrWhiteSpace(soDienThoai) || k.SoDienThoai.Contains(soDienThoai)) &&
                    (string.IsNullOrWhiteSpace(email) || (k.Email != null && k.Email.ToLower().Contains(email.ToLower())))
                ).ToList();

                dgKhachHang.ItemsSource = result;

                if (result.Count == 0)
                {
                    txtThongBaoTimKiem.Text = "🔍 Không tìm thấy khách hàng nào với điều kiện tìm kiếm!";
                    txtThongBaoTimKiem.Visibility = Visibility.Visible;
                }
                else
                {
                    txtThongBaoTimKiem.Visibility = Visibility.Collapsed;
                    MessageBox.Show($"Tìm thấy {result.Count} kết quả!", "Thông báo",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnXoaTimKiem_Click(object sender, RoutedEventArgs e)
        {
            ClearSearch();
            LoadData();
        }
    }
}