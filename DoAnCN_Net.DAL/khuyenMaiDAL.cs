using System;
using System.Collections.Generic;
using System.Linq;
namespace DoAnCN_Net.DAL
{
    public class khuyenMaiDAL
    {
        public List<KhuyenMai> GetAll()
        {
            using (var db = dataDataContext.Create())
            {
                return db.KhuyenMais.OrderByDescending(k => k.NgayBatDau).ToList();
            }
        }
        public KhuyenMai GetById(int maKhuyenMai)
        {
            using (var db = dataDataContext.Create())
            {
                return db.KhuyenMais.FirstOrDefault(k => k.MaKhuyenMai == maKhuyenMai);
            }
        }
        public bool TenDaTonTai(string ten, int? editingMa)
        {
            using (var db = dataDataContext.Create())
            {
                if (editingMa == null)
                {
                    // Thêm mới: kiểm tra tên có tồn tại chưa
                    return db.KhuyenMais.Any(k => k.TenKhuyenMai == ten);
                }
                else
                {
                    // Sửa: kiểm tra tên trùng với record khác
                    int ma = editingMa.Value;
                    return db.KhuyenMais.Any(k => k.TenKhuyenMai == ten && k.MaKhuyenMai != ma);
                }
            }
        }
        public bool DangDuocDung(int maKhuyenMai)
        {
            using (var db = dataDataContext.Create())
            {
                return db.KhuyenMaiDatSanhs.Any(k => k.MaKhuyenMai == maKhuyenMai);
            }
        }
        public bool Add(KhuyenMai km)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.KhuyenMais.InsertOnSubmit(km);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm: {ex.Message}");
                }
            }
        }
        public bool Update(KhuyenMai km)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var existing = db.KhuyenMais.FirstOrDefault(k => k.MaKhuyenMai == km.MaKhuyenMai);
                    if (existing == null) return false;
                    existing.TenKhuyenMai = km.TenKhuyenMai;
                    existing.PhanTramGiam = km.PhanTramGiam;
                    existing.SoTienGiam = km.SoTienGiam;
                    existing.NgayBatDau = km.NgayBatDau;
                    existing.NgayKetThuc = km.NgayKetThuc;
                    existing.TrangThai = km.TrangThai;
                    existing.MoTa = km.MoTa;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật: {ex.Message}");
                }
            }
        }
        public bool Delete(int maKhuyenMai)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var km = db.KhuyenMais.FirstOrDefault(k => k.MaKhuyenMai == maKhuyenMai);
                    if (km == null) return false;
                    db.KhuyenMais.DeleteOnSubmit(km);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa: {ex.Message}");
                }
            }
        }
    }
}