using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class DoiLichBLL
    {
        private readonly DoiLichDAL _dal = new DoiLichDAL();

        // ── Danh sách ─────────────────────────────────────────────────
        public List<DoiLichRow> GetAll(string trangThaiLoc = null)
            => _dal.GetAll(trangThaiLoc);

        public List<DoiLichRow> Filter(List<DoiLichRow> source, string keyword, string trangThai)
        {
            IEnumerable<DoiLichRow> q = source;

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(r =>
                    r.TenKhachHang.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.SoDienThoai.Contains(keyword) ||
                    r.TenSanh.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.MaDat.ToString().Contains(keyword));

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "Tất cả")
                q = q.Where(r => r.TrangThai == trangThai);

            return q.ToList();
        }

        // ── Chi tiết ──────────────────────────────────────────────────
        public DatSanh GetById(int maDat) => _dal.GetById(maDat);

        public List<Sanh> GetAllSanh() => _dal.GetAllSanh();
        public List<Menu> GetAllMenu() => _dal.GetAllMenu();

        // ── Đổi lịch ─────────────────────────────────────────────────
        public void DoiLich(int maDat, int maSanh, int maMenu,
                            DateTime ngayMoi, TimeSpan gioBDMoi, TimeSpan gioKTMoi,
                            string ghiChu)
        {
            // Validate
            if (ngayMoi.Date < DateTime.Today)
                throw new Exception("Ngày tổ chức mới không được trong quá khứ.");
            if (gioBDMoi >= gioKTMoi)
                throw new Exception("Giờ kết thúc phải sau giờ bắt đầu.");
            if (maSanh <= 0)
                throw new Exception("Vui lòng chọn sảnh.");
            if (maMenu <= 0)
                throw new Exception("Vui lòng chọn menu.");

            // Kiểm tra trùng lịch sảnh mới
            if (_dal.KiemTraTrungLich(maSanh, ngayMoi, gioBDMoi, gioKTMoi, boDaMaDat: maDat))
                throw new Exception("Sảnh đã có lịch trong khung giờ này. Vui lòng chọn thời gian khác.");

            bool ok = _dal.DoiLich(maDat, maSanh, maMenu, ngayMoi, gioBDMoi, gioKTMoi, ghiChu);
            if (!ok)
                throw new Exception("Không tìm thấy đơn đặt sảnh.");
        }
    }
}
