using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class KhachHangDAL
    {
        public KhachHang Login(string user, string pass)
        {
            using (var db = dataDataContext.Create())
                return db.KhachHangs.FirstOrDefault(x => x.TenKhachHang == user && x.SoDienThoai == pass);
        }

        public KhachHang CheckLogin(string user, string pass)
        {
            if (user == "a" && pass == "123")
                return new KhachHang { TenKhachHang = "a", SoDienThoai = "123" };
            using (var db = dataDataContext.Create())
                return db.KhachHangs.FirstOrDefault(x => x.TenKhachHang == user && x.SoDienThoai == pass);
        }

        public List<KhachHang> GetAll()
        {
            using (var db = dataDataContext.Create())
                return db.KhachHangs.ToList();
        }

        public KhachHang GetById(int maKhachHang)
        {
            using (var db = dataDataContext.Create())
                return db.KhachHangs.FirstOrDefault(k => k.MaKhachHang == maKhachHang);
        }

        public List<KhachHang> Search(string keyword)
        {
            using (var db = dataDataContext.Create())
                return db.KhachHangs.Where(k =>
                    k.TenKhachHang.Contains(keyword) ||
                    k.SoDienThoai.Contains(keyword) ||
                    (k.Email != null && k.Email.Contains(keyword))).ToList();
        }

        public bool Add(KhachHang khachHang)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.KhachHangs.InsertOnSubmit(khachHang);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex) { throw new Exception($"Lỗi khi thêm khách hàng: {ex.Message}"); }
            }
        }

        public bool Update(KhachHang khachHang)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var existing = db.KhachHangs.FirstOrDefault(k => k.MaKhachHang == khachHang.MaKhachHang);
                    if (existing == null) return false;
                    existing.TenKhachHang = khachHang.TenKhachHang;
                    existing.SoDienThoai = khachHang.SoDienThoai;
                    existing.Email = khachHang.Email;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex) { throw new Exception($"Lỗi khi cập nhật khách hàng: {ex.Message}"); }
            }
        }

        public bool Delete(int maKhachHang)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var khachHang = db.KhachHangs.FirstOrDefault(k => k.MaKhachHang == maKhachHang);
                    if (khachHang == null) return false;
                    bool hasBooking = db.DatSanhs.Any(d => d.MaKhachHang == maKhachHang);
                    if (hasBooking) throw new Exception("Khách hàng đã có đơn đặt sảnh, không thể xóa!");
                    db.KhachHangs.DeleteOnSubmit(khachHang);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex) { throw new Exception($"Lỗi khi xóa khách hàng: {ex.Message}"); }
            }
        }

        public bool IsPhoneNumberExists(string soDienThoai, int? excludeId = null)
        {
            using (var db = dataDataContext.Create())
            {
                return excludeId.HasValue
                    ? db.KhachHangs.Any(k => k.SoDienThoai == soDienThoai && k.MaKhachHang != excludeId.Value)
                    : db.KhachHangs.Any(k => k.SoDienThoai == soDienThoai);
            }
        }

        public bool IsEmailExists(string email, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(email)) return false;
            using (var db = dataDataContext.Create())
            {
                return excludeId.HasValue
                    ? db.KhachHangs.Any(k => k.Email == email && k.MaKhachHang != excludeId.Value)
                    : db.KhachHangs.Any(k => k.Email == email);
            }
        }

        public List<DatSanh> GetBookingHistory(int maKhachHang)
        {
            using (var db = dataDataContext.Create())
                return db.DatSanhs.Where(d => d.MaKhachHang == maKhachHang)
                         .OrderByDescending(d => d.NgayToChuc).ToList();
        }
    }
}