using DoAnCN_Net.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class user_SanhBLL
    {
        private readonly user_SanhDAL dal = new user_SanhDAL();

        // ── Sảnh ─────────────────────────────────────────────────────
        public List<Sanh> GetAll() => dal.GetAll();

        public List<Sanh> Search(string tuKhoa, int maChiNhanh)
            => dal.Search(tuKhoa, maChiNhanh);

        // ── Chi nhánh ────────────────────────────────────────────────
        public List<ChiNhanh> GetAllChiNhanh() => dal.GetAllChiNhanh();

        public string GetDiaChiChiNhanh(int maChiNhanh)
            => dal.GetDiaChiChiNhanh(maChiNhanh);

        // ── Menu / Loại sự kiện ───────────────────────────────────────
        public List<Menu> GetAllMenu() => dal.GetAllMenu();
        public List<LoaiSuKien> GetAllLoaiSuKien() => dal.GetAllLoaiSuKien();

        // ── Dịch vụ / Khuyến mãi ─────────────────────────────────────
        public List<DichVu> GetAllDichVu() => dal.GetAllDichVu();
        public List<KhuyenMai> GetKhuyenMaiHoatDong() => dal.GetKhuyenMaiHoatDong();

        // ── Kiểm tra trùng lịch ───────────────────────────────────────
        public bool KiemTraTrungLich(int maSanh, DateTime ngay,
                                      TimeSpan gioBD, TimeSpan gioKT)
            => dal.KiemTraTrungLich(maSanh, ngay, gioBD, gioKT);

        // ── Đặt sảnh cơ bản (giữ nguyên) ────────────────────────────
        public void DatSanh(Sanh sanh, string tenKhach, string sdt,
                            string email, string tenSuKien, int maLoai,
                            int maMenu, DateTime ngay,
                            TimeSpan gioBD, TimeSpan gioKT,
                            int maKhachHang)
        {
            if (string.IsNullOrWhiteSpace(tenKhach))
                throw new Exception("Vui lòng nhập tên khách hàng!");
            if (string.IsNullOrWhiteSpace(sdt))
                throw new Exception("Vui lòng nhập số điện thoại!");
            if (string.IsNullOrWhiteSpace(tenSuKien))
                throw new Exception("Vui lòng nhập tên sự kiện!");
            if (maLoai <= 0)
                throw new Exception("Vui lòng chọn loại sự kiện!");
            if (maMenu <= 0)
                throw new Exception("Vui lòng chọn gói menu!");
            if (ngay < DateTime.Today)
                throw new Exception("Ngày tổ chức phải từ hôm nay trở đi!");
            if (gioBD >= gioKT)
                throw new Exception("Giờ kết thúc phải sau giờ bắt đầu!");
            if (maKhachHang <= 0)
                throw new Exception("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại!");
            if (dal.KiemTraTrungLich(sanh.MaSanh, ngay, gioBD, gioKT))
                throw new Exception("Sảnh đã có lịch trong khung giờ này!");

            dal.DatSanh(new DatSanh
            {
                MaSanh = sanh.MaSanh,
                MaKhachHang = maKhachHang,
                MaLoai = maLoai,
                MaMenu = maMenu,
                TenSuKien = tenSuKien,
                NgayToChuc = ngay,
                GioBatDau = gioBD,
                GioKetThuc = gioKT,
                TrangThai = "Đang đặt"
            });
        }

        // ── Đặt sảnh đầy đủ (bàn + dịch vụ + khuyến mãi) ───────────
        // dsDichVuChon truyền từ UI — BLL chỉ lấy MaDichVu (List<int>)
        public void DatSanhDayDu(
            Sanh sanh,
            string tenKhach, string sdt, string email,
            string tenSuKien, int maLoai, int maMenu,
            DateTime ngay, TimeSpan gioBD, TimeSpan gioKT,
            int maKhachHang,
            int soBan, int khachMoiBan,
            List<DichVuChon> dsDichVuChon,
            int? maKhuyenMai)
        {
            // Validate bàn
            if (soBan <= 0)
                throw new Exception("Số bàn phải lớn hơn 0!");
            if (khachMoiBan <= 0)
                throw new Exception("Số khách/bàn phải lớn hơn 0!");
            if (soBan * khachMoiBan > sanh.SucChua)
                throw new Exception(
                    $"Tổng khách ({soBan * khachMoiBan}) vượt sức chứa sảnh ({sanh.SucChua})!");

            // Gọi DatSanh gốc
            DatSanh(sanh, tenKhach, sdt, email, tenSuKien,
                    maLoai, maMenu, ngay, gioBD, gioKT, maKhachHang);

            // Lấy MaDat vừa tạo
            int maDat = dal.LayMaDatMoiNhat(sanh.MaSanh);
            if (maDat <= 0)
                throw new Exception("Không lấy được mã đặt sảnh!");

            // Lưu bàn
            dal.LuuDanhSachBan(maDat, soBan, khachMoiBan);

            // Convert DichVuChon → List<int> rồi mới truyền xuống DAL
            var dsMaDv = dsDichVuChon?
                .Where(d => d.DaChon)
                .Select(d => d.MaDichVu)
                .ToList();
            if (dsMaDv != null && dsMaDv.Count > 0)
                dal.LuuDichVuSuKien(maDat, dsMaDv);

            // Lưu khuyến mãi
            if (maKhuyenMai.HasValue && maKhuyenMai.Value > 0)
                dal.LuuKhuyenMai(maDat, maKhuyenMai.Value);
        }

        //public IEnumerable GetDichVuBySanh(int maSanh)
        //{
        //    throw new NotImplementedException();
        //}
        public IEnumerable GetDichVuBySanh(int maSanh)
        {
            return dal.GetDichVuBySanh(maSanh);
        }
    }
}