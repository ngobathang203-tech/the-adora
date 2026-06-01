using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using System.Windows;
using DALMenu = DoAnCN_Net.DAL.Menu;

namespace DoAnCN_Net.UI
{
    public partial class MenuDialog : Window
    {
        private readonly int? _editingMa;
        private readonly menuBLL _bll = new menuBLL();

        // Add new
        public MenuDialog()
        {
            InitializeComponent();
            txtTitle.Text = "Them menu moi";
            btnSave.Content = "+ Them";
        }

        // Edit
        public MenuDialog(DALMenu menu)
        {
            InitializeComponent();
            _editingMa = menu.MaMenu;
            txtTitle.Text = "Chinh sua menu";
            btnSave.Content = "Luu";

            txtTenMenu.Text = menu.TenMenu;
            foreach (System.Windows.Controls.ComboBoxItem item in cboLoai.Items)
                if (item.Content.ToString() == menu.LoaiMenu)
                { item.IsSelected = true; break; }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlError.Visibility = Visibility.Collapsed;

            string ten = txtTenMenu.Text.Trim();
            string loai = (cboLoai.SelectedItem as System.Windows.Controls.ComboBoxItem)
                          ?.Content?.ToString() ?? "";

            try
            {
                if (_editingMa == null)
                    _bll.AddMenu(ten, loai);
                else
                    _bll.UpdateMenu(_editingMa.Value, ten, loai);

                DialogResult = true;
            }
            catch (System.Exception ex)
            {
                txtError.Text = ex.Message;
                pnlError.Visibility = Visibility.Visible;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void Header_MouseLeftButtonDown(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();
    }
}