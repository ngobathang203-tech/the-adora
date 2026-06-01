using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DoAnCN_Net
{
    public partial class XemChiTietSanhWindow : Window
    {
        private readonly Sanh _sanh;
        private readonly user_SanhBLL _bll;
        private readonly DanhGiaSanhBLL _dgBll = new DanhGiaSanhBLL(); // ← BLL riêng

        public event EventHandler YeuCauDatSanh;

        public XemChiTietSanhWindow(Sanh sanh, user_SanhBLL bll)
        {
            InitializeComponent();
            _sanh = sanh;
            _bll = bll;

            HienThiThongTin();
            LoadDichVu();
            LoadDanhGia();
        }

        private void HienThiThongTin()
        {
            lblTenSanh.Text = _sanh.TenSanh;
            lblChiNhanh.Text = "📍 " + _sanh.TenChiNhanh;
            lblSucChua.Text = $"{_sanh.SucChua} người";
            lblGia.Text = $"{_sanh.GiaThue:N0} VND";

            // Địa chỉ lấy từ ChiNhanh qua user_SanhBLL
            string diaChi = _bll.GetDiaChiChiNhanh(_sanh.MaChiNhanh ?? 0);
            lblDiaChi.Text = string.IsNullOrWhiteSpace(diaChi)
                             ? _sanh.TenChiNhanh
                             : $"{_sanh.TenChiNhanh} — {diaChi}";

            // Rating dùng DanhGiaSanhBLL riêng
            double rating = _dgBll.GetRating(_sanh.MaSanh);
            lblSao.Text = rating > 0 ? rating.ToString("F1") : "Chưa có";

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
            catch { }
        }

        private void LoadDichVu()
        {

            icDichVu.ItemsSource = _bll.GetDichVuBySanh(_sanh.MaSanh);
        }

        private void LoadDanhGia()
        {
            icDanhGia.ItemsSource = _dgBll.GetBySanh(_sanh.MaSanh); // ← dùng _dgBll
        }

        private void BtnGuiDanhGia_Click(object sender, RoutedEventArgs e)
        {
            var user = SessionManager.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để đánh giá!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int soSao = cboSoSao.SelectedIndex + 1;
            string noiDung = txtDanhGia.Text.Trim();

            try
            {
                _dgBll.GuiDanhGia(_sanh.MaSanh, user.MaKhachHang, soSao, noiDung);

                MessageBox.Show("Cảm ơn bạn đã đánh giá!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                txtDanhGia.Text = "";
                cboSoSao.SelectedIndex = 4;

                LoadDanhGia();  // refresh list

                double rating = _dgBll.GetRating(_sanh.MaSanh);
                lblSao.Text = rating.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDatNgay_Click(object sender, RoutedEventArgs e)
            => YeuCauDatSanh?.Invoke(this, EventArgs.Empty);

        private void BtnDong_Click(object sender, RoutedEventArgs e)
            => this.Close();
    }
}