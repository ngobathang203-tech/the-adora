using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class SanhControl : UserControl
    {
        sanhBLL bll = new sanhBLL();
        ChiNhanhBLL cnBLL = new ChiNhanhBLL();
        private int? currentSelectedMaSanh = null;

        public class SanhDisplay
        {
            public int MaSanh { get; set; }
            public string TenSanh { get; set; }
            public int SucChua { get; set; }
            public decimal GiaThue { get; set; }
            public int MaChiNhanh { get; set; }
            public string HinhAnh { get; set; }
            public string TenChiNhanh { get; set; }
        }

        public SanhControl()
        {
            InitializeComponent();
            LoadChiNhanh();   // load combo trước
            LoadData();
        }

        // ═══════════════════════════════════
        // DATA
        // ═══════════════════════════════════

        void LoadData()
        {
            try
            {
                var list = bll.GetAll();
                var chiNhanhs = cnBLL.GetAll();

                var displayList = new List<SanhDisplay>();
                foreach (var sanh in list)
                {
                    var cn = chiNhanhs.FirstOrDefault(c => c.MaChiNhanh == sanh.MaChiNhanh);
                    displayList.Add(new SanhDisplay
                    {
                        MaSanh = sanh.MaSanh,
                        TenSanh = sanh.TenSanh,
                        SucChua = sanh.SucChua ?? 0,
                        GiaThue = sanh.GiaThue ?? 0,
                        MaChiNhanh = sanh.MaChiNhanh ?? 0,
                        HinhAnh = sanh.HinhAnh,
                        TenChiNhanh = cn?.TenChiNhanh ?? "Không xác định"
                    });
                }

                icSanh.ItemsSource = displayList;

                // Cập nhật stat header
                if (lblTongSanh != null) lblTongSanh.Text = displayList.Count.ToString();
                if (lblSubtitle != null) lblSubtitle.Text = $"{displayList.Count} sảnh đang hoạt động";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadChiNhanh()
        {
            try
            {
                var list = cnBLL.GetAll();
                cboChiNhanh.ItemsSource = list;
                cboChiNhanh.SelectedValuePath = "MaChiNhanh";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải chi nhánh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // FORM HELPERS
        // ═══════════════════════════════════

        private void ClearForm()
        {
            txtTenSanh.Text = "";
            txtSucChua.Text = "";
            txtGiaThue.Text = "";
            cboChiNhanh.SelectedIndex = -1;
            txtHinhAnh.Text = "";
            currentSelectedMaSanh = null;
        }

        // ═══════════════════════════════════
        // TOOLBAR
        // ═══════════════════════════════════

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            LoadData();
            txtSearch.Text = "";
        }

        // ═══════════════════════════════════
        // SEARCH
        // ═══════════════════════════════════

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                LoadData();
            else
                SearchData();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => SearchData();

        private void SearchData()
        {
            try
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(keyword)) { LoadData(); return; }

                var allData = bll.GetAll();
                var chiNhanhs = cnBLL.GetAll();

                var displayList = allData.Select(sanh =>
                {
                    var cn = chiNhanhs.FirstOrDefault(c => c.MaChiNhanh == sanh.MaChiNhanh);
                    return new SanhDisplay
                    {
                        MaSanh = sanh.MaSanh,
                        TenSanh = sanh.TenSanh,
                        SucChua = sanh.SucChua ?? 0,
                        GiaThue = sanh.GiaThue ?? 0,
                        MaChiNhanh = sanh.MaChiNhanh ?? 0,
                        HinhAnh = sanh.HinhAnh,
                        TenChiNhanh = cn?.TenChiNhanh ?? "Không xác định"
                    };
                }).ToList();

                icSanh.ItemsSource = displayList.Where(s =>
                    s.TenSanh.ToLower().Contains(keyword) ||
                    s.SucChua.ToString().Contains(keyword) ||
                    s.GiaThue.ToString().Contains(keyword) ||
                    (s.TenChiNhanh?.ToLower().Contains(keyword) == true)
                ).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // CHỌN ẢNH
        // ═══════════════════════════════════

        private void BtnChonAnh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Title = "Chọn ảnh sảnh"
                };

                if (dlg.ShowDialog() == true)
                {
                    string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = DateTime.Now.Ticks + "_" + Path.GetFileName(dlg.FileName);
                    string dest = Path.Combine(folder, fileName);
                    File.Copy(dlg.FileName, dest, true);
                    txtHinhAnh.Text = "Images/" + fileName;

                    MessageBox.Show("Đã chọn ảnh thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi chọn ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // THÊM
        // ═══════════════════════════════════

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm(out int sucChua, out decimal giaThue)) return;

                var sanhMoi = new Sanh
                {
                    TenSanh = txtTenSanh.Text.Trim(),
                    SucChua = sucChua,
                    GiaThue = giaThue,
                    MaChiNhanh = (int)cboChiNhanh.SelectedValue,
                    HinhAnh = string.IsNullOrWhiteSpace(txtHinhAnh.Text) ? null : txtHinhAnh.Text.Trim()
                };

                if (bll.Add(sanhMoi))
                {
                    MessageBox.Show("Thêm sảnh thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // CẬP NHẬT
        // ═══════════════════════════════════

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!currentSelectedMaSanh.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn sảnh cần cập nhật (bấm 'Chỉnh sửa' trên card)!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidateForm(out int sucChua, out decimal giaThue)) return;

                var sanhCapNhat = new Sanh
                {
                    MaSanh = currentSelectedMaSanh.Value,
                    TenSanh = txtTenSanh.Text.Trim(),
                    SucChua = sucChua,
                    GiaThue = giaThue,
                    MaChiNhanh = (int)cboChiNhanh.SelectedValue,
                    HinhAnh = string.IsNullOrWhiteSpace(txtHinhAnh.Text) ? null : txtHinhAnh.Text.Trim()
                };

                if (bll.Update(sanhCapNhat))
                {
                    MessageBox.Show("Cập nhật sảnh thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // XÓA (từ form)
        // ═══════════════════════════════════

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!currentSelectedMaSanh.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn sảnh cần xóa (bấm 'Chỉnh sửa' trên card)!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (bll.IsBeingUsed(currentSelectedMaSanh.Value))
                {
                    MessageBox.Show("Không thể xóa sảnh này vì đã có đơn đặt hàng!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn chắc chắn muốn xóa sảnh '{txtTenSanh.Text}'?",
                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    if (bll.Delete(currentSelectedMaSanh.Value))
                    {
                        MessageBox.Show("Xóa sảnh thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // XÓA (trực tiếp trên card)
        // ═══════════════════════════════════

        private void BtnXoaSanh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sanh = (sender as Button)?.DataContext as SanhDisplay;
                if (sanh == null) return;

                if (bll.IsBeingUsed(sanh.MaSanh))
                {
                    MessageBox.Show("Không thể xóa sảnh này vì đã có đơn đặt hàng!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn chắc chắn muốn xóa sảnh '{sanh.TenSanh}'?",
                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    if (bll.Delete(sanh.MaSanh))
                    {
                        MessageBox.Show("Xóa sảnh thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        if (currentSelectedMaSanh == sanh.MaSanh) ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════
        // CHỌN SỬA (từ card)
        // ═══════════════════════════════════

        private void BtnChonSanh_Click(object sender, RoutedEventArgs e)
        {
            var sanh = (sender as Button)?.DataContext as SanhDisplay;
            if (sanh == null) return;

            txtTenSanh.Text = sanh.TenSanh;
            txtSucChua.Text = sanh.SucChua.ToString();
            txtGiaThue.Text = sanh.GiaThue.ToString();
            cboChiNhanh.SelectedValue = sanh.MaChiNhanh;
            txtHinhAnh.Text = sanh.HinhAnh;
            currentSelectedMaSanh = sanh.MaSanh;

            MessageBox.Show($"Đã chọn: {sanh.TenSanh}\nBạn có thể cập nhật hoặc xóa.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ═══════════════════════════════════
        // XEM CHI TIẾT
        // ═══════════════════════════════════

        private void BtnChiTiet_Click(object sender, RoutedEventArgs e)
        {
            var sanh = (sender as Button)?.DataContext as SanhDisplay;
            if (sanh == null) return;

            MessageBox.Show(
                $"THÔNG TIN SẢNH\n\n" +
                $"Tên sảnh:   {sanh.TenSanh}\n" +
                $"Sức chứa:   {sanh.SucChua} người\n" +
                $"Giá thuê:   {sanh.GiaThue:N0} VND\n" +
                $"Chi nhánh:  {sanh.TenChiNhanh}",
                "Chi tiết sảnh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ═══════════════════════════════════
        // VALIDATE
        // ═══════════════════════════════════

        private bool ValidateForm(out int sucChua, out decimal giaThue)
        {
            sucChua = 0; giaThue = 0;

            if (string.IsNullOrWhiteSpace(txtTenSanh.Text))
            { MessageBox.Show("Vui lòng nhập tên sảnh!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }

            if (!int.TryParse(txtSucChua.Text, out sucChua) || sucChua <= 0)
            { MessageBox.Show("Sức chứa phải là số nguyên lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }

            if (!decimal.TryParse(txtGiaThue.Text, out giaThue) || giaThue < 0)
            { MessageBox.Show("Giá thuê phải là số lớn hơn hoặc bằng 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }

            if (cboChiNhanh.SelectedValue == null)
            { MessageBox.Show("Vui lòng chọn chi nhánh!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }

            return true;
        }
    }
}