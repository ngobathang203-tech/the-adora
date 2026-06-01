using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class ChiNhanhView : UserControl
    {
        ChiNhanhBLL bll = new ChiNhanhBLL();
        private int? currentSelectedId = null;

        public ChiNhanhView()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            try
            {
                var list = bll.GetAll();
                dgChiNhanh.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtTen.Text = "";
            txtDiaChi.Text = "";
            txtSDT.Text = "";
            currentSelectedId = null;
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            LoadData();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chi nhánh!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSDT.Text))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ChiNhanh chiNhanh = new ChiNhanh
                {
                    TenChiNhanh = txtTen.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim(),
                    SoDienThoai = txtSDT.Text.Trim()
                };

                bool result = bll.Add(chiNhanh);

                if (result)
                {
                    MessageBox.Show("Thêm chi nhánh thành công!", "Thông báo",
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
                    MessageBox.Show("Vui lòng chọn chi nhánh cần cập nhật!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chi nhánh!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ChiNhanh chiNhanh = new ChiNhanh
                {
                    MaChiNhanh = currentSelectedId.Value,
                    TenChiNhanh = txtTen.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim(),
                    SoDienThoai = txtSDT.Text.Trim()
                };

                bool result = bll.Update(chiNhanh);

                if (result)
                {
                    MessageBox.Show("Cập nhật chi nhánh thành công!", "Thông báo",
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
                    MessageBox.Show("Vui lòng chọn chi nhánh cần xóa!", "Cảnh báo",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa chi nhánh '{txtTen.Text}'?\nLưu ý: Sẽ xóa tất cả sảnh thuộc chi nhánh này!",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = bll.Delete(currentSelectedId.Value);

                    if (success)
                    {
                        MessageBox.Show("Xóa chi nhánh thành công!", "Thông báo",
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

        private void dgChiNhanh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgChiNhanh.SelectedItem != null)
            {
                var chiNhanh = dgChiNhanh.SelectedItem as ChiNhanh;
                if (chiNhanh != null)
                {
                    txtTen.Text = chiNhanh.TenChiNhanh;
                    txtDiaChi.Text = chiNhanh.DiaChi;
                    txtSDT.Text = chiNhanh.SoDienThoai;
                    currentSelectedId = chiNhanh.MaChiNhanh;
                }
            }
        }
    }
}