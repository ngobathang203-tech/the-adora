using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class HoaDonDAL
    {
        public List<HoaDon> GetAll()
        {
            using (var db = dataDataContext.Create())
                return db.HoaDons.OrderByDescending(h => h.NgayLap).ToList();
        }

        public HoaDon GetById(int maHoaDon)
        {
            using (var db = dataDataContext.Create())
                return db.HoaDons.FirstOrDefault(h => h.MaHoaDon == maHoaDon);
        }

        public HoaDon GetByMaDat(int maDat)
        {
            using (var db = dataDataContext.Create())
                return db.HoaDons.FirstOrDefault(h => h.MaDat == maDat);
        }

        public bool DaCoHoaDon(int maDat)
        {
            using (var db = dataDataContext.Create())
                return db.HoaDons.Any(h => h.MaDat == maDat);
        }

        public bool Add(HoaDon hd)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.HoaDons.InsertOnSubmit(hd);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm hóa đơn: {ex.Message}");
                }
            }
        }

        public bool Update(HoaDon hd)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var existing = db.HoaDons.FirstOrDefault(h => h.MaHoaDon == hd.MaHoaDon);
                    if (existing == null) return false;
                    existing.TongTien = hd.TongTien;
                    existing.NgayLap = hd.NgayLap;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật hóa đơn: {ex.Message}");
                }
            }
        }

        public bool Delete(int maHoaDon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var hd = db.HoaDons.FirstOrDefault(h => h.MaHoaDon == maHoaDon);
                    if (hd == null) return false;
                    db.HoaDons.DeleteOnSubmit(hd);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa hóa đơn: {ex.Message}");
                }
            }
        }

        // Thống kê doanh thu theo tháng
        public List<DoanhThuThang> GetDoanhThuTheoThang(int nam)
        {
            using (var db = dataDataContext.Create())
            {
                return db.HoaDons
                    .Where(h => h.NgayLap.HasValue && h.NgayLap.Value.Year == nam)
                    .GroupBy(h => h.NgayLap.Value.Month)
                    .Select(g => new DoanhThuThang
                    {
                        Thang = g.Key,
                        DoanhThu = g.Sum(h => h.TongTien ?? 0),
                        SoHoaDon = g.Count()
                    })
                    .OrderBy(x => x.Thang)
                    .ToList();
            }
        }
    }

    public class DoanhThuThang
    {
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoHoaDon { get; set; }
        public string TenThang => $"Tháng {Thang}";
    }
}