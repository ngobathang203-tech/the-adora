using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class monAnDAL
    {
        public List<MonAn> GetAll()
        {
            using (var db = dataDataContext.Create())
            {
                return db.MonAns.ToList();
            }
        }

        public MonAn GetById(int maMon)
        {
            using (var db = dataDataContext.Create())
            {
                return db.MonAns.FirstOrDefault(m => m.MaMon == maMon);
            }
        }

        public bool Add(MonAn mon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.MonAns.InsertOnSubmit(mon);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm: {ex.Message}");
                }
            }
        }

        public bool Update(MonAn mon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var existing = db.MonAns.FirstOrDefault(m => m.MaMon == mon.MaMon);
                    if (existing == null) return false;
                    existing.TenMon = mon.TenMon;
                    existing.Gia = mon.Gia;
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật: {ex.Message}");
                }
            }
        }

        public bool Delete(int maMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var mon = db.MonAns.FirstOrDefault(m => m.MaMon == maMon);
                    if (mon == null) return false;
                    db.MonAns.DeleteOnSubmit(mon);
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