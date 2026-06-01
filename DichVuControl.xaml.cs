        using DoAnCN_Net.BLL;
        using DoAnCN_Net.DAL;
        using System.Collections.Generic;
        using System.Linq;
        using System.Windows;
        using System.Windows.Controls;

        namespace DoAnCN_Net.UI
        {
            public partial class DichVuControl : UserControl
            {
                private readonly dichVuBLL _bll = new dichVuBLL();
                private List<DichVuRow> _allRows = new List<DichVuRow>();

                private class DichVuRow
                {
                    public int MaDichVu { get; set; }
                    public string TenDichVu { get; set; }
                    public decimal Gia { get; set; }
                    public int SoSuKien { get; set; }
                    public string MoTa { get; set; }
                }

                public DichVuControl()
                {
                    InitializeComponent();
                    Loaded += (s, e) => LoadData();
                }

                private void LoadData()
                {
                    try
                    {
                using (var db = dataDataContext.Create())
                {
                    MessageBox.Show("Đang kết nối: " + db.Connection.ConnectionString);
                }
                var dsDichVu = _bll.GetAll();
                        var soSuKienMap = _bll.GetSoSuKienDung();

                        _allRows = dsDichVu.Select(d => new DichVuRow
                        {
                            MaDichVu = d.MaDichVu,
                            TenDichVu = d.TenDichVu,
                            Gia = d.Gia ?? 0m,
                            MoTa = d.MoTa ?? "",
                            SoSuKien = soSuKienMap.TryGetValue(d.MaDichVu, out int n) ? n : 0
                        }).ToList();

                        
                        UpdateStats(dsDichVu);
                        ApplyFilter();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                private void ApplyFilter()
                {
                    string kw = txtSearch?.Text?.Trim() ?? "";
                    var filtered = string.IsNullOrEmpty(kw)
                        ? _allRows
                        : _allRows.Where(r =>
                            r.TenDichVu.IndexOf(kw, System.StringComparison.OrdinalIgnoreCase) >= 0
                          ).ToList();
                    dgDichVu.ItemsSource = filtered;
                }

                private void UpdateStats(List<DichVu> list)
                {
                    txtTong.Text = _bll.TongSo(list).ToString();

                    if (list.Count == 0)
                    {
                        txtGiaMin.Text = "—";
                        txtGiaMax.Text = "—";
                        return;
                    }
                    txtGiaMin.Text = _bll.GiaMin(list).ToString("N0");
                    txtGiaMax.Text = _bll.GiaMax(list).ToString("N0");
                }

                private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
                    => ApplyFilter();

                private void btnAdd_Click(object sender, RoutedEventArgs e)
                {
                    var dlg = new DichVuDialog();
                    dlg.Owner = Window.GetWindow(this);
                    if (dlg.ShowDialog() != true) return;
                    LoadData();
                    MessageBox.Show("Thêm dịch vụ thành công!", "OK",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                private void btnEdit_Click(object sender, RoutedEventArgs e)
                {
                    if (!(dgDichVu.SelectedItem is DichVuRow sel))
                    {
                        MessageBox.Show("Vui lòng chọn dịch vụ cần sửa.", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var dv = new DichVu { MaDichVu = sel.MaDichVu, TenDichVu = sel.TenDichVu, Gia = sel.Gia , MoTa = sel.MoTa };
                    var dlg = new DichVuDialog(dv);
                    dlg.Owner = Window.GetWindow(this);
                    if (dlg.ShowDialog() != true) return;
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!", "OK",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                private void btnDelete_Click(object sender, RoutedEventArgs e)
                {
                    if (!(dgDichVu.SelectedItem is DichVuRow sel))
                    {
                        MessageBox.Show("Vui lòng chọn dịch vụ cần xóa.", "Cảnh báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (sel.SoSuKien > 0)
                    {
                        MessageBox.Show(
                            $"Không thể xóa! Dịch vụ \"{sel.TenDichVu}\" đang được dùng trong {sel.SoSuKien} sự kiện.",
                            "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (MessageBox.Show($"Xóa dịch vụ \"{sel.TenDichVu}\"?", "Xác nhận",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning)
                        != MessageBoxResult.Yes) return;

                    try
                    {
                        _bll.DeleteDichVu(sel.MaDichVu);
                        LoadData();
                        MessageBox.Show("Xóa thành công!", "OK",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }