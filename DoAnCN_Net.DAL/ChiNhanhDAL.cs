using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class ChiNhanhDAL
    {
        public List<ChiNhanh> GetAll()
        {
            using (var db = dataDataContext.Create())
            {
                return db.ChiNhanhs.ToList();
            }
        }

        public ChiNhanh GetById(int maChiNhanh)
        {
            using (var db = dataDataContext.Create())
            {
                return db.ChiNhanhs.FirstOrDefault(c => c.MaChiNhanh == maChiNhanh);
            }
        }

        public bool Add(ChiNhanh chiNhanh)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.ChiNhanhs.InsertOnSubmit(chiNhanh);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm: {ex.Message}");
                }
            }
        }

        public bool Update(ChiNhanh chiNhanh)
        {
            using (var db = dataDataContext.Create())
            {
                var existing = db.ChiNhanhs.FirstOrDefault(x => x.MaChiNhanh == chiNhanh.MaChiNhanh);
                if (existing == null)
                    throw new Exception("Không tìm thấy chi nhánh!");

                existing.TenChiNhanh = chiNhanh.TenChiNhanh;
                existing.DiaChi = chiNhanh.DiaChi;
                existing.SoDienThoai = chiNhanh.SoDienThoai;
                db.SubmitChanges();
                return true;
            }
        }

        public bool Delete(int id)
        {
            using (var db = dataDataContext.Create())
            {
                var chiNhanh = db.ChiNhanhs.FirstOrDefault(x => x.MaChiNhanh == id);
                if (chiNhanh == null)
                    throw new Exception("Không tìm thấy chi nhánh!");

                db.ChiNhanhs.DeleteOnSubmit(chiNhanh);
                db.SubmitChanges();
                return true;
            }
        }
    }
}