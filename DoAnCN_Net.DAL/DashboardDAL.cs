using System.Collections.Generic;
using System.Linq;
using System;
using DoAnCN_Net;

namespace DoAnCN_Net.DAL
{
    public class DashboardRow
    {
        public string TenSuKien { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime? NgayToChuc { get; set; }
        public string TrangThai { get; set; }
    }

    // ── Model cho biểu đồ doanh thu theo tháng ──────────────────────
    public class DoanhThuChartRow
    {
        public string NhanThang { get; set; }   // "T1", "T2", ...
        public decimal TongTien { get; set; }
    }

    // ── Model cho biểu đồ trạng thái đơn ────────────────────────────
    public class TrangThaiChartRow
    {
        public string TrangThai { get; set; }
        public int SoLuong { get; set; }
    }

    public class DashboardDAL
    {
        // ── Bảng gần nhất ────────────────────────────────────────────
        public List<DashboardRow> GetTopDatSanh()
        {
            var db = dataDataContext.Create();
            return (from ds in db.DatSanhs
                    join kh in db.KhachHangs on ds.MaKhachHang equals kh.MaKhachHang
                    orderby ds.MaDat descending
                    select new DashboardRow
                    {
                        TenSuKien = ds.TenSuKien,
                        TenKhachHang = kh.TenKhachHang,
                        NgayToChuc = ds.NgayToChuc,
                        TrangThai = ds.TrangThai
                    })
                    .Take(10)
                    .ToList();
        }

        // ── Thẻ tóm tắt ──────────────────────────────────────────────
        public int GetTotalOrders()
        {
            var db = dataDataContext.Create();
            return db.DatSanhs.Count();
        }

        /// Tổng doanh thu thực từ bảng HoaDon
        public decimal GetTotalRevenue()
        {
            var db = dataDataContext.Create();
            return db.HoaDons
                     .Where(h => h.TongTien.HasValue)
                     .Sum(h => h.TongTien) ?? 0m;
        }

        public int GetTodayParties()
        {
            var db = dataDataContext.Create();
            var today = DateTime.Today;
            return db.DatSanhs.Count(x => x.NgayToChuc != null
                                       && x.NgayToChuc.Value.Date == today);
        }

        public int GetPendingOrders()
        {
            var db = dataDataContext.Create();
            return db.DatSanhs.Count(x => x.TrangThai == "Đang đặt");
        }

        // ── Doanh thu theo tháng (năm hiện tại) ──────────────────────
        public List<DoanhThuChartRow> GetDoanhThuTheoThang(int nam)
        {
            var db = dataDataContext.Create();

            // Lấy dữ liệu thô từ DB
            var raw = db.HoaDons
                        .Where(h => h.NgayLap.HasValue
                                 && h.NgayLap.Value.Year == nam
                                 && h.TongTien.HasValue)
                        .GroupBy(h => h.NgayLap.Value.Month)
                        .Select(g => new { Thang = g.Key, Tong = g.Sum(h => h.TongTien) ?? 0m })
                        .ToList();

            // Đảm bảo đủ 12 tháng (tháng không có hóa đơn = 0)
            return Enumerable.Range(1, 12)
                .Select(t => new DoanhThuChartRow
                {
                    NhanThang = $"T{t}",
                    TongTien = raw.FirstOrDefault(r => r.Thang == t)?.Tong ?? 0m
                })
                .ToList();
        }

        // ── Thống kê trạng thái đơn ───────────────────────────────────
        public List<TrangThaiChartRow> GetThongKeTrangThai()
        {
            var db = dataDataContext.Create();
            return db.DatSanhs
                     .GroupBy(d => d.TrangThai)
                     .Select(g => new TrangThaiChartRow
                     {
                         TrangThai = g.Key ?? "Không xác định",
                         SoLuong = g.Count()
                     })
                     .OrderByDescending(x => x.SoLuong)
                     .ToList();
        }
    }
}