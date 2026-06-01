using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class DoiLichRow
    {
        public int      MaDat        { get; set; }
        public string   TenKhachHang { get; set; }
        public string   SoDienThoai  { get; set; }
        public string   TenSanh      { get; set; }
        public string   TenSuKien    { get; set; }
        public DateTime? NgayToChuc  { get; set; }
        public TimeSpan? GioBatDau   { get; set; }
        public TimeSpan? GioKetThuc  { get; set; }
        public string   TenMenu      { get; set; }
        public string   TrangThai    { get; set; }
    }

    public class DoiLichDAL
    {
        // ── Lấy danh sách đặt sảnh (có thể lọc theo trạng thái) ──────
        public List<DoiLichRow> GetAll(string trangThaiLoc = null)
        {
            using (var db = dataDataContext.Create())
            {
                var query = from ds in db.DatSanhs
                            join kh in db.KhachHangs  on ds.MaKhachHang equals kh.MaKhachHang
                            join s  in db.Sanhs        on ds.MaSanh      equals s.MaSanh
                            join m  in db.Menus        on ds.MaMenu      equals m.MaMenu into mj
                            from m in mj.DefaultIfEmpty()
                            orderby ds.NgayToChuc descending
                            select new DoiLichRow
                            {
                                MaDat        = ds.MaDat,
                                TenKhachHang = kh.TenKhachHang,
                                SoDienThoai  = kh.SoDienThoai,
                                TenSanh      = s.TenSanh,
                                TenSuKien    = ds.TenSuKien,
                                NgayToChuc   = ds.NgayToChuc,
                                GioBatDau    = ds.GioBatDau,
                                GioKetThuc   = ds.GioKetThuc,
                                TenMenu      = m != null ? m.TenMenu : "",
                                TrangThai    = ds.TrangThai
                            };

                if (!string.IsNullOrWhiteSpace(trangThaiLoc) && trangThaiLoc != "Tất cả")
                    query = query.Where(x => x.TrangThai == trangThaiLoc);

                return query.ToList();
            }
        }

        // ── Lấy chi tiết 1 đơn để load vào dialog ────────────────────
        public DatSanh GetById(int maDat)
        {
            using (var db = dataDataContext.Create())
                return db.DatSanhs.FirstOrDefault(d => d.MaDat == maDat);
        }

        // ── Kiểm tra trùng lịch (bỏ qua chính đơn đang sửa) ─────────
        public bool KiemTraTrungLich(int maSanh, DateTime ngay,
                                     TimeSpan gioBD, TimeSpan gioKT,
                                     int boDaMaDat)
        {
            using (var db = dataDataContext.Create())
                return db.DatSanhs.Any(d =>
                    d.MaSanh    == maSanh  &&
                    d.MaDat     != boDaMaDat &&
                    d.NgayToChuc == ngay   &&
                    d.TrangThai != "Đã hủy" &&
                    d.GioBatDau  < gioKT   &&
                    d.GioKetThuc > gioBD);
        }

        // ── Cập nhật lịch ─────────────────────────────────────────────
        public bool DoiLich(int maDat, int maSanh, int maMenu,
                            DateTime ngayMoi, TimeSpan gioBDMoi, TimeSpan gioKTMoi,
                            string ghiChu)
        {
            using (var db = dataDataContext.Create())
            {
                var ds = db.DatSanhs.FirstOrDefault(d => d.MaDat == maDat);
                if (ds == null) return false;

                ds.MaSanh      = maSanh;
                ds.MaMenu      = maMenu;
                ds.NgayToChuc  = ngayMoi;
                ds.GioBatDau   = gioBDMoi;
                ds.GioKetThuc  = gioKTMoi;
                // Ghi chú lưu vào TenSuKien nếu có (hoặc bỏ qua nếu không có cột GhiChu)
                if (!string.IsNullOrWhiteSpace(ghiChu))
                    ds.TenSuKien = ds.TenSuKien; // giữ nguyên tên sự kiện

                db.SubmitChanges();
                return true;
            }
        }

        // ── Lấy danh sách sảnh và menu cho combobox ──────────────────
        public List<Sanh>  GetAllSanh()  { using (var db = dataDataContext.Create()) return db.Sanhs.ToList(); }
        public List<Menu>  GetAllMenu()  { using (var db = dataDataContext.Create()) return db.Menus.ToList(); }
    }
}
