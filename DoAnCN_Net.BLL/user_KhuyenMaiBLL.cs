using System;
using System.Collections.Generic;
using System.Linq;
using DoAnCN_Net.DAL;

namespace DoAnCN_Net.BLL
{
    public class user_KhuyenMaiBLL
    {
        private readonly user_KhuyenMaiDAL _dal = new user_KhuyenMaiDAL();

        public List<KhuyenMaiViewModel> GetKhuyenMaiDangHoatDong()
        {
            return _dal.GetKhuyenMaiDangHoatDong()
                .Select(km => new KhuyenMaiViewModel
                {
                    MaKhuyenMai = km.MaKhuyenMai,
                    TenKhuyenMai = km.TenKhuyenMai,
                    NgayBatDau = km.NgayBatDau ?? DateTime.Today,   // ← fix nullable
                    NgayKetThuc = km.NgayKetThuc ?? DateTime.Today,   // ← fix nullable
                    TrangThai = km.TrangThai,
                    SoNgayConLai = km.NgayKetThuc.HasValue             // ← fix .Days
                                    ? (km.NgayKetThuc.Value - DateTime.Today).Days
                                    : 0,

                    BadgeText = km.PhanTramGiam.HasValue
                        ? $"🔥 GIẢM {km.PhanTramGiam}%"
                        : $"💰 GIẢM {km.SoTienGiam:N0} VNĐ",

                    GiaTriHienThi = km.PhanTramGiam.HasValue
                        ? $"-{km.PhanTramGiam}%"
                        : $"-{km.SoTienGiam:N0}đ",

                    MoTa = km.PhanTramGiam.HasValue
                        ? $"Giảm ngay {km.PhanTramGiam}% trên tổng hóa đơn"
                        : $"Giảm ngay {km.SoTienGiam:N0} VNĐ cho hóa đơn từ 30 triệu"
                })
                .OrderBy(km => km.SoNgayConLai)
                .ToList();
        }

        public List<KhuyenMaiViewModel> TimKiem(string keyword)
        {
            return GetKhuyenMaiDangHoatDong()
                .Where(km => string.IsNullOrWhiteSpace(keyword)
                          || km.TenKhuyenMai.ToLower().Contains(keyword.ToLower()))
                .ToList();
        }

        public bool KiemTraKhuyenMaiHopLe(int maKhuyenMai)
        {
            return maKhuyenMai > 0 && _dal.IsValidKhuyenMai(maKhuyenMai);
        }

        public decimal TinhTienSauGiam(decimal tongTien, int maKhuyenMai)
        {
            var km = _dal.GetKhuyenMaiById(maKhuyenMai);
            if (km == null) return tongTien;

            decimal tienGiam = km.PhanTramGiam.HasValue
                ? tongTien * km.PhanTramGiam.Value / 100
                : km.SoTienGiam ?? 0;

            return Math.Max(0, tongTien - tienGiam);
        }

        public KhuyenMaiViewModel GetKhuyenMaiTotNhat(decimal tongTien)
        {
            return GetKhuyenMaiDangHoatDong()
                .OrderByDescending(km =>
                {
                    var raw = _dal.GetKhuyenMaiById(km.MaKhuyenMai);
                    return raw?.PhanTramGiam.HasValue == true
                        ? tongTien * raw.PhanTramGiam.Value / 100
                        : raw?.SoTienGiam ?? 0;
                })
                .FirstOrDefault();
        }

        public Dictionary<string, int> ThongKeTheoLoai()
        {
            return _dal.GetKhuyenMaiDangHoatDong()
                .GroupBy(km => km.PhanTramGiam.HasValue ? "Giảm %" : "Giảm tiền mặt")
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }

    public class KhuyenMaiViewModel
    {
        public int MaKhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; }
        public string BadgeText { get; set; }
        public string GiaTriHienThi { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayBatDau { get; set; }  // non-nullable trong ViewModel
        public DateTime NgayKetThuc { get; set; }  // non-nullable trong ViewModel
        public string TrangThai { get; set; }
        public int SoNgayConLai { get; set; }
    }
}