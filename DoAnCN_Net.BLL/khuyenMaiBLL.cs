using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class khuyenMaiBLL
    {
        private readonly khuyenMaiDAL _dal = new khuyenMaiDAL();

        public List<KhuyenMai> GetAll() => _dal.GetAll();
        public KhuyenMai GetById(int ma) => _dal.GetById(ma);

        public List<KhuyenMai> Filter(List<KhuyenMai> source, string keyword, string trangThai)
        {
            IEnumerable<KhuyenMai> q = source;

            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(k =>
                    k.TenKhuyenMai.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (trangThai != "Tất cả")
                q = q.Where(k => k.TrangThai == trangThai);

            return q.ToList();
        }

        public void AddKhuyenMai(string ten, decimal? phanTram, decimal? soTien,
                                  DateTime? ngayBD, DateTime? ngayKT,
                                  string trangThai, string moTa)
        {
            Validate(ten, phanTram, soTien, ngayBD, ngayKT, editingMa: null);
            _dal.Add(new KhuyenMai
            {
                TenKhuyenMai = ten.Trim(),
                PhanTramGiam = phanTram,
                SoTienGiam = soTien,
                NgayBatDau = ngayBD,
                NgayKetThuc = ngayKT,
                TrangThai = trangThai,
                MoTa = moTa?.Trim()
            });
        }

        public void UpdateKhuyenMai(int ma, string ten, decimal? phanTram, decimal? soTien,
                                     DateTime? ngayBD, DateTime? ngayKT,
                                     string trangThai, string moTa)
        {
            Validate(ten, phanTram, soTien, ngayBD, ngayKT, editingMa: ma);
            _dal.Update(new KhuyenMai
            {
                MaKhuyenMai = ma,
                TenKhuyenMai = ten.Trim(),
                PhanTramGiam = phanTram,
                SoTienGiam = soTien,
                NgayBatDau = ngayBD,
                NgayKetThuc = ngayKT,
                TrangThai = trangThai,
                MoTa = moTa?.Trim()
            });
        }

        public void DeleteKhuyenMai(int ma)
        {
            if (_dal.DangDuocDung(ma))
                throw new Exception("Không thể xóa! Khuyến mãi đang được áp dụng cho tiệc.");
            _dal.Delete(ma);
        }

        // Stats
        public int TongSo(List<KhuyenMai> list) => list.Count;
        public int SoHoatDong(List<KhuyenMai> list) => list.Count(k => k.TrangThai == "Hoạt động");
        public int SoHetHan(List<KhuyenMai> list) => list.Count(k =>
            k.NgayKetThuc.HasValue && k.NgayKetThuc.Value.Date < DateTime.Today);

        private void Validate(string ten, decimal? phanTram, decimal? soTien,
                               DateTime? ngayBD, DateTime? ngayKT, int? editingMa)
        {
            if (string.IsNullOrWhiteSpace(ten))
                throw new Exception("Tên khuyến mãi không được để trống.");
            if (ten.Trim().Length < 3)
                throw new Exception("Tên khuyến mãi phải có ít nhất 3 ký tự.");
            if (phanTram == null && soTien == null)
                throw new Exception("Phải nhập % giảm hoặc số tiền giảm.");
            if (phanTram != null && soTien != null)
                throw new Exception("Chỉ được nhập một trong hai: % giảm hoặc số tiền giảm.");
            if (phanTram.HasValue && (phanTram < 0 || phanTram > 100))
                throw new Exception("% giảm phải từ 0 đến 100.");
            if (soTien.HasValue && soTien < 0)
                throw new Exception("Số tiền giảm không được âm.");
            if (ngayBD.HasValue && ngayKT.HasValue && ngayBD > ngayKT)
                throw new Exception("Ngày bắt đầu phải trước ngày kết thúc.");
            if (_dal.TenDaTonTai(ten.Trim(), editingMa))
                throw new Exception($"Tên \"{ten.Trim()}\" đã tồn tại.");
        }
    }
}