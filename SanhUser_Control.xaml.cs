using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class SanhUser_Control : UserControl
    {
        private readonly user_SanhBLL bll = new user_SanhBLL();

        public SanhUser_Control()
        {
            InitializeComponent();
            LoadChiNhanhFilter();
            LoadSanh();
        }

        private void LoadSanh(string tuKhoa = "", int maChiNhanh = 0)
        {
            icSanh.ItemsSource = bll.Search(tuKhoa, maChiNhanh);
        }

        private void LoadChiNhanhFilter()
        {
            var dsChiNhanh = bll.GetAllChiNhanh();
            // Thêm item "Tất cả" đầu list
            dsChiNhanh.Insert(0, new ChiNhanh { MaChiNhanh = 0, TenChiNhanh = "-- Tất cả chi nhánh --" });
            cboLocChiNhanh.ItemsSource = dsChiNhanh;
            cboLocChiNhanh.SelectedIndex = 0;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadSanh(txtSearch.Text.Trim(), GetMaChiNhanh());
        }

        private void CboLocChiNhanh_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadSanh(txtSearch.Text.Trim(), GetMaChiNhanh());
        }

        private void BtnXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cboLocChiNhanh.SelectedIndex = 0;
            LoadSanh();
        }

        private int GetMaChiNhanh()
        {
            if (cboLocChiNhanh.SelectedItem is ChiNhanh cn)
                return cn.MaChiNhanh;
            return 0;
        }

        private void BtnDatSanh_Click(object sender, RoutedEventArgs e)
        {
            var sanh = (sender as Button)?.Tag as Sanh;
            if (sanh == null) return;

            var popup = new ChiTietSanhWindow(sanh, bll);   // form đặt tiệc cũ — giữ nguyên
            popup.Owner = Window.GetWindow(this);
            popup.DatTiecThanhCong += (s, ev) =>
                (Window.GetWindow(this) as TrangChu_User)?.RefreshLichSu();
            popup.ShowDialog();
        }

        private void BtnXemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            var sanh = (sender as Button)?.Tag as Sanh;
            if (sanh == null) return;

            var popup = new XemChiTietSanhWindow(sanh, bll);  // cửa sổ mới bên dưới
            popup.Owner = Window.GetWindow(this);
            // Nếu user bấm "Đặt ngay" từ trong cửa sổ chi tiết
            popup.YeuCauDatSanh += (s, ev) =>
            {
                popup.Close();
                BtnDatSanh_Click(
                    new Button { Tag = sanh },
                    new RoutedEventArgs());
            };
            popup.ShowDialog();
        }
    }
}