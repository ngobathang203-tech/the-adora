using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Windows;
using System.Windows.Controls;

namespace DoAnCN_Net
{
    public partial class user_XemChiTietMenuTrongDatSanh : Window
    {
        private readonly menuBLL _bll = new menuBLL();

        public user_XemChiTietMenuTrongDatSanh()
        {
            InitializeComponent();

            LoadMenu();
        }

        void LoadMenu()
        {
            lstMenu.ItemsSource = _bll.GetAllMenus();
        }

        private void lstMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoAnCN_Net.DAL.Menu selected =
                lstMenu.SelectedItem as DoAnCN_Net.DAL.Menu;

            if (selected != null)
            {
                txtTenMenu.Text = selected.TenMenu;
                txtLoaiMenu.Text = selected.LoaiMenu;

                dgChiTiet.ItemsSource =
                    _bll.GetChiTietMenu(selected.MaMenu);
            }
        }
    }
}