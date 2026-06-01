using DoAnCN_Net.BLL;
using DoAnCN_Net.DAL;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DoAnCN_Net
{
    public partial class DashboardView : UserControl
    {
        // Binding properties cho XAML
        public SeriesCollection ColumnSeries { get; set; }
        public SeriesCollection PieSeries { get; set; }
        public ObservableCollection<string> Labels { get; set; }

        private readonly DashboardBLL _bus = new DashboardBLL();

        // Bảng màu cho trạng thái đơn
        private static readonly Brush[] _pieBrushes =
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")), // xanh lá
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")), // xanh dương
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")), // cam
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")), // đỏ
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9C27B0")), // tím
        };

        public DashboardView()
        {
            InitializeComponent();

            LoadCards();
            LoadColumnChart();
            LoadPieChart();
            LoadTable();

            DataContext = this;
        }

        // ── Thẻ tóm tắt ──────────────────────────────────────────────
        private void LoadCards()
        {
            try
            {
                txtTongDon.Text = _bus.GetTotalOrders().ToString("N0");
                txtDoanhThu.Text = _bus.GetTotalRevenue().ToString("N0") + " đ";
                txtTiecHomNay.Text = _bus.GetTodayParties().ToString("N0");
                txtChoDuyet.Text = _bus.GetPendingOrders().ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thẻ thống kê: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Biểu đồ cột: Doanh thu theo tháng (năm hiện tại) ─────────
        private void LoadColumnChart()
        {
            try
            {
                int nam = DateTime.Today.Year;
                var data = _bus.GetDoanhThuTheoThang(nam);

                // Đổi đơn vị sang triệu đồng để hiển thị gọn hơn
                var values = new ChartValues<decimal>(
                    data.Select(d => Math.Round(d.TongTien / 1_000_000m, 1)));

                ColumnSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title  = $"Doanh thu {nam} (triệu đ)",
                        Values = values,
                        Fill   = new SolidColorBrush(
                                     (Color)ColorConverter.ConvertFromString("#2196F3"))
                    }
                };

                Labels = new ObservableCollection<string>(
                    data.Select(d => d.NhanThang));
            }
            catch (Exception ex)
            {
                // Fallback: hiển thị biểu đồ trống thay vì crash
                ColumnSeries = new SeriesCollection
                {
                    new ColumnSeries { Title = "Doanh thu", Values = new ChartValues<decimal>() }
                };
                Labels = new ObservableCollection<string>();

                MessageBox.Show("Lỗi tải biểu đồ doanh thu: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Biểu đồ tròn: Trạng thái đơn ────────────────────────────
        private void LoadPieChart()
        {
            try
            {
                var data = _bus.GetThongKeTrangThai();

                PieSeries = new SeriesCollection();

                for (int i = 0; i < data.Count; i++)
                {
                    var row = data[i];
                    var brush = _pieBrushes[i % _pieBrushes.Length];

                    PieSeries.Add(new PieSeries
                    {
                        Title = row.TrangThai,
                        Values = new ChartValues<int> { row.SoLuong },
                        Fill = brush,
                        DataLabels = true
                    });
                }

                // Nếu chưa có dữ liệu, hiển thị placeholder
                if (PieSeries.Count == 0)
                {
                    PieSeries.Add(new PieSeries
                    {
                        Title = "Chưa có đơn",
                        Values = new ChartValues<int> { 1 },
                        Fill = Brushes.LightGray
                    });
                }
            }
            catch (Exception ex)
            {
                PieSeries = new SeriesCollection
                {
                    new PieSeries { Title = "Lỗi tải dữ liệu",
                                    Values = new ChartValues<int> { 1 },
                                    Fill = Brushes.LightGray }
                };

                MessageBox.Show("Lỗi tải biểu đồ trạng thái: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ── Bảng tiệc gần nhất ───────────────────────────────────────
        private void LoadTable()
        {
            try
            {
                var data = _bus.GetDashboardData();
                dgData.ItemsSource = data.Select(x => new
                {
                    TenSuKien = x.TenSuKien ?? "Chưa có tên",
                    TenKhachHang = x.TenKhachHang ?? "Không có",
                    NgayToChuc = x.NgayToChuc.HasValue
                                   ? x.NgayToChuc.Value.ToString("dd/MM/yyyy")
                                   : "Chưa có",
                    TrangThai = x.TrangThai ?? "Không xác định"
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng dữ liệu: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}